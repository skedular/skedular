using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Core.V1;
using Api.Shared.Services.Models;
using Booking.Shared.Configurations;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Email;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Google.Protobuf;
using QuestPDF.Fluent;
using Temporalio.Activities;
using Constants = Booking.Shared.GraphQL.Constants;
using Organization = Booking.Shared.Database.Entities.Organization;
using OrganizationArrearsInvoice = Booking.Shared.Database.Entities.OrganizationArrearsInvoice;
using OrganizationArrearsInvoiceLine = Booking.Shared.Database.Entities.OrganizationArrearsInvoiceLine;

namespace Booking.Shared.Activities;

public record GenerateOrganizationArrearsInvoicesInput(
    string OrganizationId,
    BillingPeriod BillingPeriod,
    OrganizationBillingCycle BillingCycle);

public record GetOrganizationArrearsBillingNextRunAtInput(OrganizationArrearsBillingConfiguration Configuration);

public record GetOrganizationArrearsBillingPeriodInput(
    DateTimeOffset ScheduledRunAt,
    bool RunNowRequested,
    OrganizationArrearsBillingConfiguration Configuration);

public class OrganizationArrearsBillingIntegrations(
    EmailConfiguration emailConfiguration,
    CoreConfiguration coreConfiguration,
    CoreService.CoreServiceClient coreServiceClient,
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IOrganizationArrearsBillingPlannerService organizationArrearsBillingPlannerService,
    IOrganizationArrearsInvoiceService organizationArrearsInvoiceService,
    IDbTransactionBuilder transactionBuilder,
    IOrganizationInvoiceCounterService organizationInvoiceCounterService,
    IEmailService emailService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    IRandomHelper randomHelper,
    TimeProvider timeProvider)
{
    [Activity]
    public Task<DateTimeOffset> GetNextRunAtAsync(GetOrganizationArrearsBillingNextRunAtInput args)
    {
        var now = timeProvider.GetUtcNow();

        return Task.FromResult(args.Configuration.BillingCycle switch
        {
            OrganizationBillingCycle.Weekly => GetNextWeeklyBoundary(now),
            OrganizationBillingCycle.Fortnightly => GetNextFortnightlyBoundary(now),
            OrganizationBillingCycle.Monthly => GetNextMonthlyBoundary(now),
            _ => throw new ArgumentOutOfRangeException(nameof(args.Configuration.BillingCycle))
        });
    }

    [Activity]
    public Task<BillingPeriod> GetBillingPeriodForRunAtAsync(GetOrganizationArrearsBillingPeriodInput args)
    {
        var effectiveRunAt = args.RunNowRequested ? timeProvider.GetUtcNow() : args.ScheduledRunAt;

        if (args.RunNowRequested)
        {
            return Task.FromResult(args.Configuration.BillingCycle switch
            {
                OrganizationBillingCycle.Weekly => new BillingPeriod(GetCurrentWeekStart(effectiveRunAt), effectiveRunAt),
                OrganizationBillingCycle.Fortnightly => new BillingPeriod(GetCurrentFortnightStart(effectiveRunAt), effectiveRunAt),
                OrganizationBillingCycle.Monthly => new BillingPeriod(GetCurrentMonthStart(effectiveRunAt), effectiveRunAt),
                _ => throw new ArgumentOutOfRangeException(nameof(args.Configuration.BillingCycle))
            });
        }

        return Task.FromResult(args.Configuration.BillingCycle switch
        {
            OrganizationBillingCycle.Weekly => new BillingPeriod(effectiveRunAt.AddDays(-7), effectiveRunAt),
            OrganizationBillingCycle.Fortnightly => new BillingPeriod(effectiveRunAt.AddDays(-14), effectiveRunAt),
            OrganizationBillingCycle.Monthly => new BillingPeriod(effectiveRunAt.AddMonths(-1), effectiveRunAt),
            _ => throw new ArgumentOutOfRangeException(nameof(args.Configuration.BillingCycle))
        });
    }

    [Activity]
    public async Task GenerateOrganizationArrearsInvoicesAsync(
        GenerateOrganizationArrearsInvoicesInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var bookings = await repositoryFactory.BookingRepository.GetInArrearsByOrganizationBeforeAsync(
            args.OrganizationId,
            args.BillingPeriod.StartInclusive,
            args.BillingPeriod.EndExclusive,
            cancellationToken);
        if (bookings.Count == 0)
        {
            return;
        }

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               args.OrganizationId,
                               null,
                               false,
                               false,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        var persistedProcessedSegmentKeys = await repositoryFactory.OrganizationArrearsInvoiceRepository.GetProcessedSegmentKeysAsync(
            args.OrganizationId,
            args.BillingPeriod.StartInclusive,
            args.BillingPeriod.EndExclusive,
            cancellationToken);

        var bookingModels = bookings.Select(mapper.MapTo).ToList();
        var drafts = organizationArrearsBillingPlannerService.BuildInvoiceDrafts(
            args.BillingPeriod,
            args.BillingCycle,
            bookingModels,
            persistedProcessedSegmentKeys);
        if (drafts.Count == 0)
        {
            return;
        }

        foreach (var draft in drafts)
        {
            // Invoice generation is grouped per customer so one billing-cycle email can cover all
            // arrears bookings earned in the billing period for that customer.
            var draftBookingIds = draft.Lines.Select(line => line.BookingId).ToHashSet();
            var draftBookingModels = bookingModels
                .Where(booking => draftBookingIds.Contains(booking.Id))
                .ToList();
            var recipients = draftBookingModels
                .SelectMany(booking => booking.MarketplaceBooking?.InvoiceEmailList ?? [])
                .Where(email => !string.IsNullOrWhiteSpace(email))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var invoiceNumber = await organizationInvoiceCounterService.GetNextInvoiceNumberIdAsync(args.OrganizationId, cancellationToken);
            var invoiceDocument = organizationArrearsInvoiceService.GenerateInvoice(organization, draft, invoiceNumber);

            await using var pdfStream = new MemoryStream();
            invoiceDocument.GeneratePdf(pdfStream);
            pdfStream.Seek(0, SeekOrigin.Begin);

            var invoiceUrl = await UploadInvoicePdfAsync(pdfStream, cancellationToken);
            await PersistArrearsInvoiceAndAttachToBookingsAsync(
                draft,
                draft.Lines.Select(line => line.BookingId).Distinct().ToList(),
                invoiceNumber,
                invoiceUrl,
                cancellationToken);

            if (recipients.Count != 0)
            {
                await SendInvoiceEmailAsync(recipients, organization, draft, invoiceNumber, pdfStream, cancellationToken);
            }

            foreach (var line in draft.Lines)
            {
                await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, line.BookingId, cancellationToken);
            }
        }
    }

    private static DateTimeOffset GetNextWeeklyBoundary(DateTimeOffset now)
    {
        var nextBoundary = GetCurrentWeekStart(now);
        if (nextBoundary <= now)
        {
            nextBoundary = nextBoundary.AddDays(7);
        }

        return nextBoundary;
    }

    private static DateTimeOffset GetNextFortnightlyBoundary(DateTimeOffset now)
    {
        var nextBoundary = GetCurrentFortnightStart(now);
        if (nextBoundary <= now)
        {
            nextBoundary = nextBoundary.AddDays(14);
        }

        return nextBoundary;
    }

    private static DateTimeOffset GetNextMonthlyBoundary(DateTimeOffset now)
    {
        var currentMonthStart = GetCurrentMonthStart(now);
        return currentMonthStart <= now ? currentMonthStart.AddMonths(1) : currentMonthStart;
    }

    private static DateTimeOffset GetCurrentMonthStart(DateTimeOffset value) =>
        new(value.Year, value.Month, 1, 0, 0, 0, value.Offset);

    private static DateTimeOffset GetCurrentWeekStart(DateTimeOffset value)
    {
        var daysSinceMonday = ((int)value.DayOfWeek + 6) % 7;
        return new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset).AddDays(-daysSinceMonday);
    }

    private static DateTimeOffset GetCurrentFortnightStart(DateTimeOffset value)
    {
        var weekStart = GetCurrentWeekStart(value);
        // Fortnightly billing uses alternating Mondays from a fixed system baseline so every
        // organization shares the same billing weeks.
        var baseMonday = new DateTimeOffset(1970, 1, 5, 0, 0, 0, value.Offset);
        var weeksSinceBase = (int)((weekStart - baseMonday).TotalDays / 7);
        return weeksSinceBase % 2 == 0 ? weekStart : weekStart.AddDays(-7);
    }

    private async Task PersistArrearsInvoiceAndAttachToBookingsAsync(
        ArrearsInvoiceDraft draft,
        List<string> bookingIds,
        string invoiceNumber,
        string invoiceUrl,
        CancellationToken cancellationToken)
    {
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(draft.CustomerId, true, cancellationToken) ??
                       throw new CustomerNotFound();
        var lineBookings = await repositoryFactory.BookingRepository.GetByIdsMinimalAsync(
            draft.Lines.Select(item => item.BookingId).ToList(),
            cancellationToken);

        repositoryFactory.OrganizationArrearsInvoiceRepository.Add(
            new OrganizationArrearsInvoice
            {
                Id = randomHelper.Generate(),
                OrganizationId = draft.OrganizationId,
                Customer = customer,
                InvoiceNumber = invoiceNumber,
                InvoiceUrl = invoiceUrl,
                BillingPeriodStartInclusive = draft.BillingPeriod.StartInclusive,
                BillingPeriodEndExclusive = draft.BillingPeriod.EndExclusive,
                Currency = draft.Currency.ToCurrency(),
                TotalAmount = draft.TotalAmount,
                Lines = draft.Lines.Select(line => new OrganizationArrearsInvoiceLine
                {
                    Id = randomHelper.Generate(),
                    Booking = lineBookings.First(booking => booking.Id == line.BookingId),
                    SegmentKey = line.SegmentKey,
                    ServicePeriodStartInclusive = line.ServicePeriod.StartInclusive,
                    ServicePeriodEndExclusive = line.ServicePeriod.EndExclusive,
                    EarnedAt = line.EarnedAt,
                    Amount = line.Amount,
                    Description = line.Description
                }).ToList()
            });

        if (bookingIds.Count == 0)
        {
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return;
        }

        var bookings = await repositoryFactory.BookingRepository.GetByIdsWithValidMarketplaceAsync(bookingIds, cancellationToken);

        foreach (var booking in bookings)
        {
            // Keep the first invoice reference on the booking for backward compatibility.
            if (string.IsNullOrWhiteSpace(booking.MarketplaceBooking!.InvoiceNumber))
            {
                booking.MarketplaceBooking.InvoiceNumber = invoiceNumber;
            }

            if (string.IsNullOrWhiteSpace(booking.MarketplaceBooking.InvoiceUrl))
            {
                booking.MarketplaceBooking.InvoiceUrl = invoiceUrl;
            }

            repositoryFactory.MarketplaceBookingRepository.Update(booking.MarketplaceBooking);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task<string> UploadInvoicePdfAsync(MemoryStream pdfStream, CancellationToken cancellationToken)
    {
        pdfStream.Seek(0, SeekOrigin.Begin);

        using var call =
            coreServiceClient.Admin_UploadToPrivateStorage(coreConfiguration.ApiKey.CreateMetadata(), cancellationToken: cancellationToken);

        ArgumentNullException.ThrowIfNull(call);

        int bytesRead;
        var buffer = new byte[64 * 1024];
        while ((bytesRead = await pdfStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
        {
            await call.RequestStream.WriteAsync(
                new UploadFileRequest { Extension = ".pdf", ContentType = "application/pdf", Chunk = ByteString.CopyFrom(buffer, 0, bytesRead) },
                cancellationToken);
        }

        await call.RequestStream.CompleteAsync();
        var fileUploadResponse = await call.ResponseAsync;

        return fileUploadResponse.Original.Url;
    }

    private async Task SendInvoiceEmailAsync(
        List<string> recipients,
        Organization organization,
        ArrearsInvoiceDraft draft,
        string invoiceNumber,
        MemoryStream pdfStream,
        CancellationToken cancellationToken)
    {
        var organizationName = organization.Name ?? "Organization";

        await using var htmlTemplateStream = typeof(OrganizationArrearsBillingIntegrations).Assembly.GetManifestResourceStream(
            "Booking.Shared.EmailTemplates.OrganizationArrearsInvoice.template.html");
        ArgumentNullException.ThrowIfNull(htmlTemplateStream);
        using var htmlReader = new StreamReader(htmlTemplateStream);
        var bodyHtml = await htmlReader.ReadToEndAsync(cancellationToken);

        await using var textTemplateStream = typeof(OrganizationArrearsBillingIntegrations).Assembly.GetManifestResourceStream(
            "Booking.Shared.EmailTemplates.OrganizationArrearsInvoice.template.txt");
        ArgumentNullException.ThrowIfNull(textTemplateStream);
        using var textReader = new StreamReader(textTemplateStream);
        var bodyText = await textReader.ReadToEndAsync(cancellationToken);

        bodyHtml = bodyHtml
            .Replace("{{COMPANY_NAME}}", organizationName)
            .Replace("{{INVOICE_NUMBER}}", invoiceNumber)
            .Replace("{{BILLING_PERIOD_START}}", draft.BillingPeriod.StartInclusive.ToString("yyyy-MM-dd"))
            .Replace("{{BILLING_PERIOD_END}}", draft.BillingPeriod.EndExclusive.ToString("yyyy-MM-dd"));

        bodyText = bodyText
            .Replace("{{COMPANY_NAME}}", organizationName)
            .Replace("{{INVOICE_NUMBER}}", invoiceNumber)
            .Replace("{{BILLING_PERIOD_START}}", draft.BillingPeriod.StartInclusive.ToString("yyyy-MM-dd"))
            .Replace("{{BILLING_PERIOD_END}}", draft.BillingPeriod.EndExclusive.ToString("yyyy-MM-dd"));

        var attachments = new List<EmailAttachment> { new(pdfStream, $"{invoiceNumber}.pdf", "application/pdf") };

        await emailService.SendRawEmailAsync(
            $"Invoice #{invoiceNumber} from {organizationName}",
            bodyText,
            bodyHtml,
            $"{organizationName} {emailConfiguration.BookingInvoiceEmailSender}",
            recipients,
            [],
            [],
            attachments,
            cancellationToken);
    }
}

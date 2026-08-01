using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Configurations;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Email;
using Customer = Booking.Shared.Database.Entities.Customer;
using Identity = Booking.Shared.Database.Entities.Identity;
using Organization = Booking.Shared.Database.Entities.Organization;
using OrganizationMember = Booking.Shared.Database.Entities.OrganizationMember;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundNotificationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class NotifyStatusChangedAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Skip_Send_When_Durable_Delivery_Is_Already_Sent(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IEmailService emailService,
        MarketplaceRefundNotificationService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            Status = MarketplaceRefundStatusConstants.Completed,
            RefundAmount = 25m,
            Currency = "NZD"
        };
        var organization = new Organization { Id = "org-1", Name = "Acme Coworking", ContactEmail = "ops@acme.test" };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken))
            .Returns(organization);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository.GetNotificationDeliveryAsync(
                refund.Id, refund.Status, "ops@acme.test", cancellationToken))
            .Returns(new MarketplaceRefundNotificationDelivery { Status = "Sent", RecipientId = "ops@acme.test" });

        await sut.NotifyStatusChangedAsync(refund, cancellationToken);

        A.CallTo(() => emailService.SendRawEmailAsync(
                A<string>._, A<string>._, A<string>._, A<string>._,
                A<IReadOnlyList<string>>._, A<IReadOnlyList<string>>._, A<IReadOnlyList<string>>._,
                A<IReadOnlyList<EmailAttachment>>._, cancellationToken))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Send_Customer_And_Internal_Emails_When_Recipients_Are_Available(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] IEmailService emailService,
        [Frozen] EmailConfiguration emailConfiguration,
        MarketplaceRefundNotificationService sut,
        CancellationToken cancellationToken)
    {
        emailConfiguration.BookingInvoiceEmailSender = "Skedular <no-reply@staging.skedular.app>";
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            RequestedByCustomerId = "customer-1",
            Status = MarketplaceRefundStatusConstants.Processing,
            RefundAmount = 25m,
            Currency = "NZD",
            Reason = "Approved by admin"
        };
        var organization = new Organization
        {
            Id = "org-1",
            Name = "Acme Coworking",
            ContactEmail = "ops@acme.test",
            OrganizationMembers =
            [
                new OrganizationMember
                {
                    Status = OrganizationMemberStatusConstants.Active,
                    Role = OrganizationMemberRoleConstants.Owner,
                    Customer = new Customer { Identities = [new Identity { Email = "owner@acme.test", EmailVerified = true }] }
                }
            ]
        };
        var customer = new Customer
        {
            Id = "customer-1",
            GivenName = "Jamie",
            FamilyName = "Doe",
            Identities = [new Identity { Email = "jamie@example.com", EmailVerified = true }]
        };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken)).Returns(organization);
        A.CallTo(() => customerRepository.GetByIdAsync("customer-1", true, cancellationToken)).Returns(customer);

        await sut.NotifyStatusChangedAsync(refund, cancellationToken);

        A.CallTo(() => emailService.SendRawEmailAsync(
                A<string>.That.Contains("Refund update"),
                A<string>.That.Contains("refund is still being processed"),
                A<string>._,
                A<string>.That.Contains("Acme Coworking"),
                A<IReadOnlyList<string>>.That.Matches(items => items.Count == 1 && items.Contains("jamie@example.com")),
                A<IReadOnlyList<string>>.That.Matches(items => items.Count == 0),
                A<IReadOnlyList<string>>.That.Matches(items => items.Count == 0),
                A<IReadOnlyList<EmailAttachment>>.That.Matches(items => items.Count == 0),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => emailService.SendRawEmailAsync(
                A<string>.That.Contains("Refund update"),
                A<string>.That.Contains("refund is still being processed"),
                A<string>._,
                A<string>.That.Contains("Acme Coworking"),
                A<IReadOnlyList<string>>.That.Matches(items =>
                    items.Count == 2 &&
                    items.Contains("ops@acme.test") &&
                    items.Contains("owner@acme.test")),
                A<IReadOnlyList<string>>.That.Matches(items => items.Count == 0),
                A<IReadOnlyList<string>>.That.Matches(items => items.Count == 0),
                A<IReadOnlyList<EmailAttachment>>.That.Matches(items => items.Count == 0),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Do_Nothing_When_No_Recipients_Can_Be_Resolved(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] IEmailService emailService,
        MarketplaceRefundNotificationService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            RequestedByCustomerId = "customer-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            Status = MarketplaceRefundStatusConstants.Completed
        };
        var organization = new Organization { Id = "org-1", Name = "Acme Coworking", ContactEmail = null };
        var customer = new Customer { Id = "customer-1", Identities = [new Identity { Email = "jamie@example.com", EmailVerified = false }] };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken)).Returns(organization);
        A.CallTo(() => customerRepository.GetByIdAsync("customer-1", true, cancellationToken)).Returns(customer);

        await sut.NotifyStatusChangedAsync(refund, cancellationToken);

        A.CallTo(() => emailService.SendRawEmailAsync(
                A<string>._,
                A<string>._,
                A<string>._,
                A<string>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<EmailAttachment>>._,
                cancellationToken))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Avoid_Duplicate_Send_When_Customer_And_Internal_Email_Are_The_Same(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] IEmailService emailService,
        [Frozen] EmailConfiguration emailConfiguration,
        MarketplaceRefundNotificationService sut,
        CancellationToken cancellationToken)
    {
        emailConfiguration.BookingInvoiceEmailSender = "Skedular <no-reply@staging.skedular.app>";
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            RequestedByCustomerId = "customer-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            Status = MarketplaceRefundStatusConstants.Completed,
            AccountingProvider = AccountingProviderConstants.Xero
        };
        var organization = new Organization
        {
            Id = "org-1",
            Name = "Acme Coworking",
            ContactEmail = "jamie@example.com",
            OrganizationMembers =
            [
                new OrganizationMember
                {
                    Status = OrganizationMemberStatusConstants.Active,
                    Role = OrganizationMemberRoleConstants.Owner,
                    Customer = new Customer
                    {
                        Identities =
                        [
                            new Identity { Email = "jamie@example.com", EmailVerified = true },
                            new Identity { Email = "owner2@acme.test", EmailVerified = true }
                        ]
                    }
                }
            ]
        };
        var customer = new Customer
        {
            Id = "customer-1",
            GivenName = "Jamie",
            FamilyName = "Doe",
            Identities = [new Identity { Email = "jamie@example.com", EmailVerified = true }]
        };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken)).Returns(organization);
        A.CallTo(() => customerRepository.GetByIdAsync("customer-1", true, cancellationToken)).Returns(customer);

        await sut.NotifyStatusChangedAsync(refund, cancellationToken);

        A.CallTo(() => emailService.SendRawEmailAsync(
                A<string>.That.Contains("Refund completed"),
                A<string>.That.Contains("completed through Xero"),
                A<string>._,
                A<string>.That.Contains("Acme Coworking"),
                A<IReadOnlyList<string>>.That.Matches(items => items.Count == 1 && items.Contains("jamie@example.com")),
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<EmailAttachment>>._,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => emailService.SendRawEmailAsync(
                A<string>.That.Contains("Refund completed for booking"),
                A<string>.That.Contains("completed through Xero"),
                A<string>._,
                A<string>.That.Contains("Acme Coworking"),
                A<IReadOnlyList<string>>.That.Matches(items => items.Count == 1 && items.Contains("owner2@acme.test")),
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<EmailAttachment>>._,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Include_Manual_Follow_Up_Language_For_Failed_Refunds(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] IEmailService emailService,
        [Frozen] EmailConfiguration emailConfiguration,
        MarketplaceRefundNotificationService sut,
        CancellationToken cancellationToken)
    {
        emailConfiguration.BookingInvoiceEmailSender = "Skedular <no-reply@staging.skedular.app>";
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            RequestedByCustomerId = "customer-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            Status = MarketplaceRefundStatusConstants.Failed,
            LastError = "Concrete invoice instance has not been correlated yet."
        };
        var organization = new Organization
        {
            Id = "org-1",
            Name = "Acme Coworking",
            ContactEmail = "ops@acme.test",
            OrganizationMembers =
            [
                new OrganizationMember
                {
                    Status = OrganizationMemberStatusConstants.Active,
                    Role = OrganizationMemberRoleConstants.Administrator,
                    Customer = new Customer { Identities = [new Identity { Email = "admin@acme.test", EmailVerified = true }] }
                }
            ]
        };
        var customer = new Customer
        {
            Id = "customer-1",
            GivenName = "Jamie",
            FamilyName = "Doe",
            Identities = [new Identity { Email = "jamie@example.com", EmailVerified = true }]
        };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken)).Returns(organization);
        A.CallTo(() => customerRepository.GetByIdAsync("customer-1", true, cancellationToken)).Returns(customer);

        await sut.NotifyStatusChangedAsync(refund, cancellationToken);

        A.CallTo(() => emailService.SendRawEmailAsync(
                A<string>.That.Contains("needs attention"),
                A<string>.That.Contains("manual follow-up"),
                A<string>._,
                A<string>.That.Contains("Acme Coworking"),
                A<IReadOnlyList<string>>.That.Matches(items => items.Count == 1 && items.Contains("jamie@example.com")),
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<EmailAttachment>>._,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Include_Organization_Specific_Internal_Recipients_And_Manual_Status_Copy(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] IEmailService emailService,
        [Frozen] EmailConfiguration emailConfiguration,
        MarketplaceRefundNotificationService sut,
        CancellationToken cancellationToken)
    {
        emailConfiguration.BookingInvoiceEmailSender = "Skedular <no-reply@staging.skedular.app>";
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            RequestedByCustomerId = "customer-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            Status = MarketplaceRefundStatusConstants.Failed,
            LastError = "Manual bank transfer required"
        };
        var organization = new Organization
        {
            Id = "org-1",
            Name = "Acme Coworking",
            ContactEmail = "ops@acme.test",
            RefundNotificationEmails = ["finance@acme.test", "ops@acme.test"]
        };
        var customer = new Customer
        {
            Id = "customer-1",
            GivenName = "Jamie",
            FamilyName = "Doe",
            Identities = [new Identity { Email = "jamie@example.com", EmailVerified = true }]
        };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken)).Returns(organization);
        A.CallTo(() => customerRepository.GetByIdAsync("customer-1", true, cancellationToken)).Returns(customer);

        await sut.NotifyStatusChangedAsync(refund, cancellationToken);

        A.CallTo(() => emailService.SendRawEmailAsync(
                A<string>.That.Contains("Refund failed"),
                A<string>.That.Contains("manual follow-up"),
                A<string>._,
                A<string>.That.Contains("Acme Coworking"),
                A<IReadOnlyList<string>>.That.Matches(items =>
                    items.Count == 2 && items.Contains("ops@acme.test") && items.Contains("finance@acme.test")),
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<EmailAttachment>>._,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}

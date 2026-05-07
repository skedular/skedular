using Api.Shared.Services;
using EmailValidation;
using Enterprise.Shared;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace Slack.Api.Handlers.ActionHandlers.Billing;

public class EditBillingButtonHandler(
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IEntityMapper entityMapper,
    IPageNavigator pageNavigator,
    IOrganizationBillingService organizationBillingService) : IViewSubmissionHandler
{
    public async Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission)
    {
        var cancellationToken = CancellationToken.None;
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(viewSubmission.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            viewSubmission.User.Id,
            cancellationToken);

        var workspace = entityMapper.MapTo(workspaceEntity);
        var workspaceMember = entityMapper.MapTo(workspaceMemberEntity, workspace);
        var context = CommonPageContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var values = viewSubmission.View.State.Values;
        var organizationBillingDetails = new OrganizationBillingDetails { Organization = new Organization { Id = workspace.Organization.Id } };

        if (values.TryGetValue(BillingActionTypes.CompanyName, out var companyNameBlock))
        {
            if (companyNameBlock.TryGetValue(BillingActionTypes.CompanyName, out var companyName))
            {
                if (companyName is PlainTextInputValue value)
                {
                    organizationBillingDetails.CompanyName = value.Value.ToSafeString();
                }
                else
                {
                    throw new InvalidOperationException("Company name must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("company name block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("company name block is missing");
        }

        if (values.TryGetValue(BillingActionTypes.Email, out var emailBlock))
        {
            if (emailBlock.TryGetValue(BillingActionTypes.Email, out var email))
            {
                if (email is EmailTextInputValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.Value);
                    if (!EmailValidator.Validate(value.Value))
                    {
                        throw new ArgumentException("no valid email address", value.Value);
                    }

                    organizationBillingDetails.Email = value.Value.ToSafeString();
                }
                else
                {
                    throw new InvalidOperationException("email must be EmailTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("email block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("email block is missing");
        }

        if (values.TryGetValue(BillingActionTypes.AddressLine1, out var addressLine1Block))
        {
            if (addressLine1Block.TryGetValue(BillingActionTypes.AddressLine1, out var addressLine1))
            {
                if (addressLine1 is PlainTextInputValue value)
                {
                    organizationBillingDetails.AddressLine1 = value.Value.ToSafeString();
                }
                else
                {
                    throw new InvalidOperationException("address line 1 must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("address line 1 block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("address line 1 block is missing");
        }

        if (values.TryGetValue(BillingActionTypes.AddressLine2, out var addressLine2Block))
        {
            if (addressLine2Block.TryGetValue(BillingActionTypes.AddressLine2, out var addressLine2))
            {
                if (addressLine2 is PlainTextInputValue value)
                {
                    organizationBillingDetails.AddressLine2 = value.Value.ToSafeString();
                }
                else
                {
                    throw new InvalidOperationException("address line 2 must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("address line 2 block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("address line 2 block is missing");
        }

        if (values.TryGetValue(BillingActionTypes.Suburb, out var suburbBlock))
        {
            if (suburbBlock.TryGetValue(BillingActionTypes.Suburb, out var suburb))
            {
                if (suburb is PlainTextInputValue value)
                {
                    organizationBillingDetails.Suburb = value.Value.ToSafeString();
                }
                else
                {
                    throw new InvalidOperationException("suburb must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("suburb block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("suburb block is missing");
        }

        if (values.TryGetValue(BillingActionTypes.City, out var cityBlock))
        {
            if (cityBlock.TryGetValue(BillingActionTypes.City, out var city))
            {
                if (city is PlainTextInputValue value)
                {
                    organizationBillingDetails.City = value.Value.ToSafeString();
                }
                else
                {
                    throw new InvalidOperationException("city must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("city block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("city block is missing");
        }

        if (values.TryGetValue(BillingActionTypes.Province, out var provinceBlock))
        {
            if (provinceBlock.TryGetValue(BillingActionTypes.Province, out var province))
            {
                if (province is PlainTextInputValue value)
                {
                    organizationBillingDetails.Province = value.Value.ToSafeString();
                }
                else
                {
                    throw new InvalidOperationException("province must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("province block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("province block is missing");
        }

        if (values.TryGetValue(BillingActionTypes.Zipcode, out var zipcodeBlock))
        {
            if (zipcodeBlock.TryGetValue(BillingActionTypes.Zipcode, out var zipcode))
            {
                if (zipcode is PlainTextInputValue value)
                {
                    organizationBillingDetails.Zipcode = value.Value.ToSafeString();
                }
                else
                {
                    throw new InvalidOperationException("zipcode must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("zipcode block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("zipcode block is missing");
        }

        if (values.TryGetValue(OptionLoaderKeys.CountryKey, out var countryBlock))
        {
            if (countryBlock.TryGetValue(OptionLoaderKeys.CountryKey, out var country))
            {
                if (country is ExternalSelectValue value)
                {
                    organizationBillingDetails.Country = string.IsNullOrWhiteSpace(value.SelectedOption?.Value)
                        ? string.Empty
                        : value.SelectedOption.Value;
                }
                else
                {
                    throw new InvalidOperationException("country must be ExternalSelectValue");
                }
            }
            else
            {
                throw new InvalidOperationException("country block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("country block is missing");
        }

        await organizationBillingService.AddAsync(workspaceMember.Id, organizationBillingDetails, cancellationToken);

        await pageNavigator.BackAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            viewSubmission.Hash, cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;
}

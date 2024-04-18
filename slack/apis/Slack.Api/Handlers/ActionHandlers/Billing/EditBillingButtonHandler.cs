using EmailValidation;
using Enterprise.Shared;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using SlackNet.Blocks;
using SlackNet.Interaction;
using BillingService = Api.Shared.Services.Grpc.UnityHub.Billing.V1.BillingService;
using SetOrganizationBillingInfoInput = Api.Shared.Services.Grpc.UnityHub.Billing.V1.SetOrganizationBillingInfoInput;

namespace Slack.Api.Handlers.ActionHandlers.Billing;

public class EditBillingButtonHandler(
    BillingConfiguration billingConfiguration,
    BillingService.BillingServiceClient billingServiceClient,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IMapper mapper,
    IPageNavigator pageNavigator) : IViewSubmissionHandler
{
    public async Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission)
    {
        var cancellationToken = CancellationToken.None;

        var workspaceEntity =
            await repositoryFactory.WorkspaceRepository.GetByIdAsync(viewSubmission.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, _) =
            await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
                workspaceEntity,
                viewSubmission.User.Id,
                cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var context = CommonPageContext.Deserialize(viewSubmission.View.PrivateMetadata);

        var values = viewSubmission.View.State.Values;
        var setOrganizationBillingInfoInput =
            new SetOrganizationBillingInfoInput { OrganizationId = workspace.Organization.Id };

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

                    setOrganizationBillingInfoInput.Email = value.Value.ToSafeString();
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
                    setOrganizationBillingInfoInput.AddressLine1 = value.Value.ToSafeString();
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
                    setOrganizationBillingInfoInput.AddressLine2 = value.Value.ToSafeString();
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
                    setOrganizationBillingInfoInput.Suburb = value.Value.ToSafeString();
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
                    setOrganizationBillingInfoInput.City = value.Value.ToSafeString();
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
                    setOrganizationBillingInfoInput.Province = value.Value.ToSafeString();
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
                    setOrganizationBillingInfoInput.Zipcode = value.Value.ToSafeString();
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
                    setOrganizationBillingInfoInput.Country = string.IsNullOrWhiteSpace(value.SelectedOption?.Value)
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

        await billingServiceClient.SetOrganizationBillingInfoAsync(
            setOrganizationBillingInfoInput,
            billingConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        await pageNavigator.BackAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            viewSubmission.Hash, cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;
}

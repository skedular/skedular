using Customer.Api.Mappers;
using Customer.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Customer.Api.GraphQL.Feedback;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<SubmitCustomerFeedbackPayload> SubmitCustomerFeedbackAsync(
        SubmitCustomerFeedbackInput input,
        [Service] ICustomerFeedbackService customerFeedbackService,
        CancellationToken cancellationToken)
    {
        var customerFeedback = await customerFeedbackService.SubmitFeedbackAsync(mapper.MapTo(input), cancellationToken);
        return new SubmitCustomerFeedbackPayload { ClientMutationId = input.ClientMutationId, Id = customerFeedback.Id };
    }
}

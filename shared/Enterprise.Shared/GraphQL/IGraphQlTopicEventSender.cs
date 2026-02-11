namespace Enterprise.Shared.GraphQL;

public interface IGraphQlTopicEventSender
{
    Task RaiseGraphqlChangeAsync(string topicName, string id, CancellationToken cancellationToken);
}

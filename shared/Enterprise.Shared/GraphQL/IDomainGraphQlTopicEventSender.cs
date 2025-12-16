namespace Enterprise.Shared.GraphQL;

public interface IDomainGraphQlTopicEventSender
{
    Task RaiseChangeAsync(string topicName, string id, CancellationToken cancellationToken);
}

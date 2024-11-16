using HotChocolate;

namespace Slack.Api.GraphQL;

public enum SlackDeskBookingDetailBookingStatus
{
    AWAIT_ACCEPTANCE,
    ACCEPTED,
    DECLINED
}

[GraphQLName("Version")]
public class Version
{
    [GraphQLName("major")] public int Major { get; set; }

    [GraphQLName("minor")] public int Minor { get; set; }

    [GraphQLName("build")] public int Build { get; set; }

    [GraphQLName("revision")] public int Revision { get; set; }
}

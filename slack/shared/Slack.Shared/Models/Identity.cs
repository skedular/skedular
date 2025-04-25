using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class Identity : ReplicatedModelBase, IIdentityDetails
{
    public Customer Customer { get; set; }
    public string? Email { get; set; }
    public bool? EmailVerified { get; set; }
}

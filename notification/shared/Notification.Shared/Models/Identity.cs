using Enterprise.Shared.Models;

namespace Notification.Shared.Models;

public class Identity : ReplicatedModelBase
{
    public string? Email { get; set; }
    public bool? EmailVerified { get; set; }
    public virtual Customer Customer { get; set; }
}

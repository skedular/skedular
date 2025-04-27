using Enterprise.Shared.Models;

namespace Billing.Shared.Models;

public class Identity : ReplicatedModelBase
{
    public Customer Customer { get; set; } = new();
}

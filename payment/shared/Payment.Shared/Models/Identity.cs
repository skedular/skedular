using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class Identity : ReplicatedModelBase
{
    public Customer Customer { get; set; }
}

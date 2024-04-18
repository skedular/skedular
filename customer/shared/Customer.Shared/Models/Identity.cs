using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class Identity : ModelBase
{
    public string? Email { get; set; }
    public bool? EmailVerified { get; set; }
    public Customer Customer { get; set; }
}

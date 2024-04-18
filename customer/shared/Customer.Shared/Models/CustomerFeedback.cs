using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class CustomerFeedback : ModelBase
{
    public string? Content { get; set; }
    public Customer Customer { get; set; }
}

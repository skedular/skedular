namespace Location.Shared.Models;

public static class CustomerExtensions
{
    public static string GetCustomerName(this Customer customer)
    {
        if (!string.IsNullOrWhiteSpace(customer.Name))
        {
            return customer.Name;
        }

        List<string?> allNames = [customer.GivenName, customer.MiddleName, customer.FamilyName];
        return allNames.Aggregate(string.Empty, (acc, name) => string.IsNullOrWhiteSpace(name) ? acc : $"{acc} {name}");
    }
}

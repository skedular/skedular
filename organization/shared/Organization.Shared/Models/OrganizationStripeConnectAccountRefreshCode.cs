using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Models;

public class OrganizationStripeConnectAccountRefreshCode : ModelBaseWithDeleted
{
    public string Code { get; set; } = string.Empty;
    public string RedirectUrl { get; set; }= string.Empty;
    public OrganizationStripeConnectAccount OrganizationStripeConnectAccount { get; set; } = new();
}

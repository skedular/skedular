using Api.Shared.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organization.Api.Services;

namespace Organization.Api.Controllers.Admin;

/// <summary>
///     Admin controller for verifying Host organizations.
/// </summary>
[ApiController]
[Route("api/admin/organizations")]
[Authorize(Policy = "AdminOnly")]
public class OrganizationVerificationController(
    IOrganizationService organizationService,
    IOrganizationOwnershipService organizationOwnershipService,
    ILogger<OrganizationVerificationController> logger) : ControllerBase
{
    /// <summary>
    ///     Verifies a Host organization, allowing them to create Locations and Products.
    /// </summary>
    [HttpPost("{organizationId}/verify")]
    public async Task<IActionResult> VerifyHostOrganization(
        string organizationId,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationService.GetByIdOrCustomDomainAsync(organizationId, null, true, cancellationToken);
        if (organization is null)
        {
            return NotFound();
        }

        if (organization.Type != OrganizationType.Host)
        {
            return BadRequest("Only Host organizations can be verified through this endpoint.");
        }

        if (organization.IsOwnershipVerified == true)
        {
            return BadRequest("Organization is already verified.");
        }

        await organizationOwnershipService.VerifyAsync(organizationId, null, cancellationToken);

        logger.LogInformation(
            "Host organization {OrganizationId} verified by admin",
            organizationId);

        return Ok();
    }

    /// <summary>
    ///     Un-verifies a Host organization.
    /// </summary>
    [HttpPost("{organizationId}/unverify")]
    public async Task<IActionResult> UnverifyHostOrganization(
        string organizationId,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationService.GetByIdOrCustomDomainAsync(organizationId, null, true, cancellationToken);
        if (organization is null)
        {
            return NotFound();
        }

        if (organization.Type != OrganizationType.Host)
        {
            return BadRequest("Only Host organizations can be unverified through this endpoint.");
        }

        if (organization.IsOwnershipVerified != true)
        {
            return BadRequest("Organization is not verified.");
        }

        await organizationOwnershipService.UnverifyAsync(organizationId, null, cancellationToken);

        logger.LogWarning(
            "Host organization {OrganizationId} un-verified by admin",
            organizationId);

        return Ok();
    }
}

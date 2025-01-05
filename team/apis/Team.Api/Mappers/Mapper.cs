using Api.Shared.Services.Grpc.Skedular.Team.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Models;
using Team.Api.GraphQL;
using Booking = Team.Shared.Models.Booking;
using Customer = Team.Shared.Models.Customer;
using Identity = Team.Shared.Models.Identity;
using JoinInvitation = Team.Shared.Models.JoinInvitation;
using Location = Team.Shared.Database.Entities.Location;
using Organization = Team.Shared.Database.Entities.Organization;
using OrganizationMember = Team.Shared.Models.OrganizationMember;
using OrganizationMemberRole = Api.Shared.Services.Models.OrganizationMemberRole;
using Permissions = Api.Shared.Services.Grpc.Skedular.Team.V1.Permissions;
using TeamEdge = Team.Api.GraphQL.TeamEdge;
using TeamMember = Team.Shared.Models.TeamMember;
using TeamMemberStatus = Api.Shared.Services.Models.TeamMemberStatus;

namespace Team.Api.Mappers;

public interface IMapper
{
    TeamMember MapTo(Shared.Database.Entities.TeamMember src);
    Shared.Models.Team MapTo(Shared.Database.Entities.Team src);
    Customer? MapTo(Shared.Database.Entities.Customer? src);

    Shared.Database.Entities.Team MapTo(
        Shared.Models.Team src,
        Organization? organization,
        Location? primaryLocation);

    Shared.Database.Entities.Team MergeTo(
        Shared.Models.Team src,
        Shared.Database.Entities.Team dest,
        Organization? organization,
        Location? primaryLocation);

    TeamDetails? MapTo(Shared.Models.Team? src);
    TeamMember MapTo(Shared.Database.Entities.TeamMember src, Shared.Models.Team team);
    TeamMemberDetails MapTo(TeamMember src);
    IEnumerable<TeamDetails> MapTo(IEnumerable<Shared.Models.Team> src);
    Shared.Models.Team MapTo(AddTeamInput src);
    Shared.Models.Team MapTo(UpdateTeamInput src);
    Shared.Models.Team MapTo(UpdateTeamAndTeamMembersInput src);
    TeamMember MapTo(AddTeamMemberInput src);
    ICollection<TeamMember> MapToTeamMembers(UpdateTeamMembersInput src);
    JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src);
    global::Api.Shared.Services.Grpc.Skedular.Team.V1.Team MapToGrpcResponse(Shared.Models.Team src);
    Shared.Models.Team MapTo(AddInput src);
    Shared.Models.Team MapTo(Admin_AddInput src);
    Shared.Models.Team MapTo(UpdateInput src);
    ICollection<TeamMember> MapTo(Admin_UpdateMembersInput src);
    TeamEdge MapTo(Edge<Shared.Models.Team> src);
    global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamEdge MapToGrpcResponse(Edge<Shared.Models.Team> src);

    IEnumerable<Edge<TeamMember>> MapTo(
        IEnumerable<Edge<Shared.Database.Entities.TeamMember>> src,
        Shared.Models.Team team);

    TeamMemberEdge MapTo(Edge<TeamMember> src);
}

public class Mapper : IMapper
{
    public TeamMember MapTo(Shared.Database.Entities.TeamMember src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Role = src.Role switch
            {
                TeamMemberRoleConstants.Owner => TeamMemberRole.Owner,
                TeamMemberRoleConstants.Administrator => TeamMemberRole.Administrator,
                TeamMemberRoleConstants.Member => TeamMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = src.Status switch
            {
                TeamMemberStatusConstants.Active => TeamMemberStatus.Active,
                TeamMemberStatusConstants.Inactive => TeamMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = MapTo(src.Customer)!,
            OrganizationMember = MapTo(src.OrganizationMember)
        };


    public Shared.Models.Team MapTo(Shared.Database.Entities.Team src)
    {
        var team = new Shared.Models.Team
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Organization = MapTo(src.Organization),
            PrimaryLocation = MapTo(src.PrimaryLocation)
        };

        team.TeamMembers = MapTo(src.TeamMembers, team).ToList();
        team.Bookings = MapTo(src.Bookings, team).ToList();
        team.JoinInvitations = MapTo(src.JoinInvitations, team).ToList();

        return team;
    }

    public Customer? MapTo(Shared.Database.Entities.Customer? src) =>
        src is null
            ? null
            : new Customer
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                GivenName = src.GivenName,
                MiddleName = src.MiddleName,
                FamilyName = src.FamilyName,
                PhotoUrl = src.PhotoUrl,
                PhotoUrl24 = src.PhotoUrl24,
                PhotoUrl32 = src.PhotoUrl32,
                PhotoUrl48 = src.PhotoUrl48,
                PhotoUrl72 = src.PhotoUrl72,
                PhotoUrl192 = src.PhotoUrl192,
                PhotoUrl512 = src.PhotoUrl512,
                PhoneNumber = src.PhoneNumber,
                Identities = MapTo(src.Identities).ToList()
            };

    public Shared.Database.Entities.Team MapTo(
        Shared.Models.Team src,
        Organization? organization,
        Location? primaryLocation) =>
        MergeTo(src, new Shared.Database.Entities.Team(), organization, primaryLocation);

    public Shared.Database.Entities.Team MergeTo(
        Shared.Models.Team src,
        Shared.Database.Entities.Team dest,
        Organization? organization,
        Location? primaryLocation)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.About = src.About;
        dest.Timezone = src.Timezone;
        dest.Organization = organization;
        dest.PrimaryLocation = primaryLocation;
        return dest;
    }

    public TeamDetails? MapTo(Shared.Models.Team? src) =>
        src is null
            ? null
            : new TeamDetails
            {
                Id = src.Id,
                Name = src.Name,
                About = src.About,
                Timezone = src.Timezone,
                CanModify = src.Permissions.CanModify,
                CanDelete = src.Permissions.CanDelete,
                CanInvitePeople = src.Permissions.CanInvitePeople,
                HasFutureBooking = src.HasFutureBooking,
                Organization = MapTo(src.Organization),
                PrimaryLocation = MapTo(src.PrimaryLocation),
                Members = MapTo(src.TeamMembers).ToArray()
            };

    public TeamMember MapTo(Shared.Database.Entities.TeamMember src, Shared.Models.Team team) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Role = src.Role switch
            {
                TeamMemberRoleConstants.Owner => TeamMemberRole.Owner,
                TeamMemberRoleConstants.Administrator => TeamMemberRole.Administrator,
                TeamMemberRoleConstants.Member => TeamMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = src.Status switch
            {
                TeamMemberStatusConstants.Active => TeamMemberStatus.Active,
                TeamMemberStatusConstants.Inactive => TeamMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = MapTo(src.Customer)!,
            Team = team,
            OrganizationMember = MapTo(src.OrganizationMember)
        };

    public TeamMemberDetails MapTo(TeamMember src) =>
        new()
        {
            Id = src.Id,
            Role = src.Role,
            Status = src.Status,
            Customer = MapTo(src.Customer),
            OrganizationMember = MapTo(src.OrganizationMember)
        };

    public IEnumerable<TeamDetails> MapTo(IEnumerable<Shared.Models.Team> src) =>
        src.Select(MapTo)!;

    public Shared.Models.Team MapTo(AddTeamInput src) =>
        new()
        {
            Id = string.IsNullOrWhiteSpace(src.Id) ? string.Empty : src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId)
                ? null
                : new Shared.Models.Organization { Id = src.OrganizationId },
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId)
                ? null
                : new Shared.Models.Location { Id = src.PrimaryLocationId },
            TeamMembers = src.CustomerIds
                .Select(item => new TeamMember { Customer = new Customer { Id = item } })
                .Concat(src.OrganizationMemberIds.Select(item =>
                    new TeamMember { OrganizationMember = new OrganizationMember { Id = item } }))
                .ToList()
        };

    public Shared.Models.Team MapTo(UpdateTeamInput src) =>
        new()
        {
            Id = string.IsNullOrWhiteSpace(src.Id) ? string.Empty : src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId)
                ? null
                : new Shared.Models.Organization { Id = src.OrganizationId },
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId)
                ? null
                : new Shared.Models.Location { Id = src.PrimaryLocationId }
        };

    public Shared.Models.Team MapTo(UpdateTeamAndTeamMembersInput src) =>
        new()
        {
            Id = string.IsNullOrWhiteSpace(src.Id) ? string.Empty : src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId)
                ? null
                : new Shared.Models.Organization { Id = src.OrganizationId },
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId)
                ? null
                : new Shared.Models.Location { Id = src.PrimaryLocationId },
            TeamMembers = src.CustomerIds
                .Select(item => new TeamMember { Customer = new Customer { Id = item } })
                .Concat(src.OrganizationMemberIds.Select(item =>
                    new TeamMember { OrganizationMember = new OrganizationMember { Id = item } }))
                .ToList()
        };

    public TeamMember MapTo(AddTeamMemberInput src)
    {
        var teamMember = new TeamMember
        {
            OrganizationMember = string.IsNullOrWhiteSpace(src.OrganizationMemberId)
                ? null
                : new OrganizationMember { Id = src.OrganizationMemberId }
        };

        if (!string.IsNullOrWhiteSpace(src.CustomerId))
        {
            teamMember.Customer = new Customer { Id = src.CustomerId };
        }

        return teamMember;
    }

    public ICollection<TeamMember> MapToTeamMembers(UpdateTeamMembersInput src) =>
        src.CustomerIds
            .Select(item => new TeamMember { Customer = new Customer { Id = item } })
            .Concat(src.OrganizationMemberIds.Select(item =>
                new TeamMember { OrganizationMember = new OrganizationMember { Id = item } }))
            .ToList();

    public JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status switch
            {
                InvitationStatusConstants.Pending => InvitationStatus.Pending,
                InvitationStatusConstants.Accepted => InvitationStatus.Accepted,
                InvitationStatusConstants.Rejected => InvitationStatus.Rejected,
                InvitationStatusConstants.Cancelled => InvitationStatus.Cancelled,
                _ => throw new ArgumentOutOfRangeException()
            },
            Role = src.Role switch
            {
                TeamMemberRoleConstants.Owner => TeamMemberRole.Owner,
                TeamMemberRoleConstants.Administrator => TeamMemberRole.Administrator,
                TeamMemberRoleConstants.Member => TeamMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Team = MapTo(src.Team),
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };

    public global::Api.Shared.Services.Grpc.Skedular.Team.V1.Team MapToGrpcResponse(Shared.Models.Team src)
    {
        var team = new global::Api.Shared.Services.Grpc.Skedular.Team.V1.Team
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            OrganizationId = string.IsNullOrWhiteSpace(src.Organization?.Id) ? string.Empty : src.Organization.Id,
            PrimaryLocation =
                string.IsNullOrWhiteSpace(src.PrimaryLocation?.Id)
                    ? null
                    : new global::Api.Shared.Services.Grpc.Skedular.Team.V1.Location
                    {
                        Id = src.PrimaryLocation.Id,
                        Name = src.PrimaryLocation.Name.ToSafeString()
                    },
            Permissions = new Permissions
            {
                CanView = src.Permissions.CanView,
                CanModify = src.Permissions.CanModify,
                CanDelete = src.Permissions.CanDelete,
                CanInvitePeople = src.Permissions.CanInvitePeople,
                CanCancelPeopleExistingInvitations = src.Permissions.CanCancelPeopleExistingInvitations
            },
            HasFutureBooking = src.HasFutureBooking
        };

        team.Members.AddRange(MapToGrpcResponse(src.TeamMembers));

        return team;
    }


    public Shared.Models.Team MapTo(AddInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId)
                ? null
                : new Shared.Models.Organization { Id = src.OrganizationId },
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId)
                ? null
                : new Shared.Models.Location { Id = src.PrimaryLocationId },
            TeamMembers = src.Members.Select(item => MapTo(item, new Shared.Models.Team { Id = src.Id })).ToList()
        };

    public Shared.Models.Team MapTo(Admin_AddInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId)
                ? null
                : new Shared.Models.Organization { Id = src.OrganizationId },
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId)
                ? null
                : new Shared.Models.Location { Id = src.PrimaryLocationId },
            TeamMembers = src.Members.Select(item => MapTo(item, new Shared.Models.Team { Id = src.Id })).ToList()
        };

    public Shared.Models.Team MapTo(UpdateInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId)
                ? null
                : new Shared.Models.Organization { Id = src.OrganizationId },
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId)
                ? null
                : new Shared.Models.Location { Id = src.PrimaryLocationId },
            TeamMembers = src.Members.Select(item => MapTo(item, new Shared.Models.Team { Id = src.Id })).ToList()
        };

    public ICollection<TeamMember> MapTo(Admin_UpdateMembersInput src) =>
        src.Members.Select(item => MapTo(item, new Shared.Models.Team { Id = src.Id })).ToList();

    public TeamEdge MapTo(Edge<Shared.Models.Team> src) =>
        new() { Cursor = src.Cursor, Node = MapTo(src.Node)! };

    public global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamEdge MapToGrpcResponse(Edge<Shared.Models.Team> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public IEnumerable<Edge<TeamMember>> MapTo(
        IEnumerable<Edge<Shared.Database.Entities.TeamMember>> src,
        Shared.Models.Team team) =>
        src.Select(item => MapTo(item, team));

    public TeamMemberEdge MapTo(Edge<TeamMember> src) =>
        new() { Cursor = src.Cursor, Node = MapTo(src.Node) };

    private IEnumerable<TeamMember> MapTo(
        IEnumerable<Shared.Database.Entities.TeamMember> src,
        Shared.Models.Team team) =>
        src.Select(item => MapTo(item, team));

    private IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMember>
     MapToGrpcResponse(IEnumerable<TeamMember> src) => src.Select(MapToGrpcResponse);

    private global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMember MapToGrpcResponse(TeamMember src) =>
        new()
        {
            Id = src.Id,
            Role = src.Role switch
            {
                TeamMemberRole.Owner => Role.Owner,
                TeamMemberRole.Administrator => Role.Administrator,
                TeamMemberRole.Member => Role.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = src.Status switch
            {
                TeamMemberStatus.Active => global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMemberStatus.Active,
                TeamMemberStatus.Inactive => global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMemberStatus
                    .Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = MapToGrpcResponse(src.Customer),
            OrganizationMember = src.OrganizationMember is null || string.IsNullOrWhiteSpace(src.OrganizationMember.Id)
                ? null
                : new global::Api.Shared.Services.Grpc.Skedular.Team.V1.OrganizationMember
                {
                    Id = src.OrganizationMember.Id,
                    Role = src.OrganizationMember.Role switch
                    {
                        OrganizationMemberRole.Owner => Role.Owner,
                        OrganizationMemberRole.Administrator => Role.Administrator,
                        OrganizationMemberRole.Member => Role.Member,
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    Customer = MapToGrpcResponse(src.OrganizationMember.Customer)
                }
        };

    private static global::Api.Shared.Services.Grpc.Skedular.Team.V1.Customer MapToGrpcResponse(
        Customer src)
    {
        var customer = new global::Api.Shared.Services.Grpc.Skedular.Team.V1.Customer
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            GivenName = src.GivenName.ToSafeString(),
            MiddleName = src.MiddleName.ToSafeString(),
            FamilyName = src.FamilyName.ToSafeString(),
            PhotoUrl = src.PhotoUrl.ToSafeString(),
            PhotoUrl24 = src.PhotoUrl24.ToSafeString(),
            PhotoUrl32 = src.PhotoUrl32.ToSafeString(),
            PhotoUrl48 = src.PhotoUrl48.ToSafeString(),
            PhotoUrl72 = src.PhotoUrl72.ToSafeString(),
            PhotoUrl192 = src.PhotoUrl192.ToSafeString(),
            PhotoUrl512 = src.PhotoUrl512.ToSafeString(),
            PhoneNumber = src.PhoneNumber.ToSafeString()
        };

        return customer;
    }

    private static TeamMember MapTo(
        global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMember src,
        Shared.Models.Team team) =>
        new()
        {
            Id = src.Id,
            Role = src.Role switch
            {
                Role.Owner => TeamMemberRole.Owner,
                Role.Administrator => TeamMemberRole.Administrator,
                Role.Member => TeamMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = src.Status switch
            {
                global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMemberStatus.Active => TeamMemberStatus.Active,
                global::Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMemberStatus.Inactive =>
                    TeamMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Customer { Id = src.Customer.Id },
            OrganizationMember = src.OrganizationMember is null || string.IsNullOrWhiteSpace(src.OrganizationMember.Id)
                ? null
                : new OrganizationMember
                {
                    Id = src.OrganizationMember.Id,
                    Customer = new Customer { Id = src.OrganizationMember.Customer.Id }
                },
            Team = team
        };

    private static Shared.Models.Organization? MapTo(Organization? src) =>
        src is null
            ? null
            : new Shared.Models.Organization
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                LogoUrl = src.LogoUrl,
                Offering = src.Offering
            };

    private OrganizationMember?
        MapTo(Shared.Database.Entities.OrganizationMember? src) =>
        src is null
            ? null
            : new OrganizationMember
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                Role = src.Role switch
                {
                    OrganizationMemberRoleConstants.Owner => OrganizationMemberRole.Owner,
                    OrganizationMemberRoleConstants.Administrator => OrganizationMemberRole.Administrator,
                    OrganizationMemberRoleConstants.Member => OrganizationMemberRole.Member,
                    _ => throw new ArgumentOutOfRangeException()
                },
                Status = src.Status switch
                {
                    OrganizationMemberStatusConstants.Active => OrganizationMemberStatus.Active,
                    OrganizationMemberStatusConstants.Inactive => OrganizationMemberStatus.Inactive,
                    _ => throw new ArgumentOutOfRangeException()
                },
                Customer = MapTo(src.Customer)!,
                Organization = MapTo(src.Organization)!
            };

    private TeamOrganizationMemberDetails? MapTo(OrganizationMember? src) =>
        src is null ? null : new TeamOrganizationMemberDetails { UniqueId = src.Id, Customer = MapTo(src.Customer) };

    private IEnumerable<TeamMemberDetails> MapTo(IEnumerable<TeamMember> src) => src.Select(MapTo);

    private static TeamOrganizationDetails? MapTo(Shared.Models.Organization? src) =>
        src is null
            ? null
            : new TeamOrganizationDetails { UniqueId = src.Id, Name = src.Name.ToSafeString(), LogoUrl = src.LogoUrl };

    private static TeamLocationDetails? MapTo(Shared.Models.Location? src) =>
        src is null
            ? null
            : new TeamLocationDetails { UniqueId = src.Id, Name = src.Name.ToSafeString() };

    private static TeamCustomerDetails MapTo(Customer src) =>
        new()
        {
            UniqueId = src.Id,
            Email = src.Identities
                .Where(identity => !string.IsNullOrWhiteSpace(identity.Email))
                .Select(item => item.Email!.ToLowerInvariant())
                .Distinct()
                .FirstOrDefault(),
            Name = src.Name,
            GivenName = src.GivenName,
            MiddleName = src.MiddleName,
            FamilyName = src.FamilyName,
            PhotoUrl = src.PhotoUrl,
            PhotoUrl24 = src.PhotoUrl24,
            PhotoUrl32 = src.PhotoUrl32,
            PhotoUrl48 = src.PhotoUrl48,
            PhotoUrl72 = src.PhotoUrl72,
            PhotoUrl192 = src.PhotoUrl192,
            PhotoUrl512 = src.PhotoUrl512,
            PhoneNumber = src.PhoneNumber
        };

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity> src) =>
        src.Select(MapTo);

    private static Identity MapTo(Shared.Database.Entities.Identity src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Email = src.Email,
            EmailVerified = src.EmailVerified
        };

    private static IEnumerable<Booking> MapTo(IEnumerable<Shared.Database.Entities.Booking> src,
        Shared.Models.Team team) =>
        src.Select(item => MapTo(item, team));

    private static Booking MapTo(Shared.Database.Entities.Booking src,
        Shared.Models.Team team) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            From = src.From,
            To = src.To,
            Team = team
        };

    private IEnumerable<JoinInvitation> MapTo(
        IEnumerable<Shared.Database.Entities.JoinInvitation> src,
        Shared.Models.Team team) =>
        src.Select(item => MapTo(item, team));

    private JoinInvitation MapTo(
        Shared.Database.Entities.JoinInvitation src,
        Shared.Models.Team team) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status switch
            {
                InvitationStatusConstants.Pending => InvitationStatus.Pending,
                InvitationStatusConstants.Accepted => InvitationStatus.Accepted,
                InvitationStatusConstants.Rejected => InvitationStatus.Rejected,
                InvitationStatusConstants.Cancelled => InvitationStatus.Cancelled,
                _ => throw new ArgumentOutOfRangeException()
            },
            Team = team,
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };

    private Edge<TeamMember> MapTo(Edge<Shared.Database.Entities.TeamMember> src, Shared.Models.Team team) =>
        new(src.Cursor, MapTo(src.Node, team));

    private static Shared.Models.Location? MapTo(Location? src) =>
        src is null
            ? null
            : new Shared.Models.Location
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name
            };
}

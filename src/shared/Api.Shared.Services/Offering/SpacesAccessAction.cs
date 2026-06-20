namespace Api.Shared.Services.Offering;

public enum SpacesAccessAction
{
    Read = 0,
    CreateOrModify = 1,
    CreateBookingInstance = 2,
    ProtectExistingCommitment = 3,
    AccountOrUpgrade = 4
}

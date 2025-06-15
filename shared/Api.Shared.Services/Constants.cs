namespace Api.Shared.Services;

public static class Constants
{
    public const int MaxVerifiableTokenLength = 200;

    public const int MaxPersonTitleLength = 100;

    public const int MaxPersonDesignationLength = 100;

    public const int MaxGivenNameLength = 100;

    public const int MaxMiddleNameLength = 100;

    public const int MaxFamilyNameLength = 100;

    public const int MaxPersonNameLength = MaxGivenNameLength + MaxGivenNameLength + MaxFamilyNameLength;

    public const int MaxUrlLength = 2000;

    public const int MaxContentTypeLength = 100;

    public const int MaxTimezoneLength = 64;

    public const int MaxLocaleLength = 32;

    public const int MaxPhoneNumberLength = 64;

    public const int MaxEmailLength = 64 + 1 + 255; // RFC 5321 and RFC 5322

    public const int MaxDescriptionLength = 1000;

    public const int MaxOrganizationNameLength = 200;
    public const int MaxOrganizationIndustryMainCategoryNameLength = 100;
    public const int MaxOrganizationIndustrySubCategoryNameLength = 100;

    public const int MaxLocationNameLength = 200;
    public const int MaxResourceNameLength = 200;
    public const int MaxDeskNameLength = 200;

    public const int MaxTagNameLength = 100;
    public const int MaxTagDescriptionLength = 1000;
    public const int MaxTagTypeLength = 50;
    public const int MaxColorValueLength = 32;

    public const int MaxAddressLineLength = 200;
    public const int MaxSuburbLength = 100;
    public const int MaxCityLength = 100;
    public const int MaxProvinceLength = 100;
    public const int MaxZipcodeLength = 20;
    public const int MaxCountryLength = 100;
    public const int MaxCurrencyLength = 20;
    public const int MaxTermsOfUseLength = 10000;

    public const int StripeCustomerIdLength = 200;

    public const int MaxTeamNameLength = 200;
    public const int MaxTenantNameLength = 200;

    public const int MaxBookingNotesLength = 1000;

    public const int MaxAzureTeamNameLength = 1000;
    public const int MaxAzureTeamChannelNameLength = 1000;

    public const int MaxSsoEntityIdLength = 200;

    public const int MaxRoleLength = 32;

    public const int MaxInvitationStatusLength = 32;
    public const int MaxNotificationTypeLength = 128;
    public const int MaxBookingTypeLength = 32;
    public const int MaxBookingStatusLength = 32;
    public const int MaxPaymentStatusLength = 32;

    public const int MaxOrganizationMemberStatusLength = 64;
    public const int MaxTeamMemberStatusLength = 64;

    public const int MaxFeedbackLength = 10240;
    public const int MaxFeedbackChannelLength = 32;

    public const int MaxOrganizationTypeLength = 50;
    public const int MaxOrganizationMemberVisibilityPolicyLength = 50;

    public const int MaxProductNameLength = 500;
    public const int MaxProductDescriptionLength = 10000;
    public const int MaxProductPriceCurrencyLength = 16;
    public const int MaxProductPriceUnitLength = 16;

    public const int MaxStripeConnectAccountIdLength = 100;
    public const int MaxStripeConnectAccountNameLength = 200;
    public const int MaxStripeConnectAccountTypeLength = 50;
    public const int MaxStripeCapabilitiesStatusLength = 50;
    public const int MaxStripeCurrencyLength = 10;
    public const int MaxStripeConnectAccountCompanyNameLength = 200;
    public const int MaxStripeBusinessTypeLength = 50;
    public const int MaxStripeConnectAccountRefreshCodeLength = 50;

    public const int MaxStripeSetupIntentIdLength = 100;
    public const int MaxStripePaymentMethodIdLength = 100;
    public const int MaxStripeCardBrandLength = 20;
    public const int MaxStripeCardCountryLength = 10;
    public const int MaxStripeCardDescriptionLength = 200;
    public const int MaxStripeCardFingerprintLength = 20;
    public const int MaxStripeCardFundingLength = 50;
    public const int MaxStripeCardIssuerLength = 20;
    public const int MaxStripeCardLastFourDigitLength = 10;
    public const int MaxStripeProductIdLength = 100;
    public const int MaxStripePriceIdLength = 100;
    public const int MaxStripeCheckoutSessionIdLength = 100;
    public const int MaxStripeCheckoutSessionPaymentStatusLength = 50;

    // Floor plan related constants
    public const int MaxFilePathLength = 500;
    public const int MaxShapeNameLength = 50;
}

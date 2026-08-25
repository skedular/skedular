import { array, boolean, object, string } from 'yup';

export type OrganizationDetails = {
  customDomain: string | null;
  name: string;
  website: string | null;
  customerFacingTermsAndConditionsUrl: string | null;
  industrySubCategoryIds: string[];
  contactEmail: string | null;
  contactPhone: string | null;
  refundNotificationEmailsText: string | null;
};

export type PhysicalAddressDetails = {
  osmType: string | null | undefined;
  osmId: string | null | undefined;
  placeId: string | null | undefined;
  longitude: number | null | undefined;
  latitude: number | null | undefined;
  formattedAddress: string | null | undefined;
  country: string;
  addressLine1: string;
  addressLine2: string | null;
  suburb: string | null;
  city: string | null;
  province: string | null;
  zipcode: string;
  countryCode: string;
};

export type BillingDetails = {
  companyName: string | null;
  email: string;
  osmType: string | null | undefined;
  osmId: string | null | undefined;
  placeId: string | null | undefined;
  longitude: number | null | undefined;
  latitude: number | null | undefined;
  formattedAddress: string | null | undefined;
  country: string;
  addressLine1: string;
  addressLine2: string | null;
  suburb: string | null;
  city: string | null;
  province: string | null;
  zipcode: string;
  countryCode: string;
};

export type SsoSettingsDetails = {
  entityId: string;
  loginUrl: string;
  appFederationMetadataUrl: string;
};

export type TaxDetails = {
  isRegistered: boolean;
  taxId: string;
  taxRatePercentage: string;
};

export const splitNotificationEmails = (value: string | null | undefined) =>
  (value ?? '')
    .split(/[\n,;]/)
    .map((item) => item.trim())
    .filter((item) => item.length !== 0);

export const organizationSchema = object({
  customDomain: string().nullable(),
  name: string().min(3, 'Organization name must be at least three characters long.').required('Organization name is required'),
  website: string().url('Website must be a valid Url').nullable(),
  customerFacingTermsAndConditionsUrl: string().url('Terms and Conditions must be a valid Url').nullable(),
  industrySubCategoryIds: array().nullable(),
  contactEmail: string()
    .email(({ value }) => `${value} is not a valid email`)
    .nullable(),
  contactPhone: string().nullable(),
  refundNotificationEmailsText: string()
    .nullable()
    .test('refund-notification-emails', 'Each refund notification email must be a valid email address.', (value) =>
      splitNotificationEmails(value).every((item) => string().email().isValidSync(item)),
    ),
});

export const physicalAddressSchema = object({
  addressLine1: string().required('Address line 1 is required'),
  addressLine2: string().nullable(),
  suburb: string().nullable(),
  city: string().nullable(),
  province: string().nullable(),
  zipcode: string().required('Zipcode is required'),
  countryCode: string().required('Country is required'),
});

export const billingSchema = object({
  companyName: string().nullable(),
  email: string()
    .email(({ value }) => `${value} is not a valid email`)
    .required('Email is required'),
  addressLine1: string().required('Address line 1 is required'),
  addressLine2: string().nullable(),
  suburb: string().nullable(),
  city: string().nullable(),
  province: string().nullable(),
  zipcode: string().required('Zipcode is required'),
  countryCode: string().required('Country is required'),
});

export const ssoSettingsSchema = object({
  entityId: string().required('Entity ID is required'),
  loginUrl: string().required('Login Url is required'),
  appFederationMetadataUrl: string().required('App Federation Metadata Url is required'),
});

export const taxDetailsSchema = object({
  isRegistered: boolean().required(),
  taxId: string().test('tax-id-required-when-registered', 'Tax ID / VAT / GST Number is required.', function (value) {
    if (!this.parent.isRegistered) {
      return true;
    }

    return (value ?? '').trim().length > 0;
  }),
  taxRatePercentage: string()
    .test('tax-rate-required-when-registered', 'Tax rate is required.', function (value) {
      if (!this.parent.isRegistered) {
        return true;
      }

      return (value ?? '').trim().length > 0;
    })
    .test('tax-rate-format', 'Tax rate must be a valid decimal number.', function (value) {
      if ((value ?? '').trim().length === 0) {
        return true;
      }

      return /^\d+(\.\d{1,2})?$/.test(value!);
    })
    .test('is-greater-than-zero', 'Tax rate must be greater than zero.', function (value) {
      if ((value ?? '').trim().length === 0) {
        return true;
      }

      const taxRatePercentage = Number(value);
      if (isNaN(taxRatePercentage)) {
        return true;
      }

      return taxRatePercentage > 0;
    }),
});

import { array, object, string } from 'yup';

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

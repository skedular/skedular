import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/fetch';
import { Address, PhysicalAddress } from '@/components/address';
import {
  AppBarWithStackColumn,
  BodyIconTypography,
  CreditCard,
  ExtraLargeHeadingIconTypography,
  FormFieldLabel,
  FormStackColumn,
  GridContainer,
  HelperText,
  LeadIconTypography,
  PushToRight,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@/components/commons';
import { CustomTag } from '@/components/customTag';
import { DeleteIcon, EllipseMenuIcon, ErrorIcon, NewIcon, NotPreferredIcon, PreferredIcon, TickIcon } from '@/components/icons';
import { getOrganizationBaseLink, getRootLink } from '@/components/links';
import { ListingMetadata, listingMetadataSchemaShape } from '@/components/listingMetadata';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { OrganizationMultipleChoicesIndustries } from '@/components/organization';
import { AddOrganizationCustomTagButton } from '@/components/organization/addOrganizationCustomTag';
import { AddOrganizationPaymentMethodDialog } from '@/components/organization/addOrganizationPaymentMethod';
import { AddOrganizationZoneButton } from '@/components/organization/addOrganizationZone';
import { EditOrganizationCustomTagDialog } from '@/components/organization/editOrganizationCustomTag';
import { EditOrganizationZoneDialog } from '@/components/organization/editOrganizationZone/';
import { Search } from '@/components/search';
import { Zone } from '@/components/zone';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import { defaultGridRowSelectionModelValue } from '@/libs/mui';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { coal, defaultButtonStyle, defaultGridActionPadding, defaultGridStyle, defaultPadding, emerald, secondDrawerExpandedDrawerWidthPx } from '@/libs/theme';
import { joinErrors, keyboardTextFieldDebounceTimeout } from '@/libs/utils';
import type { organizationAdmin_addCustomerPreferredOrganizationTagMutation } from '@/queries/__generated__/organizationAdmin_addCustomerPreferredOrganizationTagMutation.graphql';
import type { organizationAdmin_addOrganizationBillingDetailsMutation } from '@/queries/__generated__/organizationAdmin_addOrganizationBillingDetailsMutation.graphql';
import type { organizationAdmin_addOrganizationPhysicalAddressMutation } from '@/queries/__generated__/organizationAdmin_addOrganizationPhysicalAddressMutation.graphql';
import type { organizationAdmin_cancelOrganizationOfferingMutation } from '@/queries/__generated__/organizationAdmin_cancelOrganizationOfferingMutation.graphql';
import type { organizationAdmin_customTags_query$key } from '@/queries/__generated__/organizationAdmin_customTags_query.graphql';
import type { organizationAdmin_customTags_refetchableFragment } from '@/queries/__generated__/organizationAdmin_customTags_refetchableFragment.graphql';
import type { organizationAdmin_deleteCustomTagsMutation } from '@/queries/__generated__/organizationAdmin_deleteCustomTagsMutation.graphql';
import type { organizationAdmin_deleteOrganizationMutation } from '@/queries/__generated__/organizationAdmin_deleteOrganizationMutation.graphql';
import type { organizationAdmin_deleteZonesMutation } from '@/queries/__generated__/organizationAdmin_deleteZonesMutation.graphql';
import type { organizationAdmin_organization_query$key } from '@/queries/__generated__/organizationAdmin_organization_query.graphql';
import type { organizationAdmin_organization_refetchableFragment } from '@/queries/__generated__/organizationAdmin_organization_refetchableFragment.graphql';
import type { organizationAdmin_query$key } from '@/queries/__generated__/organizationAdmin_query.graphql';
import type { organizationAdmin_removeCustomerPreferredOrganizationTagMutation } from '@/queries/__generated__/organizationAdmin_removeCustomerPreferredOrganizationTagMutation.graphql';
import type { organizationAdmin_removeOrganizationPaymentMethodMutation } from '@/queries/__generated__/organizationAdmin_removeOrganizationPaymentMethodMutation.graphql';
import type { organizationAdmin_removeOrganizationSsoSettingsMutation } from '@/queries/__generated__/organizationAdmin_removeOrganizationSsoSettingsMutation.graphql';
import type { organizationAdmin_removeOrganizationTaxDetailsMutation } from '@/queries/__generated__/organizationAdmin_removeOrganizationTaxDetailsMutation.graphql';
import type { organizationAdmin_updateOrganizationBillingDetailsMutation } from '@/queries/__generated__/organizationAdmin_updateOrganizationBillingDetailsMutation.graphql';
import type { organizationAdmin_updateOrganizationMutation } from '@/queries/__generated__/organizationAdmin_updateOrganizationMutation.graphql';
import type { organizationAdmin_updateOrganizationOfferingMutation } from '@/queries/__generated__/organizationAdmin_updateOrganizationOfferingMutation.graphql';
import type { organizationAdmin_updateOrganizationPhysicalAddressMutation } from '@/queries/__generated__/organizationAdmin_updateOrganizationPhysicalAddressMutation.graphql';
import type { organizationAdmin_updateOrganizationSsoSettingsMutation } from '@/queries/__generated__/organizationAdmin_updateOrganizationSsoSettingsMutation.graphql';
import type { organizationAdmin_updateOrganizationTaxDetailsMutation } from '@/queries/__generated__/organizationAdmin_updateOrganizationTaxDetailsMutation.graphql';
import type { organizationAdmin_zones_query$key } from '@/queries/__generated__/organizationAdmin_zones_query.graphql';
import type { organizationAdmin_zones_refetchableFragment } from '@/queries/__generated__/organizationAdmin_zones_refetchableFragment.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import Switch from '@mui/material/Switch';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import type { TCountryCode } from 'countries-list';
import { getCountryData } from 'countries-list';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { array, object, string } from 'yup';
import OrganizationAdminLeftSideNavigationMenuContent from './organization-admin-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationAdmin_query$key;
  rootDataOrganizationRelay: organizationAdmin_organization_query$key;
  rootDataZonesRelay: organizationAdmin_zones_query$key;
  rootDataCustomTagsRelay: organizationAdmin_customTags_query$key;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

type OrganizationDetails = {
  customDomain: string | null;
  name: string;
  about: string | null;
  title: string | null;
  subTitle: string | null;
  website: string | null;
  customerFacingTermsAndConditionsUrl: string | null;
  industrySubCategoryIds: string[];
  contactEmail: string;
  contactPhone: string | null;
};

const organizationSchema = object({
  customDomain: string().nullable(),
  name: string().min(3, 'Organization name must be at least three characters long.').required('Organization name is required'),
  ...listingMetadataSchemaShape,
  website: string().url('Website must be a valid Url').nullable(),
  customerFacingTermsAndConditionsUrl: string().url('Terms and Conditions must be a valid Url').nullable(),
  industrySubCategoryIds: array().nullable(),
  contactEmail: string()
    .email(({ value }) => `${value} is not a valid email`)
    .required('Contact email is required'),
  contactPhone: string().nullable(),
});

type PhysicalAddress = {
  addressLine1: string;
  addressLine2: string | null;
  suburb: string | null;
  city: string | null;
  province: string | null;
  zipcode: string;
  countryCode: string;
};

const physicalAddressSchema = object({
  addressLine1: string().required('Address line 1 is required'),
  addressLine2: string().nullable(),
  suburb: string(),
  city: string(),
  province: string().nullable(),
  zipcode: string().required('Zipcode is required'),
  countryCode: string().required('Country is required'),
});

type BillingDetails = {
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

const billingSchema = object({
  companyName: string().nullable(),
  email: string()
    .email(({ value }) => `${value} is not a valid email`)
    .required('Email is required'),
  addressLine1: string().required('Address line 1 is required'),
  addressLine2: string().nullable(),
  suburb: string(),
  city: string(),
  province: string().nullable(),
  zipcode: string().required('Zipcode is required'),
  countryCode: string().required('Country is required'),
});

type ssoSettingsDetails = {
  entityId: string;
  loginUrl: string;
  appFederationMetadataUrl: string;
};

const ssoSettingsSchema = object({
  entityId: string().required('Entity ID is required'),
  loginUrl: string().required('Login Url is required'),
  appFederationMetadataUrl: string().required('App Federation Metadata Url is required'),
});

type TaxDetails = {
  taxId: string;
  taxRatePercentage: string;
};

const taxDetailsSchema = object({
  taxId: string().required('Tax ID / VAT / GST Number'),
  taxRatePercentage: string()
    .matches(/^\d+(\.\d{1,2})?$/, 'Tax rate must be a valid decimal number.')
    .required('Tax rate is required.')
    .test('is-greater-than-zero', 'Tax rate must be greater than zero.', function (value) {
      const taxRatePercentage = Number(value);
      if (isNaN(taxRatePercentage)) {
        return true;
      }

      return taxRatePercentage > 0;
    }),
});

type ZoneRowType = {
  id: string;
  name: string;
  description: string | null | undefined;
  preferred: boolean;
};

type CustomTagRowType = {
  id: string;
  name: string;
  description: string | null | undefined;
  preferred: boolean;
};

const OrganizationAdmin = ({ rootDataRelay, rootDataOrganizationRelay, rootDataZonesRelay, rootDataCustomTagsRelay, onReloadRequired, organizationCustomDomain }: Props) => {
  const rootData = useFragment<organizationAdmin_query$key>(
    graphql`
      fragment organizationAdmin_query on Query {
        emailsToShowLatestCapabilities
        me {
          id
          emails
          preferredZones {
            id
          }
          preferredCustomTags {
            id
          }
        }
        organizationIndustryMainCategoriesReferences {
          subCategories {
            id
            name
          }
        }
        ...organizationMultipleChoicesIndustries_query
      }
    `,
    rootDataRelay,
  );

  const [rootDataOrganization, refetchOrganization] = useRefetchableFragment<organizationAdmin_organization_refetchableFragment, organizationAdmin_organization_query$key>(
    graphql`
      fragment organizationAdmin_organization_query on Query @refetchable(queryName: "organizationAdmin_organization_refetchableFragment") {
        organization(customDomain: $organizationCustomDomain) {
          id
          customDomain
          name
          billingCycle {
            type
            name
          }
          logoUrl
          listingMetadata {
            about
            title
            subTitle
            includedFeatures
          }
          marketplaceListingMetadata {
            about
            title
            subTitle
            includedFeatures
          }
          website
          customerFacingTermsAndConditionsUrl
          canModify
          industrySubCategories {
            id
            name
          }
          contactEmail
          contactPhone
          physicalAddress {
            id
            osmType
            osmId
            placeId
            longitude
            latitude
            formattedAddress
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
            countryCode
          }
          hasAttachedPaymentMethod
          paymentMethods {
            id
            cardBrand
            cardExpiryMonth
            cardExpiryYear
            cardLastFourDigit
          }
          activeOffering {
            id
            isEnterprise
            name
            start
            end
            unitPrice
            featureSet
            underPriceLines
            free
          }
          availableOfferings {
            isEnterprise
            code
            name
            unitPrice
            featureSet
            underPriceLines
            free
          }
          ssoSettings {
            id
            isActive
            entityId
            loginUrl
            appFederationMetadataUrl
          }
          taxDetails {
            taxId
            taxRatePercentage
          }
          billingDetails {
            id
            companyName
            email
            osmType
            osmId
            placeId
            longitude
            latitude
            formattedAddress
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
            countryCode
          }
          featureImages {
            original {
              url
              height
              width
            }
            thumbnail {
              url
              height
              width
            }
          }
        }
      }
    `,
    rootDataOrganizationRelay,
  );

  const [rootDataZones, refetchZones] = useRefetchableFragment<organizationAdmin_zones_refetchableFragment, organizationAdmin_zones_query$key>(
    graphql`
      fragment organizationAdmin_zones_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationAdmin_zones_refetchableFragment") {
        organization(customDomain: $organizationCustomDomain) {
          zones(first: $count, after: $cursor, where: { nameContains: $zoneNameSearchText }) @connection(key: "organizationAdmin_zones") {
            __id
            totalCount
            edges {
              node {
                id
                name
                description
                color
              }
            }
          }
        }
      }
    `,
    rootDataZonesRelay,
  );

  const [rootDataCustomTags, refetchCustomTags] = useRefetchableFragment<organizationAdmin_customTags_refetchableFragment, organizationAdmin_customTags_query$key>(
    graphql`
      fragment organizationAdmin_customTags_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationAdmin_customTags_refetchableFragment") {
        organization(customDomain: $organizationCustomDomain) {
          customTags(first: $count, after: $cursor, where: { nameContains: $customTagNameSearchText }, orderBy: [{ direction: ASCENDING, field: NAME }])
            @connection(key: "organizationAdmin_customTags") {
            __id
            totalCount
            edges {
              node {
                id
                name
                description
                color
              }
            }
          }
        }
      }
    `,
    rootDataCustomTagsRelay,
  );

  const [commitUpdateOrganization] = useMutation<organizationAdmin_updateOrganizationMutation>(graphql`
    mutation organizationAdmin_updateOrganizationMutation($input: UpdateOrganizationInput!) @raw_response_type {
      updateOrganization(input: $input) {
        organization {
          id
          customDomain
          name
          billingCycle {
            type
            name
          }
          listingMetadata {
            about
            title
            subTitle
          }
          marketplaceListingMetadata {
            about
            title
            subTitle
            includedFeatures
          }
          website
          customerFacingTermsAndConditionsUrl
          industrySubCategories {
            id
            name
          }
          contactEmail
          contactPhone
          featureImages {
            original {
              url
              height
              width
            }
            thumbnail {
              url
              height
              width
            }
          }
        }
      }
    }
  `);

  const [commitDeleteZones] = useMutation<organizationAdmin_deleteZonesMutation>(graphql`
    mutation organizationAdmin_deleteZonesMutation($connectionIds: [ID!]!, $input: DeleteZonesInput!) {
      deleteZones(input: $input) {
        organizationTags {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitDeleteCustomTags] = useMutation<organizationAdmin_deleteCustomTagsMutation>(graphql`
    mutation organizationAdmin_deleteCustomTagsMutation($connectionIds: [ID!]!, $input: DeleteCustomTagsInput!) {
      deleteCustomTags(input: $input) {
        organizationTags {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddOrganizationBillingDetails] = useMutation<organizationAdmin_addOrganizationBillingDetailsMutation>(graphql`
    mutation organizationAdmin_addOrganizationBillingDetailsMutation($input: AddOrganizationBillingDetailsInput!) @raw_response_type {
      addOrganizationBillingDetails(input: $input) {
        organization {
          id
          billingDetails {
            id
            companyName
            email
            osmType
            osmId
            placeId
            longitude
            latitude
            formattedAddress
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
            countryCode
          }
        }
      }
    }
  `);

  const [commitUpdateOrganizationBillingDetails] = useMutation<organizationAdmin_updateOrganizationBillingDetailsMutation>(graphql`
    mutation organizationAdmin_updateOrganizationBillingDetailsMutation($input: UpdateOrganizationBillingDetailsInput!) @raw_response_type {
      updateOrganizationBillingDetails(input: $input) {
        organization {
          id
          billingDetails {
            id
            companyName
            email
            osmType
            osmId
            placeId
            longitude
            latitude
            formattedAddress
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
            countryCode
          }
        }
      }
    }
  `);

  const [commitAddOrganizationPhysicalAddress] = useMutation<organizationAdmin_addOrganizationPhysicalAddressMutation>(graphql`
    mutation organizationAdmin_addOrganizationPhysicalAddressMutation($input: AddOrganizationPhysicalAddressInput!) @raw_response_type {
      addOrganizationPhysicalAddress(input: $input) {
        organization {
          id
          physicalAddress {
            id
            osmType
            osmId
            placeId
            longitude
            latitude
            formattedAddress
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
            countryCode
          }
        }
      }
    }
  `);

  const [commitUpdateOrganizationPhysicalAddress] = useMutation<organizationAdmin_updateOrganizationPhysicalAddressMutation>(graphql`
    mutation organizationAdmin_updateOrganizationPhysicalAddressMutation($input: UpdateOrganizationPhysicalAddressInput!) @raw_response_type {
      updateOrganizationPhysicalAddress(input: $input) {
        organization {
          id
          physicalAddress {
            id
            osmType
            osmId
            placeId
            longitude
            latitude
            formattedAddress
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
            countryCode
          }
        }
      }
    }
  `);

  const [commitRemoveOrganizationPaymentMethod] = useMutation<organizationAdmin_removeOrganizationPaymentMethodMutation>(graphql`
    mutation organizationAdmin_removeOrganizationPaymentMethodMutation($input: RemoveOrganizationPaymentMethodInput!) {
      removeOrganizationPaymentMethod(input: $input) {
        clientMutationId
      }
    }
  `);

  const [commitCancelOrganizationOffering] = useMutation<organizationAdmin_cancelOrganizationOfferingMutation>(graphql`
    mutation organizationAdmin_cancelOrganizationOfferingMutation($input: CancelOrganizationOfferingInput!) {
      cancelOrganizationOffering(input: $input) {
        clientMutationId
      }
    }
  `);

  const [commitUpdateOrganizationOffering] = useMutation<organizationAdmin_updateOrganizationOfferingMutation>(graphql`
    mutation organizationAdmin_updateOrganizationOfferingMutation($input: UpdateOrganizationOfferingInput!) {
      updateOrganizationOffering(input: $input) {
        clientMutationId
      }
    }
  `);

  const [commitAddCustomerPreferredOrganizationTag] = useMutation<organizationAdmin_addCustomerPreferredOrganizationTagMutation>(graphql`
    mutation organizationAdmin_addCustomerPreferredOrganizationTagMutation($input: AddCustomerPreferredOrganizationTagInput!) {
      addCustomerPreferredOrganizationTag(input: $input) {
        customer {
          id
          preferredZones {
            id
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerPreferredOrganizationTag] = useMutation<organizationAdmin_removeCustomerPreferredOrganizationTagMutation>(graphql`
    mutation organizationAdmin_removeCustomerPreferredOrganizationTagMutation($input: RemoveCustomerPreferredOrganizationTagInput!) {
      removeCustomerPreferredOrganizationTag(input: $input) {
        customer {
          id
          preferredZones {
            id
          }
        }
      }
    }
  `);

  const [commitDeleteOrganization] = useMutation<organizationAdmin_deleteOrganizationMutation>(graphql`
    mutation organizationAdmin_deleteOrganizationMutation($input: DeleteOrganizationInput!) {
      deleteOrganization(input: $input) {
        organization {
          id
        }
      }
    }
  `);

  const [commitUpdateOrganizationSsoSettings] = useMutation<organizationAdmin_updateOrganizationSsoSettingsMutation>(graphql`
    mutation organizationAdmin_updateOrganizationSsoSettingsMutation($input: UpdateOrganizationSsoSettingsInput!) @raw_response_type {
      updateOrganizationSsoSettings(input: $input) {
        organization {
          id
          ssoSettings {
            id
            isActive
            entityId
            loginUrl
            appFederationMetadataUrl
          }
        }
      }
    }
  `);

  const [commitRemoveOrganizationSsoSettings] = useMutation<organizationAdmin_removeOrganizationSsoSettingsMutation>(graphql`
    mutation organizationAdmin_removeOrganizationSsoSettingsMutation($input: RemoveOrganizationSsoSettingsInput!) @raw_response_type {
      removeOrganizationSsoSettings(input: $input) {
        organization {
          id
          ssoSettings {
            id
            isActive
            entityId
            loginUrl
            appFederationMetadataUrl
          }
        }
      }
    }
  `);

  const [commitUpdateOrganizationTaxDetails] = useMutation<organizationAdmin_updateOrganizationTaxDetailsMutation>(graphql`
    mutation organizationAdmin_updateOrganizationTaxDetailsMutation($input: UpdateOrganizationTaxDetailsInput!) @raw_response_type {
      updateOrganizationTaxDetails(input: $input) {
        organization {
          id
          taxDetails {
            taxId
            taxRatePercentage
          }
        }
      }
    }
  `);

  const [commitRemoveOrganizationTaxDetails] = useMutation<organizationAdmin_removeOrganizationTaxDetailsMutation>(graphql`
    mutation organizationAdmin_removeOrganizationTaxDetailsMutation($input: RemoveOrganizationTaxDetailsInput!) @raw_response_type {
      removeOrganizationTaxDetails(input: $input) {
        organization {
          id
          taxDetails {
            taxId
            taxRatePercentage
          }
        }
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const [, startTransition] = useTransition();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const router = useRouter();
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const [isAddPaymentMethodDialogOpen, setIsAddPaymentMethodDialogOpen] = useState(false);

  const validateOrganizationDetails = makeValidate(organizationSchema);
  const requiredOrganizationDetailsFields = makeRequired(organizationSchema);

  const [organizationEditableCustomDomain, setOrganizationEditableCustomDomain] = useState(rootDataOrganization.organization?.customDomain);
  const debounceSetOrganizationEditableCustomDomain = useDebounceCallback(setOrganizationEditableCustomDomain, keyboardTextFieldDebounceTimeout);
  const [organizationName, setOrganizationName] = useState<string>(rootDataOrganization.organization?.name ?? '');
  const debounceSetOrganizationName = useDebounceCallback(setOrganizationName, keyboardTextFieldDebounceTimeout);
  const [organizationAbout, setOrganizationAbout] = useState(rootDataOrganization.organization?.listingMetadata.about ?? null);
  const debounceSetOrganizationAbout = useDebounceCallback(setOrganizationAbout, keyboardTextFieldDebounceTimeout);
  const [organizationTitle, setOrganizationTitle] = useState(rootDataOrganization.organization?.listingMetadata.title ?? null);
  const debounceSetOrganizationTitle = useDebounceCallback(setOrganizationTitle, keyboardTextFieldDebounceTimeout);
  const [organizationSubTitle, setOrganizationSubTitle] = useState(rootDataOrganization.organization?.listingMetadata.subTitle ?? null);
  const debounceSetOrganizationSubTitle = useDebounceCallback(setOrganizationSubTitle, keyboardTextFieldDebounceTimeout);
  const [organizationWebsite, setOrganizationWebsite] = useState(rootDataOrganization.organization?.website);
  const debounceSetOrganizationWebsite = useDebounceCallback(setOrganizationWebsite, keyboardTextFieldDebounceTimeout);
  const [organizationCustomerFacingTermsAndConditionsUrl, setOrganizationCustomerFacingTermsAndConditionsUrl] = useState(
    rootDataOrganization.organization?.customerFacingTermsAndConditionsUrl,
  );
  const debounceSetOrganizationCustomerFacingTermsAndConditionsUrl = useDebounceCallback(setOrganizationCustomerFacingTermsAndConditionsUrl, keyboardTextFieldDebounceTimeout);
  const [organizationIndustrySubCategoryIds, setOrganizationIndustrySubCategoryIds] = useState<string[]>(
    rootDataOrganization.organization?.industrySubCategories.map(({ id }) => id) ?? [],
  );
  const debounceSetOrganizationIndustrySubCategoryIds = useDebounceCallback(setOrganizationIndustrySubCategoryIds, keyboardTextFieldDebounceTimeout);
  const [organizationContactEmail, setOrganizationContactEmail] = useState<string>(rootDataOrganization.organization?.contactEmail ?? '');
  const debounceSetOrganizationContactEmail = useDebounceCallback(setOrganizationContactEmail, keyboardTextFieldDebounceTimeout);
  const [organizationContactPhone, setOrganizationContactPhone] = useState(rootDataOrganization.organization?.contactPhone);
  const debounceSetOrganizationContactPhone = useDebounceCallback(setOrganizationContactPhone, keyboardTextFieldDebounceTimeout);

  const [featureImages, setFeatureImages] = useState<FileUploadResponse[]>(
    rootDataOrganization.organization
      ? rootDataOrganization.organization.featureImages
          .filter((item) => !!item.original)
          .map((item) => ({
            id: '',
            original: {
              url: item.original!.url,
              height: item.original!.height,
              width: item.original!.width,
            },
            thumbnail: item.thumbnail
              ? {
                  url: item.thumbnail.url,
                  height: item.thumbnail.height,
                  width: item.thumbnail.width,
                }
              : null,
          }))
      : [],
  );
  const [primaryFeatureImage, setPrimaryFeatureImage] = useState<FileUploadResponse | null>(featureImages[0] ?? null);
  const validatePhysicalAddress = makeValidate(physicalAddressSchema);
  const requiredPhysicalAddressFields = makeRequired(physicalAddressSchema);
  const [physicalAddressOsmType, setPhysicalAddressOsmType] = useState(rootDataOrganization.organization?.physicalAddress?.osmType);
  const [physicalAddressOsmId, setPhysicalAddressOsmId] = useState(rootDataOrganization.organization?.physicalAddress?.osmId);
  const [physicalAddressPlaceId, setPhysicalAddressPlaceId] = useState(rootDataOrganization.organization?.physicalAddress?.placeId);
  const [physicalAddressLongitude, setPhysicalAddressLongitude] = useState(rootDataOrganization.organization?.physicalAddress?.longitude);
  const [physicalAddressLatitude, setPhysicalAddressLatitude] = useState(rootDataOrganization.organization?.physicalAddress?.latitude);
  const [physicalAddressFormattedAddress, setPhysicalAddressFormattedAddress] = useState(rootDataOrganization.organization?.physicalAddress?.formattedAddress);
  const [physicalAddressAddressLine1, setPhysicalAddressAddressLine1] = useState<string>(rootDataOrganization.organization?.physicalAddress?.addressLine1 ?? '');
  const debounceSetPhysicalAddressAddressLine1 = useDebounceCallback(setPhysicalAddressAddressLine1, keyboardTextFieldDebounceTimeout);
  const [physicalAddressAddressLine2, setPhysicalAddressAddressLine2] = useState(rootDataOrganization.organization?.physicalAddress?.addressLine2);
  const debounceSetPhysicalAddressAddressLine2 = useDebounceCallback(setPhysicalAddressAddressLine2, keyboardTextFieldDebounceTimeout);
  const [physicalAddressSuburb, setPhysicalAddressSuburb] = useState(rootDataOrganization.organization?.physicalAddress?.suburb);
  const debounceSetPhysicalAddressSuburb = useDebounceCallback(setPhysicalAddressSuburb, keyboardTextFieldDebounceTimeout);
  const [physicalAddressCity, setPhysicalAddressCity] = useState(rootDataOrganization.organization?.physicalAddress?.city);
  const debounceSetPhysicalAddressCity = useDebounceCallback(setPhysicalAddressCity, keyboardTextFieldDebounceTimeout);
  const [physicalAddressProvince, setPhysicalAddressProvince] = useState(rootDataOrganization.organization?.physicalAddress?.province);
  const debounceSetPhysicalAddressProvince = useDebounceCallback(setPhysicalAddressProvince, keyboardTextFieldDebounceTimeout);
  const [physicalAddressZipcode, setPhysicalAddressZipcode] = useState<string>(rootDataOrganization.organization?.physicalAddress?.zipcode ?? '');
  const debounceSetPhysicalAddressZipcode = useDebounceCallback(setPhysicalAddressZipcode, keyboardTextFieldDebounceTimeout);
  const [physicalAddressCountry, setPhysicalAddressCountry] = useState<string>(rootDataOrganization.organization?.physicalAddress?.country ?? '');
  const [physicalAddressCountryCode, setPhysicalAddressCountryCode] = useState<string>(rootDataOrganization.organization?.physicalAddress?.countryCode ?? '');
  const debounceSetPhysicalAddressCountryCode = useDebounceCallback(setPhysicalAddressCountryCode, keyboardTextFieldDebounceTimeout);

  const validateOrganizationBilling = makeValidate(billingSchema);
  const requiredBillingFields = makeRequired(billingSchema);
  const [billingCompanyName, setBillingCompanyName] = useState(rootDataOrganization.organization?.billingDetails?.companyName);
  const debounceSetBillingCompanyName = useDebounceCallback(setBillingCompanyName, keyboardTextFieldDebounceTimeout);
  const [billingEmail, setBillingEmail] = useState<string>(rootDataOrganization.organization?.billingDetails?.email ?? '');
  const debounceSetBillingEmail = useDebounceCallback(setBillingEmail, keyboardTextFieldDebounceTimeout);
  const [billingOsmType, setBillingOsmType] = useState(rootDataOrganization.organization?.billingDetails?.osmType);
  const [billingOsmId, setBillingOsmId] = useState(rootDataOrganization.organization?.billingDetails?.osmId);
  const [billingPlaceId, setBillingPlaceId] = useState(rootDataOrganization.organization?.billingDetails?.placeId);
  const [billingLongitude, setBillingLongitude] = useState(rootDataOrganization.organization?.billingDetails?.longitude);
  const [billingLatitude, setBillingLatitude] = useState(rootDataOrganization.organization?.billingDetails?.latitude);
  const [billingFormattedAddress, setBillingFormattedAddress] = useState(rootDataOrganization.organization?.billingDetails?.formattedAddress);
  const [billingAddressLine1, setBillingAddressLine1] = useState<string>(rootDataOrganization.organization?.billingDetails?.addressLine1 ?? '');
  const debounceSetBillingAddressLine1 = useDebounceCallback(setBillingAddressLine1, keyboardTextFieldDebounceTimeout);
  const [billingAddressLine2, setBillingAddressLine2] = useState(rootDataOrganization.organization?.billingDetails?.addressLine2);
  const debounceSetBillingAddressLine2 = useDebounceCallback(setBillingAddressLine2, keyboardTextFieldDebounceTimeout);
  const [billingSuburb, setBillingSuburb] = useState(rootDataOrganization.organization?.billingDetails?.suburb);
  const debounceSetBillingSuburb = useDebounceCallback(setBillingSuburb, keyboardTextFieldDebounceTimeout);
  const [billingCity, setBillingCity] = useState(rootDataOrganization.organization?.billingDetails?.city);
  const debounceSetBillingCity = useDebounceCallback(setBillingCity, keyboardTextFieldDebounceTimeout);
  const [billingProvince, setBillingProvince] = useState(rootDataOrganization.organization?.billingDetails?.province);
  const debounceSetBillingProvince = useDebounceCallback(setBillingProvince, keyboardTextFieldDebounceTimeout);
  const [billingZipcode, setBillingZipcode] = useState<string>(rootDataOrganization.organization?.billingDetails?.zipcode ?? '');
  const debounceSetBillingZipcode = useDebounceCallback(setBillingZipcode, keyboardTextFieldDebounceTimeout);
  const [billingCountry, setBillingCountry] = useState<string>(rootDataOrganization.organization?.billingDetails?.country ?? '');
  const [billingCountryCode, setBillingCountryCode] = useState<string>(rootDataOrganization.organization?.billingDetails?.countryCode ?? '');
  const debounceSetBillingCountryCode = useDebounceCallback(setBillingCountryCode, keyboardTextFieldDebounceTimeout);

  const validateSsoSettings = makeValidate(ssoSettingsSchema);
  const requiredSsoSettingsFields = makeRequired(ssoSettingsSchema);
  const [ssoSettingsEnabled, setSsoSettingsEnabled] = useState(rootDataOrganization.organization?.ssoSettings?.isActive);
  const [ssoSettingsEntityId, setSsoSettingsEntityId] = useState<string>(rootDataOrganization.organization?.ssoSettings?.entityId ?? '');
  const debounceSetSsoSettingsEntityId = useDebounceCallback(setSsoSettingsEntityId, keyboardTextFieldDebounceTimeout);
  const [ssoSettingsLoginUrl, setSsoSettingsLoginUrl] = useState<string>(rootDataOrganization.organization?.ssoSettings?.loginUrl ?? '');
  const debounceSetSsoSettingsLoginUrl = useDebounceCallback(setSsoSettingsLoginUrl, keyboardTextFieldDebounceTimeout);
  const [ssoSettingsAppFederationMetadataUrl, setSsoSettingsAppFederationMetadataUrl] = useState<string>(
    rootDataOrganization.organization?.ssoSettings?.appFederationMetadataUrl ?? '',
  );
  const debounceSetSsoSettingsppFederationMetadataUrl = useDebounceCallback(setSsoSettingsAppFederationMetadataUrl, keyboardTextFieldDebounceTimeout);

  const validateTaxDetails = makeValidate(taxDetailsSchema);
  const requiredTaxDetailsFields = makeRequired(taxDetailsSchema);
  const [taxDetailsEnabled, setTaxDetailsEnabled] = useState(!!rootDataOrganization.organization?.taxDetails);
  const [taxDetailsTaxId, setTaxDetailsTaxId] = useState<string>(rootDataOrganization.organization?.taxDetails?.taxId ?? '');
  const debounceSetTaxDetailsTaxId = useDebounceCallback(setTaxDetailsTaxId, keyboardTextFieldDebounceTimeout);
  const [taxDetailsTaxRatePercentage, setTaxDetailsTaxRatePercentage] = useState<string>(rootDataOrganization.organization?.taxDetails?.taxRatePercentage ?? '');
  const debounceSetTaxDetailsTaxRatePercentage = useDebounceCallback(setTaxDetailsTaxRatePercentage, keyboardTextFieldDebounceTimeout);

  const [zoneNameSearchText, setZoneNameSearchText] = useState<string>('');
  const [seledctedZones, setSeledctedZones] = useState<GridRowSelectionModel>(defaultGridRowSelectionModelValue);
  const [selectedZoneId, setSelectedZoneId] = useState<null | string>(null);
  const [zoneMoreActionsAnchorEl, setZoneMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const zoneMoreActionsMenuOpen = Boolean(zoneMoreActionsAnchorEl);
  const [isEditZoneDialogOpen, setIsEditZoneDialogOpen] = useState(false);
  const [preferredZones, setPreferredZones] = useState(rootData.me?.preferredZones.map(({ id }) => id) ?? []);
  const zones = useMemo(() => (rootDataZones.organization ? rootDataZones.organization.zones.edges.map(({ node }) => node) : []), [rootDataZones.organization]);
  const zonesConnectionIds = useMemo(() => (rootDataZones.organization ? [rootDataZones.organization.zones.__id] : []), [rootDataZones.organization]);
  const zoneMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditZone],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteZone],
  ];

  const handleRefetchZones = useCallback(
    (zoneNameSearchText: string) => {
      startTransition(() => {
        refetchZones(
          {
            zoneNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [startTransition, refetchZones],
  );

  const [customTagNameSearchText, setCustomTagNameSearchText] = useState<string>('');
  const [seledctedCustomTags, setSeledctedCustomTags] = useState<GridRowSelectionModel>(defaultGridRowSelectionModelValue);
  const [selectedCustomTagId, setSelectedCustomTagId] = useState<null | string>(null);
  const [customTagMoreActionsAnchorEl, setCustomTagMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const customTagMoreActionsMenuOpen = Boolean(customTagMoreActionsAnchorEl);
  const [isEditCustomTagDialogOpen, setIsEditCustomTagDialogOpen] = useState(false);
  const [preferredCustomTags, setPreferredCustomTags] = useState(rootData.me?.preferredCustomTags.map(({ id }) => id) ?? []);
  const customTags = useMemo(
    () => (rootDataCustomTags.organization ? rootDataCustomTags.organization.customTags.edges.map(({ node }) => node) : []),
    [rootDataCustomTags.organization],
  );
  const customTagsConnectionIds = useMemo(() => (rootDataCustomTags.organization ? [rootDataCustomTags.organization.customTags.__id] : []), [rootDataCustomTags.organization]);
  const customTagMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditCustomTag],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteCustomTag],
  ];

  const handleRefetchCustomTags = useCallback(
    (customTagNameSearchText: string) => {
      startTransition(() => {
        refetchCustomTags(
          {
            customTagNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [startTransition, refetchCustomTags],
  );

  useEffect(() => {
    if (!section || section === 'setup') {
      return;
    }

    const element = sectionRefs.current[section];
    if (!element) {
      return;
    }

    const appBarHeight = document.querySelector('.app-bar')?.clientHeight || 0;
    const elementTop = element.getBoundingClientRect().top + window.scrollY;
    window.scrollTo({
      top: elementTop - appBarHeight,
      behavior: 'smooth',
    });
  }, [section]);

  const handleRefetchOrganizationPaymentMethodsDetails = useCallback(() => {
    startTransition(() => {
      refetchOrganization(
        {},
        {
          fetchPolicy: 'store-and-network',
        },
      );
    });
  }, [startTransition, refetchOrganization]);

  const handleOrganizationDetailUpdateClick = ({
    customDomain,
    name,
    about,
    title,
    subTitle,
    website,
    customerFacingTermsAndConditionsUrl,
    industrySubCategoryIds,
    contactEmail,
    contactPhone,
  }: OrganizationDetails) => {
    const organization = rootDataOrganization.organization;
    if (!organization) {
      return;
    }

    const selectedIndustrySubCategoryIds = industrySubCategoryIds ?? [];
    const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}'...`} />, infoNotificationOptions);
    const finalFeatureImages = featureImages.map((image) => ({
      original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
      thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
    }));

    commitUpdateOrganization({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: organization.id,
          customDomain,
          name,
          listingMetadata: {
            about: about ?? '',
            title: title ?? '',
            subTitle: subTitle ?? '',
          },
          marketplaceListingMetadata: organization.marketplaceListingMetadata,
          website,
          customerFacingTermsAndConditionsUrl,
          industrySubCategoryIds: selectedIndustrySubCategoryIds,
          contactEmail,
          contactPhone,
          featureImages: finalFeatureImages,
          billingCycle: organization.billingCycle.type,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update organization '${organization?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${name} details updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update organization '${organization?.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateOrganization: {
          organization: {
            id: organization.id,
            customDomain: organization.customDomain,
            name,
            listingMetadata: {
              about: about ?? '',
              title: title ?? '',
              subTitle: subTitle ?? '',
            },
            marketplaceListingMetadata: organization.marketplaceListingMetadata,
            website,
            customerFacingTermsAndConditionsUrl,
            industrySubCategories: rootData.organizationIndustryMainCategoriesReferences
              .flatMap((mainCategory) => mainCategory.subCategories)
              .filter(({ id }) => selectedIndustrySubCategoryIds.find((selectedIndustrySubCategoryId) => selectedIndustrySubCategoryId === id))
              .map(({ id, name }) => ({ id, name })),
            contactEmail,
            contactPhone,
            featureImages: finalFeatureImages,
            billingCycle: organization.billingCycle,
          },
        },
      },
    });
  };

  const handleBillingAddressSelect = (address: Address) => {
    setBillingOsmType(address.osmType);
    setBillingOsmId(address.osmId);
    setBillingPlaceId(address.placeId);
    setBillingLongitude(address.longitude);
    setBillingLatitude(address.latitude);
    setBillingFormattedAddress(address.formattedAddress);
    setBillingAddressLine1(address.addressLine1 ?? '');
    setBillingAddressLine2(address.addressLine2 ?? '');
    setBillingSuburb(address.suburb ?? '');
    setBillingCity(address.city ?? '');
    setBillingProvince(address.province ?? '');
    setBillingZipcode(address.zipcode ?? '');
    setBillingCountry(address.country ?? '');
    setBillingCountryCode(address.countryCode ?? '');
  };

  const handleBillingDetailUpdateClick = ({ companyName, email, addressLine1, addressLine2, suburb, city, province, zipcode, countryCode }: BillingDetails) => {
    const organization = rootDataOrganization.organization;
    if (!organization) {
      return;
    }

    const countryData = getCountryData(countryCode as TCountryCode);
    let country = billingCountry;
    if (countryData) {
      country = countryData.name;
    }

    const billingDetails = organization.billingDetails;

    if (billingDetails) {
      const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}' billing...`} />, infoNotificationOptions);

      commitUpdateOrganizationBillingDetails({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: billingDetails.id,
            companyName,
            email,
            osmType: billingOsmType,
            osmId: billingOsmId,
            placeId: billingPlaceId,
            longitude: billingLongitude,
            latitude: billingLatitude,
            formattedAddress: billingFormattedAddress,
            addressLine1,
            addressLine2,
            suburb,
            city,
            province,
            zipcode,
            country,
            countryCode,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to update organization '${organization?.name}' billing. Error: ${joinErrors(errors)}.`} />,
            });

            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content={`Organization '${organization?.name}' billing updated.`} />,
          });
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update organization '${organization?.name}' billing. Error: ${error.message}.`} />,
          });
        },
        optimisticResponse: {
          updateOrganizationBillingDetails: {
            organization: {
              id: organization.id,
              billingDetails: {
                id: billingDetails.id,
                companyName,
                email,
                osmType: billingOsmType,
                osmId: billingOsmId,
                placeId: billingPlaceId,
                longitude: billingLongitude,
                latitude: billingLatitude,
                formattedAddress: billingFormattedAddress,
                addressLine1,
                addressLine2,
                suburb,
                city,
                province,
                zipcode,
                country,
                countryCode,
              },
            },
          },
        },
      });
    } else {
      const id = uuid();
      const toastId = themedToast(<NotificationContent content={`Adding organization '${organization.name}' billing...`} />, infoNotificationOptions);

      commitAddOrganizationBillingDetails({
        variables: {
          input: {
            clientMutationId: uuid(),
            organizationCustomDomain,
            id,
            companyName,
            email,
            osmType: billingOsmType,
            osmId: billingOsmId,
            placeId: billingPlaceId,
            longitude: billingLongitude,
            latitude: billingLatitude,
            formattedAddress: billingFormattedAddress,
            addressLine1,
            addressLine2,
            suburb,
            city,
            province,
            zipcode,
            country,
            countryCode,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to add organization '${organization?.name}' billing. Error: ${joinErrors(errors)}.`} />,
            });

            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content={`Organization '${organization?.name}' billing added.`} />,
          });
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add organization '${organization?.name}' billing. Error: ${error.message}.`} />,
          });
        },
        optimisticResponse: {
          addOrganizationBillingDetails: {
            organization: {
              id: organization.id,
              billingDetails: {
                id,
                companyName,
                email,
                osmType: billingOsmType,
                osmId: billingOsmId,
                placeId: billingPlaceId,
                longitude: billingLongitude,
                latitude: billingLatitude,
                formattedAddress: billingFormattedAddress,
                addressLine1,
                addressLine2,
                suburb,
                city,
                province,
                zipcode,
                country,
                countryCode,
              },
            },
          },
        },
      });
    }
  };

  const handlePhysicalAddressSelect = (address: Address) => {
    setPhysicalAddressOsmType(address.osmType);
    setPhysicalAddressOsmId(address.osmId);
    setPhysicalAddressPlaceId(address.placeId);
    setPhysicalAddressLongitude(address.longitude);
    setPhysicalAddressLatitude(address.latitude);
    setPhysicalAddressFormattedAddress(address.formattedAddress);
    setPhysicalAddressAddressLine1(address.addressLine1 ?? '');
    setPhysicalAddressAddressLine2(address.addressLine2 ?? '');
    setPhysicalAddressSuburb(address.suburb ?? '');
    setPhysicalAddressCity(address.city ?? '');
    setPhysicalAddressProvince(address.province ?? '');
    setPhysicalAddressZipcode(address.zipcode ?? '');
    setPhysicalAddressCountry(address.country ?? '');
    setPhysicalAddressCountryCode(address.countryCode ?? '');
  };

  const handlePhysicalAddressUpdateClick = ({ addressLine1, addressLine2, suburb, city, province, zipcode, countryCode }: PhysicalAddress) => {
    const organization = rootDataOrganization.organization;
    if (!organization) {
      return;
    }

    const countryData = getCountryData(countryCode as TCountryCode);
    let country = physicalAddressCountry;
    if (countryData) {
      country = countryData.name;
    }

    const physicalAddress = organization.physicalAddress;

    if (physicalAddress) {
      const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}' physical address...`} />, infoNotificationOptions);

      commitUpdateOrganizationPhysicalAddress({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: physicalAddress.id,
            osmType: physicalAddressOsmType,
            osmId: physicalAddressOsmId,
            placeId: physicalAddressPlaceId,
            longitude: physicalAddressLongitude,
            latitude: physicalAddressLatitude,
            formattedAddress: physicalAddressFormattedAddress,
            addressLine1,
            addressLine2,
            suburb,
            city,
            province,
            zipcode,
            country,
            countryCode,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to update organization '${organization?.name}' physical address. Error: ${joinErrors(errors)}.`} />,
            });

            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content={`Organization '${organization?.name}' physical address updated.`} />,
          });
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update organization '${organization?.name}' physical address. Error: ${error.message}.`} />,
          });
        },
        optimisticResponse: {
          updateOrganizationPhysicalAddress: {
            organization: {
              id: organization.id,
              physicalAddress: {
                id: physicalAddress.id,
                osmType: physicalAddressOsmType,
                osmId: physicalAddressOsmId,
                placeId: physicalAddressPlaceId,
                longitude: physicalAddressLongitude,
                latitude: physicalAddressLatitude,
                formattedAddress: physicalAddressFormattedAddress,
                addressLine1,
                addressLine2,
                suburb,
                city,
                province,
                zipcode,
                country,
                countryCode,
              },
            },
          },
        },
      });
    } else {
      const id = uuid();
      const toastId = themedToast(<NotificationContent content={`Adding organization '${organization.name}' physical address...`} />, infoNotificationOptions);

      commitAddOrganizationPhysicalAddress({
        variables: {
          input: {
            clientMutationId: uuid(),
            organizationCustomDomain,
            id,
            osmType: physicalAddressOsmType,
            osmId: physicalAddressOsmId,
            placeId: physicalAddressPlaceId,
            longitude: physicalAddressLongitude,
            latitude: physicalAddressLatitude,
            formattedAddress: physicalAddressFormattedAddress,
            addressLine1,
            addressLine2,
            suburb,
            city,
            province,
            zipcode,
            country,
            countryCode,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to add organization '${organization?.name}' physical address. Error: ${joinErrors(errors)}.`} />,
            });

            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content={`Organization '${organization?.name}' physical address added.`} />,
          });
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add organization '${organization?.name}' physical address. Error: ${error.message}.`} />,
          });
        },
        optimisticResponse: {
          addOrganizationPhysicalAddress: {
            organization: {
              id: organization.id,
              physicalAddress: {
                id,
                osmType: physicalAddressOsmType,
                osmId: physicalAddressOsmId,
                placeId: physicalAddressPlaceId,
                longitude: physicalAddressLongitude,
                latitude: physicalAddressLatitude,
                formattedAddress: physicalAddressFormattedAddress,
                addressLine1,
                addressLine2,
                suburb,
                city,
                province,
                zipcode,
                country,
                countryCode,
              },
            },
          },
        },
      });
    }
  };

  const handleEnableOrganizationSsoSettingsClick = ({ entityId, loginUrl, appFederationMetadataUrl }: ssoSettingsDetails) => {
    const organization = rootDataOrganization.organization;
    if (!organization) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}' SSO settings...`} />, infoNotificationOptions);

    commitUpdateOrganizationSsoSettings({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain: organization.customDomain,
          entityId,
          loginUrl,
          appFederationMetadataUrl,
          isActive: true,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update organization '${organization?.name}' SSO settings. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${organization?.name} SSO settings details updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update organization '${organization?.name}' SSO settings. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateOrganizationSsoSettings: {
          organization: {
            id: organization.id,
            ssoSettings: {
              id: organization.ssoSettings?.id ?? '',
              isActive: true,
              entityId,
              loginUrl,
              appFederationMetadataUrl,
            },
          },
        },
      },
    });
  };

  const handleEnableSsoChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSsoSettingsEnabled(event.target.checked);

    if (event.target.checked) {
      return;
    }

    const organization = rootDataOrganization.organization;
    if (!organization) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing organization '${organization.name}' SSO settings...`} />, infoNotificationOptions);

    commitRemoveOrganizationSsoSettings({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove organization '${organization?.name}' SSO settings. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${organization?.name} SSO settings removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove organization '${organization?.name}' SSO settings. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        removeOrganizationSsoSettings: {
          organization: {
            id: organization.id,
            ssoSettings: organization.ssoSettings
              ? {
                  id: organization.ssoSettings.id,
                  isActive: false,
                  entityId: organization.ssoSettings.entityId,
                  loginUrl: organization.ssoSettings.loginUrl,
                  appFederationMetadataUrl: organization.ssoSettings.appFederationMetadataUrl,
                }
              : null,
          },
        },
      },
    });
  };

  const handleEnableOrganizationTaxDetailsClick = ({ taxId, taxRatePercentage }: TaxDetails) => {
    const organization = rootDataOrganization.organization;
    if (!organization) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}' tax details...`} />, infoNotificationOptions);

    commitUpdateOrganizationTaxDetails({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain,
          taxId,
          taxRatePercentage: parseFloat(taxRatePercentage),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update organization '${organization?.name}' tax details. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${organization?.name} tax details details updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update organization '${organization?.name}' tax details. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateOrganizationTaxDetails: {
          organization: {
            id: organization.id,
            taxDetails: {
              id: '',
              taxId,
              taxRatePercentage,
            },
          },
        },
      },
    });
  };

  const handleEnableTaxDetailsChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setTaxDetailsEnabled(event.target.checked);

    if (event.target.checked) {
      return;
    }

    const organization = rootDataOrganization.organization;
    if (!organization) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing organization '${organization.name}' tax details...`} />, infoNotificationOptions);

    commitRemoveOrganizationTaxDetails({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove organization '${organization?.name}' tax details. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${organization?.name} tax details removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove organization '${organization?.name}' tax details. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        removeOrganizationTaxDetails: {
          organization: {
            id: organization.id,
            taxDetails: null,
          },
        },
      },
    });
  };

  const handleZonesSearchTextChange = (str: string) => {
    setZoneNameSearchText(str);

    handleRefetchZones(str);
  };

  const handleSelectedZonesChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedZones(newRowSelectionModel);
  };

  const handleZoneMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setZoneMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditZone:
        setIsEditZoneDialogOpen(true);
        break;

      case MoreActionsMenuOptionType.DeleteZone:
        handleRemoveZoneClick();
        break;
    }
  };

  const handleEditZoneClick = () => {
    setIsEditZoneDialogOpen(false);
  };

  const onEditZoneCancel = () => {
    setIsEditZoneDialogOpen(false);
  };

  const handleRemoveZonesClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing zones ..." />, infoNotificationOptions);

    commitDeleteZones({
      variables: {
        connectionIds: zonesConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: seledctedZones.ids
            .values()
            .map((id) => id as string)
            .toArray(),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove zones. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zones removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove zones. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveZoneClick = () => {
    if (!selectedZoneId) {
      return;
    }

    const toastId = themedToast(<NotificationContent content="Removing zone ..." />, infoNotificationOptions);

    commitDeleteZones({
      variables: {
        connectionIds: zonesConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: [selectedZoneId],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove zone. Error: ${joinErrors(errors)}.`} />,
          });
          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove zone. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleCustomTagsSearchTextChange = (str: string) => {
    setCustomTagNameSearchText(str);

    handleRefetchCustomTags(str);
  };

  const handleSelectedCustomTagsChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedCustomTags(newRowSelectionModel);
  };

  const handleCustomTagMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setCustomTagMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditCustomTag:
        setIsEditCustomTagDialogOpen(true);
        break;

      case MoreActionsMenuOptionType.DeleteCustomTag:
        handleRemoveCustomTagClick();
        break;
    }
  };

  const handleEditCustomTagClick = () => {
    setIsEditCustomTagDialogOpen(false);
  };

  const handleEditCustomTagCancel = () => {
    setIsEditCustomTagDialogOpen(false);
  };

  const handleRemoveCustomTagsClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing tags ..." />, infoNotificationOptions);

    commitDeleteCustomTags({
      variables: {
        connectionIds: customTagsConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: seledctedCustomTags.ids
            .values()
            .map((id) => id as string)
            .toArray(),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove tags. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Tags removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove tags. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveCustomTagClick = () => {
    if (!selectedCustomTagId) {
      return;
    }

    const toastId = themedToast(<NotificationContent content="Removing tag ..." />, infoNotificationOptions);

    commitDeleteZones({
      variables: {
        connectionIds: customTagsConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: [selectedCustomTagId],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove tag. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Tag removed.`} />,
        });

        setSelectedCustomTagId(null);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove tag. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleAddPaymentMethodClicked = () => {
    setIsAddPaymentMethodDialogOpen(true);
  };

  const handleAddPaymentMethodCancel = () => {
    setIsAddPaymentMethodDialogOpen(false);
  };

  const handleCloseClick = () => {
    router.push(getOrganizationBaseLink(integratedPlatrform, organizationCustomDomain));
  };

  const handleRemovePaymentMethodClick = (id: string) => {
    const toastId = themedToast(<NotificationContent content={`Removing payment method...`} />, infoNotificationOptions);

    commitRemoveOrganizationPaymentMethod({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove payment method. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Payment method removed.`} />,
        });

        handleRefetchOrganizationPaymentMethodsDetails();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove payment method. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleCancelActiveOfferingClick = () => {
    if (!rootDataOrganization.organization) {
      return;
    }

    const name = rootDataOrganization.organization.name;
    const toastId = themedToast(<NotificationContent content={`Cancelling organization '${name}' active offering...`} />, infoNotificationOptions);

    commitCancelOrganizationOffering({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to cancel organization '${name}' active offering. Error: ${joinErrors(errors)}.`} />,
          });

          onReloadRequired();

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization '${name}' active offering cancelled.`} />,
        });

        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to cancel organization '${name}' active offering. Error: ${error.message}.`} />,
        });

        onReloadRequired();
      },
    });
  };

  const handleUpgradeOfferingClick = (code: string) => {
    if (!rootDataOrganization.organization) {
      return;
    }

    const name = rootDataOrganization.organization.name;
    const toastId = themedToast(<NotificationContent content={`Updating organization '${name} active offering'...`} />, infoNotificationOptions);

    commitUpdateOrganizationOffering({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain,
          offeringCode: code,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update organization ${name} active offering. Error: ${joinErrors(errors)}.`} />,
          });

          onReloadRequired();

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${name} active offering updated.`} />,
        });

        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update organization ${name} active offering. Error: ${error.message}.`} />,
        });

        onReloadRequired();
      },
    });
  };

  const handleSetAsPreferredZoneClicked = (id: string) => {
    const organizationTagDetails = zones.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting zone '${organizationTagDetails.name}' as your preferred zone...`} />, infoNotificationOptions);

    commitAddCustomerPreferredOrganizationTag({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to set zone '${organizationTagDetails.name}' as your preferred zone. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone '${organizationTagDetails.name}' has been set as the preferred zone.`} />,
        });

        setPreferredZones(preferredZones.concat([organizationTagDetails.id]));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set zone '${organizationTagDetails.name}' as your preferred zone. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveAsPreferredZoneClicked = (id: string) => {
    const organizationTagDetails = zones.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing zone '${organizationTagDetails.name}' as your preferred zone...`} />, infoNotificationOptions);

    commitRemoveCustomerPreferredOrganizationTag({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the zone '${organizationTagDetails.name}' as your preferred zone. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone '${organizationTagDetails.name}' has been removed as your preferred zone.`} />,
        });

        setPreferredZones(preferredZones.filter((item) => item !== organizationTagDetails.id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the zone '${organizationTagDetails.name}' as your preferred zone. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleSetAsPreferredCustomTagClicked = (id: string) => {
    const organizationTagDetails = customTags.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting tag '${organizationTagDetails.name}' as your preferred tag...`} />, infoNotificationOptions);

    commitAddCustomerPreferredOrganizationTag({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to set tag '${organizationTagDetails.name}' as your preferred tag. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Tag '${organizationTagDetails.name}' has been set as the preferred tag.`} />,
        });

        setPreferredCustomTags(preferredCustomTags.concat([organizationTagDetails.id]));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set tag '${organizationTagDetails.name}' as your preferred tag. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveAsPreferredCustomTagClicked = (id: string) => {
    const organizationTagDetails = customTags.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing tag '${organizationTagDetails.name}' as your preferred tag...`} />, infoNotificationOptions);

    commitRemoveCustomerPreferredOrganizationTag({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the tag '${organizationTagDetails.name}' as your preferred tag. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Tag '${organizationTagDetails.name}' has been removed as your preferred tag.`} />,
        });

        setPreferredCustomTags(preferredCustomTags.filter((item) => item !== organizationTagDetails.id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the tag '${organizationTagDetails.name}' as your preferred tag. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveOrganizationClicked = () => {
    if (!rootDataOrganization.organization) {
      return;
    }

    const name = rootDataOrganization.organization.name;
    const toastId = themedToast(<NotificationContent content={`Removing organization '${name}'...`} />, infoNotificationOptions);

    commitDeleteOrganization({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: rootDataOrganization.organization.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the organization '${name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization '${name}' removed.`} />,
        });

        router.push(getRootLink(integratedPlatrform));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the organization '${name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  if (!rootDataOrganization.organization) {
    return null;
  }

  const zoneRows: ZoneRowType[] = zones.map((zone) => ({
    id: zone.id,
    name: zone.name,
    description: zone.description,
    preferred: preferredZones.includes(zone.id),
  }));

  const zoneColumns: GridColDef<(typeof zoneRows)[number]>[] = [
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => {
        const zone = zones.find((zone) => zone.id === (params.id as string));
        if (!zone) {
          return null;
        }

        return <Zone zone={zone} showFullName />;
      },
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'description',
      headerName: 'Description',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'preferred',
      headerName: 'Preferred?',
      editable: false,
      renderCell: (params) => {
        const id = params.id as string;
        if (params.value) {
          return (
            <IconButton onClick={() => handleRemoveAsPreferredZoneClicked(id)}>
              <PreferredIcon />
            </IconButton>
          );
        }

        return (
          <IconButton onClick={() => handleSetAsPreferredZoneClicked(id)}>
            <NotPreferredIcon />
          </IconButton>
        );
      },
      display: 'flex',
    },
    {
      field: 'More Actions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <Box sx={{ display: 'flex', justifyContent: 'flex-end', width: '100%' }}>
          <IconButton
            onClick={(event: React.MouseEvent<HTMLElement>) => {
              setSelectedZoneId(params.id as string);
              setZoneMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

  const customTagRows: CustomTagRowType[] = customTags.map((customTag) => ({
    id: customTag.id,
    name: customTag.name,
    description: customTag.description,
    preferred: preferredCustomTags.includes(customTag.id),
  }));

  const customTagColumns: GridColDef<(typeof customTagRows)[number]>[] = [
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => {
        const customTag = customTags.find((customTag) => customTag.id === (params.id as string));
        if (!customTag) {
          return null;
        }

        return <CustomTag customTag={customTag} showFullName />;
      },
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'description',
      headerName: 'Description',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'preferred',
      headerName: 'Preferred?',
      editable: false,
      renderCell: (params) => {
        const id = params.id as string;
        if (params.value) {
          return (
            <IconButton onClick={() => handleRemoveAsPreferredCustomTagClicked(id)}>
              <PreferredIcon />
            </IconButton>
          );
        }

        return (
          <IconButton onClick={() => handleSetAsPreferredCustomTagClicked(id)}>
            <NotPreferredIcon />
          </IconButton>
        );
      },
      display: 'flex',
    },
    {
      field: 'More Actions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <Box sx={{ display: 'flex', justifyContent: 'flex-end', width: '100%' }}>
          <IconButton
            onClick={(event: React.MouseEvent<HTMLElement>) => {
              setSelectedCustomTagId(params.id as string);
              setCustomTagMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

  const handleFeatureImageUploadCompleted = (response: FileUploadResponse) => {
    setFeatureImages((prev) => [response, ...prev]);
    setPrimaryFeatureImage((prevPrimary) => prevPrimary ?? response);
  };

  const handleRemoveFeatureImage = (image: FileUploadResponse) => {
    setFeatureImages((prev) => {
      const next = prev.filter((item) => item.original?.url !== image.original?.url);

      if (primaryFeatureImage?.original?.url === image.original?.url) {
        setPrimaryFeatureImage(next[0] ?? null);
      }

      return next;
    });
  };

  const handleSetPrimaryFeatureImage = (image: FileUploadResponse) => {
    setPrimaryFeatureImage(image);
    setFeatureImages((prev) => [image, ...prev.filter((item) => item.original?.url !== image.original?.url)]);
  };

  const organization = rootDataOrganization.organization;
  const paymentMethodExist = organization && organization.paymentMethods.length > 0;
  const activeOffering = organization ? organization.activeOffering : null;
  const availableOfferings = organization && organization.availableOfferings ? organization.availableOfferings : [];

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationAdminLeftSideNavigationMenuContent organizationCustomDomain={organizationCustomDomain} hideIcons />
        <Box sx={{ marginLeft: secondDrawerExpandedDrawerWidthPx, flexGrow: 1 }}>
          <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Organization Information">
            <Form
              onSubmit={handleOrganizationDetailUpdateClick}
              initialValues={{
                customDomain: organizationEditableCustomDomain,
                name: organizationName,
                about: organizationAbout,
                title: organizationTitle,
                subTitle: organizationSubTitle,
                website: organizationWebsite,
                customerFacingTermsAndConditionsUrl: organizationCustomerFacingTermsAndConditionsUrl,
                industrySubCategoryIds: organizationIndustrySubCategoryIds,
                contactEmail: organizationContactEmail,
                contactPhone: organizationContactPhone,
              }}
              validate={validateOrganizationDetails}
              render={({ handleSubmit, values }) => {
                debounceSetOrganizationEditableCustomDomain(values!.customDomain);
                debounceSetOrganizationName(values!.name);
                debounceSetOrganizationWebsite(values!.website);
                debounceSetOrganizationCustomerFacingTermsAndConditionsUrl(values!.customerFacingTermsAndConditionsUrl);
                debounceSetOrganizationIndustrySubCategoryIds(values!.industrySubCategoryIds);
                debounceSetOrganizationContactEmail(values!.contactEmail);
                debounceSetOrganizationContactPhone(values!.contactPhone);

                return (
                  <FormStackColumn onSubmit={handleSubmit}>
                    <StackColumn
                      sx={{
                        paddingLeft: defaultPadding,
                        paddingRight: defaultPadding,
                        paddingTop: defaultPadding,
                      }}
                      ref={(divElement) => {
                        sectionRefs.current['setup'] = divElement;
                      }}
                    >
                      <SectionIconTypography label="Organization Setup" />
                      <BodyIconTypography label="Edit your organization details" />
                      <Divider />
                    </StackColumn>

                    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                      <FormFieldLabel label="Feature Images">
                        <StackColumn>
                          <Box
                            sx={{
                              display: 'grid',
                              gridTemplateColumns: { xs: 'repeat(auto-fill, minmax(140px, 1fr))', sm: 'repeat(auto-fill, minmax(180px, 1fr))' },
                              gap: 2,
                            }}
                          >
                            {featureImages.map((image, index) => (
                              <Box
                                key={index}
                                sx={{
                                  position: 'relative',
                                  borderRadius: 2,
                                  overflow: 'hidden',
                                  border: 1,
                                  borderColor: 'divider',
                                  backgroundColor: paletteMode === 'dark' ? 'grey.900' : 'grey.50',
                                }}
                              >
                                {/* eslint-disable-next-line @next/next/no-img-element */}
                                <img src={image.original?.url ?? image.thumbnail?.url ?? ''} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                                <StackRow sx={{ position: 'absolute', top: 8, right: 8 }}>
                                  <IconButton size="small" aria-label="Remove feature image" onClick={() => handleRemoveFeatureImage(image)}>
                                    <DeleteIcon fontSize="small" />
                                  </IconButton>
                                </StackRow>
                                <StackRow sx={{ position: 'absolute', left: 8, bottom: 8 }}>
                                  {primaryFeatureImage?.original?.url === image.original?.url ? (
                                    <Chip size="small" color="success" label="Cover image" />
                                  ) : (
                                    <Button variant="contained" size="small" onClick={() => handleSetPrimaryFeatureImage(image)} sx={{ textTransform: 'none' }}>
                                      Make cover
                                    </Button>
                                  )}
                                </StackRow>
                              </Box>
                            ))}
                          </Box>

                          <ImageFileUploaderWithCropper onUploadCompleted={handleFeatureImageUploadCompleted} />
                        </StackColumn>
                      </FormFieldLabel>

                      <FormFieldLabel label="Name">
                        <TextField name="name" required={requiredOrganizationDetailsFields.name} />
                      </FormFieldLabel>

                      {rootData.me.emails.some((item) => !!rootData.emailsToShowLatestCapabilities.find((email) => email.toLocaleLowerCase() === item.toLocaleLowerCase())) && (
                        <FormFieldLabel label="Custom Domain" required={requiredOrganizationDetailsFields.customDomain}>
                          <TextField name="customDomain" required={requiredOrganizationDetailsFields.customDomain} />
                        </FormFieldLabel>
                      )}

                      <ListingMetadata
                        fields={['about', 'title', 'subTitle']}
                        requiredFields={requiredOrganizationDetailsFields}
                        onChange={({ about, title, subTitle }) => {
                          debounceSetOrganizationAbout(about);
                          debounceSetOrganizationTitle(title);
                          debounceSetOrganizationSubTitle(subTitle);
                        }}
                      />

                      <FormFieldLabel label="Website">
                        <TextField name="website" required={requiredOrganizationDetailsFields.about} helperText="https://" />
                      </FormFieldLabel>

                      <FormFieldLabel label="Terms and Conditions">
                        <TextField
                          name="customerFacingTermsAndConditionsUrl"
                          required={requiredOrganizationDetailsFields.customerFacingTermsAndConditionsUrl}
                          helperText={<HelperText text="Provide your company's official website so members can learn more or verify your organization." />}
                        />
                      </FormFieldLabel>

                      <FormFieldLabel label="Industry">
                        <OrganizationMultipleChoicesIndustries
                          rootDataRelay={rootData}
                          name="industrySubCategoryIds"
                          required={requiredOrganizationDetailsFields.industrySubCategoryIds}
                        />
                      </FormFieldLabel>

                      <SectionIconTypography label="Contact Details" />
                      <BodyIconTypography label="Edit your organization contact details" />
                      <Divider />

                      <FormFieldLabel label="Email">
                        <TextField name="contactEmail" required={requiredOrganizationDetailsFields.contactEmail} />
                      </FormFieldLabel>

                      <FormFieldLabel label="Phone Number">
                        <TextField name="contactPhone" required={requiredOrganizationDetailsFields.contactPhone} />
                      </FormFieldLabel>
                    </StackColumn>

                    <StackColumn
                      sx={{
                        paddingLeft: defaultPadding,
                        paddingRight: defaultPadding,
                        paddingTop: defaultPadding,
                      }}
                    >
                      <StackRow>
                        <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                          Update
                        </Button>
                      </StackRow>
                    </StackColumn>
                  </FormStackColumn>
                );
              }}
            />

            <Form
              onSubmit={handlePhysicalAddressUpdateClick}
              initialValues={{
                addressLine1: physicalAddressAddressLine1,
                addressLine2: physicalAddressAddressLine2,
                suburb: physicalAddressSuburb,
                city: physicalAddressCity,
                province: physicalAddressProvince,
                zipcode: physicalAddressZipcode,
                countryCode: physicalAddressCountryCode,
              }}
              validate={validatePhysicalAddress}
              render={({ handleSubmit, values, form }) => {
                debounceSetPhysicalAddressAddressLine1(values!.addressLine1);
                debounceSetPhysicalAddressAddressLine2(values!.addressLine2);
                debounceSetPhysicalAddressSuburb(values!.suburb);
                debounceSetPhysicalAddressCity(values!.city);
                debounceSetPhysicalAddressProvince(values!.province);
                debounceSetPhysicalAddressZipcode(values!.zipcode);
                debounceSetPhysicalAddressCountryCode(values!.countryCode);

                return (
                  <FormStackColumn onSubmit={handleSubmit}>
                    <StackColumn
                      sx={{
                        paddingLeft: defaultPadding,
                        paddingRight: defaultPadding,
                        paddingTop: defaultPadding,
                      }}
                      ref={(divElement) => {
                        sectionRefs.current['physical-address-setup'] = divElement;
                      }}
                    >
                      <SectionIconTypography label="Physical Address Setup" />
                      <BodyIconTypography label="Edit your organization physical address" />
                      <Divider />
                    </StackColumn>

                    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                      <PhysicalAddress
                        addressLine1Name="addressLine1"
                        addressLine1Required={requiredPhysicalAddressFields.addressLine1}
                        addressLine2Name="addressLine2"
                        addressLine2Required={requiredPhysicalAddressFields.addressLine2}
                        suburbName="suburb"
                        suburbRequired={requiredPhysicalAddressFields.suburb}
                        cityName="city"
                        cityRequired={requiredPhysicalAddressFields.city}
                        provinceName="province"
                        provinceRequired={requiredPhysicalAddressFields.province}
                        zipcodeName="zipcode"
                        zipcodeRequired={requiredPhysicalAddressFields.zipcode}
                        countryName="countryCode"
                        countryRequired={requiredPhysicalAddressFields.countryCode}
                        onSelect={(address) => {
                          handlePhysicalAddressSelect(address);
                          form.batch(() => {
                            form.change('addressLine1', address.addressLine1 ?? '');
                            form.change('addressLine2', address.addressLine2 ?? '');
                            form.change('suburb', address.suburb ?? '');
                            form.change('city', address.city ?? '');
                            form.change('province', address.province ?? '');
                            form.change('zipcode', address.zipcode ?? '');
                            form.change('countryCode', address.countryCode ?? '');
                          });
                        }}
                      />
                    </StackColumn>

                    <StackColumn
                      sx={{
                        paddingLeft: defaultPadding,
                        paddingRight: defaultPadding,
                        paddingTop: defaultPadding,
                      }}
                    >
                      <StackRow>
                        <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                          Update
                        </Button>
                      </StackRow>
                    </StackColumn>
                  </FormStackColumn>
                );
              }}
            />

            <Form
              onSubmit={handleBillingDetailUpdateClick}
              initialValues={{
                companyName: billingCompanyName,
                email: billingEmail,
                addressLine1: billingAddressLine1,
                addressLine2: billingAddressLine2,
                suburb: billingSuburb,
                city: billingCity,
                province: billingProvince,
                zipcode: billingZipcode,
                countryCode: billingCountryCode,
              }}
              validate={validateOrganizationBilling}
              render={({ handleSubmit, values, form }) => {
                debounceSetBillingCompanyName(values!.companyName);
                debounceSetBillingEmail(values!.email);
                debounceSetBillingAddressLine1(values!.addressLine1);
                debounceSetBillingAddressLine2(values!.addressLine2);
                debounceSetBillingSuburb(values!.suburb);
                debounceSetBillingCity(values!.city);
                debounceSetBillingProvince(values!.province);
                debounceSetBillingZipcode(values!.zipcode);
                debounceSetBillingCountryCode(values!.countryCode);

                return (
                  <FormStackColumn onSubmit={handleSubmit}>
                    <StackColumn
                      sx={{
                        paddingLeft: defaultPadding,
                        paddingRight: defaultPadding,
                        paddingTop: defaultPadding,
                      }}
                      ref={(divElement) => {
                        sectionRefs.current['billing-payment-setup'] = divElement;
                      }}
                    >
                      <SectionIconTypography label="Billing & Payment Setup" />
                      <BodyIconTypography label="Edit your organization billing and payment details" />
                      <Divider />
                    </StackColumn>

                    <StackColumn
                      sx={{
                        paddingLeft: defaultPadding,
                        paddingRight: defaultPadding,
                        paddingTop: defaultPadding,
                      }}
                    >
                      <FormFieldLabel label="Company name">
                        <TextField name="companyName" required={requiredBillingFields.companyName} />
                      </FormFieldLabel>

                      <FormFieldLabel label="Email">
                        <TextField name="email" required={requiredBillingFields.email} helperText="Email to send invoice to" />
                      </FormFieldLabel>

                      <PhysicalAddress
                        addressLine1Name="addressLine1"
                        addressLine1Required={requiredBillingFields.addressLine1}
                        addressLine2Name="addressLine2"
                        addressLine2Required={requiredBillingFields.addressLine2}
                        suburbName="suburb"
                        suburbRequired={requiredBillingFields.suburb}
                        cityName="city"
                        cityRequired={requiredBillingFields.city}
                        provinceName="province"
                        provinceRequired={requiredBillingFields.province}
                        zipcodeName="zipcode"
                        zipcodeRequired={requiredBillingFields.zipcode}
                        countryName="countryCode"
                        countryRequired={requiredBillingFields.countryCode}
                        onSelect={(address) => {
                          handleBillingAddressSelect(address);
                          form.batch(() => {
                            form.change('addressLine1', address.addressLine1 ?? '');
                            form.change('addressLine2', address.addressLine2 ?? '');
                            form.change('suburb', address.suburb ?? '');
                            form.change('city', address.city ?? '');
                            form.change('province', address.province ?? '');
                            form.change('zipcode', address.zipcode ?? '');
                            form.change('countryCode', address.countryCode ?? '');
                          });
                        }}
                      />
                    </StackColumn>

                    <StackColumn
                      sx={{
                        paddingLeft: defaultPadding,
                        paddingRight: defaultPadding,
                        paddingTop: defaultPadding,
                      }}
                    >
                      <StackRow>
                        <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                          Update
                        </Button>
                      </StackRow>
                    </StackColumn>
                  </FormStackColumn>
                );
              }}
            />

            <StackColumn
              sx={{
                paddingLeft: defaultPadding,
                paddingRight: defaultPadding,
                paddingTop: defaultPadding,
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Payment Method" />
                  <BodyIconTypography label="Edit your payment method" />
                </Grid>

                <Grid>
                  {!paymentMethodExist && (
                    <Button variant="text" onClick={handleAddPaymentMethodClicked} sx={{ textTransform: 'none' }}>
                      <LeadIconTypography label="Add Payment Method" endElement={<NewIcon fontSize="large" />} />
                    </Button>
                  )}
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            {paymentMethodExist && (
              <StackColumn
                sx={{
                  paddingLeft: defaultPadding,
                  paddingRight: defaultPadding,
                  paddingTop: defaultPadding,
                }}
              >
                <StackRow>
                  {organization.paymentMethods.map((item) => (
                    <StackColumn key={item.id}>
                      <CreditCard lastFourDigits={item.cardLastFourDigit} expiryDate={`${item.cardExpiryMonth}/${item.cardExpiryYear}`} cardBrand={item.cardBrand} />
                      <Button variant="contained" color="warning" onClick={() => handleRemovePaymentMethodClick(item.id)}>
                        <BodyIconTypography label="Remove Payment Method" invertDefaultColor={paletteMode === 'dark'} startElement={<DeleteIcon />} />
                      </Button>
                    </StackColumn>
                  ))}
                </StackRow>
              </StackColumn>
            )}

            {!paymentMethodExist && (
              <StackColumn
                sx={{
                  paddingLeft: defaultPadding,
                  paddingRight: defaultPadding,
                  paddingTop: defaultPadding,
                }}
              >
                <SmallIconTypography label="No payment method setup yet" />
              </StackColumn>
            )}

            <Form
              onSubmit={handleEnableOrganizationSsoSettingsClick}
              initialValues={{
                entityId: ssoSettingsEntityId,
                loginUrl: ssoSettingsLoginUrl,
                appFederationMetadataUrl: ssoSettingsAppFederationMetadataUrl,
              }}
              validate={validateSsoSettings}
              render={({ handleSubmit, values }) => {
                debounceSetSsoSettingsEntityId(values!.entityId);
                debounceSetSsoSettingsLoginUrl(values!.loginUrl);
                debounceSetSsoSettingsppFederationMetadataUrl(values!.appFederationMetadataUrl);

                return (
                  <FormStackColumn onSubmit={handleSubmit}>
                    <StackColumn
                      sx={{
                        paddingLeft: defaultPadding,
                        paddingRight: defaultPadding,
                        paddingTop: defaultPadding,
                      }}
                      ref={(divElement) => {
                        sectionRefs.current['sso-setup'] = divElement;
                      }}
                    >
                      <SectionIconTypography label="SSO Setup" />
                      <BodyIconTypography label="Edit your organization SSO settings" />
                      <Divider />
                    </StackColumn>

                    <StackColumn
                      sx={{
                        paddingLeft: defaultPadding,
                        paddingRight: defaultPadding,
                        paddingTop: defaultPadding,
                      }}
                    >
                      <FormFieldLabel label="Enable Sign sign-on">
                        <Switch defaultChecked={ssoSettingsEnabled} onChange={handleEnableSsoChange} />
                      </FormFieldLabel>

                      {ssoSettingsEnabled && (
                        <>
                          <FormFieldLabel label="Entity Id">
                            <TextField name="entityId" required={requiredSsoSettingsFields.entityId} />
                          </FormFieldLabel>

                          <FormFieldLabel label="Login Url">
                            <TextField name="loginUrl" required={requiredSsoSettingsFields.loginUrl} />
                          </FormFieldLabel>

                          <FormFieldLabel label="App Federation Metadata Url">
                            <TextField name="appFederationMetadataUrl" required={requiredSsoSettingsFields.appFederationMetadataUrl} />
                          </FormFieldLabel>
                        </>
                      )}
                    </StackColumn>

                    {ssoSettingsEnabled && (
                      <StackColumn
                        sx={{
                          paddingLeft: defaultPadding,
                          paddingRight: defaultPadding,
                          paddingTop: defaultPadding,
                        }}
                      >
                        <StackRow>
                          <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                            Update
                          </Button>
                        </StackRow>
                      </StackColumn>
                    )}
                  </FormStackColumn>
                );
              }}
            />

            <Form
              onSubmit={handleEnableOrganizationTaxDetailsClick}
              initialValues={{
                taxId: taxDetailsTaxId,
                taxRatePercentage: taxDetailsTaxRatePercentage,
              }}
              validate={validateTaxDetails}
              render={({ handleSubmit, values }) => {
                debounceSetTaxDetailsTaxId(values!.taxId);
                debounceSetTaxDetailsTaxRatePercentage(values!.taxRatePercentage);

                return (
                  <FormStackColumn onSubmit={handleSubmit}>
                    <StackColumn
                      sx={{
                        paddingLeft: defaultPadding,
                        paddingRight: defaultPadding,
                        paddingTop: defaultPadding,
                      }}
                      ref={(divElement) => {
                        sectionRefs.current['tax-details-setup'] = divElement;
                      }}
                    >
                      <SectionIconTypography label="Tax Details Setup" />
                      <BodyIconTypography label="Edit your organization tax details" />
                      <Divider />
                    </StackColumn>

                    <StackColumn
                      sx={{
                        paddingLeft: defaultPadding,
                        paddingRight: defaultPadding,
                        paddingTop: defaultPadding,
                      }}
                    >
                      <FormFieldLabel label="Is this business registered for tax (GST/VAT)?">
                        <Switch defaultChecked={taxDetailsEnabled} onChange={handleEnableTaxDetailsChange} />
                      </FormFieldLabel>

                      {taxDetailsEnabled && (
                        <>
                          <FormFieldLabel label="Tax ID / VAT / GST Number">
                            <TextField name="taxId" required={requiredTaxDetailsFields.taxId} />
                          </FormFieldLabel>

                          <FormFieldLabel label="Tax Rate (%)">
                            <TextField name="taxRatePercentage" required={requiredTaxDetailsFields.taxRatePercentage} />
                          </FormFieldLabel>
                        </>
                      )}
                    </StackColumn>

                    {taxDetailsEnabled && (
                      <StackColumn
                        sx={{
                          paddingLeft: defaultPadding,
                          paddingRight: defaultPadding,
                          paddingTop: defaultPadding,
                        }}
                      >
                        <StackRow>
                          <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                            Update
                          </Button>
                        </StackRow>
                      </StackColumn>
                    )}
                  </FormStackColumn>
                );
              }}
            />

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['zones-setup'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Zones Setup" />
                  <BodyIconTypography label="Edit your organization zones details" />
                </Grid>

                <Grid>
                  <AddOrganizationZoneButton organizationCustomDomain={organizationCustomDomain} connectionIds={zonesConnectionIds} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <PushToRight />
              <Search size="small" placeholder="Search for zones" defaultValue={zoneNameSearchText} onChange={handleZonesSearchTextChange} />
            </GridContainer>

            {seledctedZones.ids.size > 0 && (
              <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
                <Box
                  sx={{
                    backgroundColor: 'white',
                    padding: defaultGridActionPadding,
                    border: 1,
                    borderColor: (theme) => theme.palette.divider,
                    borderRadius: 2,
                    flexGrow: 1,
                  }}
                >
                  <StackRow sx={{ alignItems: 'center' }}>
                    <SmallIconTypography label={`${seledctedZones.ids.size} records selected`} />
                    <PushToRight />
                    <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveZonesClick} sx={{ textTransform: 'none' }}>
                      Remove Zone
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}

            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedZones}
                onRowSelectionModelChange={handleSelectedZonesChanged}
                rows={zoneRows}
                columns={zoneColumns}
                hideFooterPagination={zoneRows.length <= 10}
                initialState={{
                  pagination: {
                    rowCount: zoneRows.length,
                    paginationModel: {
                      pageSize: 10,
                    },
                  },
                }}
                pageSizeOptions={[10]}
                ignoreDiacritics
                disableRowSelectionOnClick
                getRowHeight={() => 'auto'}
                rowSpacingType="margin"
                getRowSpacing={() => ({ top: 3, bottom: 3 })}
                sx={defaultGridStyle}
                localeText={{ noRowsLabel: 'No zone found' }}
              />
            </StackRow>

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['tags-setup'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Tags Setup" />
                  <BodyIconTypography label="Edit your organization tags details" />
                </Grid>

                <Grid>
                  <AddOrganizationCustomTagButton organizationCustomDomain={organizationCustomDomain} connectionIds={customTagsConnectionIds} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <PushToRight />
              <Search size="small" placeholder="Search for tags" defaultValue={customTagNameSearchText} onChange={handleCustomTagsSearchTextChange} />
            </GridContainer>

            {seledctedCustomTags.ids.size > 0 && (
              <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
                <Box
                  sx={{
                    backgroundColor: 'white',
                    padding: defaultGridActionPadding,
                    border: 1,
                    borderColor: (theme) => theme.palette.divider,
                    borderRadius: 2,
                    flexGrow: 1,
                  }}
                >
                  <StackRow sx={{ alignItems: 'center' }}>
                    <SmallIconTypography label={`${seledctedCustomTags.ids.size} records selected`} />
                    <PushToRight />
                    <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveCustomTagsClick} sx={{ textTransform: 'none' }}>
                      Remove Tag
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}

            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedCustomTags}
                onRowSelectionModelChange={handleSelectedCustomTagsChanged}
                rows={customTagRows}
                columns={customTagColumns}
                hideFooterPagination={customTagRows.length <= 10}
                initialState={{
                  pagination: {
                    rowCount: customTagRows.length,
                    paginationModel: {
                      pageSize: 10,
                    },
                  },
                }}
                pageSizeOptions={[10]}
                ignoreDiacritics
                disableRowSelectionOnClick
                getRowHeight={() => 'auto'}
                rowSpacingType="margin"
                getRowSpacing={() => ({ top: 3, bottom: 3 })}
                sx={defaultGridStyle}
                localeText={{ noRowsLabel: 'No tag found' }}
              />
            </StackRow>

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['subscriptions'] = divElement;
              }}
            >
              <SectionIconTypography label="Subscriptions" />
              <Divider />
            </StackColumn>

            <GridContainer sx={{ padding: defaultPadding, justifyContent: 'center', alignItems: 'stretch' }}>
              {activeOffering && (
                <Grid>
                  <Card sx={{ width: { xs: '100%', sm: 300 }, height: '100%', backgroundColor: 'white' }}>
                    <CardContent sx={{ marginLeft: 1 }}>
                      <BodyIconTypography label={activeOffering.name} sx={{ color: coal }} />
                      <StackRow spacing={0.5} sx={{ marginTop: -2 }}>
                        <ExtraLargeHeadingIconTypography label={(activeOffering.unitPrice / 100).toFixed(0)} sx={{ paddingTop: 4, color: coal }} />
                        <BodyIconTypography label="$" sx={{ color: coal }} />
                      </StackRow>

                      <List sx={{ padding: 0 }}>
                        <Box sx={{ marginTop: 2, marginBottom: 4 }}>
                          {activeOffering.underPriceLines.map((item, index) => (
                            <ListItem key={index} alignItems="flex-start" sx={{ padding: 0 }}>
                              <ListItemText>
                                <SmallIconTypography label={item} sx={{ color: coal }} />
                              </ListItemText>
                            </ListItem>
                          ))}
                        </Box>

                        {activeOffering.featureSet.map((item, index) => (
                          <ListItem key={index} alignItems="flex-start" sx={{ padding: 0 }}>
                            <ListItemIcon sx={{ minWidth: 'auto', marginRight: 1 }}>
                              <TickIcon fontSize="small" sx={{ color: activeOffering.isEnterprise ? coal : emerald }} />
                            </ListItemIcon>
                            <ListItemText>
                              <SmallIconTypography label={item} sx={{ color: coal }} />
                            </ListItemText>
                          </ListItem>
                        ))}
                      </List>

                      <CardActions sx={{ justifyContent: 'center' }}>
                        {!activeOffering.free && (
                          <Button color="secondary" variant="contained" onClick={handleCancelActiveOfferingClick} sx={defaultButtonStyle}>
                            Cancel
                          </Button>
                        )}
                      </CardActions>
                    </CardContent>
                  </Card>
                </Grid>
              )}

              {availableOfferings.map((availableOffering) => (
                <Grid key={availableOffering.code}>
                  <Card sx={{ width: { xs: '100%', sm: 300 }, height: '100%', backgroundColor: 'white' }}>
                    <CardContent sx={{ marginLeft: 1 }}>
                      <BodyIconTypography label={availableOffering.name} sx={{ color: coal }} />
                      <StackRow spacing={0.5} sx={{ marginTop: -2 }}>
                        {availableOffering.unitPrice > 0 && (
                          <ExtraLargeHeadingIconTypography label={(availableOffering.unitPrice / 100).toFixed(0)} sx={{ paddingTop: 4, color: coal }} />
                        )}
                        {availableOffering.isEnterprise && (
                          <ExtraLargeHeadingIconTypography
                            label="TBC"
                            sx={{
                              paddingTop: 4,
                              color: coal,
                            }}
                          />
                        )}
                        <BodyIconTypography label="$" sx={{ color: coal }} />
                      </StackRow>

                      <List sx={{ padding: 0 }}>
                        <Box sx={{ marginTop: 2, marginBottom: 4 }}>
                          {availableOffering.underPriceLines.map((item, index) => (
                            <ListItem key={index} alignItems="flex-start" sx={{ padding: 0 }}>
                              <ListItemText>
                                <SmallIconTypography label={item} sx={{ color: coal }} />
                              </ListItemText>
                            </ListItem>
                          ))}
                        </Box>

                        {availableOffering.featureSet.map((item, index) => (
                          <ListItem key={index} alignItems="flex-start" sx={{ padding: 0 }}>
                            <ListItemIcon sx={{ minWidth: 'auto', marginRight: 1 }}>
                              <TickIcon fontSize="small" sx={{ color: availableOffering.isEnterprise ? coal : emerald }} />
                            </ListItemIcon>
                            <ListItemText>
                              <SmallIconTypography label={item} sx={{ color: coal }} />
                            </ListItemText>
                          </ListItem>
                        ))}

                        {!organization?.hasAttachedPaymentMethod && (
                          <ListItem alignItems="flex-start" sx={{ padding: 0, paddingTop: 1 }}>
                            <ListItemIcon sx={{ minWidth: 'auto', marginRight: 1 }}>
                              <ErrorIcon fontSize="large" sx={{ color: 'red' }} />
                            </ListItemIcon>
                            <ListItemText>
                              <SmallIconTypography label="You need to have payment method setup in order to upgrade to this offering." color="red" />
                            </ListItemText>
                          </ListItem>
                        )}
                      </List>
                    </CardContent>

                    <CardActions sx={{ justifyContent: 'center' }}>
                      {!organization?.hasAttachedPaymentMethod && (
                        <Button variant="contained" onClick={handleAddPaymentMethodClicked} sx={{ textTransform: 'none', color: 'white' }}>
                          Add Payment Method
                        </Button>
                      )}

                      {organization?.hasAttachedPaymentMethod && !availableOffering.isEnterprise && (
                        <Button
                          color="primary"
                          variant="contained"
                          onClick={() => handleUpgradeOfferingClick(availableOffering.code)}
                          sx={{ textTransform: 'none', color: 'white' }}
                        >
                          Upgrade
                        </Button>
                      )}

                      {organization?.hasAttachedPaymentMethod && availableOffering.isEnterprise && (
                        <Button
                          href="mailto:support@getskedular.com"
                          variant="contained"
                          sx={{
                            textTransform: 'none',
                            backgroundColor: 'black',
                            color: 'white',
                          }}
                        >
                          Contact Us
                        </Button>
                      )}
                    </CardActions>
                  </Card>
                </Grid>
              ))}
            </GridContainer>

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['manage-organization'] = divElement;
              }}
            >
              <SectionIconTypography label="Manage" />
              <BodyIconTypography label="Remove your organization" />
              <Divider />
            </StackColumn>

            <StackRow
              sx={{
                paddingLeft: defaultPadding,
                paddingRight: defaultPadding,
                paddingTop: defaultPadding,
              }}
            >
              <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveOrganizationClicked} sx={{ textTransform: 'none' }}>
                Remove Organization
              </Button>
            </StackRow>
          </AppBarWithStackColumn>
        </Box>
      </Box>

      <MoreActionsMenu anchorEl={zoneMoreActionsAnchorEl} open={zoneMoreActionsMenuOpen} onMenuItemClick={handleZoneMoreActionsMenuItemClick} options={zoneMoreActionsOption} />

      {selectedZoneId && (
        <EditOrganizationZoneDialog
          onReloadRequired={onReloadRequired}
          zoneId={selectedZoneId}
          isDialogOpen={isEditZoneDialogOpen}
          onAddClicked={handleEditZoneClick}
          onCancel={onEditZoneCancel}
        />
      )}

      <MoreActionsMenu
        anchorEl={customTagMoreActionsAnchorEl}
        open={customTagMoreActionsMenuOpen}
        onMenuItemClick={handleCustomTagMoreActionsMenuItemClick}
        options={customTagMoreActionsOption}
      />

      {selectedCustomTagId && (
        <EditOrganizationCustomTagDialog
          onReloadRequired={onReloadRequired}
          customTagId={selectedCustomTagId}
          isDialogOpen={isEditCustomTagDialogOpen}
          onAddClicked={handleEditCustomTagClick}
          onCancel={handleEditCustomTagCancel}
        />
      )}

      {!paymentMethodExist && isAddPaymentMethodDialogOpen && (
        <AddOrganizationPaymentMethodDialog
          organizationCustomDomain={organizationCustomDomain}
          isDialogOpen={isAddPaymentMethodDialogOpen}
          onCancel={handleAddPaymentMethodCancel}
        />
      )}
    </>
  );
};

export default memo(OrganizationAdmin);

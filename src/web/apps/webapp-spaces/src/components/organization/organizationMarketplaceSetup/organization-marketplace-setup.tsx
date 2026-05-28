import { NewBankAccountButton } from '@/components/bankAccount/addBankAccount';
import { BodyIconTypography, FormFieldLabel, FormStackColumn, GridContainer, PushToRight, SectionIconTypography, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { BillingIcon, DeleteIcon } from '@/components/icons';
import { getOrganizationAdminEditProductTagBaseLink, getOrganizationBankAccountBaseLink, getOrganizationStripeConnectAccountBaseLink } from '@/components/links';
import { ListingMetadata, listingMetadataSchemaShape } from '@/components/listingMetadata';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { SingleChoiceOrganizationBillingCycle, SingleChoiceOrganizationXeroBillingMode } from '@/components/organization';
import { AddOrganizationProductTagButton } from '@/components/organization/addOrganizationProductTag';
import OrganizationAdminTagManagementList from '@/components/organization/organizationAdmin/organization-admin-tag-management-list';
import OrganizationMarketplaceBankAccountManagementList from '@/components/organization/organizationMarketplaceSetup/organization-marketplace-bank-account-management-list';
import OrganizationMarketplaceSetupSectionNav, {
  OrganizationMarketplaceSetupSection,
} from '@/components/organization/organizationMarketplaceSetup/organization-marketplace-setup-section-nav';
import OrganizationMarketplaceStripeConnectAccountManagementList from '@/components/organization/organizationMarketplaceSetup/organization-marketplace-stripe-connect-account-management-list';
import { ProductTag } from '@/components/productTag';
import { Search } from '@/components/search';
import { ExistingStripeConnectAccountButton, NewStripeConnectAccountButton } from '@/components/stripeConnectAccount/addStripeConnectAccount';
import { PaletteModeContext, useIntegratedPlatrform } from '@skedular/shared';
import { defaultButtonStyle, defaultPadding } from '@skedular/ui';
import { getRelayErrorMessage } from '@skedular/shared';
import type { organizationMarketplaceSetup_deleteOrganizationBankAccountsMutation } from '@/queries/__generated__/organizationMarketplaceSetup_deleteOrganizationBankAccountsMutation.graphql';
import type { organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation } from '@/queries/__generated__/organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation.graphql';
import type { organizationMarketplaceSetup_deleteProductTagsMutation } from '@/queries/__generated__/organizationMarketplaceSetup_deleteProductTagsMutation.graphql';
import type { organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation } from '@/queries/__generated__/organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation.graphql';
import type { organizationMarketplaceSetup_organizationBankAccounts_query$key } from '@/queries/__generated__/organizationMarketplaceSetup_organizationBankAccounts_query.graphql';
import type { organizationMarketplaceSetup_organizationBankAccounts_refetchableFragment } from '@/queries/__generated__/organizationMarketplaceSetup_organizationBankAccounts_refetchableFragment.graphql';
import type { organizationMarketplaceSetup_organizationStripeConnectAccounts_query$key } from '@/queries/__generated__/organizationMarketplaceSetup_organizationStripeConnectAccounts_query.graphql';
import type { organizationMarketplaceSetup_organizationStripeConnectAccounts_refetchableFragment } from '@/queries/__generated__/organizationMarketplaceSetup_organizationStripeConnectAccounts_refetchableFragment.graphql';
import type { organizationMarketplaceSetup_productTags_query$key } from '@/queries/__generated__/organizationMarketplaceSetup_productTags_query.graphql';
import type { organizationMarketplaceSetup_productTags_refetchableFragment } from '@/queries/__generated__/organizationMarketplaceSetup_productTags_refetchableFragment.graphql';
import type { organizationMarketplaceSetup_query$key } from '@/queries/__generated__/organizationMarketplaceSetup_query.graphql';
import type { organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation } from '@/queries/__generated__/organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation.graphql';
import type { organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation } from '@/queries/__generated__/organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation.graphql';
import type {
  OrganizationBillingCycle,
  OrganizationPatchField,
  organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation,
} from '@/queries/__generated__/organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation.graphql';
import type { organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation } from '@/queries/__generated__/organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation.graphql';
import type {
  OrganizationXeroConnectionPatchField,
  organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation,
  OrganizationXeroBillingMode,
} from '@/queries/__generated__/organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import { PageHeaderPanel } from '@skedular/ui';
import type { TCountryCode } from 'countries-list';
import { getCountryData } from 'countries-list';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { number, object, string } from 'yup';

type Props = {
  rootDataRelay: organizationMarketplaceSetup_query$key;
  rootDataProductTagsRelay: organizationMarketplaceSetup_productTags_query$key;
  rootDataOrganizationStripeConnectAccountsRelay: organizationMarketplaceSetup_organizationStripeConnectAccounts_query$key;
  rootDataOrganizationBankAccountsRelay: organizationMarketplaceSetup_organizationBankAccounts_query$key;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  embedded?: boolean;
};

const inlinePatchDebounceTimeout = 1000;

type OrganizationMarketplaceListingMetadataDetails = {
  title: string | null;
  subTitle: string | null;
};

const organizationMarketplaceListingMetadataSchema = object({
  ...listingMetadataSchemaShape,
});

type OrganizationBillingSettingsDetails = {
  billingCycle: string;
  invoiceDueInDays: number;
};

type OrganizationXeroConnectionDetails = {
  id?: string;
  tenantId: string;
  tenantName: string;
  billingMode: OrganizationXeroBillingMode;
  scopes?: string | null;
  isActive: boolean;
  defaultSalesAccountCode?: string | null;
  defaultTrackingCategory1?: string | null;
  defaultTrackingCategory2?: string | null;
  defaultBrandingThemeId?: string | null;
  defaultReferencePrefix?: string | null;
  lastSuccessfulSyncAt?: string | null;
  lastError?: string | null;
  hasAccessToken?: boolean;
  hasRefreshToken?: boolean;
};

type XeroTenantOption = {
  tenantId: string;
  tenantName: string;
};

const xeroBillingModeLabels: Record<string, string> = {
  DISABLED: 'Disabled',
  ENABLED: 'Enabled',
  REPEATING_INVOICES: 'Repeating Invoices',
};

const xeroBillingModeGuidance: Record<string, string> = {
  DISABLED: 'Skedular stays on the local invoice flow and does not export invoices into Xero.',
  ENABLED: 'Skedular exports supported invoices into Xero as normal invoices. Customers can still review and pay each invoice separately.',
  REPEATING_INVOICES: 'Recurring bookings create a Xero repeating invoice template for supported cadences. Xero then manages the scheduled follow-up invoices from that template.',
};

const organizationBillingSettingsSchema = object({
  billingCycle: string().required('Billing cycle is required'),
  invoiceDueInDays: number()
    .transform((value, originalValue) => {
      return originalValue === '' || originalValue === null || originalValue === undefined ? NaN : Number(originalValue);
    })
    .typeError('Invoice due days is required')
    .required('Invoice due days is required')
    .integer('Invoice due days must be between 1 and 999.')
    .min(1, 'Invoice due days must be between 1 and 999.')
    .max(999, 'Invoice due days must be between 1 and 999.'),
});

const getActiveSection = (value: string | null): OrganizationMarketplaceSetupSection => {
  switch (value) {
    case 'billing-cycle':
      return 'billing-cycle';
    case 'xero-setup':
      return 'xero-setup';
    case 'stripe-connect-accounts-setup':
      return 'stripe-connect-accounts-setup';
    case 'bank-accounts-setup':
      return 'bank-accounts-setup';
    case 'product-tags-setup':
      return 'product-tags-setup';
    case 'marketplace-listing':
    default:
      return 'marketplace-listing';
  }
};

const formColumnSx = {
  width: '100%',
  maxWidth: 760,
};

const getCountryName = (countryCode: string | null | undefined) => {
  if (!countryCode) {
    return 'N/A';
  }

  const countryData = getCountryData(countryCode as TCountryCode);

  return countryData?.name ?? countryCode;
};

const OrganizationMarketplaceSetup = ({
  rootDataRelay,
  rootDataProductTagsRelay,
  rootDataOrganizationStripeConnectAccountsRelay,
  rootDataOrganizationBankAccountsRelay,
  onReloadRequired,
  organizationCustomDomain,
  embedded = false,
}: Props) => {
  const rootData = useFragment<organizationMarketplaceSetup_query$key>(
    graphql`
      fragment organizationMarketplaceSetup_query on Query {
        organization(customDomain: $organizationCustomDomain) {
          id
          name
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
          marketplaceListingMetadata {
            about
            title
            subTitle
            includedFeatures
          }
          billingCycle {
            type
            name
          }
          invoiceDueInDays
          xeroConnection {
            id
            tenantId
            tenantName
            billingMode
            scopes
            isActive
            sendInvoicesViaXero
            autoReconcilePayments
            defaultSalesAccountCode
            defaultReceivablesAccountCode
            defaultTrackingCategory1
            defaultTrackingCategory2
            defaultBrandingThemeId
            defaultReferencePrefix
            lastSuccessfulSyncAt
            lastError
            hasAccessToken
            hasRefreshToken
          }
        }
        ...existingStripeConnectAccountButton_query
        ...singleChoiceOrganizationBillingCycle_query
        ...singleChoiceOrganizationXeroBillingMode_query
      }
    `,
    rootDataRelay,
  );

  const [rootDataProductTags, refetchProductTags] = useRefetchableFragment<
    organizationMarketplaceSetup_productTags_refetchableFragment,
    organizationMarketplaceSetup_productTags_query$key
  >(
    graphql`
      fragment organizationMarketplaceSetup_productTags_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationMarketplaceSetup_productTags_refetchableFragment") {
        organization(customDomain: $organizationCustomDomain) {
          productTags(first: $count, after: $cursor, where: { nameContains: $productTagNameSearchText }, orderBy: [{ direction: ASCENDING, field: NAME }])
            @connection(key: "organizationMarketplaceSetup_productTags") {
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
    rootDataProductTagsRelay,
  );

  const [rootDataOrganizationStripeConnectAccounts, refetchOrganizationStripeConnectAccounts] = useRefetchableFragment<
    organizationMarketplaceSetup_organizationStripeConnectAccounts_refetchableFragment,
    organizationMarketplaceSetup_organizationStripeConnectAccounts_query$key
  >(
    graphql`
      fragment organizationMarketplaceSetup_organizationStripeConnectAccounts_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationMarketplaceSetup_organizationStripeConnectAccounts_refetchableFragment") {
        organizationStripeConnectAccounts(
          first: $count
          after: $cursor
          where: { organizationCustomDomain: $organizationCustomDomain, nameContains: $organizationStripeConnectAccountNameSearchText }
          orderBy: [{ direction: ASCENDING, field: NAME }]
        ) @connection(key: "organizationMarketplaceSetup_organizationStripeConnectAccounts") {
          __id
          totalCount
          edges {
            node {
              id
              isDefault
              name
              country
              defaultCurrency
              businessType
              companyName
              url
              supportUrl
              contactEmail
              contactPhone
              onboardingUrl
              chargesEnabled
              payoutsEnabled
              detailsSubmitted
              isAuthorized
              isOnboardingCompleted
              organization {
                customDomain
              }
            }
          }
        }
      }
    `,
    rootDataOrganizationStripeConnectAccountsRelay,
  );

  const [rootDataOrganizationBankAccounts, refetchOrganizationBankAccounts] = useRefetchableFragment<
    organizationMarketplaceSetup_organizationBankAccounts_refetchableFragment,
    organizationMarketplaceSetup_organizationBankAccounts_query$key
  >(
    graphql`
      fragment organizationMarketplaceSetup_organizationBankAccounts_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationMarketplaceSetup_organizationBankAccounts_refetchableFragment") {
        organizationBankAccounts(
          first: $count
          after: $cursor
          where: { organizationCustomDomain: $organizationCustomDomain, nameContains: $organizationBankAccountNameSearchText }
          orderBy: [{ direction: ASCENDING, field: NAME }]
        ) @connection(key: "organizationMarketplaceSetup_organizationBankAccounts") {
          __id
          totalCount
          edges {
            node {
              id
              isDefault
              name
              bankName
              accountHolderName
              accountNumber
              country
              organization {
                customDomain
              }
            }
          }
        }
      }
    `,
    rootDataOrganizationBankAccountsRelay,
  );

  const [commitDeleteProductTags] = useMutation<organizationMarketplaceSetup_deleteProductTagsMutation>(graphql`
    mutation organizationMarketplaceSetup_deleteProductTagsMutation($connectionIds: [ID!]!, $input: DeleteProductTagsInput!) {
      deleteProductTags(input: $input) {
        organizationTags {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitDeleteOrganizationStripeConnectAccounts] = useMutation<organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation>(graphql`
    mutation organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation($connectionIds: [ID!]!, $input: DeleteOrganizationStripeConnectAccountsInput!) {
      deleteOrganizationStripeConnectAccounts(input: $input) {
        organizationStripeConnectAccounts {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitSetOrganizationStripeConnectAccountAsDefault] = useMutation<organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation>(graphql`
    mutation organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation($input: SetOrganizationStripeConnectAccountAsDefaultInput!) @raw_response_type {
      setOrganizationStripeConnectAccountAsDefault(input: $input) {
        organizationStripeConnectAccount {
          id
          isDefault
        }
      }
    }
  `);

  const [commitDeleteOrganizationBankAccounts] = useMutation<organizationMarketplaceSetup_deleteOrganizationBankAccountsMutation>(graphql`
    mutation organizationMarketplaceSetup_deleteOrganizationBankAccountsMutation($connectionIds: [ID!]!, $input: DeleteOrganizationBankAccountsInput!) {
      deleteOrganizationBankAccounts(input: $input) {
        organizationBankAccounts {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitSetOrganizationBankAccountAsDefault] = useMutation<organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation>(graphql`
    mutation organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation($input: SetOrganizationBankAccountAsDefaultInput!) @raw_response_type {
      setOrganizationBankAccountAsDefault(input: $input) {
        organizationBankAccount {
          id
          isDefault
        }
      }
    }
  `);

  const [commitUpdateOrganizationPatchMarketplaceListingMetadata] = useMutation<organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation>(graphql`
    mutation organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation($input: UpdateOrganizationInput!) @raw_response_type {
      updateOrganization(input: $input) {
        organization {
          id
          marketplaceListingMetadata {
            about
            title
            subTitle
            includedFeatures
          }
        }
      }
    }
  `);

  const [commitUpdateOrganizationPatchBillingSettings] = useMutation<organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation>(graphql`
    mutation organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation($input: UpdateOrganizationInput!) @raw_response_type {
      updateOrganization(input: $input) {
        organization {
          id
          billingCycle {
            type
            name
          }
          invoiceDueInDays
        }
      }
    }
  `);

  const [commitUpdateOrganizationXeroConnectionPatch] = useMutation<organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation>(graphql`
    mutation organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation($input: UpdateOrganizationXeroConnectionInput!) @raw_response_type {
      updateOrganizationXeroConnection(input: $input) {
        organization {
          id
          xeroConnection {
            id
            tenantId
            tenantName
            billingMode
            scopes
            isActive
            sendInvoicesViaXero
            autoReconcilePayments
            defaultSalesAccountCode
            defaultReceivablesAccountCode
            defaultTrackingCategory1
            defaultTrackingCategory2
            defaultBrandingThemeId
            defaultReferencePrefix
            lastSuccessfulSyncAt
            lastError
            hasAccessToken
            hasRefreshToken
          }
        }
      }
    }
  `);

  const [commitDisconnectOrganizationXeroConnection] = useMutation<organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation>(graphql`
    mutation organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation($input: DisconnectOrganizationXeroConnectionInput!) @raw_response_type {
      disconnectOrganizationXeroConnection(input: $input) {
        organization {
          id
          xeroConnection {
            id
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
  const pathname = usePathname();
  const section = searchParams.get('section');
  const activeSection = getActiveSection(section);
  const [stickyTop, setStickyTop] = useState(0);
  const xeroSuggestedTenantId = searchParams.get('xeroSuggestedTenantId') ?? '';
  const xeroSuggestedTenantName = searchParams.get('xeroSuggestedTenantName') ?? '';
  const xeroMessage = searchParams.get('xeroMessage');

  const validateOrganizationMarketplaceListingMetadataDetails = makeValidate(organizationMarketplaceListingMetadataSchema);
  const requiredOrganizationMarketplaceListingMetadataDetailsFields = makeRequired(organizationMarketplaceListingMetadataSchema);

  const validateOrganizationBillingSettingsDetails = makeValidate(organizationBillingSettingsSchema);
  const requiredOrganizationBillingSettingsDetailsFields = makeRequired(organizationBillingSettingsSchema);

  const organization = rootData.organization as
    | (NonNullable<typeof rootData.organization> & {
        xeroConnection?: OrganizationXeroConnectionDetails | null;
      })
    | null
    | undefined;
  const draftOrganizationBillingSettings = useRef<OrganizationBillingSettingsDetails>({
    billingCycle: organization?.billingCycle.type ?? '',
    invoiceDueInDays: organization?.invoiceDueInDays ?? 7,
  });
  useEffect(() => {
    draftOrganizationBillingSettings.current = {
      billingCycle: organization?.billingCycle.type ?? '',
      invoiceDueInDays: organization?.invoiceDueInDays ?? 7,
    };
  }, [organization?.billingCycle.type, organization?.invoiceDueInDays]);
  const existingXeroConnection = organization?.xeroConnection;
  const draftOrganizationXeroConnection = useRef<OrganizationXeroConnectionDetails>({
    tenantId: existingXeroConnection?.tenantId ?? '',
    tenantName: existingXeroConnection?.tenantName ?? '',
    billingMode: existingXeroConnection?.billingMode ?? 'DISABLED',
    isActive: existingXeroConnection?.isActive ?? false,
    defaultSalesAccountCode: existingXeroConnection?.defaultSalesAccountCode ?? '',
    defaultTrackingCategory1: existingXeroConnection?.defaultTrackingCategory1 ?? '',
    defaultTrackingCategory2: existingXeroConnection?.defaultTrackingCategory2 ?? '',
    defaultBrandingThemeId: existingXeroConnection?.defaultBrandingThemeId ?? '',
    defaultReferencePrefix: existingXeroConnection?.defaultReferencePrefix ?? '',
  });
  useEffect(() => {
    draftOrganizationXeroConnection.current = {
      tenantId: existingXeroConnection?.tenantId ?? '',
      tenantName: existingXeroConnection?.tenantName ?? '',
      billingMode: existingXeroConnection?.billingMode ?? 'DISABLED',
      isActive: existingXeroConnection?.isActive ?? false,
      defaultSalesAccountCode: existingXeroConnection?.defaultSalesAccountCode ?? '',
      defaultTrackingCategory1: existingXeroConnection?.defaultTrackingCategory1 ?? '',
      defaultTrackingCategory2: existingXeroConnection?.defaultTrackingCategory2 ?? '',
      defaultBrandingThemeId: existingXeroConnection?.defaultBrandingThemeId ?? '',
      defaultReferencePrefix: existingXeroConnection?.defaultReferencePrefix ?? '',
    };
  }, [
    existingXeroConnection?.billingMode,
    existingXeroConnection?.defaultBrandingThemeId,
    existingXeroConnection?.defaultReferencePrefix,
    existingXeroConnection?.defaultSalesAccountCode,
    existingXeroConnection?.defaultTrackingCategory1,
    existingXeroConnection?.defaultTrackingCategory2,
    existingXeroConnection?.isActive,
    existingXeroConnection?.tenantId,
    existingXeroConnection?.tenantName,
  ]);
  const xeroTenantOptions = useMemo<XeroTenantOption[]>(() => {
    const rawValue = searchParams.get('xeroTenantOptions');
    if (!rawValue) {
      return [];
    }

    try {
      const decodedValue = typeof window === 'undefined' ? '' : window.atob(rawValue);
      return decodedValue ? (JSON.parse(decodedValue) as XeroTenantOption[]) : [];
    } catch {
      return [];
    }
  }, [searchParams]);

  const clearTransientXeroQueryParams = useCallback(() => {
    const nextSearchParams = new URLSearchParams(searchParams.toString());
    nextSearchParams.delete('xeroTenantOptions');
    nextSearchParams.delete('xeroSuggestedTenantId');
    nextSearchParams.delete('xeroSuggestedTenantName');
    nextSearchParams.delete('xeroMessage');

    const nextSearch = nextSearchParams.toString();
    router.replace(nextSearch ? `${pathname}?${nextSearch}` : pathname);
  }, [pathname, router, searchParams]);

  const [productTagNameSearchText, setProductTagNameSearchText] = useState<string>('');
  const [selectedProductTagIds, setSelectedProductTagIds] = useState<string[]>([]);
  const [selectedProductTagId, setSelectedProductTagId] = useState<null | string>(null);
  const [productTagMoreActionsAnchorEl, setProductTagMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const productTagMoreActionsMenuOpen = Boolean(productTagMoreActionsAnchorEl);
  const productTags = useMemo(
    () => (rootDataProductTags.organization ? rootDataProductTags.organization.productTags.edges.map(({ node }) => node) : []),
    [rootDataProductTags.organization],
  );
  const productTagsConnectionIds = useMemo(() => (rootDataProductTags.organization ? [rootDataProductTags.organization.productTags.__id] : []), [rootDataProductTags.organization]);
  const productTagMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditProductTag],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteProductTag],
  ];

  const handleRefetchProductTags = useCallback(
    (productTagNameSearchText: string) => {
      startTransition(() => {
        refetchProductTags(
          {
            productTagNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [startTransition, refetchProductTags],
  );

  const [organizationStripeConnectAccountNameSearchText, setOrganizationStripeConnectAccountNameSearchText] = useState<string>('');
  const [selectedOrganizationStripeConnectAccountIds, setSelectedOrganizationStripeConnectAccountIds] = useState<string[]>([]);
  const [selectedOrganizationStripeConnectAccountId, setSelectedOrganizationStripeConnectAccountId] = useState<null | string>(null);
  const [organizationStripeConnectAccountMoreActionsAnchorEl, setOrganizationStripeConnectAccountMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const organizationStripeConnectAccountMoreActionsMenuOpen = Boolean(organizationStripeConnectAccountMoreActionsAnchorEl);
  const organizationStripeConnectAccounts = useMemo(
    () => rootDataOrganizationStripeConnectAccounts.organizationStripeConnectAccounts.edges.map(({ node }) => node),
    [rootDataOrganizationStripeConnectAccounts.organizationStripeConnectAccounts],
  );
  const organizationStripeConnectAccountsConnectionIds = useMemo(
    () => [rootDataOrganizationStripeConnectAccounts.organizationStripeConnectAccounts.__id],
    [rootDataOrganizationStripeConnectAccounts.organizationStripeConnectAccounts],
  );
  const organizationStripeConnectAccountMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditOrganizationStripeConnectAccount],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.SetOrganizationStripeConnectAccountAsDefault],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteOrganizationStripeConnectAccount],
  ];

  const organizationStripeConnectAccountDetails = useMemo(
    () => organizationStripeConnectAccounts.find((item) => item.id === selectedOrganizationStripeConnectAccountId),
    [selectedOrganizationStripeConnectAccountId, organizationStripeConnectAccounts],
  );

  const handleRefetchOrganizationStripeConnectAccounts = useCallback(
    (organizationStripeConnectAccountNameSearchText: string) => {
      startTransition(() => {
        refetchOrganizationStripeConnectAccounts(
          {
            organizationStripeConnectAccountNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [startTransition, refetchOrganizationStripeConnectAccounts],
  );

  const [organizationBankAccountNameSearchText, setOrganizationBankAccountNameSearchText] = useState<string>('');
  const [selectedOrganizationBankAccountIds, setSelectedOrganizationBankAccountIds] = useState<string[]>([]);
  const [selectedOrganizationBankAccountId, setSelectedOrganizationBankAccountId] = useState<null | string>(null);
  const [organizationBankAccountMoreActionsAnchorEl, setOrganizationBankAccountMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const organizationBankAccountMoreActionsMenuOpen = Boolean(organizationBankAccountMoreActionsAnchorEl);
  const organizationBankAccounts = useMemo(
    () => rootDataOrganizationBankAccounts.organizationBankAccounts.edges.map(({ node }) => node),
    [rootDataOrganizationBankAccounts.organizationBankAccounts],
  );
  const organizationBankAccountsConnectionIds = useMemo(
    () => [rootDataOrganizationBankAccounts.organizationBankAccounts.__id],
    [rootDataOrganizationBankAccounts.organizationBankAccounts],
  );
  const organizationBankAccountMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditOrganizationBankAccount],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.SetOrganizationBankAccountAsDefault],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteOrganizationBankAccount],
  ];

  const organizationBankAccountDetails = useMemo(
    () => organizationBankAccounts.find((item) => item.id === selectedOrganizationBankAccountId),
    [selectedOrganizationBankAccountId, organizationBankAccounts],
  );

  const handleRefetchOrganizationBankAccounts = useCallback(
    (organizationBankAccountNameSearchText: string) => {
      startTransition(() => {
        refetchOrganizationBankAccounts(
          {
            organizationBankAccountNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [startTransition, refetchOrganizationBankAccounts],
  );

  useEffect(() => {
    const updateStickyTop = () => {
      setStickyTop(document.querySelector('.app-bar')?.clientHeight ?? 0);
    };

    updateStickyTop();
    window.addEventListener('resize', updateStickyTop);

    return () => {
      window.removeEventListener('resize', updateStickyTop);
    };
  }, []);

  const handleProductTagsSearchTextChange = (str: string) => {
    setProductTagNameSearchText(str);

    handleRefetchProductTags(str);
  };

  const handleSelectedProductTagsChanged = (productTagId: string) => {
    setSelectedProductTagIds((current) => (current.includes(productTagId) ? current.filter((id) => id !== productTagId) : current.concat(productTagId)));
  };

  const handleProductTagMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setProductTagMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditProductTag:
        if (selectedProductTagId) {
          const currentQuery = searchParams.toString();
          const redirectUrl = currentQuery ? `${pathname}?${currentQuery}` : pathname;
          router.push(getOrganizationAdminEditProductTagBaseLink(integratedPlatrform, organizationCustomDomain, selectedProductTagId, { redirectUrl }));
        }
        break;

      case MoreActionsMenuOptionType.DeleteProductTag:
        handleRemoveProductTagClick();
        break;
    }
  };

  const handleRemoveProductTagsClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing product tags ..." />, infoNotificationOptions);

    commitDeleteProductTags({
      variables: {
        connectionIds: productTagsConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: selectedProductTagIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove product tags. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Product tags removed.`} />,
        });
        setSelectedProductTagIds([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove product tags. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveProductTagClick = () => {
    if (!selectedProductTagId) {
      return;
    }

    const toastId = themedToast(<NotificationContent content="Removing product tag ..." />, infoNotificationOptions);

    commitDeleteProductTags({
      variables: {
        connectionIds: productTagsConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: [selectedProductTagId],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove product tag. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Product tag removed.`} />,
        });

        setSelectedProductTagId(null);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove product tag. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleOrganizationStripeConnectAccountsSearchTextChange = (str: string) => {
    setOrganizationStripeConnectAccountNameSearchText(str);

    handleRefetchOrganizationStripeConnectAccounts(str);
  };

  const handleSelectedOrganizationStripeConnectAccountsChanged = (accountId: string) => {
    setSelectedOrganizationStripeConnectAccountIds((current) => (current.includes(accountId) ? current.filter((id) => id !== accountId) : current.concat(accountId)));
  };

  const handleOrganizationStripeConnectAccountMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setOrganizationStripeConnectAccountMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditOrganizationStripeConnectAccount:
        if (!organizationStripeConnectAccountDetails) {
          return;
        }

        router.push(
          getOrganizationStripeConnectAccountBaseLink(
            integratedPlatrform,
            organizationStripeConnectAccountDetails.organization!.customDomain!,
            organizationStripeConnectAccountDetails.id,
          ),
        );
        break;

      case MoreActionsMenuOptionType.SetOrganizationStripeConnectAccountAsDefault:
        handleSetOrganizationStripeConnectAccountAsDefault();
        break;

      case MoreActionsMenuOptionType.DeleteOrganizationStripeConnectAccount:
        handleRemoveOrganizationStripeConnectAccountClick();
        break;
    }
  };

  const handleSetOrganizationStripeConnectAccountAsDefault = () => {
    if (!organizationStripeConnectAccountDetails) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Setting stripe connect account ${organizationStripeConnectAccountDetails.name} as default...`} />,
      infoNotificationOptions,
    );

    commitSetOrganizationStripeConnectAccountAsDefault({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: organizationStripeConnectAccountDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to set stripe connect account ${organizationStripeConnectAccountDetails.name} as default. Error: ${getRelayErrorMessage(errors)}`}
              />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Stripe connect account ${organizationStripeConnectAccountDetails.name} is set as default.`} />,
        });

        handleRefetchOrganizationStripeConnectAccounts(organizationStripeConnectAccountNameSearchText);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set stripe connect account ${organizationStripeConnectAccountDetails.name} as default. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        setOrganizationStripeConnectAccountAsDefault: {
          organizationStripeConnectAccount: {
            id: organizationStripeConnectAccountDetails.id,
            isDefault: true,
          },
        },
      },
    });
  };

  const handleRemoveOrganizationStripeConnectAccountsClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing Stripe Connect accounts ..." />, infoNotificationOptions);

    commitDeleteOrganizationStripeConnectAccounts({
      variables: {
        connectionIds: organizationStripeConnectAccountsConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: selectedOrganizationStripeConnectAccountIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove Stripe Connect accounts. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Stripe Connect accounts removed.`} />,
        });
        setSelectedOrganizationStripeConnectAccountIds([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove Stripe Connect accounts. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveOrganizationStripeConnectAccountClick = () => {
    if (!selectedOrganizationStripeConnectAccountId) {
      return;
    }

    const toastId = themedToast(<NotificationContent content="Removing Stripe Connect account ..." />, infoNotificationOptions);

    commitDeleteOrganizationStripeConnectAccounts({
      variables: {
        connectionIds: organizationStripeConnectAccountsConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: [selectedOrganizationStripeConnectAccountId],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove Stripe Connect account. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Stripe Connect account removed.`} />,
        });

        setSelectedOrganizationStripeConnectAccountId(null);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove Stripe Connect account. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleOrganizationBankAccountsSearchTextChange = (str: string) => {
    setOrganizationBankAccountNameSearchText(str);

    handleRefetchOrganizationBankAccounts(str);
  };

  const commitOrganizationXeroConnectionPatch = (values: OrganizationXeroConnectionDetails) => {
    if (!organization) {
      return;
    }

    const fieldsToUpdate: OrganizationXeroConnectionPatchField[] = [];
    const tenantId = values.tenantId ?? existingXeroConnection?.tenantId ?? xeroSuggestedTenantId ?? '';
    const tenantName = values.tenantName ?? existingXeroConnection?.tenantName ?? xeroSuggestedTenantName;
    const billingMode = values.billingMode ?? 'DISABLED';
    const defaultSalesAccountCode = values.defaultSalesAccountCode ?? null;
    const defaultTrackingCategory1 = values.defaultTrackingCategory1 ?? null;
    const defaultTrackingCategory2 = values.defaultTrackingCategory2 ?? null;
    const defaultBrandingThemeId = values.defaultBrandingThemeId ?? null;
    const defaultReferencePrefix = values.defaultReferencePrefix ?? null;

    if (tenantId !== (existingXeroConnection?.tenantId ?? '')) {
      fieldsToUpdate.push('TENANT_ID');
    }
    if (tenantName !== (existingXeroConnection?.tenantName ?? '')) {
      fieldsToUpdate.push('TENANT_NAME');
    }
    if (billingMode !== (existingXeroConnection?.billingMode ?? 'DISABLED')) {
      fieldsToUpdate.push('BILLING_MODE');
    }
    if (defaultSalesAccountCode !== (existingXeroConnection?.defaultSalesAccountCode ?? null)) {
      fieldsToUpdate.push('DEFAULT_SALES_ACCOUNT_CODE');
    }
    if (defaultTrackingCategory1 !== (existingXeroConnection?.defaultTrackingCategory1 ?? null)) {
      fieldsToUpdate.push('DEFAULT_TRACKING_CATEGORY1');
    }
    if (defaultTrackingCategory2 !== (existingXeroConnection?.defaultTrackingCategory2 ?? null)) {
      fieldsToUpdate.push('DEFAULT_TRACKING_CATEGORY2');
    }
    if (defaultBrandingThemeId !== (existingXeroConnection?.defaultBrandingThemeId ?? null)) {
      fieldsToUpdate.push('DEFAULT_BRANDING_THEME_ID');
    }
    if (defaultReferencePrefix !== (existingXeroConnection?.defaultReferencePrefix ?? null)) {
      fieldsToUpdate.push('DEFAULT_REFERENCE_PREFIX');
    }
    if (fieldsToUpdate.length === 0) {
      clearTransientXeroQueryParams();
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating Xero settings for ${organization.name}...`} />, infoNotificationOptions);

    commitUpdateOrganizationXeroConnectionPatch({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationId: organization.id,
          organizationCustomDomain,
          fieldsToUpdate,
          tenantId,
          tenantName,
          billingMode,
          defaultSalesAccountCode,
          defaultTrackingCategory1,
          defaultTrackingCategory2,
          defaultBrandingThemeId,
          defaultReferencePrefix,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update Xero settings. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Xero settings updated for ${organization.name}.`} />,
        });
        clearTransientXeroQueryParams();
        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update Xero settings. Error: ${error.message}.`} />,
        });
      },
    });
  };
  const debounceUpdateOrganizationXeroConnection = useDebounceCallback(commitOrganizationXeroConnectionPatch, inlinePatchDebounceTimeout);

  const handleDisconnectOrganizationXeroConnectionClick = () => {
    if (!organization) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Disconnecting Xero from ${organization.name}...`} />, infoNotificationOptions);

    commitDisconnectOrganizationXeroConnection({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationId: organization.id,
          organizationCustomDomain,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to disconnect Xero. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Xero disconnected from ${organization.name}.`} />,
        });
        clearTransientXeroQueryParams();
        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to disconnect Xero. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleSelectedOrganizationBankAccountsChanged = (accountId: string) => {
    setSelectedOrganizationBankAccountIds((current) => (current.includes(accountId) ? current.filter((id) => id !== accountId) : current.concat(accountId)));
  };

  const handleOrganizationBankAccountMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setOrganizationBankAccountMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditOrganizationBankAccount:
        if (!organizationBankAccountDetails) {
          return;
        }

        router.push(getOrganizationBankAccountBaseLink(integratedPlatrform, organizationBankAccountDetails.organization!.customDomain!, organizationBankAccountDetails.id));
        break;

      case MoreActionsMenuOptionType.SetOrganizationBankAccountAsDefault:
        handleSetOrganizationBankAccountAsDefault();
        break;

      case MoreActionsMenuOptionType.DeleteOrganizationBankAccount:
        handleRemoveOrganizationBankAccountClick();
        break;
    }
  };

  const handleSetOrganizationBankAccountAsDefault = () => {
    if (!organizationBankAccountDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting bank account ${organizationBankAccountDetails.name} as default...`} />, infoNotificationOptions);

    commitSetOrganizationBankAccountAsDefault({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: organizationBankAccountDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to set bank account ${organizationBankAccountDetails.name} as default. Error: ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Bank account ${organizationBankAccountDetails.name} is set as default.`} />,
        });

        handleRefetchOrganizationBankAccounts(organizationBankAccountNameSearchText);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set bank account ${organizationBankAccountDetails.name} as default. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        setOrganizationBankAccountAsDefault: {
          organizationBankAccount: {
            id: organizationBankAccountDetails.id,
            isDefault: true,
          },
        },
      },
    });
  };

  const handleRemoveOrganizationBankAccountsClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing Bank accounts ..." />, infoNotificationOptions);

    commitDeleteOrganizationBankAccounts({
      variables: {
        connectionIds: organizationBankAccountsConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: selectedOrganizationBankAccountIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove Bank accounts. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Bank accounts removed.`} />,
        });
        setSelectedOrganizationBankAccountIds([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove Bank accounts. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveOrganizationBankAccountClick = () => {
    if (!selectedOrganizationBankAccountId) {
      return;
    }

    const toastId = themedToast(<NotificationContent content="Removing Bank accounts ..." />, infoNotificationOptions);

    commitDeleteOrganizationBankAccounts({
      variables: {
        connectionIds: organizationBankAccountsConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: [selectedOrganizationBankAccountId],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove Bank accounts. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Bank accounts removed.`} />,
        });

        setSelectedOrganizationBankAccountId(null);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove Bank accounts. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const productTagItems = useMemo(() => productTags.map((productTag) => ({ id: productTag.id, name: productTag.name, description: productTag.description })), [productTags]);

  const organizationStripeConnectAccountItems = useMemo(
    () =>
      organizationStripeConnectAccounts.map((account) => ({
        id: account.id,
        name: account.name,
        companyName: account.companyName,
        country: getCountryName(account.country),
        defaultCurrency: account.defaultCurrency,
        businessType: account.businessType,
        website: account.url,
        supportLink: account.supportUrl,
        contactEmail: account.contactEmail,
        contactPhone: account.contactPhone,
        chargesEnabled: account.chargesEnabled,
        payoutsEnabled: account.payoutsEnabled,
        detailsSubmitted: account.detailsSubmitted,
        isAuthorized: account.isAuthorized,
        isDefault: account.isDefault,
        requiresOnboarding: !account.isOnboardingCompleted,
        onboardingUrl: account.onboardingUrl,
      })),
    [organizationStripeConnectAccounts],
  );

  const organizationBankAccountItems = useMemo(
    () =>
      organizationBankAccounts.map((account) => ({
        id: account.id,
        name: account.name,
        accountHolderName: account.accountHolderName,
        accountNumber: account.accountNumber,
        bankName: account.bankName,
        country: getCountryName(account.country),
        isDefault: account.isDefault,
      })),
    [organizationBankAccounts],
  );

  const commitOrganizationMarketplaceListingMetadataPatch = useCallback(
    ({ title, subTitle }: OrganizationMarketplaceListingMetadataDetails) => {
      if (!organization || (organization.marketplaceListingMetadata.title === title && organization.marketplaceListingMetadata.subTitle === subTitle)) {
        return;
      }

      const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}' marketplace listing...`} />, infoNotificationOptions);

      commitUpdateOrganizationPatchMarketplaceListingMetadata({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: organization.id,
            fieldsToUpdate: ['MARKETPLACE_LISTING_METADATA'],
            marketplaceListingMetadata: {
              about: organization.marketplaceListingMetadata.about ?? '',
              title: title ?? '',
              subTitle: subTitle ?? '',
              includedFeatures: organization.marketplaceListingMetadata.includedFeatures ?? [],
            },
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to update organization '${organization?.name}' marketplace listing. Error: ${getRelayErrorMessage(errors)}.`} />,
            });

            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content={`Organization ${organization?.name} marketplace listing updated.`} />,
          });
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update organization '${organization?.name}' marketplace listing. Error: ${error.message}.`} />,
          });
        },
        optimisticResponse: {
          updateOrganization: {
            organization: {
              id: organization.id,
              marketplaceListingMetadata: {
                about: organization.marketplaceListingMetadata.about ?? '',
                title: title ?? '',
                subTitle: subTitle ?? '',
                includedFeatures: organization.marketplaceListingMetadata.includedFeatures ?? [],
              },
            },
          },
        },
      });
    },
    [commitUpdateOrganizationPatchMarketplaceListingMetadata, organization, themedToast],
  );
  const debounceUpdateOrganizationMarketplaceListingMetadata = useDebounceCallback(commitOrganizationMarketplaceListingMetadataPatch, inlinePatchDebounceTimeout);

  const commitOrganizationBillingSettingsPatch = useCallback(
    async ({ billingCycle, invoiceDueInDays }: OrganizationBillingSettingsDetails) => {
      if (!organization) {
        return;
      }

      const normalizedBillingCycle = billingCycle as OrganizationBillingCycle;
      const normalizedInvoiceDueInDays = Number(invoiceDueInDays);
      try {
        await organizationBillingSettingsSchema.validate({ billingCycle: normalizedBillingCycle, invoiceDueInDays: normalizedInvoiceDueInDays });
      } catch {
        return;
      }

      const fieldsToUpdate: OrganizationPatchField[] = [];
      if (normalizedBillingCycle !== organization.billingCycle.type) {
        fieldsToUpdate.push('BILLING_CYCLE');
      }
      if (normalizedInvoiceDueInDays !== organization.invoiceDueInDays) {
        fieldsToUpdate.push('INVOICE_DUE_IN_DAYS');
      }
      if (fieldsToUpdate.length === 0) {
        return;
      }

      const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}' billing settings...`} />, infoNotificationOptions);

      commitUpdateOrganizationPatchBillingSettings({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: organization.id,
            fieldsToUpdate,
            ...(fieldsToUpdate.includes('BILLING_CYCLE') ? { billingCycle: normalizedBillingCycle } : {}),
            ...(fieldsToUpdate.includes('INVOICE_DUE_IN_DAYS') ? { invoiceDueInDays: normalizedInvoiceDueInDays } : {}),
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to update organization '${organization?.name}' billing settings. Error: ${getRelayErrorMessage(errors)}.`} />,
            });

            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content={`Organization ${organization?.name} billing settings updated.`} />,
          });
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update organization '${organization?.name}' billing settings. Error: ${error.message}.`} />,
          });
        },
        optimisticResponse: {
          updateOrganization: {
            organization: {
              id: organization.id,
              billingCycle: {
                ...organization.billingCycle,
                type: normalizedBillingCycle,
              },
              invoiceDueInDays: normalizedInvoiceDueInDays,
            },
          },
        },
      });
    },
    [commitUpdateOrganizationPatchBillingSettings, organization, themedToast],
  );
  const debounceUpdateOrganizationBillingSettings = useDebounceCallback(commitOrganizationBillingSettingsPatch, inlinePatchDebounceTimeout);

  const xeroBillingModeLabel = xeroBillingModeLabels[existingXeroConnection?.billingMode ?? 'DISABLED'] ?? existingXeroConnection?.billingMode ?? 'Disabled';
  const xeroSummaryTenantName = existingXeroConnection?.isActive ? existingXeroConnection.tenantName : '';
  const xeroSummarySuffix = `in ${xeroBillingModeLabel} mode`;
  const xeroAuthorizeUrl = organization ? `/api/v1/organization/xero/oauth/start?organizationId=${organization.id}` : undefined;
  const isTenantLocked = !!existingXeroConnection?.hasRefreshToken && !!existingXeroConnection?.tenantId;
  const hasSuggestedTenant = !!xeroSuggestedTenantId;

  const handleApplySuggestedXeroTenantClick = useCallback(
    (tenantOption: XeroTenantOption) => {
      const nextSearchParams = new URLSearchParams(searchParams.toString());
      nextSearchParams.set('section', 'xero-setup');
      nextSearchParams.set('xeroSuggestedTenantId', tenantOption.tenantId);
      nextSearchParams.set('xeroSuggestedTenantName', tenantOption.tenantName);
      router.replace(`${pathname}?${nextSearchParams.toString()}`);
    },
    [pathname, router, searchParams],
  );

  const renderActiveSection = () => {
    switch (activeSection) {
      case 'billing-cycle':
        return (
          <Form
            onSubmit={() => undefined}
            initialValues={{
              billingCycle: organization?.billingCycle.type ?? '',
              invoiceDueInDays: organization?.invoiceDueInDays ?? 7,
            }}
            validate={validateOrganizationBillingSettingsDetails}
            render={({ handleSubmit, values }) => {
              const nextInvoiceDueInDays = typeof values?.invoiceDueInDays === 'number' ? values.invoiceDueInDays : Number(values?.invoiceDueInDays ?? NaN);
              if (values?.billingCycle && !Number.isNaN(nextInvoiceDueInDays)) {
                const nextBillingSettings = {
                  billingCycle: values.billingCycle,
                  invoiceDueInDays: nextInvoiceDueInDays,
                };
                if (
                  nextBillingSettings.billingCycle !== draftOrganizationBillingSettings.current.billingCycle ||
                  nextBillingSettings.invoiceDueInDays !== draftOrganizationBillingSettings.current.invoiceDueInDays
                ) {
                  draftOrganizationBillingSettings.current = nextBillingSettings;
                  debounceUpdateOrganizationBillingSettings(nextBillingSettings);
                }
              }

              return (
                <FormStackColumn onSubmit={handleSubmit} sx={formColumnSx}>
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <SectionIconTypography label="Organization Billing Settings" />
                    <BodyIconTypography label="Edit your organization billing cycle and default invoice payment terms." />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Billing Cycle">
                      <SingleChoiceOrganizationBillingCycle rootDataRelay={rootData} name="billingCycle" required={requiredOrganizationBillingSettingsDetailsFields.billingCycle} />
                    </FormFieldLabel>
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding, paddingBottom: defaultPadding }}>
                    <FormFieldLabel label="Invoice Due Days">
                      <TextField
                        name="invoiceDueInDays"
                        type="number"
                        required={requiredOrganizationBillingSettingsDetailsFields.invoiceDueInDays}
                        helperText="How many days customers have to pay marketplace invoices by default."
                      />
                    </FormFieldLabel>
                  </StackColumn>
                </FormStackColumn>
              );
            }}
          />
        );
      case 'xero-setup':
        return (
          <Form
            key={`xero-form-${existingXeroConnection?.id ?? 'new'}-${xeroSuggestedTenantId}-${xeroSuggestedTenantName}`}
            onSubmit={() => undefined}
            initialValues={{
              tenantId: existingXeroConnection?.tenantId ?? xeroSuggestedTenantId,
              tenantName: existingXeroConnection?.tenantName ?? xeroSuggestedTenantName,
              billingMode: existingXeroConnection?.billingMode ?? 'DISABLED',
              scopes: existingXeroConnection?.scopes ?? '',
              isActive: existingXeroConnection?.isActive ?? false,
              defaultSalesAccountCode: existingXeroConnection?.defaultSalesAccountCode ?? '',
              defaultTrackingCategory1: existingXeroConnection?.defaultTrackingCategory1 ?? '',
              defaultTrackingCategory2: existingXeroConnection?.defaultTrackingCategory2 ?? '',
              defaultBrandingThemeId: existingXeroConnection?.defaultBrandingThemeId ?? '',
              defaultReferencePrefix: existingXeroConnection?.defaultReferencePrefix ?? '',
            }}
            render={({ handleSubmit, values }) => {
              const selectedXeroBillingMode = values?.billingMode ?? existingXeroConnection?.billingMode ?? 'DISABLED';
              const selectedXeroBillingModeGuidance = xeroBillingModeGuidance[selectedXeroBillingMode] ?? xeroBillingModeGuidance.DISABLED;
              const nextXeroConnection = {
                tenantId: values?.tenantId ?? existingXeroConnection?.tenantId ?? xeroSuggestedTenantId ?? '',
                tenantName: values?.tenantName ?? existingXeroConnection?.tenantName ?? xeroSuggestedTenantName,
                billingMode: selectedXeroBillingMode,
                isActive: existingXeroConnection?.isActive ?? false,
                defaultSalesAccountCode: values?.defaultSalesAccountCode ?? '',
                defaultTrackingCategory1: values?.defaultTrackingCategory1 ?? '',
                defaultTrackingCategory2: values?.defaultTrackingCategory2 ?? '',
                defaultBrandingThemeId: values?.defaultBrandingThemeId ?? '',
                defaultReferencePrefix: values?.defaultReferencePrefix ?? '',
              };

              if (
                nextXeroConnection.tenantId !== draftOrganizationXeroConnection.current.tenantId ||
                nextXeroConnection.tenantName !== draftOrganizationXeroConnection.current.tenantName ||
                nextXeroConnection.billingMode !== draftOrganizationXeroConnection.current.billingMode ||
                nextXeroConnection.defaultSalesAccountCode !== draftOrganizationXeroConnection.current.defaultSalesAccountCode ||
                nextXeroConnection.defaultTrackingCategory1 !== draftOrganizationXeroConnection.current.defaultTrackingCategory1 ||
                nextXeroConnection.defaultTrackingCategory2 !== draftOrganizationXeroConnection.current.defaultTrackingCategory2 ||
                nextXeroConnection.defaultBrandingThemeId !== draftOrganizationXeroConnection.current.defaultBrandingThemeId ||
                nextXeroConnection.defaultReferencePrefix !== draftOrganizationXeroConnection.current.defaultReferencePrefix
              ) {
                draftOrganizationXeroConnection.current = nextXeroConnection;
                debounceUpdateOrganizationXeroConnection(nextXeroConnection);
              }

              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <GridContainer sx={{ justifyContent: 'space-between' }}>
                      <Grid>
                        <SectionIconTypography label="Xero" />
                        <BodyIconTypography label="Configure how Skedular exports supported invoices into Xero, including recurring invoice behavior." />
                        {existingXeroConnection?.isActive ? (
                          <StackRow spacing={0.5}>
                            <SmallIconTypography label="Connected to" />
                            {xeroSummaryTenantName ? <SmallIconTypography label={xeroSummaryTenantName} fontWeight={700} /> : null}
                            <SmallIconTypography label={xeroSummarySuffix} />
                          </StackRow>
                        ) : (
                          <SmallIconTypography label="Not connected. Connect Xero first, then fine-tune how Skedular exports and reconciles invoices." />
                        )}
                        <SmallIconTypography
                          label={xeroMessage ?? 'If your Xero login can access multiple tenants, connect Xero to load the available tenants, then choose one to finish setup.'}
                        />
                      </Grid>

                      <Grid>
                        <StackRow>
                          <Button component="a" href={xeroAuthorizeUrl} variant="contained" sx={defaultButtonStyle} disabled={!xeroAuthorizeUrl}>
                            {existingXeroConnection?.hasRefreshToken ? 'Reconnect Xero' : 'Connect Xero'}
                          </Button>
                          <Button variant="outlined" color="warning" onClick={handleDisconnectOrganizationXeroConnectionClick} sx={defaultButtonStyle}>
                            Disconnect
                          </Button>
                        </StackRow>
                      </Grid>
                    </GridContainer>
                    <Divider />
                  </StackColumn>

                  {xeroTenantOptions.length > 0 && !isTenantLocked && (
                    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                      <SectionIconTypography label="Available Xero Tenants" />
                      <BodyIconTypography label="Choose the tenant returned by Xero to finish attaching the organization." />
                      <StackRow sx={{ flexWrap: 'wrap', gap: 1 }}>
                        {xeroTenantOptions.map((tenantOption) => (
                          <Button
                            key={tenantOption.tenantId}
                            variant={xeroSuggestedTenantId === tenantOption.tenantId ? 'contained' : 'outlined'}
                            sx={defaultButtonStyle}
                            onClick={() => handleApplySuggestedXeroTenantClick(tenantOption)}
                          >
                            {tenantOption.tenantName || 'Unnamed Xero tenant'}
                          </Button>
                        ))}
                      </StackRow>
                      {hasSuggestedTenant && <SmallIconTypography label={`Selected tenant: ${xeroSuggestedTenantName || 'Unnamed Xero tenant'}`} />}
                    </StackColumn>
                  )}

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding, paddingBottom: defaultPadding, ...formColumnSx }}>
                    <FormFieldLabel label="Billing Mode">
                      <SingleChoiceOrganizationXeroBillingMode rootDataRelay={rootData} name="billingMode" required />
                    </FormFieldLabel>
                    <SmallIconTypography label={selectedXeroBillingModeGuidance} />
                    {selectedXeroBillingMode === 'REPEATING_INVOICES' && (
                      <>
                        <SmallIconTypography label="Recurring in-arrears bookings use the organization billing cycle for the repeating schedule." />
                        <SmallIconTypography label="Other recurring bookings use the product purchase cadence. If Xero cannot represent that cadence, Skedular falls back to a normal Xero invoice." />
                        <SmallIconTypography label="Supported recurring purchase cadences for repeating templates are weekly, fortnightly, monthly, two to six months, and yearly." />
                      </>
                    )}
                    <FormFieldLabel label="Default Sales Account Code">
                      <TextField name="defaultSalesAccountCode" helperText="Optional sales account code used when creating Xero invoices." />
                    </FormFieldLabel>
                    <FormFieldLabel label="Tracking Category 1">
                      <TextField name="defaultTrackingCategory1" helperText="Optional tracking category for org-level reporting in Xero." />
                    </FormFieldLabel>
                    <FormFieldLabel label="Tracking Category 2">
                      <TextField name="defaultTrackingCategory2" helperText="Optional secondary tracking category." />
                    </FormFieldLabel>
                    <FormFieldLabel label="Branding Theme ID">
                      <TextField name="defaultBrandingThemeId" helperText="Optional Xero branding theme to apply when Xero sends the invoice." />
                    </FormFieldLabel>
                    <FormFieldLabel label="Reference Prefix">
                      <TextField name="defaultReferencePrefix" helperText="Prefix added to references before export, for example SKED or MKT." />
                    </FormFieldLabel>
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding, paddingBottom: defaultPadding, ...formColumnSx }}>
                    <StackRow sx={{ alignItems: 'center', gap: 2 }}>
                      <BodyIconTypography label={existingXeroConnection?.isActive ? 'Connection active' : 'Connection inactive'} />
                      <BodyIconTypography startElement={<BillingIcon />} label={existingXeroConnection?.hasAccessToken ? 'Access token present' : 'No access token stored yet'} />
                      <BodyIconTypography label={existingXeroConnection?.hasRefreshToken ? 'Refresh token present' : 'No refresh token stored yet'} />
                    </StackRow>
                    {existingXeroConnection?.lastError && <SmallIconTypography label={`Last sync error: ${existingXeroConnection.lastError}`} />}
                    {existingXeroConnection?.lastSuccessfulSyncAt && (
                      <SmallIconTypography label={`Last successful sync: ${new Date(existingXeroConnection.lastSuccessfulSyncAt).toLocaleString()}`} />
                    )}
                  </StackColumn>
                </FormStackColumn>
              );
            }}
          />
        );
      case 'stripe-connect-accounts-setup':
        return (
          <StackColumn spacing={2} sx={{ p: defaultPadding }}>
            <StackRow sx={{ alignItems: 'flex-start', gap: 2 }}>
              <StackColumn spacing={0.5} sx={{ minWidth: 0 }}>
                <SectionIconTypography label="Stripe Connect Accounts" />
                <BodyIconTypography label="Review connected payout accounts, onboarding readiness, and default payout routing for this organization." />
              </StackColumn>
              <PushToRight />
              <StackColumn spacing={1} sx={{ alignItems: { xs: 'flex-start', sm: 'flex-end' } }}>
                <NewStripeConnectAccountButton organizationCustomDomain={organizationCustomDomain} label="Add New Account" />
                <ExistingStripeConnectAccountButton rootDataRelay={rootData} label="Add Existing Account" />
              </StackColumn>
            </StackRow>

            <Divider />

            <StackRow sx={{ justifyContent: 'flex-end' }}>
              <Search
                size="small"
                placeholder="Search for accounts"
                defaultValue={organizationStripeConnectAccountNameSearchText}
                onChange={handleOrganizationStripeConnectAccountsSearchTextChange}
              />
            </StackRow>

            <OrganizationMarketplaceStripeConnectAccountManagementList
              items={organizationStripeConnectAccountItems}
              selectedIds={selectedOrganizationStripeConnectAccountIds}
              onToggleSelected={handleSelectedOrganizationStripeConnectAccountsChanged}
              onOpenAccount={(accountId) => {
                const account = organizationStripeConnectAccounts.find((item) => item.id === accountId);
                if (!account) {
                  return;
                }

                router.push(getOrganizationStripeConnectAccountBaseLink(integratedPlatrform, account.organization!.customDomain!, account.id));
              }}
              onOpenMoreActions={(accountId, target) => {
                setSelectedOrganizationStripeConnectAccountId(accountId);
                setOrganizationStripeConnectAccountMoreActionsAnchorEl(target);
              }}
              onRemoveSelected={handleRemoveOrganizationStripeConnectAccountsClick}
            />
          </StackColumn>
        );
      case 'bank-accounts-setup':
        return (
          <StackColumn spacing={2} sx={{ p: defaultPadding }}>
            <StackRow sx={{ alignItems: 'flex-start', gap: 2 }}>
              <StackColumn spacing={0.5} sx={{ minWidth: 0 }}>
                <SectionIconTypography label="Bank Accounts" />
                <BodyIconTypography label="Manage payout destinations and review the default bank account used for marketplace settlements." />
              </StackColumn>
              <PushToRight />
              <NewBankAccountButton organizationCustomDomain={organizationCustomDomain} />
            </StackRow>

            <Divider />

            <StackRow sx={{ justifyContent: 'flex-end' }}>
              <Search
                size="small"
                placeholder="Search for accounts"
                defaultValue={organizationBankAccountNameSearchText}
                onChange={handleOrganizationBankAccountsSearchTextChange}
              />
            </StackRow>

            <OrganizationMarketplaceBankAccountManagementList
              items={organizationBankAccountItems}
              selectedIds={selectedOrganizationBankAccountIds}
              onToggleSelected={handleSelectedOrganizationBankAccountsChanged}
              onOpenAccount={(accountId) => {
                const account = organizationBankAccounts.find((item) => item.id === accountId);
                if (!account) {
                  return;
                }

                router.push(getOrganizationBankAccountBaseLink(integratedPlatrform, account.organization!.customDomain!, account.id));
              }}
              onOpenMoreActions={(accountId, target) => {
                setSelectedOrganizationBankAccountId(accountId);
                setOrganizationBankAccountMoreActionsAnchorEl(target);
              }}
              onRemoveSelected={handleRemoveOrganizationBankAccountsClick}
            />
          </StackColumn>
        );
      case 'product-tags-setup':
        return (
          <StackColumn spacing={2} sx={{ p: defaultPadding }}>
            <StackRow sx={{ alignItems: 'flex-start', gap: 2 }}>
              <StackColumn spacing={0.5} sx={{ minWidth: 0 }}>
                <SectionIconTypography label="Product Tags" />
                <BodyIconTypography label="Manage the marketplace-facing tags used to classify products, resources, and customer filters." />
              </StackColumn>
              <PushToRight />
              <AddOrganizationProductTagButton organizationCustomDomain={organizationCustomDomain} />
            </StackRow>

            <Divider />

            <StackRow sx={{ justifyContent: 'flex-end' }}>
              <Search size="small" placeholder="Search for product tags" defaultValue={productTagNameSearchText} onChange={handleProductTagsSearchTextChange} />
            </StackRow>

            {selectedProductTagIds.length > 0 && (
              <StackColumn spacing={2}>
                <Divider />
                <StackRow sx={{ alignItems: 'center' }}>
                  <SmallIconTypography label={`${selectedProductTagIds.length} record${selectedProductTagIds.length === 1 ? '' : 's'} selected`} />
                  <PushToRight />
                  <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveProductTagsClick} sx={{ textTransform: 'none' }}>
                    Remove Product Tag
                  </Button>
                </StackRow>
              </StackColumn>
            )}

            <OrganizationAdminTagManagementList
              items={productTagItems}
              emptyTitle="No product tags found"
              emptyDescription="Adjust the search or add a new product tag for this organization."
              selectedIds={selectedProductTagIds}
              onToggleSelected={handleSelectedProductTagsChanged}
              onOpenMoreActions={(id, target) => {
                setSelectedProductTagId(id);
                setProductTagMoreActionsAnchorEl(target);
              }}
              renderPrimary={(item) => {
                const productTag = productTags.find((entry) => entry.id === item.id);

                return productTag ? <ProductTag productTag={productTag} showFullName /> : null;
              }}
              variant="plain"
            />
          </StackColumn>
        );
      case 'marketplace-listing':
      default:
        return (
          <Form
            onSubmit={() => undefined}
            initialValues={{
              title: organization?.marketplaceListingMetadata.title ?? null,
              subTitle: organization?.marketplaceListingMetadata.subTitle ?? null,
            }}
            validate={validateOrganizationMarketplaceListingMetadataDetails}
            render={({ handleSubmit }) => {
              return (
                <FormStackColumn onSubmit={handleSubmit} sx={formColumnSx}>
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <SectionIconTypography label="Organization Marketplace Listing Setup" />
                    <BodyIconTypography label="Edit your organization marketplace listing details" />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding, paddingBottom: defaultPadding }}>
                    <ListingMetadata
                      fields={['title', 'subTitle']}
                      onChange={({ subTitle, title }) => {
                        debounceUpdateOrganizationMarketplaceListingMetadata({ title, subTitle });
                      }}
                      requiredFields={requiredOrganizationMarketplaceListingMetadataDetailsFields}
                    />
                  </StackColumn>
                </FormStackColumn>
              );
            }}
          />
        );
    }
  };

  return (
    <>
      <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 0, sm: 1, md: 2 }, pb: defaultPadding }}>
        <StackColumn
          sx={{
            width: '100%',
            maxWidth: 1200,
            mx: 'auto',
            pt: { xs: 1, sm: 1, md: 2 },
            backgroundColor: 'transparent',
            gap: 2,
          }}
        >
          {!embedded && (
            <>
              <PageHeaderPanel
                eyebrow="Marketplace setup"
                title={organization?.name ?? 'Marketplace settings'}
                description="Manage listing details, billing cadence, Xero, payout rails, and product tags."
              >
                <StackColumn spacing={0.5}>
                  <SmallIconTypography label="Commerce & payouts" />
                  <BodyIconTypography label={organization?.marketplaceListingMetadata?.title || organization?.name || 'Listing, billing, Xero, Stripe, and bank accounts'} />
                </StackColumn>
              </PageHeaderPanel>

              <OrganizationMarketplaceSetupSectionNav activeSection={activeSection} organizationCustomDomain={organizationCustomDomain} stickyTop={stickyTop} />
            </>
          )}
          <Box
            sx={{
              borderRadius: 4,
              border: 1,
              borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
              bgcolor: (theme) => (theme.palette.mode === 'light' ? 'common.white' : theme.palette.background.paper),
              boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 12px 32px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
              overflow: 'hidden',
            }}
          >
            {renderActiveSection()}
          </Box>
        </StackColumn>
      </Box>

      <MoreActionsMenu
        anchorEl={productTagMoreActionsAnchorEl}
        open={productTagMoreActionsMenuOpen}
        onMenuItemClick={handleProductTagMoreActionsMenuItemClick}
        options={productTagMoreActionsOption}
      />

      <MoreActionsMenu
        anchorEl={organizationStripeConnectAccountMoreActionsAnchorEl}
        open={organizationStripeConnectAccountMoreActionsMenuOpen}
        onMenuItemClick={handleOrganizationStripeConnectAccountMoreActionsMenuItemClick}
        options={organizationStripeConnectAccountMoreActionsOption}
      />

      <MoreActionsMenu
        anchorEl={organizationBankAccountMoreActionsAnchorEl}
        open={organizationBankAccountMoreActionsMenuOpen}
        onMenuItemClick={handleOrganizationBankAccountMoreActionsMenuItemClick}
        options={organizationBankAccountMoreActionsOption}
      />
    </>
  );
};

export default memo(OrganizationMarketplaceSetup);

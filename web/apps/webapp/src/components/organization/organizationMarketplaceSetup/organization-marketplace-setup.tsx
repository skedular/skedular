import { NewBankAccountButton } from '@/components/bankAccount/addBankAccount';
import {
  AppBarWithStackColumn,
  BodyIconTypography,
  FormFieldLabel,
  FormStackColumn,
  GridContainer,
  PushToRight,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@/components/commons';
import { BillingIcon, DeleteIcon, EllipseMenuIcon } from '@/components/icons';
import { getOrganizationBankAccountBaseLink, getOrganizationBaseLink, getOrganizationStripeConnectAccountBaseLink } from '@/components/links';
import { ListingMetadata, listingMetadataSchemaShape } from '@/components/listingMetadata';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { SingleChoiceOrganizationBillingCycle, SingleChoiceOrganizationXeroBillingMode } from '@/components/organization';
import { AddOrganizationProductTagButton } from '@/components/organization/addOrganizationProductTag';
import { EditOrganizationProductTagDialog } from '@/components/organization/editOrganizationProductTag';
import { ProductTag } from '@/components/productTag';
import { Search } from '@/components/search';
import { CompleteOnboardStripeConnectAccountButton } from '@/components/stripeConnectAccount';
import { ExistingStripeConnectAccountButton, NewStripeConnectAccountButton } from '@/components/stripeConnectAccount/addStripeConnectAccount';
import { defaultGridRowSelectionModelValue } from '@/libs/mui';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultButtonStyle, defaultGridActionPadding, defaultGridStyle, defaultPadding, emerald, flame, secondDrawerExpandedDrawerWidthPx } from '@/libs/theme';
import { getRelayErrorMessage, keyboardTextFieldDebounceTimeout } from '@/libs/utils';
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
  organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation,
} from '@/queries/__generated__/organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation.graphql';
import type { organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation } from '@/queries/__generated__/organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation.graphql';
import type {
  organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation,
  OrganizationXeroBillingMode,
} from '@/queries/__generated__/organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
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
import OrganizationMarketplaceSetupLeftSideNavigationMenuContent from './organization-marketplace-setup-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationMarketplaceSetup_query$key;
  rootDataProductTagsRelay: organizationMarketplaceSetup_productTags_query$key;
  rootDataOrganizationStripeConnectAccountsRelay: organizationMarketplaceSetup_organizationStripeConnectAccounts_query$key;
  rootDataOrganizationBankAccountsRelay: organizationMarketplaceSetup_organizationBankAccounts_query$key;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

type ProductTagRowType = {
  id: string;
  name: string;
  description: string | null | undefined;
};

type OrganizationStripeConnectAccountRowType = {
  id: string;
  isDefault: boolean;
  name: string;
  companyName: string | null | undefined;
  country: string | null | undefined;
  defaultCurrency: string | null | undefined;
  businessType: string | null | undefined;
  website: string | null | undefined;
  supportLink: string | null | undefined;
  contactEmail: string | null | undefined;
  contactPhone: string | null | undefined;
  chargesEnabled: boolean;
  payoutsEnabled: boolean;
  detailsSubmitted: boolean;
  isAuthorized: boolean;
  requiresOnboarding: boolean;
};

type OrganizationBankAccountRowType = {
  id: string;
  isDefault: boolean;
  name: string;
  bankName: string;
  accountHolderName: string;
  accountNumber: string;
  country: string;
};

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
  defaultReceivablesAccountCode?: string | null;
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

const OrganizationMarketplaceSetup = ({
  rootDataRelay,
  rootDataProductTagsRelay,
  rootDataOrganizationStripeConnectAccountsRelay,
  rootDataOrganizationBankAccountsRelay,
  onReloadRequired,
  organizationCustomDomain,
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

  const [commitUpdateOrganizationMarketplaceListingMetadata] = useMutation<organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation>(graphql`
    mutation organizationMarketplaceSetup_updateOrganizationMarketplaceListingMetadataMutation($input: UpdateOrganizationMarketplaceListingMetadataInput!) @raw_response_type {
      updateOrganizationMarketplaceListingMetadata(input: $input) {
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

  const [commitUpdateOrganizationBillingSettings] = useMutation<organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation>(graphql`
    mutation organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation($input: UpdateOrganizationBillingSettingsInput!) @raw_response_type {
      updateOrganizationBillingSettings(input: $input) {
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

  const [commitUpdateOrganizationXeroConnection] = useMutation<organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation>(graphql`
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
  const xeroSuggestedTenantId = searchParams.get('xeroSuggestedTenantId') ?? '';
  const xeroSuggestedTenantName = searchParams.get('xeroSuggestedTenantName') ?? '';
  const xeroMessage = searchParams.get('xeroMessage');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});

  const validateOrganizationMarketplaceListingMetadataDetails = makeValidate(organizationMarketplaceListingMetadataSchema);
  const requiredOrganizationMarketplaceListingMetadataDetailsFields = makeRequired(organizationMarketplaceListingMetadataSchema);

  const [organizationTitle, setOrganizationTitle] = useState(rootData.organization?.marketplaceListingMetadata.title ?? null);
  const debounceSetOrganizationTitle = useDebounceCallback(setOrganizationTitle, keyboardTextFieldDebounceTimeout);
  const [organizationSubTitle, setOrganizationSubTitle] = useState(rootData.organization?.marketplaceListingMetadata.subTitle ?? null);
  const debounceSetOrganizationSubTitle = useDebounceCallback(setOrganizationSubTitle, keyboardTextFieldDebounceTimeout);

  const validateOrganizationBillingSettingsDetails = makeValidate(organizationBillingSettingsSchema);
  const requiredOrganizationBillingSettingsDetailsFields = makeRequired(organizationBillingSettingsSchema);

  const [organizationBillingCycle, setOrganizationBillingCycle] = useState(rootData.organization?.billingCycle.type ?? '');
  const debounceSetOrganizationBillingCycle = useDebounceCallback(setOrganizationBillingCycle, keyboardTextFieldDebounceTimeout);
  const [organizationInvoiceDueInDays, setOrganizationInvoiceDueInDays] = useState<number>(rootData.organization?.invoiceDueInDays ?? 7);
  const debounceSetOrganizationInvoiceDueInDays = useDebounceCallback(setOrganizationInvoiceDueInDays, keyboardTextFieldDebounceTimeout);
  const organization = rootData.organization as
    | (NonNullable<typeof rootData.organization> & {
        xeroConnection?: OrganizationXeroConnectionDetails | null;
      })
    | null
    | undefined;
  const existingXeroConnection = organization?.xeroConnection;
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
  const [seledctedProductTags, setSeledctedProductTags] = useState<GridRowSelectionModel>(defaultGridRowSelectionModelValue);
  const [selectedProductTagId, setSelectedProductTagId] = useState<null | string>(null);
  const [productTagMoreActionsAnchorEl, setProductTagMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const productTagMoreActionsMenuOpen = Boolean(productTagMoreActionsAnchorEl);
  const [isEditProductTagDialogOpen, setIsEditProductTagDialogOpen] = useState(false);
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
  const [seledctedOrganizationStripeConnectAccounts, setSeledctedOrganizationStripeConnectAccounts] = useState<GridRowSelectionModel>(defaultGridRowSelectionModelValue);
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
  const [seledctedOrganizationBankAccounts, setSeledctedOrganizationBankAccounts] = useState<GridRowSelectionModel>(defaultGridRowSelectionModelValue);
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
    if (!section || section === 'stripe-connect-accounts-setup') {
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

  const handleProductTagsSearchTextChange = (str: string) => {
    setProductTagNameSearchText(str);

    handleRefetchProductTags(str);
  };

  const handleSelectedProductTagsChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedProductTags(newRowSelectionModel);
  };

  const handleProductTagMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setProductTagMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditProductTag:
        setIsEditProductTagDialogOpen(true);
        break;

      case MoreActionsMenuOptionType.DeleteProductTag:
        handleRemoveProductTagClick();
        break;
    }
  };

  const handleEditProductTagClick = () => {
    setIsEditProductTagDialogOpen(false);
  };

  const handleEditProductTagCancel = () => {
    setIsEditProductTagDialogOpen(false);
  };

  const handleRemoveProductTagsClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing product tags ..." />, infoNotificationOptions);

    commitDeleteProductTags({
      variables: {
        connectionIds: productTagsConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: seledctedProductTags.ids
            .values()
            .map((id) => id as string)
            .toArray(),
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

  const handleSelectedOrganizationStripeConnectAccountsChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedOrganizationStripeConnectAccounts(newRowSelectionModel);
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
          ids: seledctedOrganizationStripeConnectAccounts.ids
            .values()
            .map((id) => id as string)
            .toArray(),
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

  const handleUpdateOrganizationXeroConnectionClick = (values: OrganizationXeroConnectionDetails) => {
    if (!organization) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating Xero settings for ${organization.name}...`} />, infoNotificationOptions);

    commitUpdateOrganizationXeroConnection({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationId: organization.id,
          organizationCustomDomain,
          tenantId: organization.xeroConnection?.tenantId ?? '',
          tenantName: organization.xeroConnection?.tenantName ?? '',
          billingMode: values.billingMode ?? 'DISABLED',
          scopes: organization.xeroConnection?.scopes ?? '',
          isActive: existingXeroConnection?.isActive ?? false,
          sendInvoicesViaXero: true,
          autoReconcilePayments: true,
          defaultSalesAccountCode: values.defaultSalesAccountCode ?? null,
          defaultReceivablesAccountCode: values.defaultReceivablesAccountCode ?? null,
          defaultTrackingCategory1: values.defaultTrackingCategory1 ?? null,
          defaultTrackingCategory2: values.defaultTrackingCategory2 ?? null,
          defaultBrandingThemeId: values.defaultBrandingThemeId ?? null,
          defaultReferencePrefix: values.defaultReferencePrefix ?? null,
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

  const handleSelectedOrganizationBankAccountsChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedOrganizationBankAccounts(newRowSelectionModel);
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
          ids: seledctedOrganizationBankAccounts.ids
            .values()
            .map((id) => id as string)
            .toArray(),
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

  const handleCloseClick = () => {
    router.push(getOrganizationBaseLink(integratedPlatrform, organizationCustomDomain));
  };

  const productTagRows: ProductTagRowType[] = productTags.map((productTag) => ({
    id: productTag.id,
    name: productTag.name,
    description: productTag.description,
  }));

  const productTagColumns: GridColDef<(typeof productTagRows)[number]>[] = [
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => {
        const productTag = productTags.find((productTag) => productTag.id === (params.id as string));
        if (!productTag) {
          return null;
        }

        return <ProductTag productTag={productTag} showFullName />;
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
      field: 'More Actions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <Box sx={{ display: 'flex', justifyContent: 'flex-end', width: '100%' }}>
          <IconButton
            onClick={(event: React.MouseEvent<HTMLElement>) => {
              setSelectedProductTagId(params.id as string);
              setProductTagMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

  const organizationStripeConnectAccountRows: OrganizationStripeConnectAccountRowType[] = organizationStripeConnectAccounts.map((account) => ({
    id: account.id,
    isDefault: account.isDefault,
    name: account.name,
    companyName: account.companyName,
    country: getCountryData(account.country as TCountryCode).name,
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
    requiresOnboarding: !account.isOnboardingCompleted,
  }));

  const organizationStripeConnectAccountColumns: GridColDef<(typeof organizationStripeConnectAccountRows)[number]>[] = [
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 100,
    },
    {
      field: 'requiresOnboarding',
      headerName: 'Onboarding Required',
      editable: false,
      renderCell: (params) => {
        if (!params.value) {
          return <></>;
        }

        const account = organizationStripeConnectAccounts.find((account) => account.id === (params.id as string));
        if (!account) {
          return <></>;
        }

        return <CompleteOnboardStripeConnectAccountButton onboardingUrl={account.onboardingUrl} variant="contained" size="small" sx={{ marginTop: 1, marginBottom: 1 }} />;
      },
      display: 'flex',
      minWidth: 150,
    },
    {
      field: 'isDefault',
      headerName: 'Default',
      editable: false,
      renderCell: (params) => (
        <StackRow>
          {params.value && (
            <StackRow sx={{ justifyContent: 'space-between', width: 76 }}>
              <SmallIconTypography label="Yes" />
              <Box sx={{ width: 15, height: 15, borderRadius: '50%', backgroundColor: emerald }} />
            </StackRow>
          )}
          {!params.value && (
            <StackRow sx={{ justifyContent: 'space-between', width: 76 }}>
              <SmallIconTypography label="No" />
              <Box sx={{ width: 15, height: 15, borderRadius: '50%', backgroundColor: flame }} />
            </StackRow>
          )}
        </StackRow>
      ),
      display: 'flex',
    },
    {
      field: 'companyName',
      headerName: 'Company Name',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 150,
    },
    {
      field: 'businessType',
      headerName: 'Business Type',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 150,
    },
    {
      field: 'country',
      headerName: 'Country',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 100,
    },
    {
      field: 'defaultCurrency',
      headerName: 'Currency',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 100,
    },
    {
      field: 'website',
      headerName: 'Website',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 100,
    },
    {
      field: 'supportLink',
      headerName: 'Support Link',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 100,
    },
    {
      field: 'contactEmail',
      headerName: 'Contact Email',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 100,
    },
    {
      field: 'contactPhone',
      headerName: 'Contact Phone',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 150,
    },
    {
      field: 'chargesEnabled',
      headerName: 'Charges?',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value ? 'Enabled' : 'Disabled'} />,
      display: 'flex',
      minWidth: 50,
    },
    {
      field: 'payoutsEnabled',
      headerName: 'Payouts',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value ? 'Enabled' : 'Disabled'} />,
      display: 'flex',
      minWidth: 50,
    },
    {
      field: 'detailsSubmitted',
      headerName: 'Details',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value ? 'Submitted' : 'N/A'} />,
      display: 'flex',
      minWidth: 50,
    },
    {
      field: 'isAuthorized',
      headerName: 'Authorized',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value ? 'Yes' : 'No'} />,
      display: 'flex',
      minWidth: 50,
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
              setSelectedOrganizationStripeConnectAccountId(params.id as string);
              setOrganizationStripeConnectAccountMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

  const organizationBankAccountRows: OrganizationBankAccountRowType[] = organizationBankAccounts.map((account) => ({
    id: account.id,
    isDefault: account.isDefault,
    name: account.name,
    accountHolderName: account.accountHolderName,
    accountNumber: account.accountNumber,
    bankName: account.bankName,
    country: getCountryData(account.country as TCountryCode).name,
  }));

  const organizationBankAccountColumns: GridColDef<(typeof organizationBankAccountRows)[number]>[] = [
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 100,
    },
    {
      field: 'bankName',
      headerName: 'Bank Name',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 150,
    },
    {
      field: 'accountHolderName',
      headerName: 'Account Holder Name',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 150,
    },
    {
      field: 'accountNumber',
      headerName: 'Account Number',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 250,
    },
    {
      field: 'country',
      headerName: 'Country',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 150,
    },
    {
      field: 'isDefault',
      headerName: 'Default',
      editable: false,
      renderCell: (params) => (
        <StackRow>
          {params.value && (
            <StackRow sx={{ justifyContent: 'space-between', width: 76 }}>
              <SmallIconTypography label="Yes" />
              <Box sx={{ width: 15, height: 15, borderRadius: '50%', backgroundColor: emerald }} />
            </StackRow>
          )}
          {!params.value && (
            <StackRow sx={{ justifyContent: 'space-between', width: 76 }}>
              <SmallIconTypography label="No" />
              <Box sx={{ width: 15, height: 15, borderRadius: '50%', backgroundColor: flame }} />
            </StackRow>
          )}
        </StackRow>
      ),
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
              setSelectedOrganizationBankAccountId(params.id as string);
              setOrganizationBankAccountMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

  const handleOrganizationMarketplaceListingMetadataDetailUpdateClick = ({ title, subTitle }: OrganizationMarketplaceListingMetadataDetails) => {
    const organization = rootData.organization;
    if (!organization) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}' marketplace listing...`} />, infoNotificationOptions);

    commitUpdateOrganizationMarketplaceListingMetadata({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: organization.id,
          marketplaceListingMetadata: {
            about: '',
            title: title ?? '',
            subTitle: subTitle ?? '',
            includedFeatures: [],
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
        updateOrganizationMarketplaceListingMetadata: {
          organization: {
            id: organization.id,
            marketplaceListingMetadata: {
              about: '',
              title: title ?? '',
              subTitle: subTitle ?? '',
              includedFeatures: [],
            },
          },
        },
      },
    });
  };

  const handleOrganizationBillingSettingsUpdateClick = ({ billingCycle, invoiceDueInDays }: OrganizationBillingSettingsDetails) => {
    const organization = rootData.organization;
    if (!organization) {
      return;
    }

    const normalizedInvoiceDueInDays = Number(invoiceDueInDays);

    const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}' billing settings...`} />, infoNotificationOptions);

    commitUpdateOrganizationBillingSettings({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: organization.id,
          billingCycle: billingCycle as OrganizationBillingCycle,
          invoiceDueInDays: normalizedInvoiceDueInDays,
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
        updateOrganizationBillingSettings: {
          organization: {
            id: organization.id,
            billingCycle: {
              type: billingCycle as OrganizationBillingCycle,
              name: '',
            },
            invoiceDueInDays: normalizedInvoiceDueInDays,
          },
        },
      },
    });
  };

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

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationMarketplaceSetupLeftSideNavigationMenuContent organizationCustomDomain={organizationCustomDomain} hideIcons />
        <Box sx={{ marginLeft: secondDrawerExpandedDrawerWidthPx, flexGrow: 1 }}>
          <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Marketplace Information">
            <Form
              onSubmit={handleOrganizationMarketplaceListingMetadataDetailUpdateClick}
              initialValues={{
                title: organizationTitle,
                subTitle: organizationSubTitle,
              }}
              validate={validateOrganizationMarketplaceListingMetadataDetails}
              render={({ handleSubmit }) => {
                return (
                  <FormStackColumn onSubmit={handleSubmit}>
                    <StackColumn
                      sx={{
                        paddingLeft: defaultPadding,
                        paddingRight: defaultPadding,
                        paddingTop: defaultPadding,
                      }}
                      ref={(divElement) => {
                        sectionRefs.current['marketplace-listing'] = divElement;
                      }}
                    >
                      <SectionIconTypography label="Organization Marketplace Listing Setup" />
                      <BodyIconTypography label="Edit your organization marketplace listing details" />
                      <Divider />
                    </StackColumn>

                    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                      <ListingMetadata
                        fields={['title', 'subTitle']}
                        onChange={({ subTitle, title }) => {
                          debounceSetOrganizationTitle(title);
                          debounceSetOrganizationSubTitle(subTitle);
                        }}
                        requiredFields={requiredOrganizationMarketplaceListingMetadataDetailsFields}
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
              onSubmit={handleOrganizationBillingSettingsUpdateClick}
              initialValues={{
                billingCycle: organizationBillingCycle,
                invoiceDueInDays: organizationInvoiceDueInDays,
              }}
              validate={validateOrganizationBillingSettingsDetails}
              render={({ handleSubmit, values }) => {
                debounceSetOrganizationBillingCycle(values!.billingCycle);
                const nextInvoiceDueInDays = typeof values?.invoiceDueInDays === 'number' ? values.invoiceDueInDays : Number(values?.invoiceDueInDays ?? NaN);
                if (!Number.isNaN(nextInvoiceDueInDays)) {
                  debounceSetOrganizationInvoiceDueInDays(nextInvoiceDueInDays);
                }

                return (
                  <FormStackColumn onSubmit={handleSubmit}>
                    <StackColumn
                      sx={{
                        paddingLeft: defaultPadding,
                        paddingRight: defaultPadding,
                        paddingTop: defaultPadding,
                      }}
                      ref={(divElement) => {
                        sectionRefs.current['billing-cycle'] = divElement;
                      }}
                    >
                      <SectionIconTypography label="Organization Billing Settings" />
                      <BodyIconTypography label="Edit your organization billing cycle and default invoice payment terms." />
                      <Divider />
                    </StackColumn>

                    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                      <FormFieldLabel label="Billing Cycle">
                        <SingleChoiceOrganizationBillingCycle
                          rootDataRelay={rootData}
                          name="billingCycle"
                          required={requiredOrganizationBillingSettingsDetailsFields.billingCycle}
                        />
                      </FormFieldLabel>
                    </StackColumn>

                    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                      <FormFieldLabel label="Invoice Due Days">
                        <TextField
                          name="invoiceDueInDays"
                          type="number"
                          required={requiredOrganizationBillingSettingsDetailsFields.invoiceDueInDays}
                          helperText="How many days customers have to pay marketplace invoices by default."
                        />
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
                      {!organization?.billingDetails && <SmallIconTypography label="Add billing details in organization admin first, then update billing settings here." />}
                    </StackColumn>
                  </FormStackColumn>
                );
              }}
            />

            <Form
              key={`xero-form-${existingXeroConnection?.id ?? 'new'}-${xeroSuggestedTenantId}-${xeroSuggestedTenantName}`}
              onSubmit={handleUpdateOrganizationXeroConnectionClick}
              initialValues={{
                tenantName: existingXeroConnection?.tenantName ?? xeroSuggestedTenantName,
                billingMode: existingXeroConnection?.billingMode ?? 'DISABLED',
                scopes: existingXeroConnection?.scopes ?? '',
                isActive: existingXeroConnection?.isActive ?? false,
                defaultSalesAccountCode: existingXeroConnection?.defaultSalesAccountCode ?? '',
                defaultReceivablesAccountCode: existingXeroConnection?.defaultReceivablesAccountCode ?? '',
                defaultTrackingCategory1: existingXeroConnection?.defaultTrackingCategory1 ?? '',
                defaultTrackingCategory2: existingXeroConnection?.defaultTrackingCategory2 ?? '',
                defaultBrandingThemeId: existingXeroConnection?.defaultBrandingThemeId ?? '',
                defaultReferencePrefix: existingXeroConnection?.defaultReferencePrefix ?? '',
              }}
              render={({ handleSubmit, values }) => {
                const selectedXeroBillingMode = values?.billingMode ?? existingXeroConnection?.billingMode ?? 'DISABLED';
                const selectedXeroBillingModeGuidance = xeroBillingModeGuidance[selectedXeroBillingMode] ?? xeroBillingModeGuidance.DISABLED;

                return (
                  <FormStackColumn onSubmit={handleSubmit}>
                    <StackColumn
                      sx={{
                        paddingLeft: defaultPadding,
                        paddingRight: defaultPadding,
                        paddingTop: defaultPadding,
                      }}
                      ref={(divElement) => {
                        sectionRefs.current['xero-setup'] = divElement;
                      }}
                    >
                      <GridContainer sx={{ justifyContent: 'space-between' }}>
                        <Grid>
                          <SectionIconTypography label="Xero" />
                          <BodyIconTypography label="Configure how Skedular exports supported invoices into Xero, including recurring invoice behavior." />
                          {existingXeroConnection?.isActive ? (
                            <StackRow spacing={0.5}>
                              <Typography variant="body2">Connected to</Typography>
                              {xeroSummaryTenantName ? (
                                <Typography variant="body2" fontWeight={700}>
                                  {xeroSummaryTenantName}
                                </Typography>
                              ) : null}
                              <Typography variant="body2">{xeroSummarySuffix}</Typography>
                            </StackRow>
                          ) : (
                            <SmallIconTypography label="Not connected. Connect Xero first, then fine-tune how Skedular exports and reconciles invoices." />
                          )}
                          <SmallIconTypography
                            label={
                              xeroMessage ??
                              'If your Xero login can access multiple tenants, connect Xero to load the available tenants, then choose one and save it here to finish setup.'
                            }
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
                        <BodyIconTypography label="Choose the tenant returned by Xero and save it into this setup to finish attaching the organization." />
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

                    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
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
                      <FormFieldLabel label="Default Receivables Account Code">
                        <TextField name="defaultReceivablesAccountCode" helperText="Optional receivables account code for invoice posting." />
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

                    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
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

                    <StackColumn
                      sx={{
                        paddingLeft: defaultPadding,
                        paddingRight: defaultPadding,
                        paddingTop: defaultPadding,
                      }}
                    >
                      <StackRow>
                        <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                          Save Xero Settings
                        </Button>
                      </StackRow>
                    </StackColumn>
                  </FormStackColumn>
                );
              }}
            />

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['stripe-connect-accounts-setup'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Stripe Connect Accounts" />
                  <BodyIconTypography label="Edit your organization Stripe Connect accounts details" />
                </Grid>

                <Grid>
                  <NewStripeConnectAccountButton organizationCustomDomain={organizationCustomDomain} />
                  <ExistingStripeConnectAccountButton rootDataRelay={rootData} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>
            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <PushToRight />
              <Search
                size="small"
                placeholder="Search for accounts"
                defaultValue={organizationStripeConnectAccountNameSearchText}
                onChange={handleOrganizationStripeConnectAccountsSearchTextChange}
              />
            </GridContainer>
            {seledctedOrganizationStripeConnectAccounts.ids.size > 0 && (
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
                    <SmallIconTypography label={`${seledctedOrganizationStripeConnectAccounts.ids.size} records selected`} />
                    <PushToRight />
                    <Button
                      size="medium"
                      variant="contained"
                      color="warning"
                      startIcon={<DeleteIcon />}
                      onClick={handleRemoveOrganizationStripeConnectAccountsClick}
                      sx={{ textTransform: 'none' }}
                    >
                      Remove Stripe Connect Account
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}
            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedOrganizationStripeConnectAccounts}
                onRowSelectionModelChange={handleSelectedOrganizationStripeConnectAccountsChanged}
                rows={organizationStripeConnectAccountRows}
                columns={organizationStripeConnectAccountColumns}
                hideFooterPagination={organizationStripeConnectAccountRows.length <= 10}
                initialState={{
                  pagination: {
                    rowCount: organizationStripeConnectAccountRows.length,
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
                localeText={{ noRowsLabel: 'No stripe connect account found' }}
              />
            </StackRow>
            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['bank-accounts-setup'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Bank Accounts" />
                  <BodyIconTypography label="Edit your organization Bank accounts details" />
                </Grid>

                <Grid>
                  <NewBankAccountButton organizationCustomDomain={organizationCustomDomain} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>
            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <PushToRight />
              <Search
                size="small"
                placeholder="Search for accounts"
                defaultValue={organizationBankAccountNameSearchText}
                onChange={handleOrganizationBankAccountsSearchTextChange}
              />
            </GridContainer>
            {seledctedOrganizationBankAccounts.ids.size > 0 && (
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
                    <SmallIconTypography label={`${seledctedOrganizationBankAccounts.ids.size} records selected`} />
                    <PushToRight />
                    <Button
                      size="medium"
                      variant="contained"
                      color="warning"
                      startIcon={<DeleteIcon />}
                      onClick={handleRemoveOrganizationBankAccountsClick}
                      sx={{ textTransform: 'none' }}
                    >
                      Remove Bank Account
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}
            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedOrganizationBankAccounts}
                onRowSelectionModelChange={handleSelectedOrganizationBankAccountsChanged}
                rows={organizationBankAccountRows}
                columns={organizationBankAccountColumns}
                hideFooterPagination={organizationBankAccountRows.length <= 10}
                initialState={{
                  pagination: {
                    rowCount: organizationBankAccountRows.length,
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
                localeText={{ noRowsLabel: 'No bank account found' }}
              />
            </StackRow>
            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['product-tags-setup'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Product Tags" />
                  <BodyIconTypography label="Edit your organization product tags details" />
                </Grid>

                <Grid>
                  <AddOrganizationProductTagButton organizationCustomDomain={organizationCustomDomain} connectionIds={productTagsConnectionIds} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>
            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <PushToRight />
              <Search size="small" placeholder="Search for product tags" defaultValue={productTagNameSearchText} onChange={handleProductTagsSearchTextChange} />
            </GridContainer>
            {seledctedProductTags.ids.size > 0 && (
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
                    <SmallIconTypography label={`${seledctedProductTags.ids.size} records selected`} />
                    <PushToRight />
                    <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveProductTagsClick} sx={{ textTransform: 'none' }}>
                      Remove Product Tag
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}
            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedProductTags}
                onRowSelectionModelChange={handleSelectedProductTagsChanged}
                rows={productTagRows}
                columns={productTagColumns}
                hideFooterPagination={productTagRows.length <= 10}
                initialState={{
                  pagination: {
                    rowCount: productTagRows.length,
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
                localeText={{ noRowsLabel: 'No product tag found' }}
              />
            </StackRow>
          </AppBarWithStackColumn>
        </Box>
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

      {selectedProductTagId && (
        <EditOrganizationProductTagDialog
          onReloadRequired={onReloadRequired}
          productTagId={selectedProductTagId}
          isDialogOpen={isEditProductTagDialogOpen}
          onAddClicked={handleEditProductTagClick}
          onCancel={handleEditProductTagCancel}
        />
      )}
    </>
  );
};

export default memo(OrganizationMarketplaceSetup);

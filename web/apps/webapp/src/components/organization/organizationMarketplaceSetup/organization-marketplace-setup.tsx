import { NewBankAccountButton } from '@/components/bankAccount/addBankAccount';
import { AppBarWithStackColumn, BodyIconTypography, GridContainer, PushToRight, SectionIconTypography, SmallIconTypography, StackColumn, StackRow } from '@/components/commons';
import { DeleteIcon, EllipseMenuIcon } from '@/components/icons';
import { getOrganizationBankAccountBaseLink, getOrganizationBaseLink, getOrganizationProductBaseLink, getOrganizationStripeConnectAccountBaseLink } from '@/components/links';
import { LocationTag } from '@/components/locationTag';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { AddOrganizationLocationTagButton } from '@/components/organization/addOrganizationLocationTag';
import { AddOrganizationProductTagButton } from '@/components/organization/addOrganizationProductTag';
import { EditOrganizationLocationTagDialog } from '@/components/organization/editOrganizationLocationTag';
import { EditOrganizationProductTagDialog } from '@/components/organization/editOrganizationProductTag';
import { NewProductButton } from '@/components/product/addProduct';
import { ProductTag } from '@/components/productTag';
import { Search } from '@/components/search';
import { CompleteOnboardStripeConnectAccountButton } from '@/components/stripeConnectAccount';
import { ExistingStripeConnectAccountButton, NewStripeConnectAccountButton } from '@/components/stripeConnectAccount/addStripeConnectAccount';
import { defaultGridRowSelectionModelValue } from '@/libs/mui';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultButtonStyle, defaultGridActionPadding, defaultGridStyle, defaultPadding, emerald, flame, secondDrawerExpandedDrawerWidthPx } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { organizationMarketplaceSetup_activateProductsMutation } from '@/queries/__generated__/organizationMarketplaceSetup_activateProductsMutation.graphql';
import type { organizationMarketplaceSetup_deactivateProductsMutation } from '@/queries/__generated__/organizationMarketplaceSetup_deactivateProductsMutation.graphql';
import type { organizationMarketplaceSetup_deleteLocationTagsMutation } from '@/queries/__generated__/organizationMarketplaceSetup_deleteLocationTagsMutation.graphql';
import type { organizationMarketplaceSetup_deleteOrganizationBankAccountsMutation } from '@/queries/__generated__/organizationMarketplaceSetup_deleteOrganizationBankAccountsMutation.graphql';
import type { organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation } from '@/queries/__generated__/organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation.graphql';
import type { organizationMarketplaceSetup_deleteProductsMutation } from '@/queries/__generated__/organizationMarketplaceSetup_deleteProductsMutation.graphql';
import type { organizationMarketplaceSetup_deleteProductTagsMutation } from '@/queries/__generated__/organizationMarketplaceSetup_deleteProductTagsMutation.graphql';
import type { organizationMarketplaceSetup_locationTags_query$key } from '@/queries/__generated__/organizationMarketplaceSetup_locationTags_query.graphql';
import type { organizationMarketplaceSetup_locationTags_refetchableFragment } from '@/queries/__generated__/organizationMarketplaceSetup_locationTags_refetchableFragment.graphql';
import type { organizationMarketplaceSetup_organizationBankAccounts_query$key } from '@/queries/__generated__/organizationMarketplaceSetup_organizationBankAccounts_query.graphql';
import type { organizationMarketplaceSetup_organizationBankAccounts_refetchableFragment } from '@/queries/__generated__/organizationMarketplaceSetup_organizationBankAccounts_refetchableFragment.graphql';
import type { organizationMarketplaceSetup_organizationStripeConnectAccounts_query$key } from '@/queries/__generated__/organizationMarketplaceSetup_organizationStripeConnectAccounts_query.graphql';
import type { organizationMarketplaceSetup_organizationStripeConnectAccounts_refetchableFragment } from '@/queries/__generated__/organizationMarketplaceSetup_organizationStripeConnectAccounts_refetchableFragment.graphql';
import type { organizationMarketplaceSetup_products_query$key } from '@/queries/__generated__/organizationMarketplaceSetup_products_query.graphql';
import type { organizationMarketplaceSetup_products_refetchableFragment } from '@/queries/__generated__/organizationMarketplaceSetup_products_refetchableFragment.graphql';
import type { organizationMarketplaceSetup_productTags_query$key } from '@/queries/__generated__/organizationMarketplaceSetup_productTags_query.graphql';
import type { organizationMarketplaceSetup_productTags_refetchableFragment } from '@/queries/__generated__/organizationMarketplaceSetup_productTags_refetchableFragment.graphql';
import type { organizationMarketplaceSetup_query$key } from '@/queries/__generated__/organizationMarketplaceSetup_query.graphql';
import type { organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation } from '@/queries/__generated__/organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation.graphql';
import type { organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation } from '@/queries/__generated__/organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import type { TCountryCode } from 'countries-list';
import { getCountryData } from 'countries-list';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import OrganizationMarketplaceSetupLeftSideNavigationMenuContent from './organization-marketplace-setup-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationMarketplaceSetup_query$key;
  rootDataProductsRelay: organizationMarketplaceSetup_products_query$key;
  rootDataProductTagsRelay: organizationMarketplaceSetup_productTags_query$key;
  rootDataLocationTagsRelay: organizationMarketplaceSetup_locationTags_query$key;
  rootDataOrganizationStripeConnectAccountsRelay: organizationMarketplaceSetup_organizationStripeConnectAccounts_query$key;
  rootDataOrganizationBankAccountsRelay: organizationMarketplaceSetup_organizationBankAccounts_query$key;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
};

type ProductRowType = {
  id: string;
  name: string;
  price: string;
  numberOfResourcesToBook: number;
  minDurationMinutes: number | null | undefined;
  maxDurationMinutes: number | null | undefined;
  bookAllLocationResources: boolean;
  recurrenceWindowDays: number;
  requireConsecutiveDays: boolean;
  maxBookingSpreadDays: number | null | undefined;
  status: boolean;
  isPriceTaxInclusive: boolean;
};

type ProductTagRowType = {
  id: string;
  name: string;
  description: string | null | undefined;
};

type LocationTagRowType = {
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

const OrganizationMarketplaceSetup = ({
  rootDataRelay,
  rootDataProductsRelay,
  rootDataProductTagsRelay,
  rootDataLocationTagsRelay,
  rootDataOrganizationStripeConnectAccountsRelay,
  rootDataOrganizationBankAccountsRelay,
  onReloadRequired,
  organizationUniqueAlphanumericName,
}: Props) => {
  const rootData = useFragment<organizationMarketplaceSetup_query$key>(
    graphql`
      fragment organizationMarketplaceSetup_query on Query {
        ...existingStripeConnectAccountButton_query
      }
    `,
    rootDataRelay,
  );

  const [rootDataProducts, refetchProducts] = useRefetchableFragment<organizationMarketplaceSetup_products_refetchableFragment, organizationMarketplaceSetup_products_query$key>(
    graphql`
      fragment organizationMarketplaceSetup_products_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationMarketplaceSetup_products_refetchableFragment") {
        products(
          first: $count
          after: $cursor
          where: { organizationUniqueAlphanumericNames: [$organizationUniqueAlphanumericName], nameContains: $productNameSearchText, includeInactive: true }
          orderBy: [{ direction: ASCENDING, field: NAME }]
        ) @connection(key: "organizationMarketplaceSetup_products") {
          __id
          totalCount
          edges {
            node {
              id
              inactive
              name
              description
              priceToDisplay
              priceUnit {
                name
              }
              numberOfResourcesToBook
              minDurationMinutes
              maxDurationMinutes
              bookAllLocationResources
              recurrenceWindowDays
              requireConsecutiveDays
              maxBookingSpreadDays
              organization {
                uniqueAlphanumericName
              }
              isPriceTaxInclusive
            }
          }
        }
      }
    `,
    rootDataProductsRelay,
  );

  const [rootDataProductTags, refetchProductTags] = useRefetchableFragment<
    organizationMarketplaceSetup_productTags_refetchableFragment,
    organizationMarketplaceSetup_productTags_query$key
  >(
    graphql`
      fragment organizationMarketplaceSetup_productTags_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationMarketplaceSetup_productTags_refetchableFragment") {
        productTags(
          first: $count
          after: $cursor
          where: { organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, nameContains: $productTagNameSearchText }
          orderBy: [{ direction: ASCENDING, field: NAME }]
        ) @connection(key: "organizationMarketplaceSetup_productTags") {
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
    `,
    rootDataProductTagsRelay,
  );

  const [rootDataLocationTags, refetchLocationTags] = useRefetchableFragment<
    organizationMarketplaceSetup_locationTags_refetchableFragment,
    organizationMarketplaceSetup_locationTags_query$key
  >(
    graphql`
      fragment organizationMarketplaceSetup_locationTags_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationMarketplaceSetup_locationTags_refetchableFragment") {
        locationTags(
          first: $count
          after: $cursor
          where: { organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, nameContains: $locationTagNameSearchText }
          orderBy: [{ direction: ASCENDING, field: NAME }]
        ) @connection(key: "organizationMarketplaceSetup_locationTags") {
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
    `,
    rootDataLocationTagsRelay,
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
          where: { organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, nameContains: $organizationStripeConnectAccountNameSearchText }
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
                uniqueAlphanumericName
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
          where: { organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, nameContains: $organizationBankAccountNameSearchText }
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
                uniqueAlphanumericName
              }
            }
          }
        }
      }
    `,
    rootDataOrganizationBankAccountsRelay,
  );

  const [commitDeleteProduct] = useMutation<organizationMarketplaceSetup_deleteProductsMutation>(graphql`
    mutation organizationMarketplaceSetup_deleteProductsMutation($connectionIds: [ID!]!, $input: DeleteProductsInput!) {
      deleteProducts(input: $input) {
        products {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitDeleteProductTags] = useMutation<organizationMarketplaceSetup_deleteProductTagsMutation>(graphql`
    mutation organizationMarketplaceSetup_deleteProductTagsMutation($connectionIds: [ID!]!, $input: DeleteProductTagsInput!) {
      deleteProductTags(input: $input) {
        organizationTags {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitDeleteLocationTags] = useMutation<organizationMarketplaceSetup_deleteLocationTagsMutation>(graphql`
    mutation organizationMarketplaceSetup_deleteLocationTagsMutation($connectionIds: [ID!]!, $input: DeleteLocationTagsInput!) {
      deleteLocationTags(input: $input) {
        organizationTags {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitActivateProducts] = useMutation<organizationMarketplaceSetup_activateProductsMutation>(graphql`
    mutation organizationMarketplaceSetup_activateProductsMutation($input: ActivateProductsInput!) @raw_response_type {
      activateProducts(input: $input) {
        products {
          id
          inactive
        }
      }
    }
  `);

  const [commitDeactivateProducts] = useMutation<organizationMarketplaceSetup_deactivateProductsMutation>(graphql`
    mutation organizationMarketplaceSetup_deactivateProductsMutation($input: DeactivateProductsInput!) @raw_response_type {
      deactivateProducts(input: $input) {
        products {
          id
          inactive
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

  const { integratedPlatrform } = useIntegratedPlatrform();
  const [, startTransition] = useTransition();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const router = useRouter();
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});

  const [productNameSearchText, setProductNameSearchText] = useState<string>('');
  const [seledctedProducts, setSeledctedProducts] = useState<GridRowSelectionModel>(defaultGridRowSelectionModelValue);
  const [selectedProductId, setSelectedProductId] = useState<null | string>(null);
  const [productMoreActionsAnchorEl, setProductMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const productMoreActionsMenuOpen = Boolean(productMoreActionsAnchorEl);
  const products = useMemo(() => rootDataProducts.products.edges.map(({ node }) => node), [rootDataProducts.products]);
  const productsConnectionIds = useMemo(() => [rootDataProducts.products.__id], [rootDataProducts.products]);
  const productMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditProduct],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteProduct],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.ActivateProduct],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeactivateProduct],
  ];
  const productDetails = useMemo(() => products.find((item) => item.id === selectedProductId), [selectedProductId, products]);

  const handleRefetchProducts = useCallback(
    (productNameSearchText: string) => {
      startTransition(() => {
        refetchProducts(
          {
            productNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchProducts],
  );

  const [productTagNameSearchText, setProductTagNameSearchText] = useState<string>('');
  const [seledctedProductTags, setSeledctedProductTags] = useState<GridRowSelectionModel>(defaultGridRowSelectionModelValue);
  const [selectedProductTagId, setSelectedProductTagId] = useState<null | string>(null);
  const [productTagMoreActionsAnchorEl, setProductTagMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const productTagMoreActionsMenuOpen = Boolean(productTagMoreActionsAnchorEl);
  const [isEditProductTagDialogOpen, setIsEditProductTagDialogOpen] = useState(false);
  const productTags = useMemo(() => rootDataProductTags.productTags.edges.map(({ node }) => node), [rootDataProductTags.productTags]);
  const productTagsConnectionIds = useMemo(() => [rootDataProductTags.productTags.__id], [rootDataProductTags.productTags]);
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
    [refetchProductTags],
  );

  const [locationTagNameSearchText, setLocationTagNameSearchText] = useState<string>('');
  const [seledctedLocationTags, setSeledctedLocationTags] = useState<GridRowSelectionModel>(defaultGridRowSelectionModelValue);
  const [selectedLocationTagId, setSelectedLocationTagId] = useState<null | string>(null);
  const [locationTagMoreActionsAnchorEl, setLocationTagMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const locationTagMoreActionsMenuOpen = Boolean(locationTagMoreActionsAnchorEl);
  const [isEditLocationTagDialogOpen, setIsEditLocationTagDialogOpen] = useState(false);
  const locationTags = useMemo(() => rootDataLocationTags.locationTags.edges.map(({ node }) => node), [rootDataLocationTags.locationTags]);
  const locationTagsConnectionIds = useMemo(() => [rootDataLocationTags.locationTags.__id], [rootDataLocationTags.locationTags]);
  const locationTagMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditLocationTag],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteLocationTag],
  ];

  const handleRefetchLocationTags = useCallback(
    (locationTagNameSearchText: string) => {
      startTransition(() => {
        refetchLocationTags(
          {
            locationTagNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchLocationTags],
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
    [refetchOrganizationStripeConnectAccounts],
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
    [refetchOrganizationBankAccounts],
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

  const handleProductsSearchTextChange = (str: string) => {
    setProductNameSearchText(str);

    handleRefetchProducts(str);
  };

  const handleSelectedProductsChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedProducts(newRowSelectionModel);
  };

  const handleProductMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setProductMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditProduct:
        if (!productDetails) {
          return;
        }

        router.push(getOrganizationProductBaseLink(integratedPlatrform, productDetails.organization!.uniqueAlphanumericName!, productDetails.id));
        break;

      case MoreActionsMenuOptionType.DeleteProduct:
        handleRemoveProductClick();
        break;

      case MoreActionsMenuOptionType.ActivateProduct:
        handleActivateProductClick();
        break;

      case MoreActionsMenuOptionType.DeactivateProduct:
        handleDeactivateProductClick();
        break;
    }
  };

  const handleRemoveProductsClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing product ..." />, infoNotificationOptions);

    commitDeleteProduct({
      variables: {
        connectionIds: productsConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: seledctedProducts.ids
            .values()
            .map((id) => id as string)
            .toArray(),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove product. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Products removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove product. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveProductClick = () => {
    if (!selectedProductId) {
      return;
    }

    const toastId = themedToast(<NotificationContent content="Removing product ..." />, infoNotificationOptions);

    commitDeleteProduct({
      variables: {
        connectionIds: productsConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: [selectedProductId],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove product. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Product removed.`} />,
        });

        setSelectedProductId(null);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove product. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleDeactivateProductsClick = () => {
    const toastId = themedToast(<NotificationContent content={'Deactivating products...'} />, infoNotificationOptions);

    commitDeactivateProducts({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids: seledctedProducts.ids
            .values()
            .map((id) => id as string)
            .toArray(),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate products. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Products deactivated.'} />,
        });
        setSeledctedProducts(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate products. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleActivateProductsClick = () => {
    const toastId = themedToast(<NotificationContent content={'Activating products...'} />, infoNotificationOptions);
    const ids = seledctedProducts.ids
      .values()
      .map((id) => id as string)
      .toArray();

    commitActivateProducts({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate products. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Products activated.'} />,
        });
        setSeledctedProducts(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate products. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        activateProducts: {
          products: ids.map((id) => ({
            id,
            inactive: false,
          })),
        },
      },
    });
  };

  const handleDeactivateProductClick = () => {
    if (!productDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Deactivating product...'} />, infoNotificationOptions);

    commitDeactivateProducts({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids: [productDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate product. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Product deactivated.'} />,
        });
        setSeledctedProducts(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate product. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        deactivateProducts: {
          products: [
            {
              id: productDetails.id,
              inactive: true,
            },
          ],
        },
      },
    });
  };

  const handleActivateProductClick = () => {
    if (!productDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Activating product...'} />, infoNotificationOptions);

    commitActivateProducts({
      variables: {
        input: {
          clientMutationId: uuid(),
          ids: [productDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate product. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Product activated.'} />,
        });
        setSeledctedProducts(defaultGridRowSelectionModelValue);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate product. Error: ${error.message}.`} />,
        });
      },
    });
  };

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
            render: <NotificationContent content={`Failed to remove product tags. Error: ${joinErrors(errors)}.`} />,
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
            render: <NotificationContent content={`Failed to remove product tag. Error: ${joinErrors(errors)}.`} />,
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

  const handleLocationTagsSearchTextChange = (str: string) => {
    setLocationTagNameSearchText(str);

    handleRefetchLocationTags(str);
  };

  const handleSelectedLocationTagsChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedLocationTags(newRowSelectionModel);
  };

  const handleLocationTagMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setLocationTagMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditLocationTag:
        setIsEditLocationTagDialogOpen(true);
        break;

      case MoreActionsMenuOptionType.DeleteLocationTag:
        handleRemoveLocationTagClick();
        break;
    }
  };

  const handleEditLocationTagClick = () => {
    setIsEditLocationTagDialogOpen(false);
  };

  const handleEditLocationTagCancel = () => {
    setIsEditLocationTagDialogOpen(false);
  };

  const handleRemoveLocationTagsClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing location tags ..." />, infoNotificationOptions);

    commitDeleteLocationTags({
      variables: {
        connectionIds: locationTagsConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: seledctedLocationTags.ids
            .values()
            .map((id) => id as string)
            .toArray(),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove location tags. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location tags removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove location tags. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveLocationTagClick = () => {
    if (!selectedLocationTagId) {
      return;
    }

    const toastId = themedToast(<NotificationContent content="Removing location tag ..." />, infoNotificationOptions);

    commitDeleteLocationTags({
      variables: {
        connectionIds: locationTagsConnectionIds,
        input: {
          clientMutationId: uuid(),
          ids: [selectedLocationTagId],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove location tag. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location tag removed.`} />,
        });

        setSelectedLocationTagId(null);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove location tag. Error: ${error.message}.`} />,
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
            organizationStripeConnectAccountDetails.organization!.uniqueAlphanumericName!,
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
              <NotificationContent content={`Failed to set stripe connect account ${organizationStripeConnectAccountDetails.name} as default. Error: ${joinErrors(errors)}`} />
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
            render: <NotificationContent content={`Failed to remove Stripe Connect accounts. Error: ${joinErrors(errors)}.`} />,
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
            render: <NotificationContent content={`Failed to remove Stripe Connect account. Error: ${joinErrors(errors)}.`} />,
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

        router.push(
          getOrganizationBankAccountBaseLink(integratedPlatrform, organizationBankAccountDetails.organization!.uniqueAlphanumericName!, organizationBankAccountDetails.id),
        );
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
            render: <NotificationContent content={`Failed to set bank account ${organizationBankAccountDetails.name} as default. Error: ${joinErrors(errors)}`} />,
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
            render: <NotificationContent content={`Failed to remove Bank accounts. Error: ${joinErrors(errors)}.`} />,
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
            render: <NotificationContent content={`Failed to remove Bank accounts. Error: ${joinErrors(errors)}.`} />,
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
    router.push(getOrganizationBaseLink(integratedPlatrform, organizationUniqueAlphanumericName));
  };

  const productRows: ProductRowType[] = products.map((product) => ({
    id: product.id,
    name: product.name,
    price: product.priceToDisplay,
    numberOfResourcesToBook: product.numberOfResourcesToBook,
    minDurationMinutes: product.minDurationMinutes,
    maxDurationMinutes: product.maxDurationMinutes,
    bookAllLocationResources: product.bookAllLocationResources,
    recurrenceWindowDays: product.recurrenceWindowDays,
    requireConsecutiveDays: product.requireConsecutiveDays,
    maxBookingSpreadDays: product.maxBookingSpreadDays,
    status: !product.inactive,
    isPriceTaxInclusive: product.isPriceTaxInclusive,
  }));

  const productColumns: GridColDef<(typeof productRows)[number]>[] = [
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'price',
      headerName: 'Price',
      editable: false,
      renderCell: (params) => {
        const product = products.find((product) => product.id === (params.id as string));

        return <SmallIconTypography label={`${params.value} - ${product?.priceUnit.name}`} />;
      },
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'isPriceTaxInclusive',
      headerName: 'Tax Inclusive?',
      editable: false,
      renderCell: (params) => {
        return <SmallIconTypography label={`${params.value ? 'Yes' : 'No'}`} />;
      },
      display: 'flex',
      minWidth: 120,
    },
    {
      field: 'numberOfResourcesToBook',
      headerName: 'Number of resources to book',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 50,
    },
    {
      field: 'minDurationMinutes',
      headerName: 'Min duration',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value ? `${params.value} minutes` : 'No limit'} />,
      display: 'flex',
      minWidth: 120,
    },
    {
      field: 'maxDurationMinutes',
      headerName: 'Max duration',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value ? `${params.value} minutes` : 'No limit'} />,
      display: 'flex',
      minWidth: 120,
    },
    {
      field: 'bookAllLocationResources',
      headerName: 'Book all location resources',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value ? 'Yes' : 'No'} />,
      display: 'flex',
      minWidth: 100,
    },
    {
      field: 'recurrenceWindowDays',
      headerName: 'Recurrence window days',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 50,
    },
    {
      field: 'requireConsecutiveDays',
      headerName: 'Must book consecutive days',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value ? 'Yes' : 'No'} />,
      display: 'flex',
      minWidth: 50,
    },
    {
      field: 'maxBookingSpreadDays',
      headerName: 'Max booking spread days',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value ? params.value.toString() : 'No limit'} />,
      display: 'flex',
      minWidth: 50,
    },
    {
      field: 'status',
      headerName: 'Status',
      editable: false,
      renderCell: (params) => (
        <StackRow>
          {params.value && (
            <StackRow sx={{ justifyContent: 'space-between', width: 76 }}>
              <SmallIconTypography label="Active" />
              <Box sx={{ width: 15, height: 15, borderRadius: '50%', backgroundColor: emerald }} />
            </StackRow>
          )}
          {!params.value && (
            <StackRow sx={{ justifyContent: 'space-between', width: 76 }}>
              <SmallIconTypography label="Inactive" />
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
              setSelectedProductId(params.id as string);
              setProductMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

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
          return <></>;
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

  const locationTagRows: LocationTagRowType[] = locationTags.map((locationTag) => ({
    id: locationTag.id,
    name: locationTag.name,
    description: locationTag.description,
  }));

  const locationTagColumns: GridColDef<(typeof locationTagRows)[number]>[] = [
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => {
        const locationTag = locationTags.find((locationTag) => locationTag.id === (params.id as string));
        if (!locationTag) {
          return <></>;
        }

        return <LocationTag locationTag={locationTag} showFullName />;
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
              setSelectedLocationTagId(params.id as string);
              setLocationTagMoreActionsAnchorEl(event.currentTarget);
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

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationMarketplaceSetupLeftSideNavigationMenuContent organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} hideIcons />
        <Box sx={{ marginLeft: secondDrawerExpandedDrawerWidthPx, flexGrow: 1 }}>
          <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Marketplace Information">
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
                  <NewStripeConnectAccountButton organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} />
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
                  <NewBankAccountButton organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} />
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
                  <AddOrganizationProductTagButton organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} connectionIds={productTagsConnectionIds} />
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
            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['location-tags-setup'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Location Tags" />
                  <BodyIconTypography label="Edit your organization location tags details" />
                </Grid>

                <Grid>
                  <AddOrganizationLocationTagButton organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} connectionIds={locationTagsConnectionIds} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>
            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <PushToRight />
              <Search size="small" placeholder="Search for location tags" defaultValue={locationTagNameSearchText} onChange={handleLocationTagsSearchTextChange} />
            </GridContainer>
            {seledctedLocationTags.ids.size > 0 && (
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
                    <SmallIconTypography label={`${seledctedLocationTags.ids.size} records selected`} />
                    <PushToRight />
                    <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveLocationTagsClick} sx={{ textTransform: 'none' }}>
                      Remove Location Tag
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}
            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedLocationTags}
                onRowSelectionModelChange={handleSelectedLocationTagsChanged}
                rows={locationTagRows}
                columns={locationTagColumns}
                hideFooterPagination={locationTagRows.length <= 10}
                initialState={{
                  pagination: {
                    rowCount: locationTagRows.length,
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
                localeText={{ noRowsLabel: 'No location tag found' }}
              />
            </StackRow>
            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['product-setup'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label="Product" />
                  <BodyIconTypography label="Edit your organization products details" />
                </Grid>

                <Grid>
                  <NewProductButton organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>
            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <PushToRight />
              <Search size="small" placeholder="Search for product" defaultValue={productNameSearchText} onChange={handleProductsSearchTextChange} />
            </GridContainer>
            {seledctedProducts.ids.size > 0 && (
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
                    <SmallIconTypography label={`${seledctedProducts.ids.size} records selected`} />
                    <PushToRight />
                    <Button size="medium" variant="contained" color="secondary" onClick={handleDeactivateProductsClick} sx={defaultButtonStyle}>
                      Deactivate Product
                    </Button>
                    <Button size="medium" variant="contained" color="secondary" onClick={handleActivateProductsClick} sx={defaultButtonStyle}>
                      Activate Product
                    </Button>
                    <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={handleRemoveProductsClick} sx={{ textTransform: 'none' }}>
                      Remove Product
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}
            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedProducts}
                onRowSelectionModelChange={handleSelectedProductsChanged}
                rows={productRows}
                columns={productColumns}
                hideFooterPagination={productRows.length <= 10}
                initialState={{
                  pagination: {
                    rowCount: productRows.length,
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
                localeText={{ noRowsLabel: 'No product found' }}
              />
            </StackRow>{' '}
          </AppBarWithStackColumn>
        </Box>
      </Box>

      <MoreActionsMenu
        anchorEl={productMoreActionsAnchorEl}
        open={productMoreActionsMenuOpen}
        onMenuItemClick={handleProductMoreActionsMenuItemClick}
        options={productMoreActionsOption}
      />

      <MoreActionsMenu
        anchorEl={productTagMoreActionsAnchorEl}
        open={productTagMoreActionsMenuOpen}
        onMenuItemClick={handleProductTagMoreActionsMenuItemClick}
        options={productTagMoreActionsOption}
      />

      <MoreActionsMenu
        anchorEl={locationTagMoreActionsAnchorEl}
        open={locationTagMoreActionsMenuOpen}
        onMenuItemClick={handleLocationTagMoreActionsMenuItemClick}
        options={locationTagMoreActionsOption}
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

      {selectedLocationTagId && (
        <EditOrganizationLocationTagDialog
          onReloadRequired={onReloadRequired}
          locationTagId={selectedLocationTagId}
          isDialogOpen={isEditLocationTagDialogOpen}
          onAddClicked={handleEditLocationTagClick}
          onCancel={handleEditLocationTagCancel}
        />
      )}
    </>
  );
};

export default memo(OrganizationMarketplaceSetup);

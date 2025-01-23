import { getOrganizationBaseLink } from '@/components/links';
import { OrganizationMultipleChoicesIndustries } from '@/components/organization';
import { AddOrganizationCustomTagButton } from '@/components/organization/addOrganizationCustomTag';
import { AddOrganizationPaymentMethodDialog } from '@/components/organization/addOrganizationPaymentMethod';
import { AddOrganizationZoneButton } from '@/components/organization/addOrganizationZone';
import { EditOrganizationZoneDialog } from '@/components/organization/editOrganizationZone/';
import type { organizationAdmin_addCustomerDefaultOrganizationTagMutation } from '@/queries/__generated__/organizationAdmin_addCustomerDefaultOrganizationTagMutation.graphql';
import type { organizationAdmin_cancelOrganizationOfferingMutation } from '@/queries/__generated__/organizationAdmin_cancelOrganizationOfferingMutation.graphql';
import type { organizationAdmin_customTags_query$key } from '@/queries/__generated__/organizationAdmin_customTags_query.graphql';
import type { organizationAdmin_customTags_refetchableFragment } from '@/queries/__generated__/organizationAdmin_customTags_refetchableFragment.graphql';
import type { organizationAdmin_deleteCustomTagsMutation } from '@/queries/__generated__/organizationAdmin_deleteCustomTagsMutation.graphql';
import type { organizationAdmin_deleteZonesMutation } from '@/queries/__generated__/organizationAdmin_deleteZonesMutation.graphql';
import type { organizationAdmin_organizationPaymentMethodsDetails_query$key } from '@/queries/__generated__/organizationAdmin_organizationPaymentMethodsDetails_query.graphql';
import type { organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment } from '@/queries/__generated__/organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment.graphql';
import type { organizationAdmin_query$key } from '@/queries/__generated__/organizationAdmin_query.graphql';
import type { organizationAdmin_removeCustomerDefaultOrganizationTagMutation } from '@/queries/__generated__/organizationAdmin_removeCustomerDefaultOrganizationTagMutation.graphql';
import type { organizationAdmin_removeOrganizationPaymentMethodMutation } from '@/queries/__generated__/organizationAdmin_removeOrganizationPaymentMethodMutation.graphql';
import type { organizationAdmin_setOrganizationBillingInfoMutation } from '@/queries/__generated__/organizationAdmin_setOrganizationBillingInfoMutation.graphql';
import type { organizationAdmin_updateOrganizationMutation } from '@/queries/__generated__/organizationAdmin_updateOrganizationMutation.graphql';
import type { organizationAdmin_updateOrganizationOfferingMutation } from '@/queries/__generated__/organizationAdmin_updateOrganizationOfferingMutation.graphql';
import type { organizationAdmin_zones_query$key } from '@/queries/__generated__/organizationAdmin_zones_query.graphql';
import type { organizationAdmin_zones_refetchableFragment } from '@/queries/__generated__/organizationAdmin_zones_refetchableFragment.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import IconButton from '@mui/material/IconButton';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import {
  AppBarWithStackColumn,
  BodyIconTypography,
  CreditCard,
  FormFieldLabel,
  FormStackColumn,
  GridContainer,
  LeadIconTypography,
  PushToRight,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@repo/shared/components/commons';
import { CustomTag } from '@repo/shared/components/customTag';
import { SingleChoiceCountry } from '@repo/shared/components/forms';
import { DeleteIcon, EllipseMenuIcon, ErrorIcon, NewIcon, NotPreferredIcon, PreferredIcon, TickIcon } from '@repo/shared/components/icons';
import {
  MoreActionsMenu,
  moreActionsMenuAllOptions,
  MoreActionsMenuItemType,
  MoreActionsMenuOptionType,
} from '@repo/shared/components/moreActionsMenu';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { Search } from '@repo/shared/components/search';
import { Zone } from '@repo/shared/components/zone';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { defaultButtonStyle, defaultGridActionPadding, defaultGridStyle, defaultPadding } from '@repo/shared/libs/theme';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';
import EditOrganizationCustomTagDialog from '../editOrganizationCustomTag/edit-organization-custom-tag-dialog';
import { expandedDrawerWidthPx } from './commons';
import OrganizationAdminLeftSideNavigationMenuContent from './organization-admin-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationAdmin_query$key;
  rootDataOrganizationPaymentMethodsDetailsRelay: organizationAdmin_organizationPaymentMethodsDetails_query$key;
  rootDataZonesRelay: organizationAdmin_zones_query$key;
  rootDataCustomTagsRelay: organizationAdmin_customTags_query$key;
  onReloadRequired: () => void;
  organizationId: string;
};

type OrganizationDetails = {
  name: string;
  about: string | null;
  website: string | null;
  industrySubCategoryIds: string[];
};

const organizationSchema = object({
  name: string().min(3, 'Organization name must be at least three characters long.').required('Organization name is required'),
  about: string().nullable(),
  website: string().nullable(),
  industrySubCategoryIds: array().nullable(),
});

type OrganizationBillingDetails = {
  billingEmail: string;
  billingAddressLine1: string | null;
  billingAddressLine2: string | null;
  billingSuburb: string | null;
  billingCity: string | null;
  billingProvince: string | null;
  billingZipcode: string | null;
  billingCountry: string | null;
};

const organizationBillingSchema = object({
  billingEmail: string().email(({ value }) => `${value} is not a valid email`),
  billingAddressLine1: string().nullable(),
  billingAddressLine2: string().nullable(),
  billingSuburb: string().nullable(),
  billingCity: string().nullable(),
  billingProvince: string().nullable(),
  billingZipcode: string().nullable(),
  billingCountry: string().nullable(),
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

const OrganizationAdmin = ({
  rootDataRelay,
  rootDataOrganizationPaymentMethodsDetailsRelay,
  rootDataZonesRelay,
  rootDataCustomTagsRelay,
  onReloadRequired,
  organizationId,
}: Props) => {
  const rootData = useFragment<organizationAdmin_query$key>(
    graphql`
      fragment organizationAdmin_query on Query {
        me {
          id
          preferredZones {
            uniqueId
          }
          preferredCustomTags {
            uniqueId
          }
        }
        organization(id: $organizationId) {
          id
          name
          logoUrl
          about
          website
          canModify
          industrySubCategories {
            id
            name
          }
          hasAttachedPaymentMethod
          activeOffering {
            id
            name
            startColor
            endColor
            colorTiltingAngle
            start
            end
            unitPrice
            featureSet {
              name
              description
            }
            free
          }
          availableOfferings {
            code
            name
            startColor
            endColor
            colorTiltingAngle
            unitPrice
            featureSet {
              name
              description
            }
          }
        }
        organizationIndustryMainCategoriesReferences {
          subCategories {
            id
            name
          }
        }
        organizationBillingInfo(organizationId: $organizationId) {
          id
          email
          addressLine1
          addressLine2
          suburb
          city
          province
          zipcode
          country
        }
        ...organizationMultipleChoicesIndustries_query
      }
    `,
    rootDataRelay,
  );

  const [rootDataOrganizationPaymentMethodsDetails, refetchOrganizationPaymentMethodsDetails] = useRefetchableFragment<
    organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment,
    organizationAdmin_organizationPaymentMethodsDetails_query$key
  >(
    graphql`
      fragment organizationAdmin_organizationPaymentMethodsDetails_query on Query
      @refetchable(queryName: "organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment") {
        organizationPaymentMethodsDetails(organizationId: $organizationId) {
          id
          cardBrand
          cardExpiryMonth
          cardExpiryYear
          cardLastFourDigit
        }
      }
    `,
    rootDataOrganizationPaymentMethodsDetailsRelay,
  );

  const [rootDataZones, refetchZones] = useRefetchableFragment<organizationAdmin_zones_refetchableFragment, organizationAdmin_zones_query$key>(
    graphql`
      fragment organizationAdmin_zones_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationAdmin_zones_refetchableFragment") {
        zones(first: $count, after: $cursor, where: { organizationId: $organizationId, nameContains: $zoneNameSearchText })
          @connection(key: "organizationAdmin_zones") {
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
    rootDataZonesRelay,
  );

  const [rootDataCustomTags, refetchCustomTags] = useRefetchableFragment<
    organizationAdmin_customTags_refetchableFragment,
    organizationAdmin_customTags_query$key
  >(
    graphql`
      fragment organizationAdmin_customTags_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationAdmin_customTags_refetchableFragment") {
        customTags(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $customTagNameSearchText }
          orderBy: [{ direction: Ascending, field: Name }]
        ) @connection(key: "organizationAdmin_customTags") {
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
    rootDataCustomTagsRelay,
  );

  const [commitUpdateOrganization] = useMutation<organizationAdmin_updateOrganizationMutation>(graphql`
    mutation organizationAdmin_updateOrganizationMutation($input: UpdateOrganizationInput!) @raw_response_type {
      updateOrganization(input: $input) {
        organization {
          id
          name
          about
          website
          industrySubCategories {
            id
            name
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

  const [commitSetOrganizationBillingInfo] = useMutation<organizationAdmin_setOrganizationBillingInfoMutation>(graphql`
    mutation organizationAdmin_setOrganizationBillingInfoMutation($input: SetOrganizationBillingInfoInput!) @raw_response_type {
      setOrganizationBillingInfo(input: $input) {
        organizationBillingInfo {
          id
          email
          addressLine1
          addressLine2
          suburb
          city
          province
          zipcode
          country
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

  const [commitAddCustomerDefaultOrganizationTag] = useMutation<organizationAdmin_addCustomerDefaultOrganizationTagMutation>(graphql`
    mutation organizationAdmin_addCustomerDefaultOrganizationTagMutation($input: AddCustomerDefaultOrganizationTagInput!) {
      addCustomerDefaultOrganizationTag(input: $input) {
        customer {
          id
          preferredZones {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerDefaultOrganizationTag] = useMutation<organizationAdmin_removeCustomerDefaultOrganizationTagMutation>(graphql`
    mutation organizationAdmin_removeCustomerDefaultOrganizationTagMutation($input: RemoveCustomerDefaultOrganizationTagInput!) {
      removeCustomerDefaultOrganizationTag(input: $input) {
        customer {
          id
          preferredZones {
            uniqueId
          }
        }
      }
    }
  `);

  const [, startTransition] = useTransition();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const router = useRouter();
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const validateOrganizationDetails = makeValidate(organizationSchema);
  const requiredOrganizationDetailsFields = makeRequired(organizationSchema);
  const validateOrganizationBilling = makeValidate(organizationBillingSchema);
  const requiredOrganizationBillingFields = makeRequired(organizationBillingSchema);
  const [zoneNameSearchText, setZoneNameSearchText] = useState<string>('');
  const [customTagNameSearchText, setCustomTagNameSearchText] = useState<string>('');
  const [seledctedZones, setSeledctedZones] = useState<GridRowSelectionModel>([]);
  const [seledctedCustomTags, setSeledctedCustomTags] = useState<GridRowSelectionModel>([]);
  const [selectedZoneId, setSelectedZoneId] = useState<null | string>(null);
  const [selectedCustomTagId, setSelectedCustomTagId] = useState<null | string>(null);
  const [zoneMoreActionsAnchorEl, setZoneMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const zoneMoreActionsMenuOpen = Boolean(zoneMoreActionsAnchorEl);
  const [customTagMoreActionsAnchorEl, setCustomTagMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const customTagMoreActionsMenuOpen = Boolean(customTagMoreActionsAnchorEl);
  const [isEditZoneDialogOpen, setIsEditZoneDialogOpen] = useState(false);
  const [isEditCustomTagDialogOpen, setIsEditCustomTagDialogOpen] = useState(false);
  const [isAddPaymentMethodDialogOpen, setIsAddPaymentMethodDialogOpen] = useState(false);
  const [preferredZones, setPreferredZones] = useState(rootData.me?.preferredZones.map(({ uniqueId }) => uniqueId) ?? []);
  const [preferredCustomTags, setPreferredCustomTags] = useState(rootData.me?.preferredCustomTags.map(({ uniqueId }) => uniqueId) ?? []);

  const zoneMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditZone],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteZone],
  ];

  const customTagMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditCustomTag],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteCustomTag],
  ];

  const zonesConnectionIds = useMemo(() => (rootDataZones.zones ? [rootDataZones.zones.__id] : []), [rootDataZones.zones]);
  const zones = useMemo(() => {
    if (!rootDataZones.zones) {
      return [];
    }

    return rootDataZones.zones.edges.map(({ node }) => node);
  }, [rootDataZones.zones]);

  const customTagsConnectionIds = useMemo(
    () => (rootDataCustomTags.customTags ? [rootDataCustomTags.customTags.__id] : []),
    [rootDataCustomTags.customTags],
  );
  const customTags = useMemo(() => {
    if (!rootDataCustomTags.customTags) {
      return [];
    }

    return rootDataCustomTags.customTags.edges.map(({ node }) => node);
  }, [rootDataCustomTags.customTags]);

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
    [refetchZones],
  );

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
    [refetchCustomTags],
  );

  const handleRefetchOrganizationPaymentMethodsDetails = useCallback(() => {
    startTransition(() => {
      refetchOrganizationPaymentMethodsDetails(
        {},
        {
          fetchPolicy: 'store-and-network',
        },
      );
    });
  }, [refetchOrganizationPaymentMethodsDetails]);

  const handleOrganizationDetailUpdateClick = ({ name, about, website, industrySubCategoryIds }: OrganizationDetails) => {
    if (!rootData.organization) {
      return;
    }

    const organization = rootData.organization;
    const selectedIndustrySubCategoryIds = industrySubCategoryIds ?? [];
    const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}'...`} />, infoNotificationOptions);

    commitUpdateOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: organization.id,
          name,
          about,
          website,
          industrySubCategoryIds: selectedIndustrySubCategoryIds,
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
            name,
            about,
            website,
            industrySubCategories: rootData.organizationIndustryMainCategoriesReferences
              .flatMap((mainCategory) => mainCategory.subCategories)
              .filter(({ id }) => selectedIndustrySubCategoryIds.find((selectedIndustrySubCategoryId) => selectedIndustrySubCategoryId === id))
              .map(({ id, name }) => ({ id, name })),
          },
        },
      },
    });
  };

  const handleOrganizationBillingDetailUpdateClick = ({
    billingEmail,
    billingAddressLine1,
    billingAddressLine2,
    billingSuburb,
    billingCity,
    billingProvince,
    billingZipcode,
    billingCountry,
  }: OrganizationBillingDetails) => {
    if (!rootData.organizationBillingInfo) {
      return;
    }

    const organizationBillingInfo = rootData.organizationBillingInfo;
    const toastId = themedToast(<NotificationContent content={`Updating organization '${organization.name}' billing...`} />, infoNotificationOptions);

    commitSetOrganizationBillingInfo({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationId: organization.id,
          email: billingEmail,
          addressLine1: billingAddressLine1,
          addressLine2: billingAddressLine2,
          suburb: billingSuburb,
          city: billingCity,
          province: billingProvince,
          zipcode: billingZipcode,
          country: billingCountry,
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
        setOrganizationBillingInfo: {
          organizationBillingInfo: {
            id: organizationBillingInfo.id,
            email: billingEmail,
            addressLine1: billingAddressLine1,
            addressLine2: billingAddressLine2,
            suburb: billingSuburb,
            city: billingCity,
            province: billingProvince,
            zipcode: billingZipcode,
            country: billingCountry,
          },
        },
      },
    });
  };

  const handleZonesSearchTextChange = (str: string) => {
    setZoneNameSearchText(str);

    handleRefetchZones(str);
  };

  const handleCustomTagsSearchTextChange = (str: string) => {
    setCustomTagNameSearchText(str);

    handleRefetchCustomTags(str);
  };

  const handleSelectedZonesChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedZones(newRowSelectionModel);
  };

  const handleSelectedCustomTagsChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedCustomTags(newRowSelectionModel);
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

  const handleRemoveZonesClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing zones ..." />, infoNotificationOptions);

    commitDeleteZones({
      variables: {
        connectionIds: zonesConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: seledctedZones.map((id) => id as string),
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
          clientMutationId: nanoid(),
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

  const handleRemoveCustomTagsClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing tags ..." />, infoNotificationOptions);

    commitDeleteCustomTags({
      variables: {
        connectionIds: customTagsConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: seledctedCustomTags.map((id) => id as string),
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
          clientMutationId: nanoid(),
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

  const handleEditZoneClick = () => {
    setIsEditZoneDialogOpen(false);
  };

  const onEditZoneCancel = () => {
    setIsEditZoneDialogOpen(false);
  };

  const handleEditCustomTagClick = () => {
    setIsEditCustomTagDialogOpen(false);
  };

  const handleEditCustomTagCancel = () => {
    setIsEditCustomTagDialogOpen(false);
  };

  const handleAddPaymentMethodClicked = () => {
    setIsAddPaymentMethodDialogOpen(true);
  };

  const handleAddPaymentMethodCancel = () => {
    setIsAddPaymentMethodDialogOpen(false);
  };

  const handleCloseClick = () => {
    router.push(getOrganizationBaseLink(organizationId));
  };

  const handleRemovePaymentMethodClick = (id: string) => {
    if (!rootData.organization) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing payment method...`} />, infoNotificationOptions);

    commitRemoveOrganizationPaymentMethod({
      variables: {
        input: {
          clientMutationId: nanoid(),
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
    if (!rootData.organization) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Cancelling organization '${rootData.organization.name}' active offering...`} />,
      infoNotificationOptions,
    );

    commitCancelOrganizationOffering({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: rootData.organization.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to cancel organization '${rootData.organization?.name}' active offering. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          onReloadRequired();

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization '${rootData.organization?.name}' active offering cancelled.`} />,
        });

        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent
              content={`Failed to cancel organization '${rootData.organization?.name}' active offering. Error: ${error.message}.`}
            />
          ),
        });

        onReloadRequired();
      },
    });
  };

  const handleUpgradeOfferingClick = (code: string) => {
    if (!rootData.organization) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Updating organization '${rootData.organization.name} active offering'...`} />,
      infoNotificationOptions,
    );

    commitUpdateOrganizationOffering({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: rootData.organization.id,
          offeringCode: code,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to update organization ${rootData.organization?.name} active offering. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          onReloadRequired();

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization ${rootData.organization?.name} active offering updated.`} />,
        });

        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent content={`Failed to update organization ${rootData.organization?.name} active offering. Error: ${error.message}.`} />
          ),
        });

        onReloadRequired();
      },
    });
  };

  const handleSetAsPreferredZoneClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const organizationTagDetails = zones.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Setting zone '${organizationTagDetails.name}' as your preferred zone...`} />,
      infoNotificationOptions,
    );

    commitAddCustomerDefaultOrganizationTag({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to set zone '${organizationTagDetails.name}' as your preferred zone. Error: ${joinErrors(errors)}.`}
              />
            ),
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
          render: (
            <NotificationContent content={`Failed to set zone '${organizationTagDetails.name}' as your preferred zone. Error: ${error.message}.`} />
          ),
        });
      },
    });
  };

  const handleRemoveAsPreferredZoneClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const organizationTagDetails = zones.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Removing zone '${organizationTagDetails.name}' as your preferred zone...`} />,
      infoNotificationOptions,
    );

    commitRemoveCustomerDefaultOrganizationTag({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to remove the zone '${organizationTagDetails.name}' as your preferred zone. Error: ${joinErrors(errors)}.`}
              />
            ),
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
          render: (
            <NotificationContent
              content={`Failed to remove the zone '${organizationTagDetails.name}' as your preferred zone. Error: ${error.message}.`}
            />
          ),
        });
      },
    });
  };

  const handleSetAsPreferredCustomTagClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const organizationTagDetails = customTags.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Setting tag '${organizationTagDetails.name}' as your preferred tag...`} />,
      infoNotificationOptions,
    );

    commitAddCustomerDefaultOrganizationTag({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to set tag '${organizationTagDetails.name}' as your preferred tag. Error: ${joinErrors(errors)}.`}
              />
            ),
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
          render: (
            <NotificationContent content={`Failed to set tag '${organizationTagDetails.name}' as your preferred tag. Error: ${error.message}.`} />
          ),
        });
      },
    });
  };

  const handleRemoveAsPreferredCustomTagClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const organizationTagDetails = customTags.find((item) => item.id === id);
    if (!organizationTagDetails) {
      return;
    }

    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Removing tag '${organizationTagDetails.name}' as your preferred tag...`} />,
      infoNotificationOptions,
    );

    commitRemoveCustomerDefaultOrganizationTag({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to remove the tag '${organizationTagDetails.name}' as your preferred tag. Error: ${joinErrors(errors)}.`}
              />
            ),
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
          render: (
            <NotificationContent
              content={`Failed to remove the tag '${organizationTagDetails.name}' as your preferred tag. Error: ${error.message}.`}
            />
          ),
        });
      },
    });
  };

  if (!rootData.organization) {
    return <></>;
  }

  if (!rootData.organizationBillingInfo) {
    return <></>;
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
          return <></>;
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
      field: 'moreActions',
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
          return <></>;
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
      field: 'moreActions',
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

  const organization = rootData.organization;
  const organizationBillingInfo = rootData.organizationBillingInfo;
  const billingEmail = organizationBillingInfo.email ? organizationBillingInfo.email : '';
  const billingAddressLine1 = organizationBillingInfo.addressLine1 ? organizationBillingInfo.addressLine1 : '';
  const billingAddressLine2 = organizationBillingInfo.addressLine2 ? organizationBillingInfo.addressLine2 : '';
  const billingSuburb = organizationBillingInfo.suburb ? organizationBillingInfo.suburb : '';
  const billingCity = organizationBillingInfo.city ? organizationBillingInfo.city : '';
  const billingProvince = organizationBillingInfo.province ? organizationBillingInfo.province : '';
  const billingZipcode = organizationBillingInfo.zipcode ? organizationBillingInfo.zipcode : '';
  const billingCountry = organizationBillingInfo.country ? organizationBillingInfo.country : '';
  const paymentMethodExist =
    rootDataOrganizationPaymentMethodsDetails.organizationPaymentMethodsDetails &&
    rootDataOrganizationPaymentMethodsDetails.organizationPaymentMethodsDetails.length > 0;
  const activeOffering = rootData.organization ? rootData.organization.activeOffering : null;
  const availableOfferings = rootData.organization && rootData.organization.availableOfferings ? rootData.organization.availableOfferings : [];

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationAdminLeftSideNavigationMenuContent organizationId={organizationId} hideIcons />
        <Box sx={{ marginLeft: expandedDrawerWidthPx, flexGrow: 1 }}>
          <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Organization Information">
            <Form
              onSubmit={handleOrganizationDetailUpdateClick}
              initialValues={{
                name: organization.name,
                about: organization.about,
                website: organization.website,
                industrySubCategoryIds: organization.industrySubCategories.map(({ id }) => id),
              }}
              validate={validateOrganizationDetails}
              render={({ handleSubmit }) => (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn
                    sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                    ref={(divElement) => {
                      sectionRefs.current['setup'] = divElement;
                    }}
                  >
                    <SectionIconTypography label="Organization Setup" />
                    <BodyIconTypography label="Edit your organization details" />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Name">
                      <TextField name="name" required={requiredOrganizationDetailsFields.name} />
                    </FormFieldLabel>

                    <FormFieldLabel label="About">
                      <TextField name="about" required={requiredOrganizationDetailsFields.about} multiline rows={3} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Website">
                      <TextField name="website" required={requiredOrganizationDetailsFields.about} helperText="https://" />
                    </FormFieldLabel>

                    <FormFieldLabel label="Industry">
                      <OrganizationMultipleChoicesIndustries
                        rootDataRelay={rootData}
                        name="industrySubCategoryIds"
                        required={requiredOrganizationDetailsFields.industrySubCategoryIds}
                      />
                    </FormFieldLabel>
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <StackRow>
                      <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                        Update
                      </Button>
                    </StackRow>
                  </StackColumn>
                </FormStackColumn>
              )}
            />

            <Form
              onSubmit={handleOrganizationBillingDetailUpdateClick}
              initialValues={{
                billingEmail,
                billingAddressLine1,
                billingAddressLine2,
                billingSuburb,
                billingCity,
                billingProvince,
                billingZipcode,
                billingCountry,
              }}
              validate={validateOrganizationBilling}
              render={({ handleSubmit }) => (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn
                    sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                    ref={(divElement) => {
                      sectionRefs.current['billing-payment-setup'] = divElement;
                    }}
                  >
                    <SectionIconTypography label="Billing & Payment Setup" />
                    <BodyIconTypography label="Edit your organization billing and payment details" />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Email">
                      <TextField
                        name="billingEmail"
                        required={requiredOrganizationBillingFields.billingEmail}
                        helperText="Email to send invoice to"
                      />
                    </FormFieldLabel>

                    <FormFieldLabel label="Address line 1">
                      <TextField name="billingAddressLine1" required={requiredOrganizationBillingFields.billingAddressLine1} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Address line 2">
                      <TextField name="billingAddressLine2" required={requiredOrganizationBillingFields.billingAddressLine2} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Suburb">
                      <TextField name="billingSuburb" required={requiredOrganizationBillingFields.billingSuburb} />
                    </FormFieldLabel>

                    <FormFieldLabel label="City">
                      <TextField name="billingCity" required={requiredOrganizationBillingFields.billingCity} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Province">
                      <TextField name="billingProvince" required={requiredOrganizationBillingFields.billingProvince} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Zipcode">
                      <TextField name="billingZipcode" required={requiredOrganizationBillingFields.billingZipcode} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Country">
                      <SingleChoiceCountry name="billingCountry" required={requiredOrganizationBillingFields.billingCountry} />
                    </FormFieldLabel>
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <StackRow>
                      <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                        Update
                      </Button>
                    </StackRow>
                  </StackColumn>
                </FormStackColumn>
              )}
            />

            <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
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
              <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                <StackRow>
                  {rootDataOrganizationPaymentMethodsDetails.organizationPaymentMethodsDetails.map((item) => (
                    <StackColumn key={item.id}>
                      <CreditCard
                        lastFourDigits={item.cardLastFourDigit}
                        expiryDate={`${item.cardExpiryMonth}/${item.cardExpiryYear}`}
                        cardBrand={item.cardBrand}
                      />
                      <Button variant="contained" color="warning" onClick={() => handleRemovePaymentMethodClick(item.id)}>
                        <BodyIconTypography label="Remove Payment Method" invertDefaultColor={paletteMode === 'dark'} startElement={<DeleteIcon />} />
                      </Button>
                    </StackColumn>
                  ))}
                </StackRow>
              </StackColumn>
            )}

            {!paymentMethodExist && (
              <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                <SmallIconTypography label="No payment method setup yet" />
              </StackColumn>
            )}

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['sso-setup'] = divElement;
              }}
            >
              <SectionIconTypography label="SSO Setup" />
              <BodyIconTypography label="Edit your organization SSO settings" />
              <Divider />
            </StackColumn>

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
                  <AddOrganizationZoneButton organizationId={organizationId} connectionIds={zonesConnectionIds} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <PushToRight />
              <Search size="small" placeholder="Search for zones" defaultValue={zoneNameSearchText} onChange={handleZonesSearchTextChange} />
            </GridContainer>

            {seledctedZones.length > 0 && (
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
                    <SmallIconTypography label={`${seledctedZones.length} records selected`} />
                    <PushToRight />
                    <Button
                      size="medium"
                      variant="contained"
                      color="warning"
                      startIcon={<DeleteIcon />}
                      onClick={handleRemoveZonesClick}
                      sx={{ textTransform: 'none' }}
                    >
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
                  <AddOrganizationCustomTagButton organizationId={organizationId} connectionIds={customTagsConnectionIds} />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>

            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <PushToRight />
              <Search size="small" placeholder="Search for tags" defaultValue={customTagNameSearchText} onChange={handleCustomTagsSearchTextChange} />
            </GridContainer>

            {seledctedCustomTags.length > 0 && (
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
                    <SmallIconTypography label={`${seledctedCustomTags.length} records selected`} />
                    <PushToRight />
                    <Button
                      size="medium"
                      variant="contained"
                      color="warning"
                      startIcon={<DeleteIcon />}
                      onClick={handleRemoveCustomTagsClick}
                      sx={{ textTransform: 'none' }}
                    >
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

            <GridContainer spacing={1} sx={{ padding: defaultPadding, justifyContent: 'space-between', alignItems: 'stretch' }}>
              {activeOffering && (
                <Grid>
                  <Card sx={{ width: { xs: '100%', sm: 300 }, height: '100%' }}>
                    <CardHeader
                      title={
                        <>
                          <BodyIconTypography label={activeOffering.name} invertDefaultColor />
                          <BodyIconTypography label={`Unit Price: $${(activeOffering.unitPrice / 100).toFixed(2)}`} invertDefaultColor />
                        </>
                      }
                      sx={{
                        background: `linear-gradient(${activeOffering.colorTiltingAngle}, ${activeOffering.startColor}, ${activeOffering.endColor})`,
                      }}
                    />

                    <CardContent sx={{ marginLeft: 1 }}>
                      <List sx={{ padding: 0 }}>
                        <BodyIconTypography label="Feature:" />
                        {activeOffering.featureSet.map(({ name, description }, index) => (
                          <ListItem key={index} alignItems="flex-start" sx={{ padding: 0 }}>
                            <ListItemIcon sx={{ minWidth: 'auto', marginRight: 1 }}>
                              <TickIcon fontSize="small" />
                            </ListItemIcon>
                            <ListItemText>
                              <SmallIconTypography label={`${name}: ${description}`} />
                            </ListItemText>
                          </ListItem>
                        ))}
                      </List>

                      {!activeOffering.free && (
                        <CardActions sx={{ justifyContent: 'flex-end' }}>
                          <Button color="secondary" variant="contained" onClick={handleCancelActiveOfferingClick}>
                            Cancel
                          </Button>
                        </CardActions>
                      )}
                    </CardContent>
                  </Card>
                </Grid>
              )}

              {availableOfferings.map((availableOffering) => (
                <Grid key={availableOffering.code}>
                  <Card sx={{ width: { xs: '100%', sm: 300 }, height: '100%' }}>
                    <CardHeader
                      title={
                        <>
                          <BodyIconTypography label={availableOffering.name} invertDefaultColor />
                          {availableOffering.unitPrice > 0 && (
                            <BodyIconTypography label={`Unit Price: $${(availableOffering.unitPrice / 100).toFixed(2)}`} invertDefaultColor />
                          )}
                          {availableOffering.unitPrice < 0 && <BodyIconTypography label={`Unit Price: Contact Sales`} invertDefaultColor />}
                        </>
                      }
                      sx={{
                        background: `linear-gradient(${availableOffering.colorTiltingAngle}, ${availableOffering.startColor}, ${availableOffering.endColor})`,
                      }}
                    />

                    <CardContent sx={{ marginLeft: 1 }}>
                      <List sx={{ padding: 0 }}>
                        <BodyIconTypography label="Feature:" />
                        {availableOffering.featureSet.map(({ name, description }, index) => (
                          <ListItem key={index} alignItems="flex-start" sx={{ padding: 0 }}>
                            <ListItemIcon sx={{ minWidth: 'auto', marginRight: 1 }}>
                              <TickIcon fontSize="small" />
                            </ListItemIcon>
                            <ListItemText>
                              <SmallIconTypography label={`${name}: ${description}`} />
                            </ListItemText>
                          </ListItem>
                        ))}

                        {!rootData.organization?.hasAttachedPaymentMethod && (
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

                    {!rootData.organization?.hasAttachedPaymentMethod && (
                      <CardActions sx={{ justifyContent: 'flex-end' }}>
                        <Button variant="contained" onClick={handleAddPaymentMethodClicked} sx={{ textTransform: 'none' }}>
                          <SmallIconTypography label="Add Payment Method" />
                        </Button>
                      </CardActions>
                    )}

                    {rootData.organization?.hasAttachedPaymentMethod && availableOffering.unitPrice > 0 && (
                      <CardActions sx={{ justifyContent: 'flex-end' }}>
                        <Button
                          color="primary"
                          variant="contained"
                          onClick={() => handleUpgradeOfferingClick(availableOffering.code)}
                          sx={{ textTransform: 'none' }}
                        >
                          Upgrade
                        </Button>
                      </CardActions>
                    )}
                  </Card>
                </Grid>
              ))}
            </GridContainer>
          </AppBarWithStackColumn>
        </Box>
      </Box>

      <MoreActionsMenu
        anchorEl={zoneMoreActionsAnchorEl}
        open={zoneMoreActionsMenuOpen}
        onMenuItemClick={handleZoneMoreActionsMenuItemClick}
        options={zoneMoreActionsOption}
      />

      <MoreActionsMenu
        anchorEl={customTagMoreActionsAnchorEl}
        open={customTagMoreActionsMenuOpen}
        onMenuItemClick={handleCustomTagMoreActionsMenuItemClick}
        options={customTagMoreActionsOption}
      />

      {selectedZoneId && (
        <EditOrganizationZoneDialog
          onReloadRequired={onReloadRequired}
          zoneId={selectedZoneId}
          isDialogOpen={isEditZoneDialogOpen}
          onAddClicked={handleEditZoneClick}
          onCancel={onEditZoneCancel}
        />
      )}

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
          organizationId={organizationId}
          isDialogOpen={isAddPaymentMethodDialogOpen}
          onCancel={handleAddPaymentMethodCancel}
        />
      )}
    </>
  );
};

export default memo(OrganizationAdmin);

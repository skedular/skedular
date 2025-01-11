import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import {
  BodyIconTypography,
  CreditCard,
  FormFieldLabel,
  LeadIconTypography,
  PushToRight,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  StackColumnWithSaveExitCancelAppBar,
  StackRow,
} from '@repo/shared/components/commons';
import { DeskType } from '@repo/shared/components/deskType';
import { SingleChoiceCountry } from '@repo/shared/components/forms';
import { DeleteIcon, EllipseMenuIcon, NewIcon } from '@repo/shared/components/icons';
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
import { defaultGridActionPadding, defaultGridStyle, defaultPadding } from '@repo/shared/libs/theme';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { getOrganizationBaseLink, OrganizationMultipleChoicesIndustries } from 'components/organization';
import { AddOrganizationDeskTypeButton } from 'components/organization/addOrganizationDeskType';
import { AddOrganizationPaymentMethodDialog } from 'components/organization/addOrganizationPaymentMethod';
import { AddOrganizationZoneButton } from 'components/organization/addOrganizationZone';
import { EditOrganizationZoneDialog } from 'components/organization/editOrganizationZone/';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';
import EditOrganizationDeskTypeDialog from '../editOrganizationDeskType/edit-organization-desk-type-dialog';
import type { organizationAdmin_deleteDeskTypesMutation } from './__generated__/organizationAdmin_deleteDeskTypesMutation.graphql';
import type { organizationAdmin_deleteZonesMutation } from './__generated__/organizationAdmin_deleteZonesMutation.graphql';
import type { organizationAdmin_deskTypes_query$key } from './__generated__/organizationAdmin_deskTypes_query.graphql';
import type { organizationAdmin_deskTypes_refetchableFragment } from './__generated__/organizationAdmin_deskTypes_refetchableFragment.graphql';
import type { organizationAdmin_organizationPaymentMethodsDetails_query$key } from './__generated__/organizationAdmin_organizationPaymentMethodsDetails_query.graphql';
import type { organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment } from './__generated__/organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment.graphql';
import type { organizationAdmin_query$key } from './__generated__/organizationAdmin_query.graphql';
import type { organizationAdmin_removeOrganizationPaymentMethodMutation } from './__generated__/organizationAdmin_removeOrganizationPaymentMethodMutation.graphql';
import type { organizationAdmin_setOrganizationBillingInfoMutation } from './__generated__/organizationAdmin_setOrganizationBillingInfoMutation.graphql';
import type { organizationAdmin_updateOrganizationMutation } from './__generated__/organizationAdmin_updateOrganizationMutation.graphql';
import type { organizationAdmin_zones_query$key } from './__generated__/organizationAdmin_zones_query.graphql';
import type { organizationAdmin_zones_refetchableFragment } from './__generated__/organizationAdmin_zones_refetchableFragment.graphql';
import { expandedDrawerWidthPx } from './commons';
import OrganizationAdminLeftSideNavigationMenuContent from './organization-admin-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationAdmin_query$key;
  rootDataOrganizationPaymentMethodsDetailsRelay: organizationAdmin_organizationPaymentMethodsDetails_query$key;
  rootDataZonesRelay: organizationAdmin_zones_query$key;
  rootDataDeskTypesRelay: organizationAdmin_deskTypes_query$key;
  onReloadRequired: () => void;
  organizationId: string;
};

type OrganizationDetails = {
  name: string;
  about: string | null;
  website: string | null;
  industrySubCategoryIds: string[];
  billingEmail: string;
  billingAddressLine1: string | null;
  billingAddressLine2: string | null;
  billingSuburb: string | null;
  billingCity: string | null;
  billingProvince: string | null;
  billingZipcode: string | null;
  billingCountry: string | null;
};

const organizationSchema = object({
  name: string().min(3, 'Organization name must be at least three characters long.').required('Organization name is required'),
  about: string().nullable(),
  website: string().nullable(),
  industrySubCategoryIds: array().nullable(),
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
};

type DeskTypeRowType = {
  id: string;
  name: string;
  description: string | null | undefined;
};

const OrganizationAdmin = ({
  rootDataRelay,
  rootDataOrganizationPaymentMethodsDetailsRelay,
  rootDataZonesRelay,
  rootDataDeskTypesRelay,
  onReloadRequired,
  organizationId,
}: Props) => {
  const rootData = useFragment<organizationAdmin_query$key>(
    graphql`
      fragment organizationAdmin_query on Query {
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
            }
          }
        }
      }
    `,
    rootDataZonesRelay,
  );

  const [rootDataDeskTypes, refetchDeskTypes] = useRefetchableFragment<
    organizationAdmin_deskTypes_refetchableFragment,
    organizationAdmin_deskTypes_query$key
  >(
    graphql`
      fragment organizationAdmin_deskTypes_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationAdmin_deskTypes_refetchableFragment") {
        deskTypes(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $deskTypeNameSearchText }
          orderBy: [{ direction: Ascending, field: Name }]
        ) @connection(key: "organizationAdmin_deskTypes") {
          __id
          totalCount
          edges {
            node {
              id
              name
              description
            }
          }
        }
      }
    `,
    rootDataDeskTypesRelay,
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

  const [commitDeleteDeskTypes] = useMutation<organizationAdmin_deleteDeskTypesMutation>(graphql`
    mutation organizationAdmin_deleteDeskTypesMutation($connectionIds: [ID!]!, $input: DeleteDeskTypesInput!) {
      deleteDeskTypes(input: $input) {
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

  const [, startTransition] = useTransition();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const validate = makeValidate(organizationSchema);
  const requiredFields = makeRequired(organizationSchema);
  const [zoneNameSearchText, setZoneNameSearchText] = useState<string>('');
  const [deskTypeNameSearchText, setDeskTypeNameSearchText] = useState<string>('');
  const [seledctedZones, setSeledctedZones] = useState<GridRowSelectionModel>([]);
  const [seledctedDeskTypes, setSeledctedDeskTypes] = useState<GridRowSelectionModel>([]);
  const [selectedZoneId, setSelectedZoneId] = useState<null | string>(null);
  const [selectedDeskTypeId, setSelectedDeskTypeId] = useState<null | string>(null);
  const [zoneMoreActionsAnchorEl, setZoneMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const zoneMoreActionsMenuOpen = Boolean(zoneMoreActionsAnchorEl);
  const [deskTypeMoreActionsAnchorEl, setDeskTypeMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const deskTypeMoreActionsMenuOpen = Boolean(deskTypeMoreActionsAnchorEl);
  const [isEditZoneDialogOpen, setIsEditZoneDialogOpen] = useState(false);
  const [isEditDeskTypeDialogOpen, setIsEditDeskTypeDialogOpen] = useState(false);
  const [isAddPaymentMethodDialogOpen, setIsAddPaymentMethodDialogOpen] = useState(false);

  const zoneMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditZone],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteZone],
  ];

  const deskTypeMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditDeskType],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteDeskType],
  ];

  const zonesConnectionIds = useMemo(() => (rootDataZones.zones ? [rootDataZones.zones.__id] : []), [rootDataZones.zones]);
  const zones = useMemo(() => {
    if (!rootDataZones.zones) {
      return [];
    }

    return rootDataZones.zones.edges.map(({ node }) => node);
  }, [rootDataZones.zones]);

  const deskTypesConnectionIds = useMemo(
    () => (rootDataDeskTypes.deskTypes ? [rootDataDeskTypes.deskTypes.__id] : []),
    [rootDataDeskTypes.deskTypes],
  );
  const deskTypes = useMemo(() => {
    if (!rootDataDeskTypes.deskTypes) {
      return [];
    }

    return rootDataDeskTypes.deskTypes.edges.map(({ node }) => node);
  }, [rootDataDeskTypes.deskTypes]);

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

  const handleRefetchDeskTypes = useCallback(
    (deskTypeNameSearchText: string) => {
      startTransition(() => {
        refetchDeskTypes(
          {
            deskTypeNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchDeskTypes],
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

  const handleDetailUpdateClick = ({
    name,
    about,
    website,
    industrySubCategoryIds,
    billingEmail,
    billingAddressLine1,
    billingAddressLine2,
    billingSuburb,
    billingCity,
    billingProvince,
    billingZipcode,
    billingCountry,
  }: OrganizationDetails) => {
    if (!rootData.organization) {
      return;
    }

    if (!rootData.organizationBillingInfo) {
      return;
    }

    const organization = rootData.organization;
    const organizationBillingInfo = rootData.organizationBillingInfo;

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
                render: <NotificationContent content={`Failed to update organization '${organization?.name}'. Error: ${joinErrors(errors)}.`} />,
              });

              return;
            }

            toast.update(toastId, {
              ...successNotificationOptions,
              render: <NotificationContent content={`Organization ${name} details updated.`} />,
            });

            navigate(getOrganizationBaseLink(organizationId));
          },
          onError: (error) => {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to update organization '${organization?.name}'. Error: ${error.message}.`} />,
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

  const handleZonesSearchTextChange = (str: string) => {
    setZoneNameSearchText(str);

    handleRefetchZones(str);
  };

  const handleDeskTypesSearchTextChange = (str: string) => {
    setDeskTypeNameSearchText(str);

    handleRefetchDeskTypes(str);
  };

  const handleSelectedZonesChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedZones(newRowSelectionModel);
  };

  const handleSelectedDeskTypesChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedDeskTypes(newRowSelectionModel);
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

  const handleDeskTypeMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setDeskTypeMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditDeskType:
        setIsEditDeskTypeDialogOpen(true);
        break;

      case MoreActionsMenuOptionType.DeleteDeskType:
        handleRemoveDeskTypeClick();
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

  const handleRemoveDeskTypesClick = () => {
    const toastId = themedToast(<NotificationContent content="Removing desk types ..." />, infoNotificationOptions);

    commitDeleteDeskTypes({
      variables: {
        connectionIds: deskTypesConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: seledctedDeskTypes.map((id) => id as string),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove desk types. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk types removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove desk types. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveDeskTypeClick = () => {
    if (!selectedDeskTypeId) {
      return;
    }

    const toastId = themedToast(<NotificationContent content="Removing desk type ..." />, infoNotificationOptions);

    commitDeleteZones({
      variables: {
        connectionIds: deskTypesConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: [selectedDeskTypeId],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove desk type. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk type removed.`} />,
        });

        setSelectedDeskTypeId(null);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove desk type. Error: ${error.message}.`} />,
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

  const handleEditDeskTypeClick = () => {
    setIsEditDeskTypeDialogOpen(false);
  };

  const onEditDeskTypeCancel = () => {
    setIsEditDeskTypeDialogOpen(false);
  };

  const onAddPaymentMethodClicked = () => {
    setIsAddPaymentMethodDialogOpen(true);
  };

  const onAddPaymentMethodCancel = () => {
    setIsAddPaymentMethodDialogOpen(false);
  };

  const handleCancelClick = () => {
    navigate(getOrganizationBaseLink(organizationId));
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
  }));

  const zoneColumns: GridColDef<(typeof zoneRows)[number]>[] = [
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => <Zone zone={{ id: params.id as string, name: params.value }} showFullName />,
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
      field: 'moreActions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <IconButton
          onClick={(event: React.MouseEvent<HTMLElement>) => {
            setSelectedZoneId(params.id as string);
            setZoneMoreActionsAnchorEl(event.currentTarget);
          }}
        >
          <EllipseMenuIcon />
        </IconButton>
      ),
    },
  ];

  const deskTypeRows: DeskTypeRowType[] = deskTypes.map((deskType) => ({
    id: deskType.id,
    name: deskType.name,
    description: deskType.description,
  }));

  const deskTypeColumns: GridColDef<(typeof deskTypeRows)[number]>[] = [
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => <DeskType deskType={{ id: params.id as string, name: params.value }} showFullName />,
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
      field: 'moreActions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <IconButton
          onClick={(event: React.MouseEvent<HTMLElement>) => {
            setSelectedDeskTypeId(params.id as string);
            setDeskTypeMoreActionsAnchorEl(event.currentTarget);
          }}
        >
          <EllipseMenuIcon />
        </IconButton>
      ),
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

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationAdminLeftSideNavigationMenuContent organizationId={organizationId} hideIcons />
        <Box sx={{ marginLeft: expandedDrawerWidthPx, flexGrow: 1 }}>
          <Form
            onSubmit={handleDetailUpdateClick}
            initialValues={{
              name: organization.name,
              about: organization.about,
              website: organization.website,
              industrySubCategoryIds: organization.industrySubCategories.map(({ id }) => id),
              billingEmail,
              billingAddressLine1,
              billingAddressLine2,
              billingSuburb,
              billingCity,
              billingProvince,
              billingZipcode,
              billingCountry,
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <StackColumnWithSaveExitCancelAppBar onSubmit={handleSubmit} onCancel={handleCancelClick} label="Edit Organization Information">
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
                    <TextField name="name" required={requiredFields.name} />
                  </FormFieldLabel>

                  <FormFieldLabel label="About">
                    <TextField name="about" required={requiredFields.about} multiline rows={3} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Industry">
                    <TextField name="website" required={requiredFields.about} helperText="https://" />
                  </FormFieldLabel>

                  <FormFieldLabel label="Industry">
                    <OrganizationMultipleChoicesIndustries
                      rootDataRelay={rootData}
                      name="industrySubCategoryIds"
                      required={requiredFields.industrySubCategoryIds}
                    />
                  </FormFieldLabel>
                </StackColumn>

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
                    <TextField name="billingEmail" required={requiredFields.billingEmail} helperText="Email to send invoice to" />
                  </FormFieldLabel>

                  <FormFieldLabel label="Address line 1">
                    <TextField name="billingAddressLine1" required={requiredFields.billingAddressLine1} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Address line 2">
                    <TextField name="billingAddressLine2" required={requiredFields.billingAddressLine2} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Suburb">
                    <TextField name="billingSuburb" required={requiredFields.billingSuburb} />
                  </FormFieldLabel>

                  <FormFieldLabel label="City">
                    <TextField name="billingCity" required={requiredFields.billingCity} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Province">
                    <TextField name="billingProvince" required={requiredFields.billingProvince} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Zipcode">
                    <TextField name="billingZipcode" required={requiredFields.billingZipcode} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Country">
                    <SingleChoiceCountry name="billingCountry" required={requiredFields.billingCountry} />
                  </FormFieldLabel>
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <BodyIconTypography label="Edit your payment method" />
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
                            <BodyIconTypography
                              label="Remove Payment Method"
                              invertDefaultColor={paletteMode === 'dark'}
                              startElement={<DeleteIcon />}
                            />
                          </Button>
                        </StackColumn>
                      ))}
                    </StackRow>
                  </StackColumn>
                )}

                {!paymentMethodExist && (
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <StackRow>
                      <SmallIconTypography label="No payment method setup yet" />
                      <PushToRight />
                      <Button variant="text" onClick={onAddPaymentMethodClicked} sx={{ textTransform: 'none' }}>
                        <LeadIconTypography label={'Add Payment Method'} endElement={<NewIcon fontSize="large" />} />
                      </Button>
                    </StackRow>
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
                  <SectionIconTypography label="Zones Setup" />
                  <BodyIconTypography label="Edit your organization zones details" />
                  <Divider />
                </StackColumn>

                <StackRow sx={{ padding: defaultPadding }}>
                  <PushToRight />
                  <Search size="small" placeholder="Search for zones" defaultValue={zoneNameSearchText} onChange={handleZonesSearchTextChange} />
                </StackRow>

                <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
                  <Box
                    sx={{
                      backgroundColor: (theme) => theme.palette.background.paper,
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
                        disabled={seledctedZones.length === 0}
                        onClick={handleRemoveZonesClick}
                      >
                        Remove Zone
                      </Button>
                    </StackRow>
                  </Box>
                </StackRow>

                <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
                  <PushToRight />
                  <AddOrganizationZoneButton organizationId={organizationId} connectionIds={zonesConnectionIds} />
                </StackRow>

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
                  />
                </StackRow>

                <StackColumn
                  sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                  ref={(divElement) => {
                    sectionRefs.current['desk-types-setup'] = divElement;
                  }}
                >
                  <SectionIconTypography label="Desk Types Setup" />
                  <BodyIconTypography label="Edit your organization desk types details" />
                  <Divider />
                </StackColumn>

                <StackRow sx={{ padding: defaultPadding }}>
                  <PushToRight />
                  <Search
                    size="small"
                    placeholder="Search for desk types"
                    defaultValue={deskTypeNameSearchText}
                    onChange={handleDeskTypesSearchTextChange}
                  />
                </StackRow>

                <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
                  <Box
                    sx={{
                      backgroundColor: (theme) => theme.palette.background.paper,
                      padding: defaultGridActionPadding,
                      border: 1,
                      borderColor: (theme) => theme.palette.divider,
                      borderRadius: 2,
                      flexGrow: 1,
                    }}
                  >
                    <StackRow sx={{ alignItems: 'center' }}>
                      <SmallIconTypography label={`${seledctedDeskTypes.length} records selected`} />
                      <PushToRight />
                      <Button
                        size="medium"
                        variant="contained"
                        color="warning"
                        startIcon={<DeleteIcon />}
                        disabled={seledctedDeskTypes.length === 0}
                        onClick={handleRemoveDeskTypesClick}
                      >
                        Remove Desk Type
                      </Button>
                    </StackRow>
                  </Box>
                </StackRow>

                <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
                  <PushToRight />
                  <AddOrganizationDeskTypeButton organizationId={organizationId} connectionIds={deskTypesConnectionIds} />
                </StackRow>

                <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
                  <DataGrid
                    checkboxSelection
                    rowSelectionModel={seledctedDeskTypes}
                    onRowSelectionModelChange={handleSelectedDeskTypesChanged}
                    rows={deskTypeRows}
                    columns={deskTypeColumns}
                    hideFooterPagination={deskTypeRows.length <= 10}
                    initialState={{
                      pagination: {
                        rowCount: deskTypeRows.length,
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
                  />
                </StackRow>
              </StackColumnWithSaveExitCancelAppBar>
            )}
          />
        </Box>
      </Box>

      <MoreActionsMenu
        anchorEl={zoneMoreActionsAnchorEl}
        open={zoneMoreActionsMenuOpen}
        onMenuItemClick={handleZoneMoreActionsMenuItemClick}
        options={zoneMoreActionsOption}
      />

      <MoreActionsMenu
        anchorEl={deskTypeMoreActionsAnchorEl}
        open={deskTypeMoreActionsMenuOpen}
        onMenuItemClick={handleDeskTypeMoreActionsMenuItemClick}
        options={deskTypeMoreActionsOption}
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

      {selectedDeskTypeId && (
        <EditOrganizationDeskTypeDialog
          onReloadRequired={onReloadRequired}
          deskTypeId={selectedDeskTypeId}
          isDialogOpen={isEditDeskTypeDialogOpen}
          onAddClicked={handleEditDeskTypeClick}
          onCancel={onEditDeskTypeCancel}
        />
      )}

      {!paymentMethodExist && isAddPaymentMethodDialogOpen && (
        <AddOrganizationPaymentMethodDialog
          organizationId={organizationId}
          isDialogOpen={isAddPaymentMethodDialogOpen}
          onCancel={onAddPaymentMethodCancel}
        />
      )}
    </>
  );
};

export default memo(OrganizationAdmin);

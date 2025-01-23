import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import IconButton from '@mui/material/IconButton';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
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
} from '@repo/shared/components/commons';
import { CustomTags } from '@repo/shared/components/customTag';
import { SingleChoinceTimezone } from '@repo/shared/components/forms';
import { DeleteIcon, EllipseMenuIcon, NotPreferredIcon, PreferredIcon } from '@repo/shared/components/icons';
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
import { Zones } from '@repo/shared/components/zone';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { defaultButtonStyle, defaultGridActionPadding, defaultGridStyle, defaultPadding, emerald, flame } from '@repo/shared/libs/theme';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { AddDeskButton } from 'components/desk/addDesk';
import { BulkAddDeskButton } from 'components/desk/bulkAddDesk';
import { getOrganizationLocationDeskBaseLink, getOrganizationLocationsBaseLink } from 'components/links';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
import CustomTagSelector from '../customTagSelector/custom-tag-selector';
import ZoneSelector from '../zoneSelector/zone-selector';
import type { organizationLocation_activateDesksMutation } from './__generated__/organizationLocation_activateDesksMutation.graphql';
import type { organizationLocation_addCustomerDefaultDeskMutation } from './__generated__/organizationLocation_addCustomerDefaultDeskMutation.graphql';
import type { organizationLocation_deactivateDesksMutation } from './__generated__/organizationLocation_deactivateDesksMutation.graphql';
import type { organizationLocation_deleteDesksMutation } from './__generated__/organizationLocation_deleteDesksMutation.graphql';
import type { organizationLocation_desks_query$key } from './__generated__/organizationLocation_desks_query.graphql';
import type { organizationLocation_desks_refetchableFragment } from './__generated__/organizationLocation_desks_refetchableFragment.graphql';
import type { organizationLocation_query$key } from './__generated__/organizationLocation_query.graphql';
import type { organizationLocation_removeCustomerDefaultDeskMutation } from './__generated__/organizationLocation_removeCustomerDefaultDeskMutation.graphql';
import type { organizationLocation_updateLocationMutation } from './__generated__/organizationLocation_updateLocationMutation.graphql';
import { expandedDrawerWidthPx } from './commons';
import OrganizationLocationLeftSideNavigationMenuContent from './organization-location-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationLocation_query$key;
  rootDataDesksRelay: organizationLocation_desks_query$key;
  onReloadRequired: () => void;
  organizationId: string;
  locationId: string;
};

type LocationDetails = {
  name: string;
  about: string | null;
  timezone: string;
  physicalAddress: string;
};

type CustomTagDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type ZoneDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type DeskRowType = {
  id: string;
  name: string;
  customTags: CustomTagDetails[];
  zones: ZoneDetails[];
  status: boolean;
  preferred: boolean;
};

const locationSchema = object({
  name: string().min(3, 'Location name must be at least three characters long.').required('Location name is required'),
  about: string().nullable(),
  timezone: string().required('Timezone is required'),
  physicalAddress: string().nullable(),
});

const OrganizationLocation = ({ rootDataRelay, rootDataDesksRelay, onReloadRequired, organizationId, locationId }: Props) => {
  const rootData = useFragment<organizationLocation_query$key>(
    graphql`
      fragment organizationLocation_query on Query {
        me {
          id
          preferredDesks {
            uniqueId
          }
        }
        location(id: $locationId) {
          id
          name
          about
          timezone
          physicalAddress {
            formattedAddress
          }
        }
        ...customTagSelector_allCustomTags_query
        ...zoneSelector_allZones_query
      }
    `,
    rootDataRelay,
  );

  const [rootDataDesks, refetchDesks] = useRefetchableFragment<organizationLocation_desks_refetchableFragment, organizationLocation_desks_query$key>(
    graphql`
      fragment organizationLocation_desks_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationLocation_desks_refetchableFragment") {
        desks(
          first: $count
          after: $cursor
          where: { locationId: $locationId, nameContains: $deskNameSearchText, customTagIds: $deskCustomTagIds, zoneIds: $deskZoneIds }
        ) @connection(key: "organizationLocation_desks") {
          __id
          totalCount
          edges {
            node {
              id
              name
              deactivated
              requireBookingApproval
              customTags {
                uniqueId
                name
                color
              }
              zones {
                uniqueId
                name
                color
              }
            }
          }
        }
      }
    `,
    rootDataDesksRelay,
  );

  const [commitUpdateLocation] = useMutation<organizationLocation_updateLocationMutation>(graphql`
    mutation organizationLocation_updateLocationMutation($input: UpdateLocationInput!) @raw_response_type {
      updateLocation(input: $input) {
        location {
          id
          name
          about
          timezone
          physicalAddress {
            formattedAddress
          }
        }
      }
    }
  `);

  const [commitDeleteDesks] = useMutation<organizationLocation_deleteDesksMutation>(graphql`
    mutation organizationLocation_deleteDesksMutation($connectionIds: [ID!]!, $input: DeleteDesksInput!) {
      deleteDesks(input: $input) {
        desks {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitActivateDesks] = useMutation<organizationLocation_activateDesksMutation>(graphql`
    mutation organizationLocation_activateDesksMutation($input: ActivateDesksInput!) {
      activateDesks(input: $input) {
        desks {
          id
          name
          deactivated
          requireBookingApproval
          customTags {
            uniqueId
            name
            color
          }
          zones {
            uniqueId
            name
            color
          }
        }
      }
    }
  `);

  const [commitDeactivateDesks] = useMutation<organizationLocation_deactivateDesksMutation>(graphql`
    mutation organizationLocation_deactivateDesksMutation($input: DeactivateDesksInput!) {
      deactivateDesks(input: $input) {
        desks {
          id
          name
          deactivated
          requireBookingApproval
          customTags {
            uniqueId
            name
            color
          }
          zones {
            uniqueId
            name
            color
          }
        }
      }
    }
  `);

  const [commitAddCustomerDefaultDesk] = useMutation<organizationLocation_addCustomerDefaultDeskMutation>(graphql`
    mutation organizationLocation_addCustomerDefaultDeskMutation($input: AddCustomerDefaultDeskInput!) {
      addCustomerDefaultDesk(input: $input) {
        customer {
          id
          preferredDesks {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerDefaultDesk] = useMutation<organizationLocation_removeCustomerDefaultDeskMutation>(graphql`
    mutation organizationLocation_removeCustomerDefaultDeskMutation($input: RemoveCustomerDefaultDeskInput!) {
      removeCustomerDefaultDesk(input: $input) {
        customer {
          id
          preferredDesks {
            uniqueId
          }
        }
      }
    }
  `);

  const [, startTransition] = useTransition();
  const navigate = useNavigate();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [searchParams] = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const validateLocationDetails = makeValidate(locationSchema);
  const requiredLocationDetailsFields = makeRequired(locationSchema);
  const [deskNameSearchText, setDeskNameSearchText] = useState<string>('');
  const [deskCustomTagIds, setDeskCustomTagIds] = useState<string[]>([]);
  const [deskZoneIds, setDeskZoneIds] = useState<string[]>([]);
  const [selectedDeskId, setSelectedDeskId] = useState<null | string>(null);
  const [seledctedDesks, setSeledctedDesks] = useState<GridRowSelectionModel>([]);
  const [deskMoreActionsAnchorEl, setDeskMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const deskMoreActionsMenuOpen = Boolean(deskMoreActionsAnchorEl);
  const [preferredDesks, setPreferredDesks] = useState(rootData.me?.preferredDesks.map(({ uniqueId }) => uniqueId) ?? []);

  const deskMoreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditDesk],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeactivateDesk],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.ActivateDesk],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteDesk],
  ];

  const desksConnectionIds = useMemo(() => (rootDataDesks.desks ? [rootDataDesks.desks.__id] : []), [rootDataDesks.desks]);
  const desks = useMemo(() => {
    if (!rootDataDesks.desks) {
      return [];
    }

    return rootDataDesks.desks.edges.map(({ node }) => node);
  }, [rootDataDesks.desks]);
  const deskDetails = useMemo(() => desks.find((item) => item.id === selectedDeskId), [selectedDeskId, desks]);

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

  const handleRefetchDesks = useCallback(
    (deskNameSearchText: string, deskCustomTagIds: string[], deskZoneIds: string[]) => {
      startTransition(() => {
        refetchDesks(
          {
            deskNameSearchText,
            deskCustomTagIds,
            deskZoneIds,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchDesks],
  );

  const handleLocationDetailUpdateClick = ({ name, about, timezone, physicalAddress }: LocationDetails) => {
    if (!rootData.location) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating location '${rootData.location.name}'...`} />, infoNotificationOptions);

    commitUpdateLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: rootData.location.id,
          name,
          about,
          timezone,
          organizationId,
          physicalAddress: {
            formattedAddress: physicalAddress,
          },
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update location '${rootData.location?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location ${name} details updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update location '${rootData.location?.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateLocation: {
          location: {
            id: rootData.location.id,
            name,
            about,
            timezone,
            physicalAddress: {
              formattedAddress: physicalAddress,
            },
          },
        },
      },
    });
  };

  const handleCloseClick = () => {
    navigate(getOrganizationLocationsBaseLink(organizationId));
  };

  const handleDeskNameSearchTextChange = (str: string) => {
    setDeskNameSearchText(str);

    handleRefetchDesks(str, deskZoneIds, deskCustomTagIds);
  };

  const handleCustomTagChanged = (id?: string) => {
    const newIds = id ? [id] : [];
    setDeskCustomTagIds(newIds);

    handleRefetchDesks(deskNameSearchText, newIds, deskZoneIds);
  };

  const handleZoneTypeChanged = (id?: string) => {
    const newIds = id ? [id] : [];
    setDeskZoneIds(newIds);

    handleRefetchDesks(deskNameSearchText, deskCustomTagIds, newIds);
  };

  const handleSelectedDesksChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedDesks(newRowSelectionModel);
  };

  const handleDeskMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setDeskMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditDesk:
        if (deskDetails) {
          navigate(getOrganizationLocationDeskBaseLink(organizationId, locationId, deskDetails.id));
          return;
        }

        break;

      case MoreActionsMenuOptionType.DeactivateDesk:
        handleDeactivateDeskClick();
        break;

      case MoreActionsMenuOptionType.ActivateDesk:
        handleActivateDeskClick();
        break;

      case MoreActionsMenuOptionType.DeleteDesk:
        handleRemoveDeskClick();
        break;
    }
  };

  const handleDeactivateDesksClick = () => {
    const toastId = themedToast(<NotificationContent content={'Deactivating desks...'} />, infoNotificationOptions);

    commitDeactivateDesks({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: seledctedDesks.map((id) => id as string),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate desks. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Desks deactivated.'} />,
        });
        setSeledctedDesks([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate desks. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleActivateDesksClick = () => {
    const toastId = themedToast(<NotificationContent content={'Activating desks...'} />, infoNotificationOptions);

    commitActivateDesks({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: seledctedDesks.map((id) => id as string),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate desks. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Desks activated.'} />,
        });
        setSeledctedDesks([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate desks. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveDesksClick = () => {
    const toastId = themedToast(<NotificationContent content={'Removing desks...'} />, infoNotificationOptions);

    commitDeleteDesks({
      variables: {
        connectionIds: desksConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: seledctedDesks.map((id) => id as string),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove desks. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Desks removed.'} />,
        });
        setSeledctedDesks([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove desks. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleDeactivateDeskClick = () => {
    if (!deskDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Deactivating desk...'} />, infoNotificationOptions);

    commitDeactivateDesks({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: [deskDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate desk. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Desk deactivated.'} />,
        });
        setSeledctedDesks([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate desk. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleActivateDeskClick = () => {
    if (!deskDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Activating desk...'} />, infoNotificationOptions);

    commitActivateDesks({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: [deskDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate desk. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Desk activated.'} />,
        });
        setSeledctedDesks([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate desk. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveDeskClick = () => {
    if (!deskDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={'Removing desk...'} />, infoNotificationOptions);

    commitDeleteDesks({
      variables: {
        connectionIds: desksConnectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: [deskDetails.id],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove desk. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Desk removed.'} />,
        });
        setSeledctedDesks([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove desk. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleSetAsPreferredDeskClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const deskDetails = desks.find((item) => item.id === id);
    if (!deskDetails) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Setting desk '${deskDetails.name}' as your preferred desk...`} />,
      infoNotificationOptions,
    );

    commitAddCustomerDefaultDesk({
      variables: {
        input: {
          clientMutationId: nanoid(),
          deskId: deskDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent content={`Failed to set desk '${deskDetails.name}' as your preferred desk. Error: ${joinErrors(errors)}.`} />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk '${deskDetails.name}' has been set as the preferred desk.`} />,
        });

        setPreferredDesks(preferredDesks.concat([deskDetails.id]));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set desk '${deskDetails.name}' as your preferred desk. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveAsPreferredDeskClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const deskDetails = desks.find((item) => item.id === id);
    if (!deskDetails) {
      return;
    }
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Removing desk '${deskDetails.name}' as your preferred desk...`} />,
      infoNotificationOptions,
    );

    commitRemoveCustomerDefaultDesk({
      variables: {
        input: {
          clientMutationId: nanoid(),
          deskId: deskDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to remove the desk '${deskDetails.name}' as your preferred desk. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk '${deskDetails.name}' has been removed as your preferred desk.`} />,
        });

        setPreferredDesks(preferredDesks.filter((item) => item !== deskDetails.id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent content={`Failed to remove the desk '${deskDetails.name}' as your preferred desk. Error: ${error.message}.`} />
          ),
        });
      },
    });
  };

  if (!rootData.location) {
    return <></>;
  }

  const deskRows: DeskRowType[] = desks.map((desk) => ({
    id: desk.id,
    name: desk.name,
    customTags: desk.customTags.map((item) => ({ id: item.uniqueId, name: item.name, color: item.color })),
    zones: desk.zones.map((item) => ({ id: item.uniqueId, name: item.name, color: item.color })),
    status: !desk.deactivated,
    preferred: preferredDesks.includes(desk.id),
  }));

  const deskColumns: GridColDef<(typeof deskRows)[number]>[] = [
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'customTags',
      headerName: 'Tags',
      editable: false,
      renderCell: (params) => <CustomTags customTags={params.value} hideIcon />,
      display: 'flex',
      minWidth: 250,
    },
    {
      field: 'zones',
      headerName: 'Zones',
      editable: false,
      renderCell: (params) => <Zones zones={params.value} hideIcon />,
      display: 'flex',
      minWidth: 250,
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
      field: 'preferred',
      headerName: 'Preferred?',
      editable: false,
      renderCell: (params) => {
        const id = params.id as string;
        if (params.value) {
          return (
            <IconButton onClick={() => handleRemoveAsPreferredDeskClicked(id)}>
              <PreferredIcon />
            </IconButton>
          );
        }

        return (
          <IconButton onClick={() => handleSetAsPreferredDeskClicked(id)}>
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
              setSelectedDeskId(params.id as string);
              setDeskMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

  if (!rootData.location) {
    return <></>;
  }

  const location = rootData.location;

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationLocationLeftSideNavigationMenuContent organizationId={organizationId} locationId={locationId} hideIcons />
        <Box sx={{ marginLeft: expandedDrawerWidthPx, flexGrow: 1 }}>
          <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Location Information">
            <Form
              onSubmit={handleLocationDetailUpdateClick}
              initialValues={{
                name: location.name,
                about: location.about,
                timezone: location.timezone ?? '',
                physicalAddress: location.physicalAddress?.formattedAddress,
              }}
              validate={validateLocationDetails}
              render={({ handleSubmit }) => (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn
                    sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                    ref={(divElement) => {
                      sectionRefs.current['setup'] = divElement;
                    }}
                  >
                    <SectionIconTypography label="Location Setup" />
                    <BodyIconTypography label="Edit your location name and details" />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Name">
                      <TextField name="name" required={requiredLocationDetailsFields.name} />
                    </FormFieldLabel>

                    <FormFieldLabel label="About">
                      <TextField name="about" required={requiredLocationDetailsFields.about} multiline rows={3} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Timezone">
                      <SingleChoinceTimezone name="timezone" required={requiredLocationDetailsFields.timezone} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Physical Address">
                      <TextField name="physicalAddress" required={requiredLocationDetailsFields.physicalAddress} multiline rows={5} />
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

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['manage-desks'] = divElement;
              }}
            >
              <GridContainer sx={{ justifyContent: 'space-between' }}>
                <Grid>
                  <SectionIconTypography label=" Manage Desks" />
                  <BodyIconTypography label="Manage your location desks details" />
                </Grid>

                <Grid>
                  <AddDeskButton
                    onReloadRequired={onReloadRequired}
                    organizationId={organizationId}
                    locationId={locationId}
                    connectionIds={desksConnectionIds}
                  />
                  <BulkAddDeskButton
                    onReloadRequired={onReloadRequired}
                    organizationId={organizationId}
                    locationId={locationId}
                    connectionIds={desksConnectionIds}
                  />
                </Grid>
              </GridContainer>
              <Divider />
            </StackColumn>
            <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
              <ZoneSelector rootDataRelay={rootData} onChange={handleZoneTypeChanged} />
              <CustomTagSelector rootDataRelay={rootData} onChange={handleCustomTagChanged} />
              <PushToRight />
              <Search size="small" placeholder="Search for desks" defaultValue={deskNameSearchText} onChange={handleDeskNameSearchTextChange} />
            </GridContainer>

            {seledctedDesks.length > 0 && (
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
                    <SmallIconTypography label={`${seledctedDesks.length} records selected`} />
                    <PushToRight />
                    <Button size="medium" variant="contained" color="secondary" onClick={handleDeactivateDesksClick} sx={defaultButtonStyle}>
                      Deactivate Desk
                    </Button>
                    <Button size="medium" variant="contained" color="secondary" onClick={handleActivateDesksClick} sx={defaultButtonStyle}>
                      Activate Desk
                    </Button>
                    <Button
                      size="medium"
                      variant="contained"
                      color="warning"
                      startIcon={<DeleteIcon />}
                      onClick={handleRemoveDesksClick}
                      sx={{ textTransform: 'none' }}
                    >
                      Remove Desk
                    </Button>
                  </StackRow>
                </Box>
              </StackRow>
            )}

            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                checkboxSelection
                rowSelectionModel={seledctedDesks}
                onRowSelectionModelChange={handleSelectedDesksChanged}
                rows={deskRows}
                columns={deskColumns}
                hideFooterPagination={deskRows.length <= 10}
                initialState={{
                  pagination: {
                    rowCount: deskRows.length,
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
          </AppBarWithStackColumn>
        </Box>
      </Box>

      <MoreActionsMenu
        anchorEl={deskMoreActionsAnchorEl}
        open={deskMoreActionsMenuOpen}
        onMenuItemClick={handleDeskMoreActionsMenuItemClick}
        options={deskMoreActionsOption}
      />
    </>
  );
};

export default memo(OrganizationLocation);

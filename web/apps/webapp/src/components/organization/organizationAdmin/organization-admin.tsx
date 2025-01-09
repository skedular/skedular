import { AddOrganizationDeskTypeButton } from '@/components/organization/addOrganizationDeskType';
import { AddOrganizationZoneButton } from '@/components/organization/addOrganizationZone';
import { EditOrganizationZoneDialog } from '@/components/organization/editOrganizationZone/';
import type { organizationAdmin_deleteDeskTypesMutation } from '@/queries/__generated__/organizationAdmin_deleteDeskTypesMutation.graphql';
import type { organizationAdmin_deleteZonesMutation } from '@/queries/__generated__/organizationAdmin_deleteZonesMutation.graphql';
import type { organizationAdmin_deskTypes_query$key } from '@/queries/__generated__/organizationAdmin_deskTypes_query.graphql';
import type { organizationAdmin_deskTypes_refetchableFragment } from '@/queries/__generated__/organizationAdmin_deskTypes_refetchableFragment.graphql';
import type { organizationAdmin_query$key } from '@/queries/__generated__/organizationAdmin_query.graphql';
import type { organizationAdmin_zones_query$key } from '@/queries/__generated__/organizationAdmin_zones_query.graphql';
import type { organizationAdmin_zones_refetchableFragment } from '@/queries/__generated__/organizationAdmin_zones_refetchableFragment.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import {
  BodyIconTypography,
  PushToRight,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  StackColumnWithSaveExitCancelAppBar,
  StackRow,
} from '@repo/shared/components/commons';
import { DeskType } from '@repo/shared/components/deskType';
import { DeleteIcon, EllipseMenuIcon } from '@repo/shared/components/icons';
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
import { nanoid } from 'nanoid';
import { useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import EditOrganizationDeskTypeDialog from '../editOrganizationDeskType/edit-organization-desk-type-dialog';
import { expandedDrawerWidthPx } from './commons';
import OrganizationAdminLeftSideNavigationMenuContent from './organization-admin-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationAdmin_query$key;
  rootDataZonesRelay: organizationAdmin_zones_query$key;
  rootDataDeskTypesRelay: organizationAdmin_deskTypes_query$key;
  onReloadRequired: () => void;
  organizationId: string;
};

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

const OrganizationAdmin = ({ rootDataRelay, rootDataZonesRelay, rootDataDeskTypesRelay, onReloadRequired, organizationId }: Props) => {
  const rootData = useFragment<organizationAdmin_query$key>(
    graphql`
      fragment organizationAdmin_query on Query {
        organization(id: $organizationId) {
          id
          name
          about
        }
      }
    `,
    rootDataRelay,
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

  const [, startTransition] = useTransition();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
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

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationAdminLeftSideNavigationMenuContent organizationId={organizationId} hideIcons />
        <Box sx={{ marginLeft: expandedDrawerWidthPx, flexGrow: 1 }}>
          <StackColumnWithSaveExitCancelAppBar label="Edit Organization Information" hideCancel hideSaveAndExit>
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
    </>
  );
};

export default memo(OrganizationAdmin);

import type { organizationLocation_desks_query$key } from '@/queries/__generated__/organizationLocation_desks_query.graphql';
import type { organizationLocation_desks_refetchableFragment } from '@/queries/__generated__/organizationLocation_desks_refetchableFragment.graphql';
import type { organizationLocation_query$key } from '@/queries/__generated__/organizationLocation_query.graphql';
import type { organizationLocation_updateLocationMutation } from '@/queries/__generated__/organizationLocation_updateLocationMutation.graphql';
import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import {
  BodyIconTypography,
  FormFieldLabel,
  PushToRight,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  StackColumnWithSaveExitCancelAppBar,
  StackRow,
} from '@repo/shared/components/commons';
import { DeskTypes } from '@repo/shared/components/deskType';
import { SingleChoinceTimezone } from '@repo/shared/components/forms';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { Search } from '@repo/shared/components/search';
import { Zones } from '@repo/shared/components/zone';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { defaultGridStyle, defaultPadding } from '@repo/shared/libs/theme';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
import DeskTypeSelector from '../deskTypeSelector/desk-type-selector';
import { getModernOrganizationLocationsBaseLink } from '../organization-link';
import ZoneSelector from '../zoneSelector/zone-selector';
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
  timezone?: string;
  physicalAddress: string;
};

type DeskTypeDetails = {
  id: string;
  name: string | null | undefined;
};

type ZoneDetails = {
  id: string;
  name: string | null | undefined;
};

type DeskRowType = {
  id: string;
  name: string;
  deskTypes: ZoneDetails[];
  zones: ZoneDetails[];
};

const locationSchema = object({
  name: string().min(3, 'Location name must be at least three characters long.').required('Location name is required'),
  about: string().nullable(),
  timezone: string().nullable(),
  physicalAddress: string().nullable(),
});

const OrganizationLocation = ({ rootDataRelay, rootDataDesksRelay, organizationId, locationId }: Props) => {
  const rootData = useFragment<organizationLocation_query$key>(
    graphql`
      fragment organizationLocation_query on Query {
        location(id: $locationId) {
          id
          name
          about
          timezone
          physicalAddress {
            formattedAddress
          }
        }
        ...deskTypeSelector_allDeskTypes_query
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
        locationDesks(
          first: $count
          after: $cursor
          where: { locationId: $locationId, nameContains: $deskNameSearchText, deskTypeIds: $deskDeskTypeIds, zoneIds: $deskZoneIds }
        ) @connection(key: "organizationLocation_locationDesks") {
          __id
          totalCount
          edges {
            node {
              id
              name
              deactivated
              requireBookingApproval
              deskTypes {
                uniqueId
                name
              }
              zones {
                uniqueId
                name
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

  const [, startTransition] = useTransition();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const validate = makeValidate(locationSchema);
  const requiredFields = makeRequired(locationSchema);
  const [deskNameSearchText, setDeskNameSearchText] = useState<string>('');
  const [deskDeskTypeIds, setDeskDeskTypeIds] = useState<string[]>([]);
  const [deskZoneIds, setDeskZoneIds] = useState<string[]>([]);
  const [selectedDeskId, setSelectedDeskId] = useState<null | string>(null);
  const [seledctedDesks, setSeledctedDesks] = useState<GridRowSelectionModel>([]);

  const desksConnectionIds = useMemo(() => (rootDataDesks.locationDesks ? [rootDataDesks.locationDesks.__id] : []), [rootDataDesks.locationDesks]);
  const desks = useMemo(() => {
    if (!rootDataDesks.locationDesks) {
      return [];
    }

    return rootDataDesks.locationDesks.edges.map(({ node }) => node);
  }, [rootDataDesks.locationDesks]);
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
    (deskNameSearchText: string, deskDeskTypeIds: string[], deskZoneIds: string[]) => {
      startTransition(() => {
        refetchDesks(
          {
            deskNameSearchText,
            deskDeskTypeIds,
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

  const handleDetailUpdateClick = ({ name, about, timezone, physicalAddress }: LocationDetails) => {
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

        router.push(getModernOrganizationLocationsBaseLink(organizationId));
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

  const handleCancelClick = () => {
    router.push(getModernOrganizationLocationsBaseLink(organizationId));
  };

  const handleDeskNameSearchTextChange = (str: string) => {
    setDeskNameSearchText(str);

    handleRefetchDesks(str, deskZoneIds, deskDeskTypeIds);
  };

  const handleDeskTypeChanged = (id?: string) => {
    const newIds = id ? [id] : [];
    setDeskDeskTypeIds(newIds);

    handleRefetchDesks(deskNameSearchText, newIds, deskZoneIds);
  };

  const handleZoneTypeChanged = (id?: string) => {
    const newIds = id ? [id] : [];
    setDeskZoneIds(newIds);

    handleRefetchDesks(deskNameSearchText, deskDeskTypeIds, newIds);
  };

  const handleSelectedDesksChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedDesks(newRowSelectionModel);
  };

  if (!rootData.location) {
    return <></>;
  }

  const deskRows: DeskRowType[] = desks.map((desk) => ({
    id: desk.id,
    name: desk.name,
    deskTypes: desk.deskTypes.map((item) => ({ id: item.uniqueId, name: item.name })),
    zones: desk.zones.map((item) => ({ id: item.uniqueId, name: item.name })),
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
      field: 'deskTypes',
      headerName: 'Desk Types',
      editable: false,
      renderCell: (params) => <DeskTypes deskTypes={params.value} hideIcon />,
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
  ];

  const location = rootData.location;

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationLocationLeftSideNavigationMenuContent organizationId={organizationId} locationId={locationId} hideIcons />
        <Box sx={{ marginLeft: expandedDrawerWidthPx, flexGrow: 1 }}>
          <Form
            onSubmit={handleDetailUpdateClick}
            initialValues={{
              name: location.name,
              about: location.about,
              timezone: location.timezone,
              physicalAddress: location.physicalAddress?.formattedAddress,
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <StackColumnWithSaveExitCancelAppBar onSubmit={handleSubmit} onCancel={handleCancelClick} label="Edit Location Information">
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
                    <TextField name="name" required={requiredFields.name} />
                  </FormFieldLabel>

                  <FormFieldLabel label="About">
                    <TextField name="about" required={requiredFields.about} multiline rows={3} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Timezone">
                    <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Physical Address">
                    <TextField name="physicalAddress" required={requiredFields.physicalAddress} multiline rows={5} />
                  </FormFieldLabel>
                </StackColumn>

                <StackColumn
                  sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
                  ref={(divElement) => {
                    sectionRefs.current['manage-desks'] = divElement;
                  }}
                >
                  <SectionIconTypography label=" Manage Desks" />
                  <BodyIconTypography label="Manage your location desks details" />
                  <Divider />
                </StackColumn>

                <StackRow sx={{ padding: defaultPadding }}>
                  <DeskTypeSelector rootDataRelay={rootData} onChange={handleDeskTypeChanged} />
                  <ZoneSelector rootDataRelay={rootData} onChange={handleZoneTypeChanged} />
                  <PushToRight />
                  <Search size="small" placeholder="Search for desks" defaultValue={deskNameSearchText} onChange={handleDeskNameSearchTextChange} />
                </StackRow>

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
              </StackColumnWithSaveExitCancelAppBar>
            )}
          />
        </Box>
      </Box>
    </>
  );
};

export default memo(OrganizationLocation);

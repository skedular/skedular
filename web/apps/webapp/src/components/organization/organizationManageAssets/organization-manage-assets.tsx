import type { organizationManageAssets_deskTypes_query$key } from '@/queries/__generated__/organizationManageAssets_deskTypes_query.graphql';
import type { organizationManageAssets_deskTypes_refetchableFragment } from '@/queries/__generated__/organizationManageAssets_deskTypes_refetchableFragment.graphql';
import type { organizationManageAssets_rootQuery } from '@/queries/__generated__/organizationManageAssets_rootQuery.graphql';
import type { organizationManageAssets_zones_query$key } from '@/queries/__generated__/organizationManageAssets_zones_query.graphql';
import type { organizationManageAssets_zones_refetchableFragment } from '@/queries/__generated__/organizationManageAssets_zones_refetchableFragment.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
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
import { DeleteIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Search } from '@repo/shared/components/search';
import { defaultGridActionPadding, defaultGridStyle, defaultPadding } from '@repo/shared/libs/theme';
import { nanoid } from 'nanoid';
import { useSearchParams } from 'next/navigation';
import { memo, useCallback, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { expandedDrawerWidthPx } from './commons';
import OrganizationManageAssetsLeftSideNavigationMenuContent from './organization-manage-assets-left-side-navigation-menu-content';

type Props = {
  queryReference: PreloadedQuery<organizationManageAssets_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationManageAssets_rootQuery($organizationId: String!, $zoneNameSearchText: String, $deskTypeNameSearchText: String) {
    ...organizationManageAssets_zones_query
    ...organizationManageAssets_deskTypes_query
  }
`;

type ZoneRowType = {
  id: string;
  name: string;
};

type DeskTypeRowType = {
  id: string;
  name: string;
};

const OrganizationManageAssets = ({ queryReference, organizationId }: Props) => {
  const rootData = usePreloadedQuery<organizationManageAssets_rootQuery>(RootQuery, queryReference);
  const [rootDataZones, refetchZones] = useRefetchableFragment<
    organizationManageAssets_zones_refetchableFragment,
    organizationManageAssets_zones_query$key
  >(
    graphql`
      fragment organizationManageAssets_zones_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationManageAssets_zones_refetchableFragment") {
        zones(first: $count, after: $cursor, where: { organizationId: $organizationId, nameContains: $zoneNameSearchText })
          @connection(key: "organizationManageAssets_zones") {
          __id
          totalCount
          edges {
            node {
              id
              name
            }
          }
        }
      }
    `,
    rootData,
  );

  const [rootDataDeskTypes, refetchDeskTypes] = useRefetchableFragment<
    organizationManageAssets_deskTypes_refetchableFragment,
    organizationManageAssets_deskTypes_query$key
  >(
    graphql`
      fragment organizationManageAssets_deskTypes_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationManageAssets_deskTypes_refetchableFragment") {
        deskTypes(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $deskTypeNameSearchText }
          orderBy: [{ direction: Ascending, field: Name }]
        ) @connection(key: "organizationManageAssets_deskTypes") {
          __id
          totalCount
          edges {
            node {
              id
              name
            }
          }
        }
      }
    `,
    rootData,
  );

  const [, startTransition] = useTransition();
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const [zoneNameSearchText, setZoneNameSearchText] = useState<string>('');
  const [deskTypeNameSearchText, setDeskTypeNameSearchText] = useState<string>('');
  const [seledctedZones, setSeledctedZones] = useState<GridRowSelectionModel>([]);
  const [seledctedDeskTypes, setSeledctedDeskTypes] = useState<GridRowSelectionModel>([]);

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
    if (!section || section === 'zones-setup') {
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
    setSeledctedZones(newRowSelectionModel);
  };

  const handleRemoveZonesClick = () => {};

  const handleRemoveDeskTypesClick = () => {};

  const zoneRows: ZoneRowType[] = zones.map((zone) => ({
    id: zone.id,
    name: zone.name,
  }));

  const zoneColumns: GridColDef<(typeof zoneRows)[number]>[] = [
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 200,
    },
  ];

  const deskTypeRows: DeskTypeRowType[] = deskTypes.map((deskType) => ({
    id: deskType.id,
    name: deskType.name,
  }));

  const deskTypeColumns: GridColDef<(typeof deskTypeRows)[number]>[] = [
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 200,
    },
  ];

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationManageAssetsLeftSideNavigationMenuContent organizationId={organizationId} hideIcons />
        <Box sx={{ marginLeft: expandedDrawerWidthPx, flexGrow: 1 }}>
          <StackColumnWithSaveExitCancelAppBar label="Manage Assets" hideCancel hideSaveAndExit>
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
    </>
  );
};

const MemoOrganizationManageAssets = memo(OrganizationManageAssets);

type RelayProps = {
  organizationId: string;
};

const OrganizationManageAssetsWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationManageAssets_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOrganizationManageAssets queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationManageAssetsWithRelay);

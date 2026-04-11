import { GridContainer, StackColumn } from '@/components/commons';
import { defaultPadding } from '@/libs/theme';
import type { marketplaceLocations_locations_query$key } from '@/queries/__generated__/marketplaceLocations_locations_query.graphql';
import type { marketplaceLocations_locations_refetchableFragment, OrganizationTagType } from '@/queries/__generated__/marketplaceLocations_locations_refetchableFragment.graphql';
import type { marketplaceLocations_query$key } from '@/queries/__generated__/marketplaceLocations_query.graphql';
import '@/styles/leaflet/leaflet.css';
import { useMediaQuery, useTheme } from '@mui/material';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import Pagination from '@mui/material/Pagination';
import type { Theme } from '@mui/material/styles';
import type { LatLngBounds, LatLngTuple } from 'leaflet';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { IPinfoWrapper } from 'node-ipinfo';
import { memo, startTransition, useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useMap, useMapEvents } from 'react-leaflet';
import 'react-leaflet-cluster/dist/assets/MarkerCluster.css';
import 'react-leaflet-cluster/dist/assets/MarkerCluster.Default.css';
import { graphql, useFragment, useRefetchableFragment } from 'react-relay';
import MarketplaceLocationCard from './marketplace-location-card';

let L: typeof import('leaflet');
let MapContainer: typeof import('react-leaflet').MapContainer;
let Marker: typeof import('react-leaflet').Marker;
let TileLayer: typeof import('react-leaflet').TileLayer;
let Popup: typeof import('react-leaflet').Popup;
// eslint-disable-next-line @typescript-eslint/no-explicit-any
let MarkerClusterGroup: any;

const getToolbarHeight = (theme: Theme) => {
  const minHeight = theme.mixins?.toolbar?.minHeight;

  if (typeof minHeight === 'number') {
    return minHeight;
  }

  if (typeof minHeight === 'string') {
    const parsed = Number.parseInt(minHeight, 10);
    if (Number.isFinite(parsed)) {
      return parsed;
    }
  }

  return 56;
};

type Props = {
  rootDataRelay: marketplaceLocations_query$key;
  rootDataLocationsRelay: marketplaceLocations_locations_query$key;
  onReloadRequired: () => void;
};

const pageSize = 9;

const MarketplaceLocations = ({ rootDataRelay, rootDataLocationsRelay, onReloadRequired }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment marketplaceLocations_query on Query {
        ...marketplaceLocationCard_query
      }
    `,
    rootDataRelay,
  );

  const [rootDataLocationsRefetchable, refetchLocations] = useRefetchableFragment<marketplaceLocations_locations_refetchableFragment, marketplaceLocations_locations_query$key>(
    graphql`
      fragment marketplaceLocations_locations_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "marketplaceLocations_locations_refetchableFragment") {
        marketplaceLocations(
          first: $count
          after: $cursor
          where: { searchBoundaries: $searchBoundaries, resourceType: $resourceTypeToFilterWith }
          orderBy: $locationsSortingValues
        ) @connection(key: "locations_marketplaceLocations") {
          __id
          totalCount
          edges {
            node {
              id
              name
              physicalAddress {
                longitude
                latitude
              }
              ...marketplaceLocationCard_LocationDetails
            }
          }
        }
      }
    `,
    rootDataLocationsRelay,
  );

  const theme = useTheme();
  const isMobileOrTablet = useMediaQuery(theme.breakpoints.down('md'));
  const toolbarHeight = getToolbarHeight(theme);
  const mapHeight = isMobileOrTablet ? `calc(100dvh - ${toolbarHeight}px)` : '90vh';
  const [dynamicLoadReady, setDynamicLoadReady] = useState(false);
  const locations = useMemo(() => rootDataLocationsRefetchable.marketplaceLocations.edges.map((edge) => edge.node), [rootDataLocationsRefetchable.marketplaceLocations]);
  const [initialPosition, setInitialPosition] = useState<LatLngTuple>([-36.8485, 174.7633]); // Auckland
  const [initialZoom, setInitialZoom] = useState(13);
  const [searchBoundaries, setSearchBoundaries] = useState<LatLngBounds | null>(null);
  const [centerSet, setCenterSet] = useState(false);
  const [selectedLocationId, setSelectedLocationId] = useState<string | null>(null);
  const [pageIndex, setPageIndex] = useState(0);
  const searchParams = useSearchParams();
  const pathname = usePathname();
  const router = useRouter();
  const lastQueryRef = useRef<string | null>(null); // guard against redundant router.replace loops when syncing map center/zoom to query params
  const currentUrl = useMemo(() => {
    const params = searchParams?.toString();
    return params ? `${pathname}?${params}` : pathname;
  }, [pathname, searchParams]);

  if (lastQueryRef.current === null) {
    // seed with the initial URL so the first replace is only triggered on actual changes
    lastQueryRef.current = currentUrl;
  }

  const selectedLocation = useMemo(() => {
    if (!selectedLocationId) {
      return null;
    }

    return locations.find((location) => location.id === selectedLocationId) ?? null;
  }, [locations, selectedLocationId]);

  useEffect(() => {
    if (selectedLocationId && !selectedLocation) {
      setSelectedLocationId(null);
    }
  }, [selectedLocationId, selectedLocation]);

  useEffect(() => {
    (async () => {
      // core libraries
      const leaflet = await import('leaflet');
      const rl = await import('react-leaflet');
      const rlCluster = await import('react-leaflet-cluster');

      L = leaflet;
      MapContainer = rl.MapContainer;
      Marker = rl.Marker;
      TileLayer = rl.TileLayer;
      Popup = rl.Popup;
      MarkerClusterGroup = rlCluster && (rlCluster.default ?? rlCluster);

      L.Icon.Default.mergeOptions({
        iconRetinaUrl: '/leaflet/images/marker-icon-2x.png',
        iconUrl: '/leaflet/images/marker-icon.png',
        shadowUrl: '/leaflet/images/marker-shadow.png',
      });

      const latParam = searchParams?.get('lat');
      const lngParam = searchParams?.get('lng');
      const zoomParam = searchParams?.get('zoom');
      const latNum = latParam ? Number.parseFloat(latParam) : null;
      const lngNum = lngParam ? Number.parseFloat(lngParam) : null;
      const zoomNum = zoomParam ? Number.parseInt(zoomParam, 10) : null;
      const hasQueryLocation = Number.isFinite(latNum) && Number.isFinite(lngNum);

      if (hasQueryLocation) {
        setInitialPosition([latNum as number, lngNum as number]);
        if (Number.isFinite(zoomNum)) {
          setInitialZoom(zoomNum as number);
        }
      } else if ('geolocation' in navigator) {
        navigator.geolocation.getCurrentPosition(({ coords }) => {
          setInitialPosition([coords.latitude, coords.longitude]);
          setCenterSet(false);
        });
      } else {
        const ipinfoWrapper = new IPinfoWrapper('');
        const ipinfo = await ipinfoWrapper.lookupIp('');

        if (ipinfo.loc) {
          const [latitude, longitude] = ipinfo.loc.split(',');
          if (latitude && longitude) {
            setInitialPosition([parseFloat(latitude), parseFloat(longitude)]);
            setCenterSet(false);
          }
        }
      }

      setDynamicLoadReady(true);
    })();
  }, [searchParams]);

  const handleRefetch = useCallback(
    (searchBoundaries: LatLngBounds | null, resourceType: string | null | undefined) => {
      startTransition(() => {
        refetchLocations(
          {
            searchBoundaries: searchBoundaries
              ? {
                  southWest: {
                    longitude: searchBoundaries.getSouthWest().lng,
                    latitude: searchBoundaries.getSouthWest().lat,
                  },
                  northEast: {
                    longitude: searchBoundaries.getNorthEast().lng,
                    latitude: searchBoundaries.getNorthEast().lat,
                  },
                }
              : null,
            resourceTypeToFilterWith: resourceType ? (resourceType as OrganizationTagType) : null,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchLocations],
  );

  useEffect(() => {
    handleRefetch(searchBoundaries, null);
  }, [searchBoundaries, handleRefetch]);

  useEffect(() => {
    setPageIndex(0);
  }, [isMobileOrTablet, locations.length]);

  const paginatedLocations = useMemo(() => {
    if (isMobileOrTablet) {
      return locations;
    }

    const start = pageIndex * pageSize;
    return locations.slice(start, start + pageSize);
  }, [isMobileOrTablet, locations, pageIndex]);

  const pageCount = useMemo(() => (isMobileOrTablet ? 1 : Math.max(1, Math.ceil(locations.length / pageSize))), [isMobileOrTablet, locations.length]);

  if (!dynamicLoadReady) {
    return null;
  }

  const MapInitBoundsTracker = () => {
    const map = useMap();

    useEffect(() => {
      const newBounds = map.getBounds();
      if (!searchBoundaries) {
        setSearchBoundaries(map.getBounds());

        return;
      }

      const oldSW = searchBoundaries.getSouthWest();
      const oldNE = searchBoundaries.getNorthEast();
      const newSW = newBounds.getSouthWest();
      const newNE = newBounds.getNorthEast();

      if (oldSW.lat !== newSW.lat || oldSW.lng !== newSW.lng || oldNE.lat !== newNE.lat || oldNE.lng !== newNE.lng) {
        setSearchBoundaries(map.getBounds());

        return;
      }
    }, [map]);

    return null;
  };

  const MapUpdater = ({ center }: { center: LatLngTuple }) => {
    const map = useMap();

    useEffect(() => {
      map.setView(center, map.getZoom());

      setCenterSet(true);
    }, [center, map]);

    return null;
  };

  const updateQueryFromMap = (lat: number, lng: number, zoom: number) => {
    const params = new URLSearchParams(searchParams?.toString());

    params.set('lat', lat.toString());
    params.set('lng', lng.toString());
    params.set('zoom', zoom.toString());

    const newUrl = `${pathname}?${params.toString()}`;
    if (lastQueryRef.current === newUrl) {
      return;
    }

    lastQueryRef.current = newUrl;
    router.replace(newUrl, { scroll: false });
  };

  const MapCenterTracker = () => {
    const map = useMapEvents({
      moveend: () => {
        const newBounds = map.getBounds();
        if (!searchBoundaries) {
          setSearchBoundaries(map.getBounds());
        } else {
          const oldSouthWest = searchBoundaries.getSouthWest();
          const oldNorthEast = searchBoundaries.getNorthEast();
          const newSouthWest = newBounds.getSouthWest();
          const newNorthEast = newBounds.getNorthEast();

          if (oldSouthWest.lat !== newSouthWest.lat || oldSouthWest.lng !== newSouthWest.lng || oldNorthEast.lat !== newNorthEast.lat || oldNorthEast.lng !== newNorthEast.lng) {
            setSearchBoundaries(map.getBounds());
          }
        }
        const center = map.getCenter();
        updateQueryFromMap(center.lat, center.lng, map.getZoom());
      },
      zoomend: () => {
        const center = map.getCenter();
        updateQueryFromMap(center.lat, center.lng, map.getZoom());
      },
    });

    return null;
  };

  const MapSection = (
    <Box sx={{ height: mapHeight, width: '100%', position: 'relative' }}>
      <MapContainer center={initialPosition} zoom={initialZoom} scrollWheelZoom={true} style={{ height: '100%', width: '100%' }}>
        <TileLayer attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors' url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
        <MarkerClusterGroup chunkedLoading>
          {locations
            .filter((item) => !!item.physicalAddress && !!item.physicalAddress.latitude && !!item.physicalAddress.longitude)
            .map((item) => (
              <Marker
                key={item.id}
                position={[item.physicalAddress!.latitude!, item.physicalAddress!.longitude!]}
                eventHandlers={{
                  click: () => {
                    setSelectedLocationId(item.id);
                  },
                }}
              />
            ))}
        </MarkerClusterGroup>

        {!isMobileOrTablet && selectedLocation?.physicalAddress?.latitude && selectedLocation?.physicalAddress?.longitude ? (
          <Popup
            key={selectedLocation.id}
            position={[selectedLocation.physicalAddress.latitude, selectedLocation.physicalAddress.longitude]}
            closeButton={false}
            className="marketplace-location-popup"
            eventHandlers={{
              remove: () => {
                setSelectedLocationId((current) => (current === selectedLocation.id ? null : current));
              },
              popupclose: () => {
                setSelectedLocationId((current) => (current === selectedLocation.id ? null : current));
              },
            }}
          >
            <MarketplaceLocationCard
              rootDataRelay={rootData}
              locationDetailsRelay={selectedLocation}
              onReloadRequired={onReloadRequired}
              onClose={() => setSelectedLocationId(null)}
            />
          </Popup>
        ) : null}

        <MapInitBoundsTracker />
        <MapCenterTracker />
        {!centerSet && <MapUpdater center={initialPosition} />}
      </MapContainer>
      {isMobileOrTablet && selectedLocation && (
        <Box
          sx={{
            position: 'absolute',
            bottom: `calc(${theme.spacing(3)} + env(safe-area-inset-bottom, 0px))`,
            display: 'flex',
            justifyContent: 'center',
            zIndex: 1000,
            pointerEvents: 'none',
            paddingLeft: defaultPadding,
            paddingRight: defaultPadding,
            paddingTop: defaultPadding,
            paddingBottom: defaultPadding,
          }}
        >
          <Box sx={{ pointerEvents: 'auto' }}>
            <MarketplaceLocationCard
              rootDataRelay={rootData}
              locationDetailsRelay={selectedLocation}
              onReloadRequired={onReloadRequired}
              onClose={() => setSelectedLocationId(null)}
            />
          </Box>
        </Box>
      )}
    </Box>
  );

  return (
    <StackColumn sx={{ p: isMobileOrTablet ? 0 : defaultPadding }}>
      {isMobileOrTablet ? (
        <>{MapSection}</>
      ) : (
        <GridContainer spacing={2}>
          <Grid size={{ xs: 12, md: 7 }}>
            <GridContainer sx={{ alignItems: 'stretch' }} spacing={1}>
              {paginatedLocations.map((item) => (
                <Grid key={item.id} size={{ xs: 12, sm: 6, lg: 4 }}>
                  <MarketplaceLocationCard rootDataRelay={rootData} locationDetailsRelay={item} onReloadRequired={onReloadRequired} />
                </Grid>
              ))}
            </GridContainer>
            {pageCount > 1 && (
              <StackColumn sx={{ mt: 2, gap: 1, alignItems: 'center' }}>
                <Pagination count={pageCount} page={pageIndex + 1} onChange={(_, page) => setPageIndex(page - 1)} color="primary" siblingCount={1} boundaryCount={1} />
              </StackColumn>
            )}
          </Grid>
          <Grid size={{ xs: 12, md: 5 }}>{MapSection}</Grid>
        </GridContainer>
      )}
    </StackColumn>
  );
};

export default memo(MarketplaceLocations);

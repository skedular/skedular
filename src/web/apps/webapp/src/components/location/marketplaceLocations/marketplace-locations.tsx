import logger from '@/libs/logging';
import { logAggregateMarketplaceDiscoveryCompleted } from '@/libs/logging/aggregate-marketplace-telemetry';
import type { marketplaceLocations_locations_query$key } from '@/queries/__generated__/marketplaceLocations_locations_query.graphql';
import type { marketplaceLocations_locations_refetchableFragment, OrganizationTagType } from '@/queries/__generated__/marketplaceLocations_locations_refetchableFragment.graphql';
import '@/styles/leaflet/leaflet.css';
import useMediaQuery from '@mui/material/useMediaQuery';
import { useTheme } from '@mui/material/styles';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import Pagination from '@mui/material/Pagination';
import type { Theme } from '@mui/material/styles';
import { defaultPadding, GridContainer, StackColumn } from '@skedular/ui';
import type { LatLngBounds, LatLngTuple } from 'leaflet';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { memo, startTransition, useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useMap, useMapEvents } from 'react-leaflet';
import 'react-leaflet-cluster/dist/assets/MarkerCluster.css';
import 'react-leaflet-cluster/dist/assets/MarkerCluster.Default.css';
import { graphql, useRefetchableFragment } from 'react-relay';
import MarketplaceLocationCard from './marketplace-location-card';

let L: typeof import('leaflet');
let MapContainer: typeof import('react-leaflet').MapContainer;
let Marker: typeof import('react-leaflet').Marker;
let TileLayer: typeof import('react-leaflet').TileLayer;
let Popup: typeof import('react-leaflet').Popup;
let MarkerClusterGroup: typeof import('react-leaflet-cluster').default;

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
  rootDataLocationsRelay: marketplaceLocations_locations_query$key;
  favouriteLocationIds?: ReadonlySet<string>;
  onReloadRequired: () => void;
};

const areBoundsEqual = (currentBounds: LatLngBounds | null, nextBounds: LatLngBounds) => {
  if (!currentBounds) {
    return false;
  }

  const currentSouthWest = currentBounds.getSouthWest();
  const currentNorthEast = currentBounds.getNorthEast();
  const nextSouthWest = nextBounds.getSouthWest();
  const nextNorthEast = nextBounds.getNorthEast();

  return (
    currentSouthWest.lat === nextSouthWest.lat &&
    currentSouthWest.lng === nextSouthWest.lng &&
    currentNorthEast.lat === nextNorthEast.lat &&
    currentNorthEast.lng === nextNorthEast.lng
  );
};

const MapInitBoundsTracker = ({ searchBoundaries, onBoundsChange }: { searchBoundaries: LatLngBounds | null; onBoundsChange: (bounds: LatLngBounds) => void }) => {
  const map = useMap();

  useEffect(() => {
    const nextBounds = map.getBounds();
    if (!areBoundsEqual(searchBoundaries, nextBounds)) {
      onBoundsChange(nextBounds);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [map]);

  return null;
};

const MapUpdater = ({ center, onCenterSet }: { center: LatLngTuple; onCenterSet: () => void }) => {
  const map = useMap();

  useEffect(() => {
    map.setView(center, map.getZoom());

    onCenterSet();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [center, map]);

  return null;
};

const MapCenterTracker = ({
  searchBoundaries,
  onBoundsChange,
  onMapMove,
}: {
  searchBoundaries: LatLngBounds | null;
  onBoundsChange: (bounds: LatLngBounds) => void;
  onMapMove: (lat: number, lng: number, zoom: number) => void;
}) => {
  const map = useMapEvents({
    moveend: () => {
      const nextBounds = map.getBounds();
      if (!areBoundsEqual(searchBoundaries, nextBounds)) {
        onBoundsChange(nextBounds);
      }

      const center = map.getCenter();
      onMapMove(center.lat, center.lng, map.getZoom());
    },
    zoomend: () => {
      const nextBounds = map.getBounds();
      if (!areBoundsEqual(searchBoundaries, nextBounds)) {
        onBoundsChange(nextBounds);
      }

      const center = map.getCenter();
      onMapMove(center.lat, center.lng, map.getZoom());
    },
  });

  return null;
};

const MarketplaceLocations = ({ rootDataLocationsRelay, favouriteLocationIds, onReloadRequired }: Props) => {
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
              organization {
                type {
                  type
                }
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
  const isLgUp = useMediaQuery(theme.breakpoints.up('lg'));
  const isXlUp = useMediaQuery(theme.breakpoints.up('xl'));
  const desktopPageSize = isXlUp ? 12 : isLgUp ? 9 : 6;
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

  const handleBoundsChange = useCallback((nextBounds: LatLngBounds) => {
    setSearchBoundaries((currentBounds) => (areBoundsEqual(currentBounds, nextBounds) ? currentBounds : nextBounds));
  }, []);

  const selectedLocation = useMemo(() => {
    if (!selectedLocationId) {
      return null;
    }

    return locations.find((location) => location.id === selectedLocationId) ?? null;
  }, [locations, selectedLocationId]);

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
        const geo = (await fetch('/api/geolocation').then((r) => r.json())) as { lat: string | null; lng: string | null };

        if (geo.lat && geo.lng) {
          setInitialPosition([parseFloat(geo.lat), parseFloat(geo.lng)]);
          setCenterSet(false);
        }
      }

      setDynamicLoadReady(true);
    })();
  }, [searchParams]);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    setPageIndex(0);
  }, [desktopPageSize, locations.length, isMobileOrTablet]);

  useEffect(() => {
    logAggregateMarketplaceDiscoveryCompleted({ logger, eligibleLocationCount: locations.length, isEmptyState: locations.length === 0 });
  }, [locations.length]);

  const paginatedLocations = useMemo(() => {
    if (isMobileOrTablet) {
      return locations;
    }

    const start = pageIndex * desktopPageSize;
    return locations.slice(start, start + desktopPageSize);
  }, [desktopPageSize, locations, isMobileOrTablet, pageIndex]);

  const pageCount = useMemo(() => (isMobileOrTablet ? 1 : Math.max(1, Math.ceil(locations.length / desktopPageSize))), [desktopPageSize, locations.length, isMobileOrTablet]);

  const handleRefetch = useCallback(
    (nextSearchBoundaries: LatLngBounds | null, resourceType: string | null | undefined) => {
      startTransition(() => {
        refetchLocations(
          {
            locationsSortingValues: [
              {
                direction: 'ASCENDING',
                field: 'NAME',
              },
            ],
            searchBoundaries: nextSearchBoundaries
              ? {
                  southWest: {
                    longitude: nextSearchBoundaries.getSouthWest().lng,
                    latitude: nextSearchBoundaries.getSouthWest().lat,
                  },
                  northEast: {
                    longitude: nextSearchBoundaries.getNorthEast().lng,
                    latitude: nextSearchBoundaries.getNorthEast().lat,
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

  if (!dynamicLoadReady) {
    return null;
  }

  const handleMapMove = (lat: number, lng: number, zoom: number) => {
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

  const MapSection = (
    <Box sx={{ height: mapHeight, width: '100%', position: 'relative' }}>
      <MapContainer center={initialPosition} zoom={initialZoom} scrollWheelZoom={true} style={{ height: '100%', width: '100%' }}>
        <TileLayer attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors' url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
        <MarkerClusterGroup chunkedLoading>
          {locations
            .filter((item) => Number.isFinite(item.physicalAddress?.latitude) && Number.isFinite(item.physicalAddress?.longitude))
            .map((item) => (
              <Marker
                key={item.id}
                position={[item.physicalAddress!.latitude!, item.physicalAddress!.longitude!]}
                {...(item.organization.type.type === 'HOST'
                  ? {
                      icon: L.divIcon({
                        className: 'host-location-marker',
                        html: '<span aria-label="Hosted place" style="display:grid;place-items:center;width:32px;height:32px;border-radius:50% 50% 50% 0;transform:rotate(-45deg);background:#7c3aed;color:white;font-weight:800;border:2px solid white;box-shadow:0 2px 8px #0005"><span style="transform:rotate(45deg)">H</span></span>',
                        iconSize: [32, 32],
                        iconAnchor: [16, 32],
                      }),
                    }
                  : {})}
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
              favouriteLocationIds={favouriteLocationIds}
              locationDetailsRelay={selectedLocation}
              onReloadRequired={onReloadRequired}
              onClose={() => setSelectedLocationId(null)}
            />
          </Popup>
        ) : null}

        <MapInitBoundsTracker searchBoundaries={searchBoundaries} onBoundsChange={handleBoundsChange} />
        <MapCenterTracker searchBoundaries={searchBoundaries} onBoundsChange={handleBoundsChange} onMapMove={handleMapMove} />
        {!centerSet && <MapUpdater center={initialPosition} onCenterSet={() => setCenterSet(true)} />}
      </MapContainer>
      {isMobileOrTablet && selectedLocation && (
        <Box
          sx={{
            position: 'fixed',
            left: { xs: 16, sm: 24 },
            right: { xs: 16, sm: 24 },
            width: { xs: 'calc(100dvw - 32px)', sm: 'calc(100dvw - 48px)' },
            maxWidth: { xs: 'calc(100dvw - 32px)', sm: 'calc(100dvw - 48px)' },
            top: `calc(${toolbarHeight}px + 16px)`,
            bottom: { xs: 'calc(72px + env(safe-area-inset-bottom, 0px))', sm: 'calc(80px + env(safe-area-inset-bottom, 0px))' },
            display: 'flex',
            alignItems: 'flex-end',
            justifyContent: 'center',
            zIndex: 1000,
            pointerEvents: 'none',
            boxSizing: 'border-box',
          }}
        >
          <Box
            sx={{
              pointerEvents: 'auto',
              display: 'flex',
              width: '100%',
              minWidth: 0,
              maxHeight: '100%',
              overflow: 'hidden',
              borderRadius: 4,
              overscrollBehavior: 'contain',
            }}
          >
            <MarketplaceLocationCard
              favouriteLocationIds={favouriteLocationIds}
              locationDetailsRelay={selectedLocation}
              onReloadRequired={onReloadRequired}
              onClose={() => setSelectedLocationId(null)}
              fullWidthPopup
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
            <Box
              sx={{
                display: 'grid',
                gridTemplateColumns: {
                  xs: 'minmax(0, 1fr)',
                  sm: 'repeat(2, minmax(0, 1fr))',
                  lg: 'repeat(3, minmax(0, 1fr))',
                  xl: 'repeat(4, minmax(0, 1fr))',
                },
                gap: 1,
                gridAutoRows: '1fr',
                '@media (min-width: 2560px)': {
                  gridTemplateColumns: 'repeat(6, minmax(0, 1fr))',
                },
              }}
            >
              {paginatedLocations.map((item) => (
                <Box key={item.id} sx={{ minWidth: 0, height: '100%' }}>
                  <MarketplaceLocationCard favouriteLocationIds={favouriteLocationIds} locationDetailsRelay={item} onReloadRequired={onReloadRequired} equalHeight />
                </Box>
              ))}
            </Box>
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

import { GridContainer, StackColumn } from '@/components/commons';
import { defaultPadding } from '@/libs/theme';
import type { marketplaceLocations_locations_query$key } from '@/queries/__generated__/marketplaceLocations_locations_query.graphql';
import type { marketplaceLocations_locations_refetchableFragment } from '@/queries/__generated__/marketplaceLocations_locations_refetchableFragment.graphql';
import { useMediaQuery, useTheme } from '@mui/material';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import type { LatLngBounds, LatLngTuple } from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { IPinfoWrapper } from 'node-ipinfo';
import { memo, startTransition, useCallback, useEffect, useMemo, useState } from 'react';
import { useMap, useMapEvents } from 'react-leaflet';
import 'react-leaflet-cluster/dist/assets/MarkerCluster.css';
import 'react-leaflet-cluster/dist/assets/MarkerCluster.Default.css';
import { graphql, useRefetchableFragment } from 'react-relay';
import MarketplaceLocationCard from './marketplace-location-card';
import MarketplaceLocationPopupCard from './marketplace-location-popup-card';

let L: typeof import('leaflet');
let MapContainer: typeof import('react-leaflet').MapContainer;
let Marker: typeof import('react-leaflet').Marker;
let TileLayer: typeof import('react-leaflet').TileLayer;
let Popup: typeof import('react-leaflet').Popup;
// eslint-disable-next-line @typescript-eslint/no-explicit-any
let MarkerClusterGroup: any;

type Props = {
  rootDataRelay: marketplaceLocations_locations_query$key;
  onReloadRequired: () => void;
};

const MarketplaceLocations = ({ rootDataRelay, onReloadRequired }: Props) => {
  const [rootDataRefetchable, refetch] = useRefetchableFragment<marketplaceLocations_locations_refetchableFragment, marketplaceLocations_locations_query$key>(
    graphql`
      fragment marketplaceLocations_locations_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "marketplaceLocations_locations_refetchableFragment") {
        marketplaceLocations(first: $count, after: $cursor, where: { searchBoundaries: $searchBoundaries }, orderBy: $locationsSortingValues)
          @connection(key: "locations_marketplaceLocations") {
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
              ...marketplaceLocationPopupCard_LocationDetails
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const [dynamicLoadReady, setDynamicLoadReady] = useState(false);
  const locations = useMemo(() => rootDataRefetchable.marketplaceLocations.edges.map((edge) => edge.node), [rootDataRefetchable.marketplaceLocations]);
  const [initialPosition, setInitialPosition] = useState<LatLngTuple>([-36.8485, 174.7633]); // Auckland
  const [searchBoundaries, setSearchBoundaries] = useState<{
    southWest: LatLngTuple;
    northEast: LatLngTuple;
  } | null>(null);
  const [centerSet, setCenterSet] = useState(false);
  const [activePopupId, setActivePopupId] = useState<string | null>(null);

  const updateSearchBoundariesFromBounds = useCallback((bounds: LatLngBounds) => {
    const nextSouthWest = bounds.getSouthWest();
    const nextNorthEast = bounds.getNorthEast();

    setSearchBoundaries((currentBounds) => {
      if (
        currentBounds &&
        currentBounds.southWest[0] === nextSouthWest.lat &&
        currentBounds.southWest[1] === nextSouthWest.lng &&
        currentBounds.northEast[0] === nextNorthEast.lat &&
        currentBounds.northEast[1] === nextNorthEast.lng
      ) {
        return currentBounds;
      }

      return {
        southWest: [nextSouthWest.lat, nextSouthWest.lng],
        northEast: [nextNorthEast.lat, nextNorthEast.lng],
      };
    });
  }, []);

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

      if ('geolocation' in navigator) {
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
  }, []);

  const handleRefetch = useCallback(
    (
      searchBoundaries: {
        southWest: LatLngTuple;
        northEast: LatLngTuple;
      } | null,
    ) => {
      startTransition(() => {
        refetch(
          {
            searchBoundaries: searchBoundaries
              ? {
                  southWest: {
                    longitude: searchBoundaries.southWest[1],
                    latitude: searchBoundaries.southWest[0],
                  },
                  northEast: {
                    longitude: searchBoundaries.northEast[1],
                    latitude: searchBoundaries.northEast[0],
                  },
                }
              : null,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch],
  );

  useEffect(() => {
    handleRefetch(searchBoundaries);
  }, [searchBoundaries, handleRefetch]);

  if (!dynamicLoadReady) {
    return <></>;
  }

  const MapInitBoundsTracker = ({ popupOpen }: { popupOpen: boolean }) => {
    const map = useMap();

    useEffect(() => {
      if (popupOpen) {
        return;
      }

      const newBounds = map.getBounds();
      updateSearchBoundariesFromBounds(newBounds);
    }, [map, popupOpen]);

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

  const MapCenterTracker = ({ popupOpen }: { popupOpen: boolean }) => {
    const map = useMapEvents({
      moveend: () => {
        if (popupOpen) {
          return;
        }

        const newBounds = map.getBounds();
        updateSearchBoundariesFromBounds(newBounds);
      },
    });

    useEffect(() => {
      if (!popupOpen) {
        updateSearchBoundariesFromBounds(map.getBounds());
      }
    }, [map, popupOpen]);

    return null;
  };

  const MapSection = (
    <Box sx={{ height: isMobile ? '40vh' : '90vh', width: '100%', position: 'relative' }}>
      <MapContainer center={initialPosition} zoom={13} scrollWheelZoom={true} style={{ height: '100%', width: '100%' }}>
        <TileLayer attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors' url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
        <MarkerClusterGroup chunkedLoading>
          {locations
            .filter((item) => !!item.physicalAddress && !!item.physicalAddress.latitude && !!item.physicalAddress.longitude)
            .map((item) => (
              <Marker
                key={item.id}
                position={[item.physicalAddress!.latitude!, item.physicalAddress!.longitude!]}
                eventHandlers={{
                  click: () => setActivePopupId(item.id),
                  popupopen: () => setActivePopupId(item.id),
                  popupclose: () => {
                    setActivePopupId((current) => (current === item.id ? null : current));
                  },
                }}
              >
                <Popup autoPan={!isMobile} autoPanPadding={[24, 24]}>
                  <MarketplaceLocationPopupCard key={item.id} locationDetailsRelay={item} onReloadRequired={onReloadRequired} />
                </Popup>
              </Marker>
            ))}
        </MarkerClusterGroup>

        <MapInitBoundsTracker popupOpen={!!activePopupId} />
        <MapCenterTracker popupOpen={!!activePopupId} />
        {!centerSet && <MapUpdater center={initialPosition} />}
      </MapContainer>
    </Box>
  );

  return (
    <StackColumn sx={{ p: defaultPadding }}>
      {isMobile ? (
        <>
          {MapSection}
          {locations.map((item) => (
            <Box key={item.id} mb={2}>
              <MarketplaceLocationCard locationDetailsRelay={item} onReloadRequired={onReloadRequired} />
            </Box>
          ))}
        </>
      ) : (
        <GridContainer>
          <Grid size={{ xs: 12, md: 8 }}>
            <GridContainer sx={{ alignItems: 'flex-start' }} spacing={1}>
              {locations.map((item) => (
                <Grid key={item.id}>
                  <MarketplaceLocationCard locationDetailsRelay={item} onReloadRequired={onReloadRequired} />
                </Grid>
              ))}
            </GridContainer>
          </Grid>
          <Grid size={{ xs: 12, md: 4 }}>{MapSection}</Grid>
        </GridContainer>
      )}
    </StackColumn>
  );
};

export default memo(MarketplaceLocations);

import { GridContainer, StackColumn } from '@/components/commons';
import { getRootLink } from '@/components/links';
import { useIntegratedPlatrform } from '@/libs/providers';
import { defaultPadding } from '@/libs/theme';
import type { marketplaceLocations_locations_query$key } from '@/queries/__generated__/marketplaceLocations_locations_query.graphql';
import type { marketplaceLocations_locations_refetchableFragment } from '@/queries/__generated__/marketplaceLocations_locations_refetchableFragment.graphql';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import { LatLngBounds, LatLngTuple } from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { useRouter, useSearchParams } from 'next/navigation';
import { IPinfoWrapper } from 'node-ipinfo';
import { memo, startTransition, useCallback, useEffect, useMemo, useState } from 'react';
import { useMap, useMapEvents } from 'react-leaflet';
import { graphql, useRefetchableFragment } from 'react-relay';
import MarketplaceLocationCard from './marketplace-location-card';
import MarketplaceLocationPopupCard from './marketplace-location-popup-card';

let L: typeof import('leaflet');
let MapContainer: typeof import('react-leaflet').MapContainer;
let Marker: typeof import('react-leaflet').Marker;
let TileLayer: typeof import('react-leaflet').TileLayer;
let Popup: typeof import('react-leaflet').Popup;

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

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const searchParams = useSearchParams();
  const latitude = searchParams.get('latitude');
  const longitude = searchParams.get('longitude');
  const zoom = searchParams.get('zoom');
  const [dynamicLoadReady, setDynamicLoadReady] = useState(false);
  const locations = useMemo(() => rootDataRefetchable.marketplaceLocations.edges.map((edge) => edge.node), [rootDataRefetchable.marketplaceLocations]);
  const [initialPosition, setInitialPosition] = useState<LatLngTuple>(latitude && longitude ? [parseFloat(latitude), parseFloat(longitude)] : [-36.8485, 174.7633]); // Auckland
  const initialZoom = useMemo(() => (zoom ? parseFloat(zoom) : 13), [zoom]);
  const [searchBoundaries, setSearchBoundaries] = useState<LatLngBounds | null>(null);
  const [centerSet, setCenterSet] = useState(false);

  useEffect(() => {
    (async () => {
      // core libraries
      const leaflet = await import('leaflet');
      const rl = await import('react-leaflet');

      L = leaflet;
      MapContainer = rl.MapContainer;
      Marker = rl.Marker;
      TileLayer = rl.TileLayer;
      Popup = rl.Popup;

      L.Icon.Default.mergeOptions({
        iconRetinaUrl: '/leaflet/images/marker-icon-2x.png',
        iconUrl: '/leaflet/images/marker-icon.png',
        shadowUrl: '/leaflet/images/marker-shadow.png',
      });

      if (!latitude || !longitude) {
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
      }

      setDynamicLoadReady(true);
    })();
  }, [latitude, longitude]);

  const handleRefetch = useCallback(
    (searchBoundaries: LatLngBounds | null) => {
      startTransition(() => {
        refetch(
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

  const MapCenterTracker = () => {
    const map = useMapEvents({
      moveend: () => {
        const newCenter = map.getCenter();
        router.push(getRootLink(integratedPlatrform, { latitude: newCenter.lat, longitude: newCenter.lng, zoom: map.getZoom() }));

        const newBounds = map.getBounds();
        if (!searchBoundaries) {
          setSearchBoundaries(map.getBounds());

          return;
        }

        const oldSouthWest = searchBoundaries.getSouthWest();
        const oldNorthEast = searchBoundaries.getNorthEast();
        const newSouthWest = newBounds.getSouthWest();
        const newNorthEast = newBounds.getNorthEast();

        if (oldSouthWest.lat !== newSouthWest.lat || oldSouthWest.lng !== newSouthWest.lng || oldNorthEast.lat !== newNorthEast.lat || oldNorthEast.lng !== newNorthEast.lng) {
          setSearchBoundaries(map.getBounds());

          return;
        }
      },
    });

    return null;
  };

  return (
    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
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

        <Grid size={{ xs: 12, md: 4 }}>
          <Box sx={{ height: '90vh', width: '100%' }}>
            <MapContainer center={initialPosition} zoom={initialZoom} scrollWheelZoom={true} style={{ height: '100%', width: '100%' }}>
              <TileLayer
                attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
              />
              {locations
                .filter((item) => !!item.physicalAddress && !!item.physicalAddress.latitude && !!item.physicalAddress.longitude)
                .map((item) => {
                  return (
                    <Marker key={item.id} position={[item.physicalAddress!.latitude!, item.physicalAddress!.longitude!]}>
                      <Popup>
                        <MarketplaceLocationPopupCard key={item.id} locationDetailsRelay={item} onReloadRequired={onReloadRequired} />
                      </Popup>
                    </Marker>
                  );
                })}

              <MapInitBoundsTracker />
              <MapCenterTracker />
              {!centerSet && <MapUpdater center={initialPosition} />}
            </MapContainer>
          </Box>
        </Grid>
      </GridContainer>
    </StackColumn>
  );
};

export default memo(MarketplaceLocations);

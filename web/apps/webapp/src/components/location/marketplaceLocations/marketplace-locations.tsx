import { PushToRight, StackColumn } from '@/components/commons';
import StackRow from '@/components/commons/stack-row';
import { defaultPadding } from '@/libs/theme';
import type { marketplaceLocations_locations_query$key } from '@/queries/__generated__/marketplaceLocations_locations_query.graphql';
import type { marketplaceLocations_locations_refetchableFragment } from '@/queries/__generated__/marketplaceLocations_locations_refetchableFragment.graphql';
import Box from '@mui/material/Box';
import { LatLngBounds, LatLngTuple } from 'leaflet';
import markerIcon2x from 'leaflet/dist/images/marker-icon-2x.png';
import markerIcon from 'leaflet/dist/images/marker-icon.png';
import markerShadow from 'leaflet/dist/images/marker-shadow.png';
import 'leaflet/dist/leaflet.css';
import { IPinfoWrapper } from 'node-ipinfo';
import { memo, startTransition, useCallback, useEffect, useMemo, useState } from 'react';
import { useMap, useMapEvents } from 'react-leaflet';
import { graphql, useRefetchableFragment } from 'react-relay';
import MarketplaceLocationCard from './marketplace-location-card';

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
        marketplaceLocations(first: $count, after: $cursor, where: { types: [MARKETPLACE], searchBoundaries: $searchBoundaries }, orderBy: $locationsSortingValues)
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
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [dynamicLoadReady, setDynamicLoadReady] = useState(false);
  const locations = useMemo(() => rootDataRefetchable.marketplaceLocations.edges.map((edge) => edge.node), [rootDataRefetchable.marketplaceLocations]);
  const [initialPosition, setInitialPosition] = useState<LatLngTuple>([-36.8485, 174.7633]); // Auckland
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
        iconRetinaUrl: markerIcon2x,
        iconUrl: markerIcon,
        shadowUrl: markerShadow,
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
      map.flyTo(center, map.getZoom());

      setCenterSet(true);
    }, [center, map]);

    return null;
  };

  const MapCenterTracker = () => {
    const map = useMapEvents({
      moveend: () => {
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
      <StackRow sx={{ alignItems: 'flex-start' }}>
        {locations.map((item) => (
          <MarketplaceLocationCard key={item.id} locationDetailsRelay={item} onReloadRequired={onReloadRequired} />
        ))}
        <PushToRight />
        <Box sx={{ height: '80vh', width: '30%' }}>
          <MapContainer center={initialPosition} zoom={13} scrollWheelZoom={true} style={{ height: '100%', width: '100%' }}>
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
                      <MarketplaceLocationCard key={item.id} locationDetailsRelay={item} onReloadRequired={onReloadRequired} />
                    </Popup>
                  </Marker>
                );
              })}

            <MapInitBoundsTracker />
            <MapCenterTracker />
            {!centerSet && <MapUpdater center={initialPosition} />}
          </MapContainer>
        </Box>
      </StackRow>
    </StackColumn>
  );
};

export default memo(MarketplaceLocations);

import { PushToRight, StackColumn } from '@/components/commons';
import StackRow from '@/components/commons/stack-row';
import { defaultPadding } from '@/libs/theme';
import type { marketplaceLocation_query$key } from '@/queries/__generated__/marketplaceLocation_query.graphql';
import Box from '@mui/material/Box';
import { LatLngTuple } from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { memo, useEffect, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

let L: typeof import('leaflet');
let MapContainer: typeof import('react-leaflet').MapContainer;
let Marker: typeof import('react-leaflet').Marker;
let TileLayer: typeof import('react-leaflet').TileLayer;

type Props = {
  rootDataRelay: marketplaceLocation_query$key;
};

const MarketplaceLocation = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<marketplaceLocation_query$key>(
    graphql`
      fragment marketplaceLocation_query on Query {
        location(id: $locationId) {
          id
          name
          about
          timezone
          extraMetadata {
            contactDetails {
              contactPeople
              contactEmails
              contactPhones
            }
            areaRange {
              fromInSqm
              toInSqm
            }
            peopleCapacity {
              from
              to
            }
            website
            relatedImageLinks
            relatedVideoLinks
            otherLinks
          }
          primaryFeatureImage {
            original {
              url
              height
              width
            }
          }
          physicalAddress {
            id
            osmType
            osmId
            placeId
            longitude
            latitude
            formattedAddress
            addressLine1
            addressLine2
            suburb
            city
            province
            zipcode
            country
            countryCode
          }
          openingHours {
            weekOpeningHours {
              monday {
                closed
                openAllDay
                from
                until
              }
              tuesday {
                closed
                openAllDay
                from
                until
              }
              wednesday {
                closed
                openAllDay
                from
                until
              }
              thursday {
                closed
                openAllDay
                from
                until
              }
              friday {
                closed
                openAllDay
                from
                until
              }
              saturday {
                closed
                openAllDay
                from
                until
              }
              sunday {
                closed
                openAllDay
                from
                until
              }
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [dynamicLoadReady, setDynamicLoadReady] = useState(false);
  const locaitonExists = rootData.location?.physicalAddress?.longitude && rootData.location?.physicalAddress?.latitude;
  const initialPosition: LatLngTuple = locaitonExists ? [rootData.location?.physicalAddress?.latitude, rootData.location?.physicalAddress?.longitude] : [-36.8485, 174.7633];

  useEffect(() => {
    (async () => {
      // core libraries
      const leaflet = await import('leaflet');
      const rl = await import('react-leaflet');

      L = leaflet;
      MapContainer = rl.MapContainer;
      Marker = rl.Marker;
      TileLayer = rl.TileLayer;

      L.Icon.Default.mergeOptions({
        iconRetinaUrl: '/leaflet/images/marker-icon-2x.png',
        iconUrl: '/leaflet/images/marker-icon.png',
        shadowUrl: '/leaflet/images/marker-shadow.png',
      });

      setDynamicLoadReady(true);
    })();
  }, []);

  if (!dynamicLoadReady) {
    return <></>;
  }

  return (
    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
      <StackRow sx={{ alignItems: 'flex-start' }}>
        <PushToRight />
        <Box sx={{ height: '80vh', width: '30%' }}>
          <MapContainer center={initialPosition} zoom={13} scrollWheelZoom={true} style={{ height: '100%', width: '100%' }}>
            <TileLayer
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
            {locaitonExists && <Marker position={initialPosition} />}
          </MapContainer>
        </Box>
      </StackRow>
    </StackColumn>
  );
};

export default memo(MarketplaceLocation);

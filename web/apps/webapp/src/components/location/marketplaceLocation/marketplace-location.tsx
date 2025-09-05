import { BodyIconTypography, CaptionIconTypography, GridContainer, SmallHeadingIconTypography, SmallIconTypography, StackColumn } from '@/components/commons';
import { defaultPadding } from '@/libs/theme';
import { stringCollectionToString } from '@/libs/utils';
import type { marketplaceLocation_query$key } from '@/queries/__generated__/marketplaceLocation_query.graphql';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import { LatLngTuple } from 'leaflet';
import 'leaflet/dist/leaflet.css';
import Image from 'next/image';
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
            longitude
            latitude
            multilinesFormattedAddress
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
  const locationDetails = rootData.location;
  const locaitonExists = locationDetails?.physicalAddress?.longitude && locationDetails.physicalAddress?.latitude;
  const initialPosition: LatLngTuple = locaitonExists ? [locationDetails.physicalAddress?.latitude, locationDetails.physicalAddress?.longitude] : [-36.8485, 174.7633];

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

  if (!locationDetails) {
    return <></>;
  }

  const image = locationDetails.primaryFeatureImage?.original;

  return (
    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
      <StackColumn>
        {image && <Image src={image.url} height={image.height!} width={image.width!} alt="" />}
        <GridContainer sx={{ mt: 2 }}>
          <Grid size={{ xs: 12, md: 6 }}>
            <StackColumn>
              <SmallHeadingIconTypography label={locationDetails.name} />
              <SmallIconTypography label={locationDetails.about} sx={{ whiteSpace: 'pre-line' }} />
            </StackColumn>
          </Grid>

          <Grid size={{ xs: 12, md: 6 }}>
            {locationDetails.extraMetadata?.contactDetails?.contactPeople && (
              <StackColumn>
                <CaptionIconTypography label={'Contact People'} />
                <BodyIconTypography
                  label={stringCollectionToString(locationDetails.extraMetadata.contactDetails.contactPeople)}
                  sx={{ whiteSpace: 'pre-line', paddingBottom: 2 }}
                />
              </StackColumn>
            )}
            {locationDetails.extraMetadata?.contactDetails?.contactPhones && (
              <StackColumn>
                <CaptionIconTypography label={'Phones'} />
                <BodyIconTypography
                  label={stringCollectionToString(locationDetails.extraMetadata.contactDetails.contactPhones)}
                  sx={{ whiteSpace: 'pre-line', paddingBottom: 2 }}
                />
              </StackColumn>
            )}
            {locationDetails.extraMetadata?.contactDetails?.contactEmails && (
              <StackColumn>
                <CaptionIconTypography label={'Emails'} />
                <BodyIconTypography
                  label={stringCollectionToString(locationDetails.extraMetadata.contactDetails.contactEmails)}
                  sx={{ whiteSpace: 'pre-line', paddingBottom: 2 }}
                />
              </StackColumn>
            )}
            {locationDetails.physicalAddress && (
              <StackColumn>
                <CaptionIconTypography label={'Address'} />
                <BodyIconTypography label={locationDetails.physicalAddress.multilinesFormattedAddress} sx={{ whiteSpace: 'pre-line', paddingBottom: 2 }} />
              </StackColumn>
            )}
          </Grid>
        </GridContainer>
      </StackColumn>
      <StackColumn sx={{ display: 'flex', alignItems: 'center' }}>
        <Box sx={{ height: '20vh', width: { xs: '100%', sm: '50%' } }}>
          <MapContainer center={initialPosition} zoom={13} scrollWheelZoom={true} style={{ height: '100%', width: '100%' }}>
            <TileLayer
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
            {locaitonExists && <Marker position={initialPosition} />}
          </MapContainer>
        </Box>
      </StackColumn>
    </StackColumn>
  );
};

export default memo(MarketplaceLocation);

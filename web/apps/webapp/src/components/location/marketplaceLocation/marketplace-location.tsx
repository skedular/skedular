import { ResponsiveImageCarousel } from '@/components/carousel';
import { CaptionIconTypography, GridContainer, LeadIconTypography, SmallHeadingIconTypography, SmallIconTypography, StackColumn } from '@/components/commons';
import { AreaIcon, PersonIcon } from '@/components/icons';
import { defaultPadding } from '@/libs/theme';
import { stringCollectionToString, toOpeningHoursFromTime } from '@/libs/utils';
import type { marketplaceLocation_query$key } from '@/queries/__generated__/marketplaceLocation_query.graphql';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import Link from '@mui/material/Link';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import type { LatLngTuple } from 'leaflet';
import 'leaflet/dist/leaflet.css';
import NextLink from 'next/link';
import { memo, useEffect, useMemo, useState } from 'react';
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
          featureImages {
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

  const theme = useTheme();
  const isMdUp = useMediaQuery(theme.breakpoints.up('md'));
  const [dynamicLoadReady, setDynamicLoadReady] = useState(false);
  const locationDetails = rootData.location;
  const capacity = useMemo(() => {
    if (!locationDetails?.extraMetadata?.peopleCapacity) {
      return '';
    }

    if (locationDetails.extraMetadata?.peopleCapacity.from === locationDetails.extraMetadata?.peopleCapacity.to) {
      return `${locationDetails.extraMetadata?.peopleCapacity.from} People`;
    } else {
      return `${locationDetails.extraMetadata?.peopleCapacity.from} - ${locationDetails.extraMetadata?.peopleCapacity.to} People`;
    }
  }, [locationDetails?.extraMetadata?.peopleCapacity]);

  const areaSize = useMemo(() => {
    if (!locationDetails?.extraMetadata?.areaRange) {
      return '';
    }

    if (locationDetails.extraMetadata?.areaRange.fromInSqm === locationDetails.extraMetadata?.areaRange.toInSqm) {
      return `${locationDetails.extraMetadata?.areaRange.fromInSqm} m2`;
    } else {
      return `${locationDetails.extraMetadata?.areaRange.fromInSqm} - ${locationDetails.extraMetadata?.areaRange.toInSqm} m2`;
    }
  }, [locationDetails?.extraMetadata?.areaRange]);
  const locaitonExists = locationDetails?.physicalAddress?.longitude && locationDetails?.physicalAddress?.latitude;
  const initialPosition: LatLngTuple = locaitonExists
    ? [locationDetails?.physicalAddress?.latitude as number, locationDetails?.physicalAddress?.longitude as number]
    : [-36.8485, 174.7633];

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

  const openingHours = locationDetails?.openingHours;
  const extraMetadata = locationDetails?.extraMetadata;
  const featureImages = useMemo(
    () => [
      ...(locationDetails?.featureImages?.filter((item) => !!item.original?.url).map((item) => item.original!) ?? []),
      ...(extraMetadata?.relatedImageLinks?.filter(Boolean).map((url) => ({ url, height: 200, width: 400 })) ?? []),
    ],
    [extraMetadata?.relatedImageLinks, locationDetails?.featureImages],
  );
  const toOpeningHours = ({ closed, from, openAllDay, until }: { closed: boolean; from: string | null | undefined; openAllDay: boolean; until: string | null | undefined }) => {
    if (closed) {
      return 'Closed';
    }

    if (openAllDay) {
      return 'Open All Day';
    }

    return `${toOpeningHoursFromTime(from)?.format('hh:mm a')} - ${toOpeningHoursFromTime(until)?.format('hh:mm a')}`;
  };

  if (!dynamicLoadReady || !locationDetails || !openingHours) {
    return null;
  }

  const mapBlock = (
    <Box sx={{ height: '20vh', width: '100%' }}>
      <MapContainer center={initialPosition} zoom={13} scrollWheelZoom={true} style={{ height: '100%', width: '100%' }}>
        <TileLayer attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors' url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
        {locaitonExists && <Marker position={initialPosition} />}
      </MapContainer>
    </Box>
  );

  return (
    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
      <ResponsiveImageCarousel images={featureImages} />
      <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center' }}>
        <GridContainer sx={{ mt: 2, maxWidth: 1400, width: '100%' }} spacing={{ xs: 1, md: 3 }}>
          <Grid size={{ xs: 12, md: 9 }}>
            <StackColumn>
              <SmallHeadingIconTypography label={locationDetails.name} />
              <SmallIconTypography label={locationDetails.about} sx={{ whiteSpace: 'pre-line' }} />
              <LeadIconTypography label={'Opening Hours'} />
              <SmallIconTypography label={`Monday: ${toOpeningHours(openingHours.weekOpeningHours.monday)}`} />
              <SmallIconTypography label={`Tuesday: ${toOpeningHours(openingHours.weekOpeningHours.tuesday)}`} />
              <SmallIconTypography label={`Wednesday: ${toOpeningHours(openingHours.weekOpeningHours.wednesday)}`} />
              <SmallIconTypography label={`Thursday: ${toOpeningHours(openingHours.weekOpeningHours.thursday)}`} />
              <SmallIconTypography label={`Friday: ${toOpeningHours(openingHours.weekOpeningHours.friday)}`} />
              <SmallIconTypography label={`Saturday: ${toOpeningHours(openingHours.weekOpeningHours.saturday)}`} />
              <SmallIconTypography label={`Sunday: ${toOpeningHours(openingHours.weekOpeningHours.sunday)}`} />
              {isMdUp && <StackColumn sx={{ display: 'flex', alignItems: 'stretch' }}>{mapBlock}</StackColumn>}
            </StackColumn>
          </Grid>

          <Grid size={{ xs: 12, md: 3 }}>
            {extraMetadata?.contactDetails?.contactPeople && extraMetadata.contactDetails.contactPeople.length > 0 && (
              <>
                <CaptionIconTypography label={'Contact People'} />
                <SmallIconTypography label={stringCollectionToString(extraMetadata.contactDetails.contactPeople)} sx={{ whiteSpace: 'pre-line', paddingBottom: 2 }} />
              </>
            )}

            {extraMetadata?.contactDetails?.contactPhones && extraMetadata.contactDetails.contactPhones.length > 0 && (
              <>
                <CaptionIconTypography label={'Phones'} />
                <SmallIconTypography label={stringCollectionToString(extraMetadata.contactDetails.contactPhones)} sx={{ whiteSpace: 'pre-line', paddingBottom: 2 }} />
              </>
            )}

            {extraMetadata?.contactDetails?.contactEmails && extraMetadata.contactDetails.contactEmails.length > 0 && (
              <>
                <CaptionIconTypography label={'Emails'} />
                <SmallIconTypography label={stringCollectionToString(extraMetadata.contactDetails.contactEmails)} sx={{ whiteSpace: 'pre-line', paddingBottom: 2 }} />
              </>
            )}

            {extraMetadata?.website && (
              <>
                <CaptionIconTypography label={'Website'} />
                <Link component={NextLink} href={extraMetadata.website} target="_blank" rel="noopener noreferrer">
                  <SmallIconTypography label={extraMetadata.website} sx={{ paddingBottom: 2 }} />
                </Link>
              </>
            )}

            {locationDetails.physicalAddress && (
              <>
                <CaptionIconTypography label={'Address'} />
                <SmallIconTypography label={locationDetails.physicalAddress.multilinesFormattedAddress} sx={{ whiteSpace: 'pre-line', paddingBottom: 2 }} />
              </>
            )}

            {areaSize && (
              <>
                <CaptionIconTypography label={'Total Area'} />
                <SmallIconTypography label={areaSize} startElement={<AreaIcon fontSize="small" />} sx={{ paddingBottom: 2 }} />
              </>
            )}

            {capacity && (
              <>
                <CaptionIconTypography label={'Capacity'} />
                <SmallIconTypography label={capacity} startElement={<PersonIcon fontSize="small" />} sx={{ paddingBottom: 2 }} />
              </>
            )}
          </Grid>
        </GridContainer>
      </Box>
      {!isMdUp && mapBlock}
    </StackColumn>
  );
};

export default memo(MarketplaceLocation);

import { AreaIcon, ArrowLeftIcon, CheckIcon, ContactEmailIcon, ContactPhoneIcon, LocationIcon, OpeningHoursIcon, PersonIcon } from '@/components/icons';
import { MarketplaceProductCard } from '@/components/marketplaceProductCard';
import { useKnownParams } from '@/libs/providers';
import type { marketplaceLocation_query$key } from '@/queries/__generated__/marketplaceLocation_query.graphql';
import '@/styles/leaflet/leaflet.css';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import Grid from '@mui/material/Grid';
import Link from '@mui/material/Link';
import Paper from '@mui/material/Paper';
import Typography from '@mui/material/Typography';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import type { LatLngTuple } from 'leaflet';
import { useRouter } from 'next/navigation';
import { memo, type ReactNode, useEffect, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

let L: typeof import('leaflet');
let MapContainer: typeof import('react-leaflet').MapContainer;
let Marker: typeof import('react-leaflet').Marker;
let TileLayer: typeof import('react-leaflet').TileLayer;

type Props = {
  rootDataRelay: marketplaceLocation_query$key;
};

type PricingRow = {
  amountLabel: string;
  cadence: string;
  cadenceLabel: string;
  id: string;
  taxLabel: string;
  title: string;
};

type OpeningHoursDay = {
  closed: boolean;
  from: string | null | undefined;
  openAllDay: boolean;
  until: string | null | undefined;
};

const sectionCardSx = {
  borderRadius: 5,
  border: 1,
  borderColor: 'divider',
  boxShadow: 'none',
};

const formatOpeningHours = ({ closed, from, openAllDay, until }: OpeningHoursDay) => {
  if (closed) {
    return 'Closed';
  }

  if (openAllDay) {
    return 'Open all day';
  }

  if (!from || !until) {
    return 'Hours unavailable';
  }

  const formatTime = (value: string) => {
    const [hoursRaw = '0', minutesRaw = '00'] = value.split(':');
    const hours = Number(hoursRaw);
    const minutes = Number(minutesRaw);

    if (Number.isNaN(hours) || Number.isNaN(minutes)) {
      return value;
    }

    const suffix = hours >= 12 ? 'pm' : 'am';
    const normalizedHour = hours % 12 || 12;

    return `${normalizedHour}:${`${minutes}`.padStart(2, '0')} ${suffix}`;
  };

  return `${formatTime(from)} - ${formatTime(until)}`;
};

const getFirstPopulatedValue = (values: readonly string[] | null | undefined) => values?.find((value) => value.trim().length > 0) ?? null;

const InfoRow = ({ icon, label, children }: { icon: ReactNode; label: string; children: ReactNode }) => (
  <Box sx={{ display: 'flex', gap: 1.75, alignItems: 'flex-start' }}>
    <Box sx={{ mt: 0.25, color: 'text.secondary', display: 'flex' }}>{icon}</Box>
    <Box sx={{ minWidth: 0 }}>
      <Typography sx={{ fontSize: '0.75rem', fontWeight: 700, letterSpacing: '0.08em', textTransform: 'uppercase', color: 'text.secondary', mb: 0.75 }}>{label}</Typography>
      {children}
    </Box>
  </Box>
);

const MarketplaceLocation = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<marketplaceLocation_query$key>(
    graphql`
      fragment marketplaceLocation_query on Query {
        productPricingCadences {
          type
          name
        }
        currencies {
          type
          name
        }
        location(id: $locationId) {
          id
          name
          organization {
            customDomain
          }
          listingMetadata {
            about
            title
            subTitle
            includedFeatures
          }
          timezone
          amenities {
            id
            name
          }
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
          products {
            id
            listingMetadata {
              title
              subTitle
            }
            featureImages {
              original {
                url
              }
            }
            currency {
              type
            }
            pricingOptions {
              id
              index
              listingMetadata {
                title
              }
              purchaseCadence
              price
              isTaxInclusive
              supportsSubscriptionAutoRenewal
            }
            amenities {
              id
              name
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const router = useRouter();
  const theme = useTheme();
  const isMdUp = useMediaQuery(theme.breakpoints.up('md'));
  const { organizationCustomDomain } = useKnownParams();
  const [dynamicLoadReady, setDynamicLoadReady] = useState(false);
  const [selectedHeroImageUrl, setSelectedHeroImageUrl] = useState<string>('');
  const locationDetails = rootData.location;

  const capacity = useMemo(() => {
    if (!locationDetails?.extraMetadata?.peopleCapacity) {
      return null;
    }

    const { from, to } = locationDetails.extraMetadata.peopleCapacity;

    return from === to ? `${from} people` : `${from} - ${to} people`;
  }, [locationDetails?.extraMetadata?.peopleCapacity]);

  const areaSize = useMemo(() => {
    if (!locationDetails?.extraMetadata?.areaRange) {
      return null;
    }

    const { fromInSqm, toInSqm } = locationDetails.extraMetadata.areaRange;

    return fromInSqm === toInSqm ? `${fromInSqm} m2` : `${fromInSqm} - ${toInSqm} m2`;
  }, [locationDetails?.extraMetadata?.areaRange]);

  const locationExists = Boolean(locationDetails?.physicalAddress?.longitude && locationDetails?.physicalAddress?.latitude);
  const initialPosition: LatLngTuple = locationExists
    ? [locationDetails?.physicalAddress?.latitude as number, locationDetails?.physicalAddress?.longitude as number]
    : [-36.8485, 174.7633];

  useEffect(() => {
    (async () => {
      const leaflet = await import('leaflet');
      const reactLeaflet = await import('react-leaflet');

      L = leaflet;
      MapContainer = reactLeaflet.MapContainer;
      Marker = reactLeaflet.Marker;
      TileLayer = reactLeaflet.TileLayer;

      L.Icon.Default.mergeOptions({
        iconRetinaUrl: '/leaflet/images/marker-icon-2x.png',
        iconUrl: '/leaflet/images/marker-icon.png',
        shadowUrl: '/leaflet/images/marker-shadow.png',
      });

      setDynamicLoadReady(true);
    })();
  }, []);

  const openingHours = locationDetails?.openingHours?.weekOpeningHours;
  const extraMetadata = locationDetails?.extraMetadata;
  const heroImages = useMemo(
    () => [
      ...(locationDetails?.featureImages?.filter((item) => !!item.original?.url).map((item) => item.original!) ?? []),
      ...(extraMetadata?.relatedImageLinks?.filter(Boolean).map((url) => ({ url, height: 1200, width: 1800 })) ?? []),
    ],
    [extraMetadata?.relatedImageLinks, locationDetails?.featureImages],
  );

  const heroImage = selectedHeroImageUrl || heroImages[0]?.url || '';
  const includedFeatures = useMemo(
    () => (locationDetails?.listingMetadata?.includedFeatures ?? []).filter((item): item is string => Boolean(item?.trim())),
    [locationDetails?.listingMetadata?.includedFeatures],
  );
  const amenities = (locationDetails?.amenities ?? []).filter((amenity) => !!amenity.name);
  const primaryPhone = getFirstPopulatedValue(extraMetadata?.contactDetails?.contactPhones);
  const primaryEmail = getFirstPopulatedValue(extraMetadata?.contactDetails?.contactEmails);

  const products = useMemo<
    {
      amenities: readonly { id: string; name: string }[];
      id: string;
      imageUrl: string;
      pricingRows: PricingRow[];
      subTitle: string;
      title: string;
    }[]
  >(() => {
    if (!locationDetails) {
      return [];
    }

    return [...locationDetails.products].map((product) => {
      const currencyLabel = product.currency?.type ? (rootData.currencies.find((item) => item.type === product.currency?.type)?.name ?? product.currency.type) : null;
      const pricingRows = [...product.pricingOptions]
        .sort((left, right) => left.index - right.index)
        .map((option) => ({
          id: option.id,
          title: option.listingMetadata.title ?? '',
          cadence: option.purchaseCadence,
          cadenceLabel: rootData.productPricingCadences.find((cadence) => cadence.type === option.purchaseCadence)?.name ?? option.purchaseCadence,
          amountLabel: currencyLabel ? `${currencyLabel} ${option.price}` : `${option.price}`,
          taxLabel: option.isTaxInclusive ? 'incl. tax' : 'excl. tax',
        }));

      return {
        amenities: product.amenities,
        id: product.id,
        imageUrl: product.featureImages[0]?.original?.url ?? heroImage,
        title: product.listingMetadata.title ?? 'Untitled product',
        subTitle: product.listingMetadata.subTitle ?? '',
        pricingRows,
      };
    });
  }, [heroImage, locationDetails, rootData.currencies, rootData.productPricingCadences]);

  const locationDays = openingHours
    ? [
        ['Monday', openingHours.monday] as const,
        ['Tuesday', openingHours.tuesday] as const,
        ['Wednesday', openingHours.wednesday] as const,
        ['Thursday', openingHours.thursday] as const,
        ['Friday', openingHours.friday] as const,
        ['Saturday', openingHours.saturday] as const,
        ['Sunday', openingHours.sunday] as const,
      ]
    : [];

  useEffect(() => {
    if (!heroImages.length) {
      setSelectedHeroImageUrl('');
      return;
    }

    if (!selectedHeroImageUrl || !heroImages.some((image) => image.url === selectedHeroImageUrl)) {
      setSelectedHeroImageUrl(heroImages[0]?.url ?? '');
    }
  }, [heroImages, selectedHeroImageUrl]);

  if (!dynamicLoadReady || !locationDetails || !openingHours) {
    return null;
  }

  const effectiveOrganizationCustomDomain = organizationCustomDomain || locationDetails.organization?.customDomain || '';

  return (
    <Box sx={{ bgcolor: 'background.default', minHeight: '100vh', pb: 8 }}>
      <Container maxWidth="lg" sx={{ pt: { xs: 3, md: 4 } }}>
        <Button variant="text" onClick={() => router.back()} sx={{ textTransform: 'none', px: 0, mb: 3 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
            <ArrowLeftIcon fontSize="small" />
            <Typography sx={{ color: 'text.primary' }}>Back</Typography>
          </Box>
        </Button>

        <Paper sx={{ ...sectionCardSx, overflow: 'hidden', mb: 4 }}>
          {heroImage ? (
            <Box
              component="img"
              src={heroImage}
              alt={locationDetails.listingMetadata.title ?? locationDetails.name}
              sx={{ width: '100%', height: { xs: 260, md: 420 }, objectFit: 'cover' }}
            />
          ) : (
            <Box sx={{ width: '100%', height: { xs: 260, md: 420 }, bgcolor: 'action.hover' }} />
          )}
          <Box sx={{ p: { xs: 3, md: 5 } }}>
            {heroImages.length > 1 ? (
              <Box
                sx={{
                  display: 'grid',
                  gridTemplateColumns: {
                    xs: `repeat(${Math.min(heroImages.length, 2)}, minmax(0, 1fr))`,
                    sm: `repeat(${Math.min(heroImages.length, 4)}, minmax(0, 1fr))`,
                  },
                  gap: 1.5,
                  mb: 3.5,
                }}
              >
                {heroImages.map((image, index) => {
                  const isSelected = image.url === heroImage;

                  return (
                    <Box
                      key={image.url}
                      component="button"
                      type="button"
                      onClick={() => setSelectedHeroImageUrl(image.url)}
                      sx={{
                        p: 0,
                        border: 0,
                        bgcolor: 'transparent',
                        cursor: 'pointer',
                        textAlign: 'left',
                        borderRadius: 3,
                        overflow: 'hidden',
                        outline: 'none',
                        boxShadow: isSelected ? `0 0 0 2px ${theme.palette.text.primary}` : `0 0 0 1px ${theme.palette.divider}`,
                        transition: 'box-shadow 120ms ease, transform 120ms ease',
                        '&:hover': {
                          transform: 'translateY(-1px)',
                        },
                        '&:focus-visible': {
                          boxShadow: `0 0 0 2px ${theme.palette.primary.main}`,
                        },
                      }}
                    >
                      <Box
                        component="img"
                        src={image.url}
                        alt={`${locationDetails.name} image ${index + 1}`}
                        sx={{ width: '100%', height: { xs: 88, sm: 110 }, objectFit: 'cover', display: 'block' }}
                      />
                    </Box>
                  );
                })}
              </Box>
            ) : null}

            <Typography sx={{ fontSize: '0.8rem', fontWeight: 700, letterSpacing: '0.1em', textTransform: 'uppercase', color: 'text.secondary', mb: 1.25 }}>
              {locationDetails.name}
            </Typography>
            <Typography sx={{ fontSize: { xs: '2rem', md: '3.1rem' }, fontWeight: 700, letterSpacing: '-0.04em', color: 'text.primary', mb: 1 }}>
              {locationDetails.listingMetadata.title || locationDetails.name}
            </Typography>
            {locationDetails.listingMetadata.subTitle ? (
              <Typography sx={{ fontSize: { xs: '1.05rem', md: '1.25rem' }, color: 'text.secondary', mb: 3 }}>{locationDetails.listingMetadata.subTitle}</Typography>
            ) : null}
            {locationDetails.listingMetadata.about ? (
              <Typography sx={{ fontSize: '1rem', lineHeight: 1.9, color: 'text.primary', whiteSpace: 'pre-line', maxWidth: 900 }}>
                {locationDetails.listingMetadata.about}
              </Typography>
            ) : null}
          </Box>
        </Paper>

        <Grid container spacing={3} sx={{ mb: 4 }}>
          <Grid size={{ xs: 12, md: 6 }}>
            <Paper sx={{ ...sectionCardSx, p: { xs: 3, md: 4 }, height: '100%' }}>
              <Typography sx={{ fontSize: '1.5rem', fontWeight: 700, color: 'text.primary', mb: 3 }}>Contact & Location</Typography>
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
                {locationDetails.physicalAddress?.multilinesFormattedAddress ? (
                  <InfoRow icon={<LocationIcon fontSize="small" />} label="Address">
                    <Typography sx={{ color: 'text.primary', lineHeight: 1.7, whiteSpace: 'pre-line' }}>{locationDetails.physicalAddress.multilinesFormattedAddress}</Typography>
                  </InfoRow>
                ) : null}

                {primaryPhone ? (
                  <InfoRow icon={<ContactPhoneIcon fontSize="small" />} label="Phone">
                    <Link href={`tel:${primaryPhone.trim().replace(/[^\d+]/g, '')}`} underline="hover" color="inherit">
                      <Typography sx={{ color: 'text.primary' }}>{primaryPhone}</Typography>
                    </Link>
                  </InfoRow>
                ) : null}

                {primaryEmail ? (
                  <InfoRow icon={<ContactEmailIcon fontSize="small" />} label="Email">
                    <Link href={`mailto:${primaryEmail}`} underline="hover" color="inherit">
                      <Typography sx={{ color: 'text.primary' }}>{primaryEmail}</Typography>
                    </Link>
                  </InfoRow>
                ) : null}

                {extraMetadata?.website ? (
                  <InfoRow icon={<CheckIcon fontSize="small" />} label="Website">
                    <Link href={extraMetadata.website} target="_blank" rel="noopener noreferrer" underline="hover" color="inherit">
                      <Typography sx={{ color: 'text.primary', overflowWrap: 'anywhere' }}>{extraMetadata.website}</Typography>
                    </Link>
                  </InfoRow>
                ) : null}

                {areaSize ? (
                  <InfoRow icon={<AreaIcon fontSize="small" />} label="Area">
                    <Typography sx={{ color: 'text.primary' }}>{areaSize}</Typography>
                  </InfoRow>
                ) : null}

                {capacity ? (
                  <InfoRow icon={<PersonIcon fontSize="small" />} label="Capacity">
                    <Typography sx={{ color: 'text.primary' }}>{capacity}</Typography>
                  </InfoRow>
                ) : null}
              </Box>

              {extraMetadata?.website ? (
                <Button
                  component="a"
                  href={extraMetadata.website}
                  target="_blank"
                  rel="noopener noreferrer"
                  fullWidth
                  variant="contained"
                  sx={{ textTransform: 'none', borderRadius: 3, mt: 4 }}
                >
                  Visit website
                </Button>
              ) : null}
            </Paper>
          </Grid>

          <Grid size={{ xs: 12, md: 6 }}>
            <Paper sx={{ ...sectionCardSx, p: { xs: 3, md: 4 }, height: '100%' }}>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mb: 3 }}>
                <OpeningHoursIcon />
                <Typography sx={{ fontSize: '1.5rem', fontWeight: 700, color: 'text.primary' }}>Opening Hours</Typography>
              </Box>

              <Box sx={{ display: 'flex', flexDirection: 'column' }}>
                {locationDays.map(([day, schedule], index) => (
                  <Box
                    key={day}
                    sx={{ display: 'flex', justifyContent: 'space-between', gap: 2, py: 1.4, borderBottom: index < locationDays.length - 1 ? 1 : 0, borderColor: 'divider' }}
                  >
                    <Typography sx={{ color: 'text.primary', fontWeight: 600 }}>{day}</Typography>
                    <Typography sx={{ color: 'text.secondary', textAlign: 'right' }}>{formatOpeningHours(schedule)}</Typography>
                  </Box>
                ))}
              </Box>
            </Paper>
          </Grid>
        </Grid>

        <Paper sx={{ ...sectionCardSx, p: { xs: 3, md: 5 }, mb: 4 }}>
          <Typography sx={{ fontSize: '1.75rem', fontWeight: 700, color: 'text.primary', mb: 4 }}>Amenities & Features</Typography>

          <Grid container spacing={3}>
            <Grid size={{ xs: 12, md: 6 }}>
              <Box sx={{ bgcolor: 'action.hover', borderRadius: 4, p: 3, height: '100%' }}>
                <Typography sx={{ fontSize: '1rem', fontWeight: 700, color: 'text.primary', mb: 2 }}>What&apos;s included</Typography>
                {includedFeatures.length > 0 ? (
                  <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
                    {includedFeatures.map((feature) => (
                      <Box key={feature} sx={{ display: 'flex', alignItems: 'flex-start', gap: 1.25 }}>
                        <CheckIcon sx={{ color: 'success.main', fontSize: 18, mt: '2px' }} />
                        <Typography sx={{ color: 'text.secondary' }}>{feature}</Typography>
                      </Box>
                    ))}
                  </Box>
                ) : (
                  <Typography sx={{ color: 'text.secondary' }}>No included features listed yet.</Typography>
                )}
              </Box>
            </Grid>

            <Grid size={{ xs: 12, md: 6 }}>
              <Box sx={{ bgcolor: 'action.hover', borderRadius: 4, p: 3, height: '100%' }}>
                <Typography sx={{ fontSize: '1rem', fontWeight: 700, color: 'text.primary', mb: 2 }}>Amenities</Typography>
                {amenities.length > 0 ? (
                  <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
                    {amenities.map((amenity) => (
                      <Box key={amenity.id} sx={{ display: 'flex', alignItems: 'flex-start', gap: 1.25 }}>
                        <CheckIcon sx={{ color: 'success.main', fontSize: 18, mt: '2px' }} />
                        <Typography sx={{ color: 'text.secondary' }}>{amenity.name}</Typography>
                      </Box>
                    ))}
                  </Box>
                ) : (
                  <Typography sx={{ color: 'text.secondary' }}>No amenities listed yet.</Typography>
                )}
              </Box>
            </Grid>
          </Grid>
        </Paper>

        <Paper sx={{ ...sectionCardSx, p: { xs: 3, md: 4 }, mb: 4 }}>
          <Typography sx={{ fontSize: '1.5rem', fontWeight: 700, color: 'text.primary', mb: 3 }}>Find Us Here</Typography>
          <Box sx={{ width: '100%', height: { xs: 320, md: 450 }, borderRadius: 4, overflow: 'hidden', border: 1, borderColor: 'divider' }}>
            <MapContainer center={initialPosition} zoom={13} scrollWheelZoom={isMdUp} style={{ height: '100%', width: '100%' }}>
              <TileLayer
                attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
              />
              {locationExists ? <Marker position={initialPosition} /> : null}
            </MapContainer>
          </Box>
        </Paper>

        <Paper sx={{ ...sectionCardSx, p: { xs: 3, md: 5 } }}>
          <Typography sx={{ fontSize: '1.75rem', fontWeight: 700, letterSpacing: '-0.03em', color: 'text.primary', mb: 1 }}>Available Workspaces</Typography>
          <Typography sx={{ fontSize: '1rem', color: 'text.secondary', mb: 4 }}>Book these products at this location</Typography>

          {products.length > 0 ? (
            <Grid container spacing={3}>
              {products.map((product) => (
                <Grid key={product.id} size={{ xs: 12, sm: 6, lg: 4 }}>
                  <MarketplaceProductCard
                    amenities={product.amenities}
                    imageUrl={product.imageUrl}
                    organizationCustomDomain={effectiveOrganizationCustomDomain}
                    pricingRows={product.pricingRows}
                    productId={product.id}
                    subTitle={product.subTitle}
                    title={product.title}
                  />
                </Grid>
              ))}
            </Grid>
          ) : (
            <Typography sx={{ color: 'text.secondary' }}>There are no bookable products listed for this location right now.</Typography>
          )}
        </Paper>
      </Container>
    </Box>
  );
};

export default memo(MarketplaceLocation);

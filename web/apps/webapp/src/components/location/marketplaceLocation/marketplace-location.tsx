import {
  AreaIcon,
  ArrowLeftIcon,
  CheckIcon,
  ContactEmailIcon,
  ContactPhoneIcon,
  DeskIcon,
  LocationIcon,
  OpeningHoursIcon,
  ParkingIcon,
  PersonIcon,
  RoomIcon,
} from '@/components/icons';
import {
  getMarketplaceLocationFloorPlansLink,
  getMarketplaceLocationLink,
  getMarketplaceProductBookingLink,
  getMarketplaceProductLink,
  getMarketplaceProductSubscribeLink,
} from '@/components/links';
import { MarketplaceProductCard } from '@/components/marketplaceProductCard';
import { isSubscriptionCadence } from '@/components/marketplaceProductSubscription/subscription-utils';
import { useIntegratedPlatrform, useKnownParams } from '@/libs/providers';
import { formatPriceForDisplay } from '@/libs/utils';
import type { marketplaceLocation_query$key } from '@/queries/__generated__/marketplaceLocation_query.graphql';
import type { marketplaceLocation_refetchableFragment } from '@/queries/__generated__/marketplaceLocation_refetchableFragment.graphql';
import '@/styles/leaflet/leaflet.css';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Container from '@mui/material/Container';
import Grid from '@mui/material/Grid';
import Link from '@mui/material/Link';
import Paper from '@mui/material/Paper';
import Typography from '@mui/material/Typography';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import type { LatLngTuple } from 'leaflet';
import { usePathname, useRouter } from 'next/navigation';
import { memo, type ReactNode, useEffect, useMemo, useState } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';

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

type FloorPlanProduct = {
  id: string;
  pricingRows: (PricingRow & { bookingLabel: string })[];
  productTagIds: string[];
  subTitle: string;
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

const getResourceTypeIcon = (resourceType: string | null | undefined, deskResourceType: string, roomResourceType: string, parkingResourceType: string) => {
  if (resourceType === deskResourceType) {
    return DeskIcon;
  }

  if (resourceType === roomResourceType) {
    return RoomIcon;
  }

  if (resourceType === parkingResourceType) {
    return ParkingIcon;
  }

  return DeskIcon;
};

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
  const [rootData, refetch] = useRefetchableFragment<marketplaceLocation_refetchableFragment, marketplaceLocation_query$key>(
    graphql`
      fragment marketplaceLocation_query on Query
      @argumentDefinitions(locationId: { type: "String!" }, selectedFloorPlanId: { type: "String" }, floorPlanSelected: { type: "Boolean", defaultValue: false })
      @refetchable(queryName: "marketplaceLocation_refetchableFragment") {
        productPricingCadences {
          type
          name
        }
        deskResourceType
        roomResourceType
        parkingResourceType
        floorPlans(where: { locationId: $locationId }, orderBy: [{ direction: ASCENDING, field: NAME }]) {
          edges {
            node {
              id
              name
              resourceCount
              image {
                original {
                  url
                  height
                  width
                }
              }
              resourcePositions {
                x
                y
                resource {
                  id
                }
              }
            }
          }
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
            productTags {
              id
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
          resources(where: { floorPlanId: $selectedFloorPlanId }, orderBy: [{ direction: ASCENDING, field: NAME }]) @include(if: $floorPlanSelected) {
            edges {
              node {
                id
                name
                inactive
                color
                productTags {
                  id
                  name
                  color
                }
                resourceType {
                  id
                  name
                  color
                  type
                }
              }
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const router = useRouter();
  const pathname = usePathname();
  const theme = useTheme();
  const isMdUp = useMediaQuery(theme.breakpoints.up('md'));
  const { integratedPlatrform } = useIntegratedPlatrform();
  const { isCustomDomain, organizationCustomDomain } = useKnownParams();
  const [dynamicLoadReady, setDynamicLoadReady] = useState(false);
  const [selectedHeroImageUrl, setSelectedHeroImageUrl] = useState<string>('');
  const [selectedFloorPlanId, setSelectedFloorPlanId] = useState<string>('');
  const [selectedResourceId, setSelectedResourceId] = useState<string>('');
  const locationDetails = rootData.location;
  const isFloorPlanPage = pathname.endsWith('/floorPlans');

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
          amountLabel: formatPriceForDisplay(currencyLabel, option.price, option.purchaseCadence),
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
  const floorPlans = useMemo(() => rootData.floorPlans.edges.map((edge) => edge.node).filter((item): item is NonNullable<typeof item> => !!item), [rootData.floorPlans.edges]);
  const effectiveSelectedFloorPlanId = useMemo(() => {
    if (floorPlans.some((item) => item.id === selectedFloorPlanId)) {
      return selectedFloorPlanId;
    }

    return floorPlans[0]?.id ?? '';
  }, [floorPlans, selectedFloorPlanId]);
  const selectedFloorPlan = useMemo(() => floorPlans.find((item) => item.id === effectiveSelectedFloorPlanId) ?? null, [effectiveSelectedFloorPlanId, floorPlans]);
  const floorPlanResources = useMemo(
    () => (locationDetails?.resources?.edges ?? []).map((edge) => edge.node).filter((item): item is NonNullable<typeof item> => !!item && !item.inactive),
    [locationDetails?.resources?.edges],
  );
  const effectiveSelectedResourceId = useMemo(() => {
    if (floorPlanResources.some((item) => item.id === selectedResourceId)) {
      return selectedResourceId;
    }

    const firstPlacedResourceId = selectedFloorPlan?.resourcePositions.find((position) => floorPlanResources.some((resource) => resource.id === position.resource.id))?.resource.id;
    return firstPlacedResourceId ?? floorPlanResources[0]?.id ?? '';
  }, [floorPlanResources, selectedFloorPlan?.resourcePositions, selectedResourceId]);
  const selectedResource = useMemo(() => floorPlanResources.find((item) => item.id === effectiveSelectedResourceId) ?? null, [effectiveSelectedResourceId, floorPlanResources]);
  const floorPlanProducts = useMemo<FloorPlanProduct[]>(() => {
    if (!locationDetails) {
      return [];
    }

    return locationDetails.products.map((product) => {
      const currencyLabel = product.currency?.type ? (rootData.currencies.find((item) => item.type === product.currency?.type)?.name ?? product.currency.type) : null;
      return {
        id: product.id,
        productTagIds: product.productTags.map((item) => item.id),
        title: product.listingMetadata.title ?? 'Untitled product',
        subTitle: product.listingMetadata.subTitle ?? '',
        pricingRows: [...product.pricingOptions]
          .sort((left, right) => left.index - right.index)
          .map((option) => ({
            id: option.id,
            title: option.listingMetadata.title ?? '',
            cadence: option.purchaseCadence,
            cadenceLabel: rootData.productPricingCadences.find((cadence) => cadence.type === option.purchaseCadence)?.name ?? option.purchaseCadence,
            amountLabel: formatPriceForDisplay(currencyLabel, option.price, option.purchaseCadence),
            taxLabel: option.isTaxInclusive ? 'incl. tax' : 'excl. tax',
            bookingLabel: isSubscriptionCadence(option.purchaseCadence) ? 'Choose plan' : 'Book this option',
          })),
      };
    });
  }, [locationDetails, rootData.currencies, rootData.productPricingCadences]);
  const matchedProductsForSelectedResource = useMemo(() => {
    if (!selectedResource) {
      return [];
    }

    const resourceProductTagIds = new Set(selectedResource.productTags.map((item) => item.id));
    return floorPlanProducts.filter((product) => product.productTagIds.length === 0 || product.productTagIds.some((tagId) => resourceProductTagIds.has(tagId)));
  }, [floorPlanProducts, selectedResource]);

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

  useEffect(() => {
    if (!isFloorPlanPage || !locationDetails?.id || floorPlans.length === 0) {
      return;
    }

    if (!selectedFloorPlanId) {
      setSelectedFloorPlanId(floorPlans[0].id);
      return;
    }

    refetch(
      {
        locationId: locationDetails.id,
        selectedFloorPlanId,
        floorPlanSelected: true,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [floorPlans, isFloorPlanPage, locationDetails?.id, refetch, selectedFloorPlanId]);

  if (!dynamicLoadReady || !locationDetails || !openingHours) {
    return null;
  }

  const effectiveOrganizationCustomDomain = organizationCustomDomain || locationDetails.organization?.customDomain || '';
  const locationLink = getMarketplaceLocationLink(integratedPlatrform, locationDetails.id);
  const floorPlansLink = getMarketplaceLocationFloorPlansLink(integratedPlatrform, locationDetails.id);
  const selectedFloorPlanImage = selectedFloorPlan?.image?.original;
  const selectedFloorPlanName = selectedFloorPlan?.name ?? '';
  const selectedFloorPlanResourcePositions = selectedFloorPlan?.resourcePositions ?? [];
  const selectedFloorPlanImageWidth = selectedFloorPlanImage?.width ?? 1;
  const selectedFloorPlanImageHeight = selectedFloorPlanImage?.height ?? 1;

  if (isFloorPlanPage) {
    return (
      <Box sx={{ bgcolor: 'background.default', minHeight: '100vh', pb: 8 }}>
        <Container maxWidth="lg" sx={{ pt: { xs: 3, md: 4 } }}>
          <Button variant="text" onClick={() => router.push(locationLink)} sx={{ textTransform: 'none', px: 0, mb: 3 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
              <ArrowLeftIcon fontSize="small" />
              <Typography sx={{ color: 'text.primary' }}>Back to location</Typography>
            </Box>
          </Button>

          <Paper sx={{ ...sectionCardSx, p: { xs: 3, md: 5 }, mb: 4 }}>
            <Typography sx={{ fontSize: '0.8rem', fontWeight: 700, letterSpacing: '0.1em', textTransform: 'uppercase', color: 'text.secondary', mb: 1.25 }}>
              {locationDetails.name}
            </Typography>
            <Typography sx={{ fontSize: { xs: '2rem', md: '2.8rem' }, fontWeight: 700, letterSpacing: '-0.04em', color: 'text.primary', mb: 1 }}>
              Choose a workspace from the floor plan
            </Typography>
            <Typography sx={{ fontSize: '1rem', color: 'text.secondary', maxWidth: 840 }}>
              Select a floor, tap a resource on the plan, and then book from the products that match that exact spot.
            </Typography>
          </Paper>

          {floorPlans.length === 0 ? (
            <Paper sx={{ ...sectionCardSx, p: { xs: 3, md: 5 } }}>
              <Typography sx={{ fontSize: '1.4rem', fontWeight: 700, color: 'text.primary', mb: 1 }}>No floor plans available yet</Typography>
              <Typography sx={{ color: 'text.secondary', mb: 3 }}>
                This location does not have a published floor plan yet. You can still browse the location and available workspaces.
              </Typography>
              <Button variant="contained" onClick={() => router.push(locationLink)} sx={{ textTransform: 'none', borderRadius: 999 }}>
                View location details
              </Button>
            </Paper>
          ) : (
            <Paper sx={{ ...sectionCardSx, p: { xs: 3, md: 5 } }}>
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1, mb: 3 }}>
                {floorPlans.map((floorPlan) => (
                  <Button
                    key={floorPlan.id}
                    variant={floorPlan.id === effectiveSelectedFloorPlanId ? 'contained' : 'outlined'}
                    onClick={() => {
                      setSelectedFloorPlanId(floorPlan.id);
                      setSelectedResourceId('');
                    }}
                    sx={{ textTransform: 'none', borderRadius: 999 }}
                  >
                    {floorPlan.name} ({floorPlan.resourceCount})
                  </Button>
                ))}
              </Box>

              {selectedFloorPlanImage?.url && selectedFloorPlanImage.width && selectedFloorPlanImage.height ? (
                <>
                  <Box
                    sx={{
                      position: 'relative',
                      width: '100%',
                      aspectRatio: `${selectedFloorPlanImageWidth} / ${selectedFloorPlanImageHeight}`,
                      borderRadius: 4,
                      overflow: 'hidden',
                      border: 1,
                      borderColor: 'divider',
                      bgcolor: 'background.paper',
                      mb: 3,
                    }}
                  >
                    <Box
                      component="img"
                      src={selectedFloorPlanImage.url}
                      alt={selectedFloorPlanName}
                      sx={{ width: '100%', height: '100%', objectFit: 'contain', display: 'block' }}
                    />
                    {selectedFloorPlanResourcePositions
                      .filter((position) => floorPlanResources.some((resource) => resource.id === position.resource.id))
                      .map((position) => {
                        const resource = floorPlanResources.find((item) => item.id === position.resource.id);
                        if (!resource) {
                          return null;
                        }

                        const ResourceIcon = getResourceTypeIcon(resource.resourceType.type, rootData.deskResourceType, rootData.roomResourceType, rootData.parkingResourceType);
                        const isSelected = resource.id === effectiveSelectedResourceId;

                        return (
                          <Box
                            key={resource.id}
                            component="button"
                            type="button"
                            onClick={() => setSelectedResourceId(resource.id)}
                            title={resource.name}
                            sx={{
                              position: 'absolute',
                              left: `${(position.x / selectedFloorPlanImageWidth) * 100}%`,
                              top: `${(position.y / selectedFloorPlanImageHeight) * 100}%`,
                              transform: 'translate(-50%, -50%)',
                              width: 42,
                              height: 42,
                              borderRadius: '50%',
                              border: 0,
                              cursor: 'pointer',
                              display: 'flex',
                              alignItems: 'center',
                              justifyContent: 'center',
                              color: isSelected ? 'common.white' : 'text.primary',
                              bgcolor: isSelected ? 'primary.main' : 'background.paper',
                              boxShadow: isSelected ? 5 : 2,
                              outline: 'none',
                              transition: 'transform 120ms ease, box-shadow 120ms ease, background-color 120ms ease',
                              '&:hover': {
                                transform: 'translate(-50%, -50%) scale(1.05)',
                              },
                            }}
                          >
                            <ResourceIcon fontSize="small" />
                          </Box>
                        );
                      })}
                  </Box>

                  <Box
                    sx={{
                      borderRadius: 4,
                      border: 1,
                      borderColor: 'divider',
                      bgcolor: 'background.paper',
                      p: { xs: 2.5, md: 3 },
                      mb: 3,
                    }}
                  >
                    {selectedResource ? (
                      <>
                        <Typography sx={{ fontSize: '0.8rem', fontWeight: 700, letterSpacing: '0.08em', textTransform: 'uppercase', color: 'text.secondary', mb: 1 }}>
                          Selected resource
                        </Typography>
                        <Typography sx={{ fontSize: '1.6rem', fontWeight: 700, color: 'text.primary', mb: 0.75 }}>{selectedResource.name}</Typography>
                        <Typography sx={{ color: 'text.secondary', mb: 2 }}>{selectedResource.resourceType.name}</Typography>

                        {selectedResource.productTags.length > 0 ? (
                          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                            {selectedResource.productTags.map((tag) => (
                              <Chip key={tag.id} label={tag.name} size="small" sx={{ bgcolor: 'action.hover' }} />
                            ))}
                          </Box>
                        ) : null}
                      </>
                    ) : (
                      <Typography sx={{ color: 'text.secondary' }}>
                        Choose a resource on the floor plan to see the products and pricing options available from that spot.
                      </Typography>
                    )}
                  </Box>

                  <Box>
                    <Typography sx={{ fontSize: '1.5rem', fontWeight: 700, color: 'text.primary', mb: 1.5 }}>Available products for this resource</Typography>
                    {selectedResource ? (
                      matchedProductsForSelectedResource.length > 0 ? (
                        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                          {matchedProductsForSelectedResource.map((product) => (
                            <Box key={product.id} sx={{ border: 1, borderColor: 'divider', borderRadius: 3, p: 2.5 }}>
                              <Typography sx={{ fontWeight: 700, color: 'text.primary' }}>{product.title}</Typography>
                              {product.subTitle ? <Typography sx={{ color: 'text.secondary', mt: 0.5 }}>{product.subTitle}</Typography> : null}

                              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1, mt: 1.75 }}>
                                {product.pricingRows.map((pricingRow) => (
                                  <Box
                                    key={pricingRow.id}
                                    sx={{
                                      display: 'flex',
                                      flexWrap: 'wrap',
                                      justifyContent: 'space-between',
                                      gap: 1,
                                      border: 1,
                                      borderColor: 'divider',
                                      borderRadius: 2.5,
                                      p: 1.25,
                                    }}
                                  >
                                    <Box sx={{ minWidth: 0 }}>
                                      <Typography sx={{ fontWeight: 600, color: 'text.primary' }}>{pricingRow.cadenceLabel}</Typography>
                                      {pricingRow.title ? <Typography sx={{ color: 'text.secondary', fontSize: '0.92rem' }}>{pricingRow.title}</Typography> : null}
                                    </Box>
                                    <Box sx={{ textAlign: 'right', ml: 'auto' }}>
                                      <Typography sx={{ fontWeight: 700, color: 'text.primary' }}>{pricingRow.amountLabel}</Typography>
                                      <Typography sx={{ color: 'text.secondary', fontSize: '0.85rem' }}>{pricingRow.taxLabel}</Typography>
                                    </Box>
                                    <Box sx={{ display: 'flex', width: '100%', gap: 1, mt: 0.5 }}>
                                      <Button
                                        variant="contained"
                                        onClick={() =>
                                          router.push(
                                            isSubscriptionCadence(pricingRow.cadence)
                                              ? getMarketplaceProductSubscribeLink(
                                                  integratedPlatrform,
                                                  isCustomDomain,
                                                  effectiveOrganizationCustomDomain,
                                                  product.id,
                                                  pricingRow.id,
                                                  [selectedResource.id],
                                                )
                                              : getMarketplaceProductBookingLink(
                                                  integratedPlatrform,
                                                  isCustomDomain,
                                                  effectiveOrganizationCustomDomain,
                                                  product.id,
                                                  pricingRow.id,
                                                  [selectedResource.id],
                                                ),
                                          )
                                        }
                                        sx={{ textTransform: 'none' }}
                                      >
                                        {pricingRow.bookingLabel}
                                      </Button>
                                      <Button
                                        variant="outlined"
                                        onClick={() =>
                                          router.push(
                                            getMarketplaceProductLink(integratedPlatrform, isCustomDomain, effectiveOrganizationCustomDomain, product.id, [selectedResource.id]),
                                          )
                                        }
                                        sx={{ textTransform: 'none' }}
                                      >
                                        Details
                                      </Button>
                                    </Box>
                                  </Box>
                                ))}
                              </Box>
                            </Box>
                          ))}
                        </Box>
                      ) : (
                        <Typography sx={{ color: 'text.secondary' }}>No marketplace products are currently mapped to this resource.</Typography>
                      )
                    ) : (
                      <Typography sx={{ color: 'text.secondary' }}>Select a resource first to unlock the products that can be booked from this floor plan.</Typography>
                    )}
                  </Box>
                </>
              ) : (
                <Typography sx={{ color: 'text.secondary' }}>This floor plan does not have an image yet.</Typography>
              )}
            </Paper>
          )}
        </Container>
      </Box>
    );
  }

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

              {floorPlans.length > 0 ? (
                <Box
                  sx={{
                    mt: extraMetadata?.website ? 1.5 : 4,
                    borderRadius: 4,
                    border: 1,
                    borderColor: 'divider',
                    bgcolor: 'action.hover',
                    p: 2.5,
                  }}
                >
                  <Typography sx={{ fontSize: '1rem', fontWeight: 700, color: 'text.primary', mb: 0.75 }}>Prefer choosing from the map?</Typography>
                  <Typography sx={{ color: 'text.secondary', mb: 2 }}>
                    Open the floor plan to pick a specific workspace first, then book the products available for that exact resource.
                  </Typography>
                  <Button
                    variant="contained"
                    onClick={() => router.push(floorPlansLink)}
                    sx={{
                      textTransform: 'none',
                      borderRadius: 999,
                      backgroundColor: 'success.main',
                      '&:hover': {
                        backgroundColor: 'success.dark',
                      },
                    }}
                  >
                    View floor plans
                  </Button>
                </Box>
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

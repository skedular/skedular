import {
  AreaIcon,
  ArrowLeftIcon,
  CheckIcon,
  ContactEmailIcon,
  ContactPhoneIcon,
  DeskIcon,
  LocationIcon,
  OpeningHoursIcon,
  OtherResourceIcon,
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
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import { formatPriceForDisplay, useIntegratedPlatform } from '@skedular/shared';
import {
  BodyIconTypography,
  CaptionIconTypography,
  LargeHeadingIconTypography,
  LeadIconTypography,
  SectionIconTypography,
  SmallIconTypography,
  SmallSubtitleIconTypography,
} from '@skedular/ui';
import type { LatLngTuple } from 'leaflet';
import { usePathname, useRouter } from 'next/navigation';
import { memo, type ReactNode, useEffect, useMemo, useState } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';
import useKnownParams from '@/hooks/use-known-params';

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

const customerFloorPlanMarkerXOffsetPx = 3;

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

const getResourceTypeIcon = (resourceType: string | null | undefined, deskResourceType: string, roomResourceType: string, parkingResourceType: string, color?: string | null) => {
  if (resourceType === deskResourceType) {
    return <DeskIcon sx={{ color }} />;
  }

  if (resourceType === roomResourceType) {
    return <RoomIcon sx={{ color }} />;
  }

  if (resourceType === parkingResourceType) {
    return <ParkingIcon sx={{ color }} />;
  }

  return <OtherResourceIcon sx={{ color }} />;
};

const InfoRow = ({ icon, label, children }: { icon: ReactNode; label: string; children: ReactNode }) => (
  <Box sx={{ display: 'flex', gap: 1.75, alignItems: 'flex-start' }}>
    <Box sx={{ mt: 0.25, color: 'text.secondary', display: 'flex' }}>{icon}</Box>
    <Box sx={{ minWidth: 0 }}>
      <CaptionIconTypography label={label} sx={{ fontWeight: 700, letterSpacing: '0.08em', textTransform: 'uppercase', mb: 0.75 }} color="text.secondary" />
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
            thumbnail {
              url
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
  const { integratedPlatform } = useIntegratedPlatform();
  const { isCustomDomain, organizationCustomDomain } = useKnownParams();
  const [dynamicLoadReady, setDynamicLoadReady] = useState(false);
  const [selectedHeroImageUrl, setSelectedHeroImageUrl] = useState<string>('');
  const [selectedFloorPlanId, setSelectedFloorPlanId] = useState<string>('');
  const [selectedResourceId, setSelectedResourceId] = useState<string>('');
  const locationDetails = rootData.location;
  const isFloorPlanPage = pathname.endsWith('/floorPlans');

  const capacity = (() => {
    if (!locationDetails?.extraMetadata?.peopleCapacity) return null;
    const { from, to } = locationDetails.extraMetadata.peopleCapacity;
    return from === to ? `${from} people` : `${from} - ${to} people`;
  })();

  const areaSize = (() => {
    if (!locationDetails?.extraMetadata?.areaRange) return null;
    const { fromInSqm, toInSqm } = locationDetails.extraMetadata.areaRange;
    return fromInSqm === toInSqm ? `${fromInSqm} m2` : `${fromInSqm} - ${toInSqm} m2`;
  })();

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
      ...(locationDetails?.featureImages
        ?.filter((item) => !!item.original?.url)
        .map((item) => ({
          url: item.original!.url,
          thumbnailUrl: item.thumbnail?.url ?? item.original!.url,
        })) ?? []),
      ...(extraMetadata?.relatedImageLinks?.filter(Boolean).map((url) => ({ url, thumbnailUrl: url })) ?? []),
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

    return '';
  }, [floorPlanResources, selectedResourceId]);
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
      // eslint-disable-next-line react-hooks/set-state-in-effect
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
      // eslint-disable-next-line react-hooks/set-state-in-effect
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

  const selectedFloorPlanImage = selectedFloorPlan?.image?.original;
  const selectedFloorPlanName = selectedFloorPlan?.name ?? '';
  const selectedFloorPlanResourcePositions = selectedFloorPlan?.resourcePositions ?? [];
  const selectedFloorPlanImageWidth = selectedFloorPlanImage?.width ?? 1;
  const selectedFloorPlanImageHeight = selectedFloorPlanImage?.height ?? 1;

  if (!dynamicLoadReady || !locationDetails || !openingHours) {
    return null;
  }

  const effectiveOrganizationCustomDomain = organizationCustomDomain || locationDetails.organization?.customDomain || '';
  const locationLink = getMarketplaceLocationLink(integratedPlatform, locationDetails.id);
  const floorPlansLink = getMarketplaceLocationFloorPlansLink(integratedPlatform, locationDetails.id);

  if (isFloorPlanPage) {
    return (
      <Box sx={{ bgcolor: 'background.default', minHeight: '100vh', pb: 8 }}>
        <Container maxWidth="lg" sx={{ pt: { xs: 3, md: 4 } }}>
          <Button variant="text" onClick={() => router.push(locationLink)} sx={{ textTransform: 'none', px: 0, mb: 3 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
              <ArrowLeftIcon fontSize="small" />
              <SmallIconTypography label="Back to location" color="text.primary" />
            </Box>
          </Button>

          <Paper sx={{ ...sectionCardSx, p: { xs: 3, md: 5 }, mb: 4 }}>
            <CaptionIconTypography
              label={locationDetails.name}
              sx={{ fontSize: '0.8rem', fontWeight: 700, letterSpacing: '0.1em', textTransform: 'uppercase', mb: 1.25 }}
              color="text.secondary"
            />
            <LargeHeadingIconTypography
              label="Choose a workspace from the floor plan"
              sx={{ fontSize: { xs: '2rem', md: '2.8rem' }, fontWeight: 700, letterSpacing: '-0.04em', mb: 1 }}
              color="text.primary"
            />
            <BodyIconTypography
              label="Select a floor, tap a resource on the plan, and then book from the products that match that exact spot."
              sx={{ fontSize: '1rem', maxWidth: 840 }}
              color="text.secondary"
            />
          </Paper>

          {floorPlans.length === 0 ? (
            <Paper sx={{ ...sectionCardSx, p: { xs: 3, md: 5 } }}>
              <LeadIconTypography label="No floor plans available yet" sx={{ fontSize: '1.4rem', fontWeight: 700, mb: 1 }} color="text.primary" />
              <BodyIconTypography
                label="This location does not have a published floor plan yet. You can still browse the location and available workspaces."
                sx={{ mb: 3 }}
                color="text.secondary"
              />
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
                      mb: 3,
                    }}
                  >
                    {/* eslint-disable-next-line @next/next/no-img-element -- Floor-plan coordinates must match the editor's plain image sizing. */}
                    <img src={selectedFloorPlanImage.url} alt={selectedFloorPlanName} style={{ display: 'block', width: '100%', height: '100%' }} />
                    {selectedFloorPlanResourcePositions
                      .filter((position) => floorPlanResources.some((resource) => resource.id === position.resource.id))
                      .map((position) => {
                        const resource = floorPlanResources.find((item) => item.id === position.resource.id);
                        if (!resource) {
                          return null;
                        }

                        const isSelected = resource.id === effectiveSelectedResourceId;
                        const resourceIcon = getResourceTypeIcon(
                          resource.resourceType.type,
                          rootData.deskResourceType,
                          rootData.roomResourceType,
                          rootData.parkingResourceType,
                          isSelected ? 'common.white' : 'common.black',
                        );

                        return (
                          <Box
                            key={resource.id}
                            role="button"
                            tabIndex={0}
                            onClick={() => setSelectedResourceId(resource.id)}
                            onKeyDown={(event) => {
                              if (event.key === 'Enter' || event.key === ' ') {
                                event.preventDefault();
                                setSelectedResourceId(resource.id);
                              }
                            }}
                            title={resource.name}
                            sx={{
                              position: 'absolute',
                              left: `calc(${(position.x / selectedFloorPlanImageWidth) * 100}% + ${customerFloorPlanMarkerXOffsetPx}px)`,
                              top: `${(position.y / selectedFloorPlanImageHeight) * 100}%`,
                              display: 'flex',
                              alignItems: 'center',
                              justifyContent: 'center',
                              width: 40,
                              height: 40,
                              borderRadius: '50%',
                              border: 2,
                              borderColor: isSelected ? 'primary.dark' : 'common.white',
                              backgroundColor: isSelected ? 'primary.main' : 'warning.main',
                              boxShadow: isSelected ? 4 : 3,
                              cursor: 'pointer',
                              outline: 'none',
                              transition: 'transform 120ms ease, box-shadow 120ms ease, background-color 120ms ease',
                              '&:hover': {
                                transform: 'scale(1.05)',
                              },
                            }}
                          >
                            {resourceIcon}
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
                        <CaptionIconTypography
                          label="Selected resource"
                          sx={{ fontSize: '0.8rem', fontWeight: 700, letterSpacing: '0.08em', textTransform: 'uppercase', mb: 1 }}
                          color="text.secondary"
                        />
                        <LeadIconTypography label={selectedResource.name} sx={{ fontSize: '1.6rem', fontWeight: 700, mb: 0.75 }} color="text.primary" />
                        <BodyIconTypography label={selectedResource.resourceType.name} sx={{ mb: 2 }} color="text.secondary" />

                        {selectedResource.productTags.length > 0 ? (
                          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                            {selectedResource.productTags.map((tag) => (
                              <Chip key={tag.id} label={tag.name} size="small" sx={{ bgcolor: 'action.hover' }} />
                            ))}
                          </Box>
                        ) : null}
                      </>
                    ) : (
                      <BodyIconTypography label="Choose a resource on the floor plan to see the products and pricing options available from that spot." color="text.secondary" />
                    )}
                  </Box>

                  <Box>
                    <LeadIconTypography label="Available products for this resource" sx={{ fontSize: '1.5rem', fontWeight: 700, mb: 1.5 }} color="text.primary" />
                    {selectedResource ? (
                      matchedProductsForSelectedResource.length > 0 ? (
                        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                          {matchedProductsForSelectedResource.map((product) => (
                            <Box key={product.id} sx={{ border: 1, borderColor: 'divider', borderRadius: 3, p: 2.5 }}>
                              <BodyIconTypography label={product.title} sx={{ fontWeight: 700 }} color="text.primary" />
                              {product.subTitle ? <BodyIconTypography label={product.subTitle} sx={{ mt: 0.5 }} color="text.secondary" /> : null}

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
                                      <SmallSubtitleIconTypography label={pricingRow.cadenceLabel} fontWeight={600} color="text.primary" />
                                      {pricingRow.title ? <SmallIconTypography label={pricingRow.title} sx={{ fontSize: '0.92rem' }} color="text.secondary" /> : null}
                                    </Box>
                                    <Box sx={{ textAlign: 'right', ml: 'auto' }}>
                                      <BodyIconTypography label={pricingRow.amountLabel} sx={{ fontWeight: 700 }} color="text.primary" />
                                      <SmallIconTypography label={pricingRow.taxLabel} sx={{ fontSize: '0.85rem' }} color="text.secondary" />
                                    </Box>
                                    <Box sx={{ display: 'flex', width: '100%', gap: 1, mt: 0.5 }}>
                                      <Button
                                        variant="contained"
                                        onClick={() =>
                                          router.push(
                                            isSubscriptionCadence(pricingRow.cadence)
                                              ? getMarketplaceProductSubscribeLink(
                                                  integratedPlatform,
                                                  isCustomDomain,
                                                  effectiveOrganizationCustomDomain,
                                                  product.id,
                                                  pricingRow.id,
                                                  [selectedResource.id],
                                                )
                                              : getMarketplaceProductBookingLink(integratedPlatform, isCustomDomain, effectiveOrganizationCustomDomain, product.id, pricingRow.id, [
                                                  selectedResource.id,
                                                ]),
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
                                            getMarketplaceProductLink(integratedPlatform, isCustomDomain, effectiveOrganizationCustomDomain, product.id, [selectedResource.id]),
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
                        <BodyIconTypography label="No marketplace products are currently mapped to this resource." color="text.secondary" />
                      )
                    ) : (
                      <BodyIconTypography label="Select a resource first to unlock the products that can be booked from this floor plan." color="text.secondary" />
                    )}
                  </Box>
                </>
              ) : (
                <BodyIconTypography label="This floor plan does not have an image yet." color="text.secondary" />
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
            <SmallIconTypography label="Back" color="text.primary" />
          </Box>
        </Button>

        <Box sx={{ mb: 2, minWidth: 0, maxWidth: '100%' }}>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 0.75, minWidth: 0, maxWidth: '100%' }}>
            <Box sx={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'flex-start', width: '100%', boxSizing: 'border-box', maxWidth: '100%', overflow: 'hidden' }}>
              {heroImage ? (
                <Box
                  component="img"
                  src={heroImage}
                  alt={locationDetails.listingMetadata.title ?? locationDetails.name}
                  sx={{
                    display: 'block',
                    width: { xs: '100%', md: 'auto' },
                    boxSizing: 'border-box',
                    height: 'auto',
                    maxWidth: '100%',
                    maxHeight: { md: 420 },
                    borderRadius: 3,
                    objectFit: 'contain',
                  }}
                />
              ) : (
                <Box sx={{ width: '100%', height: { xs: 260, md: 420 }, bgcolor: 'action.hover' }} />
              )}
            </Box>

            {heroImages.length > 1 ? (
              <Box
                sx={{
                  display: 'flex',
                  gap: 1,
                  width: '100%',
                  maxWidth: '100%',
                  overflowX: 'auto',
                  pb: 0.5,
                  scrollbarWidth: 'thin',
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
                        width: { xs: 72, md: 96 },
                        height: { xs: 54, md: 72 },
                        flex: '0 0 auto',
                        p: 0,
                        lineHeight: 0,
                        border: 2,
                        borderColor: isSelected ? theme.palette.primary.main : theme.palette.divider,
                        bgcolor: 'background.default',
                        cursor: 'pointer',
                        textAlign: 'left',
                        borderRadius: 1.5,
                        overflow: 'hidden',
                        outline: 'none',
                        opacity: isSelected ? 1 : 0.78,
                        '&:focus-visible': {
                          boxShadow: `0 0 0 2px ${theme.palette.primary.main}`,
                        },
                      }}
                    >
                      <Box
                        component="img"
                        src={image.thumbnailUrl}
                        alt={`${locationDetails.name} image ${index + 1}`}
                        sx={{ width: '100%', height: '100%', objectFit: 'contain', display: 'block' }}
                      />
                    </Box>
                  );
                })}
              </Box>
            ) : null}
          </Box>
        </Box>

        <Paper sx={{ ...sectionCardSx, mb: 4 }}>
          <Box sx={{ p: { xs: 3, md: 5 } }}>
            <CaptionIconTypography
              label={locationDetails.name}
              sx={{ fontSize: '0.8rem', fontWeight: 700, letterSpacing: '0.1em', textTransform: 'uppercase', mb: 1.25 }}
              color="text.secondary"
            />
            <LargeHeadingIconTypography
              label={locationDetails.listingMetadata.title || locationDetails.name}
              sx={{ fontSize: { xs: '2rem', md: '3.1rem' }, fontWeight: 700, letterSpacing: '-0.04em', mb: 1 }}
              color="text.primary"
            />
            {locationDetails.listingMetadata.subTitle ? (
              <BodyIconTypography label={locationDetails.listingMetadata.subTitle} sx={{ fontSize: { xs: '1.05rem', md: '1.25rem' }, mb: 3 }} color="text.secondary" />
            ) : null}
            {locationDetails.listingMetadata.about ? (
              <BodyIconTypography
                label={locationDetails.listingMetadata.about}
                sx={{ fontSize: '1rem', lineHeight: 1.9, whiteSpace: 'pre-line', maxWidth: 900 }}
                color="text.primary"
              />
            ) : null}
          </Box>
        </Paper>

        <Grid container spacing={3} sx={{ mb: 4 }}>
          <Grid size={{ xs: 12, md: 6 }}>
            <Paper sx={{ ...sectionCardSx, p: { xs: 3, md: 4 }, height: '100%' }}>
              <LeadIconTypography label="Contact & Location" sx={{ fontSize: '1.5rem', fontWeight: 700, mb: 3 }} color="text.primary" />
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
                {locationDetails.physicalAddress?.multilinesFormattedAddress ? (
                  <InfoRow icon={<LocationIcon fontSize="small" />} label="Address">
                    <BodyIconTypography label={locationDetails.physicalAddress.multilinesFormattedAddress} sx={{ lineHeight: 1.7, whiteSpace: 'pre-line' }} color="text.primary" />
                  </InfoRow>
                ) : null}

                {primaryPhone ? (
                  <InfoRow icon={<ContactPhoneIcon fontSize="small" />} label="Phone">
                    <Link href={`tel:${primaryPhone.trim().replace(/[^\d+]/g, '')}`} underline="hover" color="inherit">
                      <BodyIconTypography label={primaryPhone} color="text.primary" />
                    </Link>
                  </InfoRow>
                ) : null}

                {primaryEmail ? (
                  <InfoRow icon={<ContactEmailIcon fontSize="small" />} label="Email">
                    <Link href={`mailto:${primaryEmail}`} underline="hover" color="inherit">
                      <BodyIconTypography label={primaryEmail} color="text.primary" />
                    </Link>
                  </InfoRow>
                ) : null}

                {extraMetadata?.website ? (
                  <InfoRow icon={<CheckIcon fontSize="small" />} label="Website">
                    <Link href={extraMetadata.website} target="_blank" rel="noopener noreferrer" underline="hover" color="inherit">
                      <BodyIconTypography label={extraMetadata.website} sx={{ overflowWrap: 'anywhere' }} color="text.primary" />
                    </Link>
                  </InfoRow>
                ) : null}

                {areaSize ? (
                  <InfoRow icon={<AreaIcon fontSize="small" />} label="Area">
                    <BodyIconTypography label={areaSize} color="text.primary" />
                  </InfoRow>
                ) : null}

                {capacity ? (
                  <InfoRow icon={<PersonIcon fontSize="small" />} label="Capacity">
                    <BodyIconTypography label={capacity} color="text.primary" />
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
                  <BodyIconTypography label="Prefer choosing from the map?" sx={{ fontSize: '1rem', fontWeight: 700, mb: 0.75 }} color="text.primary" />
                  <BodyIconTypography
                    label="Open the floor plan to pick a specific workspace first, then book the products available for that exact resource."
                    sx={{ mb: 2 }}
                    color="text.secondary"
                  />
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
                <LeadIconTypography label="Opening Hours" sx={{ fontSize: '1.5rem', fontWeight: 700 }} color="text.primary" />
              </Box>

              <Box sx={{ display: 'flex', flexDirection: 'column' }}>
                {locationDays.map(([day, schedule], index) => (
                  <Box
                    key={day}
                    sx={{ display: 'flex', justifyContent: 'space-between', gap: 2, py: 1.4, borderBottom: index < locationDays.length - 1 ? 1 : 0, borderColor: 'divider' }}
                  >
                    <SmallSubtitleIconTypography label={day} fontWeight={600} color="text.primary" />
                    <SmallIconTypography label={formatOpeningHours(schedule)} sx={{ textAlign: 'right' }} color="text.secondary" />
                  </Box>
                ))}
              </Box>
            </Paper>
          </Grid>
        </Grid>

        <Paper sx={{ ...sectionCardSx, p: { xs: 3, md: 5 }, mb: 4 }}>
          <SectionIconTypography label="Amenities & Features" sx={{ fontSize: '1.75rem', fontWeight: 700, mb: 4 }} color="text.primary" />

          <Grid container spacing={3}>
            <Grid size={{ xs: 12, md: 6 }}>
              <Box sx={{ bgcolor: 'action.hover', borderRadius: 4, p: 3, height: '100%' }}>
                <BodyIconTypography label="What's included" sx={{ fontSize: '1rem', fontWeight: 700, mb: 2 }} color="text.primary" />
                {includedFeatures.length > 0 ? (
                  <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
                    {includedFeatures.map((feature) => (
                      <Box key={feature} sx={{ display: 'flex', alignItems: 'flex-start', gap: 1.25 }}>
                        <CheckIcon sx={{ color: 'success.main', fontSize: 18, mt: '2px' }} />
                        <BodyIconTypography label={feature} color="text.secondary" />
                      </Box>
                    ))}
                  </Box>
                ) : (
                  <BodyIconTypography label="No included features listed yet." color="text.secondary" />
                )}
              </Box>
            </Grid>

            <Grid size={{ xs: 12, md: 6 }}>
              <Box sx={{ bgcolor: 'action.hover', borderRadius: 4, p: 3, height: '100%' }}>
                <BodyIconTypography label="Amenities" sx={{ fontSize: '1rem', fontWeight: 700, mb: 2 }} color="text.primary" />
                {amenities.length > 0 ? (
                  <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
                    {amenities.map((amenity) => (
                      <Box key={amenity.id} sx={{ display: 'flex', alignItems: 'flex-start', gap: 1.25 }}>
                        <CheckIcon sx={{ color: 'success.main', fontSize: 18, mt: '2px' }} />
                        <BodyIconTypography label={amenity.name} color="text.secondary" />
                      </Box>
                    ))}
                  </Box>
                ) : (
                  <BodyIconTypography label="No amenities listed yet." color="text.secondary" />
                )}
              </Box>
            </Grid>
          </Grid>
        </Paper>

        <Paper sx={{ ...sectionCardSx, p: { xs: 3, md: 4 }, mb: 4 }}>
          <LeadIconTypography label="Find Us Here" sx={{ fontSize: '1.5rem', fontWeight: 700, mb: 3 }} color="text.primary" />
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
          <SectionIconTypography label="Available Workspaces" sx={{ fontSize: '1.75rem', fontWeight: 700, letterSpacing: '-0.03em', mb: 1 }} color="text.primary" />
          <BodyIconTypography label="Book these products at this location" sx={{ fontSize: '1rem', mb: 4 }} color="text.secondary" />

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
            <BodyIconTypography label="There are no bookable products listed for this location right now." color="text.secondary" />
          )}
        </Paper>
      </Container>
    </Box>
  );
};

export default memo(MarketplaceLocation);

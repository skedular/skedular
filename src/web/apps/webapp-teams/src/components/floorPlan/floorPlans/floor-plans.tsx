import { BookingCard } from '@/components/booking/bookings';
import { DayPicker } from '@/components/datePickers';
import { FloorPlanSelector } from '@/components/floorPlan/floorPlanSelector';
import { CustomerAvatar } from '@/components/avatars';
import { ResourceCard } from '@/components/resource';
import type { floorPlans_bookings_query$key } from '@/queries/__generated__/floorPlans_bookings_query.graphql';
import type { floorPlans_bookings_refetchableFragment } from '@/queries/__generated__/floorPlans_bookings_refetchableFragment.graphql';
import type { floorPlans_floorPlan_query$key } from '@/queries/__generated__/floorPlans_floorPlan_query.graphql';
import type { floorPlans_floorPlan_refetchableFragment } from '@/queries/__generated__/floorPlans_floorPlan_refetchableFragment.graphql';
import type { floorPlans_query$key } from '@/queries/__generated__/floorPlans_query.graphql';
import Popover from '@mui/material/Popover';
import Tooltip from '@mui/material/Tooltip';
import Box from '@mui/system/Box';
import { dateRangeToShortDateWithAdditionalDayInfo, endOfDay, getCustomerFullName, startOfDay } from '@skedular/shared';
import { BodyIconTypography, defaultPadding, emerald, flame, GridContainer, maxScreenWidth, SmallIconTypography, StackColumn } from '@skedular/ui';
import dayjs, { Dayjs } from 'dayjs';
import { memo, useCallback, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { graphql, useFragment, useRefetchableFragment } from 'react-relay';
// import { CustomTagSelector } from '@/components/organization/customTagSelector';
// import { OrganizationUserSelector } from '@/components/organization/organizationUserSelector';
// import { ZoneSelector } from '@/components/organization/zoneSelector';

type Props = {
  rootDataRelay: floorPlans_query$key;
  rootDataFloorPlanRelay: floorPlans_floorPlan_query$key;
  rootDataBookingsRelay: floorPlans_bookings_query$key;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  locationId: string;
};

const FloorPlans = ({ rootDataRelay, rootDataFloorPlanRelay, rootDataBookingsRelay, onReloadRequired, organizationCustomDomain, locationId }: Props) => {
  const rootData = useFragment<floorPlans_query$key>(
    graphql`
      fragment floorPlans_query on Query {
        me {
          id
        }
        deskResourceType
        roomResourceType
        parkingResourceType
        ...customTagSelector_allCustomTags_query
        ...zoneSelector_allZones_query
        ...floorPlanSelector_allFloorPlans_query
        ...organizationUserSelector_organizationMembers_query
        ...bookingCard_query
        ...resourceCard_query
      }
    `,
    rootDataRelay,
  );

  const [rootDataFloorPlan, refetchFloorPlan] = useRefetchableFragment<floorPlans_floorPlan_refetchableFragment, floorPlans_floorPlan_query$key>(
    graphql`
      fragment floorPlans_floorPlan_query on Query @refetchable(queryName: "floorPlans_floorPlan_refetchableFragment") {
        floorPlan(id: $floorPlanId) @include(if: $floorPlanExists) {
          id
          name
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
        location(id: $locationId) {
          resources(where: { floorPlanId: $floorPlanId }, orderBy: $resourcesSortingValues) @include(if: $floorPlanExists) {
            edges {
              node {
                id
                name
                resourceType {
                  type
                }
                ...resourceCard_ResourceDetails
              }
            }
          }
        }
      }
    `,
    rootDataFloorPlanRelay,
  );

  const [rootDataBookings, refetchBookings] = useRefetchableFragment<floorPlans_bookings_refetchableFragment, floorPlans_bookings_query$key>(
    graphql`
      fragment floorPlans_bookings_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "floorPlans_bookings_refetchableFragment") {
        bookings(first: $count, after: $cursor, where: { locationIds: [$locationId], fromGte: $bookingsSearchCriteriaFrom, fromLte: $bookingsSearchCriteriaTo })
          @connection(key: "floorPlans_bookings") {
          __id
          totalCount
          edges {
            node {
              id
              involvedCustomers {
                id
                name
                givenName
                middleName
                familyName
                photoUrl
              }
              from
              until
              category {
                name
              }
              bookingResources {
                resource {
                  id
                }
              }
              ...bookingCard_BookingDetails
            }
          }
        }
      }
    `,
    rootDataBookingsRelay,
  );

  const [, startTransition] = useTransition();
  const mountedRef = useRef(true);
  useEffect(() => {
    mountedRef.current = true;

    return () => {
      mountedRef.current = false;
    };
  }, []);
  const resources = useMemo(
    () => (rootDataFloorPlan.location && rootDataFloorPlan.location.resources ? rootDataFloorPlan.location.resources.edges.map(({ node }) => node) : []),
    [rootDataFloorPlan.location],
  );
  const resourcePositions = useMemo(
    () =>
      (rootDataFloorPlan.floorPlan?.resourcePositions ? rootDataFloorPlan.floorPlan.resourcePositions : []).reduce(
        (acc, { x, y, resource }) => acc.set(resource.id, { x, y }),
        new Map<string, { x: number; y: number }>(),
      ),
    [rootDataFloorPlan.floorPlan],
  );
  const bookings = useMemo(() => (rootDataBookings.bookings ? rootDataBookings.bookings.edges.map((edge) => edge.node) : []), [rootDataBookings.bookings]);
  const bookingConnectionIds = useMemo(() => (rootDataBookings.bookings ? [rootDataBookings.bookings.__id] : []), [rootDataBookings.bookings]);
  const [date, setDate] = useState<Dayjs>(startOfDay());
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);
  const [selectedResourceId, setSelectedResourceId] = useState<string | null>(null);
  const [selectedBookingId, setSelectedBookingId] = useState<string | null>(null);
  const selectedResource = useMemo(() => resources.find((item) => item.id === selectedResourceId), [selectedResourceId, resources]);
  const selectedBooking = useMemo(() => bookings.find((item) => item.id === selectedBookingId), [selectedBookingId, bookings]);

  const handleRefetchFloorPlan = useCallback(
    (floorPlanId: string) => {
      if (!mountedRef.current) {
        return;
      }

      startTransition(() => {
        if (!mountedRef.current) {
          return;
        }

        refetchFloorPlan(
          {
            floorPlanId,
            floorPlanExists: !!floorPlanId,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [startTransition, refetchFloorPlan],
  );

  const handleRefetchBookings = useCallback(
    (date: Dayjs) => {
      if (!mountedRef.current) {
        return;
      }

      startTransition(() => {
        if (!mountedRef.current) {
          return;
        }

        refetchBookings(
          {
            bookingsSearchCriteriaFrom: date.toISOString(),
            bookingsSearchCriteriaTo: endOfDay(date).toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [startTransition, refetchBookings],
  );

  const handleFloorPlanChanged = (id?: string) => {
    if (id) {
      handleRefetchFloorPlan(id);
    }
  };

  const handleDateChanged = (date: Dayjs) => {
    setDate(date);

    handleRefetchBookings(date);
  };

  const handleResourceClick = (event: React.MouseEvent<HTMLElement>, resourceId: string, bookingId: string | null) => {
    setAnchorEl(event.currentTarget);
    setSelectedResourceId(resourceId);
    setSelectedBookingId(bookingId);
  };

  const handlePopoverClose = () => {
    setAnchorEl(null);
    setSelectedResourceId(null);
    setSelectedBookingId(null);
  };

  const handleReloadRequired = () => {
    startTransition(() => {
      if (!mountedRef.current) {
        return;
      }

      handlePopoverClose();
      handleRefetchBookings(date);
      onReloadRequired();
    });
  };

  const floorPlanImageWidth = rootDataFloorPlan.floorPlan?.image?.original?.width ?? 1;
  const floorPlanImageHeight = rootDataFloorPlan.floorPlan?.image?.original?.height ?? 1;

  const getBookingTooltipTitle = (
    resourceName: string,
    bookings: readonly {
      readonly from: string;
      readonly until: string;
      readonly category: { readonly name: string };
      readonly involvedCustomers: readonly {
        readonly id: string;
        readonly name: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly middleName: string | null | undefined;
        readonly familyName: string | null | undefined;
        readonly photoUrl: string | null | undefined;
      }[];
    }[],
  ) => {
    if (bookings.length === 0) {
      return (
        <StackColumn spacing={0.5}>
          <BodyIconTypography label={resourceName} fontWeight={700} />
          <SmallIconTypography label="Available" />
        </StackColumn>
      );
    }

    return (
      <StackColumn spacing={0.75}>
        <BodyIconTypography label={resourceName} fontWeight={700} />
        {bookings.slice(0, 3).map((booking) => {
          const customer = booking.involvedCustomers[0];
          const dateRange = dateRangeToShortDateWithAdditionalDayInfo(dayjs(booking.from), dayjs(booking.until));
          const customerName = customer ? getCustomerFullName(customer) : 'Unknown person';

          return (
            <StackColumn key={`${booking.from}-${customer?.id ?? 'unknown'}`} spacing={0.25}>
              <SmallIconTypography label={customerName} fontWeight={700} />
              <SmallIconTypography label={`${booking.category.name} · ${dateRange.primaryLine}${dateRange.secondaryLine ? ` · ${dateRange.secondaryLine}` : ''}`} />
            </StackColumn>
          );
        })}
        {bookings.length > 3 && <SmallIconTypography label={`+${bookings.length - 3} more booking${bookings.length - 3 === 1 ? '' : 's'}`} />}
      </StackColumn>
    );
  };

  return (
    <>
      <StackColumn sx={{ maxWidth: maxScreenWidth }}>
        <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
          <FloorPlanSelector rootDataRelay={rootData} onChange={handleFloorPlanChanged} />
          <DayPicker defaultDate={date} onDateChanged={handleDateChanged} />
        </GridContainer>
        {rootDataFloorPlan.floorPlan && rootDataFloorPlan.location?.resources && (
          <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
            {rootDataFloorPlan.floorPlan.image?.original && rootDataFloorPlan.floorPlan.image.original.height && rootDataFloorPlan.floorPlan.image.original.width && (
              <Box
                sx={{
                  position: 'relative',
                  width: '100%',
                  aspectRatio: `${rootDataFloorPlan.floorPlan.image.original.width} / ${rootDataFloorPlan.floorPlan.image.original.height}`,
                }}
              >
                <img src={rootDataFloorPlan.floorPlan.image.original.url} alt="" style={{ display: 'block', width: '100%', height: '100%' }} />

                {[...resourcePositions.entries()].map(([id, position]) => {
                  const resource = resources.find((item) => item.id === id);
                  if (!resource) {
                    return null;
                  }
                  const resourceBookings = bookings.filter((item) => item.bookingResources.some((bookingResource) => bookingResource.resource.id === resource.id));
                  const booking = resourceBookings[0];
                  const primaryCustomer = booking?.involvedCustomers[0];

                  return (
                    <Tooltip
                      key={resource.id}
                      title={getBookingTooltipTitle(resource.name, resourceBookings)}
                      arrow
                      placement="top"
                      slotProps={{
                        tooltip: {
                          sx: {
                            bgcolor: 'background.paper',
                            color: 'text.primary',
                            border: 1,
                            borderColor: 'divider',
                            boxShadow: 3,
                            p: 1.25,
                            '& .MuiTypography-root': {
                              color: 'inherit',
                            },
                          },
                        },
                        arrow: {
                          sx: {
                            color: 'background.paper',
                            '&::before': {
                              border: 1,
                              borderColor: 'divider',
                            },
                          },
                        },
                      }}
                    >
                      <Box
                        sx={{
                          position: 'absolute',
                          left: `${(position.x / floorPlanImageWidth) * 100}%`,
                          top: `${(position.y / floorPlanImageHeight) * 100}%`,
                          display: 'flex',
                          alignItems: 'center',
                          justifyContent: 'center',
                          width: 40,
                          height: 40,
                          borderRadius: '50%',
                          border: 2,
                          backgroundColor: (theme) => theme.palette.background.paper,
                          boxShadow: 1,
                          cursor: 'pointer',
                          overflow: 'hidden',
                        }}
                        onClick={(event) => handleResourceClick(event, resource.id, booking ? booking.id : null)}
                      >
                        {!booking && <Box sx={{ width: 20, height: 20, borderRadius: '50%', backgroundColor: emerald }} />}
                        {booking && primaryCustomer && <CustomerAvatar name={primaryCustomer} photo={{ url: primaryCustomer.photoUrl }} size="medium" />}
                        {booking && !primaryCustomer && <Box sx={{ width: 20, height: 20, borderRadius: '50%', backgroundColor: flame }} />}
                      </Box>
                    </Tooltip>
                  );
                })}
              </Box>
            )}
          </StackColumn>
        )}
      </StackColumn>
      <Popover
        open={Boolean(anchorEl)}
        anchorEl={anchorEl}
        onClose={handlePopoverClose}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'left',
        }}
      >
        {selectedBooking && (
          <BookingCard
            rootDataRelay={rootData}
            bookingDetailsRelay={selectedBooking}
            organizationCustomDomain={organizationCustomDomain}
            connectionIds={bookingConnectionIds}
            canJoinBooking={!bookings.some((item) => item.involvedCustomers.some((involvedCustomer) => involvedCustomer.id === rootData.me.id))}
          />
        )}
        {selectedResource && !selectedBooking && (
          <ResourceCard
            rootDataRelay={rootData}
            resourceDetailsRelay={selectedResource}
            onReloadRequired={handleReloadRequired}
            organizationCustomDomain={organizationCustomDomain}
            locationId={locationId}
            date={date}
            connectionIds={bookingConnectionIds}
          />
        )}
      </Popover>
    </>
  );
};

export default memo(FloorPlans);

import { BookingCard } from '@/components/booking/bookings';
import { GridContainer, StackColumn } from '@/components/commons';
import { DayPicker } from '@/components/datePickers';
import { FloorPlanSelector } from '@/components/floorPlan/floorPlanSelector';
import { ResourceCard } from '@/components/resource';
import { defaultPadding, emerald, flame, maxScreenWidth } from '@/libs/theme';
import { endOfDay, startOfDay } from '@/libs/utils';
import type { floorPlans_bookings_query$key } from '@/queries/__generated__/floorPlans_bookings_query.graphql';
import type { floorPlans_bookings_refetchableFragment } from '@/queries/__generated__/floorPlans_bookings_refetchableFragment.graphql';
import type { floorPlans_floorPlan_query$key } from '@/queries/__generated__/floorPlans_floorPlan_query.graphql';
import type { floorPlans_floorPlan_refetchableFragment } from '@/queries/__generated__/floorPlans_floorPlan_refetchableFragment.graphql';
import type { floorPlans_query$key } from '@/queries/__generated__/floorPlans_query.graphql';
import Popover from '@mui/material/Popover';
import Box from '@mui/system/Box';
import { Dayjs } from 'dayjs';
import Image from 'next/image';
import { memo, useCallback, useMemo, useState, useTransition } from 'react';
import { graphql, useFragment, useRefetchableFragment } from 'react-relay';
// import { CustomTagSelector } from '@/components/organization/customTagSelector';
// import { OrganizationUserSelector } from '@/components/organization/organizationUserSelector';
// import { ZoneSelector } from '@/components/organization/zoneSelector';

type Props = {
  rootDataRelay: floorPlans_query$key;
  rootDataFloorPlanRelay: floorPlans_floorPlan_query$key;
  rootDataBookingsRelay: floorPlans_bookings_query$key;
  onReloadRequired: () => void;
  organizationId: string;
  locationId: string;
};

const FloorPlans = ({ rootDataRelay, rootDataFloorPlanRelay, rootDataBookingsRelay, onReloadRequired, organizationId, locationId }: Props) => {
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
        resources(where: { locationId: $locationId, floorPlanId: $floorPlanId }, orderBy: $resourcesSortingValues) @include(if: $floorPlanExists) {
          edges {
            node {
              id
              name
              resourceType {
                tagType
              }
              ...resourceCard_ResourceDetails
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
                uniqueId
              }
              resources {
                uniqueId
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
  const resources = useMemo(() => (rootDataFloorPlan.resources ? rootDataFloorPlan.resources.edges.map(({ node }) => node) : []), [rootDataFloorPlan.resources]);
  const resourcePositions = useMemo(
    () =>
      (rootDataFloorPlan.floorPlan?.resourcePositions ? rootDataFloorPlan.floorPlan.resourcePositions : []).reduce(
        (acc, { x, y, resource }, index) => acc.set(resource.id, { x, y }),
        new Map<string, { x: number; y: number }>(),
      ),
    [rootDataFloorPlan.floorPlan],
  );
  const bookings = useMemo(() => (rootDataBookings.bookings ? rootDataBookings.bookings.edges.map((edge) => edge.node) : []), [rootDataBookings.bookings]);
  const bookingConnectionIds = useMemo(() => (rootDataBookings.bookings ? [rootDataBookings.bookings.__id] : []), [rootDataBookings.bookings]);
  const [floorPlanIds, setFloorPlanIds] = useState<string[]>([]);
  const [date, setDate] = useState<Dayjs>(startOfDay());
  const [customerIds, setCustomerIds] = useState<string[]>([]);
  const [customTagIds, setCustomTagIds] = useState<string[]>([]);
  const [zoneIds, setZoneIds] = useState<string[]>([]);
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);
  const [selectedResourceId, setSelectedResourceId] = useState<string | null>(null);
  const [selectedBookingId, setSelectedBookingId] = useState<string | null>(null);
  const selectedResource = useMemo(() => resources.find((item) => item.id === selectedResourceId), [selectedResourceId, resources]);
  const selectedBooking = useMemo(() => bookings.find((item) => item.id === selectedBookingId), [selectedBookingId, bookings]);

  const handleRefetchFloorPlan = useCallback(
    (floorPlanId: string) => {
      startTransition(() => {
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
    [refetchFloorPlan],
  );

  const handleRefetchBookings = useCallback(
    (date: Dayjs) => {
      startTransition(() => {
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
    [refetchBookings],
  );

  const handleFloorPlanChanged = (id?: string) => {
    setFloorPlanIds(id ? [id] : []);

    if (id) {
      handleRefetchFloorPlan(id);
    }
  };

  const handleDateChanged = (date: Dayjs) => {
    setDate(date);

    handleRefetchBookings(date);
  };

  const handlCustomerChanged = (id?: string) => {
    setCustomerIds(id ? [id] : []);
  };

  const handleCustomTagChanged = (id?: string) => {
    setCustomTagIds(id ? [id] : []);
  };

  const handleZoneTypeChanged = (id?: string) => {
    setZoneIds(id ? [id] : []);
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
      handlePopoverClose();
      handleRefetchBookings(date);
      onReloadRequired();
    });
  };

  return (
    <>
      <StackColumn sx={{ maxWidth: maxScreenWidth }}>
        <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
          <FloorPlanSelector rootDataRelay={rootData} onChange={handleFloorPlanChanged} />
          <DayPicker defaultDate={date} onDateChanged={handleDateChanged} />
          {/* <OrganizationUserSelector rootDataOrganizationMembersRelay={rootData} onChange={handlCustomerChanged} />
          <ZoneSelector rootDataRelay={rootData} onChange={handleZoneTypeChanged} />
          <CustomTagSelector rootDataRelay={rootData} onChange={handleCustomTagChanged} /> */}
        </GridContainer>
        {rootDataFloorPlan.floorPlan && rootDataFloorPlan.resources && (
          <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
            {rootDataFloorPlan.floorPlan.image?.original && rootDataFloorPlan.floorPlan.image.original.height && rootDataFloorPlan.floorPlan.image.original.width && (
              <Box
                sx={{
                  position: 'relative',
                  display: 'inline-block',
                  width: rootDataFloorPlan.floorPlan.image.original.width,
                  height: rootDataFloorPlan.floorPlan.image.original.height,
                }}
              >
                <Image
                  src={rootDataFloorPlan.floorPlan.image.original.url}
                  height={rootDataFloorPlan.floorPlan.image.original.height}
                  width={rootDataFloorPlan.floorPlan.image.original.width}
                  alt=""
                />

                {[...resourcePositions.entries()].map(([id, position]) => {
                  const resource = resources.find((item) => item.id === id);
                  if (!resource) {
                    return <></>;
                  }
                  const booking = bookings.find((item) => item.resources.some((bookingResource) => bookingResource.uniqueId === resource.id));

                  return (
                    <Box
                      key={resource.id}
                      sx={{
                        position: 'absolute',
                        left: position.x,
                        top: position.y,
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        width: 40,
                        height: 40,
                        borderRadius: '50%',
                        border: 2,
                        backgroundColor: (theme) => theme.palette.background.paper,
                        boxShadow: 1,
                      }}
                      title={resource.name}
                      onClick={(event) => handleResourceClick(event, resource.id, booking ? booking.id : null)}
                    >
                      {!booking && <Box sx={{ width: 20, height: 20, borderRadius: '50%', backgroundColor: emerald }} />}
                      {booking && <Box sx={{ width: 20, height: 20, borderRadius: '50%', backgroundColor: flame }} />}
                    </Box>
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
            organizationId={organizationId}
            connectionIds={bookingConnectionIds}
            canJoinBooking={!bookings.some((item) => item.involvedCustomers.some((involvedCustomer) => involvedCustomer.uniqueId === rootData.me.id))}
          />
        )}
        {selectedResource && !selectedBooking && (
          <ResourceCard
            rootDataRelay={rootData}
            resourceDetailsRelay={selectedResource}
            onReloadRequired={handleReloadRequired}
            organizationId={organizationId}
            locationId={locationId}
            date={date}
          />
        )}
      </Popover>
    </>
  );
};

export default memo(FloorPlans);

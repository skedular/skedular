import { CustomerAvatar } from '@/components/avatars';
import { LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { Desks } from '@/components/desk';
import { CalendarIcon, EllipseMenuIcon, JoinIcon, LocationIcon, NotesIcon, TeamIcon } from '@/components/icons';
import { getOrganizationBookingBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { Resources } from '@/components/resource';
import { Rooms } from '@/components/room';
import { Zones } from '@/components/zone';
import { PaletteModeContext, UpdateGlobalReloadIdContext } from '@/libs/providers';
import { coal, sandstone } from '@/libs/theme';
import { getCustomerFullName, joinErrors, toShortDate, toShortDateWithAdditionalDayInfo } from '@/libs/utils';
import type { bookingCard_addBookingMutation } from '@/queries/__generated__/bookingCard_addBookingMutation.graphql';
import type { bookingCard_BookingDetails$key } from '@/queries/__generated__/bookingCard_BookingDetails.graphql';
import type { bookingCard_deleteBookingMutation } from '@/queries/__generated__/bookingCard_deleteBookingMutation.graphql';
import type { bookingCard_query$key } from '@/queries/__generated__/bookingCard_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Box from '@mui/system/Box';
import dayjs from 'dayjs';
import { nanoid } from 'nanoid';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';

type Props = {
  rootDataRelay: bookingCard_query$key;
  bookingDetailsRelay: bookingCard_BookingDetails$key;
  organizationId: string;
  connectionIds: string[];
  canJoinBooking: boolean;
};

type CustomTagDetails = {
  uniqueId: string;
  name: string | null | undefined;
  color?: string | null | undefined;
};

type ZoneDetails = {
  uniqueId: string;
  name: string | null | undefined;
  color?: string | null | undefined;
};

const BookingCard = ({ rootDataRelay, bookingDetailsRelay, organizationId, connectionIds, canJoinBooking }: Props) => {
  const rootData = useFragment<bookingCard_query$key>(
    graphql`
      fragment bookingCard_query on Query {
        me {
          id
          name
          givenName
          middleName
          familyName
          photoUrl
        }
      }
    `,
    rootDataRelay,
  );

  const bookingDetails = useFragment(
    graphql`
      fragment bookingCard_BookingDetails on BookingDetails {
        id
        from
        until
        notes
        customer {
          uniqueId
          name
          givenName
          middleName
          familyName
          photoUrl
        }
        organization {
          uniqueId
        }
        location {
          uniqueId
          name
        }
        team {
          uniqueId
          name
        }
        desks {
          uniqueId
          name
          color
          customTags {
            uniqueId
            name
            color
          }
          zones {
            uniqueId
            name
            color
          }
        }
        rooms {
          uniqueId
          name
          color
          customTags {
            uniqueId
            name
            color
          }
          zones {
            uniqueId
            name
            color
          }
        }
        resources {
          uniqueId
          name
          color
          customTags {
            uniqueId
            name
            color
          }
          zones {
            uniqueId
            name
            color
          }
        }
      }
    `,
    bookingDetailsRelay,
  );

  const [commitDeleteBooking] = useMutation<bookingCard_deleteBookingMutation>(graphql`
    mutation bookingCard_deleteBookingMutation($connectionIds: [ID!]!, $input: DeleteBookingInput!) {
      deleteBooking(input: $input) {
        booking {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddBooking] = useMutation<bookingCard_addBookingMutation>(graphql`
    mutation bookingCard_addBookingMutation($connectionIds: [ID!]!, $input: AddBookingInput!) @raw_response_type {
      addBooking(input: $input) {
        booking @appendNode(connections: $connectionIds, edgeTypeName: "BookingDetails") {
          id
          from
          until
          notes
          type
          customer {
            uniqueId
            name
            givenName
            middleName
            familyName
            photoUrl
          }
          location {
            uniqueId
            name
          }
          team {
            uniqueId
            name
          }
          desks {
            uniqueId
            name
            customTags {
              uniqueId
              name
              color
            }
            zones {
              uniqueId
              name
              color
            }
          }
          rooms {
            uniqueId
            name
            customTags {
              uniqueId
              name
              color
            }
            zones {
              uniqueId
              name
              color
            }
          }
          resources {
            uniqueId
            name
            color
            customTags {
              uniqueId
              name
              color
            }
            zones {
              uniqueId
              name
              color
            }
          }
        }
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const UpdateGlobalReloadId = useContext(UpdateGlobalReloadIdContext);
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const shortDateFormatFrom = toShortDate(bookingDetails.from);

  const moreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditBooking],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteBooking],
  ];

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditBooking:
        if (bookingDetails) {
          router.push(getOrganizationBookingBaseLink(organizationId, bookingDetails.id));
        }

        break;

      case MoreActionsMenuOptionType.DeleteBooking:
        handleRemoveBookingClick();
        break;
    }
  };

  const handleRemoveBookingClick = () => {
    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.customer)}`;
    if (bookingDetails.location) {
      bookingDetailsInfo += ` at the "${bookingDetails.location!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    const toastId = themedToast(<NotificationContent content={`Removing booking '${bookingDetailsInfo}'...`} />, infoNotificationOptions);

    commitDeleteBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id: bookingDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove booking ${bookingDetailsInfo}. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Booking ${bookingDetailsInfo} removed.`} />,
        });
        UpdateGlobalReloadId();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove booking ${bookingDetailsInfo}.`} />,
        });
      },
    });
  };

  const handleJoinClick = () => {
    if (!rootData.me) {
      return;
    }

    const id = nanoid();
    const toastId = themedToast(<NotificationContent content={`Joining booking on '${shortDateFormatFrom}'...`} />, infoNotificationOptions);
    const type = 'WorkingFromOffice';

    commitAddBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id,
          customerId: rootData.me.id,
          from: bookingDetails.from,
          until: bookingDetails.until,
          organizationId: bookingDetails.organization?.uniqueId,
          locationId: bookingDetails.location?.uniqueId,
          teamId: bookingDetails.team?.uniqueId,
          deskIds: [],
          roomIds: [],
          resourceIds: [],
          type,
        },
      },
      onCompleted: (response, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to make a booking '${shortDateFormatFrom}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        const booking = response.addBooking?.booking!;
        let message = `Booking made for ${getCustomerFullName(booking.customer)} to work`;

        if (booking.location) {
          message += ` from the "${booking.location!.name}"`;
        }

        if (booking.desks.length > 0) {
          message += ` at desk "${booking.desks.map(({ name }) => name).join(', ')}"`;

          const zones = booking.desks.flatMap(({ zones }) => zones);
          if (zones.length > 0) {
            const uniqueZones = Array.from(zones.reduce((map, zone) => map.set(zone.uniqueId, zone), new Map()).values());

            message += ` in "${uniqueZones.map(({ name }) => name).join(', ')}"`;
          }
        }

        if (booking.rooms.length > 0) {
          message += ` at room "${booking.rooms.map(({ name }) => name).join(', ')}"`;

          const zones = booking.rooms.flatMap(({ zones }) => zones);
          if (zones.length > 0) {
            const uniqueZones = Array.from(zones.reduce((map, zone) => map.set(zone.uniqueId, zone), new Map()).values());

            message += ` in "${uniqueZones.map(({ name }) => name).join(', ')}"`;
          }
        }

        message += ` on ${toShortDate(booking.from)}.`;

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={message} />,
        });

        UpdateGlobalReloadId();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to make a booking '${shortDateFormatFrom}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addBooking: {
          booking: {
            id,
            from: bookingDetails.from,
            until: bookingDetails.until,
            notes: null,
            type,
            customer: {
              uniqueId: rootData.me.id,
              name: rootData.me.name,
              givenName: rootData.me.givenName,
              middleName: rootData.me.middleName,
              familyName: rootData.me.familyName,
              photoUrl: rootData.me.photoUrl,
            },
            location: bookingDetails.location
              ? {
                  uniqueId: bookingDetails.location.uniqueId,
                  name: bookingDetails.location.name,
                }
              : null,
            team: bookingDetails.team
              ? {
                  uniqueId: bookingDetails.team.uniqueId,
                  name: bookingDetails.team.name,
                }
              : null,
            desks: [],
            rooms: [],
            resources: [],
          },
        },
      },
    });
  };

  const date = dayjs(bookingDetails.from);
  const customTags = bookingDetails.desks
    .flatMap(({ customTags }) => customTags)
    .concat(bookingDetails.rooms.flatMap(({ customTags }) => customTags))
    .concat(bookingDetails.resources.flatMap(({ customTags }) => customTags))
    .reduce((acc: CustomTagDetails[], customTag) => {
      if (!acc.some((item) => item.uniqueId === customTag.uniqueId)) {
        acc.push(customTag);
      }

      return acc;
    }, []);
  const zones = bookingDetails.desks
    .flatMap(({ zones }) => zones)
    .concat(bookingDetails.rooms.flatMap(({ zones }) => zones))
    .concat(bookingDetails.resources.flatMap(({ zones }) => zones))
    .reduce((acc: ZoneDetails[], zone) => {
      if (!acc.some((item) => item.uniqueId === zone.uniqueId)) {
        acc.push(zone);
      }

      return acc;
    }, []);

  return (
    <>
      <Card sx={{ width: { xs: '100%', sm: 380 } }}>
        <CardHeader
          title={
            <StackRow>
              <Link component={NextLink} href={getOrganizationBookingBaseLink(organizationId, bookingDetails.id)}>
                <LeadIconTypography startElement={<LocationIcon />} label={bookingDetails.location?.name} sx={{ flexWrap: undefined }} invertDefaultColor />
              </Link>

              <PushToRight />
              {canJoinBooking && (
                <Box color={paletteMode === 'dark' ? coal : sandstone}>
                  <IconButton onClick={handleJoinClick} color="inherit">
                    <JoinIcon />
                  </IconButton>
                </Box>
              )}
            </StackRow>
          }
          action={
            <>
              {moreActionsOption.length > 0 && (
                <Box color={paletteMode === 'dark' ? coal : sandstone} sx={{ paddingTop: 0.5 }}>
                  <IconButton onClick={handleMoreActionsMenuClick} color="inherit">
                    <EllipseMenuIcon />
                  </IconButton>
                </Box>
              )}
            </>
          }
        />
        <CardContent>
          <SmallIconTypography startElement={<CalendarIcon />} label={toShortDateWithAdditionalDayInfo(date)} sx={{ paddingTop: 1, paddingBottom: 1 }} />
          <Divider />
          <SmallIconTypography
            label={getCustomerFullName(bookingDetails.customer)}
            startElement={<CustomerAvatar name={bookingDetails.customer} photo={{ url: bookingDetails.customer?.photoUrl }} size="small" />}
            sx={{ paddingTop: 1, paddingBottom: 1 }}
          />
          <Divider />
          <SmallIconTypography startElement={<TeamIcon />} label={bookingDetails.team ? bookingDetails.team.name : 'N/A'} sx={{ paddingTop: 1, paddingBottom: 1 }} />
          <Divider />
          <Desks desks={bookingDetails.desks.map((desk) => ({ id: desk.uniqueId, name: desk.name, color: desk.color }))} sx={{ paddingTop: 1, paddingBottom: 1 }} />
          <Divider />
          <Rooms rooms={bookingDetails.rooms.map((room) => ({ id: room.uniqueId, name: room.name, color: room.color }))} sx={{ paddingTop: 1, paddingBottom: 1 }} />
          <Divider />
          <Resources
            resources={bookingDetails.resources.map((resource) => ({ id: resource.uniqueId, name: resource.name, color: resource.color }))}
            sx={{ paddingTop: 1, paddingBottom: 1 }}
          />
          <Divider />
          <CustomTags
            customTags={customTags.map((customTag: CustomTagDetails) => ({ id: customTag.uniqueId, name: customTag.name, color: customTag.color }))}
            sx={{ paddingTop: 1, paddingBottom: 1 }}
          />
          <Divider />
          <Zones zones={zones.map((zone: ZoneDetails) => ({ id: zone.uniqueId, name: zone.name, color: zone.color }))} sx={{ paddingTop: 1, paddingBottom: 1 }} />
          <Divider />
          <SmallIconTypography startElement={<NotesIcon />} label={bookingDetails.notes ? bookingDetails.notes : 'N/A'} sx={{ paddingTop: 1, paddingBottom: 1 }} />
        </CardContent>
      </Card>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />
    </>
  );
};

export default memo(BookingCard);

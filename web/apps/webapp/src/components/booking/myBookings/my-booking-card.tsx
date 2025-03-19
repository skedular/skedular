import { CustomerAvatar } from '@/components/avatars';
import { LeadIconTypography, SmallIconTypography, StackColumn, StackRow } from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { Desks } from '@/components/desk';
import { CalendarIcon, EllipseMenuIcon, LocationIcon, NotesIcon, TeamIcon } from '@/components/icons';
import { getOrganizationBookingBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import Resources from '@/components/resource/resources';
import { Rooms } from '@/components/room';
import { Zones } from '@/components/zone';
import { PaletteModeContext, UpdateGlobalReloadIdContext } from '@/libs/providers';
import { coal, sandstone } from '@/libs/theme';
import { dateRangeToShortDateWithAdditionalDayInfo, getCustomerFullName, joinErrors, toShortDate } from '@/libs/utils';
import type { myBookingCard_BookingDetails$key } from '@/queries/__generated__/myBookingCard_BookingDetails.graphql';
import type { myBookingCard_deleteBookingMutation } from '@/queries/__generated__/myBookingCard_deleteBookingMutation.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
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
  bookingDetailsRelay: myBookingCard_BookingDetails$key;
  organizationId: string;
  connectionIds: string[];
  otherTeammates: CustomerDetails[];
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

type CustomerDetails = {
  uniqueId: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
};

const MyBookingCard = ({ bookingDetailsRelay, organizationId, otherTeammates, connectionIds }: Props) => {
  const bookingDetails = useFragment(
    graphql`
      fragment myBookingCard_BookingDetails on BookingDetails {
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

  const [commitDeleteBooking] = useMutation<myBookingCard_deleteBookingMutation>(graphql`
    mutation myBookingCard_deleteBookingMutation($connectionIds: [ID!]!, $input: DeleteBookingInput!) {
      deleteBooking(input: $input) {
        booking {
          id @deleteEdge(connections: $connectionIds)
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
            <Link component={NextLink} href={getOrganizationBookingBaseLink(organizationId, bookingDetails.id)}>
              <LeadIconTypography startElement={<LocationIcon />} label={bookingDetails.location?.name} sx={{ flexWrap: undefined }} invertDefaultColor />
            </Link>
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
          <SmallIconTypography
            startElement={<CalendarIcon />}
            label={dateRangeToShortDateWithAdditionalDayInfo(dayjs(bookingDetails.from), dayjs(bookingDetails.until))}
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
          <Divider />
          <StackColumn sx={{ paddingTop: 1, paddingBottom: 1 }}>
            <SmallIconTypography label="Other teammates coming" />
            <StackRow>
              <AvatarGroup max={5}>
                {otherTeammates.map((item) => (
                  <CustomerAvatar key={item.uniqueId} name={item} photo={{ url: item.photoUrl }} size="medium" showFullName />
                ))}
              </AvatarGroup>
            </StackRow>
          </StackColumn>
        </CardContent>
      </Card>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />
    </>
  );
};

export default memo(MyBookingCard);

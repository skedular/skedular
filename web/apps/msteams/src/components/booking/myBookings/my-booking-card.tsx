import AvatarGroup from '@mui/material/AvatarGroup';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Box from '@mui/system/Box';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { LeadIconTypography, SmallIconTypography, StackColumn, StackRow } from '@repo/shared/components/commons';
import { CalendarIcon, DeleteIcon, DeskIcon, EditIcon, EllipseMenuIcon, LocationIcon, NotesIcon, TeamIcon } from '@repo/shared/components/icons';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { Zones } from '@repo/shared/components/zone';
import { PaletteModeContext, UpdateGlobalReloadIdContext } from '@repo/shared/libs/providers';
import { coal, sandstone } from '@repo/shared/libs/theme';
import { getCustomerFullName, joinErrors, toShortDate, toShortDateWithAdditionalDayInfo } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import dayjs from 'dayjs';
import { nanoid } from 'nanoid';
import type { JSX } from 'react';
import { memo, useContext, useState } from 'react';
import { useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import type { myBookingCard_BookingDetails$key } from './__generated__/myBookingCard_BookingDetails.graphql';
import type { myBookingCard_deleteBookingMutation } from './__generated__/myBookingCard_deleteBookingMutation.graphql';

type Props = {
  bookingDetailsRelay: myBookingCard_BookingDetails$key;
  connectionIds: string[];
  otherTeammates: CustomerDetails[];
};

type ZoneDetails = {
  uniqueId: string;
  name: string | null | undefined;
  tagType?: string | null | undefined;
};

type CustomerDetails = {
  uniqueId: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
};

enum MoreActionsMenuOptionType {
  EditBooking,
  DeleteBooking,
}

type MoreActionsMenuItemType = {
  id: MoreActionsMenuOptionType;
  label: string;
  icon: JSX.Element;
};

const moreActionsMenuAllOptions: Record<MoreActionsMenuOptionType, MoreActionsMenuItemType> = {
  [MoreActionsMenuOptionType.EditBooking]: {
    id: MoreActionsMenuOptionType.EditBooking,
    label: 'Edit Booking',
    icon: <EditIcon color="primary" />,
  },
  [MoreActionsMenuOptionType.DeleteBooking]: {
    id: MoreActionsMenuOptionType.DeleteBooking,
    label: 'Delete',
    icon: <DeleteIcon color="warning" />,
  },
};

const MyBookingCard = ({ bookingDetailsRelay, otherTeammates, connectionIds }: Props) => {
  const bookingDetails = useFragment(
    graphql`
      fragment myBookingCard_BookingDetails on BookingDetails {
        id
        from
        to
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
          deskTypes {
            uniqueId
            name
          }
          zones {
            uniqueId
            name
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
        break;

      case MoreActionsMenuOptionType.DeleteBooking:
        handleDeleteClick();
        break;
    }
  };

  const handleDeleteClick = () => {
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

  const date = dayjs(bookingDetails.from);
  const desks = bookingDetails.desks.map((desk) => desk.name).join(', ');
  const zones = bookingDetails.desks
    .flatMap(({ zones }) => zones)
    .reduce((acc: ZoneDetails[], zone) => {
      if (!acc.some((item) => item.uniqueId === zone.uniqueId)) {
        acc.push(zone);
      }

      return acc;
    }, []);

  return (
    <>
      <Card sx={{ width: 315 }}>
        <CardHeader
          title={
            <LeadIconTypography
              startElement={<LocationIcon />}
              label={bookingDetails.location?.name}
              sx={{ flexWrap: undefined }}
              invertDefaultColor
            />
          }
          action={
            <>
              {moreActionsOption.length > 0 && (
                <Box color={paletteMode === 'dark' ? coal : sandstone}>
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
            label={toShortDateWithAdditionalDayInfo(date)}
            sx={{ paddingTop: 1, paddingBottom: 1 }}
          />
          <Divider />
          <SmallIconTypography
            startElement={<TeamIcon />}
            label={bookingDetails.team ? bookingDetails.team.name : 'N/A'}
            sx={{ paddingTop: 1, paddingBottom: 1 }}
          />
          <Divider />
          <SmallIconTypography startElement={<DeskIcon />} label={desks.length === 0 ? 'N/A' : desks} sx={{ paddingTop: 1, paddingBottom: 1 }} />
          <Divider />
          <Zones zones={zones.map((zone: ZoneDetails) => ({ id: zone.uniqueId, name: zone.name }))} />
          <Divider />
          <SmallIconTypography
            startElement={<NotesIcon />}
            label={bookingDetails.notes ? bookingDetails.notes : 'N/A'}
            sx={{ paddingTop: 1, paddingBottom: 1 }}
          />
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

      <Menu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onClose={handleMoreActionsMenuItemClick}>
        {moreActionsOption.map((option) => (
          <MenuItem key={option.id} onClick={() => handleMoreActionsMenuItemClick(option.id)}>
            <SmallIconTypography label={option.label} startElement={option.icon} />
          </MenuItem>
        ))}
      </Menu>
    </>
  );
};

export default memo(MyBookingCard);

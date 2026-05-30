import { CustomerAvatar } from '@/components/avatars';
import RecurringBookingDeleteConfirmationDialog from '@/components/booking/recurring-booking-delete-confirmation-dialog';
import { CustomTags } from '@/components/customTag';
import { CalendarIcon, EllipseMenuIcon, NotesIcon, TeamIcon } from '@/components/icons';
import { getOrganizationBookingBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import Resources from '@/components/resource/resources';
import { Zones } from '@/components/zone';
import type { myBookingCard_BookingDetails$key } from '@/queries/__generated__/myBookingCard_BookingDetails.graphql';
import type { myBookingCard_deletePrivateBookingMutation } from '@/queries/__generated__/myBookingCard_deletePrivateBookingMutation.graphql';
import type { myBookingCard_deletePrivateRecurringBookingMutation } from '@/queries/__generated__/myBookingCard_deletePrivateRecurringBookingMutation.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Tooltip from '@mui/material/Tooltip';
import type { SxProps, Theme } from '@mui/system';
import Box from '@mui/system/Box';
import { dateRangeToShortDateWithAdditionalDayInfo, getCustomerFullName, getRelayErrorMessage, PaletteModeContext, toShortDate, useIntegratedPlatrform } from '@skedular/shared';
import { CaptionIconTypography, coal, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@skedular/ui';
import dayjs from 'dayjs';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  bookingDetailsRelay: myBookingCard_BookingDetails$key;
  organizationCustomDomain: string;
  connectionIds: string[];
  otherTeammates: CustomerDetails[];
};

type CustomTagDetails = {
  id: string;
  name: string | null | undefined;
  color?: string | null | undefined;
};

type ZoneDetails = {
  id: string;
  name: string | null | undefined;
  color?: string | null | undefined;
};

type PendingRecurringDeleteAction = 'occurrence' | 'series' | null;

type CustomerDetails = {
  id: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
};

const sectionSx: SxProps<Theme> = {
  border: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
  borderRadius: 3,
  p: 1.25,
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.02)' : 'transparent'),
};

const MyBookingCard = ({ bookingDetailsRelay, organizationCustomDomain, otherTeammates, connectionIds }: Props) => {
  const bookingDetails = useFragment(
    graphql`
      fragment myBookingCard_BookingDetails on BookingDetails {
        id
        from
        until
        notes
        channel {
          channel
        }
        involvedCustomers {
          id
          name
          givenName
          middleName
          familyName
          photoUrl
        }
        involvedLocations {
          uniqueId
          name
        }
        involvedTeams {
          id
          name
        }
        bookingResources {
          resource {
            id
            name
            color
            customTags {
              id
              name
              color
            }
            zones {
              id
              name
              color
            }
          }
        }
        recurringBooking {
          id
          startDate
          endDate
          frequency {
            name
          }
        }
      }
    `,
    bookingDetailsRelay,
  );

  const [commitDeletePrivateBooking] = useMutation<myBookingCard_deletePrivateBookingMutation>(graphql`
    mutation myBookingCard_deletePrivateBookingMutation($connectionIds: [ID!]!, $input: DeletePrivateBookingInput!) {
      deletePrivateBooking(input: $input) {
        booking {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitDeletePrivateRecurringBooking] = useMutation<myBookingCard_deletePrivateRecurringBookingMutation>(graphql`
    mutation myBookingCard_deletePrivateRecurringBookingMutation($input: DeletePrivateRecurringBookingInput!) {
      deletePrivateRecurringBooking(input: $input) {
        recurringBooking {
          id
        }
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const [pendingRecurringDeleteAction, setPendingRecurringDeleteAction] = useState<PendingRecurringDeleteAction>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const shortDateFormatFrom = toShortDate(bookingDetails.from);
  const recurringBooking = bookingDetails.recurringBooking;
  const canDeleteRecurringOccurrence = !!recurringBooking && bookingDetails.channel.channel === 'PRIVATE';
  const canEditRecurringSeries = canDeleteRecurringOccurrence;
  const recurringSeriesLabel = recurringBooking ? `${recurringBooking.frequency.name} recurring booking` : null;
  const recurringSeriesDateLabel = recurringBooking
    ? recurringBooking.endDate
      ? `${toShortDate(recurringBooking.startDate)} - ${toShortDate(recurringBooking.endDate)}`
      : `Starts ${toShortDate(recurringBooking.startDate)}`
    : null;
  const bookingDateRange = dateRangeToShortDateWithAdditionalDayInfo(dayjs(bookingDetails.from), dayjs(bookingDetails.until));
  const recurringSeriesActionLabel = recurringBooking ? 'Remove recurring series' : null;
  const recurringOccurrenceActionLabel = canDeleteRecurringOccurrence ? 'Remove this occurrence' : null;
  const recurringDeleteConfirmationMessage = recurringBooking
    ? `This booking is part of a recurring series. If you continue, the full recurring series will be removed, not just this booking.`
    : null;
  const recurringOccurrenceDeleteConfirmationMessage = canDeleteRecurringOccurrence
    ? 'Only this booking will be removed. The rest of the recurring series will stay active.'
    : null;
  const recurringDeleteDialogTitle = pendingRecurringDeleteAction === 'occurrence' ? 'Remove This Booking' : 'Remove Recurring Series';
  const recurringDeleteDialogDescription = pendingRecurringDeleteAction === 'occurrence' ? recurringOccurrenceDeleteConfirmationMessage : recurringDeleteConfirmationMessage;
  const recurringDeleteDialogPrimaryLabel = pendingRecurringDeleteAction === 'occurrence' ? 'Remove this booking' : 'Remove series';

  const moreActionsOption: MoreActionsMenuItemType[] = [
    canEditRecurringSeries
      ? {
          ...moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditBooking],
          label: 'Edit this occurrence',
        }
      : moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditBooking],
  ];

  if (canEditRecurringSeries) {
    moreActionsOption.push(moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditRecurringBooking]);
  }

  if (recurringOccurrenceActionLabel) {
    moreActionsOption.push({
      ...moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteBooking],
      label: recurringOccurrenceActionLabel,
    });
  } else if (!recurringBooking) {
    moreActionsOption.push(moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteBooking]);
  }

  if (recurringBooking && recurringSeriesActionLabel) {
    moreActionsOption.push({
      ...moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteRecurringBooking],
      label: recurringSeriesActionLabel,
    });
  }

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditBooking:
        router.push(getOrganizationBookingBaseLink(integratedPlatrform, organizationCustomDomain, bookingDetails.id));
        break;

      case MoreActionsMenuOptionType.EditRecurringBooking:
        router.push(getOrganizationBookingBaseLink(integratedPlatrform, organizationCustomDomain, bookingDetails.id, { editMode: 'recurring' }));
        break;

      case MoreActionsMenuOptionType.DeleteBooking:
        handleRemoveBookingClick();
        break;

      case MoreActionsMenuOptionType.DeleteRecurringBooking:
        handleRemoveRecurringBookingClick();
        break;
    }
  };

  const handleRemoveBookingClick = () => {
    if (recurringOccurrenceDeleteConfirmationMessage) {
      setPendingRecurringDeleteAction('occurrence');
      return;
    }

    removeBooking();
  };

  const handleCancelRecurringDeleteClick = () => {
    setPendingRecurringDeleteAction(null);
  };

  const handleConfirmRecurringDeleteClick = () => {
    const action = pendingRecurringDeleteAction;
    setPendingRecurringDeleteAction(null);

    if (action === 'occurrence') {
      removeBooking();
      return;
    }

    if (action === 'series') {
      removeRecurringBooking();
    }
  };

  const removeBooking = () => {
    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.involvedCustomers[0])}`;
    if (bookingDetails.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${bookingDetails.involvedLocations[0]!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    if (bookingDetails.channel.channel === 'PRIVATE') {
      commitDeletePrivateBooking({
        variables: {
          connectionIds,
          input: {
            clientMutationId: uuid(),
            id: bookingDetails.id,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            themedToast(<NotificationContent content={`We couldn't remove booking ${bookingDetailsInfo}. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

            return;
          }
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`We couldn't remove booking ${bookingDetailsInfo}. ${getRelayErrorMessage(error)}`} />, errorNotificationOptions);
        },
      });
    }
  };

  const handleRemoveRecurringBookingClick = () => {
    if (!recurringBooking || !recurringSeriesLabel || !recurringDeleteConfirmationMessage) {
      return;
    }

    setPendingRecurringDeleteAction('series');
  };

  const removeRecurringBooking = () => {
    if (!recurringBooking || !recurringSeriesLabel) {
      return;
    }

    commitDeletePrivateRecurringBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: recurringBooking.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't remove this recurring series. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        router.refresh();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't remove this recurring series. ${getRelayErrorMessage(error)}`} />, errorNotificationOptions);
      },
    });
  };

  const customTags = bookingDetails.bookingResources
    .flatMap(({ resource }) => resource.customTags)
    .reduce((acc: CustomTagDetails[], customTag) => {
      if (!acc.some((item) => item.id === customTag.id)) {
        acc.push(customTag);
      }

      return acc;
    }, []);
  const zones = bookingDetails.bookingResources
    .flatMap(({ resource }) => resource.zones)
    .reduce((acc: ZoneDetails[], zone) => {
      if (!acc.some((item) => item.id === zone.id)) {
        acc.push(zone);
      }

      return acc;
    }, []);

  const locationName =
    bookingDetails.involvedLocations
      .map((location) => location.name)
      .filter(Boolean)
      .join(', ') || 'Location pending';
  const teamName = bookingDetails.involvedTeams[0]?.name;

  return (
    <>
      <Card
        sx={{
          width: '100%',
          height: '100%',
          borderRadius: 4,
          border: 1,
          borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
          boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 10px 28px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
          backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.92)' : theme.palette.background.paper),
        }}
      >
        <CardContent sx={{ p: 2, height: '100%' }}>
          <StackColumn spacing={1.75} sx={{ height: '100%' }}>
            <StackRow sx={{ alignItems: 'flex-start', flexWrap: 'nowrap', gap: 2 }}>
              <StackColumn spacing={0.75} sx={{ minWidth: 0, flexGrow: 1 }}>
                <Tooltip title={locationName}>
                  <Link
                    component={NextLink}
                    href={getOrganizationBookingBaseLink(integratedPlatrform, organizationCustomDomain, bookingDetails.id)}
                    underline="none"
                    color="inherit"
                    sx={{ display: 'block', minWidth: 0, maxWidth: '100%' }}
                  >
                    <LeadIconTypography label={locationName} noWrap sx={{ minWidth: 0, maxWidth: '100%' }} />
                  </Link>
                </Tooltip>
                <StackColumn spacing={0.1} sx={{ minWidth: 0 }}>
                  <SmallIconTypography startElement={<CalendarIcon />} label={bookingDateRange.primaryLine} noWrap />
                  {bookingDateRange.secondaryLine ? <SmallIconTypography label={bookingDateRange.secondaryLine} noWrap sx={{ pl: 3.5 }} /> : null}
                </StackColumn>
              </StackColumn>

              {moreActionsOption.length > 0 ? (
                <IconButton onClick={handleMoreActionsMenuClick} aria-label="Open booking actions" sx={{ color: paletteMode === 'dark' ? 'inherit' : coal, mt: -0.25, mr: -0.5 }}>
                  <EllipseMenuIcon />
                </IconButton>
              ) : null}
            </StackRow>

            <Divider />

            <StackRow sx={{ gap: 1, flexWrap: 'wrap', alignItems: 'center' }}>
              {teamName ? <Chip label={teamName} size="small" icon={<TeamIcon />} /> : null}
              {recurringSeriesLabel ? (
                <Tooltip title={`${recurringSeriesLabel}. ${recurringSeriesDateLabel ?? ''}`.trim()}>
                  <Chip label="Recurring" size="small" variant="outlined" />
                </Tooltip>
              ) : null}
            </StackRow>

            <Box sx={sectionSx}>
              <StackColumn spacing={1}>
                <SubtitleIconTypography label="Booking details" />
                <Resources resources={bookingDetails.bookingResources.map((item) => ({ id: item.resource.id, name: item.resource.name, color: item.resource.color }))} hideNAText />
                <CustomTags customTags={customTags.map((customTag) => ({ id: customTag.id, name: customTag.name, color: customTag.color }))} hideNAText />
                <Zones zones={zones.map((zone) => ({ id: zone.id, name: zone.name, color: zone.color }))} hideNAText />
              </StackColumn>
            </Box>

            {bookingDetails.notes || otherTeammates.length > 0 ? (
              <Box sx={sectionSx}>
                <StackColumn spacing={1}>
                  {bookingDetails.notes ? <CaptionIconTypography startElement={<NotesIcon />} label={bookingDetails.notes} /> : null}
                  {otherTeammates.length > 0 ? (
                    <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center', gap: 1 }}>
                      <SmallIconTypography label="Other teammates" />
                      <AvatarGroup max={5}>
                        {otherTeammates.map((item) => (
                          <CustomerAvatar key={item.id} name={item} photo={{ url: item.photoUrl }} size="medium" showFullName />
                        ))}
                      </AvatarGroup>
                    </StackRow>
                  ) : null}
                </StackColumn>
              </Box>
            ) : null}

            <Box sx={{ flexGrow: 1 }} />
          </StackColumn>
        </CardContent>
      </Card>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />

      {recurringDeleteDialogDescription ? (
        <RecurringBookingDeleteConfirmationDialog
          open={pendingRecurringDeleteAction !== null}
          title={recurringDeleteDialogTitle}
          description={recurringDeleteDialogDescription}
          confirmLabel={recurringDeleteDialogPrimaryLabel}
          onConfirm={handleConfirmRecurringDeleteClick}
          onCancel={handleCancelRecurringDeleteClick}
        />
      ) : null}
    </>
  );
};

export default memo(MyBookingCard);

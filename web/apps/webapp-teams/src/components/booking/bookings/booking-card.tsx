import { CustomerAvatar } from '@/components/avatars';
import RecurringBookingDeleteConfirmationDialog from '@/components/booking/recurring-booking-delete-confirmation-dialog';
import { CaptionIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@skedular/ui';
import { CustomTags } from '@/components/customTag';
import { CalendarIcon, EllipseMenuIcon, JoinIcon, NotesIcon, TeamIcon } from '@/components/icons';
import { getOrganizationBookingBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { Resources } from '@/components/resource';
import { Zones } from '@/components/zone';
import { PaletteModeContext, useIntegratedPlatrform } from '@skedular/shared';
import { coal } from '@skedular/ui';
import { dateRangeToShortDateWithAdditionalDayInfo, getCustomerFullName, getRelayErrorMessage, toShortDate } from '@skedular/shared';
import type { bookingCard_addPrivateBookingMutation } from '@/queries/__generated__/bookingCard_addPrivateBookingMutation.graphql';
import type { bookingCard_BookingDetails$key } from '@/queries/__generated__/bookingCard_BookingDetails.graphql';
import type { bookingCard_deletePrivateBookingMutation } from '@/queries/__generated__/bookingCard_deletePrivateBookingMutation.graphql';
import type { bookingCard_deletePrivateRecurringBookingMutation } from '@/queries/__generated__/bookingCard_deletePrivateRecurringBookingMutation.graphql';
import type { bookingCard_query$key } from '@/queries/__generated__/bookingCard_query.graphql';
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
import dayjs from 'dayjs';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  rootDataRelay: bookingCard_query$key;
  bookingDetailsRelay: bookingCard_BookingDetails$key;
  organizationCustomDomain: string;
  connectionIds: string[];
  canJoinBooking: boolean;
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

const sectionSx: SxProps<Theme> = {
  border: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
  borderRadius: 3,
  p: 1.25,
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.02)' : 'transparent'),
};

const BookingCard = ({ rootDataRelay, bookingDetailsRelay, organizationCustomDomain, connectionIds, canJoinBooking }: Props) => {
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
        category {
          category
          name
        }
        channel {
          channel
          name
        }
        involvedCustomers {
          id
          name
          givenName
          middleName
          familyName
          photoUrl
        }
        involvedOrganizations {
          id
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

  const [commitDeletePrivateBooking] = useMutation<bookingCard_deletePrivateBookingMutation>(graphql`
    mutation bookingCard_deletePrivateBookingMutation($connectionIds: [ID!]!, $input: DeletePrivateBookingInput!) {
      deletePrivateBooking(input: $input) {
        booking {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitDeletePrivateRecurringBooking] = useMutation<bookingCard_deletePrivateRecurringBookingMutation>(graphql`
    mutation bookingCard_deletePrivateRecurringBookingMutation($input: DeletePrivateRecurringBookingInput!) {
      deletePrivateRecurringBooking(input: $input) {
        recurringBooking {
          id
        }
      }
    }
  `);

  const [commitAddPrivateBooking] = useMutation<bookingCard_addPrivateBookingMutation>(graphql`
    mutation bookingCard_addPrivateBookingMutation($connectionIds: [ID!]!, $input: AddPrivateBookingInput!) @raw_response_type {
      addPrivateBooking(input: $input) {
        booking @appendNode(connections: $connectionIds, edgeTypeName: "BookingDetails") {
          id
          from
          until
          notes
          channel {
            channel
          }
          category {
            category
            name
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
    ? 'This booking is part of a recurring series. If you continue, the full recurring series will be removed, not just this booking.'
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

    const toastId = themedToast(<NotificationContent content={`Removing booking ${bookingDetailsInfo}...`} />, infoNotificationOptions);

    commitDeletePrivateBooking({
      variables: { connectionIds, input: { clientMutationId: uuid(), id: bookingDetails.id } },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't remove booking ${bookingDetailsInfo}. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: (
            <NotificationContent content={`Booking ${bookingDetailsInfo} has been removed.${canDeleteRecurringOccurrence ? ' The rest of the recurring series will stay active.' : ''}`} />
          ),
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't remove booking ${bookingDetailsInfo}. ${getRelayErrorMessage(error)}`} />,
        });
      },
    });
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

    const toastId = themedToast(<NotificationContent content={`Removing ${recurringSeriesLabel.toLowerCase()}...`} />, infoNotificationOptions);

    commitDeletePrivateRecurringBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: recurringBooking.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't remove this recurring series. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`${recurringSeriesLabel} has been removed. This applies to the full series, not just this booking.`} />,
        });
        router.refresh();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't remove this recurring series. ${getRelayErrorMessage(error)}`} />,
        });
      },
    });
  };

  const handleJoinClick = () => {
    const id = uuid();
    const toastId = themedToast(<NotificationContent content={`Joining booking on '${shortDateFormatFrom}'...`} />, infoNotificationOptions);

    commitAddPrivateBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          id,
          from: bookingDetails.from,
          until: bookingDetails.until,
          customerIds: [rootData.me.id],
          organizationIds: bookingDetails.involvedOrganizations.map(({ id }) => id),
          teamIds: bookingDetails.involvedTeams.map(({ id }) => id),
          resourceIds: [],
          category: bookingDetails.category.category,
        },
      },
      onCompleted: (response, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to make a booking '${shortDateFormatFrom}'. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        const booking = response.addPrivateBooking?.booking;
        let message = `Booking made for ${getCustomerFullName(booking.involvedCustomers[0])} to work`;

        if (booking.involvedLocations.length > 0) {
          message += ` from the "${booking.involvedLocations[0]!.name}"`;
        }

        if (booking.bookingResources.length > 0) {
          message += ` at resource "${booking.bookingResources.map(({ resource }) => resource.name).join(', ')}"`;

          const zones = booking.bookingResources.flatMap(({ resource }) => resource.zones);
          if (zones.length > 0) {
            const uniqueZones = Array.from(zones.reduce((map, zone) => map.set(zone.id, zone), new Map()).values());
            message += ` in "${uniqueZones.map(({ name }) => name).join(', ')}"`;
          }
        }

        message += ` on ${toShortDate(booking.from)}.`;

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={message} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to make a booking '${shortDateFormatFrom}'. Error: ${getRelayErrorMessage(error)}.`} />,
        });
      },
      optimisticResponse: {
        addPrivateBooking: {
          booking: {
            id,
            from: bookingDetails.from,
            until: bookingDetails.until,
            notes: null,
            channel: { channel: 'PRIVATE' },
            category: {
              category: bookingDetails.category.category,
              name: bookingDetails.category.name,
            },
            involvedCustomers: [
              {
                id: rootData.me.id,
                name: rootData.me.name,
                givenName: rootData.me.givenName,
                middleName: rootData.me.middleName,
                familyName: rootData.me.familyName,
                photoUrl: rootData.me.photoUrl,
              },
            ],
            involvedLocations: [],
            involvedTeams: [],
            bookingResources: [],
          },
        },
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
          <StackColumn spacing={1.25} sx={{ height: '100%' }}>
            <StackRow sx={{ alignItems: 'flex-start', flexWrap: 'nowrap', gap: 1 }}>
              <StackColumn spacing={0.25} sx={{ minWidth: 0, flexGrow: 1 }}>
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

              {canJoinBooking ? (
                <IconButton onClick={handleJoinClick} aria-label="Join booking" sx={{ color: paletteMode === 'dark' ? 'inherit' : coal, mt: -0.25 }}>
                  <JoinIcon />
                </IconButton>
              ) : null}

              {moreActionsOption.length > 0 ? (
                <IconButton onClick={handleMoreActionsMenuClick} aria-label="Open booking actions" sx={{ color: paletteMode === 'dark' ? 'inherit' : coal, mt: -0.25, mr: -0.5 }}>
                  <EllipseMenuIcon />
                </IconButton>
              ) : null}
            </StackRow>

            <StackRow sx={{ gap: 1, flexWrap: 'wrap', alignItems: 'center' }}>
              {teamName ? <Chip label={teamName} size="small" icon={<TeamIcon />} /> : null}
              {recurringSeriesLabel ? (
                <Tooltip title={`${recurringSeriesLabel}. ${recurringSeriesDateLabel ?? ''}`.trim()}>
                  <Chip label="Recurring" size="small" variant="outlined" />
                </Tooltip>
              ) : null}
            </StackRow>

            <StackRow sx={{ alignItems: 'center', gap: 1, minWidth: 0 }}>
              <AvatarGroup max={5}>
                {bookingDetails.involvedCustomers.map((item) => (
                  <CustomerAvatar key={item.id} name={item} photo={{ url: item.photoUrl }} size="medium" showFullName />
                ))}
              </AvatarGroup>
              <Tooltip title={bookingDetails.involvedCustomers.map((item) => getCustomerFullName(item)).join(', ')}>
                <Box sx={{ minWidth: 0, flexGrow: 1 }}>
                  <SmallIconTypography label={bookingDetails.involvedCustomers.map((item) => getCustomerFullName(item)).join(', ')} noWrap sx={{ minWidth: 0 }} />
                </Box>
              </Tooltip>
            </StackRow>

            <Divider />

            <Box sx={sectionSx}>
              <StackColumn spacing={1}>
                <SubtitleIconTypography label="Booking details" />
                <Resources resources={bookingDetails.bookingResources.map((item) => ({ id: item.resource.id, name: item.resource.name, color: item.resource.color }))} hideNAText />
                <CustomTags customTags={customTags.map((customTag) => ({ id: customTag.id, name: customTag.name, color: customTag.color }))} hideNAText />
                <Zones zones={zones.map((zone) => ({ id: zone.id, name: zone.name, color: zone.color }))} hideNAText />
                {bookingDetails.notes ? <CaptionIconTypography startElement={<NotesIcon />} label={bookingDetails.notes} /> : null}
              </StackColumn>
            </Box>

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

export default memo(BookingCard);

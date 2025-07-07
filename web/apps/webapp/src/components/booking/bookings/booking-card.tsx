import { CustomerAvatar } from '@/components/avatars';
import { LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { CalendarIcon, EllipseMenuIcon, JoinIcon, LocationIcon, NotesIcon, PaymentStatusIcon, TeamIcon } from '@/components/icons';
import { getOrganizationBookingBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { Resources } from '@/components/resource';
import { Zones } from '@/components/zone';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { coal, sandstone } from '@/libs/theme';
import { dateRangeToShortDateWithAdditionalDayInfo, getCustomerFullName, joinErrors, toShortDate } from '@/libs/utils';
import type { bookingCard_addBookingMutation } from '@/queries/__generated__/bookingCard_addBookingMutation.graphql';
import type { bookingCard_BookingDetails$key } from '@/queries/__generated__/bookingCard_BookingDetails.graphql';
import type { bookingCard_confirmBookingPaymentMutation } from '@/queries/__generated__/bookingCard_confirmBookingPaymentMutation.graphql';
import type { bookingCard_deleteBookingMutation } from '@/queries/__generated__/bookingCard_deleteBookingMutation.graphql';
import type { bookingCard_makeBookingPaymentNotRequiredMutation } from '@/queries/__generated__/bookingCard_makeBookingPaymentNotRequiredMutation.graphql';
import type { bookingCard_query$key } from '@/queries/__generated__/bookingCard_query.graphql';
import type { bookingCard_rejectBookingPaymentMutation } from '@/queries/__generated__/bookingCard_rejectBookingPaymentMutation.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
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
        organizationBookingPermissions(organizationId: $organizationId) {
          canModifyPaymentMethod
        }
        paymentStatuses {
          type
          name
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
        type {
          type
          name
        }
        involvedCustomers {
          uniqueId
          name
          givenName
          middleName
          familyName
          photoUrl
        }
        involvedOrganizations {
          uniqueId
        }
        involvedLocations {
          uniqueId
          name
        }
        involvedTeams {
          uniqueId
          name
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
        isPaymentRequired
        paymentStatus {
          type
          name
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
          type {
            type
            name
          }
          involvedCustomers {
            uniqueId
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
            uniqueId
            name
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

  const [commitConfirmBookingPayment] = useMutation<bookingCard_confirmBookingPaymentMutation>(graphql`
    mutation bookingCard_confirmBookingPaymentMutation($input: ConfirmBookingPaymentInput!) @raw_response_type {
      confirmBookingPayment(input: $input) {
        booking {
          id
          paymentStatus {
            type
            name
          }
        }
      }
    }
  `);

  const [commitRejectBookingPayment] = useMutation<bookingCard_rejectBookingPaymentMutation>(graphql`
    mutation bookingCard_rejectBookingPaymentMutation($input: RejectBookingPaymentInput!) @raw_response_type {
      rejectBookingPayment(input: $input) {
        booking {
          id
          paymentStatus {
            type
            name
          }
        }
      }
    }
  `);

  const [commitMakeBookingPaymentNotRequired] = useMutation<bookingCard_makeBookingPaymentNotRequiredMutation>(graphql`
    mutation bookingCard_makeBookingPaymentNotRequiredMutation($input: MakeBookingPaymentNotRequiredInput!) @raw_response_type {
      makeBookingPaymentNotRequired(input: $input) {
        booking {
          id
          paymentStatus {
            type
            name
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
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const shortDateFormatFrom = toShortDate(bookingDetails.from);

  const moreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditBooking],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteBooking],
  ];

  if (
    rootData.organizationBookingPermissions.canModifyPaymentMethod &&
    bookingDetails.isPaymentRequired &&
    bookingDetails.paymentStatus.type !== 'REJECTED' &&
    bookingDetails.paymentStatus.type !== 'EXPIRED' &&
    bookingDetails.paymentStatus.type !== 'RECORD_NEVER_CREATED'
  ) {
    moreActionsOption.push(
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.ConfirmBookingPayment],
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.RejectBookingPayment],
      moreActionsMenuAllOptions[MoreActionsMenuOptionType.MakeBookingPaymentNotRequired],
    );
  }

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditBooking:
        if (bookingDetails) {
          router.push(getOrganizationBookingBaseLink(integratedPlatrform, organizationId, bookingDetails.id));
        }

        break;

      case MoreActionsMenuOptionType.DeleteBooking:
        handleRemoveBookingClick();
        break;

      case MoreActionsMenuOptionType.ConfirmBookingPayment:
        handleConfirmPaymentClick();
        break;

      case MoreActionsMenuOptionType.RejectBookingPayment:
        handleRejectPaymentClick();
        break;

      case MoreActionsMenuOptionType.MakeBookingPaymentNotRequired:
        handleMakePaymentNotRequiredClick();
        break;
    }
  };

  const handleRemoveBookingClick = () => {
    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.involvedCustomers[0])}`;
    if (bookingDetails.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${bookingDetails.involvedLocations[0]!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    const toastId = themedToast(<NotificationContent content={`Removing booking '${bookingDetailsInfo}'...`} />, infoNotificationOptions);

    commitDeleteBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
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
    const id = uuid();
    const toastId = themedToast(<NotificationContent content={`Joining booking on '${shortDateFormatFrom}'...`} />, infoNotificationOptions);

    commitAddBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          id,
          from: bookingDetails.from,
          until: bookingDetails.until,
          customerIds: [rootData.me.id],
          organizationIds: bookingDetails.involvedOrganizations.map(({ uniqueId }) => uniqueId),
          teamIds: bookingDetails.involvedTeams.map(({ uniqueId }) => uniqueId),
          resourceIds: [],
          type: bookingDetails.type.type,
          lineItems: [],
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
        let message = `Booking made for ${getCustomerFullName(booking.involvedCustomers[0])} to work`;

        if (booking.involvedLocations.length > 0) {
          message += ` from the "${booking.involvedLocations[0]!.name}"`;
        }

        if (booking.resources.length > 0) {
          message += ` at resource "${booking.resources.map(({ name }) => name).join(', ')}"`;

          const zones = booking.resources.flatMap(({ zones }) => zones);
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
            type: {
              type: bookingDetails.type.type,
              name: bookingDetails.type.name,
            },
            involvedCustomers: [
              {
                uniqueId: rootData.me.id,
                name: rootData.me.name,
                givenName: rootData.me.givenName,
                middleName: rootData.me.middleName,
                familyName: rootData.me.familyName,
                photoUrl: rootData.me.photoUrl,
              },
            ],
            involvedLocations: [],
            involvedTeams: [],
            resources: [],
          },
        },
      },
    });
  };

  const handleConfirmPaymentClick = () => {
    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.involvedCustomers[0])}`;
    if (bookingDetails.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${bookingDetails.involvedLocations[0]!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    const toastId = themedToast(<NotificationContent content={`Confirming payment for booking '${bookingDetailsInfo}'...`} />, infoNotificationOptions);

    commitConfirmBookingPayment({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: bookingDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to confirm payment for booking ${bookingDetailsInfo}. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Booking ${bookingDetailsInfo} payment confirmed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to confirm payment for booking '${shortDateFormatFrom}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        confirmBookingPayment: {
          booking: {
            id: bookingDetails.id,
            paymentStatus: {
              type: 'CONFIRMED',
              name: rootData.paymentStatuses.find((status) => status.type === 'CONFIRMED')!.name,
            },
          },
        },
      },
    });
  };

  const handleRejectPaymentClick = () => {
    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.involvedCustomers[0])}`;
    if (bookingDetails.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${bookingDetails.involvedLocations[0]!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    const toastId = themedToast(<NotificationContent content={`Rejecting payment for booking '${bookingDetailsInfo}'...`} />, infoNotificationOptions);

    commitRejectBookingPayment({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: bookingDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to reject payment for booking ${bookingDetailsInfo}. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Booking ${bookingDetailsInfo} payment rejected.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to reject payment for booking '${shortDateFormatFrom}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        rejectBookingPayment: {
          booking: {
            id: bookingDetails.id,
            paymentStatus: {
              type: 'REJECTED',
              name: rootData.paymentStatuses.find((status) => status.type === 'REJECTED')!.name,
            },
          },
        },
      },
    });
  };

  const handleMakePaymentNotRequiredClick = () => {
    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.involvedCustomers[0])}`;
    if (bookingDetails.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${bookingDetails.involvedLocations[0]!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    const toastId = themedToast(<NotificationContent content={`Making payment for booking '${bookingDetailsInfo}' not required...`} />, infoNotificationOptions);

    commitMakeBookingPaymentNotRequired({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: bookingDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to make payment for booking ${bookingDetailsInfo} not required. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Booking ${bookingDetailsInfo} payment made not required.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to make payment for booking '${shortDateFormatFrom}' not required. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        makeBookingPaymentNotRequired: {
          booking: {
            id: bookingDetails.id,
            paymentStatus: {
              type: 'NO_PAYMENT_REQUIRED',
              name: rootData.paymentStatuses.find((status) => status.type === 'NO_PAYMENT_REQUIRED')!.name,
            },
          },
        },
      },
    });
  };

  const customTags = bookingDetails.resources
    .flatMap(({ customTags }) => customTags)
    .reduce((acc: CustomTagDetails[], customTag) => {
      if (!acc.some((item) => item.uniqueId === customTag.uniqueId)) {
        acc.push(customTag);
      }

      return acc;
    }, []);
  const zones = bookingDetails.resources
    .flatMap(({ zones }) => zones)
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
              <Link component={NextLink} href={getOrganizationBookingBaseLink(integratedPlatrform, organizationId, bookingDetails.id)}>
                {bookingDetails.involvedLocations.map((item) => (
                  <LeadIconTypography key={item.uniqueId} startElement={<LocationIcon />} label={item?.name} sx={{ flexWrap: undefined }} invertDefaultColor />
                ))}
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
          {bookingDetails.isPaymentRequired && (
            <>
              <SmallIconTypography startElement={<PaymentStatusIcon />} label={bookingDetails.paymentStatus.name} sx={{ paddingTop: 1, paddingBottom: 1 }} />
              <Divider />
            </>
          )}
          <SmallIconTypography
            startElement={<CalendarIcon />}
            label={dateRangeToShortDateWithAdditionalDayInfo(dayjs(bookingDetails.from), dayjs(bookingDetails.until))}
            sx={{ paddingTop: 1, paddingBottom: 1 }}
          />
          <Divider />
          {bookingDetails.involvedCustomers.map((item) => (
            <SmallIconTypography
              key={item.uniqueId}
              label={getCustomerFullName(item)}
              startElement={<CustomerAvatar name={item} photo={{ url: item.photoUrl }} size="small" />}
              sx={{ paddingTop: 1, paddingBottom: 1 }}
            />
          ))}
          <Divider />
          {bookingDetails.involvedTeams.length === 0 && <SmallIconTypography startElement={<TeamIcon />} label="N/A" sx={{ paddingTop: 1, paddingBottom: 1 }} />}
          {bookingDetails.involvedTeams.length > 0 &&
            bookingDetails.involvedTeams.map((item) => (
              <SmallIconTypography key={item.uniqueId} startElement={<TeamIcon />} label={item ? item.name : 'N/A'} sx={{ paddingTop: 1, paddingBottom: 1 }} />
            ))}
          <Divider />
          <Resources
            resources={bookingDetails.resources.map((resource) => ({
              id: resource.uniqueId,
              name: resource.name,
              color: resource.color,
            }))}
            sx={{ paddingTop: 1, paddingBottom: 1 }}
          />
          <Divider />
          <CustomTags
            customTags={customTags.map((customTag: CustomTagDetails) => ({
              id: customTag.uniqueId,
              name: customTag.name,
              color: customTag.color,
            }))}
            sx={{ paddingTop: 1, paddingBottom: 1 }}
          />
          <Divider />
          <Zones
            zones={zones.map((zone: ZoneDetails) => ({
              id: zone.uniqueId,
              name: zone.name,
              color: zone.color,
            }))}
            sx={{ paddingTop: 1, paddingBottom: 1 }}
          />
          <Divider />
          <SmallIconTypography startElement={<NotesIcon />} label={bookingDetails.notes ? bookingDetails.notes : 'N/A'} sx={{ paddingTop: 1, paddingBottom: 1 }} />
        </CardContent>
      </Card>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />
    </>
  );
};

export default memo(BookingCard);

import type { bookingCard_BookingDetails$key } from '@/queries/__generated__/bookingCard_BookingDetails.graphql';
import type { bookingCard_addBookingMutation } from '@/queries/__generated__/bookingCard_addBookingMutation.graphql';
import type { bookingCard_addCustomerDefaultDeskMutation } from '@/queries/__generated__/bookingCard_addCustomerDefaultDeskMutation.graphql';
import type { bookingCard_deleteBookingMutation } from '@/queries/__generated__/bookingCard_deleteBookingMutation.graphql';
import type { bookingCard_query$key } from '@/queries/__generated__/bookingCard_query.graphql';
import type { bookingCard_removeCustomerDefaultDeskMutation } from '@/queries/__generated__/bookingCard_removeCustomerDefaultDeskMutation.graphql';
import type { bookingCard_updateBookingMutation } from '@/queries/__generated__/bookingCard_updateBookingMutation.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import {
  CustomerIcon,
  DeleteIcon,
  DeskIcon,
  EditIcon,
  JoinIcon,
  LocationIcon,
  NotesIcon,
  NotPreferredIcon,
  OrganizationIcon,
  PreferredIcon,
  TeamIcon,
} from '@repo/shared/components/icons';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { ZonesLine } from '@repo/shared/components/zone';
import { PaletteModeContext, UpdateGlobalReloadIdContext } from '@repo/shared/libs/providers';
import { endOfDay, getCustomerFullName, joinErrors, toShortDate } from '@repo/shared/libs/utils';
import dayjs, { Dayjs } from 'dayjs';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useMemo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { array, date, object, string } from 'yup';
import BookingDate from './booking-date';
import BookingDetailsSelector from './booking-details-selector';
import BookingNotes from './booking-notes';

type Props = {
  rootDataRelay: bookingCard_query$key;
  bookingDetailsRelay: bookingCard_BookingDetails$key;
  connectionIds: string[];
  hideOrganizationControl: boolean;
  hideLocationControl: boolean;
  canJoinBooking: boolean;
};

type BookingDetails = {
  date: Date;
  member: string;
  notes: string;
  organization: string | undefined;
  location: string | undefined;
  desks: string[];
};

const bookingSchema = object({
  date: date().required(),
  member: string().required(),
  notes: string().notRequired(),
  organization: string().notRequired(),
  location: string().notRequired(),
  desk: array().nullable(),
});

const Booking = ({ rootDataRelay, bookingDetailsRelay, connectionIds, hideOrganizationControl, hideLocationControl, canJoinBooking }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment bookingCard_query on Query {
        me {
          id
          name
          givenName
          middleName
          familyName
          photoUrl
          preferredDesks {
            uniqueId
          }
        }
        myOrganizations {
          id
          name
        }
        myLocations(organizationId: $nullableOrganizationId) {
          id
          name
        }
        organizationBookingPermissions(organizationId: $organizationId) @include(if: $organizationExists) {
          canUpdateBookingOnBehalf
          canDeleteBookingOnBehalf
        }
        ...bookingDetailsSelector_query
        ...bookingDetailsSelector_organizationMembers_query
        ...bookingDetailsSelector_availableLocationDesks_query
      }
    `,
    rootDataRelay,
  );

  const bookingDetails = useFragment(
    graphql`
      fragment bookingCard_BookingDetails on BookingDetails {
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
        organization {
          uniqueId
          name
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

  const [commitAddBooking] = useMutation<bookingCard_addBookingMutation>(graphql`
    mutation bookingCard_addBookingMutation($connectionIds: [ID!]!, $input: AddBookingInput!) @raw_response_type {
      addBooking(input: $input) {
        booking @appendNode(connections: $connectionIds, edgeTypeName: "BookingDetails") {
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
          organization {
            uniqueId
            name
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
      }
    }
  `);

  const [commitUpdateBooking] = useMutation<bookingCard_updateBookingMutation>(graphql`
    mutation bookingCard_updateBookingMutation($input: UpdateBookingInput!) @raw_response_type {
      updateBooking(input: $input) {
        booking {
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
          organization {
            uniqueId
            name
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
      }
    }
  `);

  const [commitDeleteBooking] = useMutation<bookingCard_deleteBookingMutation>(graphql`
    mutation bookingCard_deleteBookingMutation($connectionIds: [ID!]!, $input: DeleteBookingInput!) {
      deleteBooking(input: $input) {
        booking {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultDesk] = useMutation<bookingCard_addCustomerDefaultDeskMutation>(graphql`
    mutation bookingCard_addCustomerDefaultDeskMutation($input: AddCustomerDefaultDeskInput!) {
      addCustomerDefaultDesk(input: $input) {
        customer {
          id
          preferredDesks {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerDefaultDesk] = useMutation<bookingCard_removeCustomerDefaultDeskMutation>(graphql`
    mutation bookingCard_removeCustomerDefaultDeskMutation($input: RemoveCustomerDefaultDeskInput!) {
      removeCustomerDefaultDesk(input: $input) {
        customer {
          id
          preferredDesks {
            uniqueId
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const UpdateGlobalReloadId = useContext(UpdateGlobalReloadIdContext);
  const [editing, setEditing] = useState(false);
  const validate = makeValidate(bookingSchema);
  const requiredFields = makeRequired(bookingSchema);
  const shortDateFormatFrom = toShortDate(bookingDetails.from);
  const [from, setFrom] = useState<Dayjs | Date>(dayjs(bookingDetails.from));
  const to = useMemo(() => (from instanceof Date ? endOfDay(dayjs(from)) : endOfDay(from)), [from]);

  const handleJoinClick = () => {
    if (!rootData.me) {
      return;
    }

    const id = nanoid();
    const toastId = themedToast(<NotificationContent content={`Joining booking on '${shortDateFormatFrom}'...`} />, infoNotificationOptions);

    commitAddBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id,
          customerId: rootData.me.id,
          from: bookingDetails.from,
          to: bookingDetails.to,
          organizationId: bookingDetails.organization?.uniqueId,
          locationId: bookingDetails.location?.uniqueId,
          teamId: bookingDetails.team?.uniqueId,
          deskIds: [],
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
            to: bookingDetails.to,
            notes: null,
            customer: {
              uniqueId: rootData.me.id,
              name: rootData.me.name,
              givenName: rootData.me.givenName,
              middleName: rootData.me.middleName,
              familyName: rootData.me.familyName,
              photoUrl: rootData.me.photoUrl,
            },
            organization: bookingDetails.organization
              ? {
                  uniqueId: bookingDetails.organization.uniqueId,
                  name: bookingDetails.organization.name,
                }
              : null,
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
          },
        },
      },
    });
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

  const handleEditClick = () => {
    setEditing(true);
  };

  const handleCancelClick = () => {
    setEditing(false);
  };

  const handleSaveClick = ({ date, member: memberId, notes, organization: organizationId, location: locationId, desks: deskIds }: BookingDetails) => {
    if (!rootData.me) {
      return;
    }

    const start = date as unknown as Dayjs;
    const from = start.toISOString();
    const to = endOfDay(start).toISOString();
    const shortDateTimeFormatFrom = toShortDate(start);

    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.customer)}`;
    if (bookingDetails.location) {
      bookingDetailsInfo += ` at the "${bookingDetails.location!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    const toastId = themedToast(<NotificationContent content={`Updating booking '${bookingDetailsInfo}'...`} />, infoNotificationOptions);

    commitUpdateBooking({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: bookingDetails.id,
          customerId: memberId,
          from,
          to,
          notes,
          organizationId,
          locationId,
          teamId: bookingDetails.team?.uniqueId,
          deskIds: locationId ? deskIds : [],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update booking '${shortDateTimeFormatFrom}'. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Booking ${bookingDetailsInfo} updated.`} />,
        });
        UpdateGlobalReloadId();
        setEditing(false);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update booking '${shortDateTimeFormatFrom}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateBooking: {
          booking: {
            id: bookingDetails.id,
            from,
            to,
            notes,
            customer: {
              uniqueId: '',
              name: '',
              givenName: '',
              middleName: '',
              familyName: '',
              photoUrl: '',
            },
            organization: null,
            location: null,
            team: null,
            // TODO: 20231202 - Morteza: Below line stores the existing/old desk, but not the updated value for optimistic update, update this line with the updated value in future
            desks: bookingDetails.desks,
          },
        },
      },
    });
  };

  const handleSetAsPreferredDeskClicked = (deskId: string, deskName: string) => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting '${deskName}' as your preferred desk...`} />, infoNotificationOptions);

    commitAddCustomerDefaultDesk({
      variables: {
        input: {
          clientMutationId: nanoid(),
          deskId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to set desk '${deskName}' as your preferred desk. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk '${deskName}' has been set as the preferred desk.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set desk '${deskName}' as your preferred desk. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addCustomerDefaultDesk: {
          customer: {
            id: rootData.me.id,
            preferredDesks: rootData.me.preferredDesks.concat([
              {
                uniqueId: deskId,
              },
            ]),
          },
        },
      },
    });
  };

  const handleRemoveAsPreferredDeskClicked = (deskId: string, deskName: string) => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing '${deskName}' as your preferred desk...`} />, infoNotificationOptions);

    commitRemoveCustomerDefaultDesk({
      variables: {
        input: {
          clientMutationId: nanoid(),
          deskId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the desk '${deskName}' as your preferred desk. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk '${deskName}' has been removed as your preferred desk.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the desk '${deskName}' as your preferred desk. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        removeCustomerDefaultDesk: {
          customer: {
            id: rootData.me.id,
            preferredDesks: rootData.me.preferredDesks.filter(({ uniqueId }) => uniqueId === deskId),
          },
        },
      },
    });
  };

  if (!rootData.me) {
    return <></>;
  }

  const canUpdateBooking =
    rootData.me.id === bookingDetails.customer?.uniqueId ||
    (rootData.organizationBookingPermissions && rootData.organizationBookingPermissions.canUpdateBookingOnBehalf);
  const canDeleteBooking =
    rootData.me.id === bookingDetails.customer?.uniqueId ||
    (rootData.organizationBookingPermissions && rootData.organizationBookingPermissions.canDeleteBookingOnBehalf);

  return (
    <>
      {!editing && (
        <Card elevation={24} sx={{ minWidth: 400, height: '100%' }}>
          <CardHeader
            title={
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                <CustomerAvatar name={bookingDetails.customer} photo={{ url: bookingDetails.customer?.photoUrl }} />
                <Typography variant="h6" component="div">
                  {shortDateFormatFrom}
                </Typography>
              </Stack>
            }
          />

          <CardContent>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <CustomerIcon />
              <Typography variant="h6" component="div">
                {getCustomerFullName(bookingDetails.customer)}
              </Typography>
            </Stack>

            {bookingDetails.notes && (
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                <NotesIcon />
                <Typography variant="body1" component="div">
                  {bookingDetails.notes}
                </Typography>
              </Stack>
            )}

            {bookingDetails.organization && (
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                <OrganizationIcon />
                <Typography variant="body1" component="div">
                  {bookingDetails.organization.name}
                </Typography>
              </Stack>
            )}

            {bookingDetails.location && (
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                <LocationIcon />
                <Typography variant="body1" component="div">
                  {bookingDetails.location.name}
                </Typography>
              </Stack>
            )}

            {bookingDetails.team && (
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                <TeamIcon />
                <Typography variant="body1" component="div">
                  {bookingDetails.team.name}
                </Typography>
              </Stack>
            )}

            {bookingDetails.desks?.map(({ uniqueId, name, zones }) => {
              const isPreferredDesk = !!rootData.me?.preferredDesks.find((desk) => desk.uniqueId === uniqueId);

              return (
                <Stack key={uniqueId} direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                  <DeskIcon />
                  <Typography variant="body1" component="div">
                    {name}
                  </Typography>
                  {isPreferredDesk && (
                    <Tooltip title={'Remove as preferred desk'}>
                      <Button size="small" color="primary" onClick={() => handleRemoveAsPreferredDeskClicked(uniqueId, name)}>
                        <PreferredIcon />
                      </Button>
                    </Tooltip>
                  )}
                  {!isPreferredDesk && (
                    <Tooltip title={'Set as preferred desk'}>
                      <Button size="small" color="primary" onClick={() => handleSetAsPreferredDeskClicked(uniqueId, name)}>
                        <NotPreferredIcon />
                      </Button>
                    </Tooltip>
                  )}

                  <ZonesLine
                    zones={zones.map(({ uniqueId, name }) => ({
                      id: uniqueId,
                      name,
                    }))}
                  />
                </Stack>
              );
            })}
          </CardContent>

          {(canUpdateBooking || canDeleteBooking || canJoinBooking) && (
            <CardActions sx={{ justifyContent: 'flex-end' }}>
              {canUpdateBooking && (
                <Button size="small" color="primary" onClick={handleEditClick}>
                  <EditIcon />
                </Button>
              )}

              {canDeleteBooking && (
                <Button size="small" color="warning" onClick={handleDeleteClick}>
                  <DeleteIcon />
                </Button>
              )}

              {canJoinBooking && (
                <Button size="small" color="primary" onClick={handleJoinClick}>
                  <JoinIcon />
                </Button>
              )}
            </CardActions>
          )}
        </Card>
      )}

      {canUpdateBooking && editing && (
        <Paper elevation={24} sx={{ padding: 2 }}>
          <Form
            onSubmit={handleSaveClick}
            initialValues={{
              date: from,
              notes: bookingDetails.notes,
              organization: bookingDetails.organization ? bookingDetails.organization.uniqueId : undefined,
              member: bookingDetails.customer.uniqueId,
              location: bookingDetails.location ? bookingDetails.location.uniqueId : undefined,
              desks: bookingDetails.desks ? bookingDetails.desks.map(({ uniqueId }) => uniqueId) : [],
            }}
            validate={validate}
            render={({ handleSubmit, values }) => {
              setFrom(values.date);

              return (
                <Stack direction="column" spacing={2} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
                  <BookingDate name="date" required={requiredFields.date} />
                  <BookingNotes name="notes" required={requiredFields.notes} />
                  <BookingDetailsSelector
                    rootDataRelay={rootData}
                    rootDataPaginatedOrganizationMembersRelay={rootData}
                    rootDataAvailableLocationDesksRelay={rootData}
                    defaultOrganizationId={bookingDetails.organization?.uniqueId}
                    organizationName="organization"
                    organizationRequired={requiredFields.organization}
                    hideOrganizationControl={hideOrganizationControl}
                    organizationMemberName="member"
                    organizationMemberRequired={requiredFields.member}
                    hideOrganizationMemberControl={true}
                    defaultLocationId={bookingDetails.location?.uniqueId}
                    locationName="location"
                    locationRequired={requiredFields.location}
                    hideLocationControl={hideLocationControl}
                    deskName="desks"
                    deskRequired={requiredFields.desks}
                    defaultDeskIds={bookingDetails.desks ? bookingDetails.desks.map(({ uniqueId }) => uniqueId) : []}
                    hideDesksControl={false}
                    bookingFrom={from}
                    bookingTo={to}
                  />

                  <Stack sx={{ justifyContent: 'flex-end' }} direction="row" spacing={1}>
                    <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                      Cancel
                    </Button>
                    <Button color="primary" variant="contained" type="submit">
                      Update
                    </Button>
                  </Stack>
                </Stack>
              );
            }}
          />
        </Paper>
      )}
    </>
  );
};

export default memo(Booking);

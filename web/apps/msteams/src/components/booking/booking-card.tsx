import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Switch from '@mui/material/Switch';
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
  OrganizationIcon,
  TeamIcon,
} from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { endOfDay, getCustomerFullName, joinErrors, startOfDay, toShortDate } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { TAG_TYPE_LOCATION_ZONE, ZonesLine } from 'components/zone';
import dayjs, { Dayjs } from 'dayjs';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useMemo, useState } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { array, date, object, string } from 'yup';
import type { bookingCard_BookingDetails$key } from './__generated__/bookingCard_BookingDetails.graphql';
import type { bookingCard_addBookingMutation } from './__generated__/bookingCard_addBookingMutation.graphql';
import type { bookingCard_addCustomerDefaultDeskMutation } from './__generated__/bookingCard_addCustomerDefaultDeskMutation.graphql';
import type { bookingCard_deleteBookingMutation } from './__generated__/bookingCard_deleteBookingMutation.graphql';
import type { bookingCard_query$key } from './__generated__/bookingCard_query.graphql';
import type { bookingCard_removeCustomerDefaultDeskMutation } from './__generated__/bookingCard_removeCustomerDefaultDeskMutation.graphql';
import type { bookingCard_updateBookingMutation } from './__generated__/bookingCard_updateBookingMutation.graphql';
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

interface BookingDetails {
  date: Date;
  member: string;
  notes: string;
  organization: string | undefined;
  location: string | undefined;
  desks: string[];
}

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
        myLocations(organizationId: $organizationId) {
          id
          name
        }
        organizationBookingPermissions(organizationId: $organizationId) {
          canUpdateBookingOnBehalf
          canDeleteBookingOnBehalf
        }
        ...bookingDetailsSelector_query
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
          locationTags {
            uniqueId
            name
            tagType
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
            locationTags {
              uniqueId
              name
              tagType
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
            locationTags {
              uniqueId
              name
              tagType
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

  const { enqueueSnackbar } = useSnackbar();
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
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to add booking '${shortDateFormatFrom}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to add booking '${shortDateFormatFrom}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
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
          enqueueSnackbar(`Failed to delete booking '${shortDateFormatFrom}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to delete booking '${shortDateFormatFrom}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
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
    setEditing(false);

    if (!rootData.me) {
      return;
    }

    const finalDate = date as unknown as Dayjs;
    const start = startOfDay(finalDate);
    const from = start.toISOString();
    const to = endOfDay(finalDate).toISOString();
    const shortDateTimeFormatFrom = toShortDate(start);

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
          enqueueSnackbar(`Failed to update booking '${shortDateTimeFormatFrom}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to update booking '${shortDateTimeFormatFrom}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
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

  const handleDefaultDeskStateChange = (event: React.ChangeEvent<HTMLInputElement>, deskId: string, deskName: string) => {
    if (!rootData.me) {
      return;
    }

    if (event.target.checked) {
      commitAddCustomerDefaultDesk({
        variables: {
          input: {
            clientMutationId: nanoid(),
            deskId,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to set desk '${deskName}' as default. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });
          }
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to set desk '${deskName}' as default. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
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
    } else {
      commitRemoveCustomerDefaultDesk({
        variables: {
          input: {
            clientMutationId: nanoid(),
            deskId,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to clear default desk '${deskName}'. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });
          }
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to clear default desk '${deskName}'. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
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
    }
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
  const isMyBooking = rootData.me.id === bookingDetails.customer?.uniqueId;

  return (
    <>
      {!editing && (
        <Card elevation={24} sx={{ minWidth: 400, height: '100%' }}>
          <CardHeader
            title={
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                <CustomerAvatar
                  name={{
                    name: bookingDetails.customer?.name,
                    givenName: bookingDetails.customer?.givenName,
                    middleName: bookingDetails.customer?.middleName,
                    familyName: bookingDetails.customer?.familyName,
                  }}
                  photo={{
                    url: bookingDetails.customer?.photoUrl,
                  }}
                />
                <Typography variant="h6" component="div">
                  {shortDateFormatFrom}
                </Typography>
              </Stack>
            }
          />

          <CardContent>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
              <CustomerIcon />
              <Typography variant="h6" component="div">
                {getCustomerFullName(bookingDetails.customer)}
              </Typography>
            </Stack>

            {bookingDetails.notes && (
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                <NotesIcon />
                <Typography variant="body1" component="div">
                  {bookingDetails.notes}
                </Typography>
              </Stack>
            )}

            {bookingDetails.organization && (
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                <OrganizationIcon />
                <Typography variant="body1" component="div">
                  {bookingDetails.organization.name}
                </Typography>
              </Stack>
            )}

            {bookingDetails.location && (
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                <LocationIcon />
                <Typography variant="body1" component="div">
                  {bookingDetails.location.name}
                </Typography>
              </Stack>
            )}

            {bookingDetails.team && (
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                <TeamIcon />
                <Typography variant="body1" component="div">
                  {bookingDetails.team.name}
                </Typography>
              </Stack>
            )}

            {bookingDetails.desks?.map(({ uniqueId, name, locationTags }) => {
              const zones = locationTags.filter(({ tagType }) => tagType === TAG_TYPE_LOCATION_ZONE);

              return (
                <Stack key={uniqueId} direction="row" spacing={2} sx={{ marginBottom: 1 }}>
                  <DeskIcon />
                  <Typography variant="body1" component="div">
                    {name}
                  </Typography>
                  {isMyBooking && (
                    <Switch
                      checked={!!rootData.me?.preferredDesks.find((desk) => desk.uniqueId === uniqueId)}
                      onChange={(event) => handleDefaultDeskStateChange(event, uniqueId, name)}
                    />
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
                <Stack direction="column" component="form" noValidate onSubmit={handleSubmit} spacing={2}>
                  <BookingDate name="date" required={requiredFields.date} />
                  <BookingNotes name="notes" required={requiredFields.notes} />
                  <BookingDetailsSelector
                    rootDataRelay={rootData}
                    defaultOrganizationId={bookingDetails.organization ? bookingDetails.organization.uniqueId : null}
                    organizationName="organization"
                    organizationRequired={requiredFields.organization}
                    hideOrganizationControl={hideOrganizationControl}
                    organizationMemberName="member"
                    organizationMemberRequired={requiredFields.member}
                    hideOrganizationMemberControl={true}
                    defaultLocationId={bookingDetails.location ? bookingDetails.location.uniqueId : null}
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
                      Save
                    </Button>
                  </Stack>
                  <Stack sx={{ flex: 1 }} direction="row" spacing={2} />
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

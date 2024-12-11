import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import Stack from '@mui/material/Stack';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext, UpdateGlobalReloadIdContext } from '@repo/shared/libs/providers';
import { endOfDay, getCustomerFullName, joinErrors, startOfDay, toShortDate } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { BookingDate, BookingDetailsSelector, BookingNotes } from 'components/booking';
import dayjs, { Dayjs } from 'dayjs';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useMemo, useState } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { array, date, object, string } from 'yup';
import type { newBookingDialog_addBookingMutation } from './__generated__/newBookingDialog_addBookingMutation.graphql';
import type { newBookingDialog_query$key } from './__generated__/newBookingDialog_query.graphql';

type Props = {
  rootDataRelay: newBookingDialog_query$key;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancelClicked: () => void;
  organizationId: string;
  locationId?: string;
  defaultTeamId?: string;
  hideOrganizationControl?: boolean;
  hideLocationControl?: boolean;
  defaultDate?: Dayjs;
};

type BookingDetails = {
  date: Date;
  member: string | undefined;
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

const bookingWithoutMemberSchema = object({
  date: date().required(),
  notes: string().notRequired(),
  organization: string().notRequired(),
  location: string().notRequired(),
  desk: array().nullable(),
});

const NewBookingDialog = ({
  rootDataRelay,
  connectionIds,
  isDialogOpen,
  onAddClicked,
  onCancelClicked,
  organizationId,
  locationId,
  defaultTeamId,
  hideOrganizationControl,
  hideLocationControl,
  defaultDate,
}: Props) => {
  const rootData = useFragment(
    graphql`
      fragment newBookingDialog_query on Query {
        me {
          id
        }
        organizationBookingPermissions(organizationId: $organizationId) {
          canAddBookingOnBehalf
        }
        ...bookingDetailsSelector_query
        ...bookingDetailsSelector_organizationMembers_query
        ...bookingDetailsSelector_availableLocationDesks_query
      }
    `,
    rootDataRelay,
  );

  const [commitAddBooking] = useMutation<newBookingDialog_addBookingMutation>(graphql`
    mutation newBookingDialog_addBookingMutation($connectionIds: [ID!]!, $input: AddBookingInput!) @raw_response_type {
      addBooking(input: $input) {
        booking @appendNode(connections: $connectionIds, edgeTypeName: "BookingDetails") {
          id
          from
          to
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

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const UpdateGlobalReloadId = useContext(UpdateGlobalReloadIdContext);
  const schema = !!rootData.organizationBookingPermissions?.canAddBookingOnBehalf ? bookingSchema : bookingWithoutMemberSchema;
  const validate = makeValidate(schema);
  const requiredFields = makeRequired(schema);
  const [from, setFrom] = useState<Dayjs | Date>(defaultDate ?? startOfDay());
  const to = useMemo(() => {
    if (from instanceof Date) {
      return endOfDay(dayjs(from));
    }

    return endOfDay(from);
  }, [from]);

  useEffect(() => {
    setFrom(defaultDate ?? startOfDay());
  }, [defaultDate]);

  const handleAddClick = ({ date, member, notes, organization: organizationId, location: locationId, desks: deskIds }: BookingDetails) => {
    if (!rootData.me) {
      return;
    }

    const id = nanoid();
    const finalDate = date as unknown as Dayjs;
    const from = startOfDay(finalDate).toISOString();
    const to = endOfDay(finalDate).toISOString();
    const fromToPrint = toShortDate(startOfDay(finalDate));
    const customerId = member ?? rootData.me?.id;
    const toastId = themedToast(<NotificationContent content={`Making a booking on '${fromToPrint}'...`} />, infoNotificationOptions);
    const type = 'WorkingFromOffice';

    commitAddBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id,
          customerId,
          from,
          to,
          notes,
          organizationId,
          locationId,
          deskIds,
          teamId: defaultTeamId,
          type,
        },
      },
      onCompleted: (response, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to make a booking '${fromToPrint}'. Error: ${joinErrors(errors)}.`} />,
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

        onAddClicked();
        UpdateGlobalReloadId();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to make a booking '${fromToPrint}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addBooking: {
          booking: {
            id,
            from,
            to,
            notes,
            type,
            customer: {
              uniqueId: rootData.me.id,
              name: '',
              givenName: '',
              middleName: '',
              familyName: '',
              photoUrl: '',
            },
            organization: organizationId
              ? {
                  uniqueId: organizationId,
                  name: '',
                }
              : null,
            location: locationId
              ? {
                  uniqueId: locationId,
                  name: '',
                }
              : null,
            team: defaultTeamId
              ? {
                  uniqueId: defaultTeamId,
                  name: '',
                }
              : null,
            desks: [],
          },
        },
      },
    });
  };

  if (!rootData.me) {
    return <></>;
  }

  return (
    <Dialog TransitionComponent={DialogTransition} open={isDialogOpen}>
      <DialogTitle>Make a booking</DialogTitle>
      <DialogContent>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            date: from,
            notes: '',
            organization: organizationId,
            member: null,
            location: locationId,
            desks: [],
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
                  defaultOrganizationId={organizationId}
                  organizationName="organization"
                  organizationRequired={requiredFields.organization}
                  hideOrganizationControl={hideOrganizationControl}
                  organizationMemberName="member"
                  organizationMemberRequired={requiredFields.member}
                  hideOrganizationMemberControl={!!!rootData.organizationBookingPermissions?.canAddBookingOnBehalf}
                  defaultLocationId={locationId}
                  locationName="location"
                  locationRequired={requiredFields.location}
                  hideLocationControl={hideLocationControl}
                  deskName="desks"
                  deskRequired={requiredFields.desks}
                  defaultDeskIds={[]}
                  hideDesksControl={false}
                  bookingFrom={from}
                  bookingTo={to}
                />

                <DialogActions>
                  <Button color="secondary" variant="contained" onClick={onCancelClicked}>
                    Cancel
                  </Button>
                  <Button color="primary" variant="contained" type="submit">
                    Add
                  </Button>
                </DialogActions>
              </Stack>
            );
          }}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(NewBookingDialog);

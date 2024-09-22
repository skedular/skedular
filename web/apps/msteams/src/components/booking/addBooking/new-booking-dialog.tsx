import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import Stack from '@mui/material/Stack';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { endOfDay, joinErrors, startOfDay, toShortDate } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { BookingDate, BookingDetailsSelector, BookingNotes } from 'components/booking';
import dayjs, { Dayjs } from 'dayjs';
import { makeRequired, makeValidate } from 'mui-rff';
import { useSnackbar } from 'notistack';
import { memo, useMemo, useState } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { v4 as uuidv4 } from 'uuid';
import { array, date, object, string } from 'yup';
import type { newBookingDialog_addBookingMutation } from './__generated__/newBookingDialog_addBookingMutation.graphql';
import type { newBookingDialog_query$key } from './__generated__/newBookingDialog_query.graphql';

type Props = {
  rootDataRelay: newBookingDialog_query$key;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancelClicked: () => void;
  organizationId: string | null;
  locationId: string | null;
  defaultTeamId: string | null;
  hideOrganizationControl: boolean;
  hideLocationControl: boolean;
};

interface BookingDetails {
  date: Date;
  member: string | undefined;
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
          customer {
            uniqueId
          }
        }
      }
    }
  `);

  const { enqueueSnackbar } = useSnackbar();

  const schema = !!rootData.organizationBookingPermissions?.canAddBookingOnBehalf ? bookingSchema : bookingWithoutMemberSchema;
  const validate = makeValidate(schema);
  const requiredFields = makeRequired(schema);
  const [from, setFrom] = useState<Dayjs | Date>(startOfDay(null));
  const to = useMemo(() => {
    if (from instanceof Date) {
      return endOfDay(dayjs(from));
    }

    return endOfDay(from);
  }, [from]);

  const handleAddClick = ({ date, member, notes, organization: organizationId, location: locationId, desks: deskIds }: BookingDetails) => {
    if (!rootData.me) {
      return;
    }

    const id = uuidv4();
    const finalDate = date as unknown as Dayjs;
    const from = startOfDay(finalDate).toISOString();
    const to = endOfDay(finalDate).toISOString();
    const fromToPrint = toShortDate(startOfDay(finalDate));
    const customerId = member ?? rootData.me?.id;

    commitAddBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuidv4(),
          id,
          customerId,
          from,
          to,
          notes,
          organizationId,
          locationId,
          deskIds,
          teamId: defaultTeamId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to add booking '${fromToPrint}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        } else {
          onAddClicked();
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to add booking '${fromToPrint}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        addBooking: {
          booking: {
            id,
            from,
            to,
            notes,
            customer: {
              uniqueId: customerId,
            },
          },
        },
      },
    });
  };

  if (!rootData.me) {
    return <></>;
  }

  return (
    <Dialog fullWidth={true} open={isDialogOpen}>
      <DialogTitle>Add Booking</DialogTitle>
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
              <Stack direction="column" component="form" noValidate onSubmit={handleSubmit} spacing={2}>
                <BookingDate name="date" required={requiredFields.date} />
                <BookingNotes name="notes" required={requiredFields.notes} />
                <BookingDetailsSelector
                  rootDataRelay={rootData}
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

import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import Stack from '@mui/material/Stack';
import { DialogTransition } from '@repo/shared/components/transitions';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { endOfDay, joinErrors, startOfDay, toShortDate } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { BookingDate, BookingDetailsSelector, BookingNotes } from 'components/booking';
import dayjs, { Dayjs } from 'dayjs';
import { UpdateGlobalReloadIdContext } from 'libs/providers';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useContext, useMemo, useState } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
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
        organizationBookingPermissions(organizationId: $organizationId) @include(if: $organizationExists) {
          canAddBookingOnBehalf
        }
        ...bookingDetailsSelector_query
        ...bookingDetailsSelector_paginatedOrganizationMembers_query
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
          customer {
            uniqueId
          }
        }
      }
    }
  `);

  const UpdateGlobalReloadId = useContext(UpdateGlobalReloadIdContext);
  const { enqueueSnackbar } = useSnackbar();
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
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to make a booking '${fromToPrint}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        onAddClicked();
        UpdateGlobalReloadId();
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to make a booking '${fromToPrint}'. Error: ${error.message}`, {
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
              <Stack direction="column" spacing={1} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
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

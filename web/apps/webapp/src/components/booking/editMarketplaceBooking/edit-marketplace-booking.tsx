import { CustomerAvatar } from '@/components/avatars';
import { SingleChoiceMarketplaceBookingType } from '@/components/booking';
import {
  AppBarWithStackColumn,
  BodyIconTypography,
  FormFieldLabel,
  FormStackColumn,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@/components/commons';
import { PdfIcon } from '@/components/icons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import {
  getCustomerFullName,
  getOpeningHoursFromDateTime,
  isMidnight,
  joinErrors,
  keyboardSearchDebounceTimeout,
  toOpeningHoursFromTime,
  toShortDate,
  toShortTime,
} from '@/libs/utils';
import type { editMarketplaceBooking_booking_query$key } from '@/queries/__generated__/editMarketplaceBooking_booking_query.graphql';
import type { editMarketplaceBooking_booking_refetchableFragment } from '@/queries/__generated__/editMarketplaceBooking_booking_refetchableFragment.graphql';
import type { editMarketplaceBooking_customerTeams_query$key } from '@/queries/__generated__/editMarketplaceBooking_customerTeams_query.graphql';
import type { editMarketplaceBooking_customerTeams_refetchableFragment } from '@/queries/__generated__/editMarketplaceBooking_customerTeams_refetchableFragment.graphql';
import type { editMarketplaceBooking_organizationMembers_query$key } from '@/queries/__generated__/editMarketplaceBooking_organizationMembers_query.graphql';
import type { editMarketplaceBooking_organizationMembers_refetchableFragment } from '@/queries/__generated__/editMarketplaceBooking_organizationMembers_refetchableFragment.graphql';
import type { editMarketplaceBooking_query$key } from '@/queries/__generated__/editMarketplaceBooking_query.graphql';
import type { BookingType, editMarketplaceBooking_updateBookingMutation } from '@/queries/__generated__/editMarketplaceBooking_updateBookingMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Link from '@mui/material/Link';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { DateRange } from '@mui/x-date-pickers-pro/models';
import { Dayjs } from 'dayjs';
import { Autocomplete, makeRequired, makeValidate, TextField } from 'mui-rff';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

type Props = {
  rootDataRelay: editMarketplaceBooking_query$key;
  rootDataBookingRelay: editMarketplaceBooking_booking_query$key;
  rootDataOrganizationMembersRelay: editMarketplaceBooking_organizationMembers_query$key;
  rootDataTeamsRelay: editMarketplaceBooking_customerTeams_query$key;
  onReloadRequired?: () => void;
};

type CustomerDetails = {
  id: string;
  name: string | null | undefined;
  givenName: string | null | undefined;
  middleName: string | null | undefined;
  familyName: string | null | undefined;
  photoUrl: string | null | undefined;
};

type OrganizationMemberDetails = {
  id: string;
  customer: CustomerDetails;
};

type TeamDetails = {
  id: string;
  name: string;
};

type BookingDetails = {
  member: string;
  notes: string | null | undefined;
  team: string | undefined;
  type: string;
};

const bookingSchema = object({
  member: string().required('User is required'),
  notes: string().notRequired(),
  team: string().notRequired(),
  type: string().required('Type is required'),
});

const EditMarketplaceBooking = ({ rootDataRelay, rootDataBookingRelay, rootDataTeamsRelay, rootDataOrganizationMembersRelay }: Props) => {
  const rootData = useFragment<editMarketplaceBooking_query$key>(
    graphql`
      fragment editMarketplaceBooking_query on Query {
        openingHoursMinutesStep
        ...singleChoiceMarketplaceBookingType_query
      }
    `,
    rootDataRelay,
  );

  const [rootDataBooking] = useRefetchableFragment<editMarketplaceBooking_booking_refetchableFragment, editMarketplaceBooking_booking_query$key>(
    graphql`
      fragment editMarketplaceBooking_booking_query on Query @refetchable(queryName: "editMarketplaceBooking_booking_refetchableFragment") {
        booking(id: $bookingId) {
          id
          from
          until
          notes
          type {
            type
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
            name
          }
          involvedLocations {
            id
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
          isPaymentRequired
          paymentStatus {
            type
            name
          }
          invoiceUrl
        }
      }
    `,
    rootDataBookingRelay,
  );

  const [rootDataOrganizationMembers, refetchOrganizationMembers] = useRefetchableFragment<
    editMarketplaceBooking_organizationMembers_refetchableFragment,
    editMarketplaceBooking_organizationMembers_query$key
  >(
    graphql`
      fragment editMarketplaceBooking_organizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "editMarketplaceBooking_organizationMembers_refetchableFragment") {
        organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
          members(first: $count, after: $cursor, where: { nameContains: $peopleNameSearchText }, orderBy: $organizationMembersSortingValues)
            @connection(key: "bookingDetailsSelectorQuery_members") {
            __id
            totalCount
            edges {
              node {
                id
                customer {
                  id
                  name
                  givenName
                  middleName
                  familyName
                  photoUrl
                }
              }
            }
          }
        }
      }
    `,
    rootDataOrganizationMembersRelay,
  );

  const [rootDataTeams, refetchTeams] = useRefetchableFragment<editMarketplaceBooking_customerTeams_refetchableFragment, editMarketplaceBooking_customerTeams_query$key>(
    graphql`
      fragment editMarketplaceBooking_customerTeams_query on Query @refetchable(queryName: "editMarketplaceBooking_customerTeams_refetchableFragment") {
        customerTeams(where: { organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, customerId: $customerId }, orderBy: $teamsSortingValues)
          @include(if: $customerExists) {
          __id
          totalCount
          edges {
            node {
              id
              name
            }
          }
        }
      }
    `,
    rootDataTeamsRelay,
  );

  const [commitUpdateBooking] = useMutation<editMarketplaceBooking_updateBookingMutation>(graphql`
    mutation editMarketplaceBooking_updateBookingMutation($input: UpdateBookingInput!) @raw_response_type {
      updateBooking(input: $input) {
        booking {
          id
          from
          until
          notes
          type {
            type
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
            name
          }
          involvedLocations {
            id
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

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [, startTransition] = useTransition();
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const validate = makeValidate(bookingSchema);
  const requiredFields = makeRequired(bookingSchema);
  const allDay = useMemo<boolean>(
    () => isMidnight(rootDataBooking.booking?.from) && isMidnight(rootDataBooking.booking?.until),
    [rootDataBooking.booking?.from, rootDataBooking.booking?.until],
  );
  const timeRange = useMemo<DateRange<Dayjs>>(
    () => [toOpeningHoursFromTime(getOpeningHoursFromDateTime(rootDataBooking.booking?.from)), toOpeningHoursFromTime(getOpeningHoursFromDateTime(rootDataBooking.booking?.until))],
    [rootDataBooking.booking?.from, rootDataBooking.booking?.until],
  );
  const [customerId, setCustomerId] = useState<string | undefined>(
    rootDataBooking.booking?.involvedCustomers && rootDataBooking.booking?.involvedCustomers.length > 0 ? rootDataBooking.booking?.involvedCustomers[0].id : undefined,
  );
  const [teamId, setTeamId] = useState<string | undefined>(
    rootDataBooking.booking?.involvedTeams && rootDataBooking.booking?.involvedTeams.length > 0 ? rootDataBooking.booking?.involvedTeams[0].id : undefined,
  );
  const filterTeam = createFilterOptions<TeamDetails>();

  const customers = useMemo<OrganizationMemberDetails[]>(
    () => (rootDataOrganizationMembers.organization?.members ? rootDataOrganizationMembers.organization?.members.edges.map(({ node }) => node) : []),
    [rootDataOrganizationMembers.organization?.members],
  );
  const teams = useMemo<TeamDetails[]>(() => (rootDataTeams.customerTeams ? rootDataTeams.customerTeams.edges.map(({ node }) => node) : []), [rootDataTeams.customerTeams]);

  const handleRefetchOrganizationMembers = useCallback(
    (peopleNameSearchText: string) => {
      startTransition(() => {
        refetchOrganizationMembers(
          {
            peopleNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [startTransition, refetchOrganizationMembers],
  );

  const handleRefetchTeams = useCallback(
    (customerId: string | undefined) => {
      startTransition(() => {
        refetchTeams(
          {
            customerId: customerId ?? '',
            customerExists: !!customerId,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [startTransition, refetchTeams],
  );

  useEffect(() => {
    if (!rootDataBooking.booking?.involvedCustomers || rootDataBooking.booking?.involvedCustomers.length === 0) {
      return;
    }

    handleRefetchTeams(rootDataBooking.booking.involvedCustomers[0].id);
  }, [handleRefetchTeams, rootDataBooking.booking?.involvedCustomers]);

  const handleCloseClick = () => {
    router.back();
  };

  const handleBookingDetailUpdateClick = ({ member: memberId, notes, team: teamId, type }: BookingDetails) => {
    const booking = rootDataBooking.booking;
    if (!booking) {
      return;
    }

    const shortDateTimeFormatFrom = toShortDate(booking.from);

    let bookingDetailsInfo = `for ${getCustomerFullName(booking.involvedCustomers[0])}`;
    if (booking.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${booking.involvedLocations[0]!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateTimeFormatFrom}`;

    const toastId = themedToast(<NotificationContent content={`Updating booking '${bookingDetailsInfo}'...`} />, infoNotificationOptions);

    commitUpdateBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: booking.id,
          from: booking.from,
          until: booking.until,
          notes,
          type: type as BookingType,
          customerIds: [memberId],
          organizationIds: booking.involvedOrganizations.map(({ id }) => id),
          teamIds: teamId ? [teamId] : [],
          resourceIds: booking.bookingResources.map(({ resource }) => resource.id),
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

        router.back();
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
            id: booking.id,
            from: booking.from,
            until: booking.until,
            notes,
            type: {
              type: type as BookingType,
              name: '',
            },
            involvedCustomers: [
              {
                id: '',
                name: '',
                givenName: '',
                middleName: '',
                familyName: '',
                photoUrl: '',
              },
            ],
            involvedOrganizations: [],
            involvedLocations: [],
            involvedTeams: [],
            bookingResources: booking.bookingResources,
          },
        },
      },
    });
  };

  const handleMemberChange = (option: OrganizationMemberDetails | null) => {
    if (!rootDataBooking.booking) {
      return;
    }

    const customerId = option?.customer.id;
    setCustomerId(customerId);
    handleRefetchTeams(customerId);
  };

  const handleTeamChange = (option: TeamDetails | null) => {
    if (!rootDataBooking.booking) {
      return;
    }

    setTeamId(option?.id);
  };

  const handlePeopleNameSearchTextChange = (str: string) => {
    setPeopleNameSearchText(str);

    handleRefetchOrganizationMembers(str);
  };

  const debounceSearchTextChange = useDebounceCallback(handlePeopleNameSearchTextChange, keyboardSearchDebounceTimeout);

  if (!rootDataBooking.booking) {
    return <></>;
  }

  const booking = rootDataBooking.booking;

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Booking Information">
          <Form
            onSubmit={handleBookingDetailUpdateClick}
            initialValues={{
              member: customerId,
              notes: booking.notes,
              team: teamId,
              type: booking.type.type,
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <SectionIconTypography label="Edit Booking" />
                  <BodyIconTypography label="Edit your booking details" />
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <FormFieldLabel label="Payment Status">
                    <SmallIconTypography label={booking.paymentStatus.name} sx={{ paddingTop: 1, paddingBottom: 1 }} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Date/Time">
                    <StackRow>
                      <BodyIconTypography label={`${toShortDate(booking.from)}, `} />
                      {allDay && <BodyIconTypography label="All day" />}
                      {!allDay && <BodyIconTypography label={`${toShortTime(timeRange[0])} - ${toShortTime(timeRange[1])}`} />}
                    </StackRow>
                  </FormFieldLabel>

                  <FormFieldLabel label="Invoice">
                    {booking.invoiceUrl && (
                      <Link component={NextLink} href={booking.invoiceUrl} target="_blank" rel="noopener noreferrer">
                        <SmallIconTypography label="Download Invoice" startElement={<PdfIcon />} />
                      </Link>
                    )}
                  </FormFieldLabel>

                  <FormFieldLabel label="User">
                    <Autocomplete
                      name="member"
                      multiple={false}
                      required={requiredFields.member}
                      options={customers}
                      getOptionValue={(option) => (option as OrganizationMemberDetails).customer.id}
                      getOptionLabel={(option: string | OrganizationMemberDetails) => getCustomerFullName((option as OrganizationMemberDetails).customer)}
                      renderOption={(props, option) => {
                        const castedOption = (option as OrganizationMemberDetails).customer;

                        return (
                          <li {...props} key={castedOption.id}>
                            <BodyIconTypography
                              label={getCustomerFullName(castedOption)}
                              startElement={<CustomerAvatar name={castedOption} photo={{ url: castedOption.photoUrl }} size="small" />}
                            />
                          </li>
                        );
                      }}
                      filterOptions={(options, params) => {
                        if (params.inputValue !== peopleNameSearchText) {
                          debounceSearchTextChange(params.inputValue);
                        }

                        return options;
                      }}
                      selectOnFocus
                      clearOnBlur
                      handleHomeEndKeys
                      onChange={(_, option) => handleMemberChange(option as OrganizationMemberDetails)}
                    />
                  </FormFieldLabel>

                  <FormFieldLabel label="Notes">
                    <TextField name="notes" required={requiredFields.notes} helperText="e.g. I will be half an hour late this morning" multiline rows={2} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Type">
                    <SingleChoiceMarketplaceBookingType rootDataRelay={rootData} name="type" required={requiredFields.type} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Team">
                    <Autocomplete
                      name="team"
                      multiple={false}
                      required={requiredFields.team}
                      options={teams}
                      getOptionValue={(option) => (option as TeamDetails).id}
                      getOptionLabel={(option: string | TeamDetails) => (option as TeamDetails).name}
                      renderOption={(props, option) => {
                        const castedOption = option as TeamDetails;

                        return (
                          <li {...props} key={castedOption.id}>
                            <BodyIconTypography label={castedOption.name} />
                          </li>
                        );
                      }}
                      filterOptions={(options, params) => filterTeam(options as TeamDetails[], params)}
                      selectOnFocus
                      clearOnBlur
                      handleHomeEndKeys
                      onChange={(_, option) => handleTeamChange(option as TeamDetails)}
                    />
                  </FormFieldLabel>
                </StackColumn>

                {(booking.paymentStatus.type === 'NO_PAYMENT_REQUIRED' || booking.paymentStatus.type === 'CONFIRMED') && (
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <StackRow>
                      <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                        Update
                      </Button>
                    </StackRow>
                  </StackColumn>
                )}
              </FormStackColumn>
            )}
          />
        </AppBarWithStackColumn>
      </Box>
    </Box>
  );
};

export default memo(EditMarketplaceBooking);

import { CustomerAvatar } from '@/components/avatars';
import { SingleChoiceBookingCategory } from '@/components/booking';
import { BodyIconTypography, ErrorTypography, FormFieldLabel, FormStackColumn, SmallIconTypography, StackColumn, StackRow } from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { Autocomplete } from '@/components/forms';
import { CalendarIcon } from '@/components/icons';
import { getOrganizationBookingsBaseLink } from '@/components/links';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { Zones } from '@/components/zone';
import { PaletteModeContext, useIntegratedPlatrform, useKnownParams } from '@/libs/providers';
import { getCustomerFullName, getRelayErrorMessage, isMidnight, keyboardSearchDebounceTimeout, startOfDay, toOpeningHoursFromTime, toShortDate } from '@/libs/utils';
import type {
  BookingCategory as AddPrivateBookingCategory,
  addPrivateBookingPage_addPrivateBookingMutation,
} from '@/queries/__generated__/addPrivateBookingPage_addPrivateBookingMutation.graphql';
import type {
  addPrivateBookingPage_addPrivateRecurringBookingMutation,
  BookingCategory as AddPrivateRecurringBookingCategory,
} from '@/queries/__generated__/addPrivateBookingPage_addPrivateRecurringBookingMutation.graphql';
import type { addPrivateBookingPage_availableResources_query$key } from '@/queries/__generated__/addPrivateBookingPage_availableResources_query.graphql';
import type { addPrivateBookingPage_availableResources_refetchableFragment } from '@/queries/__generated__/addPrivateBookingPage_availableResources_refetchableFragment.graphql';
import type { addPrivateBookingPage_customerTeams_query$key } from '@/queries/__generated__/addPrivateBookingPage_customerTeams_query.graphql';
import type { addPrivateBookingPage_customerTeams_refetchableFragment } from '@/queries/__generated__/addPrivateBookingPage_customerTeams_refetchableFragment.graphql';
import type { addPrivateBookingPage_organizationMembers_query$key } from '@/queries/__generated__/addPrivateBookingPage_organizationMembers_query.graphql';
import type { addPrivateBookingPage_organizationMembers_refetchableFragment } from '@/queries/__generated__/addPrivateBookingPage_organizationMembers_refetchableFragment.graphql';
import type { addPrivateBookingPage_query$key } from '@/queries/__generated__/addPrivateBookingPage_query.graphql';
import type { addPrivateBookingPage_rootQuery } from '@/queries/__generated__/addPrivateBookingPage_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import TextField from '@mui/material/TextField';
import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { DateRange } from '@mui/x-date-pickers-pro/models';
import { TimeRangePicker } from '@mui/x-date-pickers-pro/TimeRangePicker';
import { EditorActionBar, PageHeaderPanel, SettingsSectionCard, StickyReviewRail } from '@skedular/ui';
import dayjs, { Dayjs } from 'dayjs';
import { DatePicker, makeRequired, makeValidate, Switches } from 'mui-rff';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { Form, FormSpy } from 'react-final-form';
import { graphql, PreloadedQuery, useFragment, useMutation, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { array, boolean, mixed, object, string } from 'yup';

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

type LocationDetails = {
  id: string;
  name: string;
};

type CustomTagDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type ZoneDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type ResourceDetails = {
  id: string;
  name: string;
  customTags: CustomTagDetails[];
  zones: ZoneDetails[];
};

type BookingDetails = {
  date: Dayjs;
  allDay: boolean;
  member: string;
  notes: string;
  team: string | undefined;
  location: string | undefined;
  resources: string[];
  category: string;
};

type RecurrenceMode = 'single' | 'recurring';
type RecurrenceFrequency = 'DAILY' | 'WEEKLY' | 'MONTHLY';
type RecurrenceEndType = 'NEVER' | 'UNTIL_DATE' | 'AFTER_OCCURRENCES';
type DayOfWeek = 'MONDAY' | 'TUESDAY' | 'WEDNESDAY' | 'THURSDAY' | 'FRIDAY' | 'SATURDAY' | 'SUNDAY';

const RootQuery = graphql`
  query addPrivateBookingPage_rootQuery(
    $organizationCustomDomain: String!
    $peopleNameSearchText: String
    $locationId: String!
    $dateFromToGetAvailableResources: DateTime!
    $dateUntilToGetAvailableResources: DateTime!
    $organizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $customerId: String!
    $customerExists: Boolean!
    $teamsSortingValues: [TeamOrderInput!]
    $locationsSortingValues: [LocationOrderInput!]
  ) {
    ...addPrivateBookingPage_query
    ...addPrivateBookingPage_organizationMembers_query
    ...addPrivateBookingPage_customerTeams_query
    ...addPrivateBookingPage_availableResources_query
  }
`;

const bookingSchema = object({
  date: mixed<Dayjs>()
    .test('is-dayjs', 'Date must be a valid Dayjs object', (value) => value != null && dayjs.isDayjs(value))
    .required('Date/Time is required'),
  allDay: boolean(),
  member: string().required('User is required'),
  notes: string().notRequired(),
  team: string().notRequired(),
  location: string().notRequired(),
  resources: array().nullable(),
  category: string().required('Category is required'),
});

const recurrenceFrequencyOptions: { value: RecurrenceFrequency; label: string }[] = [
  { value: 'DAILY', label: 'Daily' },
  { value: 'WEEKLY', label: 'Weekly' },
  { value: 'MONTHLY', label: 'Monthly' },
];

const recurrenceEndTypeOptions: { value: RecurrenceEndType; label: string }[] = [
  { value: 'NEVER', label: 'Never ends' },
  { value: 'UNTIL_DATE', label: 'Until date' },
  { value: 'AFTER_OCCURRENCES', label: 'After occurrences' },
];

const dayOfWeekOptions: { value: DayOfWeek; label: string; dayjsDay: number }[] = [
  { value: 'MONDAY', label: 'Mon', dayjsDay: 1 },
  { value: 'TUESDAY', label: 'Tue', dayjsDay: 2 },
  { value: 'WEDNESDAY', label: 'Wed', dayjsDay: 3 },
  { value: 'THURSDAY', label: 'Thu', dayjsDay: 4 },
  { value: 'FRIDAY', label: 'Fri', dayjsDay: 5 },
  { value: 'SATURDAY', label: 'Sat', dayjsDay: 6 },
  { value: 'SUNDAY', label: 'Sun', dayjsDay: 0 },
];

const toRecurringDayOfWeek = (date: Dayjs): DayOfWeek => dayOfWeekOptions.find((item) => item.dayjsDay === date.day())?.value ?? 'MONDAY';

const getDateRange = (allDay: boolean, date: Dayjs | Date, { timeFrom, timeUntil }: { timeFrom: Dayjs | null; timeUntil: Dayjs | null }) => {
  const allDayFrom = dayjs(date).utc();
  const allDayUntil = dayjs(date).utc().add(1, 'day');
  const invalidResult = { valid: false, from: allDayFrom, until: allDayUntil, errorMessage: 'Time required when not booking full day.' };

  if (allDay) {
    return { valid: true, from: allDayFrom, until: allDayUntil, errorMessage: '' };
  }

  if (!timeFrom || !timeUntil) {
    return invalidResult;
  }

  if (isMidnight(timeFrom) && isMidnight(timeUntil)) {
    return { valid: true, from: allDayFrom, until: allDayUntil, errorMessage: '' };
  }

  const utcDate = dayjs(date).utc();
  const from = utcDate.set('hour', timeFrom.get('hour')).set('minute', timeFrom.get('minute'));
  const until = utcDate.set('hour', timeUntil.get('hour')).set('minute', timeUntil.get('minute'));

  if (from.isAfter(until)) {
    return { ...invalidResult, errorMessage: 'Time values are incorrect.' };
  }

  return {
    valid: true,
    from,
    until,
    errorMessage: '',
  };
};

type PageProps = {
  queryReference: PreloadedQuery<addPrivateBookingPage_rootQuery, Record<string, unknown>>;
  organizationCustomDomain: string;
  defaultDate: Dayjs;
  defaultLocationId?: string;
  defaultResourceIds: string[];
  redirectUrl?: string;
};

const AddPrivateBookingPage = ({ queryReference, organizationCustomDomain, defaultDate, defaultLocationId, defaultResourceIds, redirectUrl }: PageProps) => {
  const rootData = usePreloadedQuery<addPrivateBookingPage_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const { integratedPlatrform } = useIntegratedPlatrform();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [, startTransition] = useTransition();

  const rootDataMain = useFragment<addPrivateBookingPage_query$key>(
    graphql`
      fragment addPrivateBookingPage_query on Query {
        me {
          id
        }
        locations(where: { organizationCustomDomain: $organizationCustomDomain }, orderBy: $locationsSortingValues) {
          edges {
            node {
              id
              name
            }
          }
        }
        bookingSlotSizeInMinutes
        ...singleChoiceBookingCategory_query
      }
    `,
    rootData,
  );

  const [rootDataOrganizationMembers, refetchOrganizationMembers] = useRefetchableFragment<
    addPrivateBookingPage_organizationMembers_refetchableFragment,
    addPrivateBookingPage_organizationMembers_query$key
  >(
    graphql`
      fragment addPrivateBookingPage_organizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "addPrivateBookingPage_organizationMembers_refetchableFragment") {
        organization(customDomain: $organizationCustomDomain) {
          members(first: $count, after: $cursor, where: { nameContains: $peopleNameSearchText }, orderBy: $organizationMembersSortingValues)
            @connection(key: "addPrivateBookingPage_members") {
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
    rootData,
  );

  const [rootDataTeams, refetchTeams] = useRefetchableFragment<addPrivateBookingPage_customerTeams_refetchableFragment, addPrivateBookingPage_customerTeams_query$key>(
    graphql`
      fragment addPrivateBookingPage_customerTeams_query on Query @refetchable(queryName: "addPrivateBookingPage_customerTeams_refetchableFragment") {
        customerTeams(where: { organizationCustomDomain: $organizationCustomDomain, customerId: $customerId }, orderBy: $teamsSortingValues) @include(if: $customerExists) {
          edges {
            node {
              id
              name
            }
          }
        }
      }
    `,
    rootData,
  );

  const [rootDataAvailableResources, refetchAvailableResources] = useRefetchableFragment<
    addPrivateBookingPage_availableResources_refetchableFragment,
    addPrivateBookingPage_availableResources_query$key
  >(
    graphql`
      fragment addPrivateBookingPage_availableResources_query on Query @refetchable(queryName: "addPrivateBookingPage_availableResources_refetchableFragment") {
        availableResources(
          where: { organizationCustomDomain: $organizationCustomDomain, locationId: $locationId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources }
        ) {
          resource {
            id
            name
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
    `,
    rootData,
  );

  const [commitAddPrivateBooking] = useMutation<addPrivateBookingPage_addPrivateBookingMutation>(graphql`
    mutation addPrivateBookingPage_addPrivateBookingMutation($input: AddPrivateBookingInput!) {
      addPrivateBooking(input: $input) {
        booking {
          id
          from
          until
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
          bookingResources {
            resource {
              id
              name
              zones {
                id
                name
              }
            }
          }
        }
      }
    }
  `);

  const [commitAddPrivateRecurringBooking] = useMutation<addPrivateBookingPage_addPrivateRecurringBookingMutation>(graphql`
    mutation addPrivateBookingPage_addPrivateRecurringBookingMutation($input: AddPrivateRecurringBookingInput!) {
      addPrivateRecurringBooking(input: $input) {
        recurringBooking {
          id
          startDate
          endDate
          frequency {
            frequency
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
        }
      }
    }
  `);

  const validate = makeValidate(bookingSchema);
  const requiredFields = makeRequired(bookingSchema);
  const [from, setFrom] = useState<Dayjs>(defaultDate);
  const [allDay, setAllDay] = useState<boolean>(true);
  const [timeRange, setTimeRange] = useState<DateRange<Dayjs>>([toOpeningHoursFromTime('00:00'), toOpeningHoursFromTime('00:00')]);
  const [peopleNameSearchText, setPeopleNameSearchText] = useState('');
  const [customerId, setCustomerId] = useState<string | undefined>();
  const [locationId, setLocationId] = useState<string | undefined>(defaultLocationId);
  const [notes, setNotes] = useState('');
  const [category, setCategory] = useState('WORKING_FROM_OFFICE');
  const [resourceIds, setResourceIds] = useState<string[]>(defaultResourceIds);
  const [recurrenceMode, setRecurrenceMode] = useState<RecurrenceMode>('single');
  const [recurrenceFrequency, setRecurrenceFrequency] = useState<RecurrenceFrequency>('WEEKLY');
  const [recurrenceInterval, setRecurrenceInterval] = useState<number>(1);
  const [recurrenceWeekDays, setRecurrenceWeekDays] = useState<DayOfWeek[]>([toRecurringDayOfWeek(defaultDate)]);
  const [recurrenceEndType, setRecurrenceEndType] = useState<RecurrenceEndType>('NEVER');
  const [recurrenceEndDate, setRecurrenceEndDate] = useState<Dayjs>(defaultDate.add(1, 'month'));
  const [recurrenceOccurrenceCount, setRecurrenceOccurrenceCount] = useState<number>(10);
  const [recurrenceError, setRecurrenceError] = useState<string>('');

  const customers = useMemo<OrganizationMemberDetails[]>(
    () => (rootDataOrganizationMembers.organization ? rootDataOrganizationMembers.organization.members.edges.map(({ node }: { node: OrganizationMemberDetails }) => node) : []),
    [rootDataOrganizationMembers.organization],
  );
  const teams = useMemo<TeamDetails[]>(
    () => (rootDataTeams.customerTeams ? rootDataTeams.customerTeams.edges.map(({ node }: { node: TeamDetails }) => node) : []),
    [rootDataTeams.customerTeams],
  );
  const locations = useMemo<LocationDetails[]>(() => rootDataMain.locations.edges.map(({ node }: { node: LocationDetails }) => node), [rootDataMain.locations]);
  const resources = useMemo<ResourceDetails[]>(
    () =>
      rootDataAvailableResources.availableResources.map(({ resource }) => ({
        id: resource.id,
        name: resource.name,
        customTags: resource.customTags.map(({ id, name, color }) => ({ id, name, color })),
        zones: resource.zones.map(({ id, name, color }) => ({ id, name, color })),
      })),
    [rootDataAvailableResources.availableResources],
  );

  const dateRange = useMemo(() => {
    const [timeFrom, timeUntil] = timeRange;
    return getDateRange(allDay, from, { timeFrom, timeUntil });
  }, [allDay, from, timeRange]);

  useEffect(() => {
    if (!dateRange.valid) {
      return;
    }

    startTransition(() => {
      refetchAvailableResources(
        {
          locationId,
          dateFromToGetAvailableResources: dateRange.from.toISOString(),
          dateUntilToGetAvailableResources: dateRange.until.toISOString(),
        },
        { fetchPolicy: 'store-and-network' },
      );
    });
  }, [dateRange, locationId, refetchAvailableResources, startTransition]);

  const filterTeam = createFilterOptions<TeamDetails>();
  const filterLocation = createFilterOptions<LocationDetails>();
  const filterResource = createFilterOptions<ResourceDetails>();

  const handleRefetchOrganizationMembers = useCallback(
    (nameContains: string) => {
      startTransition(() => {
        refetchOrganizationMembers({ peopleNameSearchText: nameContains }, { fetchPolicy: 'store-and-network' });
      });
    },
    [refetchOrganizationMembers, startTransition],
  );

  const handleRefetchTeams = useCallback(
    (customerIdValue: string | undefined) => {
      startTransition(() => {
        refetchTeams({ customerId: customerIdValue ?? '', customerExists: !!customerIdValue }, { fetchPolicy: 'store-and-network' });
      });
    },
    [refetchTeams, startTransition],
  );

  const handlePeopleNameSearchTextChange = (value: string) => {
    setPeopleNameSearchText(value);
    handleRefetchOrganizationMembers(value);
  };

  const debounceSearchTextChange = useDebounceCallback(handlePeopleNameSearchTextChange, keyboardSearchDebounceTimeout);

  const goBack = () => {
    if (redirectUrl) {
      router.push(redirectUrl);
      return;
    }

    router.push(getOrganizationBookingsBaseLink(integratedPlatrform, organizationCustomDomain));
  };

  const handleSubmit = ({ date, allDay: allDayValue, member, notes: notesValue, team, resources: selectedResourceIds, category: categoryValue }: BookingDetails) => {
    const [timeFrom, timeUntil] = timeRange;
    const computedDateRange = getDateRange(allDayValue, date, { timeFrom, timeUntil });

    if (!computedDateRange.valid) {
      return;
    }

    if (recurrenceMode === 'recurring') {
      if (recurrenceFrequency === 'WEEKLY' && recurrenceWeekDays.length === 0) {
        setRecurrenceError('Pick at least one day for a weekly recurring booking.');
        return;
      }

      if (recurrenceEndType === 'AFTER_OCCURRENCES' && recurrenceOccurrenceCount < 1) {
        setRecurrenceError('Occurrence count must be at least 1.');
        return;
      }

      setRecurrenceError('');
    }

    const toastId = themedToast(
      <NotificationContent content={`${recurrenceMode === 'recurring' ? 'Creating recurring booking' : 'Making booking'} on '${toShortDate(computedDateRange.from)}'...`} />,
      infoNotificationOptions,
    );

    if (recurrenceMode === 'single') {
      commitAddPrivateBooking({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: uuid(),
            from: computedDateRange.from.toISOString(),
            until: computedDateRange.until.toISOString(),
            notes: notesValue,
            category: categoryValue as AddPrivateBookingCategory,
            customerIds: [member],
            organizationCustomDomains: [organizationCustomDomain],
            teamIds: team ? [team] : [],
            resourceIds: selectedResourceIds ?? [],
          },
        },
        onCompleted: (response, errors) => {
          if (errors?.length) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to make booking. Error: ${getRelayErrorMessage(errors)}.`} />,
            });
            return;
          }

          const booking = response.addPrivateBooking.booking;
          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content={`Booking created for ${getCustomerFullName(booking.involvedCustomers[0])} on ${toShortDate(booking.from)}.`} />,
          });
          goBack();
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to make booking. Error: ${getRelayErrorMessage(error)}.`} />,
          });
        },
      });

      return;
    }

    commitAddPrivateRecurringBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: uuid(),
          category: categoryValue as AddPrivateRecurringBookingCategory,
          customerIds: [member],
          organizationCustomDomains: [organizationCustomDomain],
          teamIds: team ? [team] : [],
          from: computedDateRange.from.toISOString(),
          until: computedDateRange.until.toISOString(),
          startDate: dayjs(date).utc().startOf('day').toISOString(),
          endDate: recurrenceEndType === 'UNTIL_DATE' ? recurrenceEndDate.utc().endOf('day').toISOString() : null,
          endType: recurrenceEndType,
          frequency: recurrenceFrequency,
          interval: recurrenceInterval,
          occurrenceCount: recurrenceEndType === 'AFTER_OCCURRENCES' ? recurrenceOccurrenceCount : null,
          byWeekDays: recurrenceFrequency === 'WEEKLY' ? recurrenceWeekDays : [],
          byMonthDay: recurrenceFrequency === 'MONTHLY' ? dayjs(date).date() : null,
          bySetPosition: null,
          requestedResourceIds: selectedResourceIds ?? [],
          skippedDates: [],
        },
      },
      onCompleted: (response, errors) => {
        if (errors?.length) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to create recurring booking. Error: ${getRelayErrorMessage(errors)}.`} />,
          });
          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Recurring booking created starting ${toShortDate(response.addPrivateRecurringBooking.recurringBooking.startDate)}.`} />,
        });
        goBack();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to create recurring booking. Error: ${getRelayErrorMessage(error)}.`} />,
        });
      },
    });
  };

  return (
    <Box sx={{ px: { xs: 2, md: 3 }, py: 3 }}>
      <Box sx={{ maxWidth: 1320, mx: 'auto', display: 'grid', gridTemplateColumns: { xs: 'minmax(0, 1fr)', xl: 'minmax(0, 2fr) 320px' }, gap: 3 }}>
        <StackColumn spacing={2.5} sx={{ minWidth: 0 }}>
          <PageHeaderPanel
            title="Add booking"
            description="Create a one-time or recurring private booking without leaving the working flow behind."
            actions={
              <ToggleButtonGroup
                size="small"
                exclusive
                value={recurrenceMode}
                onChange={(_, value: RecurrenceMode | null) => {
                  if (value) {
                    setRecurrenceMode(value);
                  }
                }}
              >
                <ToggleButton value="single">One-time</ToggleButton>
                <ToggleButton value="recurring">Recurring</ToggleButton>
              </ToggleButtonGroup>
            }
          />

          <Form
            onSubmit={handleSubmit}
            initialValues={{
              member: customerId,
              date: from,
              allDay,
              notes,
              team: undefined,
              location: locationId,
              resources: resourceIds,
              category,
            }}
            validate={validate}
            render={({ handleSubmit: handleFormSubmit }) => (
              <FormStackColumn onSubmit={handleFormSubmit}>
                <FormSpy
                  subscription={{ values: true }}
                  onChange={({ values: currentValues }) => {
                    if (!currentValues) return;
                    if (currentValues.date !== from) setFrom(currentValues.date);
                    if (currentValues.allDay !== allDay) setAllDay(currentValues.allDay);
                    if (currentValues.notes !== notes) setNotes(currentValues.notes);
                    if (JSON.stringify(currentValues.resources) !== JSON.stringify(resourceIds)) setResourceIds(currentValues.resources);
                    if (currentValues.category !== category) setCategory(currentValues.category);
                  }}
                />

                <SettingsSectionCard title="Booking basics" description="Choose who the booking is for and what kind of work this booking represents.">
                  <StackColumn spacing={2}>
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
                        onChange={(_, option) => {
                          const nextCustomerId = (option as OrganizationMemberDetails | null)?.customer.id;
                          setCustomerId(nextCustomerId);
                          handleRefetchTeams(nextCustomerId);
                        }}
                      />
                    </FormFieldLabel>

                    <FormFieldLabel label="Category">
                      <SingleChoiceBookingCategory rootDataRelay={rootDataMain} name="category" required={requiredFields.category} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Notes">
                      <TextField name="notes" required={requiredFields.notes} multiline rows={3} />
                    </FormFieldLabel>
                  </StackColumn>
                </SettingsSectionCard>

                <SettingsSectionCard
                  title="Schedule"
                  description={recurrenceMode === 'recurring' ? 'Choose the first slot, then define how the booking repeats.' : 'Pick the date and time for this booking.'}
                  actions={recurrenceMode === 'recurring' ? <Chip size="small" label="Recurring series" color="primary" variant="outlined" /> : null}
                >
                  <StackColumn spacing={2}>
                    <FormFieldLabel label="Date and time">
                      <StackColumn spacing={1.5}>
                        <StackRow sx={{ gap: 1.5, alignItems: 'center', flexWrap: 'wrap' }}>
                          <Box sx={{ width: 'fit-content' }}>
                            <DatePicker name="date" required={requiredFields.date} />
                          </Box>
                          <Switches name="allDay" required={requiredFields.allDay} data={{ label: 'All Day', value: 'allDay' }} />
                        </StackRow>
                        <Box sx={{ width: 'fit-content' }}>
                          <TimeRangePicker minutesStep={rootDataMain.bookingSlotSizeInMinutes} disabled={allDay} defaultValue={timeRange} onChange={setTimeRange} />
                        </Box>
                      </StackColumn>
                    </FormFieldLabel>

                    <ErrorTypography errorMessage={dateRange.errorMessage} />

                    {recurrenceMode === 'recurring' ? (
                      <StackColumn spacing={2}>
                        <Divider />

                        <FormFieldLabel label="Frequency">
                          <ToggleButtonGroup
                            size="small"
                            exclusive
                            value={recurrenceFrequency}
                            onChange={(_, value: RecurrenceFrequency | null) => {
                              if (value) {
                                setRecurrenceFrequency(value);
                              }
                            }}
                          >
                            {recurrenceFrequencyOptions.map((option) => (
                              <ToggleButton key={option.value} value={option.value}>
                                {option.label}
                              </ToggleButton>
                            ))}
                          </ToggleButtonGroup>
                        </FormFieldLabel>

                        <FormFieldLabel label="Repeat every">
                          <TextField
                            type="number"
                            value={recurrenceInterval}
                            onChange={(event) => setRecurrenceInterval(Math.max(1, Number(event.target.value) || 1))}
                            helperText={recurrenceFrequency === 'DAILY' ? 'Every N days' : recurrenceFrequency === 'WEEKLY' ? 'Every N weeks' : 'Every N months'}
                          />
                        </FormFieldLabel>

                        {recurrenceFrequency === 'WEEKLY' ? (
                          <FormFieldLabel label="Days">
                            <ToggleButtonGroup
                              size="small"
                              value={recurrenceWeekDays}
                              onChange={(_, value: DayOfWeek[]) => setRecurrenceWeekDays(value.length > 0 ? value : [toRecurringDayOfWeek(from)])}
                            >
                              {dayOfWeekOptions.map((option) => (
                                <ToggleButton key={option.value} value={option.value}>
                                  {option.label}
                                </ToggleButton>
                              ))}
                            </ToggleButtonGroup>
                          </FormFieldLabel>
                        ) : null}

                        <FormFieldLabel label="Series ends">
                          <ToggleButtonGroup
                            size="small"
                            exclusive
                            value={recurrenceEndType}
                            onChange={(_, value: RecurrenceEndType | null) => {
                              if (value) {
                                setRecurrenceEndType(value);
                              }
                            }}
                          >
                            {recurrenceEndTypeOptions.map((option) => (
                              <ToggleButton key={option.value} value={option.value}>
                                {option.label}
                              </ToggleButton>
                            ))}
                          </ToggleButtonGroup>
                        </FormFieldLabel>

                        {recurrenceEndType === 'UNTIL_DATE' ? (
                          <FormFieldLabel label="End date">
                            <TextField
                              type="date"
                              value={recurrenceEndDate.format('YYYY-MM-DD')}
                              onChange={(event) => {
                                const value = dayjs(event.target.value);
                                if (value.isValid()) {
                                  setRecurrenceEndDate(value);
                                }
                              }}
                            />
                          </FormFieldLabel>
                        ) : null}

                        {recurrenceEndType === 'AFTER_OCCURRENCES' ? (
                          <FormFieldLabel label="Occurrences">
                            <TextField
                              type="number"
                              value={recurrenceOccurrenceCount}
                              onChange={(event) => setRecurrenceOccurrenceCount(Math.max(1, Number(event.target.value) || 1))}
                            />
                          </FormFieldLabel>
                        ) : null}

                        <ErrorTypography errorMessage={recurrenceError} />
                      </StackColumn>
                    ) : null}
                  </StackColumn>
                </SettingsSectionCard>

                <SettingsSectionCard title="Assignments" description="Pick the team, location, and any specific resources to reserve for this booking.">
                  <StackColumn spacing={2}>
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
                      />
                    </FormFieldLabel>

                    <FormFieldLabel label="Location">
                      <Autocomplete
                        name="location"
                        multiple={false}
                        required={requiredFields.location}
                        options={locations}
                        getOptionValue={(option) => (option as LocationDetails).id}
                        getOptionLabel={(option: string | LocationDetails) => (option as LocationDetails).name}
                        renderOption={(props, option) => {
                          const castedOption = option as LocationDetails;
                          return (
                            <li {...props} key={castedOption.id}>
                              <BodyIconTypography label={castedOption.name} />
                            </li>
                          );
                        }}
                        filterOptions={(options, params) => filterLocation(options as LocationDetails[], params)}
                        selectOnFocus
                        clearOnBlur
                        handleHomeEndKeys
                        onChange={(_, option) => setLocationId((option as LocationDetails | null)?.id)}
                      />
                    </FormFieldLabel>

                    <FormFieldLabel label="Resources">
                      {resources.length > 0 ? (
                        <Autocomplete
                          name="resources"
                          multiple={true}
                          required={requiredFields.resources}
                          options={resources}
                          getOptionValue={(option) => (option as ResourceDetails).id}
                          getOptionLabel={(option: string | ResourceDetails) => (option as ResourceDetails).name}
                          renderOption={(props, option) => {
                            const castedOption = option as ResourceDetails;

                            return (
                              <li {...props} key={castedOption.id}>
                                <StackRow sx={{ alignItems: 'center' }}>
                                  <BodyIconTypography label={castedOption.name} />
                                  <CustomTags customTags={castedOption.customTags} hideNAText />
                                  <Zones zones={castedOption.zones} hideIcon hideNAText />
                                </StackRow>
                              </li>
                            );
                          }}
                          filterOptions={(options, params) => filterResource(options as ResourceDetails[], params)}
                          selectOnFocus
                          clearOnBlur
                          handleHomeEndKeys
                        />
                      ) : (
                        <BodyIconTypography
                          label={
                            !locationId
                              ? 'Pick a location to load available resources.'
                              : !allDay && (!timeRange[0] || !timeRange[1])
                                ? 'Pick a start and end time to load available resources.'
                                : !dateRange.valid
                                  ? 'Fix the selected time to load availability.'
                                  : 'No resources are available for this slot.'
                          }
                        />
                      )}
                    </FormFieldLabel>
                  </StackColumn>
                </SettingsSectionCard>

                <EditorActionBar
                  secondaryActions={
                    <Button type="button" variant="text" onClick={goBack} sx={{ textTransform: 'none' }}>
                      Cancel
                    </Button>
                  }
                  primaryAction={recurrenceMode === 'recurring' ? 'Create recurring booking' : 'Create booking'}
                />
              </FormStackColumn>
            )}
          />
        </StackColumn>

        <StickyReviewRail title="Booking help" description="Keep the flow focused: basics first, then schedule, then assignment.">
          <SettingsSectionCard title="What this page does" description="This replaces the old dialog with a proper booking flow.">
            <StackColumn spacing={1}>
              <SmallIconTypography label="Use one-time bookings for isolated private reservations." />
              <SmallIconTypography label="Use recurring bookings for repeating desk or room patterns." />
              <SmallIconTypography label="Recurring bookings reserve the requested resources for each generated instance when availability allows." />
            </StackColumn>
          </SettingsSectionCard>

          <SettingsSectionCard title="Current selection" description="A quick summary of what will be created when you submit.">
            <StackColumn spacing={1}>
              <StackRow sx={{ alignItems: 'center' }}>
                <CalendarIcon fontSize="small" />
                <SmallIconTypography
                  label={dateRange.valid ? `${toShortDate(dateRange.from)}${recurrenceMode === 'recurring' ? ' onward' : ''}` : 'Choose a valid date and time'}
                />
              </StackRow>
              <SmallIconTypography label={recurrenceMode === 'single' ? 'One-time booking' : `${recurrenceFrequency.toLowerCase()} recurring booking`} />
              <SmallIconTypography label={customerId ? 'User selected' : 'Pick a user before submitting'} />
              <SmallIconTypography label={locationId ? `Location selected` : 'No location selected yet'} />
              <SmallIconTypography
                label={resourceIds.length > 0 ? `${resourceIds.length} resource${resourceIds.length > 1 ? 's' : ''} requested` : 'No specific resources requested'}
              />
            </StackColumn>
          </SettingsSectionCard>
        </StickyReviewRail>
      </Box>
    </Box>
  );
};

const AddPrivateBookingPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<addPrivateBookingPage_rootQuery>(RootQuery);
  const { organizationCustomDomain } = useKnownParams();
  const searchParams = useSearchParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  const defaultDateValue = searchParams.get('date');
  const defaultLocationId = searchParams.get('locationId') ?? undefined;
  const defaultResourceIdsValue = searchParams.get('resourceIds') ?? '';
  const redirectUrl = searchParams.get('redirectUrl') ?? undefined;

  const defaultDate = useMemo(() => {
    if (defaultDateValue && dayjs(defaultDateValue).isValid()) {
      return dayjs(defaultDateValue);
    }

    return startOfDay();
  }, [defaultDateValue]);

  const defaultResourceIds = useMemo(() => defaultResourceIdsValue.split(',').filter(Boolean), [defaultResourceIdsValue]);

  const initialQueryWindow = useMemo(
    () => ({
      from: defaultDate.toISOString(),
      until: defaultDate.add(1, 'day').toISOString(),
    }),
    [defaultDate],
  );

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        locationId: defaultLocationId ?? '',
        dateFromToGetAvailableResources: initialQueryWindow.from,
        dateUntilToGetAvailableResources: initialQueryWindow.until,
        customerId: '',
        customerExists: false,
        teamsSortingValues: [{ direction: 'ASCENDING', field: 'NAME' }],
        locationsSortingValues: [{ direction: 'ASCENDING', field: 'NAME' }],
        organizationMembersSortingValues: [{ direction: 'ASCENDING', field: 'NAME' }],
      },
      { fetchPolicy: 'store-and-network' },
    );
  }, [defaultLocationId, initialQueryWindow.from, initialQueryWindow.until, loadQuery, organizationCustomDomain]);

  if (!queryReference) {
    return null;
  }

  return (
    <AddPrivateBookingPage
      queryReference={queryReference}
      organizationCustomDomain={organizationCustomDomain}
      defaultDate={defaultDate}
      defaultLocationId={defaultLocationId}
      defaultResourceIds={defaultResourceIds}
      redirectUrl={redirectUrl}
    />
  );
};

export default memo(AddPrivateBookingPageWithRelay);

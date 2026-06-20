import { CustomerAvatar } from '@/components/avatars';
import { CustomTags } from '@/components/customTag';
import { CalendarIcon } from '@/components/icons';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { Zones } from '@/components/zone';
import type { editPrivateRecurringBooking_availableResources_query$key } from '@/queries/__generated__/editPrivateRecurringBooking_availableResources_query.graphql';
import type { editPrivateRecurringBooking_availableResources_refetchableFragment } from '@/queries/__generated__/editPrivateRecurringBooking_availableResources_refetchableFragment.graphql';
import type { editPrivateRecurringBooking_organizationMembers_query$key } from '@/queries/__generated__/editPrivateRecurringBooking_organizationMembers_query.graphql';
import type { editPrivateRecurringBooking_organizationMembers_refetchableFragment } from '@/queries/__generated__/editPrivateRecurringBooking_organizationMembers_refetchableFragment.graphql';
import type { editPrivateRecurringBooking_query$key } from '@/queries/__generated__/editPrivateRecurringBooking_query.graphql';
import type {
  BookingFrequency,
  DayOfWeek,
  editPrivateRecurringBooking_updatePrivateRecurringBookingMutation,
  PrivateRecurringBookingPatchField,
  RecurringBookingEndType,
  BookingCategory as UpdatePrivateRecurringBookingCategory,
} from '@/queries/__generated__/editPrivateRecurringBooking_updatePrivateRecurringBookingMutation.graphql';
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
import { getCustomerFullName, getRelayErrorMessage, isMidnight, keyboardSearchDebounceTimeout, PaletteModeContext, toOpeningHoursFromTime, toShortDate } from '@skedular/shared';
import {
  BodyIconTypography,
  EditorActionBar,
  ErrorTypography,
  FormFieldLabel,
  FormStackColumn,
  PageHeaderPanel,
  SettingsSectionCard,
  SmallIconTypography,
  StackColumn,
  StackRow,
  StickyReviewRail,
} from '@skedular/ui';
import dayjs, { Dayjs } from 'dayjs';
import { Autocomplete, DatePicker, makeRequired, makeValidate, Switches } from 'mui-rff';
import { useRouter } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { Form, FormSpy } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { array, boolean, mixed, object, string } from 'yup';

type Props = {
  rootDataRelay: editPrivateRecurringBooking_query$key;
  rootDataOrganizationMembersRelay: editPrivateRecurringBooking_organizationMembers_query$key;
  rootDataAvailableResourcesRelay: editPrivateRecurringBooking_availableResources_query$key;
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
  location: string | undefined;
  resources: string[];
};

type RecurrenceFrequency = 'DAILY' | 'WEEKLY' | 'MONTHLY';
type RecurrenceEndType = 'NEVER' | 'UNTIL_DATE' | 'AFTER_OCCURRENCES';
type RecurrenceDayOfWeek = 'MONDAY' | 'TUESDAY' | 'WEDNESDAY' | 'THURSDAY' | 'FRIDAY' | 'SATURDAY' | 'SUNDAY';

const bookingSchema = object({
  date: mixed<Dayjs>()
    .test('is-dayjs', 'Date must be a valid Dayjs object', (value) => value != null && dayjs.isDayjs(value))
    .required('Date/Time is required'),
  allDay: boolean(),
  member: string().required('User is required'),
  location: string().notRequired(),
  resources: array().nullable(),
});

const toAllDayBoolean = (value: unknown): boolean => value === true || value === 'allDay' || (Array.isArray(value) && value.includes('allDay'));
const bookingAutosaveDebounceTimeout = 1000;

type RecurrenceState = {
  frequency: RecurrenceFrequency;
  interval: number;
  weekDays: RecurrenceDayOfWeek[];
  endType: RecurrenceEndType;
  endDate: Dayjs;
  occurrenceCount: number;
};

const getChangedRecurringBookingFields = (
  left: BookingDetails | null,
  right: BookingDetails,
  leftTimeRange: DateRange<Dayjs>,
  rightTimeRange: DateRange<Dayjs>,
  leftRecurrence: RecurrenceState,
  rightRecurrence: RecurrenceState,
): PrivateRecurringBookingPatchField[] => {
  if (!left) return [];
  const changed: PrivateRecurringBookingPatchField[] = [];
  if (left.member !== right.member) changed.push('PARTICIPANTS');
  if (JSON.stringify(left.resources) !== JSON.stringify(right.resources)) changed.push('REQUESTED_RESOURCES');
  if (left.date !== right.date || left.allDay !== right.allDay || JSON.stringify(leftTimeRange) !== JSON.stringify(rightTimeRange)) changed.push('SCHEDULE');
  if (JSON.stringify(leftRecurrence) !== JSON.stringify(rightRecurrence)) changed.push('RECURRENCE');
  return changed;
};

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

const dayOfWeekOptions: { value: RecurrenceDayOfWeek; label: string; dayjsDay: number }[] = [
  { value: 'MONDAY', label: 'Mon', dayjsDay: 1 },
  { value: 'TUESDAY', label: 'Tue', dayjsDay: 2 },
  { value: 'WEDNESDAY', label: 'Wed', dayjsDay: 3 },
  { value: 'THURSDAY', label: 'Thu', dayjsDay: 4 },
  { value: 'FRIDAY', label: 'Fri', dayjsDay: 5 },
  { value: 'SATURDAY', label: 'Sat', dayjsDay: 6 },
  { value: 'SUNDAY', label: 'Sun', dayjsDay: 0 },
];

const toRecurringDayOfWeek = (date: Dayjs): RecurrenceDayOfWeek => dayOfWeekOptions.find((item) => item.dayjsDay === date.day())?.value ?? 'MONDAY';

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

const EditPrivateRecurringBooking = ({ rootDataRelay, rootDataOrganizationMembersRelay, rootDataAvailableResourcesRelay }: Props) => {
  const rootData = useFragment<editPrivateRecurringBooking_query$key>(
    graphql`
      fragment editPrivateRecurringBooking_query on Query {
        booking(id: $bookingId) {
          id
          involvedOrganizations {
            id
            name
          }
          involvedLocations {
            uniqueId
            name
          }
          recurringBooking {
            id
            from
            until
            category {
              category
              name
            }
            frequency {
              frequency
              name
            }
            interval
            byMonthDay
            bySetPosition
            byWeekDays {
              dayOfWeek
              name
            }
            endType {
              endType
              name
            }
            startDate
            endDate
            occurrenceCount
            skippedDates
            involvedCustomers {
              id
              name
              givenName
              middleName
              familyName
              photoUrl
            }
            involvedTeams {
              id
              name
            }
            requestedResources {
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
        locations(where: { organizationCustomDomain: $organizationCustomDomain }, orderBy: $locationsSortingValues) {
          edges {
            node {
              id
              name
            }
          }
        }
        bookingSlotSizeInMinutes
      }
    `,
    rootDataRelay,
  );

  const [rootDataOrganizationMembers, refetchOrganizationMembers] = useRefetchableFragment<
    editPrivateRecurringBooking_organizationMembers_refetchableFragment,
    editPrivateRecurringBooking_organizationMembers_query$key
  >(
    graphql`
      fragment editPrivateRecurringBooking_organizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "editPrivateRecurringBooking_organizationMembers_refetchableFragment") {
        organization(customDomain: $organizationCustomDomain) {
          members(first: $count, after: $cursor, where: { nameContains: $peopleNameSearchText }, orderBy: $organizationMembersSortingValues)
            @connection(key: "bookingDetailsSelectorQuery_members") {
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

  const [rootDataAvailableResources, refetchAvailableResources] = useRefetchableFragment<
    editPrivateRecurringBooking_availableResources_refetchableFragment,
    editPrivateRecurringBooking_availableResources_query$key
  >(
    graphql`
      fragment editPrivateRecurringBooking_availableResources_query on Query @refetchable(queryName: "editPrivateRecurringBooking_availableResources_refetchableFragment") {
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
    rootDataAvailableResourcesRelay,
  );

  const [commitUpdatePrivateRecurringBooking] = useMutation<editPrivateRecurringBooking_updatePrivateRecurringBookingMutation>(graphql`
    mutation editPrivateRecurringBooking_updatePrivateRecurringBookingMutation($input: UpdatePrivateRecurringBookingInput!) {
      updatePrivateRecurringBooking(input: $input) {
        recurringBooking {
          id
          startDate
          endDate
          frequency {
            frequency
            name
          }
        }
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [, startTransition] = useTransition();
  const validate = makeValidate(bookingSchema);
  const requiredFields = makeRequired(bookingSchema);

  const booking = rootData.booking;
  const recurringBooking = booking?.recurringBooking;

  const [from, setFrom] = useState<Dayjs>(dayjs(recurringBooking?.from));
  const [allDay, setAllDay] = useState<boolean>(isMidnight(recurringBooking?.from) && isMidnight(recurringBooking?.until));
  const [timeRange, setTimeRange] = useState<DateRange<Dayjs>>([
    recurringBooking ? dayjs(recurringBooking.from) : toOpeningHoursFromTime('00:00'),
    recurringBooking ? dayjs(recurringBooking.until) : toOpeningHoursFromTime('00:00'),
  ]);
  const [peopleNameSearchText, setPeopleNameSearchText] = useState('');
  const [customerId, setCustomerId] = useState<string | undefined>(recurringBooking?.involvedCustomers[0]?.id);
  const [locationId, setLocationId] = useState<string | undefined>(booking?.involvedLocations[0]?.uniqueId);
  const [resourceIds, setResourceIds] = useState<string[]>(recurringBooking?.requestedResources.map((item) => item.id) ?? []);
  const [recurrenceFrequency, setRecurrenceFrequency] = useState<RecurrenceFrequency>((recurringBooking?.frequency.frequency as RecurrenceFrequency | undefined) ?? 'WEEKLY');
  const [recurrenceInterval, setRecurrenceInterval] = useState<number>(recurringBooking?.interval ?? 1);
  const [recurrenceWeekDays, setRecurrenceWeekDays] = useState<RecurrenceDayOfWeek[]>(
    recurringBooking?.byWeekDays.length
      ? recurringBooking.byWeekDays.map((item) => item.dayOfWeek as RecurrenceDayOfWeek)
      : [toRecurringDayOfWeek(dayjs(recurringBooking?.startDate))],
  );
  const [recurrenceEndType, setRecurrenceEndType] = useState<RecurrenceEndType>((recurringBooking?.endType.endType as RecurrenceEndType | undefined) ?? 'NEVER');
  const [recurrenceEndDate, setRecurrenceEndDate] = useState<Dayjs>(
    recurringBooking?.endDate ? dayjs(recurringBooking.endDate) : dayjs(recurringBooking?.startDate).add(1, 'month'),
  );
  const [recurrenceOccurrenceCount, setRecurrenceOccurrenceCount] = useState<number>(recurringBooking?.occurrenceCount ?? 10);
  const [recurrenceError, setRecurrenceError] = useState<string>('');

  const customers = useMemo<OrganizationMemberDetails[]>(
    () => (rootDataOrganizationMembers.organization ? rootDataOrganizationMembers.organization.members.edges.map(({ node }: { node: OrganizationMemberDetails }) => node) : []),
    [rootDataOrganizationMembers.organization],
  );
  const locations = useMemo<LocationDetails[]>(() => rootData.locations.edges.map(({ node }: { node: LocationDetails }) => node), [rootData.locations]);
  const availableResources = useMemo<ResourceDetails[]>(
    () =>
      rootDataAvailableResources.availableResources.map(({ resource }) => ({
        id: resource.id,
        name: resource.name,
        customTags: resource.customTags.map(({ id, name, color }) => ({ id, name, color })),
        zones: resource.zones.map(({ id, name, color }) => ({ id, name, color })),
      })),
    [rootDataAvailableResources.availableResources],
  );

  const resources = useMemo<ResourceDetails[]>(() => {
    if (!recurringBooking) {
      return availableResources;
    }

    return availableResources.concat(
      recurringBooking.requestedResources
        .filter((item) => !availableResources.some((availableResource) => availableResource.id === item.id))
        .map((item) => ({
          id: item.id,
          name: item.name,
          customTags: item.customTags.map(({ id, name, color }) => ({ id, name, color })),
          zones: item.zones.map(({ id, name, color }) => ({ id, name, color })),
        })),
    );
  }, [availableResources, recurringBooking]);

  const dateRange = useMemo(() => {
    const [timeFrom, timeUntil] = timeRange;
    return getDateRange(allDay, from, { timeFrom, timeUntil });
  }, [allDay, from, timeRange]);
  const initialBookingValues = useMemo<BookingDetails | null>(
    () =>
      recurringBooking
        ? {
            member: customerId ?? '',
            date: from,
            allDay,
            location: locationId,
            resources: resourceIds,
          }
        : null,
    [allDay, customerId, from, locationId, recurringBooking, resourceIds],
  );
  const previousBookingValues = useRef<BookingDetails | null>(initialBookingValues);
  const previousBookingTimeRange = useRef<DateRange<Dayjs>>(timeRange);
  const previousRecurrenceState = useRef<RecurrenceState>({
    frequency: recurrenceFrequency,
    interval: recurrenceInterval,
    weekDays: recurrenceWeekDays,
    endType: recurrenceEndType,
    endDate: recurrenceEndDate,
    occurrenceCount: recurrenceOccurrenceCount,
  });
  const debouncedBookingDetailUpdate = useDebounceCallback((save: () => void) => save(), bookingAutosaveDebounceTimeout);

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

  const debounceSearchTextChange = useDebounceCallback((value: string) => {
    setPeopleNameSearchText(value);
    handleRefetchOrganizationMembers(value);
  }, keyboardSearchDebounceTimeout);

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

  if (!booking || !recurringBooking) {
    return null;
  }

  const goBack = () => {
    router.back();
  };

  const handleSubmit = (fieldsToUpdate: PrivateRecurringBookingPatchField[], { date, allDay: allDayValue, member, resources: selectedResourceIds }: BookingDetails) => {
    const [timeFrom, timeUntil] = timeRange;
    const computedDateRange = getDateRange(toAllDayBoolean(allDayValue), date, { timeFrom, timeUntil });

    if (!computedDateRange.valid) {
      return;
    }

    if (recurrenceFrequency === 'WEEKLY' && recurrenceWeekDays.length === 0) {
      setRecurrenceError('Pick at least one day for a weekly recurring booking.');
      return;
    }

    if (recurrenceEndType === 'AFTER_OCCURRENCES' && recurrenceOccurrenceCount < 1) {
      setRecurrenceError('Occurrence count must be at least 1.');
      return;
    }

    setRecurrenceError('');

    commitUpdatePrivateRecurringBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: recurringBooking.id,
          fieldsToUpdate,
          category: recurringBooking.category.category as UpdatePrivateRecurringBookingCategory,
          customerIds: [member],
          organizationIds: booking.involvedOrganizations.map(({ id }) => id),
          teamIds: [],
          from: computedDateRange.from.toISOString(),
          until: computedDateRange.until.toISOString(),
          startDate: dayjs(date).utc().startOf('day').toISOString(),
          endDate: recurrenceEndType === 'UNTIL_DATE' ? recurrenceEndDate.utc().endOf('day').toISOString() : null,
          endType: recurrenceEndType as RecurringBookingEndType,
          frequency: recurrenceFrequency as BookingFrequency,
          interval: recurrenceInterval,
          occurrenceCount: recurrenceEndType === 'AFTER_OCCURRENCES' ? recurrenceOccurrenceCount : null,
          byWeekDays: recurrenceFrequency === 'WEEKLY' ? (recurrenceWeekDays as DayOfWeek[]) : [],
          byMonthDay: recurrenceFrequency === 'MONTHLY' ? dayjs(date).date() : null,
          bySetPosition: recurringBooking.bySetPosition,
          requestedResourceIds: selectedResourceIds ?? [],
          skippedDates: recurringBooking.skippedDates,
        },
      },
      onCompleted: (_, errors) => {
        if (errors?.length) {
          themedToast(<NotificationContent content={`We couldn't update this recurring booking. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't update this recurring booking. ${getRelayErrorMessage(error)}`} />, errorNotificationOptions);
      },
    });
  };

  const pageTitle =
    recurringBooking.involvedCustomers.length > 0 ? `Edit recurring booking — ${getCustomerFullName(recurringBooking.involvedCustomers[0])}` : 'Edit recurring booking';

  return (
    <Box sx={{ px: { xs: 2, md: 3 }, py: 3 }}>
      <Box sx={{ maxWidth: 1320, mx: 'auto', display: 'grid', gridTemplateColumns: { xs: 'minmax(0, 1fr)', xl: 'minmax(0, 2fr) 320px' }, gap: 2 }}>
        <StackColumn spacing={2.5} sx={{ minWidth: 0 }}>
          <PageHeaderPanel
            title={pageTitle}
            description="Redefine the recurring series without converting it into a one-time booking. Use the booking instance editor when you only need to change one occurrence."
            actions={<Chip size="small" label="Recurring series" color="primary" variant="outlined" />}
          />

          <Form
            onSubmit={() => undefined}
            initialValues={initialBookingValues}
            validate={validate}
            render={({ handleSubmit: handleFormSubmit, values }) => {
              const bookingValues = values as BookingDetails;
              const currentRecurrenceState: RecurrenceState = {
                frequency: recurrenceFrequency,
                interval: recurrenceInterval,
                weekDays: recurrenceWeekDays,
                endType: recurrenceEndType,
                endDate: recurrenceEndDate,
                occurrenceCount: recurrenceOccurrenceCount,
              };
              const changedFields = getChangedRecurringBookingFields(
                previousBookingValues.current,
                bookingValues,
                previousBookingTimeRange.current,
                timeRange,
                previousRecurrenceState.current,
                currentRecurrenceState,
              );
              if (changedFields.length > 0) {
                previousBookingValues.current = bookingValues;
                previousBookingTimeRange.current = timeRange;
                previousRecurrenceState.current = currentRecurrenceState;
                debouncedBookingDetailUpdate(() => handleSubmit(changedFields, bookingValues));
              }

              return (
                <FormStackColumn onSubmit={handleFormSubmit}>
                  <FormSpy
                    subscription={{ values: true }}
                    onChange={({ values: currentValues }) => {
                      if (!currentValues) return;
                      if (currentValues.date !== from) setFrom(currentValues.date);
                      const normalizedAllDay = toAllDayBoolean(currentValues.allDay);
                      if (normalizedAllDay !== allDay) setAllDay(normalizedAllDay);
                      if (JSON.stringify(currentValues.resources) !== JSON.stringify(resourceIds)) setResourceIds(currentValues.resources);
                    }}
                  />

                  <SettingsSectionCard title="Booking basics" description="Choose who this recurring booking is for.">
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
                          }}
                        />
                      </FormFieldLabel>
                    </StackColumn>
                  </SettingsSectionCard>

                  <SettingsSectionCard title="Recurring schedule" description="Change the recurring rule for future bookings in this series.">
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
                            <TimeRangePicker minutesStep={rootData.bookingSlotSizeInMinutes} disabled={allDay} value={timeRange} onChange={setTimeRange} />
                          </Box>
                        </StackColumn>
                      </FormFieldLabel>

                      <ErrorTypography errorMessage={dateRange.errorMessage} />

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
                            onChange={(_, value: RecurrenceDayOfWeek[]) => setRecurrenceWeekDays(value.length > 0 ? value : [toRecurringDayOfWeek(from)])}
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
                  </SettingsSectionCard>

                  <SettingsSectionCard title="Assignments" description="Pick the location and requested resources for future bookings in this series.">
                    <StackColumn spacing={2}>
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
                  />
                </FormStackColumn>
              );
            }}
          />
        </StackColumn>

        <StickyReviewRail title="Recurring booking help" description="This editor updates the whole recurring series, not just the current occurrence.">
          <SettingsSectionCard title="How edits work" description="Recurring bookings stay recurring for their whole lifetime.">
            <StackColumn spacing={1}>
              <SmallIconTypography label="Use this page to redefine the recurring rule, customer, or requested resources for the series." />
              <SmallIconTypography label="Use the standard booking editor when you only need to change one generated booking occurrence." />
              <SmallIconTypography label="Deleted occurrences stay skipped because the existing skipped dates are preserved when you update the series." />
            </StackColumn>
          </SettingsSectionCard>

          <SettingsSectionCard title="Current selection" description="A quick summary of the recurring series you are updating.">
            <StackColumn spacing={1}>
              <StackRow sx={{ alignItems: 'center' }}>
                <CalendarIcon fontSize="small" />
                <SmallIconTypography label={dateRange.valid ? `${toShortDate(dateRange.from)} onward` : 'Choose a valid date and time'} />
              </StackRow>
              <SmallIconTypography label={`${recurrenceFrequency.toLowerCase()} recurring booking`} />
              <SmallIconTypography label={customerId ? 'User selected' : 'Pick a user before submitting'} />
              <SmallIconTypography label={locationId ? 'Location selected' : 'No location selected yet'} />
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

export default memo(EditPrivateRecurringBooking);

import { CustomerAvatar } from '@/components/avatars';
import { SingleChoiceBookingCategory } from '@/components/booking';
import { CustomTags } from '@/components/customTag';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { Zones } from '@/components/zone';
import type { editPrivateBooking_availableResources_query$key } from '@/queries/__generated__/editPrivateBooking_availableResources_query.graphql';
import type { editPrivateBooking_availableResources_refetchableFragment } from '@/queries/__generated__/editPrivateBooking_availableResources_refetchableFragment.graphql';
import type { editPrivateBooking_organizationMembers_query$key } from '@/queries/__generated__/editPrivateBooking_organizationMembers_query.graphql';
import type { editPrivateBooking_organizationMembers_refetchableFragment } from '@/queries/__generated__/editPrivateBooking_organizationMembers_refetchableFragment.graphql';
import type { editPrivateBooking_query$key } from '@/queries/__generated__/editPrivateBooking_query.graphql';
import type {
  BookingCategory,
  editPrivateBooking_updatePrivateBookingMutation,
  PrivateBookingPatchField,
} from '@/queries/__generated__/editPrivateBooking_updatePrivateBookingMutation.graphql';
import Box from '@mui/material/Box';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { DateRange } from '@mui/x-date-pickers-pro/models';
import { TimeRangePicker } from '@mui/x-date-pickers-pro/TimeRangePicker';
import {
  getCustomerFullName,
  getOpeningHoursFromDateTime,
  getRelayErrorMessage,
  isMidnight,
  keyboardSearchDebounceTimeout,
  PaletteModeContext,
  toOpeningHoursFromTime,
  toShortDate,
} from '@skedular/shared';
import { BodyIconTypography, defaultPadding, ErrorTypography, FormFieldLabel, FormStackColumn, PageHeaderPanel, SettingsSectionCard, StackColumn, StackRow } from '@skedular/ui';
import dayjs, { Dayjs } from 'dayjs';
import { Autocomplete, DatePicker, makeRequired, makeValidate, Switches, TextField } from 'mui-rff';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { Form, FormSpy } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { array, boolean, mixed, object, string } from 'yup';

type Props = {
  rootDataRelay: editPrivateBooking_query$key;
  rootDataOrganizationMembersRelay: editPrivateBooking_organizationMembers_query$key;
  rootDataAvailableResourcesRelay: editPrivateBooking_availableResources_query$key;
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

type DateRangeValidationResult = {
  valid: boolean;
  from: Dayjs;
  until: Dayjs;
  errorMessage: string;
};

type BookingDetails = {
  date: Dayjs;
  allDay: boolean;
  member: string;
  notes: string | null | undefined;
  location: string | undefined;
  resources: string[];
  category: string;
};

const bookingSchema = object({
  date: mixed<Dayjs>()
    .test('is-dayjs', 'Date must be a valid Dayjs object', (value) => {
      return value != null && dayjs.isDayjs(value);
    })
    .required('Date/Time is required'),
  allDay: boolean(),
  member: string().required('User is required'),
  notes: string().notRequired(),
  location: string().notRequired(),
  resources: array().nullable(),
  category: string().required('Category is required'),
});

const toAllDayBoolean = (value: unknown): boolean => value === true || value === 'allDay' || (Array.isArray(value) && value.includes('allDay'));
const bookingAutosaveDebounceTimeout = 1000;

type PrivateBookingFormField = keyof Pick<BookingDetails, 'member' | 'date' | 'allDay' | 'notes' | 'category' | 'resources'>;
const privateBookingFieldGroups: ReadonlyArray<[PrivateBookingPatchField, ReadonlyArray<PrivateBookingFormField>]> = [
  ['PARTICIPANTS', ['member']],
  ['SCHEDULE', ['date', 'allDay']],
  ['NOTES', ['notes']],
  ['CATEGORY', ['category']],
  ['RESOURCES', ['resources']],
];

const getChangedPrivateBookingFields = (
  left: BookingDetails | null,
  right: BookingDetails,
  leftTimeRange: DateRange<Dayjs>,
  rightTimeRange: DateRange<Dayjs>,
): PrivateBookingPatchField[] => {
  if (!left) return [];
  const changed = privateBookingFieldGroups
    .filter(([, formFields]) => formFields.some((field) => JSON.stringify(left[field]) !== JSON.stringify(right[field])))
    .map(([patchField]) => patchField);
  if (!changed.includes('SCHEDULE') && JSON.stringify(leftTimeRange) !== JSON.stringify(rightTimeRange)) {
    changed.push('SCHEDULE');
  }
  return changed;
};

const EditPrivateBooking = ({ rootDataRelay, rootDataOrganizationMembersRelay, rootDataAvailableResourcesRelay }: Props) => {
  const rootData = useFragment<editPrivateBooking_query$key>(
    graphql`
      fragment editPrivateBooking_query on Query {
        locations(where: { organizationCustomDomain: $organizationCustomDomain }, orderBy: $locationsSortingValues) {
          __id
          totalCount
          edges {
            node {
              id
              name
            }
          }
        }
        booking(id: $bookingId) {
          id
          from
          until
          notes
          category {
            category
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
        bookingSlotSizeInMinutes
        ...singleChoiceBookingCategory_query
      }
    `,
    rootDataRelay,
  );

  const [rootDataOrganizationMembers, refetchOrganizationMembers] = useRefetchableFragment<
    editPrivateBooking_organizationMembers_refetchableFragment,
    editPrivateBooking_organizationMembers_query$key
  >(
    graphql`
      fragment editPrivateBooking_organizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "editPrivateBooking_organizationMembers_refetchableFragment") {
        organization(customDomain: $organizationCustomDomain) {
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

  const [rootDataAvailableResources, refetchAvailableResources] = useRefetchableFragment<
    editPrivateBooking_availableResources_refetchableFragment,
    editPrivateBooking_availableResources_query$key
  >(
    graphql`
      fragment editPrivateBooking_availableResources_query on Query @refetchable(queryName: "editPrivateBooking_availableResources_refetchableFragment") {
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

  const [commitUpdatePrivateBooking] = useMutation<editPrivateBooking_updatePrivateBookingMutation>(graphql`
    mutation editPrivateBooking_updatePrivateBookingMutation($input: UpdatePrivateBookingInput!) @raw_response_type {
      updatePrivateBooking(input: $input) {
        booking {
          id
          from
          until
          notes
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
          involvedOrganizations {
            id
            name
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

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const booking = rootData.booking;
  const [, startTransition] = useTransition();
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const validate = makeValidate(bookingSchema);
  const requiredFields = makeRequired(bookingSchema);
  const [from, setFrom] = useState<Dayjs>(dayjs(rootData.booking?.from));
  const [allDay, setAllDay] = useState<boolean>(isMidnight(rootData.booking?.from) && isMidnight(rootData.booking?.until));
  const [timeRange, setTimeRange] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(getOpeningHoursFromDateTime(rootData.booking?.from)),
    toOpeningHoursFromTime(getOpeningHoursFromDateTime(rootData.booking?.until)),
  ]);
  const [customerId, setCustomerId] = useState<string | undefined>(
    rootData.booking?.involvedCustomers && rootData.booking?.involvedCustomers.length > 0 ? rootData.booking?.involvedCustomers[0].id : undefined,
  );
  const [locationId, setLocationId] = useState<string | undefined>(
    rootData.booking?.involvedLocations && rootData.booking?.involvedLocations.length > 0 ? rootData.booking?.involvedLocations[0].uniqueId : undefined,
  );
  const filterLocation = createFilterOptions<LocationDetails>();
  const filterResource = createFilterOptions<ResourceDetails>();
  const customers = useMemo<OrganizationMemberDetails[]>(
    () => (rootDataOrganizationMembers.organization?.members ? rootDataOrganizationMembers.organization.members.edges.map(({ node }) => node) : []),
    [rootDataOrganizationMembers.organization],
  );
  const locations = useMemo<LocationDetails[]>(() => rootData.locations.edges.map(({ node }) => node), [rootData.locations]);

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

  const handleRefetchAvailableResources = useCallback(
    ({ from, until }: { from: Dayjs | Date; until: Dayjs | Date }, locationId?: string) => {
      startTransition(() => {
        refetchAvailableResources(
          {
            locationId,
            dateFromToGetAvailableResources: dayjs(from).utc().toISOString(),
            dateUntilToGetAvailableResources: dayjs(until).utc().toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [startTransition, refetchAvailableResources],
  );

  const getDateRange = useCallback(
    (allDay: boolean, date: Dayjs | Date, { timeFrom, timeUntil }: { timeFrom: Dayjs | null; timeUntil: Dayjs | null }): DateRangeValidationResult => {
      const allDayFrom = dayjs(date).utc();
      const allDayUntil = dayjs(date).utc().add(1, 'day');
      const invalidResult = (errorMessage: string): DateRangeValidationResult => ({
        valid: false,
        from: allDayFrom,
        until: allDayUntil,
        errorMessage,
      });

      if (allDay) {
        return { valid: true, from: allDayFrom, until: allDayUntil, errorMessage: '' };
      }

      if (!timeFrom || !timeUntil) {
        return invalidResult('Time required when not booking full day.');
      }

      if (isMidnight(timeFrom) && isMidnight(timeUntil)) {
        return { valid: true, from: allDayFrom, until: allDayUntil, errorMessage: '' };
      }

      const utcDate = dayjs(date).utc();
      const from = utcDate.set('hour', timeFrom.get('hour')).set('minute', timeFrom.get('minute'));
      const until = utcDate.set('hour', timeUntil.get('hour')).set('minute', timeUntil.get('minute'));

      if (!from.isValid() || !until.isValid() || from.isAfter(until)) {
        return invalidResult('Time values are incorrect.');
      }

      return {
        valid: true,
        from,
        until,
        errorMessage: '',
      };
    },
    [],
  );
  const dateRangeValidation = useMemo(() => {
    const [timeFrom, timeUntil] = timeRange;

    return getDateRange(allDay, from, { timeFrom, timeUntil });
  }, [allDay, from, timeRange, getDateRange]);
  const { valid: timeRangeValid, errorMessage: dateTimeErrorMessage } = dateRangeValidation;
  const resources = useMemo<ResourceDetails[]>(() => {
    if (!timeRangeValid || !rootDataAvailableResources.availableResources) {
      return [];
    }

    const availableResources = rootDataAvailableResources.availableResources
      .map(({ resource }) => resource)
      .map(({ id, name, customTags, zones }) => ({
        id,
        name,
        customTags: customTags.map(({ id, name, color }) => ({ id, name, color })),
        zones: zones.map(({ id, name, color }) => ({ id, name, color })),
      }));

    if (from && booking?.from) {
      return availableResources.concat(
        booking.bookingResources
          .filter((item) => !availableResources.some((resource) => resource.id === item.resource.id))
          .map(({ resource: { id, name, customTags, zones } }) => ({
            id,
            name,
            customTags: customTags.map(({ id, name, color }) => ({ id, name, color })),
            zones: zones.map(({ id, name, color }) => ({ id, name, color })),
          })),
      );
    }

    return availableResources;
  }, [rootDataAvailableResources.availableResources, timeRangeValid, from, booking]);
  const initialBookingValues = useMemo<BookingDetails | null>(
    () =>
      booking
        ? {
            member: customerId ?? '',
            date: from,
            allDay,
            notes: booking.notes,
            location: locationId,
            resources: booking.bookingResources ? booking.bookingResources.map(({ resource }) => resource.id) : [],
            category: booking.category.category,
          }
        : null,
    [allDay, booking, customerId, from, locationId],
  );
  const previousBookingValues = useRef<BookingDetails | null>(initialBookingValues);
  const previousBookingTimeRange = useRef<DateRange<Dayjs>>(timeRange);

  useEffect(() => {
    if (!dateRangeValidation.valid) {
      return;
    }

    handleRefetchAvailableResources(dateRangeValidation, locationId);
  }, [dateRangeValidation, handleRefetchAvailableResources, locationId]);

  const handleBookingDetailUpdateClick = (
    fieldsToUpdate: PrivateBookingPatchField[],
    { date, allDay, member: memberId, notes, resources: resourceIds, category }: BookingDetails,
  ) => {
    if (!booking || !bookingSchema.isValidSync({ date, allDay, member: memberId, notes, resources: resourceIds, category })) {
      return;
    }

    const start = date as unknown as Dayjs;
    const [timeFrom, timeUntil] = timeRange;
    const dateRange = getDateRange(toAllDayBoolean(allDay), start, { timeFrom, timeUntil });
    if (!dateRange.valid) {
      return;
    }

    const from = dateRange.from.toISOString();
    const until = dateRange.until.toISOString();
    const shortDateTimeFormatFrom = toShortDate(start);

    commitUpdatePrivateBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: booking.id,
          fieldsToUpdate,
          from,
          until,
          notes,
          category: category as BookingCategory,
          customerIds: [memberId],
          organizationIds: booking.involvedOrganizations.map(({ id }) => id),
          teamIds: [],
          resourceIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to update booking '${shortDateTimeFormatFrom}'. Error: ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to update booking '${shortDateTimeFormatFrom}'. Error: ${getRelayErrorMessage(error)}.`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        updatePrivateBooking: {
          booking: {
            id: booking.id,
            from,
            until,
            notes,
            category: {
              category: category as BookingCategory,
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
            // TODO: 20240112 - Morteza: Below line stores the existing/old resource, but not the updated value for optimistic update, update this line with the updated value in future
            bookingResources: booking.bookingResources,
          },
        },
      },
    });
  };
  const debouncedBookingDetailUpdate = useDebounceCallback(handleBookingDetailUpdateClick, bookingAutosaveDebounceTimeout);

  const handleMemberChange = (option: OrganizationMemberDetails | null) => {
    if (!rootData.booking) {
      return;
    }

    const customerId = option?.customer.id;
    setCustomerId(customerId);
  };

  const handleLocationChange = (option: LocationDetails | null) => {
    if (!rootData.booking) {
      return;
    }

    const locationId = option?.id;

    setLocationId(locationId);

    const [timeFrom, timeUntil] = timeRange;
    const range = getDateRange(allDay, from, { timeFrom, timeUntil });
    if (!range.valid) {
      return;
    }

    handleRefetchAvailableResources(range, locationId);
  };

  const handlePeopleNameSearchTextChange = (str: string) => {
    setPeopleNameSearchText(str);

    handleRefetchOrganizationMembers(str);
  };

  const debounceSearchTextChange = useDebounceCallback(handlePeopleNameSearchTextChange, keyboardSearchDebounceTimeout);

  if (!booking) {
    return null;
  }

  const pageTitle = booking.involvedCustomers.length > 0 ? `Edit Booking — ${getCustomerFullName(booking.involvedCustomers[0])}` : 'Edit Booking';

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 0, sm: 1, md: 2 }, pb: defaultPadding }}>
      <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', backgroundColor: 'transparent', gap: 2 }}>
        <PageHeaderPanel eyebrow="Private Booking" title={pageTitle} description="Update the date, time, member, location, and resources for this booking." />

        <Form
          onSubmit={() => undefined}
          initialValues={initialBookingValues}
          validate={validate}
          render={({ handleSubmit, values }) => {
            const bookingValues = values as BookingDetails;
            const changedFields = getChangedPrivateBookingFields(previousBookingValues.current, bookingValues, previousBookingTimeRange.current, timeRange);
            if (changedFields.length > 0) {
              previousBookingValues.current = bookingValues;
              previousBookingTimeRange.current = timeRange;
              debouncedBookingDetailUpdate(changedFields, bookingValues);
            }

            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <FormSpy
                  subscription={{ values: true }}
                  onChange={({ values }) => {
                    const normalizedAllDay = toAllDayBoolean(values?.allDay);

                    if (normalizedAllDay !== allDay) {
                      setAllDay(normalizedAllDay);
                    }
                  }}
                />
                <StackColumn spacing={3}>
                  <SettingsSectionCard title="Booking Details" description="Edit the member, date, time, category, location and resources for this booking.">
                    <StackColumn>
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

                      <FormFieldLabel label="Date/Time">
                        <StackColumn>
                          <StackRow>
                            <Box sx={{ width: 'fit-content' }}>
                              <DatePicker
                                name="date"
                                required={requiredFields.date}
                                fieldProps={{
                                  onChange: (value: unknown) => {
                                    if (value && dayjs.isDayjs(value)) {
                                      setFrom(value);
                                    }
                                  },
                                }}
                              />
                            </Box>
                            <Switches name="allDay" required={requiredFields.allDay} data={{ label: 'All Day', value: 'allDay' }} />
                          </StackRow>

                          <Box sx={{ width: 'fit-content' }}>
                            <TimeRangePicker minutesStep={rootData.bookingSlotSizeInMinutes} disabled={allDay} value={timeRange} onChange={setTimeRange} />
                          </Box>
                        </StackColumn>
                      </FormFieldLabel>

                      <FormFieldLabel>
                        <ErrorTypography errorMessage={dateTimeErrorMessage} />
                      </FormFieldLabel>

                      <FormFieldLabel label="Notes">
                        <TextField name="notes" required={requiredFields.notes} helperText="e.g. I will be half an hour late this morning" multiline rows={2} />
                      </FormFieldLabel>

                      <FormFieldLabel label="Category">
                        <SingleChoiceBookingCategory rootDataRelay={rootData} name="category" required={requiredFields.category} />
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
                          onChange={(_, option) => handleLocationChange(option as LocationDetails)}
                        />
                      </FormFieldLabel>

                      <FormFieldLabel label="Resources">
                        {resources.length > 0 && (
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
                        )}

                        {resources.length === 0 && !locationId && <BodyIconTypography label="There are currently no available resources." />}
                        {resources.length === 0 && locationId && <BodyIconTypography label="There are currently no available resources in the chosen location." />}
                      </FormFieldLabel>
                    </StackColumn>
                  </SettingsSectionCard>
                </StackColumn>
              </FormStackColumn>
            );
          }}
        />
      </StackColumn>
    </Box>
  );
};

export default memo(EditPrivateBooking);

import { CustomerAvatar } from '@/components/avatars';
import { CustomTags } from '@/components/customTag';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { SpacesQuotaUpgradePrompt } from '@/components/booking/spaces-quota-upgrade-prompt';
import { DialogTransition } from '@/components/transitions';
import { Zones } from '@/components/zone';
import type { BookingCategory, newBookingDialog_addPrivateBookingMutation } from '@/queries/__generated__/newBookingDialog_addPrivateBookingMutation.graphql';
import type { newBookingDialog_availableResources_query$key } from '@/queries/__generated__/newBookingDialog_availableResources_query.graphql';
import type { newBookingDialog_availableResources_refetchableFragment } from '@/queries/__generated__/newBookingDialog_availableResources_refetchableFragment.graphql';
import type { newBookingDialog_organizationMembers_query$key } from '@/queries/__generated__/newBookingDialog_organizationMembers_query.graphql';
import type { newBookingDialog_organizationMembers_refetchableFragment } from '@/queries/__generated__/newBookingDialog_organizationMembers_refetchableFragment.graphql';
import type { newBookingDialog_query$key } from '@/queries/__generated__/newBookingDialog_query.graphql';
import Box from '@mui/material/Box';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { DateRange } from '@mui/x-date-pickers-pro/models';
import { TimeRangePicker } from '@mui/x-date-pickers-pro/TimeRangePicker';
import {
  getCustomerFullName,
  getRelayErrorMessage,
  isMidnight,
  keyboardSearchDebounceTimeout,
  PaletteModeContext,
  startOfDay,
  toOpeningHoursFromTime,
  toShortDate,
} from '@skedular/shared';
import { BodyIconTypography, DefaultDialogTitle, ErrorTypography, FormFieldLabel, FormStackColumn, StackColumn, StackRow, TwoButtonsDialogActions } from '@skedular/ui';
import dayjs, { Dayjs } from 'dayjs';
import { Autocomplete, DatePicker, makeRequired, makeValidate, Switches } from 'mui-rff';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { Form, FormSpy } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { array, boolean, mixed, object, string } from 'yup';

type Props = {
  rootDataRelay: newBookingDialog_query$key;
  rootDataOrganizationMembersRelay: newBookingDialog_organizationMembers_query$key;
  rootDataAvailableResourcesRelay: newBookingDialog_availableResources_query$key;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
  organizationCustomDomain: string;
  defaultLocationId?: string;
  defaultDate?: Dayjs;
  defaultResourceIds?: string[];
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

type QuotaErrorDetails = {
  currentUsage: number;
  quotaLimit: number;
  upgradePlans: readonly {
    planCode: number;
    name: string;
    availability: string;
    priceDescription: string | null | undefined;
  }[];
};

const DEFAULT_PRIVATE_BOOKING_CATEGORY = 'WORKING_FROM_OFFICE';
const DEFAULT_PRIVATE_BOOKING_NOTES = '';

const bookingSchema = object({
  date: mixed<Dayjs>()
    .test('is-dayjs', 'Date must be a valid Dayjs object', (value) => {
      return value != null && dayjs.isDayjs(value);
    })
    .required('Date/Time is required'),
  allDay: boolean(),
  member: string().required('User is required'),
  location: string().notRequired(),
  resources: array().nullable(),
});

const NewBookingDialog = ({
  rootDataRelay,
  rootDataOrganizationMembersRelay,
  rootDataAvailableResourcesRelay,
  connectionIds,
  isDialogOpen,
  onAddClicked,
  onCancel,
  organizationCustomDomain,
  defaultLocationId,
  defaultDate,
  defaultResourceIds,
}: Props) => {
  const rootData = useFragment(
    graphql`
      fragment newBookingDialog_query on Query {
        me {
          id
        }
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
        bookingSlotSizeInMinutes
      }
    `,
    rootDataRelay,
  );

  const [rootDataOrganizationMembers, refetchOrganizationMembers] = useRefetchableFragment<
    newBookingDialog_organizationMembers_refetchableFragment,
    newBookingDialog_organizationMembers_query$key
  >(
    graphql`
      fragment newBookingDialog_organizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "newBookingDialog_organizationMembers_refetchableFragment") {
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
    newBookingDialog_availableResources_refetchableFragment,
    newBookingDialog_availableResources_query$key
  >(
    graphql`
      fragment newBookingDialog_availableResources_query on Query @refetchable(queryName: "newBookingDialog_availableResources_refetchableFragment") {
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

  const [commitAddPrivateBooking] = useMutation<newBookingDialog_addPrivateBookingMutation>(graphql`
    mutation newBookingDialog_addPrivateBookingMutation($connectionIds: [ID!]!, $input: AddPrivateBookingInput!) @raw_response_type {
      addPrivateBooking(input: $input) {
        booking @appendNode(connections: $connectionIds, edgeTypeName: "BookingDetails") {
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
        quotaError {
          errorCode
          reasonCode {
            type
            name
          }
          currentUsage
          quotaLimit
          upgradePlans {
            planCode
            name
            availability
            priceDescription
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [, startTransition] = useTransition();
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const validate = makeValidate(bookingSchema);
  const requiredFields = makeRequired(bookingSchema);
  const [from, setFrom] = useState<Dayjs>(defaultDate ?? startOfDay());
  const [allDay, setAllDay] = useState<boolean>(true);
  const [timeRange, setTimeRange] = useState<DateRange<Dayjs>>([toOpeningHoursFromTime('00:00'), toOpeningHoursFromTime('00:00')]);
  // date/time validation message is derived from inputs (computed later)
  const [customerId, setCustomerId] = useState<string | undefined>(rootData.me.id);
  const [locationId, setLocationId] = useState<string | undefined>(defaultLocationId);
  const [resourceIds, setResourceIds] = useState<string[]>(defaultResourceIds ?? []);
  const [quotaError, setQuotaError] = useState<QuotaErrorDetails | null>(null);

  // Note: `resourceIds` is initialized from `defaultResourceIds` and thereafter controlled by the form.
  const filterLocation = createFilterOptions<LocationDetails>();
  const filterResource = createFilterOptions<ResourceDetails>();
  const customers = useMemo<OrganizationMemberDetails[]>(
    () => (rootDataOrganizationMembers.organization ? rootDataOrganizationMembers.organization.members.edges.map(({ node }) => node) : []),
    [rootDataOrganizationMembers.organization],
  );
  const locations = useMemo<LocationDetails[]>(() => rootData.locations.edges.map(({ node }) => node), [rootData.locations]);
  // resources will be derived after validating the time range (see dateRange below)

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

  // Pure validation helper: returns validity, computed from/until (UTC Dayjs) and an error message (if any).
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

  // Derive the validated date range from current inputs. Memoized so we only recompute when inputs change.
  const dateRange = useMemo(() => {
    const [timeFrom, timeUntil] = timeRange;
    return getDateRange(allDay, from, { timeFrom, timeUntil });
  }, [allDay, from, timeRange]);

  // Trigger refetch when the derived dateRange is valid. Error message is derived directly from dateRange.
  useEffect(() => {
    if (dateRange.valid) {
      handleRefetchAvailableResources(dateRange, locationId);
    }
  }, [handleRefetchAvailableResources, dateRange, locationId]);

  const timeRangeValidDerived = dateRange.valid;
  const dateTimeErrorMessageDerived = dateRange.errorMessage || '';

  const resources = useMemo<ResourceDetails[]>(
    () =>
      timeRangeValidDerived
        ? rootDataAvailableResources.availableResources.map(({ resource: { id, name, customTags, zones } }) => ({
            id,
            name,
            customTags: customTags.map(({ id, name, color }) => ({ id, name, color })),
            zones: zones.map(({ id, name, color }) => ({ id, name, color })),
          }))
        : [],
    [rootDataAvailableResources.availableResources, timeRangeValidDerived],
  );

  const handleAddClick = ({ date, allDay, member, location: locationId, resources: resourceIds }: BookingDetails) => {
    const id = uuid();
    const start = date as unknown as Dayjs;
    const [timeFrom, timeUntil] = timeRange;
    const dateRange = getDateRange(allDay, start, { timeFrom, timeUntil });
    if (!dateRange.valid) {
      return;
    }

    const from = dateRange.from.toISOString();
    const until = dateRange.until.toISOString();
    const fromToPrint = toShortDate(dateRange.from);
    const customerId = member ?? rootData.me?.id;

    commitAddPrivateBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          id,
          from,
          until,
          notes: DEFAULT_PRIVATE_BOOKING_NOTES,
          category: DEFAULT_PRIVATE_BOOKING_CATEGORY as BookingCategory,
          customerIds: [customerId],
          organizationCustomDomains: [organizationCustomDomain],
          teamIds: [],
          resourceIds,
        },
      },
      onCompleted: (response, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to make a booking '${fromToPrint}'. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);

          return;
        }

        if (response.addPrivateBooking.quotaError) {
          setQuotaError(response.addPrivateBooking.quotaError);

          return;
        }

        setQuotaError(null);
        onAddClicked();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to make a booking '${fromToPrint}'. Error: ${getRelayErrorMessage(error)}.`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        addPrivateBooking: {
          quotaError: null,
          booking: {
            id,
            from,
            until,
            notes: DEFAULT_PRIVATE_BOOKING_NOTES,
            category: {
              category: DEFAULT_PRIVATE_BOOKING_CATEGORY as BookingCategory,
              name: '',
            },
            involvedCustomers: [
              {
                id: rootData.me.id,
                name: '',
                givenName: '',
                middleName: '',
                familyName: '',
                photoUrl: '',
              },
            ],
            involvedOrganizations: [],
            involvedLocations: locationId ? [{ uniqueId: locationId, name: '' }] : [],
            involvedTeams: [],
            bookingResources: [],
          },
        },
      },
    });
  };

  const handleMemberChange = (option: OrganizationMemberDetails | null) => {
    const customerId = option?.customer.id;

    setCustomerId(customerId);
  };

  const handleLocationChange = (option: LocationDetails | null) => {
    const locationId = option?.id;

    setLocationId(locationId);

    const [timeFrom, timeUntil] = timeRange;
    const range = getDateRange(allDay, from, { timeFrom, timeUntil });
    if (range.valid) {
      handleRefetchAvailableResources(range, locationId);
    } else {
      // invalid range: resources will be empty because validity is derived
    }
  };

  const handlePeopleNameSearchTextChange = (str: string) => {
    setPeopleNameSearchText(str);

    handleRefetchOrganizationMembers(str);
  };

  const debounceSearchTextChange = useDebounceCallback(handlePeopleNameSearchTextChange, keyboardSearchDebounceTimeout);

  return (
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Make a booking" />
      <DialogContent sx={{ marginTop: 2 }}>
        {quotaError && (
          <Box sx={{ mb: 2 }}>
            <SpacesQuotaUpgradePrompt currentUsage={quotaError.currentUsage} quotaLimit={quotaError.quotaLimit} upgradePlans={quotaError.upgradePlans} />
          </Box>
        )}
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            member: customerId,
            date: from,
            allDay,
            location: locationId,
            resources: resourceIds,
          }}
          validate={validate}
          render={({ handleSubmit }) => {
            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <FormSpy
                  subscription={{ values: true }}
                  onChange={({ values }) => {
                    if (!values) return;
                    if (values.date !== from) setFrom(values.date);
                    if (values.allDay !== allDay) setAllDay(values.allDay);
                    if (JSON.stringify(values.resources) !== JSON.stringify(resourceIds)) setResourceIds(values.resources);
                  }}
                />
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
                        <DatePicker name="date" required={requiredFields.date} />
                      </Box>
                      <Switches name="allDay" required={requiredFields.allDay} data={{ label: 'All Day', value: 'allDay' }} />
                    </StackRow>

                    <Box sx={{ width: 'fit-content' }}>
                      <TimeRangePicker minutesStep={rootData.bookingSlotSizeInMinutes} disabled={allDay} defaultValue={timeRange} onChange={setTimeRange} />
                    </Box>
                  </StackColumn>
                </FormFieldLabel>

                <FormFieldLabel>
                  <ErrorTypography errorMessage={dateTimeErrorMessageDerived} />
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

                  {resources.length === 0 && (
                    <BodyIconTypography
                      label={
                        !locationId
                          ? 'Pick a location to load available resources.'
                          : !allDay && (!timeRange[0] || !timeRange[1])
                            ? 'Pick a start and end time to load available resources.'
                            : !timeRangeValidDerived
                              ? 'Time values look off. Adjust them to load availability.'
                              : 'No resources are available for this slot.'
                      }
                    />
                  )}
                </FormFieldLabel>

                <TwoButtonsDialogActions onSecondaryClicked={onCancel} primaryLabel="Add" secondaryLabel="Cancel" />
              </FormStackColumn>
            );
          }}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(NewBookingDialog);

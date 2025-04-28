import { CustomerAvatar } from '@/components/avatars';
import { SingleChoiceBookingType } from '@/components/booking';
import { BodyIconTypography, DefaultDialogTitle, ErrorTypography, FormFieldLabel, FormStackColumn, StackRow, TwoButtonsDialogActions } from '@/components/commons';
import StackColumn from '@/components/commons/stack-column';
import { CustomTags } from '@/components/customTag';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { DialogTransition } from '@/components/transitions';
import { Zones } from '@/components/zone';
import { PaletteModeContext, UpdateGlobalReloadIdContext } from '@/libs/providers';
import { getCustomerFullName, isMidnight, joinErrors, keyboardDebounceTimeout, startOfDay, toOpeningHoursFromTime, toShortDate } from '@/libs/utils';
import type { BookingType, newBookingDialog_addBookingMutation } from '@/queries/__generated__/newBookingDialog_addBookingMutation.graphql';
import type { newBookingDialog_availableResources_query$key } from '@/queries/__generated__/newBookingDialog_availableResources_query.graphql';
import type { newBookingDialog_availableResources_refetchableFragment } from '@/queries/__generated__/newBookingDialog_availableResources_refetchableFragment.graphql';
import type { newBookingDialog_customerTeams_query$key } from '@/queries/__generated__/newBookingDialog_customerTeams_query.graphql';
import type { newBookingDialog_customerTeams_refetchableFragment } from '@/queries/__generated__/newBookingDialog_customerTeams_refetchableFragment.graphql';
import type { newBookingDialog_organizationMembers_query$key } from '@/queries/__generated__/newBookingDialog_organizationMembers_query.graphql';
import type { newBookingDialog_organizationMembers_refetchableFragment } from '@/queries/__generated__/newBookingDialog_organizationMembers_refetchableFragment.graphql';
import type { newBookingDialog_query$key } from '@/queries/__generated__/newBookingDialog_query.graphql';
import Box from '@mui/material/Box';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { DateRange } from '@mui/x-date-pickers-pro/models';
import { TimeRangePicker } from '@mui/x-date-pickers-pro/TimeRangePicker';
import dayjs, { Dayjs } from 'dayjs';
import { Autocomplete, DatePicker, makeRequired, makeValidate, Switches, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { array, boolean, date, object, string } from 'yup';

type Props = {
  rootDataRelay: newBookingDialog_query$key;
  rootDataOrganizationMembersRelay: newBookingDialog_organizationMembers_query$key;
  rootDataTeamsRelay: newBookingDialog_customerTeams_query$key;
  rootDataAvailableResourcesRelay: newBookingDialog_availableResources_query$key;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
  organizationId: string;
  defaultLocationId?: string;
  defaultDate?: Dayjs;
};

type CustomerDetails = {
  uniqueId: string;
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
  uniqueId: string;
  name: string;
  customTags: CustomTagDetails[];
  zones: ZoneDetails[];
};

type BookingDetails = {
  date: Date;
  allDay: boolean;
  member: string;
  notes: string;
  team: string | undefined;
  location: string | undefined;
  resources: string[];
  type: string;
};

const bookingSchema = object({
  date: date().required('Date/Time is required'),
  allDay: boolean(),
  member: string().required('User is required'),
  notes: string().notRequired(),
  team: string().notRequired(),
  location: string().notRequired(),
  resources: array().nullable(),
  type: string().required('Type is required'),
});

const NewBookingDialog = ({
  rootDataRelay,
  rootDataTeamsRelay,
  rootDataOrganizationMembersRelay,
  rootDataAvailableResourcesRelay,
  connectionIds,
  isDialogOpen,
  onAddClicked,
  onCancel,
  organizationId,
  defaultLocationId,
  defaultDate,
}: Props) => {
  const rootData = useFragment(
    graphql`
      fragment newBookingDialog_query on Query {
        me {
          id
        }
        locations(where: { organizationId: $organizationId }, orderBy: $locationsSortingValues) {
          __id
          totalCount
          edges {
            node {
              id
              name
            }
          }
        }
        openingHoursMinutesStep
        ...singleChoiceBookingType_query
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
        organizationMembers(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $peopleNameSearchText }
          orderBy: $organizationMembersSortingValues
        ) @connection(key: "bookingDetailsSelectorQuery_organizationMembers") {
          __id
          totalCount
          edges {
            node {
              id
              customer {
                uniqueId
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
    `,
    rootDataOrganizationMembersRelay,
  );

  const [rootDataTeams, refetchTeams] = useRefetchableFragment<newBookingDialog_customerTeams_refetchableFragment, newBookingDialog_customerTeams_query$key>(
    graphql`
      fragment newBookingDialog_customerTeams_query on Query @refetchable(queryName: "newBookingDialog_customerTeams_refetchableFragment") {
        customerTeams(where: { organizationId: $organizationId, customerId: $customerId }, orderBy: $teamsSortingValues) @include(if: $customerExists) {
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

  const [rootDataAvailableResources, refetchAvailableResources] = useRefetchableFragment<
    newBookingDialog_availableResources_refetchableFragment,
    newBookingDialog_availableResources_query$key
  >(
    graphql`
      fragment newBookingDialog_availableResources_query on Query @refetchable(queryName: "newBookingDialog_availableResources_refetchableFragment") {
        availableResources(where: { organizationId: $organizationId, locationId: $locationId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources }) {
          uniqueId
          name
          customTags {
            uniqueId
            name
            color
          }
          zones {
            uniqueId
            name
            color
          }
        }
      }
    `,
    rootDataAvailableResourcesRelay,
  );

  const [commitAddBooking] = useMutation<newBookingDialog_addBookingMutation>(graphql`
    mutation newBookingDialog_addBookingMutation($connectionIds: [ID!]!, $input: AddBookingInput!) @raw_response_type {
      addBooking(input: $input) {
        booking @appendNode(connections: $connectionIds, edgeTypeName: "BookingDetails") {
          id
          from
          until
          notes
          type {
            type
            name
          }
          involvedCustomers {
            uniqueId
            name
            givenName
            middleName
            familyName
            photoUrl
          }
          involvedOrganizations {
            uniqueId
            name
          }
          involvedLocations {
            uniqueId
            name
          }
          involvedTeams {
            uniqueId
            name
          }
          resources {
            uniqueId
            name
            color
            customTags {
              uniqueId
              name
              color
            }
            zones {
              uniqueId
              name
              color
            }
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const UpdateGlobalReloadId = useContext(UpdateGlobalReloadIdContext);
  const [, startTransition] = useTransition();
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const validate = makeValidate(bookingSchema);
  const requiredFields = makeRequired(bookingSchema);
  const [from, setFrom] = useState<Dayjs | Date>(defaultDate ?? startOfDay());
  const [allDay, setAllDay] = useState<boolean>(true);
  const [timeRange, setTimeRange] = useState<DateRange<Dayjs>>([toOpeningHoursFromTime('00:00'), toOpeningHoursFromTime('00:00')]);
  const [timeRangeValid, setTimeRangeValid] = useState<boolean>(true);
  const [dateTimeErrorMessage, setDateTimeErrorMessage] = useState('');
  const [customerId, setCustomerId] = useState<string | undefined>();
  const [teamId, setTeamId] = useState<string | undefined>();
  const [locationId, setLocationId] = useState<string | undefined>(defaultLocationId);
  const [notes, setNotes] = useState<string>('');
  const [bookingType, setBookingType] = useState<string>('WorkingFromOffice');
  const [resourceIds, setResourceIds] = useState<string[]>([]);
  const filterTeam = createFilterOptions<TeamDetails>();
  const filterLocation = createFilterOptions<LocationDetails>();
  const filterResource = createFilterOptions<ResourceDetails>();
  const customers = useMemo<OrganizationMemberDetails[]>(
    () => rootDataOrganizationMembers.organizationMembers.edges.map(({ node }) => node),
    [rootDataOrganizationMembers.organizationMembers],
  );
  const teams = useMemo<TeamDetails[]>(() => (rootDataTeams.customerTeams ? rootDataTeams.customerTeams.edges.map(({ node }) => node) : []), [rootDataTeams.customerTeams]);
  const locations = useMemo<LocationDetails[]>(() => rootData.locations.edges.map(({ node }) => node), [rootData.locations]);
  const resources = useMemo<ResourceDetails[]>(
    () =>
      timeRangeValid
        ? rootDataAvailableResources.availableResources.map(({ uniqueId, name, customTags, zones }) => ({
            uniqueId,
            name,
            customTags: customTags.map(({ uniqueId: id, name, color }) => ({ id, name, color })),
            zones: zones.map(({ uniqueId: id, name, color }) => ({ id, name, color })),
          }))
        : [],
    [rootDataAvailableResources.availableResources, timeRangeValid],
  );

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
    [refetchOrganizationMembers],
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
    [refetchTeams],
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
    [refetchAvailableResources],
  );

  const getDateRange = useCallback((allDay: boolean, date: Dayjs | Date, { timeFrom, timeUntil }: { timeFrom: Dayjs | null; timeUntil: Dayjs | null }) => {
    const allDayFrom = dayjs(date).utc();
    const allDayUntil = dayjs(date).utc().add(1, 'day');
    const invalidResult = { valid: false, from: allDayFrom, until: allDayUntil };

    if (allDay) {
      setDateTimeErrorMessage('');

      return { valid: true, from: allDayFrom, until: allDayUntil };
    }

    if (!timeFrom || !timeUntil) {
      setDateTimeErrorMessage('Time required when not booking full day.');

      return invalidResult;
    }

    if (isMidnight(timeFrom) && isMidnight(timeUntil)) {
      setDateTimeErrorMessage('');

      return { valid: true, from: allDayFrom, until: allDayUntil };
    }

    const utcDate = dayjs(date).utc();
    const from = utcDate.set('hour', timeFrom.get('hour')).set('minute', timeFrom.get('minute'));
    const until = utcDate.set('hour', timeUntil.get('hour')).set('minute', timeUntil.get('minute'));

    if (from.isAfter(until)) {
      setDateTimeErrorMessage('Time values are incorrect.');

      return invalidResult;
    }

    setDateTimeErrorMessage('');

    return {
      valid: true,
      from,
      until,
    };
  }, []);

  useEffect(() => {
    const [timeFrom, timeUntil] = timeRange;
    const range = getDateRange(allDay, from, { timeFrom, timeUntil });
    if (range.valid) {
      setTimeRangeValid(true);
      handleRefetchAvailableResources(range, locationId);
    } else {
      setTimeRangeValid(false);
    }
  }, [handleRefetchAvailableResources, from, allDay, timeRange, locationId, getDateRange]);

  const handleAddClick = ({ date, allDay, member, notes, team: teamId, location: locationId, resources: resourceIds, type }: BookingDetails) => {
    if (!rootData.me) {
      return;
    }

    const id = nanoid();
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
    const toastId = themedToast(<NotificationContent content={`Making a booking on '${fromToPrint}'...`} />, infoNotificationOptions);

    commitAddBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id,
          from,
          until,
          notes,
          type: type as BookingType,
          customerIds: [customerId],
          organizationIds: [organizationId],
          teamIds: teamId ? [teamId] : [],
          resourceIds,
          productVersionIds: [],
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
        let message = `Booking made for ${getCustomerFullName(booking.involvedCustomers[0])} to work`;

        if (booking.involvedLocations.length > 0) {
          message += ` from the "${booking.involvedLocations[0]!.name}"`;
        }

        if (booking.resources.length > 0) {
          message += ` at resource "${booking.resources.map(({ name }) => name).join(', ')}"`;

          const zones = booking.resources.flatMap(({ zones }) => zones);
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
            until,
            notes,
            type: {
              type: type as BookingType,
              name: '',
            },
            involvedCustomers: [
              {
                uniqueId: rootData.me.id,
                name: '',
                givenName: '',
                middleName: '',
                familyName: '',
                photoUrl: '',
              },
            ],
            involvedOrganizations: organizationId ? [{ uniqueId: organizationId, name: '' }] : [],
            involvedLocations: locationId ? [{ uniqueId: locationId, name: '' }] : [],
            involvedTeams: teamId ? [{ uniqueId: teamId, name: '' }] : [],
            resources: [],
          },
        },
      },
    });
  };

  const handleMemberChange = (option: OrganizationMemberDetails | null) => {
    const customerId = option?.customer.uniqueId;

    setCustomerId(customerId);
    handleRefetchTeams(customerId);
  };

  const handleTeamChange = (option: LocationDetails | null) => {
    setTeamId(option?.id);
  };

  const handleLocationChange = (option: LocationDetails | null) => {
    const locationId = option?.id;

    setLocationId(locationId);

    const [timeFrom, timeUntil] = timeRange;
    const range = getDateRange(allDay, from, { timeFrom, timeUntil });
    if (range.valid) {
      setTimeRangeValid(true);
      handleRefetchAvailableResources(range, locationId);
    } else {
      setTimeRangeValid(false);
    }
  };

  const handlePeopleNameSearchTextChange = (str: string) => {
    setPeopleNameSearchText(str);

    handleRefetchOrganizationMembers(str);
  };

  const debounceSearchTextChange = useDebounceCallback(handlePeopleNameSearchTextChange, keyboardDebounceTimeout);

  if (!rootData.me) {
    return <></>;
  }

  return (
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Make a booking" />
      <DialogContent sx={{ marginTop: 2 }}>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            member: customerId,
            date: from,
            allDay,
            notes,
            team: teamId,
            location: locationId,
            resources: resourceIds,
            type: bookingType,
          }}
          validate={validate}
          render={({ handleSubmit, values }) => {
            setFrom(values.date);
            setAllDay(values.allDay);
            setNotes(values.notes);
            setResourceIds(values.resources);
            setBookingType(values.type);

            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <FormFieldLabel label="User" useWiderSpace>
                  <Autocomplete
                    name="member"
                    multiple={false}
                    required={requiredFields.member}
                    options={customers}
                    getOptionValue={(option) => (option as OrganizationMemberDetails).customer.uniqueId}
                    getOptionLabel={(option: string | OrganizationMemberDetails) => getCustomerFullName((option as OrganizationMemberDetails).customer)}
                    renderOption={(props, option) => {
                      const castedOption = (option as OrganizationMemberDetails).customer;

                      return (
                        <li {...props} key={castedOption.uniqueId}>
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

                <FormFieldLabel label="Date/Time" useWiderSpace>
                  <StackColumn>
                    <StackRow>
                      <Box sx={{ width: 'fit-content' }}>
                        <DatePicker name="date" required={requiredFields.date} />
                      </Box>
                      <Switches name="allDay" required={requiredFields.allDay} data={{ label: 'All Day', value: 'allDay' }} />
                    </StackRow>

                    <Box sx={{ width: 'fit-content' }}>
                      <TimeRangePicker minutesStep={rootData.openingHoursMinutesStep} disabled={allDay} defaultValue={timeRange} onChange={setTimeRange} />
                    </Box>
                  </StackColumn>
                </FormFieldLabel>

                <FormFieldLabel useWiderSpace>
                  <ErrorTypography errorMessage={dateTimeErrorMessage} />
                </FormFieldLabel>

                <FormFieldLabel label="Notes" useWiderSpace>
                  <TextField name="notes" required={requiredFields.notes} helperText="e.g. I will be half an hour late this morning" multiline rows={2} />
                </FormFieldLabel>

                <FormFieldLabel label="Type" useWiderSpace>
                  <SingleChoiceBookingType rootDataRelay={rootData} name="type" required={requiredFields.type} />
                </FormFieldLabel>

                <FormFieldLabel label="Team" useWiderSpace>
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

                <FormFieldLabel label="Location" useWiderSpace>
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

                <FormFieldLabel label="Resources" useWiderSpace>
                  {resources.length > 0 && (
                    <Autocomplete
                      name="resources"
                      multiple={true}
                      required={requiredFields.resources}
                      options={resources}
                      getOptionValue={(option) => (option as ResourceDetails).uniqueId}
                      getOptionLabel={(option: string | ResourceDetails) => (option as ResourceDetails).name}
                      renderOption={(props, option) => {
                        const castedOption = option as ResourceDetails;

                        return (
                          <li {...props} key={castedOption.uniqueId}>
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

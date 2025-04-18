import { CustomerAvatar } from '@/components/avatars';
import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, SectionIconTypography, StackColumn, StackRow } from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { autoCloseErrorNotificationOptions, errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { Zones } from '@/components/zone';
import { PaletteModeContext, UpdateGlobalReloadIdContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { getCustomerFullName, getOpeningHoursFromDateTime, isMidnight, joinErrors, keyboardDebounceTimeout, toOpeningHoursFromTime, toShortDate } from '@/libs/utils';
import type { editBooking_availableResources_query$key } from '@/queries/__generated__/editBooking_availableResources_query.graphql';
import type { editBooking_availableResources_refetchableFragment } from '@/queries/__generated__/editBooking_availableResources_refetchableFragment.graphql';
import type { editBooking_customerTeams_query$key } from '@/queries/__generated__/editBooking_customerTeams_query.graphql';
import type { editBooking_customerTeams_refetchableFragment } from '@/queries/__generated__/editBooking_customerTeams_refetchableFragment.graphql';
import type { editBooking_organizationMembers_query$key } from '@/queries/__generated__/editBooking_organizationMembers_query.graphql';
import type { editBooking_organizationMembers_refetchableFragment } from '@/queries/__generated__/editBooking_organizationMembers_refetchableFragment.graphql';
import type { editBooking_query$key } from '@/queries/__generated__/editBooking_query.graphql';
import type { editBooking_updateBookingMutation } from '@/queries/__generated__/editBooking_updateBookingMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { DateRange } from '@mui/x-date-pickers-pro/models';
import { TimeRangePicker } from '@mui/x-date-pickers-pro/TimeRangePicker';
import dayjs, { Dayjs } from 'dayjs';
import { Autocomplete, DatePicker, makeRequired, makeValidate, Switches, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { array, boolean, date, object, string } from 'yup';

type Props = {
  rootDataRelay: editBooking_query$key;
  rootDataOrganizationMembersRelay: editBooking_organizationMembers_query$key;
  rootDataTeamsRelay: editBooking_customerTeams_query$key;
  rootDataAvailableResourcesRelay: editBooking_availableResources_query$key;
  onReloadRequired?: () => void;
  organizationId: string;
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
  organization: string | undefined;
  team: string | undefined;
  location: string | undefined;
  resources: string[];
};

const bookingSchema = object({
  date: date().required('Date/Time is required'),
  allDay: boolean(),
  member: string().required('User is required'),
  notes: string().notRequired(),
  organization: string().notRequired(),
  team: string().notRequired(),
  location: string().notRequired(),
  resources: array().nullable(),
});

const EditBooking = ({ rootDataRelay, rootDataTeamsRelay, rootDataOrganizationMembersRelay, rootDataAvailableResourcesRelay }: Props) => {
  const rootData = useFragment<editBooking_query$key>(
    graphql`
      fragment editBooking_query on Query {
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
        booking(id: $bookingId) {
          id
          from
          until
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
        openingHoursMinutesStep
      }
    `,
    rootDataRelay,
  );

  const [rootDataOrganizationMembers, refetchOrganizationMembers] = useRefetchableFragment<
    editBooking_organizationMembers_refetchableFragment,
    editBooking_organizationMembers_query$key
  >(
    graphql`
      fragment editBooking_organizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "editBooking_organizationMembers_refetchableFragment") {
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

  const [rootDataTeams, refetchTeams] = useRefetchableFragment<editBooking_customerTeams_refetchableFragment, editBooking_customerTeams_query$key>(
    graphql`
      fragment editBooking_customerTeams_query on Query @refetchable(queryName: "editBooking_customerTeams_refetchableFragment") {
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
    editBooking_availableResources_refetchableFragment,
    editBooking_availableResources_query$key
  >(
    graphql`
      fragment editBooking_availableResources_query on Query @refetchable(queryName: "editBooking_availableResources_refetchableFragment") {
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

  const [commitUpdateBooking] = useMutation<editBooking_updateBookingMutation>(graphql`
    mutation editBooking_updateBookingMutation($input: UpdateBookingInput!) @raw_response_type {
      updateBooking(input: $input) {
        booking {
          id
          from
          until
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

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const UpdateGlobalReloadId = useContext(UpdateGlobalReloadIdContext);
  const [, startTransition] = useTransition();
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const validate = makeValidate(bookingSchema);
  const requiredFields = makeRequired(bookingSchema);
  const [from, setFrom] = useState<Dayjs | Date>(dayjs(rootData.booking?.from));
  const [allDay, setAllDay] = useState<boolean>(isMidnight(rootData.booking?.from) && isMidnight(rootData.booking?.until));
  const [timeRange, setTimeRange] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(getOpeningHoursFromDateTime(rootData.booking?.from)),
    toOpeningHoursFromTime(getOpeningHoursFromDateTime(rootData.booking?.until)),
  ]);
  const [timeRangeValid, setTimeRangeValid] = useState<boolean>(true);
  const [customerId, setCustomerId] = useState<string | undefined>(rootData.booking?.customer?.uniqueId);
  const [teamId, setTeamId] = useState<string | undefined>(rootData.booking?.team?.uniqueId);
  const [locationId, setLocationId] = useState<string | undefined>(rootData.booking?.location?.uniqueId);
  const filterTeam = createFilterOptions<TeamDetails>();
  const filterLocation = createFilterOptions<LocationDetails>();
  const filterResource = createFilterOptions<ResourceDetails>();

  const customers = useMemo<OrganizationMemberDetails[]>(
    () => (rootDataOrganizationMembers.organizationMembers ? rootDataOrganizationMembers.organizationMembers.edges.map(({ node }) => node) : []),
    [rootDataOrganizationMembers.organizationMembers],
  );
  const teams = useMemo<TeamDetails[]>(() => (rootDataTeams.customerTeams ? rootDataTeams.customerTeams.edges.map(({ node }) => node) : []), [rootDataTeams.customerTeams]);
  const locations = useMemo<LocationDetails[]>(() => rootData.locations.edges.map(({ node }) => node), [rootData.locations]);

  const resources = useMemo<ResourceDetails[]>(() => {
    if (!timeRangeValid || !rootDataAvailableResources.availableResources) {
      return [];
    }

    const availableResources = rootDataAvailableResources.availableResources.map(({ uniqueId, name, customTags, zones }) => ({
      uniqueId,
      name,
      customTags: customTags.map(({ uniqueId: id, name, color }) => ({ id, name, color })),
      zones: zones.map(({ uniqueId: id, name, color }) => ({ id, name, color })),
    }));

    if (from && rootData.booking?.from) {
      return availableResources.concat(
        rootData.booking.resources
          .filter((item) => !availableResources.some((resource) => resource.uniqueId === item.uniqueId))
          .map(({ uniqueId, name, customTags, zones }) => ({
            uniqueId,
            name,
            customTags: customTags.map(({ uniqueId: id, name, color }) => ({ id, name, color })),
            zones: zones.map(({ uniqueId: id, name, color }) => ({ id, name, color })),
          })),
      );
    }

    return availableResources;
  }, [rootDataAvailableResources.availableResources, from, timeRangeValid, rootData.booking?.from, rootData.booking?.resources]);

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

  const getDateRange = useCallback(
    (allDay: boolean, date: Dayjs | Date, { timeFrom, timeUntil }: { timeFrom: Dayjs | null; timeUntil: Dayjs | null }) => {
      const allDayFrom = dayjs(date).utc();
      const allDayUntil = dayjs(date).utc().add(1, 'day');

      if (allDay) {
        return { valid: true, from: allDayFrom, until: allDayUntil };
      }

      if (!timeFrom || !timeUntil) {
        themedToast(<NotificationContent content={`Time required when not booking full day.`} />, autoCloseErrorNotificationOptions);

        return { valid: false, from: allDayFrom, until: allDayUntil };
      }

      if (isMidnight(timeFrom) && isMidnight(timeUntil)) {
        return { valid: true, from: allDayFrom, until: allDayUntil };
      }

      const utcDate = dayjs(date).utc();
      const from = utcDate.set('hour', timeFrom.get('hour')).set('minute', timeFrom.get('minute'));
      const until = utcDate.set('hour', timeUntil.get('hour')).set('minute', timeUntil.get('minute'));

      if (from.isAfter(until)) {
        themedToast(<NotificationContent content={`Time values are incorrect.`} />, autoCloseErrorNotificationOptions);

        return { valid: false, from: allDayFrom, until: allDayUntil };
      }

      return {
        valid: true,
        from,
        until,
      };
    },
    [themedToast],
  );

  useEffect(() => handleRefetchTeams(rootData.booking?.customer?.uniqueId), [handleRefetchTeams, rootData.booking?.customer?.uniqueId]);
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

  const handleCloseClick = () => {
    router.back();
  };

  const handleBookingDetailUpdateClick = ({
    date,
    allDay,
    member: memberId,
    notes,
    organization: organizationId,
    location: locationId,
    team: teamId,
    resources: resourceIds,
  }: BookingDetails) => {
    if (!rootData.booking) {
      return;
    }

    const booking = rootData.booking;
    const start = date as unknown as Dayjs;
    const [timeFrom, timeUntil] = timeRange;
    const dateRange = getDateRange(allDay, start, { timeFrom, timeUntil });
    if (!dateRange.valid) {
      return;
    }

    const from = dateRange.from.toISOString();
    const until = dateRange.until.toISOString();
    const shortDateTimeFormatFrom = toShortDate(start);
    const type = booking.type;

    let bookingDetailsInfo = `for ${getCustomerFullName(booking.customer)}`;
    if (booking.location) {
      bookingDetailsInfo += ` at the "${booking.location!.name}"`;
    }

    bookingDetailsInfo += ` on ${toShortDate(dateRange.from)}`;

    const toastId = themedToast(<NotificationContent content={`Updating booking '${bookingDetailsInfo}'...`} />, infoNotificationOptions);

    commitUpdateBooking({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: booking.id,
          customerId: memberId,
          from,
          until,
          notes,
          organizationId,
          locationId,
          teamId,
          resourceIds,
          type,
          productVersionIds: [],
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

        UpdateGlobalReloadId();
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
            from,
            until,
            notes,
            type,
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
            // TODO: 20240112 - Morteza: Below line stores the existing/old resource, but not the updated value for optimistic update, update this line with the updated value in future
            resources: booking.resources,
          },
        },
      },
    });
  };

  const handleMemberChange = (option: OrganizationMemberDetails | null) => {
    if (!rootData.booking) {
      return;
    }

    const customerId = option?.customer.uniqueId;
    setCustomerId(customerId);
    handleRefetchTeams(customerId);
  };

  const handleTeamChange = (option: LocationDetails | null) => {
    if (!rootData.booking) {
      return;
    }

    setTeamId(option?.id);
  };

  const handleLocationChange = (option: LocationDetails | null) => {
    if (!rootData.booking) {
      return;
    }

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

  if (!rootData.booking) {
    return <></>;
  }

  const booking = rootData.booking;

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Booking Information">
          <Form
            onSubmit={handleBookingDetailUpdateClick}
            initialValues={{
              member: customerId,
              date: from,
              allDay,
              notes: booking.notes,
              team: teamId,
              location: locationId,
              resources: booking.resources ? booking.resources.map(({ uniqueId }) => uniqueId) : [],
            }}
            validate={validate}
            render={({ handleSubmit, values }) => {
              setFrom(values.date);
              setAllDay(values.allDay);

              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <SectionIconTypography label="Edit Booking" />
                    <BodyIconTypography label="Edit your booking details" />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="User">
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

                    <FormFieldLabel label="Date/Time">
                      <StackRow>
                        <Box sx={{ width: 'fit-content' }}>
                          <DatePicker name="date" required={requiredFields.date} />
                        </Box>
                        <Switches name="allDay" required={requiredFields.allDay} data={{ label: 'All Day', value: 'allDay' }} />
                        <TimeRangePicker minutesStep={rootData.openingHoursMinutesStep} disabled={allDay} defaultValue={timeRange} onChange={setTimeRange} />
                      </StackRow>
                    </FormFieldLabel>

                    <FormFieldLabel label="Notes">
                      <TextField name="notes" required={requiredFields.notes} helperText="e.g. I will be half an hour late this morning" multiline rows={2} />
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
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <StackRow>
                      <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                        Update
                      </Button>
                    </StackRow>
                  </StackColumn>
                </FormStackColumn>
              );
            }}
          />
        </AppBarWithStackColumn>
      </Box>
    </Box>
  );
};

export default memo(EditBooking);

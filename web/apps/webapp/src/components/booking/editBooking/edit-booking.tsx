import type { editBooking_availableLocationDesks_query$key } from '@/queries/__generated__/editBooking_availableLocationDesks_query.graphql';
import type { editBooking_availableLocationDesks_refetchableFragment } from '@/queries/__generated__/editBooking_availableLocationDesks_refetchableFragment.graphql';
import type { editBooking_customerTeams_query$key } from '@/queries/__generated__/editBooking_customerTeams_query.graphql';
import type { editBooking_customerTeams_refetchableFragment } from '@/queries/__generated__/editBooking_customerTeams_refetchableFragment.graphql';
import type { editBooking_organizationMembers_query$key } from '@/queries/__generated__/editBooking_organizationMembers_query.graphql';
import type { editBooking_organizationMembers_refetchableFragment } from '@/queries/__generated__/editBooking_organizationMembers_refetchableFragment.graphql';
import type { editBooking_query$key } from '@/queries/__generated__/editBooking_query.graphql';
import type { editBooking_updateBookingMutation } from '@/queries/__generated__/editBooking_updateBookingMutation.graphql';
import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import {
  BodyIconTypography,
  FormFieldLabel,
  SectionIconTypography,
  StackColumn,
  StackColumnWithSaveExitCancelAppBar,
  StackRow,
} from '@repo/shared/components/commons';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { Zones } from '@repo/shared/components/zone';
import { PaletteModeContext, UpdateGlobalReloadIdContext } from '@repo/shared/libs/providers';
import { defaultPadding } from '@repo/shared/libs/theme';
import { endOfDay, getCustomerFullName, joinErrors, keyboardDebounceTimeout, toShortDate } from '@repo/shared/libs/utils';
import dayjs, { Dayjs } from 'dayjs';
import { Autocomplete, DatePicker, makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, usePaginationFragment, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { array, date, object, string } from 'yup';

type Props = {
  rootDataRelay: editBooking_query$key;
  rootDataOrganizationMembersRelay: editBooking_organizationMembers_query$key;
  rootDataTeamsRelay: editBooking_customerTeams_query$key;
  rootDataAvailableLocationDesksRelay: editBooking_availableLocationDesks_query$key;
  onReloadRequired?: () => void;
  organizationId?: string;
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

type ZoneDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type DeskDetails = {
  uniqueId: string;
  name: string;
  zones: ZoneDetails[];
};

type BookingDetails = {
  date: Date;
  member: string;
  notes: string;
  organization: string | undefined;
  team: string | undefined;
  location: string | undefined;
  desks: string[];
};

const bookingSchema = object({
  date: date().required(),
  member: string().required(),
  notes: string().notRequired(),
  organization: string().notRequired(),
  team: string().notRequired(),
  location: string().notRequired(),
  desk: array().nullable(),
});

const EditBooking = ({
  rootDataRelay,
  rootDataTeamsRelay,
  rootDataOrganizationMembersRelay,
  rootDataAvailableLocationDesksRelay,
  organizationId,
}: Props) => {
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
    `,
    rootDataRelay,
  );

  const { data: rootDataOrganizationMembers, refetch: refetchOrganizationMembers } = usePaginationFragment<
    editBooking_organizationMembers_refetchableFragment,
    editBooking_organizationMembers_query$key
  >(
    graphql`
      fragment editBooking_organizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 20 })
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
        customerTeams(where: { organizationId: $organizationId, customerId: $customerId }, orderBy: $teamsSortingValues)
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

  const [rootDataAvailableLocationDesks, refetchAvailableLocationDesks] = useRefetchableFragment<
    editBooking_availableLocationDesks_refetchableFragment,
    editBooking_availableLocationDesks_query$key
  >(
    graphql`
      fragment editBooking_availableLocationDesks_query on Query @refetchable(queryName: "editBooking_availableLocationDesks_refetchableFragment") {
        availableDesks(where: { locationId: $locationId, date: $dateToGetAvailableDesks, deskIdsToInclude: $deskIdsToIncludeToGetAvailableDesks })
          @include(if: $locationExists) {
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
    rootDataAvailableLocationDesksRelay,
  );

  const [commitUpdateBooking] = useMutation<editBooking_updateBookingMutation>(graphql`
    mutation editBooking_updateBookingMutation($input: UpdateBookingInput!) @raw_response_type {
      updateBooking(input: $input) {
        booking {
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
  const [, setPage] = useState(0);
  const [pageSize] = useState(20);
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const validate = makeValidate(bookingSchema);
  const requiredFields = makeRequired(bookingSchema);
  const [from, setFrom] = useState<Dayjs | Date>(dayjs(rootData.booking?.from));
  const [customerId, setCustomerId] = useState<string | undefined>(rootData.booking?.customer?.uniqueId);
  const [teamId, setTeamId] = useState<string | undefined>(rootData.booking?.team?.uniqueId);
  const [locationId, setLocationId] = useState<string | undefined>(rootData.booking?.location?.uniqueId);
  const defaultDeskIds = useMemo<string[]>(
    () =>
      rootData.booking?.desks && from.toISOString() === rootData.booking.from && rootData.booking.location?.uniqueId === locationId
        ? rootData.booking.desks.map(({ uniqueId }) => uniqueId)
        : [],
    [rootData.booking?.desks, rootData.booking?.from, from, rootData.booking?.location?.uniqueId, locationId],
  );
  const filterTeam = createFilterOptions<TeamDetails>();
  const filterLocation = createFilterOptions<LocationDetails>();
  const filterDesk = createFilterOptions<DeskDetails>();

  const customers = useMemo<OrganizationMemberDetails[]>(() => {
    if (!rootDataOrganizationMembers.organizationMembers) {
      return [];
    }

    return rootDataOrganizationMembers.organizationMembers.edges.map(({ node }) => node);
  }, [rootDataOrganizationMembers.organizationMembers]);

  const teams = useMemo<TeamDetails[]>(
    () => (rootDataTeams.customerTeams ? rootDataTeams.customerTeams.edges.map(({ node }) => node) : []),
    [rootDataTeams.customerTeams],
  );
  const locations = useMemo<LocationDetails[]>(
    () => (rootData.locations ? rootData.locations.edges.map(({ node }) => node) : []),
    [rootData.locations],
  );

  const desks = useMemo<DeskDetails[]>(() => {
    if (!rootDataAvailableLocationDesks.availableDesks) {
      return [];
    }

    return rootDataAvailableLocationDesks.availableDesks.map(({ uniqueId, name, zones }) => ({
      uniqueId,
      name,
      zones: zones.map(({ uniqueId: id, name, color }) => ({ id, name, color })),
    }));
  }, [rootDataAvailableLocationDesks.availableDesks]);

  const handleRefetchOrganizationMembers = useCallback(
    (peopleNameSearchText: string) => {
      startTransition(() => {
        refetchOrganizationMembers(
          {
            count: pageSize,
            peopleNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
            onComplete: () => {
              setPage(0);
            },
          },
        );
      });
    },
    [refetchOrganizationMembers, pageSize],
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

  const handleRefetchAvailableLocationDesks = useCallback(
    (deskIds: string[], from: Dayjs | Date, locationId?: string) => {
      startTransition(() => {
        refetchAvailableLocationDesks(
          {
            locationId: locationId ?? '',
            locationExists: !!locationId,
            deskIdsToIncludeToGetAvailableDesks: deskIds,
            dateToGetAvailableDesks: from,
          },
          {
            fetchPolicy: 'store-and-network',
            onComplete: () => {
              setPage(0);
            },
          },
        );
      });
    },
    [refetchAvailableLocationDesks],
  );

  useEffect(() => handleRefetchTeams(rootData.booking?.customer?.uniqueId), [handleRefetchTeams, rootData.booking?.customer?.uniqueId]);
  useEffect(
    () => handleRefetchAvailableLocationDesks(defaultDeskIds, from, locationId),
    [defaultDeskIds, handleRefetchAvailableLocationDesks, from, locationId],
  );

  const handleCancelClick = () => {
    router.back();
  };

  const handleSaveClick = ({
    date,
    member: memberId,
    notes,
    organization: organizationId,
    location: locationId,
    team: teamId,
    desks: deskIds,
  }: BookingDetails) => {
    if (!rootData.booking) {
      return;
    }

    const booking = rootData.booking;
    const start = date as unknown as Dayjs;
    const from = start.toISOString();
    const to = endOfDay(start).toISOString();
    const shortDateTimeFormatFrom = toShortDate(start);
    const type = booking.type;
    const shortDateFormatFrom = toShortDate(booking.from);

    let bookingDetailsInfo = `for ${getCustomerFullName(booking.customer)}`;
    if (booking.location) {
      bookingDetailsInfo += ` at the "${booking.location!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    const toastId = themedToast(<NotificationContent content={`Updating booking '${bookingDetailsInfo}'...`} />, infoNotificationOptions);

    commitUpdateBooking({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: booking.id,
          customerId: memberId,
          from,
          to,
          notes,
          organizationId,
          locationId,
          teamId,
          deskIds: locationId
            ? deskIds.filter((deskId) => rootDataAvailableLocationDesks.availableDesks?.find((availableDesk) => availableDesk.uniqueId === deskId))
            : [],
          type,
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
            to,
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
            // TODO: 20240112 - Morteza: Below line stores the existing/old desk, but not the updated value for optimistic update, update this line with the updated value in future
            desks: booking.desks,
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

    const teamId = option?.id;
    setTeamId(teamId);
  };

  const handleLocationChange = (option: LocationDetails | null) => {
    if (!rootData.booking) {
      return;
    }

    const locationId = option?.id;
    setLocationId(locationId);

    const deskIds =
      rootData.booking?.desks && from.toISOString() === rootData.booking.from && rootData.booking.location?.uniqueId === locationId
        ? rootData.booking.desks.map(({ uniqueId }) => uniqueId)
        : [];

    handleRefetchAvailableLocationDesks(deskIds, from, locationId);
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
        <Form
          onSubmit={handleSaveClick}
          initialValues={{
            date: from,
            notes: booking.notes,
            member: customerId,
            team: teamId,
            location: locationId,
            desks: booking.desks ? booking.desks.map(({ uniqueId }) => uniqueId) : [],
          }}
          validate={validate}
          render={({ handleSubmit, values }) => {
            setFrom(values.date);

            return (
              <StackColumnWithSaveExitCancelAppBar onSubmit={handleSubmit} onCancel={handleCancelClick} label="Edit Booking Information">
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <SectionIconTypography label="Edit Booking" />
                  <BodyIconTypography label="Edit your booking details" />
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <FormFieldLabel label="Date">
                    <DatePicker name="date" required={requiredFields.date} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Notes">
                    <TextField
                      name="notes"
                      required={requiredFields.notes}
                      helperText="e.g. I will be half an hour late this morning"
                      multiline
                      rows={2}
                    />
                  </FormFieldLabel>

                  <FormFieldLabel label="User">
                    <Autocomplete
                      name="member"
                      multiple={false}
                      required={requiredFields.member}
                      options={customers}
                      getOptionValue={(option) => (option as OrganizationMemberDetails).customer.uniqueId}
                      getOptionLabel={(option: string | OrganizationMemberDetails) =>
                        getCustomerFullName((option as OrganizationMemberDetails).customer)
                      }
                      renderOption={(props, option) => {
                        const castedOption = (option as OrganizationMemberDetails).customer;

                        return (
                          <li {...props}>
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
                          <li {...props}>
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
                          <li {...props}>
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

                  {locationId && (
                    <FormFieldLabel label="Desks">
                      {desks.length > 0 && (
                        <Autocomplete
                          name="desks"
                          multiple={true}
                          required={requiredFields.desks}
                          options={desks}
                          getOptionValue={(option) => (option as DeskDetails).uniqueId}
                          getOptionLabel={(option: string | DeskDetails) => (option as DeskDetails).name}
                          renderOption={(props, option) => {
                            const castedOption = option as DeskDetails;

                            return (
                              <li {...props}>
                                <StackRow sx={{ alignItems: 'center' }}>
                                  <BodyIconTypography label={castedOption.name} />
                                  <Zones zones={castedOption.zones} />
                                </StackRow>
                              </li>
                            );
                          }}
                          disableCloseOnSelect
                          filterOptions={(options, params) => filterDesk(options as DeskDetails[], params)}
                          selectOnFocus
                          clearOnBlur
                          handleHomeEndKeys
                        />
                      )}

                      {desks.length === 0 && <BodyIconTypography label="There are currently no available desks in the chosen location." />}
                    </FormFieldLabel>
                  )}
                </StackColumn>
              </StackColumnWithSaveExitCancelAppBar>
            );
          }}
        />
      </Box>
    </Box>
  );
};

export default memo(EditBooking);

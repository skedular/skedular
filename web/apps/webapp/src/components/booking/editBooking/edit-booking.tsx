import { CustomerAvatar } from '@/components/avatars';
import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, SectionIconTypography, StackColumn, StackRow } from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { Zones } from '@/components/zone';
import { PaletteModeContext, UpdateGlobalReloadIdContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { endOfDay, getCustomerFullName, joinErrors, keyboardDebounceTimeout, toShortDate } from '@/libs/utils';
import type { editBooking_availableLocationDesks_query$key } from '@/queries/__generated__/editBooking_availableLocationDesks_query.graphql';
import type { editBooking_availableLocationDesks_refetchableFragment } from '@/queries/__generated__/editBooking_availableLocationDesks_refetchableFragment.graphql';
import type { editBooking_availableLocationRooms_query$key } from '@/queries/__generated__/editBooking_availableLocationRooms_query.graphql';
import type { editBooking_availableLocationRooms_refetchableFragment } from '@/queries/__generated__/editBooking_availableLocationRooms_refetchableFragment.graphql';
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
  rootDataAvailableLocationRoomsRelay: editBooking_availableLocationRooms_query$key;
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

type DeskDetails = {
  uniqueId: string;
  name: string;
  customTags: CustomTagDetails[];
  zones: ZoneDetails[];
};

type RoomDetails = {
  uniqueId: string;
  name: string;
  customTags: CustomTagDetails[];
  zones: ZoneDetails[];
};

type ResourceDetails = {
  uniqueId: string;
  name: string;
  customTags: CustomTagDetails[];
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
  rooms: string[];
  resources: string[];
};

const bookingSchema = object({
  date: date().required(),
  member: string().required(),
  notes: string().notRequired(),
  organization: string().notRequired(),
  team: string().notRequired(),
  location: string().notRequired(),
  desks: array().nullable(),
  rooms: array().nullable(),
  resources: array().nullable(),
});

const EditBooking = ({
  rootDataRelay,
  rootDataTeamsRelay,
  rootDataOrganizationMembersRelay,
  rootDataAvailableLocationDesksRelay,
  rootDataAvailableLocationRoomsRelay,
  rootDataAvailableResourcesRelay,
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
          rooms {
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

  const [rootDataAvailableLocationDesks, refetchAvailableLocationDesks] = useRefetchableFragment<
    editBooking_availableLocationDesks_refetchableFragment,
    editBooking_availableLocationDesks_query$key
  >(
    graphql`
      fragment editBooking_availableLocationDesks_query on Query @refetchable(queryName: "editBooking_availableLocationDesks_refetchableFragment") {
        availableDesks(where: { locationId: $locationId, date: $dateToGetAvailableDesks, deskIdsToInclude: $deskIdsToIncludeToGetAvailableDesks }) @include(if: $locationExists) {
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

  const [rootDataAvailableLocationRooms, refetchAvailableLocationRooms] = useRefetchableFragment<
    editBooking_availableLocationRooms_refetchableFragment,
    editBooking_availableLocationRooms_query$key
  >(
    graphql`
      fragment editBooking_availableLocationRooms_query on Query @refetchable(queryName: "editBooking_availableLocationRooms_refetchableFragment") {
        availableRooms(where: { locationId: $locationId, date: $dateToGetAvailableRooms, roomIdsToInclude: $roomIdsToIncludeToGetAvailableRooms }) @include(if: $locationExists) {
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
    rootDataAvailableLocationRoomsRelay,
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
          rooms {
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
  const validateBookingDetails = makeValidate(bookingSchema);
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
  const defaultRoomIds = useMemo<string[]>(
    () =>
      rootData.booking?.rooms && from.toISOString() === rootData.booking.from && rootData.booking.location?.uniqueId === locationId
        ? rootData.booking.rooms.map(({ uniqueId }) => uniqueId)
        : [],
    [rootData.booking?.rooms, rootData.booking?.from, from, rootData.booking?.location?.uniqueId, locationId],
  );
  const filterTeam = createFilterOptions<TeamDetails>();
  const filterLocation = createFilterOptions<LocationDetails>();
  const filterDesk = createFilterOptions<DeskDetails>();
  const filterRoom = createFilterOptions<RoomDetails>();
  const filterResource = createFilterOptions<ResourceDetails>();

  const customers = useMemo<OrganizationMemberDetails[]>(
    () => (rootDataOrganizationMembers.organizationMembers ? rootDataOrganizationMembers.organizationMembers.edges.map(({ node }) => node) : []),
    [rootDataOrganizationMembers.organizationMembers],
  );
  const teams = useMemo<TeamDetails[]>(() => (rootDataTeams.customerTeams ? rootDataTeams.customerTeams.edges.map(({ node }) => node) : []), [rootDataTeams.customerTeams]);
  const locations = useMemo<LocationDetails[]>(() => (rootData.locations ? rootData.locations.edges.map(({ node }) => node) : []), [rootData.locations]);

  const desks = useMemo<DeskDetails[]>(
    () =>
      rootDataAvailableLocationDesks.availableDesks
        ? rootDataAvailableLocationDesks.availableDesks.map(({ uniqueId, name, customTags, zones }) => ({
            uniqueId,
            name,
            customTags: customTags.map(({ uniqueId: id, name, color }) => ({ id, name, color })),
            zones: zones.map(({ uniqueId: id, name, color }) => ({ id, name, color })),
          }))
        : [],
    [rootDataAvailableLocationDesks.availableDesks],
  );

  const rooms = useMemo<RoomDetails[]>(
    () =>
      rootDataAvailableLocationRooms.availableRooms
        ? rootDataAvailableLocationRooms.availableRooms.map(({ uniqueId, name, customTags, zones }) => ({
            uniqueId,
            name,
            customTags: customTags.map(({ uniqueId: id, name, color }) => ({ id, name, color })),
            zones: zones.map(({ uniqueId: id, name, color }) => ({ id, name, color })),
          }))
        : [],
    [rootDataAvailableLocationRooms.availableRooms],
  );

  const resources = useMemo<ResourceDetails[]>(() => {
    if (!rootDataAvailableResources.availableResources) {
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
  }, [rootDataAvailableResources.availableResources, from, rootData.booking?.from, rootData.booking?.resources]);

  const handleRefetchOrganizationMembers = useCallback(
    (peopleNameSearchText: string) => {
      startTransition(() => {
        refetchOrganizationMembers(
          {
            count: 20,
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
          },
        );
      });
    },
    [refetchAvailableLocationDesks],
  );

  const handleRefetchAvailableLocationRooms = useCallback(
    (roomIds: string[], from: Dayjs | Date, locationId?: string) => {
      startTransition(() => {
        refetchAvailableLocationRooms(
          {
            locationId: locationId ?? '',
            locationExists: !!locationId,
            roomIdsToIncludeToGetAvailableRooms: roomIds,
            dateToGetAvailableRooms: from,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchAvailableLocationRooms],
  );

  const handleRefetchAvailableResources = useCallback(
    (from: Dayjs | Date, locationId?: string) => {
      startTransition(() => {
        refetchAvailableResources(
          {
            locationId,
            dateFromToGetAvailableResources: from,
            dateUntilToGetAvailableResources: endOfDay(from).toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchAvailableResources],
  );

  useEffect(() => handleRefetchTeams(rootData.booking?.customer?.uniqueId), [handleRefetchTeams, rootData.booking?.customer?.uniqueId]);
  useEffect(() => {
    handleRefetchAvailableLocationDesks(defaultDeskIds, from, locationId);
    handleRefetchAvailableLocationRooms(defaultRoomIds, from, locationId);
    handleRefetchAvailableResources(from, locationId);
  }, [defaultDeskIds, handleRefetchAvailableLocationDesks, defaultRoomIds, handleRefetchAvailableLocationRooms, handleRefetchAvailableResources, from, locationId]);

  const handleCloseClick = () => {
    router.back();
  };

  const handleBookingDetailUpdateClick = ({
    date,
    member: memberId,
    notes,
    organization: organizationId,
    location: locationId,
    team: teamId,
    desks: deskIds,
    rooms: roomIds,
    resources: resourceIds,
  }: BookingDetails) => {
    if (!rootData.booking) {
      return;
    }

    const booking = rootData.booking;
    const start = date as unknown as Dayjs;
    const from = start.toISOString();
    const until = endOfDay(start).toISOString();
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
          until,
          notes,
          organizationId,
          locationId,
          teamId,
          deskIds: locationId ? deskIds.filter((deskId) => rootDataAvailableLocationDesks.availableDesks?.find((availableDesk) => availableDesk.uniqueId === deskId)) : [],
          roomIds: locationId ? roomIds.filter((roomId) => rootDataAvailableLocationRooms.availableRooms?.find((availableRoom) => availableRoom.uniqueId === roomId)) : [],
          resourceIds,
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
            // TODO: 20240112 - Morteza: Below line stores the existing/old desk, but not the updated value for optimistic update, update this line with the updated value in future
            desks: booking.desks,
            rooms: booking.rooms,
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

    const deskIds =
      rootData.booking?.desks && from.toISOString() === rootData.booking.from && rootData.booking.location?.uniqueId === locationId
        ? rootData.booking.desks.map(({ uniqueId }) => uniqueId)
        : [];

    handleRefetchAvailableLocationDesks(deskIds, from, locationId);

    const roomIds =
      rootData.booking?.rooms && from.toISOString() === rootData.booking.from && rootData.booking.location?.uniqueId === locationId
        ? rootData.booking.rooms.map(({ uniqueId }) => uniqueId)
        : [];

    handleRefetchAvailableLocationRooms(roomIds, from, locationId);
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
              notes: booking.notes,
              team: teamId,
              location: locationId,
              desks: booking.desks ? booking.desks.map(({ uniqueId }) => uniqueId) : [],
              rooms: booking.rooms ? booking.rooms.map(({ uniqueId }) => uniqueId) : [],
              resources: booking.resources ? booking.resources.map(({ uniqueId }) => uniqueId) : [],
            }}
            validate={validateBookingDetails}
            render={({ handleSubmit, values }) => {
              setFrom(values.date);

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

                    <FormFieldLabel label="Date">
                      <DatePicker name="date" required={requiredFields.date} />
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
                                    <CustomTags customTags={castedOption.customTags} hideNAText />
                                    <Zones zones={castedOption.zones} hideIcon hideNAText />
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

                    {locationId && (
                      <FormFieldLabel label="Rooms">
                        {rooms.length > 0 && (
                          <Autocomplete
                            name="rooms"
                            multiple={true}
                            required={requiredFields.rooms}
                            options={rooms}
                            getOptionValue={(option) => (option as RoomDetails).uniqueId}
                            getOptionLabel={(option: string | RoomDetails) => (option as RoomDetails).name}
                            renderOption={(props, option) => {
                              const castedOption = option as RoomDetails;

                              return (
                                <li {...props}>
                                  <StackRow sx={{ alignItems: 'center' }}>
                                    <BodyIconTypography label={castedOption.name} />
                                    <CustomTags customTags={castedOption.customTags} hideNAText />
                                    <Zones zones={castedOption.zones} hideIcon hideNAText />
                                  </StackRow>
                                </li>
                              );
                            }}
                            disableCloseOnSelect
                            filterOptions={(options, params) => filterRoom(options as RoomDetails[], params)}
                            selectOnFocus
                            clearOnBlur
                            handleHomeEndKeys
                          />
                        )}

                        {rooms.length === 0 && <BodyIconTypography label="There are currently no available rooms in the chosen location." />}
                      </FormFieldLabel>
                    )}

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
                              <li {...props}>
                                <StackRow sx={{ alignItems: 'center' }}>
                                  <BodyIconTypography label={castedOption.name} />
                                  <CustomTags customTags={castedOption.customTags} hideNAText />
                                  <Zones zones={castedOption.zones} hideIcon hideNAText />
                                </StackRow>
                              </li>
                            );
                          }}
                          disableCloseOnSelect
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

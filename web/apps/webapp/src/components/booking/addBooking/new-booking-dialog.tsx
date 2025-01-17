import type { newBookingDialog_addBookingMutation } from '@/queries/__generated__/newBookingDialog_addBookingMutation.graphql';
import type { newBookingDialog_availableLocationDesks_query$key } from '@/queries/__generated__/newBookingDialog_availableLocationDesks_query.graphql';
import type { newBookingDialog_availableLocationDesks_refetchableFragment } from '@/queries/__generated__/newBookingDialog_availableLocationDesks_refetchableFragment.graphql';
import type { newBookingDialog_customerTeams_query$key } from '@/queries/__generated__/newBookingDialog_customerTeams_query.graphql';
import type { newBookingDialog_customerTeams_refetchableFragment } from '@/queries/__generated__/newBookingDialog_customerTeams_refetchableFragment.graphql';
import type { newBookingDialog_organizationMembers_query$key } from '@/queries/__generated__/newBookingDialog_organizationMembers_query.graphql';
import type { newBookingDialog_organizationMembers_refetchableFragment } from '@/queries/__generated__/newBookingDialog_organizationMembers_refetchableFragment.graphql';
import type { newBookingDialog_query$key } from '@/queries/__generated__/newBookingDialog_query.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import {
  BodyIconTypography,
  DefaultDialogTitle,
  FormFieldLabel,
  FormStackColumn,
  StackRow,
  TwoButtonsDialogActions,
} from '@repo/shared/components/commons';
import { CustomTags } from '@repo/shared/components/customTag';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { Zones } from '@repo/shared/components/zone';
import { PaletteModeContext, UpdateGlobalReloadIdContext } from '@repo/shared/libs/providers';
import { endOfDay, getCustomerFullName, joinErrors, keyboardDebounceTimeout, startOfDay, toShortDate } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { Autocomplete, DatePicker, makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, usePaginationFragment, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { array, date, object, string } from 'yup';

type Props = {
  rootDataRelay: newBookingDialog_query$key;
  rootDataOrganizationMembersRelay: newBookingDialog_organizationMembers_query$key;
  rootDataTeamsRelay: newBookingDialog_customerTeams_query$key;
  rootDataAvailableLocationDesksRelay: newBookingDialog_availableLocationDesks_query$key;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
  organizationId?: string;
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

type DeskDetails = {
  uniqueId: string;
  name: string;
  customTags: CustomTagDetails[];
  zones: ZoneDetails[];
};

type BookingDetails = {
  date: Date;
  member: string;
  notes: string;
  team: string | undefined;
  location: string | undefined;
  desks: string[];
};

const bookingSchema = object({
  date: date().required(),
  member: string().required(),
  notes: string().notRequired(),
  team: string().notRequired(),
  location: string().notRequired(),
  desk: array().nullable(),
});

const NewBookingDialog = ({
  rootDataRelay,
  rootDataTeamsRelay,
  rootDataOrganizationMembersRelay,
  rootDataAvailableLocationDesksRelay,
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
        locations(where: { organizationId: $organizationId }, orderBy: $locationsSortingValues) @include(if: $organizationExists) {
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
    rootDataRelay,
  );

  const { data: rootDataOrganizationMembers, refetch: refetchOrganizationMembers } = usePaginationFragment<
    newBookingDialog_organizationMembers_refetchableFragment,
    newBookingDialog_organizationMembers_query$key
  >(
    graphql`
      fragment newBookingDialog_organizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 20 })
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

  const [rootDataTeams, refetchTeams] = useRefetchableFragment<
    newBookingDialog_customerTeams_refetchableFragment,
    newBookingDialog_customerTeams_query$key
  >(
    graphql`
      fragment newBookingDialog_customerTeams_query on Query @refetchable(queryName: "newBookingDialog_customerTeams_refetchableFragment") {
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
    newBookingDialog_availableLocationDesks_refetchableFragment,
    newBookingDialog_availableLocationDesks_query$key
  >(
    graphql`
      fragment newBookingDialog_availableLocationDesks_query on Query
      @refetchable(queryName: "newBookingDialog_availableLocationDesks_refetchableFragment") {
        availableDesks(where: { locationId: $locationId, date: $dateToGetAvailableDesks }) @include(if: $locationExists) {
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

  const [commitAddBooking] = useMutation<newBookingDialog_addBookingMutation>(graphql`
    mutation newBookingDialog_addBookingMutation($connectionIds: [ID!]!, $input: AddBookingInput!) @raw_response_type {
      addBooking(input: $input) {
        booking @appendNode(connections: $connectionIds, edgeTypeName: "BookingDetails") {
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

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const UpdateGlobalReloadId = useContext(UpdateGlobalReloadIdContext);
  const [, startTransition] = useTransition();
  const [, setPage] = useState(0);
  const [pageSize] = useState(20);
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const validate = makeValidate(bookingSchema);
  const requiredFields = makeRequired(bookingSchema);
  const [from, setFrom] = useState<Dayjs | Date>(defaultDate ?? startOfDay());
  const [customerId, setCustomerId] = useState<string | undefined>();
  const [teamId, setTeamId] = useState<string | undefined>();
  const [locationId, setLocationId] = useState<string | undefined>(defaultLocationId);
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
    (from: Dayjs | Date, locationId?: string) => {
      startTransition(() => {
        refetchAvailableLocationDesks(
          {
            locationId: locationId ?? '',
            locationExists: !!locationId,
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

  useEffect(() => handleRefetchAvailableLocationDesks(from, locationId), [handleRefetchAvailableLocationDesks, from, locationId]);

  const handleAddClick = ({ date, member, notes, team: teamId, location: locationId, desks: deskIds }: BookingDetails) => {
    if (!rootData.me) {
      return;
    }

    const id = nanoid();
    const finalDate = date as unknown as Dayjs;
    const from = startOfDay(finalDate).toISOString();
    const to = endOfDay(finalDate).toISOString();
    const fromToPrint = toShortDate(startOfDay(finalDate));
    const customerId = member ?? rootData.me?.id;
    const toastId = themedToast(<NotificationContent content={`Making a booking on '${fromToPrint}'...`} />, infoNotificationOptions);
    const type = 'WorkingFromOffice';

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
          teamId,
          locationId,
          deskIds,
          type,
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
        let message = `Booking made for ${getCustomerFullName(booking.customer)} to work`;

        if (booking.location) {
          message += ` from the "${booking.location!.name}"`;
        }

        if (booking.desks.length > 0) {
          message += ` at desk "${booking.desks.map(({ name }) => name).join(', ')}"`;

          const zones = booking.desks.flatMap(({ zones }) => zones);
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
            to,
            notes,
            type,
            customer: {
              uniqueId: rootData.me.id,
              name: '',
              givenName: '',
              middleName: '',
              familyName: '',
              photoUrl: '',
            },
            organization: organizationId
              ? {
                  uniqueId: organizationId,
                  name: '',
                }
              : null,
            location: locationId
              ? {
                  uniqueId: locationId,
                  name: '',
                }
              : null,
            team: teamId
              ? {
                  uniqueId: teamId,
                  name: '',
                }
              : null,
            desks: [],
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
    handleRefetchAvailableLocationDesks(from, locationId);
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
    <Dialog TransitionComponent={DialogTransition} open={isDialogOpen} fullWidth>
      <DefaultDialogTitle title="Make a booking" />
      <DialogContent>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            member: customerId,
            date: from,
            notes: '',
            team: teamId,
            location: locationId,
            desks: [],
          }}
          validate={validate}
          render={({ handleSubmit, values }) => {
            setFrom(values.date);

            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <FormFieldLabel label="User" useWiderSpace>
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

                <FormFieldLabel label="Date" useWiderSpace>
                  <DatePicker name="date" required={requiredFields.date} />
                </FormFieldLabel>

                <FormFieldLabel label="Notes" useWiderSpace>
                  <TextField
                    name="notes"
                    required={requiredFields.notes}
                    helperText="e.g. I will be half an hour late this morning"
                    multiline
                    rows={2}
                  />
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
                  <FormFieldLabel label="Desks" useWiderSpace>
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
                                <CustomTags customTags={castedOption.customTags} />
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

import { createFilterOptions } from '@mui/material/useAutocomplete';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography, FormFieldLabel, StackRow } from '@repo/shared/components/commons';
import { Zones } from '@repo/shared/components/zone';
import { getCustomerFullName, keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { Dayjs } from 'dayjs';
import { Autocomplete } from 'mui-rff';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { useFragment, usePaginationFragment, useRefetchableFragment } from 'react-relay';
import { useDebounceCallback } from 'usehooks-ts';
import type { bookingDetailsSelector_availableLocationDesks_query$key } from './__generated__/bookingDetailsSelector_availableLocationDesks_query.graphql';
import type { bookingDetailsSelector_availableLocationDesks_refetchableFragment } from './__generated__/bookingDetailsSelector_availableLocationDesks_refetchableFragment.graphql';
import type { bookingDetailsSelector_organizationMembers_query$key } from './__generated__/bookingDetailsSelector_organizationMembers_query.graphql';
import type { bookingDetailsSelector_organizationMembers_refetchableFragment } from './__generated__/bookingDetailsSelector_organizationMembers_refetchableFragment.graphql';
import type { bookingDetailsSelector_query$key } from './__generated__/bookingDetailsSelector_query.graphql';

type Props = {
  rootDataRelay: bookingDetailsSelector_query$key;
  rootDataOrganizationMembersRelay: bookingDetailsSelector_organizationMembers_query$key;
  rootDataAvailableLocationDesksRelay: bookingDetailsSelector_availableLocationDesks_query$key;

  defaultOrganizationId?: string;
  organizationName: string;
  organizationRequired?: boolean;
  hideOrganizationControl?: boolean;

  organizationMemberName: string;
  organizationMemberRequired?: boolean;
  hideOrganizationMemberControl?: boolean;

  defaultLocationId?: string;
  locationName: string;
  locationRequired?: boolean;
  hideLocationControl?: boolean;

  defaultDeskIds: string[];
  deskName: string;
  deskRequired?: boolean;
  hideDesksControl: boolean;

  bookingFrom: Dayjs | Date;
  bookingTo: Dayjs | Date;
};

type OrganizationDetails = {
  id: string;
  name: string;
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

type LocationDetails = {
  id: string;
  name: string;
};

type ZoneDetails = {
  id: string;
  name: string;
};

type DeskDetails = {
  uniqueId: string;
  name: string;
  zones: ZoneDetails[];
};

const BookingDetailsSelector = ({
  rootDataRelay,
  rootDataOrganizationMembersRelay,
  rootDataAvailableLocationDesksRelay,

  defaultOrganizationId,
  organizationName,
  organizationRequired,
  hideOrganizationControl,

  organizationMemberName,
  organizationMemberRequired,
  hideOrganizationMemberControl,

  defaultLocationId,
  locationName,
  locationRequired,
  hideLocationControl,

  defaultDeskIds,
  deskName,
  deskRequired,
  hideDesksControl,

  bookingFrom,
  bookingTo,
}: Props) => {
  const rootData = useFragment<bookingDetailsSelector_query$key>(
    graphql`
      fragment bookingDetailsSelector_query on Query {
        myOrganizations {
          id
          name
        }
        myLocations(organizationId: $nullableOrganizationId) {
          id
          name
          organization {
            uniqueId
          }
        }
        myTeams(organizationId: $nullableOrganizationId) {
          id
          name
          organization {
            uniqueId
          }
        }
      }
    `,
    rootDataRelay,
  );
  const { data: rootDataOrganizationMembers, refetch: refetchOrganizationMembers } = usePaginationFragment<
    bookingDetailsSelector_organizationMembers_refetchableFragment,
    bookingDetailsSelector_organizationMembers_query$key
  >(
    graphql`
      fragment bookingDetailsSelector_organizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 20 })
      @refetchable(queryName: "bookingDetailsSelector_organizationMembers_refetchableFragment") {
        organizationMembers(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $bookingPeopleNameSearchText }
          orderBy: $bookingDetailsSelectorOrganizationMembersSortingValues
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

  const [rootDataAvailableLocationDesks, refetchAvailableLocationDesks] = useRefetchableFragment<
    bookingDetailsSelector_availableLocationDesks_refetchableFragment,
    bookingDetailsSelector_availableLocationDesks_query$key
  >(
    graphql`
      fragment bookingDetailsSelector_availableLocationDesks_query on Query
      @refetchable(queryName: "bookingDetailsSelector_availableLocationDesks_refetchableFragment") {
        availableDesks(where: { locationId: $locationId, date: $dateToGetAvailableDesks, deskIdsToInclude: $deskIdsToIncludeToGetAvailableDesks })
          @include(if: $locationExists) {
          uniqueId
          name
          deskTypes {
            uniqueId
            name
          }
          zones {
            uniqueId
            name
          }
        }
      }
    `,
    rootDataAvailableLocationDesksRelay,
  );

  const [, startTransition] = useTransition();
  const [, setPage] = useState(0);
  const [pageSize] = useState(20);
  const [bookingPeopleNameSearchText, setBookingPeopleNameSearchText] = useState<string>('');
  const [organizationId, setOrganizationId] = useState(defaultOrganizationId);
  const [locationId, setLocationId] = useState(defaultLocationId);
  const organizations = useMemo<LocationDetails[]>(
    () => (rootData.myOrganizations ? rootData.myOrganizations.map((organization) => organization) : []),
    [rootData.myOrganizations],
  );

  const customers = useMemo<OrganizationMemberDetails[]>(() => {
    if (!rootDataOrganizationMembers.organizationMembers) {
      return [];
    }

    return rootDataOrganizationMembers.organizationMembers.edges.map(({ node }) => node);
  }, [rootDataOrganizationMembers.organizationMembers]);

  const locations = useMemo<LocationDetails[]>(() => {
    const myLocations = rootData.myLocations ? rootData.myLocations.map((location) => location) : [];

    return organizationId ? myLocations.filter((location) => location.organization?.uniqueId === organizationId) : myLocations;
  }, [rootData.myLocations, organizationId]);

  const desks = useMemo<DeskDetails[]>(() => {
    if (!rootDataAvailableLocationDesks.availableDesks) {
      return [];
    }

    return rootDataAvailableLocationDesks.availableDesks.map(({ uniqueId, name, zones }) => ({
      uniqueId,
      name,
      zones: zones.map(({ uniqueId: id, name }) => ({ id, name })),
    }));
  }, [rootDataAvailableLocationDesks.availableDesks]);

  const handleRefetchOrganizationMembers = useCallback(
    (bookingPeopleNameSearchText: string, organizationId?: string) => {
      startTransition(() => {
        refetchOrganizationMembers(
          {
            count: pageSize,
            bookingPeopleNameSearchText,
            organizationId,
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

  const handleRefetchAvailableLocationDesks = useCallback(
    (deskIds: string[], locationId?: string) => {
      startTransition(() => {
        refetchAvailableLocationDesks(
          {
            locationId: locationId ?? '',
            locationExists: !!locationId,
            deskIdsToIncludeToGetAvailableDesks: deskIds,
            dateToGetAvailableDesks: bookingFrom,
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
    [refetchAvailableLocationDesks, bookingFrom],
  );

  // Workaround to ensure we have the entire form refreshed once any dependent values change
  useEffect(() => {
    handleRefetchOrganizationMembers(bookingPeopleNameSearchText, organizationId);
    handleRefetchAvailableLocationDesks(defaultDeskIds, locationId);
  }, [
    handleRefetchOrganizationMembers,
    handleRefetchAvailableLocationDesks,
    bookingPeopleNameSearchText,
    organizationId,
    locationId,
    bookingFrom,
    bookingTo,
    defaultDeskIds,
  ]);

  const filterOrganization = createFilterOptions<OrganizationDetails>();
  const filterLocation = createFilterOptions<LocationDetails>();
  const filterDesk = createFilterOptions<DeskDetails>();

  const handleOrganizationChange = (option: OrganizationDetails | null) => {
    const id = option?.id;
    setOrganizationId(id);
    handleRefetchOrganizationMembers(bookingPeopleNameSearchText, id);
  };

  const handleLocationChange = (option: LocationDetails | null) => {
    const id = option?.id;
    setLocationId(id);
    handleRefetchAvailableLocationDesks(defaultDeskIds, id);
  };

  const handlePeopleNameSearchTextChange = (str: string) => {
    setBookingPeopleNameSearchText(str);

    handleRefetchOrganizationMembers(str, organizationId);
  };

  const debouncePeopleNameSearchTextChange = useDebounceCallback(handlePeopleNameSearchTextChange, keyboardDebounceTimeout);

  return (
    <>
      {!hideOrganizationControl && (
        <FormFieldLabel label="Organization" useWiderSpace>
          <Autocomplete
            name={organizationName}
            multiple={false}
            required={organizationRequired}
            options={organizations}
            getOptionValue={(option) => (option as OrganizationDetails).id}
            getOptionLabel={(option: string | OrganizationDetails) => (option as OrganizationDetails).name}
            renderOption={(props, option) => {
              const castedOption = option as OrganizationDetails;

              return (
                <li {...props}>
                  <BodyIconTypography label={castedOption.name} />
                </li>
              );
            }}
            disableCloseOnSelect={false}
            freeSolo={true}
            filterOptions={(options, params) => filterOrganization(options as OrganizationDetails[], params)}
            selectOnFocus
            clearOnBlur
            handleHomeEndKeys
            onChange={(_, option) => handleOrganizationChange(option as OrganizationDetails)}
          />
        </FormFieldLabel>
      )}

      {!hideOrganizationMemberControl && (
        <FormFieldLabel label="Organization Member" useWiderSpace>
          <Autocomplete
            name={organizationMemberName}
            multiple={false}
            required={organizationMemberRequired}
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
            disableCloseOnSelect={false}
            freeSolo={true}
            filterOptions={(options, params) => {
              if (params.inputValue !== bookingPeopleNameSearchText) {
                debouncePeopleNameSearchTextChange(params.inputValue);
              }

              return options;
            }}
            selectOnFocus
            clearOnBlur
            handleHomeEndKeys
          />
        </FormFieldLabel>
      )}

      {!hideLocationControl && (
        <FormFieldLabel label="Location" useWiderSpace>
          <Autocomplete
            name={locationName}
            multiple={false}
            required={locationRequired}
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
            disableCloseOnSelect={false}
            freeSolo={true}
            filterOptions={(options, params) => filterLocation(options as LocationDetails[], params)}
            selectOnFocus
            clearOnBlur
            handleHomeEndKeys
            onChange={(_, option) => handleLocationChange(option as LocationDetails)}
          />
        </FormFieldLabel>
      )}

      {!hideDesksControl && (
        <>
          {desks.length !== 0 && (
            <FormFieldLabel label="Desks" useWiderSpace>
              <Autocomplete
                name={deskName}
                multiple={true}
                required={deskRequired}
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
                disableCloseOnSelect={true}
                freeSolo={true}
                filterOptions={(options, params) => filterDesk(options as DeskDetails[], params)}
                selectOnFocus
                clearOnBlur
                handleHomeEndKeys
              />
            </FormFieldLabel>
          )}

          {locationId && desks.length === 0 && (
            <BodyIconTypography label="There are currently no available desks in the chosen location." color="warning" />
          )}
        </>
      )}
    </>
  );
};

export default memo(BookingDetailsSelector);

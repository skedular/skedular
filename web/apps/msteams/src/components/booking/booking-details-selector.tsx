import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { TAG_TYPE_LOCATION_ZONE, ZonesLine } from '@repo/shared/components/zone';
import { getCustomerFullName, keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { Dayjs } from 'dayjs';
import { Autocomplete } from 'mui-rff';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { useFragment, usePaginationFragment, useRefetchableFragment } from 'react-relay';
import { useDebounceCallback } from 'usehooks-ts';
import type { bookingDetailsSelector_availableLocationDesks_query$key } from './__generated__/bookingDetailsSelector_availableLocationDesks_query.graphql';
import type { bookingDetailsSelector_availableLocationDesks_refetchableFragment } from './__generated__/bookingDetailsSelector_availableLocationDesks_refetchableFragment.graphql';
import type { bookingDetailsSelector_paginatedOrganizationMembers_query$key } from './__generated__/bookingDetailsSelector_paginatedOrganizationMembers_query.graphql';
import type { bookingDetailsSelector_paginatedOrganizationMembers_refetchableFragment } from './__generated__/bookingDetailsSelector_paginatedOrganizationMembers_refetchableFragment.graphql';
import type { bookingDetailsSelector_query$key } from './__generated__/bookingDetailsSelector_query.graphql';

type Props = {
  rootDataRelay: bookingDetailsSelector_query$key;
  rootDataPaginatedOrganizationMembersRelay: bookingDetailsSelector_paginatedOrganizationMembers_query$key;
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
  rootDataPaginatedOrganizationMembersRelay,
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
        myLocations(organizationId: $organizationId) {
          id
          name
        }
      }
    `,
    rootDataRelay,
  );
  const { data: rootDataPaginatedOrganizationMembers, refetch: refetchPaginatedOrganizationMembers } = usePaginationFragment<
    bookingDetailsSelector_paginatedOrganizationMembers_refetchableFragment,
    bookingDetailsSelector_paginatedOrganizationMembers_query$key
  >(
    graphql`
      fragment bookingDetailsSelector_paginatedOrganizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 20 })
      @refetchable(queryName: "bookingDetailsSelector_paginatedOrganizationMembers_refetchableFragment") {
        paginatedOrganizationMembers(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $bookingPeopleNameSearchText }
          orderBy: $bookingDetailsSelectorOrganizationMembersSortingValues
        ) @connection(key: "bookingDetailsSelectorQuery_paginatedOrganizationMembers") @include(if: $organizationExists) {
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
    rootDataPaginatedOrganizationMembersRelay,
  );

  const [rootDataAvailableLocationDesks, refetchAvailableLocationDesks] = useRefetchableFragment<
    bookingDetailsSelector_availableLocationDesks_refetchableFragment,
    bookingDetailsSelector_availableLocationDesks_query$key
  >(
    graphql`
      fragment bookingDetailsSelector_availableLocationDesks_query on Query
      @refetchable(queryName: "bookingDetailsSelector_availableLocationDesks_refetchableFragment") {
        availableLocationDesks(locationId: $locationId, date: $dateToGetAvailableDesks, deskIdsToInclude: $deskIdsToIncludeToGetAvailableDesks)
          @include(if: $locationExists) {
          uniqueId
          name
          locationTags {
            uniqueId
            name
            tagType
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
    if (!rootDataPaginatedOrganizationMembers.paginatedOrganizationMembers) {
      return [];
    }

    return rootDataPaginatedOrganizationMembers.paginatedOrganizationMembers.edges.map(({ node }) => node);
  }, [rootDataPaginatedOrganizationMembers.paginatedOrganizationMembers]);

  const locations = useMemo<LocationDetails[]>(
    () => (rootData.myLocations ? rootData.myLocations.map((location) => location) : []),
    [rootData.myLocations],
  );

  const desks = useMemo<DeskDetails[]>(() => {
    if (!rootDataAvailableLocationDesks.availableLocationDesks) {
      return [];
    }

    return rootDataAvailableLocationDesks.availableLocationDesks.map(({ uniqueId, name, locationTags }) => ({
      uniqueId,
      name,
      zones: locationTags
        .filter(({ tagType }) => tagType === TAG_TYPE_LOCATION_ZONE)
        .map(({ uniqueId: id, name }) => ({
          id,
          name,
        })),
    }));
  }, [rootDataAvailableLocationDesks.availableLocationDesks]);

  const handleRefetchPaginatedOrganizationMembers = useCallback(
    (bookingPeopleNameSearchText: string, organizationId?: string) => {
      startTransition(() => {
        refetchPaginatedOrganizationMembers(
          {
            count: pageSize,
            bookingPeopleNameSearchText,
            organizationId: organizationId ?? '',
            organizationExists: !!organizationId,
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
    [refetchPaginatedOrganizationMembers, pageSize],
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
    handleRefetchPaginatedOrganizationMembers(bookingPeopleNameSearchText, organizationId);
    handleRefetchAvailableLocationDesks(defaultDeskIds, locationId);
  }, [
    handleRefetchPaginatedOrganizationMembers,
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
    handleRefetchPaginatedOrganizationMembers(bookingPeopleNameSearchText, id);
  };

  const handleLocationChange = (option: LocationDetails | null) => {
    const id = option?.id;
    setLocationId(id);
    handleRefetchAvailableLocationDesks(defaultDeskIds, id);
  };

  const handleSearchTextChange = (str: string) => {
    setBookingPeopleNameSearchText(str);

    handleRefetchPaginatedOrganizationMembers(str, organizationId);
  };

  const debounceSearchTextChange = useDebounceCallback(handleSearchTextChange, keyboardDebounceTimeout);

  return (
    <>
      {!hideOrganizationControl && (
        <Autocomplete
          label="Organization"
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
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                  <Typography variant="body1">{castedOption.name}</Typography>
                </Stack>
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
      )}

      {!hideOrganizationMemberControl && (
        <Autocomplete
          label="Organization Member"
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
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                  <CustomerAvatar name={castedOption} photo={{ url: castedOption.photoUrl }} size="small" />
                  <Typography variant="body1">{getCustomerFullName(castedOption)}</Typography>
                </Stack>
              </li>
            );
          }}
          disableCloseOnSelect={false}
          freeSolo={true}
          filterOptions={(options, params) => {
            if (params.inputValue !== bookingPeopleNameSearchText) {
              debounceSearchTextChange(params.inputValue);
            }

            return options;
          }}
          selectOnFocus
          clearOnBlur
          handleHomeEndKeys
        />
      )}

      {!hideLocationControl && (
        <Autocomplete
          label="Location"
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
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                  <Typography variant="body1">{castedOption.name}</Typography>
                </Stack>
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
      )}

      {!hideDesksControl && (
        <>
          {desks.length !== 0 && (
            <Autocomplete
              label="Desks"
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
                    <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                      <Typography variant="body1">{castedOption.name}</Typography>
                      <ZonesLine zones={castedOption.zones} />
                    </Stack>
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
          )}

          {locationId && desks.length === 0 && (
            <Typography variant="body1" color="warning">
              There are currently no available desks in the chosen location.
            </Typography>
          )}
        </>
      )}
    </>
  );
};

export default memo(BookingDetailsSelector);

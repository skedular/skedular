import type { bookingDetailsSelectorQuery } from '@/queries/__generated__/bookingDetailsSelectorQuery.graphql';
import type { bookingDetailsSelector_query$key } from '@/queries/__generated__/bookingDetailsSelector_query.graphql';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { TAG_TYPE_LOCATION_ZONE, ZonesLine } from '@repo/shared/components/zone';
import { getCustomerFullName, keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import debounce from 'lodash.debounce';
import { Autocomplete } from 'mui-rff';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { graphql, usePaginationFragment } from 'react-relay';

type Props = {
  rootDataRelay: bookingDetailsSelector_query$key;

  defaultOrganizationId: string | null;
  organizationName: string;
  organizationRequired?: boolean;
  hideOrganizationControl: boolean;

  organizationMemberName: string;
  organizationMemberRequired?: boolean;
  hideOrganizationMemberControl: boolean;

  defaultLocationId: string | null;
  locationName: string;
  locationRequired?: boolean;
  hideLocationControl: boolean;

  defaultDeskIds: string[];
  deskName: string;
  deskRequired?: boolean;
  hideDesksControl: boolean;

  bookingFrom: Dayjs | Date;
  bookingTo: Dayjs | Date;
};

interface OrganizationDetails {
  id: string;
  name: string;
}

interface CustomerDetails {
  uniqueId: string;
  name: string | null | undefined;
  givenName: string | null | undefined;
  middleName: string | null | undefined;
  familyName: string | null | undefined;
  photoUrl: string | null | undefined;
}

interface OrganizationMemberDetails {
  id: string;
  customer: CustomerDetails;
}

interface LocationDetails {
  id: string;
  name: string;
}

interface ZoneDetails {
  id: string;
  name: string;
}

interface DeskDetails {
  uniqueId: string;
  name: string;
  zones: ZoneDetails[];
}

const BookingDetailsSelector = ({
  rootDataRelay,

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
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<bookingDetailsSelectorQuery, bookingDetailsSelector_query$key>(
    graphql`
      fragment bookingDetailsSelector_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 20 })
      @refetchable(queryName: "bookingDetailsSelectorQuery") {
        myOrganizations {
          id
          name
        }

        myLocations(organizationId: $organizationId) {
          id
          name
        }

        availableLocationDesks(locationId: $locationId, date: $dateToGetAvailableDesks, deskIdsToInclude: $deskIdsToIncludeToGetAvailableDesks) {
          uniqueId
          name
          locationTags {
            uniqueId
            name
            tagType
          }
        }

        bookingDetailsSelectorQueryPaginatedOrganizationMembers: paginatedOrganizationMembers(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $bookingPeopleNameSearchText }
          orderBy: $bookingDetailsSelectorOrganizationMembersSortingValues
        ) @connection(key: "bookingDetailsSelectorQuery_bookingDetailsSelectorQueryPaginatedOrganizationMembers") {
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
    rootDataRelay,
  );

  const [, startTransition] = useTransition();
  const [, setPage] = useState(0);
  const [pageSize] = useState(20);
  const [bookingPeopleNameSearchText, setBookingPeopleNameSearchText] = useState<string>('');
  const [organizationId, setOrganizationId] = useState(defaultOrganizationId);
  const [locationId, setLocationId] = useState(defaultLocationId);
  const organizations = useMemo<LocationDetails[]>(() => rootData.myOrganizations.map((organization) => organization), [rootData.myOrganizations]);

  const customers = useMemo<OrganizationMemberDetails[]>(() => {
    if (!rootData.bookingDetailsSelectorQueryPaginatedOrganizationMembers) {
      return [];
    }

    return rootData.bookingDetailsSelectorQueryPaginatedOrganizationMembers.edges.map(({ node }) => node);
  }, [rootData.bookingDetailsSelectorQueryPaginatedOrganizationMembers]);

  const locations = useMemo<LocationDetails[]>(() => rootData.myLocations.map((location) => location), [rootData.myLocations]);

  const desks = useMemo<DeskDetails[]>(() => {
    if (!rootData.availableLocationDesks) {
      return [];
    }

    return rootData.availableLocationDesks.map(({ uniqueId, name, locationTags }) => ({
      uniqueId,
      name,
      zones: locationTags
        .filter(({ tagType }) => tagType === TAG_TYPE_LOCATION_ZONE)
        .map(({ uniqueId: id, name }) => ({
          id,
          name,
        })),
    }));
  }, [rootData.availableLocationDesks]);

  const handleRefetch = useCallback(
    (bookingPeopleNameSearchText: string, organizationId: string | null, locationId: string | null, deskIds: string[]) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            bookingPeopleNameSearchText,
            organizationId: organizationId ?? '',
            locationId: locationId ?? '',
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
    [refetch, pageSize, bookingFrom],
  );

  // Workaround to ensure we have all the entire form refreshed once any dependent values change
  useEffect(() => {
    handleRefetch(bookingPeopleNameSearchText, organizationId, locationId, defaultDeskIds);
  }, [handleRefetch, bookingPeopleNameSearchText, organizationId, locationId, bookingFrom, bookingTo, defaultDeskIds]);

  const filterOrganization = createFilterOptions<OrganizationDetails>();
  const filterLocation = createFilterOptions<LocationDetails>();
  const filterDesk = createFilterOptions<DeskDetails>();

  const handleOrganizationChange = (option: OrganizationDetails | null) => {
    const id = option?.id ?? null;
    setOrganizationId(id);

    setLocationId(id);
    handleRefetch(bookingPeopleNameSearchText, id, locationId, defaultDeskIds);
  };

  const handleLocationChange = (option: LocationDetails | null) => {
    const id = option?.id ?? null;

    setLocationId(id);
    handleRefetch(bookingPeopleNameSearchText, organizationId, id, defaultDeskIds);
  };

  const handleSearchTextChange = (str: string) => {
    setBookingPeopleNameSearchText(str);

    handleRefetch(str, organizationId, locationId, defaultDeskIds);
  };

  const debounceSearchTextChange = debounce(handleSearchTextChange, keyboardDebounceTimeout);

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

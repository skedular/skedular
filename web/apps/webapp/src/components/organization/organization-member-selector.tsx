import type { organizationMemberSelector_organizationMembersPaginationQuery } from '@/queries/__generated__/organizationMemberSelector_organizationMembersPaginationQuery.graphql';
import type { organizationMemberSelector_query$key } from '@/queries/__generated__/organizationMemberSelector_query.graphql';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { getCustomerFullName, keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import debounce from 'lodash.debounce';
import { Autocomplete } from 'mui-rff';
import { memo, useCallback, useMemo, useState, useTransition } from 'react';
import { graphql, usePaginationFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizationMemberSelector_query$key;
  name: string;
  required?: boolean;
  readOnly?: boolean;
  multiple: boolean;
  useMemberId: boolean;
};

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

const OrganizationMemberSelector = ({ rootDataRelay, name, required, readOnly, multiple, useMemberId }: Props) => {
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<organizationMemberSelector_organizationMembersPaginationQuery, organizationMemberSelector_query$key>(
    graphql`
      fragment organizationMemberSelector_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 20 })
      @refetchable(queryName: "organizationMemberSelector_organizationMembersPaginationQuery") {
        organizationMemberSelectorPaginatedOrganizationMembers: paginatedOrganizationMembers(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $bookingPeopleNameSearchText }
          orderBy: $organizationMemberSelectorOrganizationMembersSortingValues
        ) @connection(key: "organizationMemberSelector_organizationMemberSelectorPaginatedOrganizationMembers") {
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

  const customers = useMemo<OrganizationMemberDetails[]>(() => {
    if (!rootData.organizationMemberSelectorPaginatedOrganizationMembers) {
      return [];
    }

    return rootData.organizationMemberSelectorPaginatedOrganizationMembers.edges.map(({ node }) => node);
  }, [rootData.organizationMemberSelectorPaginatedOrganizationMembers]);

  const handleRefetch = useCallback(
    (pageSize: number, bookingPeopleNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            bookingPeopleNameSearchText,
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
    [refetch],
  );

  if (!rootData.organizationMemberSelectorPaginatedOrganizationMembers) {
    return <></>;
  }

  const handleSearchTextChange = (str: string) => {
    setBookingPeopleNameSearchText(str);

    handleRefetch(pageSize, str);
  };

  const debounceSearchTextChange = debounce(handleSearchTextChange, keyboardDebounceTimeout);

  return (
    <Autocomplete
      label="Organization Member"
      name={name}
      multiple={multiple}
      required={required}
      options={customers}
      getOptionValue={(option) => (useMemberId ? (option as OrganizationMemberDetails).id : (option as OrganizationMemberDetails).customer.uniqueId)}
      getOptionLabel={(option: string | OrganizationMemberDetails) => getCustomerFullName((option as OrganizationMemberDetails).customer)}
      renderOption={(props, option) => {
        const castedOption = (option as OrganizationMemberDetails).customer;

        return (
          <li {...props}>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
              <CustomerAvatar
                name={{
                  name: castedOption.name,
                  givenName: castedOption.givenName,
                  middleName: castedOption.middleName,
                  familyName: castedOption.familyName,
                }}
                photo={{
                  url: castedOption.photoUrl,
                }}
              />

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
      readOnly={readOnly}
    />
  );
};

export default memo(OrganizationMemberSelector);

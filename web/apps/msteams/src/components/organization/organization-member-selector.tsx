import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { getCustomerFullName, keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { Autocomplete } from 'mui-rff';
import { memo, useCallback, useMemo, useState, useTransition } from 'react';
import { usePaginationFragment } from 'react-relay';
import { useDebounceCallback } from 'usehooks-ts';
import type { organizationMemberSelector_query$key } from './__generated__/organizationMemberSelector_query.graphql';
import type { organizationMemberSelector_refetchableFragment } from './__generated__/organizationMemberSelector_refetchableFragment.graphql';

type Props = {
  rootDataRelay: organizationMemberSelector_query$key;
  name: string;
  required?: boolean;
  readOnly?: boolean;
  multiple: boolean;
  useMemberId: boolean;
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

const OrganizationMemberSelector = ({ rootDataRelay, name, required, readOnly, multiple, useMemberId }: Props) => {
  const { data: rootData, refetch } = usePaginationFragment<organizationMemberSelector_refetchableFragment, organizationMemberSelector_query$key>(
    graphql`
      fragment organizationMemberSelector_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 20 })
      @refetchable(queryName: "organizationMemberSelector_refetchableFragment") {
        organizationMemberSelectorPaginatedOrganizationMembers: organizationMembers(
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
  const [bookingPeopleNameSearchText, setBookingPeopleNameSearchText] = useState<string>('');
  const customers = useMemo<OrganizationMemberDetails[]>(
    () => (rootData.organizationMemberSelectorPaginatedOrganizationMembers ? rootData.organizationMemberSelectorPaginatedOrganizationMembers.edges.map(({ node }) => node) : []),
    [rootData.organizationMemberSelectorPaginatedOrganizationMembers],
  );

  const handleRefetch = useCallback(
    (bookingPeopleNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            count: 20,
            bookingPeopleNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch],
  );

  const handleSearchTextChange = (str: string) => {
    setBookingPeopleNameSearchText(str);

    handleRefetch(str);
  };

  const debounceSearchTextChange = useDebounceCallback(handleSearchTextChange, keyboardDebounceTimeout);

  if (!rootData.organizationMemberSelectorPaginatedOrganizationMembers) {
    return <></>;
  }

  return (
    <Autocomplete
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
            <BodyIconTypography
              label={getCustomerFullName(castedOption)}
              startElement={<CustomerAvatar name={castedOption} photo={{ url: castedOption.photoUrl }} size="small" />}
            />
          </li>
        );
      }}
      disableCloseOnSelect={multiple}
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

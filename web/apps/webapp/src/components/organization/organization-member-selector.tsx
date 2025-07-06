import { CustomerAvatar } from '@/components/avatars';
import { BodyIconTypography } from '@/components/commons';
import { getCustomerFullName, keyboardSearchDebounceTimeout } from '@/libs/utils';
import type { organizationMemberSelector_query$key } from '@/queries/__generated__/organizationMemberSelector_query.graphql';
import type { organizationMemberSelector_refetchableFragment } from '@/queries/__generated__/organizationMemberSelector_refetchableFragment.graphql';
import { Autocomplete } from 'mui-rff';
import { memo, useCallback, useMemo, useState, useTransition } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';
import { useDebounceCallback } from 'usehooks-ts';

type Props = {
  rootDataRelay: organizationMemberSelector_query$key;
  organizationId: string;
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

const OrganizationMemberSelector = ({ rootDataRelay, organizationId, name, required, readOnly, multiple, useMemberId }: Props) => {
  const [rootData, refetch] = useRefetchableFragment<organizationMemberSelector_refetchableFragment, organizationMemberSelector_query$key>(
    graphql`
      fragment organizationMemberSelector_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationMemberSelector_refetchableFragment") {
        organizationMemberSelectorPaginatedOrganizationMembers: organizationMembers(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $bookingPeopleNameSearchText }
          orderBy: $organizationMemberSelectorOrganizationMembersSortingValues
        ) @connection(key: "organizationMemberSelector_organizationMemberSelectorPaginatedOrganizationMembers") @include(if: $organizationExists) {
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
            bookingPeopleNameSearchText,
            organizationExists: !!organizationId,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch, organizationId],
  );

  const handleSearchTextChange = (str: string) => {
    setBookingPeopleNameSearchText(str);

    handleRefetch(str);
  };

  const debounceSearchTextChange = useDebounceCallback(handleSearchTextChange, keyboardSearchDebounceTimeout);

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
          <li {...props} key={castedOption.uniqueId}>
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

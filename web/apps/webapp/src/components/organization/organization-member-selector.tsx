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
  name: string;
  required?: boolean;
  readOnly?: boolean;
  multiple: boolean;
  useMemberId: boolean;
};

type CustomerDetails = {
  id: string;
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
  const [rootData, refetch] = useRefetchableFragment<organizationMemberSelector_refetchableFragment, organizationMemberSelector_query$key>(
    graphql`
      fragment organizationMemberSelector_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationMemberSelector_refetchableFragment") {
        organization(customDomain: $organizationCustomDomain) {
          members(first: $count, after: $cursor, where: { nameContains: $bookingPeopleNameSearchText }, orderBy: $organizationMemberSelectorOrganizationMembersSortingValues)
            @connection(key: "organizationMemberSelector_members") {
            __id
            totalCount
            edges {
              node {
                id
                customer {
                  id
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
      }
    `,
    rootDataRelay,
  );

  const [, startTransition] = useTransition();
  const [bookingPeopleNameSearchText, setBookingPeopleNameSearchText] = useState<string>('');
  const items = useMemo<OrganizationMemberDetails[]>(
    () => (rootData.organization?.members ? rootData.organization.members.edges.map(({ node }) => node) : []),
    [rootData.organization],
  );

  const handleRefetch = useCallback(
    (bookingPeopleNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            bookingPeopleNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [startTransition, refetch],
  );

  const handleSearchTextChange = (str: string) => {
    setBookingPeopleNameSearchText(str);

    handleRefetch(str);
  };

  const debounceSearchTextChange = useDebounceCallback(handleSearchTextChange, keyboardSearchDebounceTimeout);

  if (!rootData.organization?.members) {
    return null;
  }

  return (
    <Autocomplete
      name={name}
      multiple={multiple}
      required={required}
      options={items}
      getOptionValue={(option) => (useMemberId ? (option as OrganizationMemberDetails).id : (option as OrganizationMemberDetails).customer.id)}
      getOptionLabel={(option: string | OrganizationMemberDetails) => getCustomerFullName((option as OrganizationMemberDetails).customer)}
      renderOption={(props, option) => {
        const castedOption = (option as OrganizationMemberDetails).customer;

        return (
          <li {...props} key={castedOption.id}>
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

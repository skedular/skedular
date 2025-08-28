import { CustomerAvatar } from '@/components/avatars';
import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@/components/commons';
import { UserIcon } from '@/components/icons';
import { DefaultSelect } from '@/components/styled';
import { getCustomerFullName } from '@/libs/utils';
import type { organizationUserSelector_organizationMembers_query$key } from '@/queries/__generated__/organizationUserSelector_organizationMembers_query.graphql';
import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import { SelectChangeEvent } from '@mui/material/Select';
import { memo, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataOrganizationMembersRelay: organizationUserSelector_organizationMembers_query$key;
  onChange: (id?: string) => void;
  defaultValue?: string | null;
};

type CustomerDetails = {
  uniqueId: string;
  name: string | null | undefined;
  givenName: string | null | undefined;
  middleName: string | null | undefined;
  familyName: string | null | undefined;
  photoUrl: string | null | undefined;
};

const allId = 'kkigMVsUXwi2YMSSrXv7i';

const OrganizationUserSelector = ({ rootDataOrganizationMembersRelay, onChange, defaultValue }: Props) => {
  const rootDataOrganizationMembers = useFragment<organizationUserSelector_organizationMembers_query$key>(
    graphql`
      fragment organizationUserSelector_organizationMembers_query on Query {
        organizationMembers(
          where: { organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, nameContains: $peopleNameSearchText }
          orderBy: $organizationMembersSortingValues
        ) {
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

  const [id, setId] = useState<string>(defaultValue ?? allId);
  const customers = useMemo<CustomerDetails[]>(
    () => (rootDataOrganizationMembers.organizationMembers ? rootDataOrganizationMembers.organizationMembers.edges.map(({ node }) => node.customer) : []),
    [rootDataOrganizationMembers.organizationMembers],
  );

  const handleChanged = (event: SelectChangeEvent<unknown>) => {
    const id = event.target.value as string;

    setId(id);
    onChange(id === allId ? undefined : id);
  };

  return (
    <DefaultSelect
      value={id}
      onChange={handleChanged}
      size="small"
      renderValue={(selectedId) => {
        const selectedItem = customers.find((item) => item.uniqueId === selectedId);
        if (selectedItem) {
          return (
            <StackRow>
              <LeadIconTypography label="User" startElement={<UserIcon />} />
              <Divider orientation="vertical" flexItem />
              <PushToRight />
              <BodyIconTypography label={getCustomerFullName(selectedItem)} />
            </StackRow>
          );
        }

        return (
          <StackRow>
            <LeadIconTypography label="User" startElement={<UserIcon />} />
            <Divider orientation="vertical" flexItem />
            <PushToRight />
            <SmallIconTypography label="All" />
          </StackRow>
        );
      }}
    >
      <MenuItem value={allId}>
        <BodyIconTypography label="All" />
      </MenuItem>

      {customers.map((item) => (
        <MenuItem key={item.uniqueId} value={item.uniqueId}>
          <BodyIconTypography startElement={<CustomerAvatar photo={{ url: item.photoUrl }} name={item} size="small" />} label={getCustomerFullName(item)} />
        </MenuItem>
      ))}
    </DefaultSelect>
  );
};

export default memo(OrganizationUserSelector);

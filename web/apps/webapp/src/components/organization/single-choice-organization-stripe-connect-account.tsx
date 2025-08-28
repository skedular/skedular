import { BodyIconTypography } from '@/components/commons';
import type { singleChoiceOrganizationStripeConnectAccount_query$key } from '@/queries/__generated__/singleChoiceOrganizationStripeConnectAccount_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceOrganizationStripeConnectAccount_query$key;
  name: string;
  required?: boolean;
};

type OrganizationStripeConnectAccount = {
  readonly id: string;
  readonly name: string;
};

const SingleChoiceOrganizationStripeConnectAccount = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceOrganizationStripeConnectAccount_query$key>(
    graphql`
      fragment singleChoiceOrganizationStripeConnectAccount_query on Query @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null }) {
        organizationStripeConnectAccounts(
          first: $count
          after: $cursor
          where: { organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, onboardingCompleted: true }
          orderBy: $singleChoiceOrganizationStripeConnectAccountSortingValues
        ) @connection(key: "singleChoiceOrganizationStripeConnectAccount_organizationStripeConnectAccounts") {
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

  const accounts = useMemo<OrganizationStripeConnectAccount[]>(
    () => rootData.organizationStripeConnectAccounts.edges.map(({ node }) => node),
    [rootData.organizationStripeConnectAccounts],
  );
  const filter = createFilterOptions<OrganizationStripeConnectAccount>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={accounts}
      getOptionValue={(option) => (option as OrganizationStripeConnectAccount).id}
      getOptionLabel={(option: string | OrganizationStripeConnectAccount) => (option as OrganizationStripeConnectAccount).name}
      renderOption={(props, option) => {
        const castedOption = option as OrganizationStripeConnectAccount;

        return (
          <li {...props} key={castedOption.id}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as OrganizationStripeConnectAccount[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceOrganizationStripeConnectAccount);

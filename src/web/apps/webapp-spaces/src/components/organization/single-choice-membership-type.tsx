import type { singleChoiceMembershipTerm_query$key } from '@/queries/__generated__/singleChoiceMembershipTerm_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { BodyIconTypography } from '@skedular/ui';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceMembershipTerm_query$key;
  name: string;
  required?: boolean;
};

type MembershipTermDetails = {
  type: string;
  name: string;
};

const SingleChoiceMembershipTerm = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceMembershipTerm_query$key>(
    graphql`
      fragment singleChoiceMembershipTerm_query on Query {
        membershipTerms {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<MembershipTermDetails[]>(() => rootData.membershipTerms.map((item) => item), [rootData.membershipTerms]);
  const filter = createFilterOptions<MembershipTermDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={items}
      getOptionValue={(option) => (option as MembershipTermDetails).type}
      getOptionLabel={(option: string | MembershipTermDetails) => (option as MembershipTermDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as MembershipTermDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as MembershipTermDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceMembershipTerm);

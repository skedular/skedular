import { BodyIconTypography, HelperText } from '@/components/commons';
import type { singleChoiceUserPersonalInformationVisibility_query$key } from '@/queries/__generated__/singleChoiceUserPersonalInformationVisibility_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceUserPersonalInformationVisibility_query$key;
  name: string;
  required?: boolean;
};

type PersonalInformationVisibilityDetails = {
  type: string;
  name: string;
};

const SingleChoiceUserPersonalInformationVisibility = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceUserPersonalInformationVisibility_query$key>(
    graphql`
      fragment singleChoiceUserPersonalInformationVisibility_query on Query {
        personalInformationVisibilityTypes {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<PersonalInformationVisibilityDetails[]>(
    () => rootData.personalInformationVisibilityTypes.map((item) => item),
    [rootData.personalInformationVisibilityTypes],
  );
  const filter = createFilterOptions<PersonalInformationVisibilityDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={items}
      getOptionValue={(option) => (option as PersonalInformationVisibilityDetails).type}
      getOptionLabel={(option: string | PersonalInformationVisibilityDetails) => (option as PersonalInformationVisibilityDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as PersonalInformationVisibilityDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as PersonalInformationVisibilityDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
      helperText={<HelperText text="Choose whether your information is visible to others or shown in a redacted form" />}
    />
  );
};

export default memo(SingleChoiceUserPersonalInformationVisibility);

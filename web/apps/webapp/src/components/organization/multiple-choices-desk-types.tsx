import type { multipleChoicesDeskTypes_query$key } from '@/queries/__generated__/multipleChoicesDeskTypes_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesDeskTypes_query$key;
  name: string;
  required?: boolean;
};

type DeskTypeDetails = {
  id: string;
  name: string;
};

const MultipleChoicesDeskTypes = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<multipleChoicesDeskTypes_query$key>(
    graphql`
      fragment multipleChoicesDeskTypes_query on Query {
        deskTypes(where: { organizationId: $organizationId }, orderBy: $multipleChoicesDeskTypesSortingValues) {
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

  const deskTypes = useMemo<DeskTypeDetails[]>(() => {
    if (!rootData.deskTypes) {
      return [];
    }

    return rootData.deskTypes.edges.map(({ node }) => node);
  }, [rootData.deskTypes]);

  if (!rootData.deskTypes) {
    return <></>;
  }

  const filter = createFilterOptions<DeskTypeDetails>();

  return (
    <Autocomplete
      label="Desk types"
      name={name}
      multiple={true}
      required={required}
      options={deskTypes}
      getOptionValue={(option) => (option as DeskTypeDetails).id}
      getOptionLabel={(option: string | DeskTypeDetails) => (option as DeskTypeDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as DeskTypeDetails;

        return (
          <li {...props}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      disableCloseOnSelect={true}
      freeSolo={true}
      filterOptions={(options, params) => filter(options as DeskTypeDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(MultipleChoicesDeskTypes);

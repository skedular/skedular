import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import graphql from 'babel-plugin-relay/macro';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { useFragment } from 'react-relay';
import type { deskMultipleChoicesDeskTypes_query$key } from './__generated__/deskMultipleChoicesDeskTypes_query.graphql';

type Props = {
  rootDataRelay: deskMultipleChoicesDeskTypes_query$key;
  name: string;
  required?: boolean;
};

type DeskTypeDetails = {
  id: string;
  name: string;
};

const DeskMultipleChoicesDeskTypes = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<deskMultipleChoicesDeskTypes_query$key>(
    graphql`
      fragment deskMultipleChoicesDeskTypes_query on Query {
        deskTypes(where: { organizationId: $organizationId }, orderBy: $deskMultipleChoicesDeskTypesSortingValues) {
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
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <Typography variant="body1">{castedOption.name}</Typography>
            </Stack>
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

export default memo(DeskMultipleChoicesDeskTypes);

import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import graphql from 'babel-plugin-relay/macro';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { useFragment } from 'react-relay';
import type { bookingSingleChoiceOrganization_query$key } from './__generated__/bookingSingleChoiceOrganization_query.graphql';

type Props = {
  rootDataRelay: bookingSingleChoiceOrganization_query$key;
  name: string;
  required?: boolean;
  readOnly?: boolean;
};

type OrganizationDetails = {
  id: string;
  name: string;
};

const BookingSingleChoiceOrganization = ({ rootDataRelay, name, required, readOnly }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment bookingSingleChoiceOrganization_query on Query {
        myOrganizations {
          id
          name
        }
      }
    `,
    rootDataRelay,
  );

  const organizations = useMemo<OrganizationDetails[]>(
    () => (rootData.myOrganizations ? rootData.myOrganizations.map((organization) => organization) : []),
    [rootData.myOrganizations],
  );

  const filter = createFilterOptions<OrganizationDetails>();

  return (
    <Autocomplete
      label="Organization"
      name={name}
      multiple={false}
      required={required}
      options={organizations}
      getOptionValue={(option) => (option as OrganizationDetails).id}
      getOptionLabel={(option: string | OrganizationDetails) => (option as OrganizationDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as OrganizationDetails;

        return (
          <li {...props}>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <Typography variant="body1">{castedOption.name}</Typography>
            </Stack>
          </li>
        );
      }}
      disableCloseOnSelect={false}
      freeSolo={true}
      filterOptions={(options, params) => filter(options as OrganizationDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
      readOnly={readOnly}
    />
  );
};

export default memo(BookingSingleChoiceOrganization);

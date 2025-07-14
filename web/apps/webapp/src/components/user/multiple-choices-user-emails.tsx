import { BodyIconTypography } from '@/components/commons';
import type { multipleChoicesUserEmails_query$key } from '@/queries/__generated__/multipleChoicesUserEmails_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesUserEmails_query$key;
  name: string;
  required?: boolean;
};

const MultipleChoicesUserEmails = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<multipleChoicesUserEmails_query$key>(
    graphql`
      fragment multipleChoicesUserEmails_query on Query {
        me {
          emails
        }
      }
    `,
    rootDataRelay,
  );

  const zones = useMemo<string[]>(() => rootData.me.emails.map((item) => item), [rootData.me.emails]);
  const filter = createFilterOptions<string>();

  return (
    <Autocomplete
      name={name}
      multiple={true}
      required={required}
      options={zones}
      getOptionValue={(option) => option as string}
      getOptionLabel={(option: string) => option as string}
      renderOption={(props, option) => {
        const castedOption = option as string;

        return (
          <li {...props} key={castedOption}>
            <BodyIconTypography label={castedOption} />
          </li>
        );
      }}
      disableCloseOnSelect
      filterOptions={(options, params) => filter(options as string[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(MultipleChoicesUserEmails);

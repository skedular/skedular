import { AddressJsonV2, NominatimV4Client } from '@/libs/address/nominatim';
import { keyboardSearchDebounceTimeout } from '@/libs/utils';
import Autocomplete from '@mui/material/Autocomplete';
import CircularProgress from '@mui/material/CircularProgress';
import TextField from '@mui/material/TextField';
import { memo, useState } from 'react';
import { useDebounceCallback } from 'usehooks-ts';

type Props = {
  onSelect: (address: AddressJsonV2) => void;
};

const AddressSearch = ({ onSelect }: Props) => {
  const [open, setOpen] = useState(false);
  const [options, setOptions] = useState<AddressJsonV2[]>([]);
  const [loading, setLoading] = useState(false);

  const handleSearch = async (query: string) => {
    if (!query || query.length < 3) {
      return;
    }

    setLoading(true);

    try {
      const client = new NominatimV4Client();
      const result = await client.search.searchFreeForm(query);

      setOptions(result);
    } catch (error) {
      console.error('Address lookup error:', error);
    } finally {
      setLoading(false);
    }
  };

  const debounceHandleSearch = useDebounceCallback(handleSearch, keyboardSearchDebounceTimeout);

  return (
    <Autocomplete
      freeSolo
      disableClearable
      open={open && options.length > 0}
      onOpen={() => setOpen(true)}
      onClose={() => setOpen(false)}
      onInputChange={(event, value) => debounceHandleSearch(value)}
      options={options}
      loading={loading}
      filterOptions={(x) => x} // prevent client-side filtering removing server results
      getOptionLabel={(option: string | AddressJsonV2) => (option as AddressJsonV2).display_name ?? ''}
      onChange={(event, value: string | AddressJsonV2) => {
        if (value) {
          onSelect(value as AddressJsonV2);
        }
      }}
      renderInput={(params) => (
        <TextField
          {...params}
          label="Start typing to search for an address"
          autoComplete="new-password"
          slotProps={{
            input: {
              ...params.InputProps,
              endAdornment: (
                <>
                  {loading ? <CircularProgress color="inherit" size={20} /> : null}
                  {params.InputProps.endAdornment}
                </>
              ),
            },
          }}
        />
      )}
    />
  );
};

export default memo(AddressSearch);

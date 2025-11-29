import { createPlacesSessionToken, fetchPlaceDetails, fetchPlacePredictions } from '@/libs/address/google-places';
import { PlaceDetailsResult, PlacePrediction } from '@/libs/address/google-places-types';
import { keyboardSearchDebounceTimeout } from '@/libs/utils';
import Autocomplete from '@mui/material/Autocomplete';
import CircularProgress from '@mui/material/CircularProgress';
import TextField from '@mui/material/TextField';
import { memo, useState } from 'react';
import { useDebounceCallback } from 'usehooks-ts';

type Props = {
  onSelect: (address: PlaceDetailsResult) => void;
};

const GoogleAddressLookup = ({ onSelect }: Props) => {
  const [open, setOpen] = useState(false);
  const [options, setOptions] = useState<PlacePrediction[]>([]);
  const [loading, setLoading] = useState(false);
  const [sessionToken, setSessionToken] = useState(() => createPlacesSessionToken());

  const handleSearch = async (query: string) => {
    if (!query || query.length < 3) {
      setOptions([]);
      return;
    }

    setLoading(true);

    try {
      const predictions = await fetchPlacePredictions(query, sessionToken);
      setOptions(predictions);
    } catch (error) {
      console.error('Google Places lookup error:', error);
      setOptions([]);
    } finally {
      setLoading(false);
    }
  };

  const debounceHandleSearch = useDebounceCallback(handleSearch, keyboardSearchDebounceTimeout);

  return (
    <Autocomplete<PlacePrediction, false, true, true>
      freeSolo
      disableClearable
      open={open && options.length > 0}
      onOpen={() => setOpen(true)}
      onClose={() => setOpen(false)}
      onInputChange={(event, value, reason) => {
        if (reason === 'input') {
          debounceHandleSearch(value);
        }

        if (reason === 'clear' || value.length === 0) {
          setOptions([]);
          setLoading(false);
        }
      }}
      options={options}
      loading={loading}
      filterOptions={(x) => x}
      getOptionLabel={(option) => (typeof option === 'string' ? option : (option.description ?? ''))}
      isOptionEqualToValue={(option, value) => (typeof value === 'string' ? option.description === value : option.place_id === value.place_id)}
      onChange={async (event, value, reason) => {
        if (reason !== 'selectOption' || !value || typeof value === 'string') {
          return;
        }

        setLoading(true);

        try {
          const details = await fetchPlaceDetails(value.place_id, sessionToken);
          onSelect(details);
          setSessionToken(createPlacesSessionToken());
        } catch (error) {
          console.error('Google Places details lookup error:', error);
        } finally {
          setLoading(false);
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

export type GooglePlaceDetails = PlaceDetailsResult;

export default memo(GoogleAddressLookup);

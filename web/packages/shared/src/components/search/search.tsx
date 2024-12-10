import Divider from '@mui/material/Divider';
import OutlinedInput from '@mui/material/OutlinedInput';
import Stack from '@mui/material/Stack';
import debounce from 'lodash.debounce';
import { memo } from 'react';
import { SearchRoundedIcon } from '../../components/icons';
import { keyboardDebounceTimeout } from '../../libs/utils';

type Props = {
  size?: 'small' | 'medium';
  placeholder?: string;
  defaultValue?: unknown;
  onChange?: (searchTerm: string) => void;
};

const Search = ({ size, placeholder, defaultValue, onChange }: Props) => {
  const handleChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (!onChange) {
      return;
    }

    onChange(event.target.value);
  };

  const debounceChanged = debounce(handleChanged, keyboardDebounceTimeout);

  return (
    <OutlinedInput
      size={size}
      placeholder={placeholder}
      startAdornment={
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', paddingRight: 1 }}>
          <SearchRoundedIcon />
          <Divider orientation="vertical" flexItem />
        </Stack>
      }
      onChange={debounceChanged}
      defaultValue={defaultValue}
      sx={{
        '& .MuiOutlinedInput-notchedOutline': {
          borderRadius: 4,
        },
        width: {
          xs: '100%',
          sm: 'min(100%, 250px)',
        },
      }}
    />
  );
};

export default memo(Search);

import SearchRoundedIcon from '@mui/icons-material/SearchRounded';
import InputAdornment from '@mui/material/InputAdornment';
import OutlinedInput from '@mui/material/OutlinedInput';
import debounce from 'lodash.debounce';
import { memo } from 'react';
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
        <InputAdornment position="start">
          <SearchRoundedIcon fontSize="small" />
        </InputAdornment>
      }
      onChange={debounceChanged}
      defaultValue={defaultValue}
    />
  );
};

export default memo(Search);

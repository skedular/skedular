import SearchRoundedIcon from '@mui/icons-material/SearchRounded';
import InputAdornment from '@mui/material/InputAdornment';
import OutlinedInput from '@mui/material/OutlinedInput';
import { memo } from 'react';

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

  return (
    <OutlinedInput
      size={size}
      placeholder={placeholder}
      startAdornment={
        <InputAdornment position="start">
          <SearchRoundedIcon fontSize="small" />
        </InputAdornment>
      }
      onChange={handleChanged}
      defaultValue={defaultValue}
    />
  );
};

export default memo(Search);

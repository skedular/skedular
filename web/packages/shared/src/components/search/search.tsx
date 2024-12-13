import Divider from '@mui/material/Divider';
import OutlinedInput from '@mui/material/OutlinedInput';
import debounce from 'lodash.debounce';
import { memo } from 'react';
import { SearchRoundedIcon } from '../../components/icons';
import { keyboardDebounceTimeout } from '../../libs/utils';
import { StackRow } from '../commons';

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
        <StackRow sx={{ paddingRight: 1 }}>
          <SearchRoundedIcon />
          <Divider orientation="vertical" flexItem />
        </StackRow>
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

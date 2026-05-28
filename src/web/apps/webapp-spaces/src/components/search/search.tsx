import { StackRow } from '@skedular/ui';
import { SearchRoundedIcon } from '@/components/icons';
import { keyboardSearchDebounceTimeout } from '@skedular/shared';
import Divider from '@mui/material/Divider';
import OutlinedInput from '@mui/material/OutlinedInput';
import type { SxProps, Theme } from '@mui/system';
import { memo } from 'react';
import { useDebounceCallback } from 'usehooks-ts';

type Props = {
  size?: 'small' | 'medium';
  placeholder?: string;
  defaultValue?: unknown;
  sx?: SxProps<Theme>;
  onChange?: (searchTerm: string) => void;
};

const Search = ({ size, placeholder, defaultValue, sx, onChange }: Props) => {
  const handleChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (!onChange) {
      return;
    }

    onChange(event.target.value);
  };

  const debounceChanged = useDebounceCallback(handleChanged, keyboardSearchDebounceTimeout);

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
        ...sx,
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

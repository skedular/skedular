import type { SelectProps } from '@mui/material/Select';
import Select from '@mui/material/Select';
import { styled } from '@mui/material/styles';

const DefaultSelect = styled(Select)(({ theme }) => ({
  width: '100%',
  [theme.breakpoints.up('sm')]: {
    width: 'min(100%, 320px)',
  },
  '& .MuiOutlinedInput-root': {
    borderRadius: 30,
  },
  '& .MuiOutlinedInput-notchedOutline': {
    borderRadius: 30,
  },
  '& .MuiSelect-select': {
    borderRadius: 30,
  },
})) as React.ComponentType<SelectProps>;

export default DefaultSelect;

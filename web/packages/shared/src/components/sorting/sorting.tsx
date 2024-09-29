import SortIcon from '@mui/icons-material/Sort';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { memo, useState } from 'react';
import { AscDirectionIcon, DescDirectionIcon } from '../icons';

export type Direction = 'Ascending' | 'Descending';

type Props = {
  options: SortByOption[];
  defaultOption: string;
  defaultSortingDirectionValue: Direction;
  onValueChange: (sortingDirection: Direction, option: string) => void;
};

export interface SortByOption {
  id: string;
  label: string;
}

const SortingDirection = ({ options, defaultOption, defaultSortingDirectionValue, onValueChange }: Props) => {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);
  const [selectedOption, setSelectedOption] = useState(defaultOption);
  const [selectedSortingDirection, setSelectedSortingDirection] = useState(defaultSortingDirectionValue);

  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleAscendingDirectionClicked = () => {
    setAnchorEl(null);
    setSelectedSortingDirection('Ascending');
    onValueChange('Ascending', selectedOption);
  };

  const handleDescendingDirectionClicked = () => {
    setAnchorEl(null);
    setSelectedSortingDirection('Descending');
    onValueChange('Descending', selectedOption);
  };

  const handleOptionChange = (id: string) => {
    setAnchorEl(null);
    setSelectedOption(id);
    onValueChange(selectedSortingDirection, id);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  return (
    <>
      <Tooltip title="Sort">
        <IconButton id="long-button" onClick={handleClick}>
          <SortIcon />
        </IconButton>
      </Tooltip>
      <Menu anchorEl={anchorEl} open={open} onClose={handleMenuClose}>
        <MenuItem selected={false}>
          <Typography textAlign="center">Direction</Typography>
        </MenuItem>

        <MenuItem selected={selectedSortingDirection === 'Ascending'} onClick={handleAscendingDirectionClicked}>
          <ListItemIcon>
            <AscDirectionIcon fontSize="small" />
          </ListItemIcon>
          <Typography textAlign="center">Sort Ascending</Typography>
        </MenuItem>

        <MenuItem selected={selectedSortingDirection === 'Descending'} onClick={handleDescendingDirectionClicked}>
          <ListItemIcon>
            <DescDirectionIcon fontSize="small" />
          </ListItemIcon>
          <Typography textAlign="center">Sort Descending</Typography>
        </MenuItem>

        <Divider />

        <MenuItem selected={false}>
          <Typography textAlign="center">Sort by</Typography>
        </MenuItem>

        {options.map((option) => (
          <MenuItem key={option.id} selected={option.id === selectedOption} onClick={() => handleOptionChange(option.id)}>
            <Typography textAlign="center">{option.label}</Typography>
          </MenuItem>
        ))}
      </Menu>
    </>
  );
};

export default memo(SortingDirection);

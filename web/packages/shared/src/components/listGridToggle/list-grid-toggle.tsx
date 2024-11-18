import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import { memo, useState } from 'react';
import { GridViewIcon, ListViewIcon } from '../icons';

type Props = {
  defaultValue: 'list' | 'grid';
  onChange: (view: 'list' | 'grid') => void;
};

const ListGridToggle = ({ defaultValue, onChange }: Props) => {
  const [alignment, setAlignment] = useState<string>(defaultValue ?? 'list');

  const handleChange = (_: React.MouseEvent<HTMLElement>, newAlignment: string) => {
    if (!newAlignment) {
      return;
    }

    setAlignment(newAlignment);
    onChange(newAlignment as 'list' | 'grid');
  };

  return (
    <ToggleButtonGroup
      value={alignment}
      exclusive
      onChange={handleChange}
      sx={{
        borderRadius: 4,
        overflow: 'hidden', // Ensures no visual artifacts from children
      }}
    >
      <ToggleButton value="list">
        <ListViewIcon />
      </ToggleButton>
      <ToggleButton value="grid">
        <GridViewIcon />
      </ToggleButton>
    </ToggleButtonGroup>
  );
};

export default memo(ListGridToggle);

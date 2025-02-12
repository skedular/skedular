import { GridViewIcon, ListViewIcon } from '@/components/icons';
import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import Tooltip from '@mui/material/Tooltip';
import { memo, useState } from 'react';

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
      <Tooltip title="List layout">
        <ToggleButton value="list">
          <ListViewIcon />
        </ToggleButton>
      </Tooltip>
      <Tooltip title="Grid layout">
        <ToggleButton value="grid">
          <GridViewIcon />
        </ToggleButton>
      </Tooltip>
    </ToggleButtonGroup>
  );
};

export default memo(ListGridToggle);

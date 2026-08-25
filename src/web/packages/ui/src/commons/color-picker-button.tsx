'use client';

import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Popover from '@mui/material/Popover';
import { useEffect, useState } from 'react';
import ColorPicker from './color-picker';

export type ColorPickerButtonProps = {
  defaultColor?: string | null;
  onChange?: (color: string) => void;
  label?: string;
};

const ColorPickerButton = ({ defaultColor, onChange, label = 'Change color' }: ColorPickerButtonProps) => {
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);
  const [selectedColor, setSelectedColor] = useState(defaultColor ?? '#87CEEB');

  useEffect(() => {
    if (defaultColor) setSelectedColor(defaultColor);
  }, [defaultColor]);

  const handleChange = (color: string) => {
    setSelectedColor(color);
    onChange?.(color);
  };

  return (
    <>
      <Button
        variant="outlined"
        color="inherit"
        onClick={(event) => setAnchorEl(event.currentTarget)}
        startIcon={<Box sx={{ width: 14, height: 14, borderRadius: '50%', backgroundColor: selectedColor, border: 1, borderColor: 'divider' }} />}
        sx={{ textTransform: 'none' }}
      >
        {label}
      </Button>
      <Popover open={Boolean(anchorEl)} anchorEl={anchorEl} onClose={() => setAnchorEl(null)} anchorOrigin={{ vertical: 'bottom', horizontal: 'left' }}>
        <Box sx={{ p: 2, maxWidth: 360 }}>
          <ColorPicker onChange={handleChange} defaultColor={selectedColor} />
        </Box>
      </Popover>
    </>
  );
};

export default ColorPickerButton;

import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid2';
import { useEffect, useState } from 'react';
import { SelectedTickIcon } from '../icons';
import GridContainer from './grid-container';

type Props = {
  defaultColor?: string | null | undefined;
  onChange?: (color: string) => void;
};

const colors = [
  '#87CEEB',
  '#FFD700',
  '#FF6347',
  '#32CD32',
  '#98FB98',
  '#B0E0E6',
  '#F5DEB3',
  '#20B2AA',
  '#4682B4',
  '#DAA520',
  '#FF8C00',
  '#2E8B57',
  '#8A2BE2',
  '#FF00FF',
  '#D2691E',
  '#CD5C5C',
];

const ColorPicker = ({ defaultColor, onChange }: Props) => {
  const [selectedColor, setSelectedColor] = useState<string>(defaultColor && colors.includes(defaultColor) ? defaultColor : colors[0]!);

  useEffect(() => {
    if (onChange) {
      onChange(selectedColor);
    }
  }, []);

  const handleChange = (color: string) => {
    if (onChange) {
      onChange(color);
    }

    setSelectedColor(color);
  };

  return (
    <GridContainer spacing={1}>
      {colors.map((item) => (
        <Grid key={item}>
          <Box
            sx={{
              width: 40,
              height: 40,
              borderRadius: '50%',
              backgroundColor: item,
              alignItems: 'center',
              display: 'flex',
              justifyContent: 'center',
              position: 'relative',
              cursor: 'pointer',
              border: selectedColor === item ? '2px solid #000' : 'none',
            }}
            onClick={() => handleChange(item)}
          >
            {selectedColor === item && <SelectedTickIcon sx={{ color: '#fff', fontSize: 24 }} />}
          </Box>
        </Grid>
      ))}
    </GridContainer>
  );
};

export default ColorPicker;

import Stack from '@mui/material/Stack';
import type { SxProps, Theme } from '@mui/system';
import { ResponsiveStyleValue } from '@mui/system';
import { PropsWithChildren } from 'react';

type Props = {
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
};

const StackRow = ({ children, sx, spacing }: PropsWithChildren<Props>) => (
  <Stack direction="row" spacing={spacing ?? 1} sx={{ alignItems: 'center', flexWrap: 'wrap', ...sx }}>
    {children}
  </Stack>
);

export default StackRow;

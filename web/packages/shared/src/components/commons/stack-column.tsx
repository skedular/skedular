import Stack from '@mui/material/Stack';
import type { SxProps, Theme } from '@mui/system';
import { ResponsiveStyleValue } from '@mui/system';
import { PropsWithChildren } from 'react';

type Props = {
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
};

const StackColumn = ({ children, sx, spacing }: PropsWithChildren<Props>) => (
  <Stack direction="column" spacing={spacing ?? 1} sx={sx}>
    {children}
  </Stack>
);
export default StackColumn;

import Stack from '@mui/material/Stack';
import type { SxProps, Theme } from '@mui/system';
import { PropsWithChildren } from 'react';

type Props = {
  sx?: SxProps<Theme>;
};

const StackColumn = ({ children, sx }: PropsWithChildren<Props>) => (
  <Stack direction="column" spacing={1} sx={sx}>
    {children}
  </Stack>
);
export default StackColumn;

import Stack from '@mui/material/Stack';
import type { SxProps, Theme } from '@mui/system';
import { PropsWithChildren } from 'react';

type Props = {
  sx?: SxProps<Theme>;
};

const StackRow = ({ children, sx }: PropsWithChildren<Props>) => (
  <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap', ...sx }}>
    {children}
  </Stack>
);

export default StackRow;

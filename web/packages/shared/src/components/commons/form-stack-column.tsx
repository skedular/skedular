import Stack from '@mui/material/Stack';
import type { SxProps, Theme } from '@mui/system';
import { ResponsiveStyleValue } from '@mui/system';
import type { PropsWithChildren } from 'react';

interface AnyObject {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  [key: string]: any;
}

type Props = {
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
  onSubmit: (event?: Partial<Pick<React.SyntheticEvent, 'preventDefault' | 'stopPropagation'>>) => Promise<AnyObject | undefined> | undefined;
};

const FormStackColumn = ({ children, sx, spacing, onSubmit }: PropsWithChildren<Props>) => (
  <Stack direction="column" spacing={spacing ?? 1} sx={{ padding: 2, ...sx }} component="form" noValidate onSubmit={onSubmit}>
    {children}
  </Stack>
);

export default FormStackColumn;

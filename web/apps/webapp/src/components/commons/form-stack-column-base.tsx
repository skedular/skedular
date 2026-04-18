import Stack from '@mui/material/Stack';
import type { SxProps, Theme } from '@mui/system';
import { ResponsiveStyleValue } from '@mui/system';
import type { PropsWithChildren } from 'react';

type Props = {
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
  onSubmit: (event?: Partial<Pick<React.SyntheticEvent, 'preventDefault' | 'stopPropagation'>>) => Promise<unknown> | undefined;
};

const FormStackColumnBase = ({ children, sx, spacing, onSubmit }: PropsWithChildren<Props>) => (
  <Stack direction="column" spacing={spacing === undefined ? 1 : spacing} sx={sx} component="form" noValidate onSubmit={onSubmit}>
    {children}
  </Stack>
);

export default FormStackColumnBase;

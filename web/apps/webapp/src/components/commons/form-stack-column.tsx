import Stack from '@mui/material/Stack';
import type { SxProps, Theme } from '@mui/system';
import { ResponsiveStyleValue } from '@mui/system';
import type { PropsWithChildren } from 'react';

interface AnyObject {
  [key: string]: any;
}

type Props = {
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
  onSubmit: (event?: Partial<Pick<React.SyntheticEvent, 'preventDefault' | 'stopPropagation'>>) => Promise<AnyObject | undefined> | undefined;
};

const FormStackColumn = ({ children, sx, spacing, onSubmit }: PropsWithChildren<Props>) => (
  <Stack direction="column" spacing={spacing === undefined ? 1 : spacing} sx={{ paddingTop: 0, ...sx }} component="form" noValidate onSubmit={onSubmit}>
    {children}
  </Stack>
);

export default FormStackColumn;

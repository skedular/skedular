import type { SxProps, Theme } from '@mui/system';
import { ResponsiveStyleValue } from '@mui/system';
import type { PropsWithChildren } from 'react';
import FormStackColumnBase from './form-stack-column-base';

interface AnyObject {
  [key: string]: unknown;
}

type Props = {
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
  onSubmit: (event?: Partial<Pick<React.SyntheticEvent, 'preventDefault' | 'stopPropagation'>>) => Promise<AnyObject | undefined> | undefined;
};

const FormStackColumn = ({ children, sx, spacing, onSubmit }: PropsWithChildren<Props>) => (
  <FormStackColumnBase spacing={spacing === undefined ? 1 : spacing} sx={{ paddingTop: 0, ...sx }} onSubmit={onSubmit}>
    {children}
  </FormStackColumnBase>
);

export default FormStackColumn;

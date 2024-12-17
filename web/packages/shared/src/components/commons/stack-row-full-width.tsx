import type { SxProps, Theme } from '@mui/system';
import { ResponsiveStyleValue } from '@mui/system';
import { PropsWithChildren } from 'react';
import StackRow from './stack-row';

type Props = {
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
};

const StackRowFullWidth = ({ children, sx, spacing }: PropsWithChildren<Props>) => (
  <StackRow spacing={spacing} sx={{ justifyContent: 'space-between', width: '100%', alignItems: 'center', ...sx }}>
    {children}
  </StackRow>
);

export default StackRowFullWidth;

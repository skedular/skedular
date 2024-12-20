import type { CSSProperties } from '@mui/material/styles/createTypography';
import type { SxProps, Theme } from '@mui/system';
import { ResponsiveStyleValue } from '@mui/system';
import { PropsWithChildren } from 'react';
import StackRow from './stack-row';

type Props = {
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
  color?: CSSProperties['color'];
};

const StackRowFullWidth = ({ children, sx, spacing, color }: PropsWithChildren<Props>) => (
  <StackRow spacing={spacing} sx={{ justifyContent: 'space-between', width: '100%', alignItems: 'center', ...sx }} color={color}>
    {children}
  </StackRow>
);

export default StackRowFullWidth;

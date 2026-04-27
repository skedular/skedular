'use client';

import Stack from '@mui/material/Stack';
import type { CSSProperties } from '@mui/material/styles';
import type { SxProps, Theme } from '@mui/system';
import { ResponsiveStyleValue } from '@mui/system';
import type { ForwardedRef, PropsWithChildren } from 'react';
import { forwardRef } from 'react';

type Props = {
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
  color?: CSSProperties['color'];
};

const StackColumn = ({ children, sx, spacing, color }: PropsWithChildren<Props>, ref: ForwardedRef<HTMLDivElement>) => (
  <Stack direction="column" spacing={spacing === undefined ? 1 : spacing} sx={sx} color={color} ref={ref}>
    {children}
  </Stack>
);

export default forwardRef<HTMLDivElement, PropsWithChildren<Props>>(StackColumn);

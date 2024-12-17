import type { CSSProperties } from '@mui/material/styles/createTypography';
import type { ResponsiveStyleValue, SxProps, Theme } from '@mui/system';
import type { JSX } from 'react';
import IconTypography from './icon-typography';

type Props = {
  icon?: React.ReactNode | JSX.Element;
  stackMode?: 'row' | 'column';
  label?: string | null | undefined;
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
  color?: CSSProperties['color'];
};

const LeadIconTypography = ({ icon, stackMode, label, sx, spacing, color }: Props) => (
  <IconTypography icon={icon} stackMode={stackMode} label={label} variant="h6" sx={sx} spacing={spacing} color={color} />
);
export default LeadIconTypography;

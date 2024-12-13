import type { CSSProperties } from '@mui/material/styles/createTypography';
import type { SxProps, Theme } from '@mui/system';
import type { JSX } from 'react';
import IconTypography from './icon-typography';

type Props = {
  icon?: React.ReactNode | JSX.Element;
  stackMode?: 'row' | 'column';
  label?: string | null | undefined;
  sx?: SxProps<Theme>;
  color?: CSSProperties['color'];
};

const SmallHeadingIconTypography = ({ icon, stackMode, label, sx, color }: Props) => (
  <IconTypography icon={icon} stackMode={stackMode} label={label} variant="h4" sx={sx} color={color} />
);
export default SmallHeadingIconTypography;

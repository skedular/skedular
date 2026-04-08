import type { CSSProperties } from '@mui/material/styles';
import type { ResponsiveStyleValue, SxProps, Theme } from '@mui/system';
import type { ReactNode } from 'react';
import IconTypography from './icon-typography';

type Props = {
  startElement?: ReactNode;
  endElement?: ReactNode;
  stackMode?: 'row' | 'column';
  label?: ReactNode;
  noWrap?: boolean;
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
  color?: CSSProperties['color'];
  invertDefaultColor?: boolean;
  fontWeight?: CSSProperties['fontWeight'];
};

const LargeHeadingIconTypography = ({ startElement, endElement, stackMode, label, noWrap, sx, spacing, color, fontWeight }: Props) => (
  <IconTypography
    startElement={startElement}
    endElement={endElement}
    stackMode={stackMode}
    label={label}
    noWrap={noWrap}
    variant="h2"
    sx={sx}
    spacing={spacing}
    color={color}
    fontWeight={fontWeight}
  />
);

export default LargeHeadingIconTypography;

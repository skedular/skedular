import type { CSSProperties } from '@mui/material/styles';
import type { ResponsiveStyleValue, SxProps, Theme } from '@mui/system';
import type { JSX } from 'react';
import IconTypography from './icon-typography';

type Props = {
  startElement?: React.ReactNode | JSX.Element;
  endElement?: React.ReactNode | JSX.Element;
  stackMode?: 'row' | 'column';
  label?: string | null | undefined;
  noWrap?: boolean;
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
  color?: CSSProperties['color'];
  invertDefaultColor?: boolean;
  fontWeight?: CSSProperties['fontWeight'];
};

const SectionIconTypography = ({ startElement, endElement, stackMode, label, noWrap, sx, spacing, color, invertDefaultColor, fontWeight }: Props) => (
  <IconTypography
    startElement={startElement}
    endElement={endElement}
    stackMode={stackMode}
    label={label}
    noWrap={noWrap}
    variant="h5"
    sx={sx}
    spacing={spacing}
    color={color}
    invertDefaultColor={invertDefaultColor}
    fontWeight={fontWeight}
  />
);

export default SectionIconTypography;

import type { CSSProperties } from '@mui/material/styles';
import type { ResponsiveStyleValue, SxProps, Theme } from '@mui/system';
import type { JSX } from 'react';
import IconTypography from './icon-typography';

type Props = {
  startElement?: React.ReactNode | JSX.Element;
  endElement?: React.ReactNode | JSX.Element;
  stackMode?: 'row' | 'column';
  label?: string | null | undefined;
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
  color?: CSSProperties['color'];
  invertDefaultColor?: boolean;
};

const SubtitleIconTypography = ({ startElement, endElement, stackMode, label, sx, spacing, color, invertDefaultColor }: Props) => (
  <IconTypography
    startElement={startElement}
    endElement={endElement}
    stackMode={stackMode}
    label={label}
    variant="subtitle1"
    sx={sx}
    spacing={spacing}
    color={color}
    invertDefaultColor={invertDefaultColor}
  />
);

export default SubtitleIconTypography;

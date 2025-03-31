import { PaletteModeContext } from '@/libs/providers';
import { coal, sandstone } from '@/libs/theme';
import type { CSSProperties, TypographyVariant } from '@mui/material/styles';
import Typography from '@mui/material/Typography';
import type { SxProps, Theme } from '@mui/system';
import { ResponsiveStyleValue } from '@mui/system';
import { useContext, type JSX } from 'react';
import StackColumn from './stack-column';
import StackRow from './stack-row';

type Props = {
  startElement?: React.ReactNode | JSX.Element;
  endElement?: React.ReactNode | JSX.Element;
  stackMode?: 'row' | 'column';
  label?: string | null | undefined;
  noWrap?: boolean;
  variant?: TypographyVariant;
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
  color?: CSSProperties['color'];
  invertDefaultColor?: boolean;
};

const IconTypography = ({ startElement, endElement, stackMode, label, noWrap, variant, sx, spacing, color, invertDefaultColor }: Props) => {
  const paletteMode = useContext(PaletteModeContext);
  const finalColor = invertDefaultColor ? (paletteMode === 'dark' ? coal : sandstone) : color;

  if (!startElement && !label && !endElement) {
    return <></>;
  }

  if (!startElement && !endElement) {
    return (
      <Typography variant={variant} sx={sx} color={finalColor} noWrap={noWrap}>
        {label}
      </Typography>
    );
  }

  if (stackMode === 'column') {
    return (
      <StackColumn sx={sx} spacing={spacing}>
        {startElement}
        {label && (
          <Typography variant={variant} color={finalColor} noWrap={noWrap}>
            {label}
          </Typography>
        )}
        {endElement}
      </StackColumn>
    );
  }

  return (
    <StackRow sx={sx} spacing={spacing} color={finalColor}>
      {startElement}
      {label && (
        <Typography variant={variant} color={finalColor} noWrap={noWrap}>
          {label}
        </Typography>
      )}
      {endElement}
    </StackRow>
  );
};

export default IconTypography;

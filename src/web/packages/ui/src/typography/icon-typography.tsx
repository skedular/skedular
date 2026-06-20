'use client';

import type { CSSProperties, TypographyVariant } from '@mui/material/styles';
import Typography from '@mui/material/Typography';
import type { SxProps, Theme } from '@mui/system';
import { ResponsiveStyleValue } from '@mui/system';
import { useContext, type ElementType, type ReactNode } from 'react';
import StackColumn from '../stack-column';
import StackRow from '../stack-row';
import { PaletteModeContext } from '../theme/palette-mode-context';
import { coal, sandstone } from '../theme/theme-primitives';

type Props = {
  startElement?: ReactNode;
  endElement?: ReactNode;
  stackMode?: 'row' | 'column';
  label?: ReactNode;
  noWrap?: boolean;
  variant?: TypographyVariant;
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
  color?: CSSProperties['color'];
  invertDefaultColor?: boolean;
  fontWeight?: CSSProperties['fontWeight'];
  component?: ElementType;
  'aria-hidden'?: boolean | 'true' | 'false';
};

const IconTypography = ({
  startElement,
  endElement,
  stackMode,
  label,
  noWrap,
  variant,
  sx,
  spacing,
  color,
  invertDefaultColor,
  fontWeight,
  component,
  'aria-hidden': ariaHidden,
}: Props) => {
  const paletteMode = useContext(PaletteModeContext);
  const finalColor = invertDefaultColor ? (paletteMode === 'dark' ? coal : sandstone) : color;
  const typographySx = fontWeight === undefined ? sx : Array.isArray(sx) ? [{ fontWeight }, ...sx] : sx ? [{ fontWeight }, sx] : [{ fontWeight }];
  const componentProps = component ? { component } : {};

  if (!startElement && !label && !endElement) {
    return null;
  }

  if (!startElement && !endElement) {
    return (
      <Typography {...componentProps} aria-hidden={ariaHidden} variant={variant} sx={typographySx} color={finalColor} noWrap={noWrap}>
        {label}
      </Typography>
    );
  }

  if (stackMode === 'column') {
    return (
      <StackColumn sx={sx} spacing={spacing}>
        {startElement}
        {label && (
          <Typography {...componentProps} aria-hidden={ariaHidden} variant={variant} color={finalColor} noWrap={noWrap} sx={{ fontWeight }}>
            {label}
          </Typography>
        )}
        {endElement}
      </StackColumn>
    );
  }

  return (
    <StackRow sx={sx} spacing={spacing}>
      {startElement}
      {label && (
        <Typography {...componentProps} aria-hidden={ariaHidden} variant={variant} color={finalColor} noWrap={noWrap} sx={{ fontWeight }}>
          {label}
        </Typography>
      )}
      {endElement}
    </StackRow>
  );
};

export default IconTypography;

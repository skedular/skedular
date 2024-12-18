import type { CSSProperties, Variant } from '@mui/material/styles/createTypography';
import Typography from '@mui/material/Typography';
import type { SxProps, Theme } from '@mui/system';
import { ResponsiveStyleValue } from '@mui/system';
import type { JSX } from 'react';
import StackColumn from './stack-column';
import StackRow from './stack-row';

type Props = {
  startElement?: React.ReactNode | JSX.Element;
  endElement?: React.ReactNode | JSX.Element;
  stackMode?: 'row' | 'column';
  label?: string | null | undefined;
  variant?: Variant;
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
  color?: CSSProperties['color'];
};

const IconTypography = ({ startElement, endElement, stackMode, label, variant, sx, spacing, color }: Props) => {
  if (!startElement && !label && !endElement) {
    return <></>;
  }

  if (!startElement && !endElement) {
    return (
      <Typography variant={variant} sx={sx}>
        {label}
      </Typography>
    );
  }

  if (stackMode === 'column') {
    return (
      <StackColumn sx={sx} spacing={spacing}>
        {startElement}
        {label && (
          <Typography variant={variant} color={color}>
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
        <Typography variant={variant} color={color}>
          {label}
        </Typography>
      )}
      {endElement}
    </StackRow>
  );
};

export default IconTypography;

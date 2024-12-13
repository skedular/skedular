import type { CSSProperties, Variant } from '@mui/material/styles/createTypography';
import Typography from '@mui/material/Typography';
import type { SxProps, Theme } from '@mui/system';
import type { JSX } from 'react';
import StackColumn from './stack-column';
import StackRow from './stack-row';

type Props = {
  icon?: React.ReactNode | JSX.Element;
  stackMode?: 'row' | 'column';
  label?: string | null | undefined;
  variant?: Variant;
  sx?: SxProps<Theme>;
  color?: CSSProperties['color'];
};

const IconTypography = ({ icon, stackMode, label, variant, sx, color }: Props) => {
  if (!icon && !label) {
    return <></>;
  }

  if (!icon) {
    return (
      <Typography variant={variant} sx={sx}>
        {label}
      </Typography>
    );
  }

  if (stackMode === 'column') {
    return (
      <StackColumn sx={sx}>
        {icon}
        {label && (
          <Typography variant={variant} color={color}>
            {label}
          </Typography>
        )}
      </StackColumn>
    );
  }

  return (
    <StackRow sx={sx}>
      {icon}
      {label && (
        <Typography variant={variant} color={color}>
          {label}
        </Typography>
      )}
    </StackRow>
  );
};

export default IconTypography;

import { getOrganizationAddLink } from '@/components/organization';
import Button from '@mui/material/Button';
import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@repo/shared/components/commons';
import { NewIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { coal, emerald, sandstone } from '@repo/shared/libs/theme';
import { memo, useContext } from 'react';

type Props = {
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
  invertDefaultColor?: boolean;
};

const NewOrganizationButton = ({ fullWidth, label, hideIcon, variant, size, invertDefaultColor }: Props) => {
  const paletteMode = useContext(PaletteModeContext);

  return (
    <Button
      href={getOrganizationAddLink()}
      variant={variant ?? 'text'}
      fullWidth={fullWidth}
      sx={{ borderRadius: 4, backgroundColor: invertDefaultColor ? (paletteMode === 'dark' ? coal : sandstone) : 'inherit' }}
    >
      {size === 'small' && (
        <SmallIconTypography
          label={label ?? 'Add Organization'}
          endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} sx={{ color: emerald }} />}
        />
      )}
      {size === 'medium' && (
        <BodyIconTypography
          label={label ?? 'Add Organization'}
          endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} sx={{ color: emerald }} />}
        />
      )}
      {(size === 'large' || !size) && (
        <LeadIconTypography
          label={label ?? 'Add Organization'}
          endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} sx={{ color: emerald }} />}
        />
      )}
    </Button>
  );
};

export default memo(NewOrganizationButton);

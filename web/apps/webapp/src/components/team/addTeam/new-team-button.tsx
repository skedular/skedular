import { getTeamAddLink } from '@/components/team';
import Button from '@mui/material/Button';
import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@repo/shared/components/commons';
import { NewIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { coal, emerald, sandstone } from '@repo/shared/libs/theme';
import { memo, useContext } from 'react';

type Props = {
  organizationId?: string;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
  invertDefaultColor?: boolean;
};

const NewTeamButton = ({ organizationId, fullWidth, label, hideIcon, variant, size, invertDefaultColor }: Props) => {
  const paletteMode = useContext(PaletteModeContext);

  return (
    <Button
      href={getTeamAddLink(organizationId)}
      variant={variant ?? 'text'}
      fullWidth={fullWidth}
      sx={{ borderRadius: 4, backgroundColor: invertDefaultColor ? (paletteMode === 'dark' ? coal : sandstone) : 'inherit' }}
    >
      {size === 'small' && (
        <SmallIconTypography
          label={label ?? 'Create a Team'}
          endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} sx={{ color: emerald }} />}
        />
      )}
      {size === 'medium' && (
        <BodyIconTypography
          label={label ?? 'Create a Team'}
          endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} sx={{ color: emerald }} />}
        />
      )}
      {(size === 'large' || !size) && (
        <LeadIconTypography
          label={label ?? 'Create a Team'}
          endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} sx={{ color: emerald }} />}
        />
      )}
    </Button>
  );
};

export default memo(NewTeamButton);

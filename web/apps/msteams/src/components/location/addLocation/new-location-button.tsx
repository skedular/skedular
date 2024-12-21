import Button from '@mui/material/Button';
import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@repo/shared/components/commons';
import { NewIcon } from '@repo/shared/components/icons';
import { getLocationAddLink } from 'components/location';
import { memo } from 'react';

type Props = {
  organizationId: string;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const NewLocationButton = ({ organizationId, fullWidth, label, hideIcon, variant, size }: Props) => (
  <Button href={getLocationAddLink(organizationId)} variant={variant ?? 'text'} fullWidth={fullWidth} sx={{ borderRadius: 4 }}>
    {size === 'small' && (
      <SmallIconTypography label={label ?? 'Add Location'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} />
    )}
    {size === 'medium' && (
      <BodyIconTypography label={label ?? 'Add Locationm'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} />
    )}
    {(size === 'large' || !size) && (
      <LeadIconTypography label={label ?? 'Add Location'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} />
    )}
  </Button>
);

export default memo(NewLocationButton);

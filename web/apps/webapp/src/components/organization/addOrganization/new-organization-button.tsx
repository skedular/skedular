import { getOrganizationAddLink } from '@/components/organization';
import Button from '@mui/material/Button';
import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@repo/shared/components/commons';
import { NewIcon } from '@repo/shared/components/icons';
import { memo } from 'react';

type Props = {
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const NewOrganizationButton = ({ fullWidth, label, hideIcon, variant, size }: Props) => (
  <Button href={getOrganizationAddLink()} variant={variant ?? 'text'} fullWidth={fullWidth} sx={{ borderRadius: 4 }}>
    {size === 'small' && (
      <SmallIconTypography label={label ?? 'Add Organization'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} />
    )}
    {size === 'medium' && (
      <BodyIconTypography label={label ?? 'Add Organization'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} />
    )}
    {(size === 'large' || !size) && (
      <LeadIconTypography label={label ?? 'Add Organization'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} />
    )}
  </Button>
);

export default memo(NewOrganizationButton);

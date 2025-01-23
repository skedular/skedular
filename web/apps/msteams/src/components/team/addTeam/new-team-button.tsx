import Button from '@mui/material/Button';
import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@repo/shared/components/commons';
import { NewIcon } from '@repo/shared/components/icons';
import { getTeamAddLink } from 'components/links';
import { memo } from 'react';

type Props = {
  organizationId: string;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const NewTeamButton = ({ organizationId, fullWidth, label, hideIcon, variant, size }: Props) => (
  <Button href={getTeamAddLink(organizationId)} variant={variant ?? 'text'} fullWidth={fullWidth}>
    {size === 'small' && (
      <SmallIconTypography label={label ?? 'Create a Team'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} />
    )}
    {size === 'medium' && (
      <BodyIconTypography label={label ?? 'Create a Team'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} />
    )}
    {(size === 'large' || !size) && (
      <LeadIconTypography label={label ?? 'Create a Team'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} />
    )}
  </Button>
);

export default memo(NewTeamButton);

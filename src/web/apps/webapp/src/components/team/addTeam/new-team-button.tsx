import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@skedular/ui';
import { NewIcon } from '@/components/icons';
import { getOrganizationTeamAddLink } from '@/components/links';
import { useIntegratedPlatform } from '@skedular/shared';
import Button from '@mui/material/Button';
import { memo } from 'react';

type Props = {
  organizationCustomDomain: string;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const NewTeamButton = ({ organizationCustomDomain, fullWidth, label, hideIcon, variant, size }: Props) => {
  const { integratedPlatform } = useIntegratedPlatform();

  return (
    <Button href={getOrganizationTeamAddLink(integratedPlatform, organizationCustomDomain)} variant={variant ?? 'text'} fullWidth={fullWidth}>
      {size === 'small' && <SmallIconTypography label={label ?? 'Create a Team'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} />}
      {size === 'medium' && <BodyIconTypography label={label ?? 'Create a Team'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} />}
      {(size === 'large' || !size) && <LeadIconTypography label={label ?? 'Create a Team'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} />}
    </Button>
  );
};

export default memo(NewTeamButton);

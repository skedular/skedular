import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@/components/commons';
import { NewIcon } from '@/components/icons';
import { getOrganizationTeamAddLink } from '@/components/links';
import { useIntegratedPlatrform } from '@/libs/providers';
import Button from '@mui/material/Button';
import { memo } from 'react';

type Props = {
  organizationUniqueAlphanumericName: string;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const NewTeamButton = ({ organizationUniqueAlphanumericName, fullWidth, label, hideIcon, variant, size }: Props) => {
  const { integratedPlatrform } = useIntegratedPlatrform();

  return (
    <Button href={getOrganizationTeamAddLink(integratedPlatrform, organizationUniqueAlphanumericName)} variant={variant ?? 'text'} fullWidth={fullWidth}>
      {size === 'small' && <SmallIconTypography label={label ?? 'Create a Team'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} />}
      {size === 'medium' && <BodyIconTypography label={label ?? 'Create a Team'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} />}
      {(size === 'large' || !size) && <LeadIconTypography label={label ?? 'Create a Team'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} />}
    </Button>
  );
};

export default memo(NewTeamButton);

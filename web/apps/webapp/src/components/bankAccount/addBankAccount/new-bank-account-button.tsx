import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@/components/commons';
import { NewIcon } from '@/components/icons';
import { getOrganizationBankAccountAddLink } from '@/components/links';
import { useIntegratedPlatrform } from '@/libs/providers';
import Button from '@mui/material/Button';
import { memo } from 'react';

type Props = {
  organizationId: string;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const NewBankAccountButton = ({ organizationId, fullWidth, label, hideIcon, variant, size }: Props) => {
  const { integratedPlatrform } = useIntegratedPlatrform();

  return (
    <Button href={getOrganizationBankAccountAddLink(integratedPlatrform, organizationId)} variant={variant ?? 'text'} fullWidth={fullWidth} sx={{ textTransform: 'none' }}>
      {size === 'small' && <SmallIconTypography label={label ?? 'Add Bank Account'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} />}
      {size === 'medium' && <BodyIconTypography label={label ?? 'Add Bank Account'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} />}
      {(size === 'large' || !size) && <LeadIconTypography label={label ?? 'Add Bank Account'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} />}
    </Button>
  );
};

export default memo(NewBankAccountButton);

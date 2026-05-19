import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@skedular/ui';
import { NewIcon } from '@/components/icons';
import { getOrganizationStripeConnectAccountAddLink } from '@/components/links';
import { useIntegratedPlatrform } from '@skedular/shared';
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

const NewStripeConnectAccountButton = ({ organizationCustomDomain, fullWidth, label, hideIcon, variant, size }: Props) => {
  const { integratedPlatrform } = useIntegratedPlatrform();

  return (
    <Button
      href={getOrganizationStripeConnectAccountAddLink(integratedPlatrform, organizationCustomDomain)}
      variant={variant ?? 'text'}
      fullWidth={fullWidth}
      sx={{ textTransform: 'none' }}
    >
      {size === 'small' && <SmallIconTypography label={label ?? 'Add New Stripe Connect Account'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} />}
      {size === 'medium' && <BodyIconTypography label={label ?? 'Add New Stripe Connect Account'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} />}
      {(size === 'large' || !size) && (
        <LeadIconTypography label={label ?? 'Add New Stripe Connect Account'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} />
      )}
    </Button>
  );
};

export default memo(NewStripeConnectAccountButton);

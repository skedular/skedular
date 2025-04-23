import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@/components/commons';
import Button from '@mui/material/Button';
import Link from '@mui/material/Link';
import type { SxProps, Theme } from '@mui/system';
import NextLink from 'next/link';
import { memo } from 'react';

type Props = {
  onboardingUrl: string;
  fullWidth?: boolean;
  label?: string;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
  sx?: SxProps<Theme>;
};

const CompleteOnboardStripeConnectAccountButton = ({ onboardingUrl, fullWidth, label, variant, size, sx }: Props) => {
  return (
    <Link component={NextLink} href={onboardingUrl}>
      <Button variant={variant ?? 'text'} fullWidth={fullWidth} sx={{ textTransform: 'none', ...sx }}>
        {size === 'small' && <SmallIconTypography label={label ?? 'Complete Onboarding'} />}
        {size === 'medium' && <BodyIconTypography label={label ?? 'Complete Onboarding'} />}
        {(size === 'large' || !size) && <LeadIconTypography label={label ?? 'Complete Onboarding'} />}
      </Button>
    </Link>
  );
};

export default memo(CompleteOnboardStripeConnectAccountButton);

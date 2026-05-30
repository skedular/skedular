import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@skedular/ui';
import { getOrganizationAdminAddZoneBaseLink } from '@/components/links';
import { NewIcon } from '@/components/icons';
import Button from '@mui/material/Button';
import { useIntegratedPlatform } from '@skedular/shared';
import NextLink from 'next/link';
import { usePathname, useSearchParams } from 'next/navigation';
import { memo, useMemo } from 'react';

type Props = {
  organizationCustomDomain: string;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const AddOrganizationZoneButton = ({ organizationCustomDomain, fullWidth, label, hideIcon, variant, size }: Props) => {
  const { integratedPlatform } = useIntegratedPlatform();
  const pathname = usePathname();
  const searchParams = useSearchParams();

  const href = useMemo(() => {
    const currentQuery = searchParams.toString();
    const redirectUrl = currentQuery ? `${pathname}?${currentQuery}` : pathname;
    return getOrganizationAdminAddZoneBaseLink(integratedPlatform, organizationCustomDomain, { redirectUrl });
  }, [integratedPlatform, organizationCustomDomain, pathname, searchParams]);

  return (
    <Button component={NextLink} href={href} variant={variant ?? 'text'} fullWidth={fullWidth} sx={{ textTransform: 'none' }}>
      {size === 'small' && <SmallIconTypography label={label ?? 'Add Zone'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} />}
      {size === 'medium' && <BodyIconTypography label={label ?? 'Add Zone'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} />}
      {(size === 'large' || !size) && <LeadIconTypography label={label ?? 'Add Zone'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} />}
    </Button>
  );
};

export default memo(AddOrganizationZoneButton);

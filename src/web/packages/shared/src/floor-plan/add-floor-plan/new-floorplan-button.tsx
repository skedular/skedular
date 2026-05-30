'use client';

import AddCircleIcon from '@mui/icons-material/AddCircle';
import Button from '@mui/material/Button';
import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@skedular/ui';
import { memo } from 'react';
import { useIntegratedPlatform } from '../../hooks/index';

type Props = {
  organizationCustomDomain: string;
  locationId: string;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const NewFloorplanButton = ({ organizationCustomDomain, locationId, fullWidth, label, hideIcon, variant, size }: Props) => {
  const { integratedPlatform } = useIntegratedPlatform();

  const href = integratedPlatform
    ? `${integratedPlatform}/organizations/${organizationCustomDomain}/locations/${locationId}/floorPlans/add`
    : `/organizations/${organizationCustomDomain}/locations/${locationId}/floorPlans/add`;

  const icon = hideIcon ? null : <AddCircleIcon fontSize={size ?? 'large'} />;

  return (
    <Button href={href} variant={variant ?? 'text'} fullWidth={fullWidth} sx={{ textTransform: 'none' }}>
      {size === 'small' && <SmallIconTypography label={label ?? 'Add Floor Plan'} endElement={hideIcon ? null : <AddCircleIcon fontSize="small" />} />}
      {size === 'medium' && <BodyIconTypography label={label ?? 'Add Floor Plan'} endElement={hideIcon ? null : <AddCircleIcon fontSize="medium" />} />}
      {(size === 'large' || !size) && <LeadIconTypography label={label ?? 'Add Floor Plan'} endElement={icon} />}
    </Button>
  );
};

export default memo(NewFloorplanButton);

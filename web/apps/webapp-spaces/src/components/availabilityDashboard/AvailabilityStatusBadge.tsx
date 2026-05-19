import Chip from '@mui/material/Chip';
import type { SxProps, Theme } from '@mui/system';
import { memo } from 'react';

type ResourceAvailabilityStatus = 'AVAILABLE' | 'UNAVAILABLE' | 'PARTIALLY_BOOKED' | 'FULLY_BOOKED' | 'OCCUPIED' | 'BLOCKED' | (string & {});

type Props = {
  status: ResourceAvailabilityStatus;
};

const statusConfig: Record<ResourceAvailabilityStatus, { label: string; color: string }> = {
  AVAILABLE: { label: 'Available', color: 'success' },
  UNAVAILABLE: { label: 'Unavailable', color: 'default' },
  PARTIALLY_BOOKED: { label: 'Partially Booked', color: 'warning' },
  FULLY_BOOKED: { label: 'Fully Booked', color: 'error' },
  OCCUPIED: { label: 'Occupied', color: 'error' },
  BLOCKED: { label: 'Blocked', color: 'default' },
};

const chipSx: SxProps<Theme> = { fontWeight: 600 };

const AvailabilityStatusBadge = ({ status }: Props) => {
  const config = statusConfig[status] ?? { label: status, color: 'default' };

  return (
    <Chip label={config.label} color={config.color as 'success' | 'warning' | 'error' | 'default'} size="small" sx={chipSx} aria-label={`Availability status: ${config.label}`} />
  );
};

export default memo(AvailabilityStatusBadge);

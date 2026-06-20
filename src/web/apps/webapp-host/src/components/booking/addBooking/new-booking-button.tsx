import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@skedular/ui';
import { NewIcon } from '@/components/icons';
import { getOrganizationBookingAddLink } from '@/components/links';
import { useIntegratedPlatform } from '@skedular/shared';
import { coal } from '@skedular/ui';
import Button from '@mui/material/Button';
import type { SxProps, Theme } from '@mui/system';
import { Dayjs } from 'dayjs';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { memo } from 'react';
import { useSpacesSubscription } from '@/components/rootShell/spaces-subscription-context';

type Props = {
  onReloadRequired?: () => void;
  connectionIds?: string[];
  organizationCustomDomain: string;
  defaultLocationId?: string;
  defaultDate?: Dayjs;
  defaultResourceIds?: string[];
  isInitiallyOpen?: boolean;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
  sx?: SxProps<Theme>;
  invertDefaultColor?: boolean;
  onOpenRequested?: () => void;
};

const NewBookingButton = ({
  organizationCustomDomain,
  defaultLocationId,
  defaultDate,
  defaultResourceIds,
  fullWidth,
  label,
  hideIcon,
  variant,
  size,
  sx,
  invertDefaultColor,
  onOpenRequested,
}: Props) => {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const { integratedPlatform } = useIntegratedPlatform();
  const subscription = useSpacesSubscription();
  const blocked = subscription?.canAcceptBookings === false;

  const handleButtonClicked = () => {
    onOpenRequested?.();

    const currentQuery = searchParams.toString();
    const redirectUrl = currentQuery ? `${pathname}?${currentQuery}` : pathname;

    router.push(
      getOrganizationBookingAddLink(integratedPlatform, organizationCustomDomain, {
        locationId: defaultLocationId,
        date: defaultDate?.toISOString(),
        resourceIds: defaultResourceIds,
        redirectUrl,
      }),
    );
  };

  const borderSx = variant === 'contained' ? { backgroundColor: 'white', borderColor: coal, borderWidth: 1, borderStyle: 'solid' } : {};
  const buttonSx: SxProps<Theme> = sx ? [borderSx, ...(Array.isArray(sx) ? sx : [sx])] : borderSx;

  return (
    <Button
      variant={variant ?? 'text'}
      onClick={handleButtonClicked}
      fullWidth={fullWidth}
      sx={buttonSx}
      disabled={blocked}
      title={blocked ? 'Upgrade to continue accepting bookings.' : undefined}
    >
      {size === 'small' && (
        <SmallIconTypography label={label ?? 'Add Booking'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} invertDefaultColor={invertDefaultColor} />
      )}
      {size === 'medium' && (
        <BodyIconTypography label={label ?? 'Add Booking'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} invertDefaultColor={invertDefaultColor} />
      )}
      {(size === 'large' || !size) && (
        <LeadIconTypography label={label ?? 'Add Booking'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} invertDefaultColor={invertDefaultColor} />
      )}
    </Button>
  );
};

export default memo(NewBookingButton);

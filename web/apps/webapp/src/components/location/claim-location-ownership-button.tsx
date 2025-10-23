import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@/components/commons';
import { ClaimOwnership } from '@/components/icons';
import { coal } from '@/libs/theme';
import Button from '@mui/material/Button';
import type { SxProps, Theme } from '@mui/system';
import { memo, useEffect, useState } from 'react';
import ClaimLocationOwnershipDialog from './claim-location-ownership-dialog';

type Props = {
  onReloadRequired?: () => void;
  connectionIds?: string[];
  organizationUniqueAlphanumericName: string;
  isInitiallyOpen?: boolean;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
  sx?: SxProps<Theme>;
  invertDefaultColor?: boolean;
  onClaimClicked?: () => void;
};

const ClaimLocationOwnershipButton = ({
  onReloadRequired,
  connectionIds,
  organizationUniqueAlphanumericName,
  isInitiallyOpen = false,
  fullWidth,
  label,
  hideIcon,
  variant,
  size,
  sx,
  invertDefaultColor,
  onClaimClicked,
}: Props) => {
  const [isDialogOpen, setIsDialogOpen] = useState(isInitiallyOpen);

  useEffect(() => {
    if (!isInitiallyOpen) {
      return;
    }

    queueMicrotask(() => {
      setIsDialogOpen(true);
    });
  }, [isInitiallyOpen]);

  const handleButtonClicked = () => {
    setIsDialogOpen(true);
  };

  const handleClaimClicked = () => {
    setIsDialogOpen(false);

    if (onClaimClicked) {
      onClaimClicked();
    }

    if (onReloadRequired) {
      onReloadRequired();
    }
  };

  const handleCancelClicked = () => {
    setIsDialogOpen(false);
  };

  const borderSx = variant === 'contained' ? { backgroundColor: 'white', borderColor: coal, borderWidth: 1, borderStyle: 'solid' } : {};

  return (
    <>
      <Button variant={variant ?? 'text'} onClick={handleButtonClicked} fullWidth={fullWidth} sx={{ ...sx, ...borderSx }}>
        {size === 'small' && (
          <SmallIconTypography
            label={label ?? 'Claim Location'}
            endElement={hideIcon ? null : <ClaimOwnership fontSize={size ?? 'small'} />}
            invertDefaultColor={invertDefaultColor}
          />
        )}
        {size === 'medium' && (
          <BodyIconTypography
            label={label ?? 'Claim Location'}
            endElement={hideIcon ? null : <ClaimOwnership fontSize={size ?? 'medium'} />}
            invertDefaultColor={invertDefaultColor}
          />
        )}
        {(size === 'large' || !size) && (
          <LeadIconTypography
            label={label ?? 'Claim Location'}
            endElement={hideIcon ? null : <ClaimOwnership fontSize={size ?? 'large'} />}
            invertDefaultColor={invertDefaultColor}
          />
        )}
      </Button>
      <ClaimLocationOwnershipDialog
        connectionIds={connectionIds ?? []}
        isDialogOpen={isDialogOpen}
        onClaimClicked={handleClaimClicked}
        onCancel={handleCancelClicked}
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
      />
    </>
  );
};

export default memo(ClaimLocationOwnershipButton);

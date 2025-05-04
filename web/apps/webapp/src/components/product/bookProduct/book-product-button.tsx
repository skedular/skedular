import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@/components/commons';
import { NewIcon } from '@/components/icons';
import { getOrganizationBookingProductLink } from '@/components/links';
import { useIntegratedPlatrform } from '@/libs/providers';
import { coal } from '@/libs/theme';
import Button from '@mui/material/Button';
import Link from '@mui/material/Link';
import type { SxProps, Theme } from '@mui/system';
import { memo } from 'react';

type Props = {
  organizationId: string;
  productId: string;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
  sx?: SxProps<Theme>;
  invertDefaultColor?: boolean;
};

const BookProductButton = ({ organizationId, productId, fullWidth, label, hideIcon, variant, size, sx, invertDefaultColor }: Props) => {
  const { integratedPlatrform } = useIntegratedPlatrform();
  const borderSx = variant === 'contained' ? { backgroundColor: 'white', borderColor: coal, borderWidth: 1, borderStyle: 'solid' } : {};

  return (
    <>
      <Button
        variant={variant ?? 'text'}
        LinkComponent={Link}
        href={getOrganizationBookingProductLink(integratedPlatrform, organizationId, productId)}
        fullWidth={fullWidth}
        sx={{ ...sx, ...borderSx }}
      >
        {size === 'small' && (
          <SmallIconTypography label={label ?? 'Book'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} invertDefaultColor={invertDefaultColor} />
        )}
        {size === 'medium' && (
          <BodyIconTypography label={label ?? 'Book'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} invertDefaultColor={invertDefaultColor} />
        )}
        {(size === 'large' || !size) && (
          <LeadIconTypography label={label ?? 'Book'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} invertDefaultColor={invertDefaultColor} />
        )}
      </Button>
    </>
  );
};

export default memo(BookProductButton);

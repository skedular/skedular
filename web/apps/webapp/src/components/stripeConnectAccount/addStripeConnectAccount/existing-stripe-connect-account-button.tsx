import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@/components/commons';
import { NewIcon } from '@/components/icons';
import type { existingStripeConnectAccountButton_query$key } from '@/queries/__generated__/existingStripeConnectAccountButton_query.graphql';
import Button from '@mui/material/Button';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: existingStripeConnectAccountButton_query$key;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const ExistingStripeConnectAccountButton = ({ rootDataRelay, fullWidth, label, hideIcon, variant, size }: Props) => {
  const rootData = useFragment<existingStripeConnectAccountButton_query$key>(
    graphql`
      fragment existingStripeConnectAccountButton_query on Query {
        organization(customDomain: $organizationCustomDomain) {
          stripeAuthorizeExistingConnectAccountUrl
        }
      }
    `,
    rootDataRelay,
  );

  if (!rootData.organization) {
    return null;
  }

  return (
    <Button href={rootData.organization.stripeAuthorizeExistingConnectAccountUrl} variant={variant ?? 'text'} fullWidth={fullWidth} sx={{ textTransform: 'none' }}>
      {size === 'small' && <SmallIconTypography label={label ?? 'Add Existing Stripe Connect Account'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} />}
      {size === 'medium' && <BodyIconTypography label={label ?? 'Add Existing Stripe Connect Account'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} />}
      {(size === 'large' || !size) && (
        <LeadIconTypography label={label ?? 'Add Existing Stripe Connect Account'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} />
      )}
    </Button>
  );
};

export default memo(ExistingStripeConnectAccountButton);

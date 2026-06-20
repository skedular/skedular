import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@skedular/ui';
import { NewIcon } from '@/components/icons';
import { getOrganizationLocationAddMarketplaceLink, getOrganizationLocationAddPrivateLink } from '@/components/links';
import { useIntegratedPlatform } from '@skedular/shared';
import type { newLocationButton_query$key } from '@/queries/__generated__/newLocationButton_query.graphql';
import Button from '@mui/material/Button';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';
import { useSpacesSubscription } from '@/components/rootShell/spaces-subscription-context';

type Props = {
  rootDataRelay: newLocationButton_query$key;
  organizationCustomDomain: string;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const NewLocationButton = ({ rootDataRelay, organizationCustomDomain, fullWidth, label, hideIcon, variant, size }: Props) => {
  const rootData = useFragment<newLocationButton_query$key>(
    graphql`
      fragment newLocationButton_query on Query {
        organization(customDomain: $organizationCustomDomain) {
          type {
            type
          }
        }
      }
    `,
    rootDataRelay,
  );

  const { integratedPlatform } = useIntegratedPlatform();
  const subscription = useSpacesSubscription();
  const blocked = subscription?.canUseProduct === false;

  if (!rootData.organization) {
    return null;
  }

  return (
    <Button
      href={
        rootData.organization.type.type === 'PRIVATE'
          ? getOrganizationLocationAddPrivateLink(integratedPlatform, organizationCustomDomain)
          : getOrganizationLocationAddMarketplaceLink(integratedPlatform, organizationCustomDomain)
      }
      variant={variant ?? 'text'}
      fullWidth={fullWidth}
      disabled={blocked}
      title={blocked ? 'Upgrade to continue adding locations.' : undefined}
    >
      {size === 'small' && <SmallIconTypography label={label ?? 'Add Location'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} />}
      {size === 'medium' && <BodyIconTypography label={label ?? 'Add Location'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} />}
      {(size === 'large' || !size) && <LeadIconTypography label={label ?? 'Add Location'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} />}
    </Button>
  );
};

export default memo(NewLocationButton);

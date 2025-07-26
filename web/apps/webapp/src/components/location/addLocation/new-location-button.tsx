import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@/components/commons';
import { NewIcon } from '@/components/icons';
import { getOrganizationLocationAddMarketplaceLink, getOrganizationLocationAddPrivateLink } from '@/components/links';
import { useIntegratedPlatrform } from '@/libs/providers';
import type { newLocationButton_query$key } from '@/queries/__generated__/newLocationButton_query.graphql';
import Button from '@mui/material/Button';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: newLocationButton_query$key;
  organizationId: string;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const NewLocationButton = ({ rootDataRelay, organizationId, fullWidth, label, hideIcon, variant, size }: Props) => {
  const rootData = useFragment<newLocationButton_query$key>(
    graphql`
      fragment newLocationButton_query on Query {
        organization(id: $organizationId) {
          type {
            type
          }
        }
      }
    `,
    rootDataRelay,
  );

  const { integratedPlatrform } = useIntegratedPlatrform();

  if (!rootData.organization) {
    return <></>;
  }

  return (
    <Button
      href={
        rootData.organization.type.type === 'PRIVATE'
          ? getOrganizationLocationAddPrivateLink(integratedPlatrform, organizationId)
          : getOrganizationLocationAddMarketplaceLink(integratedPlatrform, organizationId)
      }
      variant={variant ?? 'text'}
      fullWidth={fullWidth}
    >
      {size === 'small' && <SmallIconTypography label={label ?? 'Add Location'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} />}
      {size === 'medium' && <BodyIconTypography label={label ?? 'Add Location'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} />}
      {(size === 'large' || !size) && <LeadIconTypography label={label ?? 'Add Location'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} />}
    </Button>
  );
};

export default memo(NewLocationButton);

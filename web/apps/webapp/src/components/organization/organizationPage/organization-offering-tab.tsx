import type { organizationOfferingTab_query$key } from '@/queries/__generated__/organizationOfferingTab_query.graphql';
import Stack from '@mui/material/Stack';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';
import OrganizationAvailableOfferings from './organization-available-offerings';
import OrganizationOffering from './organization-offering';

type Props = {
  rootDataRelay: organizationOfferingTab_query$key;
  onReloadRequired: () => void;
};

const OrganizationOfferingTab = ({ rootDataRelay, onReloadRequired }: Props) => {
  const rootData = useFragment<organizationOfferingTab_query$key>(
    graphql`
      fragment organizationOfferingTab_query on Query {
        ...organizationOffering_query
        ...organizationAvailableOfferings_query
        organization(id: $organizationId) {
          id
          availableOfferings {
            code
          }
        }
      }
    `,
    rootDataRelay,
  );

  return (
    <Stack direction="column" spacing={1}>
      <OrganizationOffering rootDataRelay={rootData} onReloadRequired={onReloadRequired} />
      {rootData.organization?.availableOfferings && rootData.organization?.availableOfferings.length > 0 && (
        <OrganizationAvailableOfferings rootDataRelay={rootData} onReloadRequired={onReloadRequired} />
      )}
    </Stack>
  );
};

export default memo(OrganizationOfferingTab);

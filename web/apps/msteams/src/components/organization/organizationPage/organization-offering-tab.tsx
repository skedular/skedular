import Stack from '@mui/material/Stack';
import graphql from 'babel-plugin-relay/macro';
import { memo } from 'react';
import { useFragment } from 'react-relay';
import type { organizationOfferingTab_query$key } from './__generated__/organizationOfferingTab_query.graphql';
import OrganizationAvailableOfferings from './organization-available-offerings';
import OrganizationOffering from './organization-offering';

type Props = {
  rootDataRelay: organizationOfferingTab_query$key;
  onRefetchRequired: () => void;
};

const OrganizationOfferingTab = ({ rootDataRelay, onRefetchRequired }: Props) => {
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
      <OrganizationOffering rootDataRelay={rootData} onRefetchRequired={onRefetchRequired} />
      {rootData.organization?.availableOfferings && rootData.organization?.availableOfferings.length > 0 && (
        <OrganizationAvailableOfferings rootDataRelay={rootData} onRefetchRequired={onRefetchRequired} />
      )}
    </Stack>
  );
};

export default memo(OrganizationOfferingTab);

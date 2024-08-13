import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import Stack from '@mui/material/Stack';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';
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
    <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
      <Stack direction="column">
        <Container sx={{ marginBottom: '5px' }}>
          <OrganizationOffering rootDataRelay={rootData} onRefetchRequired={onRefetchRequired} />
        </Container>
        {rootData.organization?.availableOfferings && rootData.organization?.availableOfferings.length > 0 && (
          <Container sx={{ marginTop: '5px', marginBottom: '5px' }}>
            <OrganizationAvailableOfferings rootDataRelay={rootData} onRefetchRequired={onRefetchRequired} />
          </Container>
        )}
      </Stack>
    </Box>
  );
};

export default memo(OrganizationOfferingTab);

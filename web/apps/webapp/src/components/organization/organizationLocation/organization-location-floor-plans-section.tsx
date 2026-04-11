import { FloorPlanCard } from '@/components/floorPlan';
import { NewFloorplanButton } from '@/components/floorPlan/addFloorPlan';
import { SettingsSectionCard } from '@skedular/ui';
import { defaultPadding } from '@/libs/theme';
import type { organizationLocationFloorPlansSectionQuery } from '@/queries/__generated__/organizationLocationFloorPlansSectionQuery.graphql';
import Box from '@mui/material/Box';
import { memo, useMemo } from 'react';
import { graphql, useLazyLoadQuery } from 'react-relay';

type Props = {
  organizationCustomDomain: string;
  locationId: string;
};

const FloorPlansSectionQuery = graphql`
  query organizationLocationFloorPlansSectionQuery($floorPlansSortingValues: [FloorPlanOrderInput!], $locationId: String!) {
    floorPlans(where: { locationId: $locationId }, orderBy: $floorPlansSortingValues) {
      __id
      edges {
        node {
          id
          name
          ...floorPlanCard_FloorPlanDetails
        }
      }
    }
  }
`;

const OrganizationLocationFloorPlansSection = ({ organizationCustomDomain, locationId }: Props) => {
  const rootData = useLazyLoadQuery<organizationLocationFloorPlansSectionQuery>(
    FloorPlansSectionQuery,
    {
      locationId,
      floorPlansSortingValues: [
        {
          direction: 'ASCENDING',
          field: 'NAME',
        },
      ],
    },
    {
      fetchPolicy: 'store-and-network',
    },
  );

  const floorPlans = useMemo(() => rootData.floorPlans.edges.map((edge) => edge.node), [rootData.floorPlans.edges]);
  const floorPlansConnectionIds = useMemo(() => [rootData.floorPlans.__id], [rootData.floorPlans.__id]);

  return (
    <Box sx={{ pb: defaultPadding }}>
      <SettingsSectionCard
        title="Manage Floor Plans"
        description="Create and maintain the floor plans that describe how this location is laid out."
        actions={<NewFloorplanButton organizationCustomDomain={organizationCustomDomain} locationId={locationId} />}
      >
        <Box
          sx={{
            display: 'grid',
            gridTemplateColumns: { xs: '1fr', sm: 'repeat(auto-fit, minmax(280px, 360px))' },
            gap: 2,
            justifyContent: 'start',
          }}
        >
          {floorPlans.map((floorPlan) => (
            <Box key={floorPlan.id}>
              <FloorPlanCard
                floorPlanDetailsRelay={floorPlan}
                connectionIds={floorPlansConnectionIds}
                organizationCustomDomain={organizationCustomDomain}
                locationId={locationId}
              />
            </Box>
          ))}
        </Box>
      </SettingsSectionCard>
    </Box>
  );
};

export default memo(OrganizationLocationFloorPlansSection);

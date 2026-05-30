import { FloorPlanCard } from '@/components/floorPlan';
import { NewFloorplanButton } from '@/components/floorPlan/addFloorPlan';
import type { organizationLocationFloorPlansSectionQuery } from '@/queries/__generated__/organizationLocationFloorPlansSectionQuery.graphql';
import Box from '@mui/material/Box';
import { defaultPadding, LeadIconTypography, SettingsSectionCard, SmallIconTypography, StackColumn } from '@skedular/ui';
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
        {floorPlans.length === 0 ? (
          <Box sx={{ border: 1, borderStyle: 'dashed', borderColor: 'divider', borderRadius: 3, p: 3, backgroundColor: 'background.paper' }}>
            <LeadIconTypography label="No floor plans found" />
            <SmallIconTypography label="Add a floor plan to describe how this location is laid out." />
          </Box>
        ) : (
          <StackColumn spacing={1}>
            {floorPlans.map((floorPlan) => (
              <FloorPlanCard
                key={floorPlan.id}
                floorPlanDetailsRelay={floorPlan}
                connectionIds={floorPlansConnectionIds}
                organizationCustomDomain={organizationCustomDomain}
                locationId={locationId}
              />
            ))}
          </StackColumn>
        )}
      </SettingsSectionCard>
    </Box>
  );
};

export default memo(OrganizationLocationFloorPlansSection);

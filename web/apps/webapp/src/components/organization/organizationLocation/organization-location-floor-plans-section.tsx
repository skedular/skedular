import { BodyIconTypography, GridContainer, SectionIconTypography, StackColumn } from '@/components/commons';
import { FloorPlanCard } from '@/components/floorPlan';
import { NewFloorplanButton } from '@/components/floorPlan/addFloorPlan';
import { defaultPadding } from '@/libs/theme';
import type { organizationLocationFloorPlansSectionQuery } from '@/queries/__generated__/organizationLocationFloorPlansSectionQuery.graphql';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
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
    <StackColumn sx={{ padding: defaultPadding }}>
      <GridContainer sx={{ justifyContent: 'space-between' }}>
        <Grid>
          <SectionIconTypography label="Manage Floor Plans" />
          <BodyIconTypography label="Manage your location floor plans details" />
        </Grid>

        <Grid>
          <NewFloorplanButton organizationCustomDomain={organizationCustomDomain} locationId={locationId} />
        </Grid>
      </GridContainer>
      <Divider />

      <GridContainer>
        {floorPlans.map((floorPlan) => (
          <Grid key={floorPlan.id}>
            <FloorPlanCard floorPlanDetailsRelay={floorPlan} connectionIds={floorPlansConnectionIds} organizationCustomDomain={organizationCustomDomain} locationId={locationId} />
          </Grid>
        ))}
      </GridContainer>
    </StackColumn>
  );
};

export default memo(OrganizationLocationFloorPlansSection);

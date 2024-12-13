import Grid from '@mui/material/Grid2';
import { GridContainer } from '@repo/shared/components/commons';
import { OrganizationBookingInsightRoot } from 'components/organization/organizationBookingInsight';
import { OrganizationMemberAttendancyInsightRoot } from 'components/organization/organizationMemberAttendancyInsight';
import { memo } from 'react';

type Props = {
  onReloadRequired: () => void;
  organizationId: string;
  organizationName?: string;
};

const OrganizationAnalyticsTab = ({ onReloadRequired, organizationId, organizationName }: Props) => (
  <GridContainer spacing={1}>
    <Grid>
      <OrganizationBookingInsightRoot
        onReloadRequired={onReloadRequired}
        organizationId={organizationId}
        organizationName={organizationName}
        hideOrganizationDetails
      />
    </Grid>
    <Grid>
      <OrganizationMemberAttendancyInsightRoot
        onReloadRequired={onReloadRequired}
        organizationId={organizationId}
        organizationName={organizationName}
        hideOrganizationDetails
      />
    </Grid>
  </GridContainer>
);

export default memo(OrganizationAnalyticsTab);

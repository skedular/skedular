import Grid from '@mui/material/Grid2';
import { OrganizationBookingInsightRoot } from 'components/organization/organizationBookingInsight';
import { OrganizationMemberAttendancyInsightRoot } from 'components/organization/organizationMemberAttendancyInsight';
import { memo } from 'react';

type Props = {
  organizationId: string;
};

const OrganizationAnalyticsTab = ({ organizationId }: Props) => (
  <Grid container spacing={1}>
    <Grid>
      <OrganizationBookingInsightRoot organizationId={organizationId} organizationName="" hideOrganizationDetails />
    </Grid>
    <Grid>
      <OrganizationMemberAttendancyInsightRoot organizationId={organizationId} organizationName="" hideOrganizationDetails />
    </Grid>
  </Grid>
);

export default memo(OrganizationAnalyticsTab);

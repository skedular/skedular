import { OrganizationBookingInsightRoot } from '@/components/organization/organizationBookingInsight';
import { OrganizationMemberAttendancyInsightRoot } from '@/components/organization/organizationMemberAttendancyInsight';
import Grid from '@mui/material/Grid2';
import { memo } from 'react';

type Props = {
  onReloadRequired: () => void;
  organizationId: string;
  organizationName?: string;
};

const OrganizationAnalyticsTab = ({ onReloadRequired, organizationId, organizationName }: Props) => (
  <Grid container spacing={1}>
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
  </Grid>
);

export default memo(OrganizationAnalyticsTab);

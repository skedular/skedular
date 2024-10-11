import { LocationBookingInsightRoot } from '@/components/location/locationBookingInsight';
import { LocationDeskOccupancyInsightRoot } from '@/components/location/locationDeskOccupancyInsight';
import Grid from '@mui/material/Grid2';
import { memo } from 'react';

type Props = {
  onReloadRequired: () => void;
  organizationId?: string;
  locationId: string;
  locationName?: string;
};

const LocationAnalyticsTab = ({ onReloadRequired, organizationId, locationId, locationName }: Props) => (
  <Grid container spacing={1}>
    <Grid>
      <LocationBookingInsightRoot onReloadRequired={onReloadRequired} organizationId={organizationId} locationId={locationId} hideLocationDetails />
    </Grid>
    <Grid>
      <LocationDeskOccupancyInsightRoot
        onReloadRequired={onReloadRequired}
        organizationId={organizationId}
        locationId={locationId}
        locationName={locationName}
        hideLocationDetails
      />
    </Grid>
  </Grid>
);

export default memo(LocationAnalyticsTab);

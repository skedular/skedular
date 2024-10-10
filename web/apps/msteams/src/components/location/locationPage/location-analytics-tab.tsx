import Grid from '@mui/material/Grid2';
import { LocationBookingInsightRoot } from 'components/location/locationBookingInsight';
import { LocationDeskOccupancyInsightRoot } from 'components/location/locationDeskOccupancyInsight';
import { memo } from 'react';

type Props = {
  organizationId: string;
  locationId: string;
};

const LocationAnalyticsTab = ({ organizationId, locationId }: Props) => (
  <Grid container spacing={1}>
    <Grid>
      <LocationBookingInsightRoot organizationId={organizationId} locationId={locationId} locationName="" hideLocationDetails />
    </Grid>
    <Grid>
      <LocationDeskOccupancyInsightRoot organizationId={organizationId} locationId={locationId} locationName="" hideLocationDetails />
    </Grid>
  </Grid>
);

export default memo(LocationAnalyticsTab);

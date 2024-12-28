import Grid from '@mui/material/Grid2';
import { GridContainer } from '@repo/shared/components/commons';
import { LocationBookingInsightRoot } from 'components/location/locationBookingInsight';
import { LocationDeskOccupancyInsightRoot } from 'components/location/locationDeskOccupancyInsight';
import { memo } from 'react';

type Props = {
  onReloadRequired: () => void;
  locationId: string;
};

const LocationAnalyticsTab = ({ onReloadRequired, locationId }: Props) => (
  <GridContainer>
    <Grid>
      <LocationBookingInsightRoot onReloadRequired={onReloadRequired} locationId={locationId} hideLocationDetails />
    </Grid>
    <Grid>
      <LocationDeskOccupancyInsightRoot onReloadRequired={onReloadRequired} locationId={locationId} hideLocationDetails />
    </Grid>
  </GridContainer>
);

export default memo(LocationAnalyticsTab);

import Grid from '@mui/material/Grid2';
import { GridContainer } from '@repo/shared/components/commons';
import { LocationBookingInsightRoot } from 'components/location/locationBookingInsight';
import { LocationDeskOccupancyInsightRoot } from 'components/location/locationDeskOccupancyInsight';
import { memo } from 'react';

type Props = {
  onReloadRequired: () => void;
  organizationId: string;
  locationId: string;
  locationName?: string;
};

const LocationAnalyticsTab = ({ onReloadRequired, organizationId, locationId, locationName }: Props) => (
  <GridContainer>
    <Grid>
      <LocationBookingInsightRoot
        onReloadRequired={onReloadRequired}
        organizationId={organizationId}
        locationId={locationId}
        locationName={locationName}
        hideLocationDetails
      />
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
  </GridContainer>
);

export default memo(LocationAnalyticsTab);

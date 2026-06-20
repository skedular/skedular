'use client';

import Typography from '@/components/commons/Typography';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import { SectionIconTypography } from '@skedular/ui';
import { TextField } from 'mui-rff';

export type LocationFormData = {
  name: string | null;
  timezone: string | null;
  type: string | null;
  openingHours: {
    days: Array<{
      day: string;
      open: boolean;
      from: string;
      to: string;
    }>;
  } | null;
  physicalAddress?: {
    multilinesFormattedAddress: string | null;
    city: string | null;
    country: { code: string; name: string } | null;
  };
};

export type LocationSectionProps = {
  disabled?: boolean;
};

const LocationSection = ({ disabled = false }: LocationSectionProps) => (
  <Box sx={{ mb: 4 }}>
    <SectionIconTypography label="Location Information" />
    <Stack spacing={3}>
      {/* Name */}
      <TextField name="name" label="Location Name" placeholder="Enter location name (e.g., 'My Downtown Apartment')" disabled={disabled} required fullWidth />

      {/* Type */}
      <TextField name="type" label="Property Type" placeholder="e.g., Office" disabled={disabled} fullWidth />

      {/* Timezone */}
      <TextField name="timezone" label="Timezone" placeholder="e.g., America/New_York" disabled={disabled} helperText="Used for scheduling and availability display" fullWidth />

      {/* Physical Address - Basic fields shown first */}
      <Box sx={{ mt: 2 }}>
        <Typography variant="h6" component="h3" sx={{ mb: 1 }}>
          Address
        </Typography>
        <TextField
          name="physicalAddress.multilinesFormattedAddress"
          label="Full Address"
          placeholder="Enter the complete address"
          disabled={disabled}
          multiline
          rows={2}
          fullWidth
        />
      </Box>
    </Stack>
  </Box>
);

export default LocationSection;

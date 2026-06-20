'use client';

import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import { SectionIconTypography } from '@skedular/ui';
import { TextField } from 'mui-rff';

export type ListingFormData = {
  title: string | null;
  subTitle: string | null;
  includedFeatures: string | null;
};

export type ListingSectionProps = {
  disabled?: boolean;
};

const ListingSection = ({ disabled = false }: ListingSectionProps) => (
  <Box sx={{ mb: 4 }}>
    <SectionIconTypography label="Listing Information" />
    <Stack spacing={3}>
      {/* Title */}
      <TextField
        name="title"
        label="Listing Title"
        placeholder="e.g., Cozy Downtown Studio"
        disabled={disabled}
        required
        fullWidth
        helperText="This is the public-facing name for your listing"
      />

      {/* Subtitle */}
      <TextField name="subTitle" label="Subtitle" placeholder="Short description that appears below the title" disabled={disabled} multiline rows={2} fullWidth />

      {/* Included Features */}
      <TextField
        name="includedFeatures"
        label="Included Features"
        placeholder="List key features (e.g., 'Free WiFi, Smart TV, Fully Equipped Kitchen')"
        disabled={disabled}
        multiline
        rows={3}
        fullWidth
      />
    </Stack>
  </Box>
);

export default ListingSection;

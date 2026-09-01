'use client';

import Typography from '@/components/commons/Typography';
import { DurationField } from '@/components/product/duration-input';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import { SectionIconTypography } from '@skedular/ui';
import { TextField } from 'mui-rff';

export type PricingFormData = {
  currency: string | null;
  pricingOptions: Array<{
    id: string;
    title: string | null;
    cadence: string | null;
    price: string | null;
    billingMode: string | null;
    cancellationPolicyType: string | null;
    isTaxInclusive: boolean;
    minDurationMinutes: string | null;
    minDurationDisplayUnit?: string | null;
    maxDurationMinutes: string | null;
    maxDurationDisplayUnit?: string | null;
  }>;
};

export type PricingSectionProps = {
  disabled?: boolean;
};

const PricingSection = ({ disabled = false }: PricingSectionProps) => (
  <Box sx={{ mb: 4 }}>
    <SectionIconTypography label="Pricing & Availability" />
    <Stack spacing={3}>
      {/* Currency */}
      <TextField name="currency" label="Currency" placeholder="e.g., USD" disabled={disabled} fullWidth />

      {/* Pricing Options */}
      <Stack spacing={2}>
        <Typography variant="h6" component="h3">
          Pricing Model
        </Typography>

        {/* Billing Mode Toggle */}
        <Stack direction="row" spacing={2}>
          <Box sx={{ flex: 1 }}>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
              Per-booking (Standard)
            </Typography>
            <TextField name="pricingOptions.0.price" label="Price per booking" placeholder="e.g., 150" disabled={disabled} fullWidth />
          </Box>
          <Box sx={{ flex: 1 }}>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
              Purchase Term
            </Typography>
            <TextField name="pricingOptions.0.cadence" label="Cadence" placeholder="e.g., DAILY" disabled={disabled} fullWidth />
          </Box>
        </Stack>

        {/* Cancellation Policy */}
        <Stack spacing={2}>
          <Typography variant="body2" color="text.secondary">
            Cancellation Policy
          </Typography>
          <TextField name="pricingOptions.0.cancellationPolicyType" label="Cancellation policy" placeholder="e.g., NO_CANCELLATION" disabled={disabled} fullWidth />
        </Stack>

        {/* Duration Settings */}
        <Stack direction="row" spacing={2}>
          <DurationField name="pricingOptions.0.minDurationMinutes" unitName="pricingOptions.0.minDurationDisplayUnit" label="Minimum booking duration" required />
          <DurationField name="pricingOptions.0.maxDurationMinutes" unitName="pricingOptions.0.maxDurationDisplayUnit" label="Maximum booking duration" required />
        </Stack>
      </Stack>
    </Stack>
  </Box>
);

export default PricingSection;

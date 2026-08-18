import MenuItem from '@mui/material/MenuItem';
import TextField from '@mui/material/TextField';
import { memo } from 'react';

export type OperatorEntitlementOption = {
  id: string;
  pricingId: string;
  availableQuantity: number;
  expiresAt: string;
};

export type OperatorEntitlementSelectorProps = {
  options: readonly OperatorEntitlementOption[];
  value?: string | null;
  onChange: (entitlementId: string | null) => void;
};

const OperatorEntitlementSelector = ({ options, value, onChange }: OperatorEntitlementSelectorProps) => (
  <TextField
    select
    fullWidth
    label="Booking credits"
    value={value ?? ''}
    onChange={(event) => onChange(event.target.value || null)}
    helperText="Select a customer's eligible entitlement, or leave blank to use normal payment."
  >
    <MenuItem value="">Normal payment</MenuItem>
    {options.map((option) => (
      <MenuItem key={option.id} value={option.id}>
        {option.availableQuantity} credits · expires {new Date(option.expiresAt).toLocaleDateString()}
      </MenuItem>
    ))}
  </TextField>
);

export default memo(OperatorEntitlementSelector);

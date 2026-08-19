'use client';

import AccessTimeRoundedIcon from '@mui/icons-material/AccessTimeRounded';
import InputAdornment from '@mui/material/InputAdornment';
import Switch from '@mui/material/Switch';
import TextField from '@mui/material/TextField';
import { useState } from 'react';
import BodyIconTypography from './typography/body-icon-typography';
import SmallIconTypography from './typography/small-icon-typography';
import StackColumn from './stack-column';

export type DurationInputProps = {
  label: string;
  value: string | null | undefined;
  onChange: (value: string) => void;
  disabled?: boolean;
  required?: boolean;
};

type DurationUnit = 'minutes' | 'hours';

const formatHours = (value: string) => {
  const minutes = Number(value);
  if (!value.trim() || !Number.isFinite(minutes)) return '';
  return Number.isInteger(minutes / 60)
    ? String(minutes / 60)
    : (minutes / 60)
        .toFixed(2)
        .replace(/\.0+$/, '')
        .replace(/(\.\d*[1-9])0+$/, '$1');
};

const DurationInput = ({ label, value = '', onChange, disabled, required }: DurationInputProps) => {
  const [unit, setUnit] = useState<DurationUnit>('hours');
  const safeValue = value ?? '';
  const displayValue = unit === 'hours' ? formatHours(safeValue) : safeValue;
  const minutes = Number(safeValue);
  const helperText = safeValue && Number.isFinite(minutes) ? `Saved as ${minutes} minutes` : 'Choose minutes for precise increments, or hours for a quicker entry.';

  return (
    <StackColumn spacing={1}>
      <BodyIconTypography label={required ? `${label} *` : label} />
      <TextField
        value={displayValue}
        type="number"
        fullWidth
        disabled={disabled}
        slotProps={{
          htmlInput: { min: 0, step: unit === 'hours' ? 0.25 : 1, inputMode: 'decimal' },
          input: {
            startAdornment: (
              <InputAdornment position="start">
                <AccessTimeRoundedIcon fontSize="small" />
              </InputAdornment>
            ),
            endAdornment: (
              <InputAdornment position="end">
                <SmallIconTypography label="Minutes" />
                <Switch
                  checked={unit === 'hours'}
                  onChange={(_, checked) => setUnit(checked ? 'hours' : 'minutes')}
                  disabled={disabled}
                  slotProps={{ input: { 'aria-label': `${label} unit` } }}
                  size="small"
                />
                <SmallIconTypography label="Hours" />
              </InputAdornment>
            ),
          },
        }}
        onChange={(event) => {
          const next = event.target.value;
          onChange(!next ? '' : unit === 'hours' ? String(Number(next) * 60) : next);
        }}
      />
      <SmallIconTypography label={helperText} />
    </StackColumn>
  );
};

export default DurationInput;

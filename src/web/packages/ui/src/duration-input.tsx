'use client';

import AccessTimeRoundedIcon from '@mui/icons-material/AccessTimeRounded';
import InputAdornment from '@mui/material/InputAdornment';
import Switch from '@mui/material/Switch';
import TextField from '@mui/material/TextField';
import { useState } from 'react';
import FieldHelp from './commons/field-help';
import BodyIconTypography from './typography/body-icon-typography';
import SmallIconTypography from './typography/small-icon-typography';
import StackColumn from './stack-column';

export type DurationInputProps = {
  label: string;
  value: string | null | undefined;
  onChange: (value: string) => void;
  initialUnit?: DurationUnit;
  unit?: DurationUnit;
  onUnitChange?: (unit: DurationUnit) => void;
  disabled?: boolean;
  required?: boolean;
  help?: string;
  hideLabel?: boolean;
};

export type DurationUnit = 'minutes' | 'hours';

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

const DurationInput = ({
  label,
  value = '',
  onChange,
  initialUnit = 'hours',
  unit: controlledUnit,
  onUnitChange,
  disabled,
  required,
  help,
  hideLabel = false,
}: DurationInputProps) => {
  const [localUnit, setLocalUnit] = useState<DurationUnit>(initialUnit);
  const unit = controlledUnit ?? localUnit;
  const safeValue = value ?? '';
  const displayValue = unit === 'hours' ? formatHours(safeValue) : safeValue;
  const minutes = Number(safeValue);
  const helperText = safeValue && Number.isFinite(minutes) ? `Saved as ${minutes} minutes` : 'Choose minutes for precise increments, or hours for a quicker entry.';

  return (
    <StackColumn spacing={1}>
      {!hideLabel ? (
        <StackColumn spacing={0.5} sx={{ flexDirection: 'row', alignItems: 'center' }}>
          <BodyIconTypography label={required ? `${label} *` : label} />
          {help ? <FieldHelp label={label}>{help}</FieldHelp> : null}
        </StackColumn>
      ) : null}
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
                  onChange={(_, checked) => {
                    const nextUnit = checked ? 'hours' : 'minutes';
                    setLocalUnit(nextUnit);
                    onUnitChange?.(nextUnit);
                  }}
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

'use client';

import Alert from '@mui/material/Alert';
import Autocomplete from '@mui/material/Autocomplete';
import Box from '@mui/material/Box';
import FormControlLabel from '@mui/material/FormControlLabel';
import Switch from '@mui/material/Switch';
import TextField from '@mui/material/TextField';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { TimeRangePicker } from '@mui/x-date-pickers-pro/TimeRangePicker';
import type { DateRange } from '@mui/x-date-pickers-pro/models';
import dayjs, { type Dayjs } from 'dayjs';
import { useEffect, useMemo, useState } from 'react';
import SettingsSectionCard from './settings-section-card';
import StackColumn from './stack-column';
import StackRow from './stack-row';
import { BodyIconTypography } from './typography';
import TwoButtonsDialogActions from './commons/two-buttons-dialog-actions';

export type MarketplaceBookingResourceOption = { id: string; name: string; available: boolean };
export type MarketplaceBookingLocationOption = { id: string; name: string };
export type MarketplaceBookingModificationFormValues = { from: string; until: string; reason: string | null; resourceIds: ReadonlyArray<string>; locationId: string | null };

type Props = {
  initialFrom: string;
  initialUntil: string;
  currentResourceIds: ReadonlyArray<string>;
  currentResources: ReadonlyArray<{ id: string; name: string }>;
  currentLocationId: string | null;
  locations: ReadonlyArray<MarketplaceBookingLocationOption>;
  resources: ReadonlyArray<MarketplaceBookingResourceOption>;
  canSelectResources: boolean;
  maximumResourceCount: number;
  isSubmitting?: boolean;
  error?: string | null;
  onWindowChange?: (values: { from: string; until: string; locationId: string | null }) => void;
  onSubmit: (values: MarketplaceBookingModificationFormValues) => void;
  onCancel: () => void;
};

const isUtcMidnight = (value: Dayjs) => value.utc().hour() === 0 && value.utc().minute() === 0 && value.utc().second() === 0;

const MarketplaceBookingModificationForm = ({
  initialFrom,
  initialUntil,
  currentResourceIds,
  currentResources,
  currentLocationId,
  locations,
  resources,
  canSelectResources,
  maximumResourceCount,
  isSubmitting = false,
  error = null,
  onWindowChange,
  onSubmit,
  onCancel,
}: Props) => {
  const initialStart = dayjs.utc(initialFrom);
  const initialEnd = dayjs.utc(initialUntil);
  const [selectedDate, setSelectedDate] = useState<Dayjs | null>(initialStart.startOf('day'));
  const [allDay, setAllDay] = useState(isUtcMidnight(initialStart) && initialEnd.isSame(initialStart.add(1, 'day')));
  const [timeRange, setTimeRange] = useState<DateRange<Dayjs>>([initialStart, initialEnd]);
  const [reason, setReason] = useState('');
  const [locationId, setLocationId] = useState<string | null>(currentLocationId);
  const [resourceIds, setResourceIds] = useState<ReadonlyArray<string>>(currentResourceIds);
  const [validationError, setValidationError] = useState<string | null>(null);
  const resourceOptions = useMemo(() => {
    const selected =
      locationId === currentLocationId
        ? currentResources.filter((current) => !resources.some((resource) => resource.id === current.id)).map((current) => ({ ...current, available: false }))
        : [];
    return [...resources, ...selected];
  }, [currentLocationId, currentResources, locationId, resources]);
  const filterLocation = createFilterOptions<MarketplaceBookingLocationOption>();
  const canSubmitResources = !canSelectResources || (resourceIds.length > 0 && resourceIds.length <= maximumResourceCount);
  const canSubmitReason = reason.trim().length > 0;
  const getWindow = () => {
    const [startTime, endTime] = timeRange;
    if (!selectedDate || (!allDay && (!startTime || !endTime))) return null;
    const from = allDay ? selectedDate.utc().startOf('day') : selectedDate.utc().hour(startTime!.hour()).minute(startTime!.minute()).second(0);
    const until = allDay ? from.add(1, 'day') : selectedDate.utc().hour(endTime!.hour()).minute(endTime!.minute()).second(0);
    return until.isAfter(from) ? { from, until } : null;
  };
  useEffect(() => {
    const window = getWindow();
    if (window) onWindowChange?.({ from: window.from.toISOString(), until: window.until.toISOString(), locationId });
  }, [allDay, locationId, selectedDate, timeRange]); // The callback deliberately receives only complete windows.

  const submit = () => {
    const window = getWindow();
    if (!window) {
      setValidationError('Choose an end time after the new start time.');
      return;
    }
    if (!canSubmitReason) {
      setValidationError('Provide a reason for this booking change.');
      return;
    }
    const validResourceIds = resourceIds.filter((id) => resourceOptions.some((resource) => resource.id === id));
    if (!canSubmitResources || validResourceIds.length !== resourceIds.length) {
      setValidationError(`Choose between 1 and ${maximumResourceCount} eligible resources.`);
      return;
    }
    onSubmit({ from: window.from.toISOString(), until: window.until.toISOString(), reason: reason.trim() || null, resourceIds: validResourceIds, locationId });
  };

  return (
    <Box sx={{ width: '100%', maxWidth: 1200, mx: 'auto', px: { xs: 1, sm: 2, md: 3 }, pt: { xs: 2, md: 3 }, pb: 4 }}>
      <SettingsSectionCard title="Schedule" description="Pick the date and time for this booking.">
        <StackColumn spacing={2} sx={{ pt: 1 }}>
          {error || validationError ? <Alert severity="error">{error ?? validationError}</Alert> : null}
          <StackColumn spacing={1.5}>
            <StackRow spacing={2} sx={{ alignItems: 'center' }}>
              <DatePicker
                label="Date"
                value={selectedDate}
                onChange={(value) => setSelectedDate(value ? dayjs.utc(value.format('YYYY-MM-DD')).startOf('day') : null)}
                disablePast
                sx={{ width: 245 }}
              />
              <FormControlLabel control={<Switch checked={allDay} onChange={(event) => setAllDay(event.target.checked)} />} label="All Day" />
            </StackRow>
            <TimeRangePicker value={timeRange} onChange={setTimeRange} disabled={allDay} sx={{ width: 245 }} />
          </StackColumn>
          {locations.length > 0 ? (
            <Autocomplete
              options={locations}
              value={locations.find((location) => location.id === locationId) ?? null}
              getOptionLabel={(location) => location.name}
              filterOptions={filterLocation}
              renderInput={(params) => <TextField {...params} label="Location" helperText="Choose a location with resources eligible for this product." />}
              onChange={(_, location) => {
                setLocationId(location?.id ?? null);
                setResourceIds(location?.id === currentLocationId ? currentResourceIds : []);
              }}
            />
          ) : null}
          {canSelectResources ? (
            <StackColumn spacing={0.5}>
              <BodyIconTypography label={`Resources (${resourceIds.length}/${maximumResourceCount})`} />
              <BodyIconTypography
                label={`Select up to ${maximumResourceCount} eligible resource${maximumResourceCount === 1 ? '' : 's'}. Availability is checked again when you confirm.`}
                sx={{ opacity: 0.75 }}
              />
              {resourceOptions.length > 0 ? (
                maximumResourceCount === 1 ? (
                  <Autocomplete
                    options={resourceOptions}
                    value={resourceOptions.find((resource) => resourceIds.includes(resource.id)) ?? null}
                    isOptionEqualToValue={(option, value) => option.id === value.id}
                    getOptionLabel={(resource) => (resource.available ? resource.name : `${resource.name} (unavailable)`)}
                    getOptionDisabled={(resource) => !resource.available && !resourceIds.includes(resource.id)}
                    onChange={(_, selected) => setResourceIds(selected ? [selected.id] : [])}
                    renderInput={(params) => <TextField {...params} label="Resources" helperText="Select one resource." />}
                  />
                ) : (
                  <Autocomplete
                    multiple
                    options={resourceOptions}
                    value={resourceOptions.filter((resource) => resourceIds.includes(resource.id))}
                    isOptionEqualToValue={(option, value) => option.id === value.id}
                    getOptionLabel={(resource) => (resource.available ? resource.name : `${resource.name} (unavailable)`)}
                    getOptionDisabled={(resource) => !resource.available && !resourceIds.includes(resource.id)}
                    onChange={(_, selected) => setResourceIds(selected.map((resource) => resource.id).slice(0, maximumResourceCount))}
                    renderInput={(params) => <TextField {...params} label="Resources" helperText={`Select up to ${maximumResourceCount} resources.`} />}
                  />
                )
              ) : (
                <Alert severity="warning">No eligible resources are available for selection.</Alert>
              )}
            </StackColumn>
          ) : null}
          <TextField
            label="Reason"
            required
            value={reason}
            onChange={(event) => setReason(event.target.value)}
            helperText="Explain why this booking is being changed."
            multiline
            minRows={2}
          />
          <TwoButtonsDialogActions
            primaryLabel="Update booking"
            secondaryLabel="Cancel"
            primaryDisabled={isSubmitting || !canSubmitResources || !canSubmitReason}
            onPrimaryClicked={submit}
            onSecondaryClicked={onCancel}
          />
        </StackColumn>
      </SettingsSectionCard>
    </Box>
  );
};

export default MarketplaceBookingModificationForm;

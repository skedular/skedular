'use client';

import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import logger from '@/libs/logging';
import {
  logCustomerMarketplaceBookingModificationCompleted,
  logCustomerMarketplaceBookingModificationFailed,
  logCustomerMarketplaceBookingModificationStarted,
} from '@/libs/logging/aggregate-marketplace-telemetry';
import type { modifyMarketplaceBookingDialog_booking_refetchableFragment } from '@/queries/__generated__/modifyMarketplaceBookingDialog_booking_refetchableFragment.graphql';
import type { modifyMarketplaceBookingDialog_query$key } from '@/queries/__generated__/modifyMarketplaceBookingDialog_query.graphql';
import type { modifyMarketplaceBookingDialog_modifyMarketplaceBookingMutation } from '@/queries/__generated__/modifyMarketplaceBookingDialog_modifyMarketplaceBookingMutation.graphql';
import Alert from '@mui/material/Alert';
import Autocomplete from '@mui/material/Autocomplete';
import Box from '@mui/material/Box';
import FormControlLabel from '@mui/material/FormControlLabel';
import Switch from '@mui/material/Switch';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import TextField from '@mui/material/TextField';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { TimeRangePicker } from '@mui/x-date-pickers-pro/TimeRangePicker';
import type { DateRange } from '@mui/x-date-pickers-pro/models';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { getOpeningHoursFromDateTime, getRelayErrorMessage, isMidnight, toOpeningHoursFromTime } from '@skedular/shared';
import { BodyIconTypography, DefaultDialogTitle, SettingsSectionCard, StackColumn, StackRow, TwoButtonsDialogActions } from '@skedular/ui';
import dayjs, { type Dayjs } from 'dayjs';
import { useEffect, useMemo, useState } from 'react';
import { graphql, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type ResourceOption = { id: string; name: string; available: boolean };
type LocationOption = { id: string; name: string };

type Props = {
  bookingId: string;
  expectedVersion: number;
  initialFrom: string;
  initialUntil: string;
  currentResourceIds?: ReadonlyArray<string>;
  currentResources?: ReadonlyArray<{ id: string; name: string }>;
  currentLocationId?: string;
  rootDataRelay: modifyMarketplaceBookingDialog_query$key;
  page?: boolean;
  onClose: () => void;
  onModified: () => void;
};

const ModifyMarketplaceBookingMutation = graphql`
  mutation modifyMarketplaceBookingDialog_modifyMarketplaceBookingMutation($input: ModifyMarketplaceBookingInput!) {
    modifyMarketplaceBooking(input: $input) {
      booking {
        id
        from
        until
      }
      eligibilityError {
        code
        message
      }
      availabilityError {
        message
      }
      conflictError {
        code
        message
      }
      accessError {
        message
      }
    }
  }
`;

const ModifyMarketplaceBookingDialog = ({
  bookingId,
  expectedVersion,
  initialFrom,
  initialUntil,
  currentResourceIds = [],
  currentResources = [],
  currentLocationId,
  rootDataRelay,
  page = false,
  onClose,
  onModified,
}: Props) => {
  const initialStart = dayjs(initialFrom);
  const initialEnd = dayjs(initialUntil);
  const [selectedDate, setSelectedDate] = useState<Dayjs | null>(dayjs.utc(initialFrom).startOf('day'));
  const [timeRange, setTimeRange] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(getOpeningHoursFromDateTime(initialFrom)),
    toOpeningHoursFromTime(getOpeningHoursFromDateTime(initialUntil)),
  ]);
  // An all-day marketplace booking is exactly one UTC calendar day, not merely two
  // timestamps that happen to fall at midnight.
  const [allDay, setAllDay] = useState(isMidnight(initialStart.utc()) && initialEnd.utc().isSame(initialStart.utc().add(1, 'day')));
  const [reason, setReason] = useState('');
  const [selectedLocationId, setSelectedLocationId] = useState<string | undefined>(currentLocationId);
  const [selectedResourceIds, setSelectedResourceIds] = useState<ReadonlyArray<string>>(currentResourceIds);
  const [validationError, setValidationError] = useState<string | null>(null);
  const [commitModifyMarketplaceBooking, isInFlight] = useMutation<modifyMarketplaceBookingDialog_modifyMarketplaceBookingMutation>(ModifyMarketplaceBookingMutation);
  const [resourceSelectionData, refetchResourceSelection] = useRefetchableFragment<
    modifyMarketplaceBookingDialog_booking_refetchableFragment,
    modifyMarketplaceBookingDialog_query$key
  >(
    graphql`
      fragment modifyMarketplaceBookingDialog_query on Query
      @argumentDefinitions(bookingId: { type: "String!" }, from: { type: "DateTime" }, until: { type: "DateTime" }, locationId: { type: "String" })
      @refetchable(queryName: "modifyMarketplaceBookingDialog_booking_refetchableFragment") {
        booking(id: $bookingId) {
          marketplaceBookingResourceSelection(from: $from, until: $until, locationId: $locationId) {
            canSelectResources
            maximumResourceCount
            availableResourceIds
            eligibleLocations {
              uniqueId
              name
            }
            eligibleResources {
              resource {
                id
                name
              }
            }
          }
        }
      }
    `,
    rootDataRelay,
  );
  const resourceSelection = resourceSelectionData.booking?.marketplaceBookingResourceSelection;
  const canSelectResources = resourceSelection?.canSelectResources === true;
  const maximumResourceCount = resourceSelection?.maximumResourceCount ?? 0;
  const resourceOptions = useMemo<ReadonlyArray<ResourceOption>>(() => {
    const availableResourceIds = new Set<string>(resourceSelection?.availableResourceIds ?? []);
    const eligible =
      resourceSelection?.eligibleResources.map(({ resource }: { resource: { id: string; name: string } }) => ({
        ...resource,
        available: availableResourceIds.has(resource.id),
      })) ?? [];
    const current =
      selectedLocationId === currentLocationId
        ? currentResources
            .filter((resource) => !eligible.some((eligibleResource: ResourceOption) => eligibleResource.id === resource.id))
            .map((resource) => ({ ...resource, available: false }))
        : [];
    return [...eligible, ...current];
  }, [currentLocationId, currentResources, resourceSelection, selectedLocationId]);
  const locations = useMemo<ReadonlyArray<LocationOption>>(
    () =>
      resourceSelection?.eligibleLocations.map((location: { uniqueId: string; name: string }) => ({
        id: location.uniqueId,
        name: location.name,
      })) ?? [],
    [resourceSelection],
  );
  const filterLocation = createFilterOptions<LocationOption>();
  const canSubmitResourceSelection = !canSelectResources || (selectedResourceIds.length > 0 && selectedResourceIds.length <= maximumResourceCount);

  useEffect(() => {
    const [startTime, endTime] = timeRange;
    if (!selectedDate || !startTime || !endTime) {
      return;
    }

    const from = allDay ? selectedDate.utc().startOf('day') : selectedDate.utc().hour(startTime.hour()).minute(startTime.minute()).second(0);
    const until = allDay ? from.add(1, 'day') : selectedDate.utc().hour(endTime.hour()).minute(endTime.minute()).second(0);
    if (!until.isAfter(from)) {
      return;
    }

    refetchResourceSelection(
      {
        bookingId,
        from: from.toISOString(),
        until: until.toISOString(),
        locationId: selectedLocationId ?? null,
      },
      { fetchPolicy: 'store-and-network' },
    );
  }, [allDay, bookingId, refetchResourceSelection, selectedDate, selectedLocationId, timeRange]);

  const submit = () => {
    const [startTime, endTime] = timeRange;
    const from =
      selectedDate && (allDay || startTime)
        ? allDay
          ? selectedDate.utc().startOf('day')
          : selectedDate.utc().hour(startTime!.hour()).minute(startTime!.minute()).second(0)
        : null;
    const until =
      selectedDate && (allDay || endTime) ? (allDay ? (from?.add(1, 'day') ?? null) : selectedDate.utc().hour(endTime!.hour()).minute(endTime!.minute()).second(0)) : null;
    if (!from || !until || !until.isAfter(from)) {
      setValidationError('Choose an end time after the new start time.');
      return;
    }
    if (!reason.trim()) {
      setValidationError('Provide a reason for this booking change.');
      return;
    }
    const validSelectedResourceIds = selectedResourceIds.filter((resourceId) => resourceOptions.some((resource) => resource.id === resourceId));
    if (!canSubmitResourceSelection || validSelectedResourceIds.length !== selectedResourceIds.length) {
      setValidationError(`Choose between 1 and ${maximumResourceCount} eligible resources.`);
      return;
    }

    logCustomerMarketplaceBookingModificationStarted({ logger, bookingId });
    commitModifyMarketplaceBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          bookingId,
          expectedVersion,
          from: from.toISOString(),
          until: until.toISOString(),
          reason: reason.trim(),
          actorKind: 'CUSTOMER',
          resourceIds: canSelectResources ? validSelectedResourceIds : null,
        },
      },
      onCompleted: (data, errors) => {
        const payload = data?.modifyMarketplaceBooking;
        const error = payload?.eligibilityError ?? payload?.availabilityError ?? payload?.conflictError ?? payload?.accessError;
        if (error || (errors?.length ?? 0) > 0) {
          const message = error?.message ?? getRelayErrorMessage(errors ?? []);
          setValidationError(message);
          logCustomerMarketplaceBookingModificationFailed({
            logger,
            bookingId,
            reasonCode: 'api_error',
          });
          return;
        }

        logCustomerMarketplaceBookingModificationCompleted({
          logger,
          bookingId,
        });
        toast(<NotificationContent content="Your booking dates and times have been updated." />);
        onModified();
      },
      onError: (error) => {
        const message = getRelayErrorMessage(error);
        setValidationError(message);
        logCustomerMarketplaceBookingModificationFailed({
          logger,
          bookingId,
          reasonCode: 'network_error',
        });
        toast(<NotificationContent content={`We couldn't update this booking. ${message}`} />, errorNotificationOptions);
      },
    });
  };

  const formContent = (
    <StackColumn spacing={2} sx={{ pt: 1 }}>
      {!page ? <BodyIconTypography label="Your purchase, price, and payment stay the same. We’ll check availability before saving this change." /> : null}
      {validationError ? <Alert severity="error">{validationError}</Alert> : null}
      {page ? (
        <StackColumn spacing={1.5}>
          <StackRow spacing={2} sx={{ alignItems: 'center' }}>
            <DatePicker
              label="Date"
              value={selectedDate}
              onChange={(value) => {
                const normalizedValue = value ? dayjs.utc(value.format('YYYY-MM-DD')).startOf('day') : null;
                setSelectedDate(normalizedValue);
                if (allDay && normalizedValue) {
                  setTimeRange([normalizedValue, normalizedValue.add(1, 'day')]);
                }
              }}
              disablePast
              sx={{ width: 245 }}
            />
            <FormControlLabel
              control={
                <Switch
                  checked={allDay}
                  onChange={(event) => {
                    const checked = event.target.checked;
                    setAllDay(checked);
                    if (checked && selectedDate) {
                      const dayStart = selectedDate.utc().startOf('day');
                      setTimeRange([dayStart, dayStart.add(1, 'day')]);
                    }
                  }}
                />
              }
              label="All Day"
            />
          </StackRow>
          <TimeRangePicker value={timeRange} onChange={setTimeRange} disabled={allDay} sx={{ width: 245 }} />
        </StackColumn>
      ) : (
        <>
          <DatePicker label="Date" value={selectedDate} onChange={setSelectedDate} disablePast />
          <TimeRangePicker value={timeRange} onChange={setTimeRange} />
        </>
      )}
      {locations.length > 0 ? (
        <Autocomplete
          options={locations}
          value={locations.find((location) => location.id === selectedLocationId) ?? null}
          getOptionLabel={(location) => location.name}
          filterOptions={filterLocation}
          renderOption={(props, option) => (
            <li {...props} key={option.id}>
              {option.name}
            </li>
          )}
          renderInput={(params) => <TextField {...params} label="Location" helperText="Choose a location with resources eligible for this product." />}
          onChange={(_, location) => {
            setSelectedLocationId(location?.id);
            setSelectedResourceIds(location?.id === currentLocationId ? currentResourceIds : []);
          }}
        />
      ) : null}
      {canSelectResources ? (
        <StackColumn spacing={0.5}>
          <BodyIconTypography label={`Resources (${selectedResourceIds.length}/${maximumResourceCount})`} />
          <SmallResourceSelectionHelp maximumResourceCount={maximumResourceCount} />
          {resourceOptions.length > 0 ? (
            <Autocomplete
              multiple
              options={resourceOptions}
              value={resourceOptions.filter((resource) => selectedResourceIds.includes(resource.id))}
              getOptionLabel={(resource) => (resource.available ? resource.name : `${resource.name} (unavailable)`)}
              getOptionDisabled={(resource) => !resource.available && !selectedResourceIds.includes(resource.id)}
              onChange={(_, resources) => {
                const nextResourceIds = resources.map((resource) => resource.id);
                setSelectedResourceIds(maximumResourceCount === 1 ? nextResourceIds.slice(-1) : nextResourceIds.slice(0, maximumResourceCount));
              }}
              renderInput={(params) => (
                <TextField {...params} label="Resources" helperText={`Select up to ${maximumResourceCount} resource${maximumResourceCount === 1 ? '' : 's'}.`} />
              )}
            />
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
        primaryDisabled={isInFlight || !canSubmitResourceSelection || !reason.trim()}
        onPrimaryClicked={submit}
        onSecondaryClicked={onClose}
      />
    </StackColumn>
  );

  if (page) {
    return (
      <Box
        sx={{
          width: '100%',
          maxWidth: 1200,
          mx: 'auto',
          px: { xs: 1, sm: 2, md: 3 },
          pt: { xs: 2, md: 3 },
          pb: 4,
        }}
      >
        <SettingsSectionCard title="Schedule" description="Pick the date and time for this booking.">
          {formContent}
        </SettingsSectionCard>
      </Box>
    );
  }

  return (
    <Dialog open onClose={isInFlight ? undefined : onClose} fullWidth maxWidth="sm">
      <DefaultDialogTitle title="Change booking date and time" />
      <DialogContent>{formContent}</DialogContent>
    </Dialog>
  );
};

export const SmallResourceSelectionHelp = ({ maximumResourceCount }: { maximumResourceCount: number }) => (
  <BodyIconTypography
    label={`Select up to ${maximumResourceCount} eligible resource${maximumResourceCount === 1 ? '' : 's'}. Availability is checked again when you confirm.`}
    sx={{ opacity: 0.75 }}
  />
);

export default ModifyMarketplaceBookingDialog;

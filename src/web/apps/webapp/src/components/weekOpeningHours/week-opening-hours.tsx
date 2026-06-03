'use client';

import { ClosedOpenAllDayCustomToggle } from '@/components/closedOpenAllDayCustomToggle';
import type { weekOpeningHours_query$key } from '@/queries/__generated__/weekOpeningHours_query.graphql';
import { DateRange } from '@mui/x-date-pickers-pro/models';
import { TimeRangePicker } from '@mui/x-date-pickers-pro/TimeRangePicker';
import { getOpeningHoursFromDateTime, toOpeningHoursFromTime } from '@skedular/shared';
import { defaultPadding, ErrorTypography, FormFieldLabel, StackColumn, StackRow } from '@skedular/ui';
import { Dayjs } from 'dayjs';
import { memo, useEffect, useMemo, useRef, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
import { useDebounceCallback } from 'usehooks-ts';

type Props = {
  rootDataRelay: weekOpeningHours_query$key;
  defaultValue: WeekOpeningHoursDetails;
  onWeekOpeningHoursDetailUpdateClick: (weekOpeningHours: WeekOpeningHoursDetails) => void;
};

export type OpeningHoursDetails = {
  closed: boolean;
  openAllDay: boolean;
  from: string | null | undefined;
  until: string | null | undefined;
};

export type WeekOpeningHoursDetails = {
  monday: OpeningHoursDetails;
  tuesday: OpeningHoursDetails;
  wednesday: OpeningHoursDetails;
  thursday: OpeningHoursDetails;
  friday: OpeningHoursDetails;
  saturday: OpeningHoursDetails;
  sunday: OpeningHoursDetails;
};

type OpeningHoursState = 'closed' | 'openAllDay' | 'custom';
export type OpeningHoursDetailsInternal<T = Date | Dayjs> = {
  state: OpeningHoursState;
  from: T | null;
  until: T | null;
};

const weekdays = ['monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday'] as const;
const openingHoursAutosaveDebounceTimeout = 1000;

const WeekOpeningHours = ({ rootDataRelay, defaultValue, onWeekOpeningHoursDetailUpdateClick }: Props) => {
  const rootData = useFragment<weekOpeningHours_query$key>(
    graphql`
      fragment weekOpeningHours_query on Query {
        bookingSlotSizeInMinutes
      }
    `,
    rootDataRelay,
  );

  const minutesStep = rootData.bookingSlotSizeInMinutes;

  const validate = (state: OpeningHoursState, from: Dayjs | null, until: Dayjs | null) => {
    if (state === 'custom' && (!from || !until)) {
      return { result: false, errorMessage: 'From and Until required when not closed or open all day' };
    }

    if (state === 'custom' && from && (from.isSame(until) || from.isAfter(until))) {
      return { result: false, errorMessage: 'From cannot be same or after Until' };
    }

    return { result: true, errorMessage: '' };
  };

  const getValue = (state: OpeningHoursState, from: Dayjs | null, until: Dayjs | null) =>
    state === 'closed'
      ? { closed: true, openAllDay: false, from: null, until: null }
      : state === 'openAllDay'
        ? { closed: false, openAllDay: true, from: null, until: null }
        : { closed: false, openAllDay: false, from: getOpeningHoursFromDateTime(from), until: getOpeningHoursFromDateTime(until) };

  const [states, setStates] = useState<Record<string, OpeningHoursState>>(() =>
    Object.fromEntries(weekdays.map((day) => [day, defaultValue[day].closed ? 'closed' : defaultValue[day].openAllDay ? 'openAllDay' : 'custom'])),
  );

  const [hours, setHours] = useState<Record<string, DateRange<Dayjs>>>(() =>
    Object.fromEntries(weekdays.map((day) => [day, [toOpeningHoursFromTime(defaultValue[day].from), toOpeningHoursFromTime(defaultValue[day].until)]])),
  );

  const validations = useMemo(() => Object.fromEntries(weekdays.map((day) => [day, validate(states[day], hours[day][0], hours[day][1])])), [states, hours]);

  const handleStateChange = (day: string, value: OpeningHoursState) => {
    setStates((prev) => ({ ...prev, [day]: value }));
  };

  const handleHoursChange = (day: string, value: DateRange<Dayjs>) => {
    setHours((prev) => ({ ...prev, [day]: value }));
  };

  const nextWeekOpeningHours = useMemo(() => {
    const allValid = weekdays.every((day) => validations[day].result);
    if (!allValid) {
      return null;
    }

    return Object.fromEntries(weekdays.map((day) => [day, getValue(states[day], hours[day][0], hours[day][1])])) as WeekOpeningHoursDetails;
  }, [validations, states, hours]);
  const submittedWeekOpeningHoursKey = useRef(JSON.stringify(defaultValue));
  const debouncedUpdateWeekOpeningHours = useDebounceCallback(onWeekOpeningHoursDetailUpdateClick, openingHoursAutosaveDebounceTimeout);

  useEffect(() => {
    if (!nextWeekOpeningHours) {
      return;
    }

    const nextKey = JSON.stringify(nextWeekOpeningHours);
    if (nextKey === submittedWeekOpeningHoursKey.current) {
      return;
    }

    submittedWeekOpeningHoursKey.current = nextKey;
    debouncedUpdateWeekOpeningHours(nextWeekOpeningHours);
  }, [debouncedUpdateWeekOpeningHours, nextWeekOpeningHours]);

  return (
    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
      {weekdays.map((day) => (
        <div key={day}>
          <FormFieldLabel label={day.charAt(0).toUpperCase() + day.slice(1)} stackLabelOnTop>
            <StackRow>
              <ClosedOpenAllDayCustomToggle defaultValue={states[day]} onChange={(value) => handleStateChange(day, value)} />
              <TimeRangePicker minutesStep={minutesStep} disabled={states[day] !== 'custom'} defaultValue={hours[day]} onChange={(value) => handleHoursChange(day, value)} />
            </StackRow>
          </FormFieldLabel>
          <FormFieldLabel stackLabelOnTop>
            <ErrorTypography errorMessage={validations[day].errorMessage} />
          </FormFieldLabel>
        </div>
      ))}
    </StackColumn>
  );
};

export default memo(WeekOpeningHours);

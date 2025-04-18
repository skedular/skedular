import { ClosedOpenAllDayCustomToggle } from '@/components/closedOpenAllDayCustomToggle';
import { FormFieldLabel, StackColumn, StackRow } from '@/components/commons';
import { autoCloseErrorNotificationOptions, NotificationContent } from '@/components/notification';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { getOpeningHoursFromDateTime, toOpeningHoursFromTime } from '@/libs/utils';
import type { weekOpeningHours_query$key } from '@/queries/__generated__/weekOpeningHours_query.graphql';
import Button from '@mui/material/Button';
import { DateRange } from '@mui/x-date-pickers-pro/models';
import { TimeRangePicker } from '@mui/x-date-pickers-pro/TimeRangePicker';
import { Dayjs } from 'dayjs';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
import { toast } from 'react-toastify';

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

export type OpeningHoursDetailsInternal = {
  state: 'closed' | 'openAllDay' | 'custom';
  from: Date | Dayjs | null;
  until: Date | Dayjs | null;
};

const WeekOpeningHours = ({ rootDataRelay, defaultValue, onWeekOpeningHoursDetailUpdateClick }: Props) => {
  const rootData = useFragment<weekOpeningHours_query$key>(
    graphql`
      fragment weekOpeningHours_query on Query {
        openingHoursMinutesStep
      }
    `,
    rootDataRelay,
  );

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [mondayOpeningState, setMondayOpeningState] = useState<'closed' | 'openAllDay' | 'custom'>(
    defaultValue.monday.closed ? 'closed' : defaultValue.monday.openAllDay ? 'openAllDay' : 'custom',
  );
  const [mondayOpeningHours, setMondayOpeningHours] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(defaultValue.monday.from),
    toOpeningHoursFromTime(defaultValue.monday.until),
  ]);

  const [tuesdayOpeningState, setTuesdayOpeningState] = useState<'closed' | 'openAllDay' | 'custom'>(
    defaultValue.tuesday.closed ? 'closed' : defaultValue.tuesday.openAllDay ? 'openAllDay' : 'custom',
  );
  const [tuesdayOpeningHours, setTuesdayOpeningHours] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(defaultValue.tuesday.from),
    toOpeningHoursFromTime(defaultValue.tuesday.until),
  ]);

  const [wednesdayOpeningState, setWednesdayOpeningState] = useState<'closed' | 'openAllDay' | 'custom'>(
    defaultValue.wednesday.closed ? 'closed' : defaultValue.wednesday.openAllDay ? 'openAllDay' : 'custom',
  );
  const [wednesdayOpeningHours, setWednesdayOpeningHours] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(defaultValue.wednesday.from),
    toOpeningHoursFromTime(defaultValue.wednesday.until),
  ]);

  const [thursdayOpeningState, setThursdayOpeningState] = useState<'closed' | 'openAllDay' | 'custom'>(
    defaultValue.thursday.closed ? 'closed' : defaultValue.thursday.openAllDay ? 'openAllDay' : 'custom',
  );
  const [thursdayOpeningHours, setThursdayOpeningHours] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(defaultValue.thursday.from),
    toOpeningHoursFromTime(defaultValue.thursday.until),
  ]);

  const [fridayOpeningState, setFridayOpeningState] = useState<'closed' | 'openAllDay' | 'custom'>(
    defaultValue.friday.closed ? 'closed' : defaultValue.friday.openAllDay ? 'openAllDay' : 'custom',
  );
  const [fridayOpeningHours, setFridayOpeningHours] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(defaultValue.friday.from),
    toOpeningHoursFromTime(defaultValue.friday.until),
  ]);

  const [saturdayOpeningState, setSaturdayOpeningState] = useState<'closed' | 'openAllDay' | 'custom'>(
    defaultValue.saturday.closed ? 'closed' : defaultValue.saturday.openAllDay ? 'openAllDay' : 'custom',
  );
  const [saturdayOpeningHours, setSaturdayOpeningHours] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(defaultValue.saturday.from),
    toOpeningHoursFromTime(defaultValue.saturday.until),
  ]);

  const [sundayOpeningState, setSundayOpeningState] = useState<'closed' | 'openAllDay' | 'custom'>(
    defaultValue.sunday.closed ? 'closed' : defaultValue.sunday.openAllDay ? 'openAllDay' : 'custom',
  );
  const [sundayOpeningHours, setSundayOpeningHours] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(defaultValue.sunday.from),
    toOpeningHoursFromTime(defaultValue.sunday.until),
  ]);
  const minutesStep = rootData.openingHoursMinutesStep;

  const validate = (weekday: string, state: 'closed' | 'openAllDay' | 'custom', from: Dayjs | null, until: Dayjs | null): boolean => {
    if (state === 'custom' && (!from || !until)) {
      themedToast(<NotificationContent content={`${weekday}: From and Until required when not closed or open all day`} />, autoCloseErrorNotificationOptions);

      return false;
    }

    if (state === 'custom' && from && (from.isSame(until) || from.isAfter(until))) {
      themedToast(<NotificationContent content={`${weekday}: From cannot be same or after Until`} />, autoCloseErrorNotificationOptions);

      return false;
    }

    return true;
  };

  const getValue = (state: 'closed' | 'openAllDay' | 'custom', from: Dayjs | null, until: Dayjs | null) => {
    return state === 'closed'
      ? { closed: true, openAllDay: false, from: null, until: null }
      : state === 'openAllDay'
        ? { closed: false, openAllDay: true, from: null, until: null }
        : { closed: false, openAllDay: false, from: getOpeningHoursFromDateTime(from), until: getOpeningHoursFromDateTime(until) };
  };

  const handleWeekOpeningHoursDetailUpdateClick = () => {
    const [mondayOpeningHoursFrom, mondayOpeningHoursUntil] = mondayOpeningHours;
    if (!validate('Monday', mondayOpeningState, mondayOpeningHoursFrom, mondayOpeningHoursUntil)) {
      return;
    }

    const [tuesdayOpeningHoursFrom, tuesdayOpeningHoursUntil] = tuesdayOpeningHours;
    if (!validate('Tuesday', tuesdayOpeningState, tuesdayOpeningHoursFrom, tuesdayOpeningHoursUntil)) {
      return;
    }

    const [wednesdayOpeningHoursFrom, wednesdayOpeningHoursUntil] = wednesdayOpeningHours;
    if (!validate('Wednesday', wednesdayOpeningState, wednesdayOpeningHoursFrom, wednesdayOpeningHoursUntil)) {
      return;
    }

    const [thursdayOpeningHoursFrom, thursdayOpeningHoursUntil] = thursdayOpeningHours;
    if (!validate('Thursday', thursdayOpeningState, thursdayOpeningHoursFrom, thursdayOpeningHoursUntil)) {
      return;
    }

    const [fridayOpeningHoursFrom, fridayOpeningHoursUntil] = fridayOpeningHours;
    if (!validate('Friday', fridayOpeningState, fridayOpeningHoursFrom, fridayOpeningHoursUntil)) {
      return;
    }

    const [saturdayOpeningHoursFrom, saturdayOpeningHoursUntil] = saturdayOpeningHours;
    if (!validate('Saturday', saturdayOpeningState, saturdayOpeningHoursFrom, saturdayOpeningHoursUntil)) {
      return;
    }

    const [sundayOpeningHoursFrom, sundayOpeningHoursUntil] = sundayOpeningHours;
    if (!validate('Sunday', sundayOpeningState, sundayOpeningHoursFrom, sundayOpeningHoursUntil)) {
      return;
    }

    onWeekOpeningHoursDetailUpdateClick({
      monday: getValue(mondayOpeningState, mondayOpeningHoursFrom, mondayOpeningHoursUntil),
      tuesday: getValue(tuesdayOpeningState, tuesdayOpeningHoursFrom, tuesdayOpeningHoursUntil),
      wednesday: getValue(wednesdayOpeningState, wednesdayOpeningHoursFrom, wednesdayOpeningHoursUntil),
      thursday: getValue(thursdayOpeningState, thursdayOpeningHoursFrom, thursdayOpeningHoursUntil),
      friday: getValue(fridayOpeningState, fridayOpeningHoursFrom, fridayOpeningHoursUntil),
      saturday: getValue(saturdayOpeningState, saturdayOpeningHoursFrom, saturdayOpeningHoursUntil),
      sunday: getValue(sundayOpeningState, sundayOpeningHoursFrom, sundayOpeningHoursUntil),
    });
  };

  return (
    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
      <FormFieldLabel label="Monday">
        <StackRow>
          <ClosedOpenAllDayCustomToggle defaultValue={mondayOpeningState} onChange={setMondayOpeningState} />
          <TimeRangePicker minutesStep={minutesStep} disabled={mondayOpeningState !== 'custom'} defaultValue={mondayOpeningHours} onChange={setMondayOpeningHours} />
        </StackRow>
      </FormFieldLabel>

      <FormFieldLabel label="Tuesday">
        <StackRow>
          <ClosedOpenAllDayCustomToggle defaultValue={tuesdayOpeningState} onChange={setTuesdayOpeningState} />
          <TimeRangePicker minutesStep={minutesStep} disabled={tuesdayOpeningState !== 'custom'} defaultValue={tuesdayOpeningHours} onChange={setTuesdayOpeningHours} />
        </StackRow>
      </FormFieldLabel>

      <FormFieldLabel label="Wednesday">
        <StackRow>
          <ClosedOpenAllDayCustomToggle defaultValue={wednesdayOpeningState} onChange={setWednesdayOpeningState} />
          <TimeRangePicker minutesStep={minutesStep} disabled={wednesdayOpeningState !== 'custom'} defaultValue={wednesdayOpeningHours} onChange={setWednesdayOpeningHours} />
        </StackRow>
      </FormFieldLabel>

      <FormFieldLabel label="Thursday">
        <StackRow>
          <ClosedOpenAllDayCustomToggle defaultValue={thursdayOpeningState} onChange={setThursdayOpeningState} />
          <TimeRangePicker minutesStep={minutesStep} disabled={thursdayOpeningState !== 'custom'} defaultValue={thursdayOpeningHours} onChange={setThursdayOpeningHours} />
        </StackRow>
      </FormFieldLabel>

      <FormFieldLabel label="Friday">
        <StackRow>
          <ClosedOpenAllDayCustomToggle defaultValue={fridayOpeningState} onChange={setFridayOpeningState} />
          <TimeRangePicker minutesStep={minutesStep} disabled={fridayOpeningState !== 'custom'} defaultValue={fridayOpeningHours} onChange={setFridayOpeningHours} />
        </StackRow>
      </FormFieldLabel>

      <FormFieldLabel label="Saturday">
        <StackRow>
          <ClosedOpenAllDayCustomToggle defaultValue={saturdayOpeningState} onChange={setSaturdayOpeningState} />
          <TimeRangePicker minutesStep={minutesStep} disabled={saturdayOpeningState !== 'custom'} defaultValue={saturdayOpeningHours} onChange={setSaturdayOpeningHours} />
        </StackRow>
      </FormFieldLabel>

      <FormFieldLabel label="Sunday">
        <StackRow>
          <ClosedOpenAllDayCustomToggle defaultValue={sundayOpeningState} onChange={setSundayOpeningState} />
          <TimeRangePicker minutesStep={minutesStep} disabled={sundayOpeningState !== 'custom'} defaultValue={sundayOpeningHours} onChange={setSundayOpeningHours} />
        </StackRow>
      </FormFieldLabel>

      <StackRow>
        <Button variant="contained" sx={defaultButtonStyle} onClick={handleWeekOpeningHoursDetailUpdateClick}>
          Update
        </Button>
      </StackRow>
    </StackColumn>
  );
};

export default memo(WeekOpeningHours);

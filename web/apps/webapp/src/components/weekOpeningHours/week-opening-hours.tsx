import { ClosedOpenAllDayCustomToggle } from '@/components/closedOpenAllDayCustomToggle';
import { ErrorTypography, FormFieldLabel, StackColumn, StackRow } from '@/components/commons';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { getOpeningHoursFromDateTime, toOpeningHoursFromTime } from '@/libs/utils';
import type { weekOpeningHours_query$key } from '@/queries/__generated__/weekOpeningHours_query.graphql';
import Button from '@mui/material/Button';
import { DateRange } from '@mui/x-date-pickers-pro/models';
import { TimeRangePicker } from '@mui/x-date-pickers-pro/TimeRangePicker';
import { Dayjs } from 'dayjs';
import { memo, useCallback, useContext, useEffect, useState } from 'react';
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

type OpeningHoursState = 'closed' | 'openAllDay' | 'custom';

export type OpeningHoursDetailsInternal<T = Date | Dayjs> = {
  state: OpeningHoursState;
  from: T | null;
  until: T | null;
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
  const [mondayErrorMessage, setMondayErrorMessage] = useState<string>('');
  const [mondayOpeningState, setMondayOpeningState] = useState<OpeningHoursState>(defaultValue.monday.closed ? 'closed' : defaultValue.monday.openAllDay ? 'openAllDay' : 'custom');
  const [mondayOpeningHours, setMondayOpeningHours] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(defaultValue.monday.from),
    toOpeningHoursFromTime(defaultValue.monday.until),
  ]);

  const [tuesdayErrorMessage, setTuesdayErrorMessage] = useState<string>('');
  const [tuesdayOpeningState, setTuesdayOpeningState] = useState<OpeningHoursState>(
    defaultValue.tuesday.closed ? 'closed' : defaultValue.tuesday.openAllDay ? 'openAllDay' : 'custom',
  );
  const [tuesdayOpeningHours, setTuesdayOpeningHours] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(defaultValue.tuesday.from),
    toOpeningHoursFromTime(defaultValue.tuesday.until),
  ]);

  const [wednesdayErrorMessage, setWednesdayErrorMessage] = useState<string>('');
  const [wednesdayOpeningState, setWednesdayOpeningState] = useState<OpeningHoursState>(
    defaultValue.wednesday.closed ? 'closed' : defaultValue.wednesday.openAllDay ? 'openAllDay' : 'custom',
  );
  const [wednesdayOpeningHours, setWednesdayOpeningHours] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(defaultValue.wednesday.from),
    toOpeningHoursFromTime(defaultValue.wednesday.until),
  ]);

  const [thursdayErrorMessage, setThursdayErrorMessage] = useState<string>('');
  const [thursdayOpeningState, setThursdayOpeningState] = useState<OpeningHoursState>(
    defaultValue.thursday.closed ? 'closed' : defaultValue.thursday.openAllDay ? 'openAllDay' : 'custom',
  );
  const [thursdayOpeningHours, setThursdayOpeningHours] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(defaultValue.thursday.from),
    toOpeningHoursFromTime(defaultValue.thursday.until),
  ]);

  const [fridayErrorMessage, setFridayErrorMessage] = useState<string>('');
  const [fridayOpeningState, setFridayOpeningState] = useState<OpeningHoursState>(defaultValue.friday.closed ? 'closed' : defaultValue.friday.openAllDay ? 'openAllDay' : 'custom');
  const [fridayOpeningHours, setFridayOpeningHours] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(defaultValue.friday.from),
    toOpeningHoursFromTime(defaultValue.friday.until),
  ]);

  const [saturdayErrorMessage, setSaturdayErrorMessage] = useState<string>('');
  const [saturdayOpeningState, setSaturdayOpeningState] = useState<OpeningHoursState>(
    defaultValue.saturday.closed ? 'closed' : defaultValue.saturday.openAllDay ? 'openAllDay' : 'custom',
  );
  const [saturdayOpeningHours, setSaturdayOpeningHours] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(defaultValue.saturday.from),
    toOpeningHoursFromTime(defaultValue.saturday.until),
  ]);

  const [sundayErrorMessage, setSundayErrorMessage] = useState<string>('');
  const [sundayOpeningState, setSundayOpeningState] = useState<OpeningHoursState>(defaultValue.sunday.closed ? 'closed' : defaultValue.sunday.openAllDay ? 'openAllDay' : 'custom');
  const [sundayOpeningHours, setSundayOpeningHours] = useState<DateRange<Dayjs>>([
    toOpeningHoursFromTime(defaultValue.sunday.from),
    toOpeningHoursFromTime(defaultValue.sunday.until),
  ]);
  const minutesStep = rootData.openingHoursMinutesStep;

  const validate = (state: OpeningHoursState, from: Dayjs | null, until: Dayjs | null) => {
    if (state === 'custom' && (!from || !until)) {
      return {
        result: false,
        errorMessage: 'From and Until required when not closed or open all day',
      };
    }

    if (state === 'custom' && from && (from.isSame(until) || from.isAfter(until))) {
      return {
        result: false,
        errorMessage: 'From cannot be same or after Until',
      };
    }

    return {
      result: true,
      errorMessage: '',
    };
  };

  const getValue = (state: OpeningHoursState, from: Dayjs | null, until: Dayjs | null) => {
    return state === 'closed'
      ? { closed: true, openAllDay: false, from: null, until: null }
      : state === 'openAllDay'
        ? { closed: false, openAllDay: true, from: null, until: null }
        : { closed: false, openAllDay: false, from: getOpeningHoursFromDateTime(from), until: getOpeningHoursFromDateTime(until) };
  };

  const validateAll = useCallback(() => {
    let result = true;

    let validationResult = validate(mondayOpeningState, mondayOpeningHours[0], mondayOpeningHours[1]);
    if (validationResult.result) {
      setMondayErrorMessage('');
    } else {
      setMondayErrorMessage(validationResult.errorMessage);
      result = false;
    }

    validationResult = validate(tuesdayOpeningState, tuesdayOpeningHours[0], tuesdayOpeningHours[1]);
    if (validationResult.result) {
      setTuesdayErrorMessage('');
    } else {
      setTuesdayErrorMessage(validationResult.errorMessage);
      result = false;
    }

    validationResult = validate(wednesdayOpeningState, wednesdayOpeningHours[0], wednesdayOpeningHours[1]);
    if (validationResult.result) {
      setWednesdayErrorMessage('');
    } else {
      setWednesdayErrorMessage(validationResult.errorMessage);
      result = false;
    }

    validationResult = validate(thursdayOpeningState, thursdayOpeningHours[0], thursdayOpeningHours[1]);
    if (validationResult.result) {
      setThursdayErrorMessage('');
    } else {
      setThursdayErrorMessage(validationResult.errorMessage);
      result = false;
    }

    validationResult = validate(fridayOpeningState, fridayOpeningHours[0], fridayOpeningHours[1]);
    if (validationResult.result) {
      setFridayErrorMessage('');
    } else {
      setFridayErrorMessage(validationResult.errorMessage);
      result = false;
    }

    validationResult = validate(saturdayOpeningState, saturdayOpeningHours[0], saturdayOpeningHours[1]);
    if (validationResult.result) {
      setSaturdayErrorMessage('');
    } else {
      setSaturdayErrorMessage(validationResult.errorMessage);
      result = false;
    }

    validationResult = validate(sundayOpeningState, sundayOpeningHours[0], sundayOpeningHours[1]);
    if (validationResult.result) {
      setSundayErrorMessage('');
    } else {
      setSundayErrorMessage(validationResult.errorMessage);
      result = false;
    }

    return result;
  }, [
    mondayOpeningState,
    mondayOpeningHours,
    tuesdayOpeningState,
    tuesdayOpeningHours,
    wednesdayOpeningState,
    wednesdayOpeningHours,
    thursdayOpeningState,
    thursdayOpeningHours,
    fridayOpeningState,
    fridayOpeningHours,
    saturdayOpeningState,
    saturdayOpeningHours,
    sundayOpeningState,
    sundayOpeningHours,
  ]);

  const handleWeekOpeningHoursDetailUpdateClick = () => {
    if (!validateAll()) {
      return;
    }

    onWeekOpeningHoursDetailUpdateClick({
      monday: getValue(mondayOpeningState, mondayOpeningHours[0], mondayOpeningHours[1]),
      tuesday: getValue(tuesdayOpeningState, tuesdayOpeningHours[0], tuesdayOpeningHours[1]),
      wednesday: getValue(wednesdayOpeningState, wednesdayOpeningHours[0], wednesdayOpeningHours[1]),
      thursday: getValue(thursdayOpeningState, thursdayOpeningHours[0], thursdayOpeningHours[1]),
      friday: getValue(fridayOpeningState, fridayOpeningHours[0], fridayOpeningHours[1]),
      saturday: getValue(saturdayOpeningState, saturdayOpeningHours[0], saturdayOpeningHours[1]),
      sunday: getValue(sundayOpeningState, sundayOpeningHours[0], sundayOpeningHours[1]),
    });
  };

  useEffect(() => {
    validateAll();
  }, [validateAll]);

  return (
    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
      <FormFieldLabel label="Monday">
        <StackRow>
          <ClosedOpenAllDayCustomToggle defaultValue={mondayOpeningState} onChange={setMondayOpeningState} />
          <TimeRangePicker minutesStep={minutesStep} disabled={mondayOpeningState !== 'custom'} defaultValue={mondayOpeningHours} onChange={setMondayOpeningHours} />
        </StackRow>
      </FormFieldLabel>

      <FormFieldLabel>
        <ErrorTypography errorMessage={mondayErrorMessage} />
      </FormFieldLabel>

      <FormFieldLabel label="Tuesday">
        <StackRow>
          <ClosedOpenAllDayCustomToggle defaultValue={tuesdayOpeningState} onChange={setTuesdayOpeningState} />
          <TimeRangePicker minutesStep={minutesStep} disabled={tuesdayOpeningState !== 'custom'} defaultValue={tuesdayOpeningHours} onChange={setTuesdayOpeningHours} />
        </StackRow>
      </FormFieldLabel>

      <FormFieldLabel>
        <ErrorTypography errorMessage={tuesdayErrorMessage} />
      </FormFieldLabel>

      <FormFieldLabel label="Wednesday">
        <StackRow>
          <ClosedOpenAllDayCustomToggle defaultValue={wednesdayOpeningState} onChange={setWednesdayOpeningState} />
          <TimeRangePicker minutesStep={minutesStep} disabled={wednesdayOpeningState !== 'custom'} defaultValue={wednesdayOpeningHours} onChange={setWednesdayOpeningHours} />
        </StackRow>
      </FormFieldLabel>

      <FormFieldLabel>
        <ErrorTypography errorMessage={wednesdayErrorMessage} />
      </FormFieldLabel>

      <FormFieldLabel label="Thursday">
        <StackRow>
          <ClosedOpenAllDayCustomToggle defaultValue={thursdayOpeningState} onChange={setThursdayOpeningState} />
          <TimeRangePicker minutesStep={minutesStep} disabled={thursdayOpeningState !== 'custom'} defaultValue={thursdayOpeningHours} onChange={setThursdayOpeningHours} />
        </StackRow>
      </FormFieldLabel>

      <FormFieldLabel>
        <ErrorTypography errorMessage={thursdayErrorMessage} />
      </FormFieldLabel>

      <FormFieldLabel label="Friday">
        <StackRow>
          <ClosedOpenAllDayCustomToggle defaultValue={fridayOpeningState} onChange={setFridayOpeningState} />
          <TimeRangePicker minutesStep={minutesStep} disabled={fridayOpeningState !== 'custom'} defaultValue={fridayOpeningHours} onChange={setFridayOpeningHours} />
        </StackRow>
      </FormFieldLabel>

      <FormFieldLabel>
        <ErrorTypography errorMessage={fridayErrorMessage} />
      </FormFieldLabel>

      <FormFieldLabel label="Saturday">
        <StackRow>
          <ClosedOpenAllDayCustomToggle defaultValue={saturdayOpeningState} onChange={setSaturdayOpeningState} />
          <TimeRangePicker minutesStep={minutesStep} disabled={saturdayOpeningState !== 'custom'} defaultValue={saturdayOpeningHours} onChange={setSaturdayOpeningHours} />
        </StackRow>
      </FormFieldLabel>

      <FormFieldLabel>
        <ErrorTypography errorMessage={saturdayErrorMessage} />
      </FormFieldLabel>

      <FormFieldLabel label="Sunday">
        <StackRow>
          <ClosedOpenAllDayCustomToggle defaultValue={sundayOpeningState} onChange={setSundayOpeningState} />
          <TimeRangePicker minutesStep={minutesStep} disabled={sundayOpeningState !== 'custom'} defaultValue={sundayOpeningHours} onChange={setSundayOpeningHours} />
        </StackRow>
      </FormFieldLabel>

      <FormFieldLabel>
        <ErrorTypography errorMessage={sundayErrorMessage} />
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

import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import dayjs from 'dayjs';
import advancedFormat from 'dayjs/plugin/advancedFormat';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import { PropsWithChildren } from 'react';

dayjs.extend(utc);
dayjs.extend(timezone);
dayjs.extend(advancedFormat);

const DatePickerLocalizationProvider = ({ children }: PropsWithChildren) => (
  <LocalizationProvider dateAdapter={AdapterDayjs}>{children}</LocalizationProvider>
);

export default DatePickerLocalizationProvider;

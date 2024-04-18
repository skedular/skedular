import { StaticDatePickerSlotProps } from '@mui/x-date-pickers/StaticDatePicker';
import type { Dayjs } from 'dayjs';
import { memo } from 'react';

export const SimpleCalendarSlotProps: StaticDatePickerSlotProps<Dayjs> = {
  leftArrowIcon: { fontSize: 'medium' },
  rightArrowIcon: { fontSize: 'medium' },
  previousIconButton: {
    size: 'medium',
  },
  nextIconButton: {
    size: 'medium',
  },
  actionBar: {
    actions: [],
  },
};

const EmptyCalendarToolbar = () => {
  return <></>;
};

export default memo(EmptyCalendarToolbar);

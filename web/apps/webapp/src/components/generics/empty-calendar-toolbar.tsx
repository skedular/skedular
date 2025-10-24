import { StaticDatePickerSlotProps } from '@mui/x-date-pickers/StaticDatePicker';

export const SimpleCalendarSlotProps: StaticDatePickerSlotProps = {
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
  return null;
};

export default EmptyCalendarToolbar;

import Button from '@mui/material/Button';
import { BodyIconTypography, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';

const calendarDays = [
  ['MONDAY', 'Mon'],
  ['TUESDAY', 'Tue'],
  ['WEDNESDAY', 'Wed'],
  ['THURSDAY', 'Thu'],
  ['FRIDAY', 'Fri'],
  ['SATURDAY', 'Sat'],
  ['SUNDAY', 'Sun'],
] as const;

type Props = {
  availableDays?: string[];
  onChange: (availableDays: string[]) => void;
};

/** An empty selection deliberately means every calendar day is available. */
const CalendarDayPicker = ({ availableDays, onChange }: Props) => {
  const selectedDays = availableDays ?? [];
  const toggleDay = (day: string) => onChange(selectedDays.includes(day) ? selectedDays.filter((availableDay) => availableDay !== day) : [...selectedDays, day]);

  return (
    <StackColumn spacing={1}>
      <BodyIconTypography label="Available calendar days" />
      <SmallIconTypography label="Leave all days unselected to make this price available every day." />
      <StackRow sx={{ gap: 1, flexWrap: 'wrap' }}>
        {calendarDays.map(([day, label]) => (
          <Button key={day} variant={selectedDays.includes(day) ? 'contained' : 'outlined'} onClick={() => toggleDay(day)} sx={{ textTransform: 'none' }}>
            {label}
          </Button>
        ))}
      </StackRow>
    </StackColumn>
  );
};

export default CalendarDayPicker;

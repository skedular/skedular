import { LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@skedular/ui';
import { EmptyCalendarToolbar, SimpleCalendarSlotProps } from '@/components/generics';
import { CalendarIcon } from '@/components/icons';
import { DefaultSelect } from '@/components/styled';
import { startOfDay, toShortDateWithoutWeekDay } from '@skedular/shared';
import Divider from '@mui/material/Divider';
import Popover from '@mui/material/Popover';
import { StaticDatePicker } from '@mui/x-date-pickers/StaticDatePicker';
import { Dayjs } from 'dayjs';
import { memo, useState } from 'react';

type Props = {
  defaultDate?: Dayjs;
  onDateChanged: (date: Dayjs) => void;
  disablePastDaysSelection?: boolean;
};

const DayPicker = ({ defaultDate, onDateChanged, disablePastDaysSelection }: Props) => {
  const [date, setDate] = useState(defaultDate ?? startOfDay());
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);

  const handleChanged = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleChange = (date: Dayjs | null) => {
    const newDate = date ?? startOfDay();

    if (disablePastDaysSelection) {
      const today = startOfDay();

      if (newDate.isBefore(today)) {
        return;
      }
    }

    setDate(newDate);
    handleClose();

    onDateChanged(newDate);
  };

  return (
    <>
      <DefaultSelect
        displayEmpty
        open={false}
        onClick={handleChanged}
        size="small"
        renderValue={() => (
          <StackRow>
            <LeadIconTypography label="Date" startElement={<CalendarIcon />} />
            <Divider orientation="vertical" flexItem />
            <PushToRight />
            <SmallIconTypography label={toShortDateWithoutWeekDay(date)} />
          </StackRow>
        )}
        value=""
      />

      <Popover
        open={Boolean(anchorEl)}
        anchorEl={anchorEl}
        onClose={handleClose}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'left',
        }}
      >
        <StaticDatePicker slots={{ toolbar: EmptyCalendarToolbar }} slotProps={SimpleCalendarSlotProps} defaultValue={date} onChange={handleChange} />
      </Popover>
    </>
  );
};

export default memo(DayPicker);

import { SmallIconTypography } from '@/components/commons';
import { EmptyCalendarToolbar, SimpleCalendarSlotProps } from '@/components/generics';
import { ArrowDownIcon } from '@/components/icons';
import { startOfDay, toShortDate } from '@/libs/utils';
import Button from '@mui/material/Button';
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

  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
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
      <Button variant="text" color="inherit" onClick={handleClick} endIcon={<ArrowDownIcon />}>
        <SmallIconTypography label={toShortDate(date)} />
      </Button>
      <Popover
        open={Boolean(anchorEl)}
        anchorEl={anchorEl}
        onClose={handleClose}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'left',
        }}
      >
        <StaticDatePicker
          slots={{
            toolbar: EmptyCalendarToolbar,
          }}
          slotProps={SimpleCalendarSlotProps}
          defaultValue={date}
          onChange={handleChange}
        />
      </Popover>
    </>
  );
};

export default memo(DayPicker);

import Button from '@mui/material/Button';
import ClickAwayListener from '@mui/material/ClickAwayListener';
import Popover from '@mui/material/Popover';
import Typography from '@mui/material/Typography';
import { StaticDatePicker } from '@mui/x-date-pickers/StaticDatePicker';
import { EmptyCalendarToolbar, SimpleCalendarSlotProps } from '@repo/shared/components/generics';
import { ArrowDownIcon } from '@repo/shared/components/icons';
import { startOfDay, toShortDate } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { memo, useState } from 'react';

type Props = {
  defaultDate?: Dayjs;
  onDateChanged: (date: Dayjs) => void;
};

const DayPicker = ({ defaultDate, onDateChanged }: Props) => {
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

    setDate(newDate);
    handleClose();

    onDateChanged(newDate);
  };

  return (
    <ClickAwayListener onClickAway={handleClose}>
      <>
        <Button variant="text" color="inherit" onClick={handleClick} endIcon={<ArrowDownIcon />}>
          <Typography variant="h6">{toShortDate(date)}</Typography>
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
    </ClickAwayListener>
  );
};

export default memo(DayPicker);

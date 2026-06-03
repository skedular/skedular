'use client';

import { ArrowDownIcon, ArrowLeftIcon, ArrowRightIcon, TodayIcon } from '@/components/icons';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import Popover from '@mui/material/Popover';
import { styled } from '@mui/material/styles';
import { DateCalendar } from '@mui/x-date-pickers/DateCalendar';
import { PickerDay, PickerDayProps } from '@mui/x-date-pickers/PickerDay';
import { endOfWeek, isInSameMonth, isInSameWeek, isInSameYear, startOfWeek } from '@skedular/shared';
import { SmallIconTypography } from '@skedular/ui';
import { Dayjs } from 'dayjs';
import { memo, useState } from 'react';

interface CustomPickerDayProps extends PickerDayProps {
  isSelected: boolean;
  isHovered: boolean;
}

const CustomPickersDay = styled(PickerDay, {
  shouldForwardProp: (prop) => prop !== 'isSelected' && prop !== 'isHovered',
})<CustomPickerDayProps>(({ theme, isSelected, isHovered, day }) => ({
  borderRadius: 0,
  ...(isSelected && {
    backgroundColor: theme.palette.primary.main,
    color: theme.palette.primary.contrastText,
    '&:hover, &:focus': {
      backgroundColor: theme.palette.primary.main,
    },
  }),
  ...(isHovered && {
    backgroundColor: theme.palette.primary.light,
    '&:hover, &:focus': {
      backgroundColor: theme.palette.primary.light,
    },
    ...theme.applyStyles('dark', {
      backgroundColor: theme.palette.primary.dark,
      '&:hover, &:focus': {
        backgroundColor: theme.palette.primary.dark,
      },
    }),
  }),
  ...(day.day() === 0 && {
    borderTopLeftRadius: '50%',
    borderBottomLeftRadius: '50%',
  }),
  ...(day.day() === 6 && {
    borderTopRightRadius: '50%',
    borderBottomRightRadius: '50%',
  }),
})) as React.ComponentType<CustomPickerDayProps>;

const Day = (
  props: PickerDayProps & {
    selectedDay?: Dayjs | null;
    hoveredDay?: Dayjs | null;
  },
) => {
  const { day, selectedDay, hoveredDay, ...other } = props;

  return <CustomPickersDay {...other} day={day} sx={{ px: 2.5 }} selected={false} isSelected={isInSameWeek(day, selectedDay)} isHovered={isInSameWeek(day, hoveredDay)} />;
};

type Props = {
  defaultStartWeek?: Dayjs;
  onWeekChanged: (date: Dayjs) => void;
};

const WeekPicker = ({ defaultStartWeek, onWeekChanged }: Props) => {
  const [hoveredDay, setHoveredDay] = useState<Dayjs | null>(null);
  const [start, setStart] = useState(defaultStartWeek ?? startOfWeek());
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);

  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleChange = (date: Dayjs) => {
    const newStart = startOfWeek(date);

    setStart(newStart);
    handleClose();

    onWeekChanged(newStart);
  };

  const handleTodayClick = () => {
    const newStart = startOfWeek();

    setStart(newStart);
    onWeekChanged(newStart);
  };

  const handlePreviousWeekClick = () => {
    const newStart = start.add(-1, 'week');

    setStart(newStart);
    onWeekChanged(newStart);
  };

  const handleNextWeekClick = () => {
    const newStart = start.add(1, 'week');

    setStart(newStart);
    onWeekChanged(newStart);
  };

  let buttonTitle = '';
  const end = endOfWeek(start).add(-1, 'milliseconds');
  if (isInSameMonth(start, end)) {
    buttonTitle = `${start.date()}-${end.date()} ${end.format('MMM')}, ${end.format('YYYY')}`;
  } else if (isInSameYear(start, end)) {
    buttonTitle = `${start.date()} ${start.format('MMM')} - ${end.date()} ${end.format('MMM')}, ${end.format('YYYY')}`;
  } else {
    buttonTitle = `${start.date()} ${start.format('MMM')}, ${start.format('YYYY')} - ${end.date()} ${end.format('MMM')}, ${end.format('YYYY')}`;
  }

  return (
    <>
      <Button variant="outlined" color="inherit" onClick={handleTodayClick} startIcon={<TodayIcon />}>
        Today
      </Button>
      <IconButton color="inherit" onClick={handlePreviousWeekClick}>
        <ArrowLeftIcon />
      </IconButton>
      <IconButton color="inherit" onClick={handleNextWeekClick}>
        <ArrowRightIcon />
      </IconButton>
      <Button variant="text" color="inherit" onClick={handleClick} endIcon={<ArrowDownIcon />}>
        <SmallIconTypography label={buttonTitle} />
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
        <DateCalendar
          value={start}
          onChange={(newValue) => handleChange(newValue as Dayjs)}
          showDaysOutsideCurrentMonth
          slots={{ day: Day }}
          slotProps={{
            day: (ownerState) => ({
              selectedDay: start,
              hoveredDay,
              onPointerEnter: () => setHoveredDay(ownerState.day),
              onPointerLeave: () => setHoveredDay(null),
            }),
          }}
        />
      </Popover>
    </>
  );
};

export default memo(WeekPicker);

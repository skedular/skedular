import Divider from '@mui/material/Divider';
import Popover from '@mui/material/Popover';
import Select from '@mui/material/Select';
import Stack from '@mui/material/Stack';
import { styled } from '@mui/material/styles';
import Typography from '@mui/material/Typography';
import { DateCalendar } from '@mui/x-date-pickers/DateCalendar';
import { PickersDay, PickersDayProps } from '@mui/x-date-pickers/PickersDay';
import { CalendarIcon } from '@repo/shared/components/icons';
import { endOfWeek, isInSameMonth, isInSameWeek, isInSameYear, startOfWeek } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { memo, useState } from 'react';

interface CustomPickerDayProps extends PickersDayProps<Dayjs> {
  isSelected: boolean;
  isHovered: boolean;
}

const CustomPickersDay = styled(PickersDay, {
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
  props: PickersDayProps<Dayjs> & {
    selectedDay?: Dayjs | null;
    hoveredDay?: Dayjs | null;
  },
) => {
  const { day, selectedDay, hoveredDay, ...other } = props;

  return (
    <CustomPickersDay
      {...other}
      day={day}
      sx={{ px: 2.5 }}
      disableMargin
      selected={false}
      isSelected={isInSameWeek(day, selectedDay)}
      isHovered={isInSameWeek(day, hoveredDay)}
    />
  );
};

type Props = {
  defaultStartWeek?: Dayjs;
  onWeekChanged: (date: Dayjs) => void;
};

const WeekRangePicker = ({ defaultStartWeek, onWeekChanged }: Props) => {
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
      <Select
        displayEmpty
        open={false}
        onClick={handleClick}
        sx={{
          '& .MuiOutlinedInput-notchedOutline': {
            borderRadius: 4,
          },
          width: {
            xs: '100%',
            sm: 'min(100%, 250px)',
          },
        }}
        size="small"
        renderValue={() => (
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <CalendarIcon />
            <Typography variant="h6">Date</Typography>
            <Divider orientation="vertical" flexItem />
            <Typography variant="body1">{buttonTitle}</Typography>
          </Stack>
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
        <DateCalendar
          value={start}
          onChange={(newValue) => handleChange(newValue)}
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

export default memo(WeekRangePicker);

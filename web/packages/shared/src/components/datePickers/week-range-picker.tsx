import Divider from '@mui/material/Divider';
import Popover from '@mui/material/Popover';
import { styled } from '@mui/material/styles';
import { DateCalendar } from '@mui/x-date-pickers/DateCalendar';
import { PickersDay, PickersDayProps } from '@mui/x-date-pickers/PickersDay';
import { Dayjs } from 'dayjs';
import { memo, useState } from 'react';
import { endOfWeek, isInSameMonth, isInSameWeek, isInSameYear, startOfWeek } from '../../libs/utils';
import { LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '../commons';
import { CalendarIcon } from '../icons';
import { DefaultSelect } from '../styled';

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

  const handleChanged = (event: React.MouseEvent<HTMLElement>) => {
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
            <SmallIconTypography label={buttonTitle} />
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

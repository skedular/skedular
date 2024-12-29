import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import { DateRangePicker } from '@mui/x-date-pickers-pro/DateRangePicker';
import type { Dayjs } from 'dayjs';
import { memo, useState } from 'react';
import { startOfDay } from '../../libs/utils';
import { StackColumn } from '../commons';

export type Period = 'week' | 'month' | '3months' | 'custom';

type Props = {
  defaultPeriod: Period;
  defaultCustomFrom?: Dayjs;
  defaultCustomUntil?: Dayjs;
  onDateRangeChange: (from: Dayjs, until: Dayjs) => void;
};

const AnalyticsDaterangeSelector = ({ defaultPeriod, defaultCustomFrom, defaultCustomUntil, onDateRangeChange }: Props) => {
  const [until, setUntil] = useState(defaultPeriod === 'custom' && defaultCustomFrom && defaultCustomUntil ? defaultCustomUntil : startOfDay());
  const [from, setFrom] = useState(
    defaultPeriod === 'custom' && (!defaultCustomFrom || !defaultCustomUntil)
      ? until.subtract(1, 'months')
      : defaultPeriod === 'custom' && defaultCustomFrom && defaultCustomUntil
        ? defaultCustomFrom
        : defaultPeriod === 'week'
          ? until.subtract(1, 'weeks')
          : defaultPeriod === 'month'
            ? until.subtract(1, 'months')
            : until.subtract(3, 'months'),
  );
  const [period, setPeriod] = useState(defaultPeriod === 'custom' && (defaultCustomFrom || !defaultCustomUntil) ? 'month' : defaultPeriod);
  const handlePeriodChange = (event: React.MouseEvent<HTMLElement>, newPeriod: Period) => {
    const start = startOfDay();
    let until = start;
    let from = until.subtract(1, 'months');

    switch (newPeriod) {
      case 'week':
        until = start;
        from = start.subtract(1, 'weeks');

        break;

      case 'month':
        until = start;
        from = start.subtract(1, 'months');

        break;

      case '3months':
        until = start;
        from = start.subtract(3, 'months');

        break;
    }

    setFrom(from);
    setUntil(until);
    setPeriod(newPeriod);

    onDateRangeChange(from, until);
  };

  const handleSelectedDateChange = (from: Dayjs | null, until: Dayjs | null) => {
    if (!from || !until) {
      return;
    }

    setFrom(from);
    setUntil(until);
    onDateRangeChange(from, until);
  };

  return (
    <StackColumn>
      <ToggleButtonGroup color="primary" value={period} exclusive onChange={handlePeriodChange} size="small">
        <ToggleButton value="week">1 Week</ToggleButton>
        <ToggleButton value="month">1 Month</ToggleButton>
        <ToggleButton value="3months">3 Months</ToggleButton>
        <ToggleButton value="custom">Custom</ToggleButton>
      </ToggleButtonGroup>
      {period === 'custom' && (
        <DateRangePicker
          localeText={{ start: 'From', end: 'To' }}
          defaultValue={[from, until]}
          onChange={(dateRangeValue) => handleSelectedDateChange(dateRangeValue[0], dateRangeValue[1])}
        />
      )}
    </StackColumn>
  );
};

export default memo(AnalyticsDaterangeSelector);

'use client';

import { ClosedAllDayIcon, CustomOpeningHoursIcon, OpenAllDayIcon } from '@/components/icons';
import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import Tooltip from '@mui/material/Tooltip';
import { memo, useState } from 'react';

type Props = {
  defaultValue: 'closed' | 'openAllDay' | 'custom';
  onChange: (view: 'closed' | 'openAllDay' | 'custom') => void;
};

const ClosedOpenAllDayCustomToggle = ({ defaultValue, onChange }: Props) => {
  const [value, setValue] = useState<string>(defaultValue ?? 'openAllDay');

  const handleChange = (_: React.MouseEvent<HTMLElement>, newValue: string) => {
    if (!newValue) {
      return;
    }

    setValue(newValue);
    onChange(newValue as 'closed' | 'openAllDay' | 'custom');
  };

  return (
    <ToggleButtonGroup
      value={value}
      exclusive
      onChange={handleChange}
      sx={{
        borderRadius: 4,
      }}
    >
      <Tooltip title="Closed (Not open for business)">
        <ToggleButton value="closed">
          <ClosedAllDayIcon />
        </ToggleButton>
      </Tooltip>

      <Tooltip title="Open all day (No time restrictions)">
        <ToggleButton value="openAllDay">
          <OpenAllDayIcon />
        </ToggleButton>
      </Tooltip>

      <Tooltip title="Set specific opening hour">
        <ToggleButton value="custom">
          <CustomOpeningHoursIcon />
        </ToggleButton>
      </Tooltip>
    </ToggleButtonGroup>
  );
};

export default memo(ClosedOpenAllDayCustomToggle);

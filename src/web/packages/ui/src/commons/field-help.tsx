'use client';

import HelpOutlineRoundedIcon from '@mui/icons-material/HelpOutlineRounded';
import IconButton from '@mui/material/IconButton';
import Popover from '@mui/material/Popover';
import Typography from '@mui/material/Typography';
import { useState } from 'react';
import type { ReactNode } from 'react';

type Props = {
  label: string;
  children: ReactNode;
};

const FieldHelp = ({ label, children }: Props) => {
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);

  return (
    <>
      <IconButton
        size="small"
        aria-label={`Help for ${label}`}
        onClick={(event) => {
          event.stopPropagation();
          setAnchorEl(event.currentTarget);
        }}
        sx={{ p: 0.25, color: 'text.secondary' }}
      >
        <HelpOutlineRoundedIcon sx={{ fontSize: 17 }} />
      </IconButton>
      <Popover
        open={Boolean(anchorEl)}
        anchorEl={anchorEl}
        onClose={() => setAnchorEl(null)}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'left' }}
        transformOrigin={{ vertical: 'top', horizontal: 'left' }}
        slotProps={{ paper: { sx: { maxWidth: 360, p: 2, borderRadius: 2.5 } } }}
      >
        <Typography variant="subtitle2" sx={{ mb: 0.5 }}>
          {label}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {children}
        </Typography>
      </Popover>
    </>
  );
};

export default FieldHelp;

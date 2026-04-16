import Box from '@mui/material/Box';
import type { SxProps, Theme } from '@mui/system';
import type { ReactNode } from 'react';

type Props = {
  filters?: ReactNode;
  actions?: ReactNode;
  sx?: SxProps<Theme>;
};

const baseSurfaceSx: SxProps<Theme> = {
  borderRadius: 4,
  border: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.88)' : theme.palette.background.paper),
  boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : '0 1px 3px rgba(0, 0, 0, 0.24)'),
};

const CollectionToolbar = ({ filters, actions, sx }: Props) => {
  if (!filters && !actions) {
    return null;
  }

  return (
    <Box
      sx={[
        baseSurfaceSx,
        {
          px: 2,
          py: 1.5,
          display: 'grid',
          gridTemplateColumns: { xs: '1fr', md: 'minmax(0, 1fr) auto' },
          alignItems: 'start',
          gap: 1.5,
        },
        ...(sx != null ? (Array.isArray(sx) ? sx : [sx]) : []),
      ]}
    >
      <Box sx={{ minWidth: 0 }}>{filters}</Box>
      {actions ? (
        <Box
          sx={{
            minWidth: 0,
            justifySelf: { xs: 'stretch', md: 'end' },
            alignSelf: 'start',
            display: 'flex',
            flexWrap: 'wrap',
            gap: 1,
          }}
        >
          {actions}
        </Box>
      ) : null}
    </Box>
  );
};

export default CollectionToolbar;

import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import type { ReactNode } from 'react';

type Props = {
  asideTitle?: string;
  asideDescription?: string;
  asideChildren?: ReactNode;
  mainTitle?: string;
  mainDescription?: string;
  children: ReactNode;
};

const SetupSplitLayout = ({ asideTitle, asideDescription, asideChildren, mainTitle, mainDescription, children }: Props) => (
  <Box
    sx={{
      display: 'grid',
      gridTemplateColumns: { xs: '1fr', lg: 'minmax(280px, 360px) minmax(0, 1fr)' },
      gap: 3,
      alignItems: 'start',
    }}
  >
    <Box
      sx={{
        display: 'grid',
        gap: 2,
        px: 3,
        py: 3,
        borderRadius: 5,
        background: (theme) =>
          theme.palette.mode === 'dark' ? 'linear-gradient(180deg, rgba(18,18,18,0.98) 0%, rgba(30,64,55,0.92) 100%)' : 'linear-gradient(180deg, #4EBE73 0%, #8573E6 100%)',
        color: 'common.white',
      }}
    >
      {(asideTitle || asideDescription) && (
        <Stack spacing={1}>
          {asideTitle && (
            <Typography variant="h5" sx={{ fontWeight: 700, color: 'inherit' }}>
              {asideTitle}
            </Typography>
          )}
          {asideDescription && (
            <Typography variant="body1" sx={{ color: 'rgba(255,255,255,0.86)' }}>
              {asideDescription}
            </Typography>
          )}
        </Stack>
      )}
      {asideChildren}
    </Box>

    <Box sx={{ display: 'grid', gap: 2, minWidth: 0 }}>
      {(mainTitle || mainDescription) && (
        <Stack spacing={1}>
          {mainTitle && (
            <Typography variant="h4" sx={{ fontWeight: 700 }}>
              {mainTitle}
            </Typography>
          )}
          {mainDescription && (
            <Typography variant="body1" color="text.secondary">
              {mainDescription}
            </Typography>
          )}
        </Stack>
      )}
      {children}
    </Box>
  </Box>
);

export default SetupSplitLayout;

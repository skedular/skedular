'use client';

import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import type { ReactNode } from 'react';
import { StackColumn } from './index';

type Props = {
  asideTitle?: string;
  asideDescription?: string;
  asideChildren?: ReactNode;
  mainTitle?: string;
  mainDescription?: string;
  children: ReactNode;
};

const SetupSplitLayout = ({ asideTitle, asideDescription, asideChildren, mainTitle, mainDescription, children }: Props) => (
  <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 2, sm: 3, md: 4 }, py: { xs: 2, sm: 3, md: 3 } }}>
    <Box
      sx={{
        width: '100%',
        maxWidth: 1200,
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
          <StackColumn spacing={1}>
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
          </StackColumn>
        )}
        {asideChildren}
      </Box>

      <Box sx={{ display: 'grid', gap: 2, minWidth: 0 }}>
        {(mainTitle || mainDescription) && (
          <StackColumn spacing={1}>
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
          </StackColumn>
        )}
        {children}
      </Box>
    </Box>
  </Box>
);

export default SetupSplitLayout;

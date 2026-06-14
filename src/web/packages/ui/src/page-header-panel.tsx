'use client';

import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import type { SxProps, Theme } from '@mui/system';
import type { PropsWithChildren, ReactNode } from 'react';
import StackColumn from './stack-column';
import StackRow from './stack-row';

type Props = {
  title: ReactNode;
  description?: ReactNode;
  eyebrow?: ReactNode;
  actions?: ReactNode;
  sx?: SxProps<Theme>;
};

const PageHeaderPanel = ({ title, description, eyebrow, actions, sx, children }: PropsWithChildren<Props>) => (
  <Card
    variant="outlined"
    sx={[
      {
        borderRadius: 4,
        overflow: 'hidden',
        borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
        boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : theme.shadows[1]),
        backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.88)' : theme.palette.background.paper),
      },
      ...(sx != null ? (Array.isArray(sx) ? sx : [sx]) : []),
    ]}
  >
    <CardContent sx={{ p: 2 }}>
      <StackColumn spacing={2}>
        <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ justifyContent: 'space-between', alignItems: { xs: 'flex-start', md: 'center' } }}>
          <StackColumn spacing={0.75} sx={{ minWidth: 0 }}>
            {eyebrow ? (
              typeof eyebrow === 'string' ? (
                <Chip label={eyebrow} size="small" sx={{ alignSelf: 'flex-start', textTransform: 'uppercase', letterSpacing: '0.04em' }} />
              ) : (
                eyebrow
              )
            ) : null}
            <Typography variant="h6">{title}</Typography>
            {description ? (
              <Typography variant="body2" sx={{ opacity: 0.78 }}>
                {description}
              </Typography>
            ) : null}
          </StackColumn>
          {actions ? (
            <StackRow spacing={1} sx={{ flexWrap: 'wrap', justifyContent: { xs: 'flex-start', md: 'flex-end' } }}>
              {actions}
            </StackRow>
          ) : null}
        </Stack>
        {children != null ? (
          <Box>
            {typeof children === 'string' || typeof children === 'number' ? (
              <Typography variant="body2" sx={{ opacity: 0.72 }}>
                {children}
              </Typography>
            ) : (
              children
            )}
          </Box>
        ) : null}
      </StackColumn>
    </CardContent>
  </Card>
);

export default PageHeaderPanel;

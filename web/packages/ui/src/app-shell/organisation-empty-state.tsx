'use client';

import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import type { SxProps, Theme } from '@mui/system';
import type { ReactNode } from 'react';
import BodyIconTypography from '../typography/body-icon-typography';
import MediumHeadingIconTypography from '../typography/medium-heading-icon-typography';

type Props = {
  title: ReactNode;
  description: ReactNode;
  actionLabel?: ReactNode;
  actionHref?: string;
  sx?: SxProps<Theme>;
};

const OrganisationEmptyState = ({ title, description, actionLabel, actionHref, sx }: Props) => (
  <Box
    sx={[
      {
        border: '1px dashed',
        borderColor: 'divider',
        borderRadius: 2,
        p: { xs: 2, md: 3 },
        bgcolor: 'background.paper',
      },
      ...(sx != null ? (Array.isArray(sx) ? sx : [sx]) : []),
    ]}
  >
    <Stack spacing={1.5} sx={{ alignItems: 'flex-start' }}>
      <MediumHeadingIconTypography label={title} />
      <BodyIconTypography label={description} sx={{ color: 'text.secondary' }} />
      {actionLabel ? (
        <Button href={actionHref} variant="outlined" size="small">
          {actionLabel}
        </Button>
      ) : null}
    </Stack>
  </Box>
);

export default OrganisationEmptyState;

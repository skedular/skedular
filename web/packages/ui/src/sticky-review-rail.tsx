import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import type { SxProps, Theme } from '@mui/system';
import type { PropsWithChildren, ReactNode } from 'react';

type Props = {
  title: ReactNode;
  description?: ReactNode;
  top?: number;
  sx?: SxProps<Theme>;
};

const StickyReviewRail = ({ title, description, top = 24, sx, children }: PropsWithChildren<Props>) => (
  <Stack
    spacing={2}
    sx={[
      {
        pl: { xs: 2, xl: 0 },
        pr: 2,
        pt: 2,
        position: { xl: 'sticky' },
        top: { xl: top },
        alignSelf: 'start',
      },
      ...(sx != null ? (Array.isArray(sx) ? sx : [sx]) : []),
    ]}
  >
    <Box>
      <Typography variant="subtitle1">{title}</Typography>
      {description ? (
        <Typography variant="body2" sx={{ opacity: 0.75, mt: 0.5 }}>
          {description}
        </Typography>
      ) : null}
    </Box>
    {children}
  </Stack>
);

export default StickyReviewRail;

import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Divider from '@mui/material/Divider';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import type { SxProps, Theme } from '@mui/system';
import type { PropsWithChildren, ReactNode } from 'react';

type Props = {
  title: ReactNode;
  description?: ReactNode;
  sx?: SxProps<Theme>;
};

const PageSectionCard = ({ title, description, sx, children }: PropsWithChildren<Props>) => (
  <Card
    variant="outlined"
    sx={[
      {
        borderRadius: 4,
        borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
        boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : theme.shadows[1]),
        backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.88)' : theme.palette.background.paper),
      },
      ...(sx != null ? (Array.isArray(sx) ? sx : [sx]) : []),
    ]}
  >
    <CardContent sx={{ p: 2 }}>
      <Stack spacing={2}>
        <Stack spacing={0.5}>
          <Typography variant="subtitle1">{title}</Typography>
          {description ? (
            <Typography variant="body2" sx={{ opacity: 0.8 }}>
              {description}
            </Typography>
          ) : null}
        </Stack>
        <Divider />
        {children}
      </Stack>
    </CardContent>
  </Card>
);

export default PageSectionCard;

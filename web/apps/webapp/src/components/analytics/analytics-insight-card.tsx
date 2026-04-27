import { SectionIconTypography } from '@skedular/ui';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import type { ReactNode } from 'react';

type Props = {
  title: string;
  children: ReactNode;
};

const AnalyticsInsightCard = ({ title, children }: Props) => {
  return (
    <Card
      sx={{
        width: '100%',
        maxWidth: 500,
        height: '100%',
        borderRadius: 4,
        border: 1,
        borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
        boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 10px 28px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
        backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.92)' : theme.palette.background.paper),
      }}
    >
      <CardContent sx={{ p: 0, height: '100%', display: 'flex', flexDirection: 'column' }}>
        <Box
          sx={{
            px: 2.5,
            py: 2,
            borderBottom: 1,
            borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
            backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.02)' : theme.palette.action.hover),
          }}
        >
          <SectionIconTypography label={title} />
        </Box>

        <Box sx={{ p: 2.5, display: 'flex', flexDirection: 'column', gap: 2, flexGrow: 1 }}>{children}</Box>
      </CardContent>
    </Card>
  );
};

export default AnalyticsInsightCard;

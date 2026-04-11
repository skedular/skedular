import Card from '@mui/material/Card';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import type { ReactNode } from 'react';

type Props = {
  icon?: ReactNode;
  title: string;
  description: string;
};

const SetupFeatureCard = ({ icon, title, description }: Props) => (
  <Card
    sx={{
      backgroundColor: 'rgba(255,255,255,0.12)',
      color: 'common.white',
      px: 2,
      py: 1.75,
      borderRadius: 3,
      border: '1px solid rgba(255,255,255,0.12)',
      boxShadow: 'none',
    }}
  >
    <Stack direction="row" spacing={1.5} sx={{ alignItems: 'flex-start' }}>
      {icon}
      <Stack spacing={0.5} sx={{ minWidth: 0 }}>
        <Typography variant="subtitle1" sx={{ fontWeight: 700, color: 'inherit' }}>
          {title}
        </Typography>
        <Typography variant="body2" sx={{ color: 'rgba(255,255,255,0.82)' }}>
          {description}
        </Typography>
      </Stack>
    </Stack>
  </Card>
);

export default SetupFeatureCard;

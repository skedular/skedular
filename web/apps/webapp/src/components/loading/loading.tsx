import { LeadIconTypography } from '@/components/commons';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import LinearProgress from '@mui/material/LinearProgress';
import { memo } from 'react';

const indicatorSize = 80;

interface Props {
  message?: string;
}

const Loading = ({ message }: Props) => {
  if (!message) {
    return <LinearProgress />;
  }

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}>
      <CircularProgress size={indicatorSize} />
      {message && <LeadIconTypography label={message} />}
    </Box>
  );
};

export default memo(Loading);

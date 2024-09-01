import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import Typography from '@mui/material/Typography';
import { memo } from 'react';

const indicatorSize = 80;

interface Props {
  message?: string;
}

const Loading = ({ message }: Props) => {
  return (
    <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
      <CircularProgress size={indicatorSize} />
      {message && <Typography variant="h6">{message}</Typography>}
    </Box>
  );
};

export default memo(Loading);

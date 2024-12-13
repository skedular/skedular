import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import { memo } from 'react';
import { LeadIconTypography } from '../commons';

const indicatorSize = 80;

interface Props {
  message?: string;
}

const Loading = ({ message }: Props) => {
  return (
    <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
      <CircularProgress size={indicatorSize} />
      {message && <LeadIconTypography label={message} />}
    </Box>
  );
};

export default memo(Loading);

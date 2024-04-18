import { RefreshIcon } from '@repo/shared/components/icons';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import { memo } from 'react';

export interface Error {
  message: string;
}

export interface Source {
  errors?: Error[];
}

export interface RootError {
  message: string;
  source?: Source;
}

interface Props {
  error: RootError;
}

const RelayError = ({ error }: Props) => {
  const handleRefreshClicked = () => {
    window.location.reload();
  };

  return (
    <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
      <Typography variant="h2">Following error occurred while fetching the data, please refresh the page</Typography>
      <Button variant="contained" startIcon={<RefreshIcon />} onClick={handleRefreshClicked}>
        Refresh
      </Button>

      {error?.source?.errors &&
        error.source.errors.map((item, index) => (
          <Typography variant="h4" key={index}>
            {item.message}
          </Typography>
        ))}

      {!error?.source?.errors && <Typography variant="h4">{error.message}</Typography>}
    </Box>
  );
};

export default memo(RelayError);

import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { memo } from 'react';
import { BodyIconTypography, MediumHeadingIconTypography } from '../commons';
import { RefreshIcon } from '../icons';

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
      <MediumHeadingIconTypography label="Following error occurred while fetching the data, please refresh the page" />
      {error?.source?.errors && error.source.errors.map((item, index) => <BodyIconTypography key={index} label={item.message} />)}
      {!error?.source?.errors && <BodyIconTypography label={error.message} />}
      <Button variant="contained" startIcon={<RefreshIcon />} onClick={handleRefreshClicked}>
        Refresh
      </Button>
    </Box>
  );
};

export default memo(RelayError);

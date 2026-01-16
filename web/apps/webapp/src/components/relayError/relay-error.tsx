import { BodyIconTypography, MediumHeadingIconTypography } from '@/components/commons';
import { RefreshIcon } from '@/components/icons';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { memo } from 'react';
import type { FallbackProps } from 'react-error-boundary';

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

export const toRootError = (error: FallbackProps['error']): RootError => {
  if (typeof error === 'object' && error !== null && 'message' in error) {
    const typedError = error as { message?: string; source?: RootError['source'] };
    return {
      message: typedError.message ?? 'Unknown error',
      source: typedError.source,
    };
  }

  return { message: typeof error === 'string' ? error : 'Unknown error' };
};

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

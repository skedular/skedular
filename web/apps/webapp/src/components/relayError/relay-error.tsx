'use client';

import { BodyIconTypography, MediumHeadingIconTypography, SmallHeadingIconTypography, SmallIconTypography } from '@/components/commons';
import { ErrorIcon, HomeIcon, RefreshIcon } from '@/components/icons';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Collapse from '@mui/material/Collapse';
import Divider from '@mui/material/Divider';
import Stack from '@mui/material/Stack';
import { memo, useMemo, useState } from 'react';
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

const DEFAULT_ERROR_MESSAGE = 'We could not load this page right now.';
const DEFAULT_HELPER_MESSAGE = 'Please try again. If the problem continues, refresh the page or come back in a moment.';

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
  const [showDetails, setShowDetails] = useState(false);

  const detailMessages = useMemo(() => {
    const messages = error?.source?.errors?.map((item) => item.message).filter(Boolean) ?? [];
    if (messages.length > 0) {
      return [...new Set(messages)];
    }

    return error?.message ? [error.message] : [];
  }, [error]);

  const handleRefreshClicked = () => {
    window.location.reload();
  };

  const handleGoHomeClicked = () => {
    window.location.assign('/');
  };

  const handleGoBackClicked = () => {
    if (window.history.length > 1) {
      window.history.back();
      return;
    }

    handleGoHomeClicked();
  };

  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: '100vh',
        p: { xs: 2, sm: 4 },
        background: 'linear-gradient(180deg, rgba(18,52,88,0.04) 0%, rgba(18,52,88,0.08) 100%)',
      }}
    >
      <Card
        elevation={0}
        sx={{
          width: '100%',
          maxWidth: 720,
          borderRadius: 4,
          border: '1px solid',
          borderColor: 'divider',
          overflow: 'hidden',
        }}
      >
        <CardContent sx={{ padding: { xs: 3, sm: 5 } }}>
          <Stack spacing={3}>
            <Stack spacing={2} sx={{ alignItems: 'flex-start' }}>
              <Chip icon={<ErrorIcon color="error" />} label="Error" color="error" variant="outlined" sx={{ borderRadius: 2 }} />
              <MediumHeadingIconTypography label="Something went wrong" />
              <BodyIconTypography label={DEFAULT_ERROR_MESSAGE} sx={{ maxWidth: 560 }} />
              <SmallIconTypography label={DEFAULT_HELPER_MESSAGE} sx={{ maxWidth: 560 }} />
            </Stack>

            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5}>
              <Button variant="contained" startIcon={<RefreshIcon />} onClick={handleRefreshClicked}>
                Try Again
              </Button>
              <Button variant="outlined" startIcon={<HomeIcon />} onClick={handleGoHomeClicked}>
                Go Home
              </Button>
              <Button variant="text" onClick={handleGoBackClicked}>
                Go Back
              </Button>
            </Stack>

            {detailMessages.length > 0 && (
              <>
                <Divider />
                <Stack spacing={1.5}>
                  <Button variant="text" sx={{ alignSelf: 'flex-start', paddingLeft: 0, paddingRight: 0 }} onClick={() => setShowDetails((current) => !current)}>
                    {showDetails ? 'Hide details' : 'Show details'}
                  </Button>
                  <Collapse in={showDetails}>
                    <Stack
                      spacing={1.5}
                      sx={{
                        padding: 2,
                        borderRadius: 3,
                        backgroundColor: 'grey.50',
                        border: '1px solid',
                        borderColor: 'divider',
                      }}
                    >
                      <SmallHeadingIconTypography label="Error details" />
                      {detailMessages.map((message, index) => (
                        <BodyIconTypography key={`${message}-${index}`} label={message} />
                      ))}
                    </Stack>
                  </Collapse>
                </Stack>
              </>
            )}
          </Stack>
        </CardContent>
      </Card>
    </Box>
  );
};

export default memo(RelayError);

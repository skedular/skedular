'use client';

import GoogleIcon from '@mui/icons-material/Google';
import WindowIcon from '@mui/icons-material/Window';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import TextField from '@mui/material/TextField';
import type { SxProps } from '@mui/material/styles';
import { alpha, type Theme } from '@mui/material/styles';
import { BodyIconTypography, LargeHeadingIconTypography, SmallIconTypography } from '@skedular/ui';
import NextLink from 'next/link';
import { memo } from 'react';

type Props = {
  mode: 'sign-in' | 'sign-up';
  organizationName?: string | null;
  organizationLogoUrl?: string | null;
  organizationFeatureImageUrl?: string | null;
  returnTo?: string;
  error?: string | null;
};

const authCopy = {
  'sign-in': {
    title: 'Sign in',
    subtitle: 'Access your bookings, subscriptions, and workspace account.',
    primaryAction: 'Sign in',
    alternateLabel: 'Need an account?',
    alternateAction: 'Create account',
    alternateHref: '/auth/signup',
    formAction: '/api/auth/password/signin',
  },
  'sign-up': {
    title: 'Create account',
    subtitle: 'Join this workspace to book resources and manage your visits.',
    primaryAction: 'Create account',
    alternateLabel: 'Already have an account?',
    alternateAction: 'Sign in',
    alternateHref: '/auth/signin',
    formAction: '/api/auth/password/signup',
  },
};

const errorMessages: Record<string, string> = {
  create_account_failed: 'We could not create that account. Try signing in, or use another email address.',
  invalid_credentials: 'The email or password is incorrect.',
  missing_credentials: 'Enter your email and password.',
  oauth_failed: 'The social sign in could not be completed.',
  oauth_state_invalid: 'The sign in session expired. Try again.',
  password_mismatch: 'Passwords do not match.',
  unsupported_provider: 'That sign in provider is not available.',
};

const socialAuthButtonSx: SxProps<Theme> = {
  justifyContent: 'flex-start',
  textTransform: 'none',
  color: (theme) => (theme.palette.mode === 'dark' ? 'grey.50' : 'grey.900'),
  borderColor: (theme) => (theme.palette.mode === 'dark' ? 'grey.700' : 'grey.300'),
  bgcolor: (theme) => (theme.palette.mode === 'dark' ? 'grey.900' : 'common.white'),
  fontWeight: 600,
  '&:hover': {
    borderColor: (theme) => (theme.palette.mode === 'dark' ? 'grey.500' : 'grey.600'),
    bgcolor: (theme) => (theme.palette.mode === 'dark' ? 'grey.800' : 'grey.50'),
  },
  '&:focus-visible': {
    outline: '3px solid',
    outlineColor: (theme) => (theme.palette.mode === 'dark' ? 'primary.light' : 'primary.main'),
    outlineOffset: 2,
  },
};

const CustomOrganizationAuthPage = ({ mode, organizationName, organizationLogoUrl, organizationFeatureImageUrl, returnTo, error }: Props) => {
  const copy = authCopy[mode];
  const searchParams = new URLSearchParams();
  if (returnTo?.startsWith('/')) {
    searchParams.set('returnTo', returnTo);
  }
  const alternateHref = `${copy.alternateHref}${searchParams.toString() ? `?${searchParams.toString()}` : ''}`;
  const providerQuery = new URLSearchParams(searchParams);
  providerQuery.set('mode', mode);
  const providerSuffix = providerQuery.toString() ? `?${providerQuery.toString()}` : '';

  return (
    <Box
      sx={{
        minHeight: '100vh',
        bgcolor: (theme) => (theme.palette.mode === 'dark' ? 'grey.900' : 'grey.50'),
        display: 'grid',
        gridTemplateColumns: { xs: '1fr', md: 'minmax(360px, 520px) 1fr' },
      }}
    >
      <Box
        sx={{
          px: { xs: 3, sm: 5, md: 7 },
          py: { xs: 5, md: 8 },
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'center',
          borderRight: { md: 1 },
          borderColor: 'divider',
          bgcolor: (theme) => theme.palette.background.paper,
        }}
      >
        <Stack spacing={4} sx={{ width: '100%', maxWidth: 420 }}>
          <Stack spacing={2}>
            <Box
              sx={{
                width: 88,
                height: 88,
                borderRadius: 2,
                border: 1,
                borderColor: 'divider',
                bgcolor: (theme) => alpha(theme.palette.primary.main, 0.06),
                display: 'grid',
                placeItems: 'center',
                overflow: 'hidden',
                p: 1.25,
              }}
            >
              {organizationLogoUrl ? (
                // eslint-disable-next-line @next/next/no-img-element
                <img src={organizationLogoUrl} alt={`${organizationName ?? 'Organization'} logo`} style={{ maxWidth: '100%', maxHeight: '100%', objectFit: 'contain' }} />
              ) : (
                <SmallIconTypography label={(organizationName ?? 'Skedular').slice(0, 2).toUpperCase()} />
              )}
            </Box>

            <Stack spacing={1}>
              <SmallIconTypography label={organizationName ?? 'Skedular'} />
              <LargeHeadingIconTypography label={copy.title} />
              <BodyIconTypography label={copy.subtitle} />
            </Stack>
          </Stack>

          <Box component="form" action={copy.formAction} method="post">
            <Stack spacing={2.5}>
              {returnTo?.startsWith('/') ? <input type="hidden" name="returnTo" value={returnTo} /> : null}
              {error ? <BodyIconTypography label={errorMessages[error] ?? 'Authentication could not be completed.'} color="error" /> : null}
              <TextField type="email" name="email" label="Email" autoComplete="email" required fullWidth />
              <TextField type="password" name="password" label="Password" autoComplete={mode === 'sign-up' ? 'new-password' : 'current-password'} required fullWidth />
              {mode === 'sign-up' ? <TextField type="password" name="confirmPassword" label="Confirm password" autoComplete="new-password" required fullWidth /> : null}
              <Button type="submit" variant="contained" size="large" sx={{ textTransform: 'none', alignSelf: 'flex-start' }}>
                {copy.primaryAction}
              </Button>
            </Stack>
          </Box>

          <Divider>
            <SmallIconTypography label="or" />
          </Divider>

          <Stack spacing={1.5}>
            <Button href={`/api/auth/oauth/google/start${providerSuffix}`} variant="outlined" size="large" startIcon={<GoogleIcon />} sx={socialAuthButtonSx}>
              Continue with Google
            </Button>
            <Button href={`/api/auth/oauth/microsoft/start${providerSuffix}`} variant="outlined" size="large" startIcon={<WindowIcon />} sx={socialAuthButtonSx}>
              Continue with Microsoft
            </Button>
          </Stack>

          <BodyIconTypography
            label={
              <>
                {copy.alternateLabel}{' '}
                <Link component={NextLink} href={alternateHref}>
                  {copy.alternateAction}
                </Link>
              </>
            }
          />
        </Stack>
      </Box>

      <Box
        sx={{
          display: { xs: 'none', md: 'flex' },
          alignItems: 'center',
          px: 10,
          position: 'relative',
          overflow: 'hidden',
          bgcolor: (theme) => alpha(theme.palette.primary.main, theme.palette.mode === 'dark' ? 0.16 : 0.08),
          ...(organizationFeatureImageUrl
            ? {
                backgroundImage: `linear-gradient(90deg, rgba(15, 23, 42, 0.72), rgba(15, 23, 42, 0.34)), url(${organizationFeatureImageUrl})`,
                backgroundPosition: 'center',
                backgroundSize: 'cover',
              }
            : {}),
        }}
      >
        <Stack spacing={2} sx={{ maxWidth: 560, position: 'relative', zIndex: 1 }}>
          <LargeHeadingIconTypography
            label={organizationName ? `Welcome to ${organizationName}` : 'Welcome to Skedular'}
            sx={organizationFeatureImageUrl ? { color: 'common.white' } : undefined}
          />
          <BodyIconTypography
            label="Book spaces, manage schedules, and keep your workspace activity in one place."
            sx={organizationFeatureImageUrl ? { color: 'common.white' } : undefined}
          />
        </Stack>
      </Box>
    </Box>
  );
};

export default memo(CustomOrganizationAuthPage);

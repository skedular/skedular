'use client';

import Box from '@mui/material/Box';
import Chip from '@mui/material/Chip';
import Container from '@mui/material/Container';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import Image from 'next/image';
import type { SxProps, Theme } from '@mui/system';
import type { ReactNode } from 'react';
import BodyIconTypography from '../typography/body-icon-typography';
import LargeHeadingIconTypography from '../typography/large-heading-icon-typography';
import SmallIconTypography from '../typography/small-icon-typography';
import AppReviewBanner from './app-review-banner';

export type AppShellNavigationItem = {
  label: string;
  href: string;
  disabled?: boolean;
};

type Props = {
  appName: string;
  title: ReactNode;
  description: ReactNode;
  navigationItems?: readonly AppShellNavigationItem[];
  reviewNote?: ReactNode;
  children: ReactNode;
  sx?: SxProps<Theme>;
};

const AppShellLayout = ({ appName, title, description, navigationItems = [], reviewNote, children, sx }: Props) => (
  <Box
    component="main"
    sx={[
      {
        minHeight: '100dvh',
        bgcolor: 'background.default',
        color: 'text.primary',
      },
      ...(sx != null ? (Array.isArray(sx) ? sx : [sx]) : []),
    ]}
  >
    <Box
      component="header"
      sx={{
        borderBottom: 1,
        borderColor: 'divider',
        bgcolor: 'background.paper',
      }}
    >
      <Container maxWidth={false} sx={{ maxWidth: 1700, py: 1.5 }}>
        <Stack direction="row" spacing={2} sx={{ alignItems: 'center', justifyContent: 'space-between' }}>
          <Stack direction="row" spacing={1.5} sx={{ alignItems: 'center', minWidth: 0 }}>
            <Image src="/images/skedular-logo-primary.svg" alt="Skedular" width={132} height={32} priority />
            <Chip label={appName} size="small" sx={{ fontWeight: 600 }} />
          </Stack>
          {navigationItems.length > 0 ? (
            <Stack direction="row" spacing={1.5} useFlexGap sx={{ flexWrap: 'wrap', justifyContent: 'flex-end' }}>
              {navigationItems.map((item) => (
                <Link
                  key={`${item.href}-${item.label}`}
                  href={item.href}
                  underline="hover"
                  aria-disabled={item.disabled}
                  sx={{
                    pointerEvents: item.disabled ? 'none' : undefined,
                    color: item.disabled ? 'text.disabled' : 'primary.main',
                    fontWeight: 600,
                  }}
                >
                  <SmallIconTypography label={item.label} />
                </Link>
              ))}
            </Stack>
          ) : null}
        </Stack>
      </Container>
    </Box>
    <Container maxWidth={false} sx={{ maxWidth: 1700, py: { xs: 3, md: 4 } }}>
      <Stack spacing={3}>
        <Stack spacing={2}>
          <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ justifyContent: 'space-between', alignItems: { xs: 'flex-start', md: 'center' } }}>
            <Stack spacing={1} sx={{ minWidth: 0 }}>
              <LargeHeadingIconTypography label={title} />
              <BodyIconTypography label={description} sx={{ color: 'text.secondary', maxWidth: 760 }} />
            </Stack>
          </Stack>
          {reviewNote ? <AppReviewBanner title="Review checkpoint" description={reviewNote} /> : null}
        </Stack>
        {children}
      </Stack>
    </Container>
  </Box>
);

export default AppShellLayout;

'use client';

import DashboardLayout from '@/components/dashboard-layout/DashboardLayout';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import Stack from '@mui/material/Stack';
import { BodyIconTypography, MediumHeadingIconTypography } from '@skedular/ui';
import Link from 'next/link';

const NewBookingPage = () => (
  <DashboardLayout>
    <Container maxWidth="sm">
      <Stack spacing={3} sx={{ py: 8, textAlign: 'center', alignItems: 'center' }}>
        <MediumHeadingIconTypography label="Bookings are made by guests" />
        <BodyIconTypography label="Customers purchase your published products through the Skedular marketplace. New bookings and payout details appear on your Host dashboard." />
        <Button component={Link} href="/dashboard" variant="contained">
          View booking history
        </Button>
      </Stack>
    </Container>
  </DashboardLayout>
);

export default NewBookingPage;

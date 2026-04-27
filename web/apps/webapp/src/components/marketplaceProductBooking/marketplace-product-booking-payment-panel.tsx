import InvoiceDownloadLinks, { type InvoiceLinkItem } from '@/components/booking/invoice-download-links';
import { BodyIconTypography, CaptionIconTypography, SubtitleIconTypography } from '@skedular/ui';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CircularProgress from '@mui/material/CircularProgress';
import Stack from '@mui/material/Stack';
import dayjs from 'dayjs';
import { memo, useEffect, useState } from 'react';

type Props = {
  checkoutUrl: string | null;
  ctaLabel?: string;
  entityLabel?: string;
  invoices?: readonly InvoiceLinkItem[];
  invoiceUrl: string | null;
  isPaymentRequired: boolean;
  pendingStatusMessage?: string;
  paymentExpiry: string | null;
  paymentMethodType?: string | null;
  paymentStatusLabel: string;
  paymentStatusType: string | null;
};

const MarketplaceProductBookingPaymentPanel = ({
  checkoutUrl,
  ctaLabel = 'Pay now',
  entityLabel = 'Booking',
  invoices = [],
  invoiceUrl,
  isPaymentRequired,
  pendingStatusMessage,
  paymentExpiry,
  paymentMethodType,
  paymentStatusLabel,
  paymentStatusType,
}: Props) => {
  const supportsHostedCheckout = paymentMethodType === 'CARD';
  const isWaitingForCheckout = isPaymentRequired && paymentStatusType === 'PENDING' && supportsHostedCheckout && !checkoutUrl;
  const isWaitingForManualConfirmation = isPaymentRequired && paymentStatusType === 'PENDING' && !supportsHostedCheckout;
  const isSettled = paymentStatusType === 'CONFIRMED' || paymentStatusType === 'NO_PAYMENT_REQUIRED';
  const canPayNow = isPaymentRequired && paymentStatusType === 'PENDING' && supportsHostedCheckout && !!checkoutUrl;
  const showsPendingStatusMessage = paymentStatusType === 'PENDING' && !isWaitingForCheckout && !isWaitingForManualConfirmation && !canPayNow && !!pendingStatusMessage;
  const [, setCountdownTick] = useState(0);
  const timeLeftToPay = getTimeLeftToPay(paymentExpiry);

  useEffect(() => {
    if (!paymentExpiry) {
      return;
    }

    const interval = window.setInterval(() => {
      setCountdownTick((current) => current + 1);
    }, 1000);

    return () => window.clearInterval(interval);
  }, [paymentExpiry]);

  if (!paymentStatusType) {
    return null;
  }

  return (
    <Card sx={{ mt: 2.5, borderRadius: 3, border: 1, borderColor: (theme) => theme.palette.divider }}>
      <CardContent sx={{ p: 2.5 }}>
        <CaptionIconTypography label="Payment progress" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.68 }} />
        <SubtitleIconTypography label={isSettled ? `${entityLabel} ready` : `${entityLabel} created`} sx={{ mt: 1 }} />
        <BodyIconTypography
          label={
            isWaitingForCheckout
              ? 'We are preparing your payment link. This screen updates automatically when checkout is ready.'
              : isWaitingForManualConfirmation
                ? 'We are waiting for your payment to be confirmed. Come back to this page later or refresh it to see the latest status.'
                : showsPendingStatusMessage
                  ? pendingStatusMessage
                  : `Payment status: ${paymentStatusLabel}`
          }
          sx={{ mt: 1, opacity: 0.82 }}
        />
        {isPaymentRequired && paymentStatusType === 'PENDING' ? (
          <BodyIconTypography label={`Time left to pay: ${timeLeftToPay ?? 'Expired'}`} sx={{ mt: 1, color: 'error.main' }} />
        ) : null}

        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.25} sx={{ mt: 2 }}>
          {isWaitingForCheckout ? (
            <Button variant="outlined" disabled startIcon={<CircularProgress size={16} />}>
              Preparing checkout
            </Button>
          ) : null}

          {canPayNow ? (
            <Button variant="contained" href={checkoutUrl}>
              {ctaLabel}
            </Button>
          ) : null}

          {invoiceUrl || invoices.length > 0 ? <InvoiceDownloadLinks invoices={invoices} legacyInvoiceUrl={invoiceUrl} linkLabel="View invoice" size="body" /> : null}
        </Stack>
      </CardContent>
    </Card>
  );
};

const getTimeLeftToPay = (paymentExpiry: string | null) => {
  if (!paymentExpiry) {
    return null;
  }

  const expiryTime = dayjs(paymentExpiry).utc();
  const currentTime = dayjs().utc();
  if (expiryTime.isBefore(currentTime)) {
    return null;
  }

  const totalSeconds = expiryTime.diff(currentTime, 'second');
  if (totalSeconds > 24 * 60 * 60) {
    const totalDays = expiryTime.diff(currentTime, 'day');

    return `${totalDays} day(s) and ${new Date(totalSeconds * 1000).toISOString().slice(11, 19)}`;
  }

  return new Date(totalSeconds * 1000).toISOString().slice(11, 19);
};

export default memo(MarketplaceProductBookingPaymentPanel);

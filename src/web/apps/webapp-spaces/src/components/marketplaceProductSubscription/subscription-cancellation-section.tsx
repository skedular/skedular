import type { SupportedMarketplaceBookingSubscriptionCancellationModeDetails } from '@/components/marketplaceProductSubscription/marketplace-booking-subscription-cancellation-mode';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { alpha } from '@mui/material/styles';
import type { SxProps, Theme } from '@mui/system';
import { BodyIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@skedular/ui';

const cancellationButtonSx: SxProps<Theme> = {
  borderColor: (theme) => alpha(theme.palette.mode === 'dark' ? theme.palette.info.light : theme.palette.info.main, 0.45),
  color: (theme) => (theme.palette.mode === 'dark' ? theme.palette.info.light : theme.palette.info.dark),
  backgroundColor: (theme) => alpha(theme.palette.mode === 'dark' ? theme.palette.info.light : theme.palette.info.main, theme.palette.mode === 'dark' ? 0.12 : 0.08),
  fontWeight: 600,
  px: 1.75,
  textTransform: 'none',
  '&:hover': {
    borderColor: (theme) => (theme.palette.mode === 'dark' ? theme.palette.info.light : theme.palette.info.main),
    backgroundColor: (theme) => alpha(theme.palette.mode === 'dark' ? theme.palette.info.light : theme.palette.info.main, theme.palette.mode === 'dark' ? 0.18 : 0.14),
  },
};

type Props = {
  cancelAtPeriodEnd: boolean;
  isInFlight: boolean;
  hasConfirmedPayment: boolean;
  immediateCancellationMode: SupportedMarketplaceBookingSubscriptionCancellationModeDetails | null;
  atPeriodEndCancellationMode: SupportedMarketplaceBookingSubscriptionCancellationModeDetails | null;
  onImmediateCancellationClick: () => void;
  onAtPeriodEndCancellationClick: () => void;
};

const SubscriptionCancellationSection = ({
  cancelAtPeriodEnd,
  isInFlight,
  hasConfirmedPayment,
  immediateCancellationMode,
  atPeriodEndCancellationMode,
  onImmediateCancellationClick,
  onAtPeriodEndCancellationClick,
}: Props) => (
  <StackColumn spacing={1.25}>
    <Box>
      <SmallIconTypography label="Subscription actions" sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
      <SubtitleIconTypography label="Cancel subscription" sx={{ mt: 0.35 }} />
      <BodyIconTypography
        label={
          hasConfirmedPayment
            ? 'Choose how to stop future renewals. Ending at period end keeps the current period active. Immediate cancellation stops future billing now. Issued invoices stay on record, and any refund review is handled separately.'
            : 'Choose how to stop future renewals. Ending at period end keeps the current period active. Immediate cancellation stops future billing now. If payment for the current period was never confirmed, cancellation does not create a refund.'
        }
        sx={{ mt: 0.75, opacity: 0.82 }}
      />
    </Box>

    {cancelAtPeriodEnd ? (
      <Alert
        severity="info"
        sx={{ borderRadius: 2 }}
        action={
          immediateCancellationMode ? (
            <Button
              size="small"
              variant="outlined"
              disabled={isInFlight}
              onClick={onImmediateCancellationClick}
              aria-label="Cancel subscription now instead"
              sx={cancellationButtonSx}
            >
              Cancel now
            </Button>
          ) : undefined
        }
      >
        This subscription is already set to stop at the end of the current period.
      </Alert>
    ) : (
      <StackRow sx={{ rowGap: 1 }}>
        {atPeriodEndCancellationMode ? (
          <Button variant="outlined" disabled={isInFlight} onClick={onAtPeriodEndCancellationClick} aria-label="Cancel subscription at period end" sx={cancellationButtonSx}>
            Cancel at period end
          </Button>
        ) : null}
        {immediateCancellationMode ? (
          <Button variant="outlined" disabled={isInFlight} onClick={onImmediateCancellationClick} aria-label="Cancel subscription now" sx={cancellationButtonSx}>
            Cancel now
          </Button>
        ) : null}
      </StackRow>
    )}
  </StackColumn>
);

export default SubscriptionCancellationSection;

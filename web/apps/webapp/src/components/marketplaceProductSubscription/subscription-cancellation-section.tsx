import { BodyIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@/components/commons';
import type { SupportedMarketplaceBookingSubscriptionCancellationModeDetails } from '@/components/marketplaceProductSubscription/marketplace-booking-subscription-cancellation-mode';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';

type Props = {
  cancelAtPeriodEnd: boolean;
  isInFlight: boolean;
  immediateCancellationMode: SupportedMarketplaceBookingSubscriptionCancellationModeDetails | null;
  atPeriodEndCancellationMode: SupportedMarketplaceBookingSubscriptionCancellationModeDetails | null;
  onImmediateCancellationClick: () => void;
  onAtPeriodEndCancellationClick: () => void;
};

const SubscriptionCancellationSection = ({
  cancelAtPeriodEnd,
  isInFlight,
  immediateCancellationMode,
  atPeriodEndCancellationMode,
  onImmediateCancellationClick,
  onAtPeriodEndCancellationClick,
}: Props) => (
  <Box
    sx={{
      mt: 3,
      p: 2,
      borderRadius: 3,
      border: 1,
      borderColor: (theme) => theme.palette.divider,
      bgcolor: (theme) => theme.palette.action.hover,
    }}
  >
    <StackColumn spacing={1.25}>
      <Box>
        <SmallIconTypography label="Subscription actions" sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
        <SubtitleIconTypography label="Cancel subscription" sx={{ mt: 0.35 }} />
        <BodyIconTypography
          label="Choose how to stop future renewals. Ending at period end keeps the current period active. Immediate cancellation stops future billing now. Issued invoices stay on record."
          sx={{ mt: 0.75, opacity: 0.82 }}
        />
      </Box>

      {cancelAtPeriodEnd ? (
        <Alert
          severity="info"
          sx={{ borderRadius: 2 }}
          action={
            immediateCancellationMode ? (
              <Button size="small" color="error" disabled={isInFlight} onClick={onImmediateCancellationClick}>
                {immediateCancellationMode.name}
              </Button>
            ) : undefined
          }
        >
          This subscription is already set to stop at the end of the current period.
        </Alert>
      ) : (
        <StackRow sx={{ rowGap: 1 }}>
          {atPeriodEndCancellationMode ? (
            <Button variant="outlined" disabled={isInFlight} onClick={onAtPeriodEndCancellationClick}>
              {atPeriodEndCancellationMode.name}
            </Button>
          ) : null}
          {immediateCancellationMode ? (
            <Button color="error" variant="text" disabled={isInFlight} onClick={onImmediateCancellationClick}>
              {immediateCancellationMode.name}
            </Button>
          ) : null}
        </StackRow>
      )}
    </StackColumn>
  </Box>
);

export default SubscriptionCancellationSection;

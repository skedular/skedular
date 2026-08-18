import { BodyIconTypography, CaptionIconTypography, DefaultDialogTitle, StackColumn, TwoButtonsDialogActions } from '@skedular/ui';
import Box from '@mui/material/Box';
import LinearProgress from '@mui/material/LinearProgress';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import { graphql, useMutation } from 'react-relay';
import { memo, useState } from 'react';
import type { entitlementBalanceCard_setRenewalPolicyMutation } from '@/queries/__generated__/entitlementBalanceCard_setRenewalPolicyMutation.graphql';
import type { entitlementBalanceCard_cancelEntitlementMutation } from '@/queries/__generated__/entitlementBalanceCard_cancelEntitlementMutation.graphql';

export type EntitlementBalanceCardProps = {
  id: string;
  availableQuantity: number;
  grantedQuantity: number;
  expiresAt: string;
  currency?: string;
  refundAmount?: number | null;
  restrictions?: { availableDays: readonly string[]; minDurationMinutes?: number | null; maxDurationMinutes?: number | null; numberOfResourcesToBook: number } | null;
  renewalStatus?: string;
  nextRenewalAt?: string | null;
  renewalFailureReason?: string | null;
  autoRenew: boolean;
  cancelAtPeriodEnd: boolean;
};

const EntitlementBalanceCard = ({
  id,
  availableQuantity,
  grantedQuantity,
  expiresAt,
  currency,
  refundAmount,
  restrictions,
  renewalStatus,
  nextRenewalAt,
  renewalFailureReason,
  autoRenew,
  cancelAtPeriodEnd,
}: EntitlementBalanceCardProps) => {
  const [commit, inFlight] = useMutation<entitlementBalanceCard_setRenewalPolicyMutation>(graphql`
    mutation entitlementBalanceCard_setRenewalPolicyMutation($input: SetEntitlementRenewalPolicyInput!) {
      setEntitlementRenewalPolicy(input: $input) {
        entitlement {
          id
          autoRenew
          cancelAtPeriodEnd
          status
          nextRenewalAt
          renewalFailureReason
        }
        error
      }
    }
  `);
  const [error, setError] = useState<string | null>(null);
  const [showCancelDialog, setShowCancelDialog] = useState(false);
  const [cancelEntitlement, cancellationInFlight] = useMutation<entitlementBalanceCard_cancelEntitlementMutation>(graphql`
    mutation entitlementBalanceCard_cancelEntitlementMutation($input: CancelEntitlementInput!) {
      cancelEntitlement(input: $input) {
        entitlement {
          id
          status
          autoRenew
          cancelAtPeriodEnd
          nextRenewalAt
          renewalFailureReason
        }
        error
      }
    }
  `);
  const updatePolicy = (nextAutoRenew: boolean, nextCancelAtPeriodEnd: boolean) => {
    setError(null);
    commit({
      variables: { input: { clientMutationId: id, entitlementId: id, autoRenew: nextAutoRenew, cancelAtPeriodEnd: nextCancelAtPeriodEnd } },
      onCompleted: (response) => {
        if (response.setEntitlementRenewalPolicy.error) setError(response.setEntitlementRenewalPolicy.error);
      },
      onError: (err) => setError(err.message),
    });
  };
  const cancel = () => {
    setError(null);
    cancelEntitlement({
      variables: { input: { clientMutationId: id, entitlementId: id, reason: 'Operator cancelled entitlement.' } },
      onCompleted: (response) => {
        if (response.cancelEntitlement.error) setError(response.cancelEntitlement.error);
        else {
          setShowCancelDialog(false);
        }
      },
      onError: (err) => setError(err.message),
    });
  };
  const progress = grantedQuantity > 0 ? Math.max(0, Math.min(100, (availableQuantity / grantedQuantity) * 100)) : 0;
  return (
    <Box sx={{ p: 2, borderRadius: 3, border: 1, borderColor: 'divider' }}>
      <StackColumn spacing={1}>
        <CaptionIconTypography label="Booking credits" sx={{ textTransform: 'uppercase', opacity: 0.72 }} />
        <BodyIconTypography label={`${availableQuantity} credits remaining`} sx={{ fontWeight: 700 }} />
        <LinearProgress variant="determinate" value={progress} aria-label="Remaining booking credits" />
        <BodyIconTypography label={`Expires ${new Date(expiresAt).toLocaleDateString()}`} />
        {refundAmount != null && <BodyIconTypography label={`Refund amount: ${refundAmount} ${currency ?? ''}`} />}
        {restrictions?.availableDays.length ? <BodyIconTypography label={`Available days: ${restrictions.availableDays.join(', ')}`} /> : null}
        {restrictions?.minDurationMinutes != null && <BodyIconTypography label={`Minimum booking duration: ${restrictions.minDurationMinutes} minutes`} />}
        {restrictions?.maxDurationMinutes != null && <BodyIconTypography label={`Maximum booking duration: ${restrictions.maxDurationMinutes} minutes`} />}
        {restrictions && <BodyIconTypography label={`Resources per booking: ${restrictions.numberOfResourcesToBook}`} />}
        {renewalStatus && <BodyIconTypography label={`Renewal: ${renewalStatus.replaceAll('_', ' ').toLowerCase()}`} />}
        {nextRenewalAt && <BodyIconTypography label={`Next renewal ${new Date(nextRenewalAt).toLocaleDateString()}`} />}
        {renewalFailureReason && <BodyIconTypography label={`Renewal needs attention: ${renewalFailureReason}`} />}
        {error && <BodyIconTypography label={error} sx={{ color: 'error.main' }} />}
        <Button size="small" disabled={inFlight} onClick={() => updatePolicy(!autoRenew, false)}>
          {autoRenew ? 'Disable auto-renew' : 'Enable auto-renew'}
        </Button>
        {autoRenew && (
          <Button size="small" disabled={inFlight} onClick={() => updatePolicy(true, !cancelAtPeriodEnd)}>
            {cancelAtPeriodEnd ? 'Keep renewing' : 'Cancel at period end'}
          </Button>
        )}
        <Button color="error" size="small" disabled={inFlight || cancellationInFlight} onClick={() => setShowCancelDialog(true)}>
          Cancel entitlement
        </Button>
      </StackColumn>
      <Dialog open={showCancelDialog} onClose={() => setShowCancelDialog(false)}>
        <DefaultDialogTitle title="Cancel entitlement" />
        <DialogContent>
          <DialogContentText>Cancel this entitlement? Unused credits will be forfeited or refunded according to the product policy.</DialogContentText>
          <TwoButtonsDialogActions onPrimaryClicked={cancel} onSecondaryClicked={() => setShowCancelDialog(false)} primaryLabel="Confirm cancellation" secondaryLabel="Go back" />
        </DialogContent>
      </Dialog>
    </Box>
  );
};

export default memo(EntitlementBalanceCard);

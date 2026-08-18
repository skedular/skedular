import { errorNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import type { marketplaceRefundAdminPanel_approveMarketplaceRefundMutation } from '@/queries/__generated__/marketplaceRefundAdminPanel_approveMarketplaceRefundMutation.graphql';
import type { marketplaceRefundAdminPanel_cancelMarketplaceRefundMutation } from '@/queries/__generated__/marketplaceRefundAdminPanel_cancelMarketplaceRefundMutation.graphql';
import type { marketplaceRefundAdminPanel_rejectMarketplaceRefundMutation } from '@/queries/__generated__/marketplaceRefundAdminPanel_rejectMarketplaceRefundMutation.graphql';
import type { marketplaceRefundAdminPanel_resolveRefundReconciliationRequiredMutation } from '@/queries/__generated__/marketplaceRefundAdminPanel_resolveRefundReconciliationRequiredMutation.graphql';
import type { marketplaceRefundAdminPanel_retryMarketplaceRefundMutation } from '@/queries/__generated__/marketplaceRefundAdminPanel_retryMarketplaceRefundMutation.graphql';
import Alert from '@mui/material/Alert';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import TextField from '@mui/material/TextField';
import { getRelayErrorMessage, PaletteModeContext } from '@skedular/shared';
import { DefaultDialogTitle, StackRow, TwoButtonsDialogActions } from '@skedular/ui';
import { useContext, useState } from 'react';
import { graphql, useMutation } from 'react-relay';
import type { PayloadError } from 'relay-runtime';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import MarketplaceRefundTimeline from './marketplace-refund-timeline';
import { formatRefundAmount } from './refund-display';

type Props = {
  entityLabel: string;
  refund: {
    id: string;
    currency?: { type: string; name: string } | null;
    status: { type: string; name: string };
    refundAmount?: number | null | undefined;
    currencyToDisplay: string;
    reason?: string | null | undefined;
    lastError?: string | null | undefined;
    externalRefundNumber?: string | null | undefined;
    requestedByCustomerName?: string | null | undefined;
    events?: ReadonlyArray<{
      id: string;
      eventType: { type: string; name: string };
      occurredAt?: string | null | undefined;
      refundAmount?: number | null | undefined;
      currencyToDisplay: string;
      reason?: string | null | undefined;
      lastError?: string | null | undefined;
      externalRefundNumber?: string | null | undefined;
      actorName?: string | null | undefined;
    }> | null;
  };
};

type DialogMode = 'REJECT' | 'CANCEL' | 'RESOLVE_COMPLETE' | 'RESOLVE_FAILED' | null;

const toRefundStatusType = (value?: string | null | undefined) => value?.replace(/([a-z])([A-Z])/g, '$1_$2').toUpperCase() ?? '';

const toRefundErrorMessage = (value?: string | null | undefined) =>
  value?.includes('Only AUTHORISED invoices can have allocations applied to them')
    ? 'Xero created the credit note, but could not apply it to the invoice because the invoice is not authorized. Manual accounting follow-up is required.'
    : value;

const MarketplaceRefundAdminPanel = ({ refund }: Props) => {
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [dialogMode, setDialogMode] = useState<DialogMode>(null);
  const [reason, setReason] = useState('');

  const [approve] = useMutation<marketplaceRefundAdminPanel_approveMarketplaceRefundMutation>(graphql`
    mutation marketplaceRefundAdminPanel_approveMarketplaceRefundMutation($input: ApproveMarketplaceRefundInput!) {
      approveMarketplaceRefund(input: $input) {
        marketplaceRefund {
          id
          status {
            type
            name
          }
          lastError
          externalRefundNumber
        }
      }
    }
  `);
  const [reject] = useMutation<marketplaceRefundAdminPanel_rejectMarketplaceRefundMutation>(graphql`
    mutation marketplaceRefundAdminPanel_rejectMarketplaceRefundMutation($input: RejectMarketplaceRefundInput!) {
      rejectMarketplaceRefund(input: $input) {
        marketplaceRefund {
          id
          status {
            type
            name
          }
          lastError
          externalRefundNumber
        }
      }
    }
  `);
  const [cancel] = useMutation<marketplaceRefundAdminPanel_cancelMarketplaceRefundMutation>(graphql`
    mutation marketplaceRefundAdminPanel_cancelMarketplaceRefundMutation($input: CancelMarketplaceRefundInput!) {
      cancelMarketplaceRefund(input: $input) {
        marketplaceRefund {
          id
          status {
            type
            name
          }
          lastError
          externalRefundNumber
        }
      }
    }
  `);
  const [retry] = useMutation<marketplaceRefundAdminPanel_retryMarketplaceRefundMutation>(graphql`
    mutation marketplaceRefundAdminPanel_retryMarketplaceRefundMutation($input: RetryMarketplaceRefundInput!) {
      retryMarketplaceRefund(input: $input) {
        marketplaceRefund {
          id
          status {
            type
            name
          }
          lastError
          externalRefundNumber
        }
      }
    }
  `);
  const [resolve] = useMutation<marketplaceRefundAdminPanel_resolveRefundReconciliationRequiredMutation>(graphql`
    mutation marketplaceRefundAdminPanel_resolveRefundReconciliationRequiredMutation($input: ResolveRefundReconciliationRequiredInput!) {
      resolveRefundReconciliationRequired(input: $input) {
        marketplaceRefund {
          id
          status {
            type
            name
          }
          lastError
          externalRefundNumber
        }
      }
    }
  `);

  const status = toRefundStatusType(refund.status.type);
  const severity = refund.lastError ? 'warning' : status === 'COMPLETED' ? 'success' : status === 'FAILED' || status === 'REJECTED' ? 'warning' : 'info';
  const amountLabel = formatRefundAmount(refund.refundAmount, refund.currency?.type, refund.currencyToDisplay);

  type RelayCompleted = (response: unknown, errors?: PayloadError[] | null) => void;
  type RelayError = (error: Error) => void;
  const run = (action: (onCompleted: RelayCompleted, onError: RelayError) => void, successMessage: string) => {
    action(
      (_, errors) => {
        if (errors?.length) {
          themedToast(<NotificationContent content={`We couldn't update this refund. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
          return;
        }
        setDialogMode(null);
        themedToast(<NotificationContent content={successMessage} />, successNotificationOptions);
      },
      (error) => themedToast(<NotificationContent content={`We couldn't update this refund. ${getRelayErrorMessage(error)}`} />, errorNotificationOptions),
    );
  };

  const executeDialogAction = () => {
    const trimmedReason = reason.trim();
    if (!trimmedReason && dialogMode !== 'RESOLVE_COMPLETE') {
      return;
    }

    if (dialogMode === 'REJECT') {
      run((onCompleted, onError) => reject({ variables: { input: { clientMutationId: uuid(), id: refund.id, reason: trimmedReason } }, onCompleted, onError }), 'Refund rejected.');
    } else if (dialogMode === 'CANCEL') {
      run((onCompleted, onError) => cancel({ variables: { input: { clientMutationId: uuid(), id: refund.id, reason: trimmedReason } }, onCompleted, onError }), 'Refund canceled.');
    } else if (dialogMode === 'RESOLVE_COMPLETE' || dialogMode === 'RESOLVE_FAILED') {
      run(
        (onCompleted, onError) =>
          resolve({
            variables: {
              input: {
                clientMutationId: uuid(),
                id: refund.id,
                completed: dialogMode === 'RESOLVE_COMPLETE',
                reason: trimmedReason || 'Resolved by an operator.',
                providerReference: refund.externalRefundNumber ?? undefined,
              },
            },
            onCompleted,
            onError,
          }),
        dialogMode === 'RESOLVE_COMPLETE' ? 'Refund marked completed.' : 'Refund marked failed.',
      );
    }
  };

  const dialogTitle =
    dialogMode === 'REJECT'
      ? 'Reject refund'
      : dialogMode === 'CANCEL'
        ? 'Cancel refund'
        : dialogMode === 'RESOLVE_COMPLETE'
          ? 'Resolve refund as completed'
          : 'Resolve refund as failed';

  return (
    <>
      <Alert severity={severity} sx={{ mb: 1, borderRadius: 2 }}>
        Refund status: {refund.status.name}
        {amountLabel ? `, amount ${amountLabel}.` : '.'}
        {refund.externalRefundNumber ? ` Reference ${refund.externalRefundNumber}.` : ''}
        {toRefundErrorMessage(refund.lastError) ? ` ${toRefundErrorMessage(refund.lastError)}` : ''}
      </Alert>
      {refund.requestedByCustomerName ? (
        <Alert severity="info" sx={{ mb: 1, borderRadius: 2 }}>
          Requested by {refund.requestedByCustomerName}
        </Alert>
      ) : null}
      <StackRow sx={{ mb: 1, rowGap: 1 }}>
        {status === 'UNDER_REVIEW' ? (
          <>
            <Button
              size="small"
              variant="outlined"
              onClick={() =>
                run((onCompleted, onError) => approve({ variables: { input: { clientMutationId: uuid(), id: refund.id } }, onCompleted, onError }), 'Refund approved.')
              }
            >
              Approve
            </Button>
            <Button
              size="small"
              variant="outlined"
              onClick={() => {
                setReason(refund.reason ?? '');
                setDialogMode('REJECT');
              }}
            >
              Reject
            </Button>
          </>
        ) : null}
        {status === 'FAILED' ? (
          <Button
            size="small"
            variant="outlined"
            onClick={() =>
              run((onCompleted, onError) => retry({ variables: { input: { clientMutationId: uuid(), id: refund.id } }, onCompleted, onError }), 'Refund retry queued.')
            }
          >
            Retry refund
          </Button>
        ) : null}
        {status === 'RECONCILIATION_REQUIRED' ? (
          <>
            <Button
              size="small"
              color="success"
              variant="text"
              onClick={() => {
                setReason('');
                setDialogMode('RESOLVE_COMPLETE');
              }}
            >
              Resolve as completed
            </Button>
            <Button
              size="small"
              color="warning"
              variant="text"
              onClick={() => {
                setReason('');
                setDialogMode('RESOLVE_FAILED');
              }}
            >
              Resolve as failed
            </Button>
          </>
        ) : null}
        {status === 'REQUESTED' || status === 'UNDER_REVIEW' ? (
          <Button
            size="small"
            color="warning"
            variant="text"
            onClick={() => {
              setReason(refund.reason ?? '');
              setDialogMode('CANCEL');
            }}
          >
            Cancel refund
          </Button>
        ) : null}
      </StackRow>
      <MarketplaceRefundTimeline refund={refund} />
      <Dialog open={dialogMode !== null} onClose={() => setDialogMode(null)}>
        <DefaultDialogTitle title={dialogTitle} />
        <DialogContent sx={{ mt: 2, minWidth: { xs: 0, sm: 420 } }}>
          <DialogContentText>
            {dialogMode === 'REJECT'
              ? 'Provide a reason for rejecting this refund.'
              : dialogMode === 'CANCEL'
                ? 'Provide a reason for canceling this refund.'
                : 'Record the operator resolution reason.'}
          </DialogContentText>
          <TextField
            fullWidth
            margin="normal"
            multiline
            minRows={3}
            label="Reason"
            value={reason}
            onChange={(event) => setReason(event.target.value)}
            required={dialogMode !== 'RESOLVE_COMPLETE'}
          />
          <TwoButtonsDialogActions
            onPrimaryClicked={executeDialogAction}
            onSecondaryClicked={() => setDialogMode(null)}
            primaryLabel={dialogMode === 'REJECT' ? 'Reject' : dialogMode === 'CANCEL' ? 'Cancel refund' : 'Resolve'}
            secondaryLabel="Close"
            primaryDisabled={!reason.trim() && dialogMode !== 'RESOLVE_COMPLETE'}
          />
        </DialogContent>
      </Dialog>
    </>
  );
};

export default MarketplaceRefundAdminPanel;

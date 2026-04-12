import { DefaultDialogTitle, StackRow, TwoButtonsDialogActions } from '@/components/commons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { PaletteModeContext } from '@/libs/providers';
import { getRelayErrorMessage } from '@/libs/utils';
import type { marketplaceRefundAdminPanel_completeMarketplaceRefundMutation } from '@/queries/__generated__/marketplaceRefundAdminPanel_completeMarketplaceRefundMutation.graphql';
import type { marketplaceRefundAdminPanel_failMarketplaceRefundMutation } from '@/queries/__generated__/marketplaceRefundAdminPanel_failMarketplaceRefundMutation.graphql';
import type { marketplaceRefundAdminPanel_markMarketplaceRefundManualCompletedMutation } from '@/queries/__generated__/marketplaceRefundAdminPanel_markMarketplaceRefundManualCompletedMutation.graphql';
import type { marketplaceRefundAdminPanel_markMarketplaceRefundManualRequiredMutation } from '@/queries/__generated__/marketplaceRefundAdminPanel_markMarketplaceRefundManualRequiredMutation.graphql';
import type { marketplaceRefundAdminPanel_markMarketplaceRefundPendingAccountingMutation } from '@/queries/__generated__/marketplaceRefundAdminPanel_markMarketplaceRefundPendingAccountingMutation.graphql';
import type { marketplaceRefundAdminPanel_processMarketplaceRefundInXeroMutation } from '@/queries/__generated__/marketplaceRefundAdminPanel_processMarketplaceRefundInXeroMutation.graphql';
import Alert from '@mui/material/Alert';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import TextField from '@mui/material/TextField';
import { useContext, useState } from 'react';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import MarketplaceRefundTimeline from './marketplace-refund-timeline';
import { formatRefundAmount, hasDisplayCurrency } from './refund-display';

type Props = {
  entityLabel: string;
  refund: {
    id: string;
    currency?: {
      type: string;
      name: string;
    } | null;
    status: {
      type: string;
      name: string;
    };
    requestedAt?: string | null | undefined;
    lastProcessedAt?: string | null | undefined;
    refundAmount?: number | null | undefined;
    refundPercentage?: number | null | undefined;
    currencyToDisplay: string;
    reason?: string | null | undefined;
    lastError?: string | null | undefined;
    externalRefundNumber?: string | null | undefined;
    canProcessInXero?: boolean | null | undefined;
    xeroProcessingBlockedReason?: string | null | undefined;
    requestedByCustomerName?: string | null | undefined;
    events?: ReadonlyArray<{
      id: string;
      eventType: {
        type: string;
        name: string;
      };
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

const toRefundStatusType = (value?: string | null | undefined) => value?.replace(/([a-z])([A-Z])/g, '$1_$2').toUpperCase() ?? '';

const MarketplaceRefundAdminPanel = ({ entityLabel, refund }: Props) => {
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [refundApprovalDialogOpen, setRefundApprovalDialogOpen] = useState(false);
  const [refundApprovalAmount, setRefundApprovalAmount] = useState('');
  const [refundApprovalReason, setRefundApprovalReason] = useState('');
  const [refundResolutionDialogMode, setRefundResolutionDialogMode] = useState<'COMPLETE' | 'FAIL' | null>(null);
  const [manualResolutionDialogMode, setManualResolutionDialogMode] = useState<'REQUIRE' | 'COMPLETE' | null>(null);
  const [refundResolutionReason, setRefundResolutionReason] = useState('');
  const [commitMarkMarketplaceRefundPendingAccounting] = useMutation<marketplaceRefundAdminPanel_markMarketplaceRefundPendingAccountingMutation>(graphql`
    mutation marketplaceRefundAdminPanel_markMarketplaceRefundPendingAccountingMutation($input: MarkMarketplaceRefundPendingAccountingInput!) @raw_response_type {
      markMarketplaceRefundPendingAccounting(input: $input) {
        marketplaceRefund {
          id
          status {
            type
            name
          }
          refundAmount
          currencyToDisplay
          reason
          lastError
          externalRefundNumber
        }
      }
    }
  `);
  const [commitCompleteMarketplaceRefund] = useMutation<marketplaceRefundAdminPanel_completeMarketplaceRefundMutation>(graphql`
    mutation marketplaceRefundAdminPanel_completeMarketplaceRefundMutation($input: CompleteMarketplaceRefundInput!) @raw_response_type {
      completeMarketplaceRefund(input: $input) {
        marketplaceRefund {
          id
          status {
            type
            name
          }
          refundAmount
          currencyToDisplay
          reason
          lastError
          externalRefundNumber
        }
      }
    }
  `);
  const [commitFailMarketplaceRefund] = useMutation<marketplaceRefundAdminPanel_failMarketplaceRefundMutation>(graphql`
    mutation marketplaceRefundAdminPanel_failMarketplaceRefundMutation($input: FailMarketplaceRefundInput!) @raw_response_type {
      failMarketplaceRefund(input: $input) {
        marketplaceRefund {
          id
          status {
            type
            name
          }
          refundAmount
          currencyToDisplay
          reason
          lastError
          externalRefundNumber
        }
      }
    }
  `);
  const [commitProcessMarketplaceRefundInXero] = useMutation<marketplaceRefundAdminPanel_processMarketplaceRefundInXeroMutation>(graphql`
    mutation marketplaceRefundAdminPanel_processMarketplaceRefundInXeroMutation($input: ProcessMarketplaceRefundInXeroInput!) @raw_response_type {
      processMarketplaceRefundInXero(input: $input) {
        marketplaceRefund {
          id
          status {
            type
            name
          }
          refundAmount
          currencyToDisplay
          reason
          lastError
          externalRefundNumber
        }
      }
    }
  `);
  const [commitMarkMarketplaceRefundManualRequired] = useMutation<marketplaceRefundAdminPanel_markMarketplaceRefundManualRequiredMutation>(graphql`
    mutation marketplaceRefundAdminPanel_markMarketplaceRefundManualRequiredMutation($input: MarkMarketplaceRefundManualRequiredInput!) @raw_response_type {
      markMarketplaceRefundManualRequired(input: $input) {
        marketplaceRefund {
          id
          status {
            type
            name
          }
          refundAmount
          currencyToDisplay
          reason
          lastError
          externalRefundNumber
        }
      }
    }
  `);
  const [commitMarkMarketplaceRefundManualCompleted] = useMutation<marketplaceRefundAdminPanel_markMarketplaceRefundManualCompletedMutation>(graphql`
    mutation marketplaceRefundAdminPanel_markMarketplaceRefundManualCompletedMutation($input: MarkMarketplaceRefundManualCompletedInput!) @raw_response_type {
      markMarketplaceRefundManualCompleted(input: $input) {
        marketplaceRefund {
          id
          status {
            type
            name
          }
          refundAmount
          currencyToDisplay
          reason
          lastError
          externalRefundNumber
        }
      }
    }
  `);

  const refundStatusType = toRefundStatusType(refund.status.type);
  const refundSeverity =
    refundStatusType === 'COMPLETED' || refundStatusType === 'MANUAL_COMPLETED'
      ? 'success'
      : refundStatusType === 'FAILED' || refundStatusType === 'MANUAL_REQUIRED'
        ? 'warning'
        : 'info';
  const refundAmountLabel = formatRefundAmount(refund.refundAmount, refund.currency?.type, refund.currencyToDisplay);
  const canProcessInXero = refund.canProcessInXero ?? true;
  const xeroBlockedMessage = refund.xeroProcessingBlockedReason;
  const parsedRefundApprovalAmount = refundApprovalAmount.trim() === '' ? undefined : Number(refundApprovalAmount);
  const isRefundApprovalAmountInvalid =
    parsedRefundApprovalAmount !== undefined &&
    (!Number.isFinite(parsedRefundApprovalAmount) || parsedRefundApprovalAmount <= 0 || (refund.refundAmount != null && parsedRefundApprovalAmount > Number(refund.refundAmount)));

  const runRefundAction = (actionLabel: string, runMutation: (toastId: string | number) => void) => {
    const toastId = themedToast(<NotificationContent content={`${actionLabel} for ${entityLabel}...`} />, infoNotificationOptions);
    runMutation(toastId);
  };

  const handleQueueRefundClick = () => {
    setRefundApprovalAmount(refund.refundAmount != null ? String(refund.refundAmount) : '');
    setRefundApprovalReason(refund.reason ?? '');
    setRefundApprovalDialogOpen(true);
  };

  const handleCloseRefundApprovalDialog = () => {
    setRefundApprovalDialogOpen(false);
  };

  const handleOpenRefundResolutionDialog = (mode: 'COMPLETE' | 'FAIL') => {
    setRefundResolutionReason(refund.reason ?? '');
    setRefundResolutionDialogMode(mode);
  };

  const handleCloseRefundResolutionDialog = () => {
    setRefundResolutionDialogMode(null);
  };

  const handleOpenManualResolutionDialog = (mode: 'REQUIRE' | 'COMPLETE') => {
    setRefundResolutionReason(refund.reason ?? '');
    setManualResolutionDialogMode(mode);
  };

  const handleCloseManualResolutionDialog = () => {
    setManualResolutionDialogMode(null);
  };

  const handleConfirmQueueRefundClick = () => {
    if (isRefundApprovalAmountInvalid) {
      return;
    }

    runRefundAction('Queueing refund', (toastId) => {
      commitMarkMarketplaceRefundPendingAccounting({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: refund.id,
            refundAmount: parsedRefundApprovalAmount,
            reason: refundApprovalReason.trim() || undefined,
          },
        },
        onCompleted: (_, errors) => {
          if (errors?.length) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`We couldn't queue this refund. ${getRelayErrorMessage(errors)}`} />,
            });
            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content="This refund has been queued for accounting." />,
          });
          setRefundApprovalDialogOpen(false);
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't queue this refund. ${getRelayErrorMessage(error)}`} />,
          });
        },
      });
    });
  };

  const handleProcessRefundInXeroClick = () => {
    runRefundAction('Sending refund to Xero', (toastId) => {
      commitProcessMarketplaceRefundInXero({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: refund.id,
          },
        },
        onCompleted: (_, errors) => {
          if (errors?.length) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`We couldn't send this refund to Xero. ${getRelayErrorMessage(errors)}`} />,
            });
            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content="This refund has been sent to Xero." />,
          });
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't send this refund to Xero. ${getRelayErrorMessage(error)}`} />,
          });
        },
      });
    });
  };

  const handleCompleteRefundClick = () => {
    runRefundAction('Completing refund', (toastId) => {
      commitCompleteMarketplaceRefund({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: refund.id,
            reason: refundResolutionReason.trim() || undefined,
          },
        },
        onCompleted: (_, errors) => {
          if (errors?.length) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`We couldn't complete this refund. ${getRelayErrorMessage(errors)}`} />,
            });
            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content="This refund has been marked as completed." />,
          });
          setRefundResolutionDialogMode(null);
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't complete this refund. ${getRelayErrorMessage(error)}`} />,
          });
        },
      });
    });
  };

  const handleFailRefundClick = () => {
    runRefundAction('Marking refund as failed', (toastId) => {
      commitFailMarketplaceRefund({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: refund.id,
            reason: refundResolutionReason.trim() || undefined,
          },
        },
        onCompleted: (_, errors) => {
          if (errors?.length) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`We couldn't update this refund. ${getRelayErrorMessage(errors)}`} />,
            });
            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content="This refund has been marked as failed." />,
          });
          setRefundResolutionDialogMode(null);
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't update this refund. ${getRelayErrorMessage(error)}`} />,
          });
        },
      });
    });
  };

  const handleMarkManualRequiredClick = () => {
    runRefundAction('Moving refund to manual follow-up', (toastId) => {
      commitMarkMarketplaceRefundManualRequired({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: refund.id,
            reason: refundResolutionReason.trim() || undefined,
          },
        },
        onCompleted: (_, errors) => {
          if (errors?.length) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`We couldn't move this refund to manual follow-up. ${getRelayErrorMessage(errors)}`} />,
            });
            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content="This refund has been moved to manual follow-up." />,
          });
          setManualResolutionDialogMode(null);
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't move this refund to manual follow-up. ${getRelayErrorMessage(error)}`} />,
          });
        },
      });
    });
  };

  const handleMarkManualCompletedClick = () => {
    runRefundAction('Completing refund manually', (toastId) => {
      commitMarkMarketplaceRefundManualCompleted({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: refund.id,
            reason: refundResolutionReason.trim() || undefined,
          },
        },
        onCompleted: (_, errors) => {
          if (errors?.length) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`We couldn't complete this refund manually. ${getRelayErrorMessage(errors)}`} />,
            });
            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content="This refund has been marked as completed manually." />,
          });
          setManualResolutionDialogMode(null);
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't complete this refund manually. ${getRelayErrorMessage(error)}`} />,
          });
        },
      });
    });
  };

  return (
    <>
      <Alert severity={refundSeverity} sx={{ mb: 1, borderRadius: 2 }}>
        Refund status: {refund.status.name}
        {refundAmountLabel ? `, amount ${refundAmountLabel}.` : '.'}
        {refund.externalRefundNumber ? ` Reference ${refund.externalRefundNumber}.` : ''}
        {refund.lastError ? ` ${refund.lastError}` : ''}
      </Alert>
      {refund.requestedByCustomerName ? (
        <Alert severity="info" sx={{ mb: 1, borderRadius: 2 }}>
          Requested by {refund.requestedByCustomerName}
        </Alert>
      ) : null}
      <StackRow sx={{ mb: 1, rowGap: 1 }}>
        {refundStatusType === 'REQUESTED' || refundStatusType === 'FAILED' || refundStatusType === 'MANUAL_REQUIRED' ? (
          <Button size="small" variant="outlined" onClick={handleQueueRefundClick}>
            {refundStatusType === 'FAILED' || refundStatusType === 'MANUAL_REQUIRED' ? 'Retry refund' : 'Queue refund'}
          </Button>
        ) : null}
        {refundStatusType === 'PENDING_ACCOUNTING' ? (
          <>
            {canProcessInXero ? (
              <Button size="small" variant="outlined" onClick={handleProcessRefundInXeroClick}>
                Send to Xero
              </Button>
            ) : null}
            <Button size="small" variant="outlined" onClick={() => handleOpenManualResolutionDialog('REQUIRE')}>
              Needs manual follow-up
            </Button>
            <Button size="small" color="success" variant="text" onClick={() => handleOpenRefundResolutionDialog('COMPLETE')}>
              Mark complete
            </Button>
            <Button size="small" color="warning" variant="text" onClick={() => handleOpenRefundResolutionDialog('FAIL')}>
              Mark failed
            </Button>
          </>
        ) : null}
        {refundStatusType === 'MANUAL_REQUIRED' ? (
          <Button size="small" color="success" variant="text" onClick={() => handleOpenManualResolutionDialog('COMPLETE')}>
            Mark manual payout complete
          </Button>
        ) : null}
      </StackRow>
      {refundStatusType === 'PENDING_ACCOUNTING' && !canProcessInXero && xeroBlockedMessage ? (
        <Alert severity="info" sx={{ mb: 1, borderRadius: 2 }}>
          {xeroBlockedMessage}
        </Alert>
      ) : null}
      <MarketplaceRefundTimeline refund={refund} />
      <Dialog open={refundApprovalDialogOpen} onClose={handleCloseRefundApprovalDialog}>
        <DefaultDialogTitle title="Approve Refund" />
        <DialogContent sx={{ mt: 2, minWidth: { xs: 0, sm: 420 } }}>
          <DialogContentText>
            Queue this refund for accounting with an approved amount and optional note. The approved amount cannot exceed the current refund amount on the record.
          </DialogContentText>
          <TextField
            fullWidth
            margin="normal"
            label={`Approved amount${hasDisplayCurrency(refund.currency?.type, refund.currencyToDisplay) ? ` (${refund.currency?.type ?? refund.currencyToDisplay})` : ''}`}
            value={refundApprovalAmount}
            onChange={(event) => setRefundApprovalAmount(event.target.value)}
            error={isRefundApprovalAmountInvalid}
            helperText={
              isRefundApprovalAmountInvalid
                ? `Enter an amount greater than zero${refund.refundAmount != null ? ` and no more than ${refund.refundAmount}` : ''}.`
                : refund.refundAmount != null
                  ? `Policy amount currently recorded: ${formatRefundAmount(refund.refundAmount, refund.currency?.type, refund.currencyToDisplay)}`
                  : 'Leave unchanged to keep the current amount.'
            }
          />
          <TextField
            fullWidth
            margin="normal"
            multiline
            minRows={3}
            label="Admin note"
            value={refundApprovalReason}
            onChange={(event) => setRefundApprovalReason(event.target.value)}
            helperText="Optional note recorded with the refund status."
          />
          <TwoButtonsDialogActions
            onPrimaryClicked={handleConfirmQueueRefundClick}
            onSecondaryClicked={handleCloseRefundApprovalDialog}
            primaryLabel="Queue refund"
            secondaryLabel="Cancel"
            primaryDisabled={isRefundApprovalAmountInvalid}
          />
        </DialogContent>
      </Dialog>
      <Dialog open={refundResolutionDialogMode !== null} onClose={handleCloseRefundResolutionDialog}>
        <DefaultDialogTitle title={refundResolutionDialogMode === 'COMPLETE' ? 'Complete Refund' : 'Fail Refund'} />
        <DialogContent sx={{ mt: 2, minWidth: { xs: 0, sm: 420 } }}>
          <DialogContentText>
            {refundResolutionDialogMode === 'COMPLETE'
              ? 'Record an optional note for how this refund was completed.'
              : 'Record an optional note explaining why this refund failed or what needs follow-up.'}
          </DialogContentText>
          <TextField
            fullWidth
            margin="normal"
            multiline
            minRows={3}
            label="Admin note"
            value={refundResolutionReason}
            onChange={(event) => setRefundResolutionReason(event.target.value)}
            helperText="Optional note recorded with the refund status."
          />
          <TwoButtonsDialogActions
            onPrimaryClicked={refundResolutionDialogMode === 'COMPLETE' ? handleCompleteRefundClick : handleFailRefundClick}
            onSecondaryClicked={handleCloseRefundResolutionDialog}
            primaryLabel={refundResolutionDialogMode === 'COMPLETE' ? 'Mark complete' : 'Mark failed'}
            secondaryLabel="Cancel"
          />
        </DialogContent>
      </Dialog>
      <Dialog open={manualResolutionDialogMode !== null} onClose={handleCloseManualResolutionDialog}>
        <DefaultDialogTitle title={manualResolutionDialogMode === 'COMPLETE' ? 'Complete Refund Manually' : 'Move Refund To Manual Follow-Up'} />
        <DialogContent sx={{ mt: 2, minWidth: { xs: 0, sm: 420 } }}>
          <DialogContentText>
            {manualResolutionDialogMode === 'COMPLETE'
              ? 'Record an optional note for the manual completion of this refund.'
              : 'Record an optional note explaining the manual follow-up required for this refund.'}
          </DialogContentText>
          <TextField
            fullWidth
            margin="normal"
            multiline
            minRows={3}
            label="Admin note"
            value={refundResolutionReason}
            onChange={(event) => setRefundResolutionReason(event.target.value)}
            helperText="Optional note recorded with the refund status."
          />
          <TwoButtonsDialogActions
            onPrimaryClicked={manualResolutionDialogMode === 'COMPLETE' ? handleMarkManualCompletedClick : handleMarkManualRequiredClick}
            onSecondaryClicked={handleCloseManualResolutionDialog}
            primaryLabel={manualResolutionDialogMode === 'COMPLETE' ? 'Mark manual completion' : 'Move to manual follow-up'}
            secondaryLabel="Cancel"
          />
        </DialogContent>
      </Dialog>
    </>
  );
};

export default MarketplaceRefundAdminPanel;

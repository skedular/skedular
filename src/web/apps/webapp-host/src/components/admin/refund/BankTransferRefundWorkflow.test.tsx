import { fireEvent, render, screen, waitFor } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { BankTransferRefundWorkflow } from './BankTransferRefundWorkflow';

describe('BankTransferRefundWorkflow', () => {
  it('requires a transfer reference before recording sent', () => {
    render(<BankTransferRefundWorkflow status="Approved" onApprove={vi.fn()} onRecordSent={vi.fn()} onConfirmReceived={vi.fn()} />);
    expect(screen.getByRole('button', { name: 'Record transfer sent' })).toBeDisabled();
  });

  it('runs the approve, sent, and received actions for each state', async () => {
    const onApprove = vi.fn().mockResolvedValue(undefined);
    const onRecordSent = vi.fn().mockResolvedValue(undefined);
    const onConfirmReceived = vi.fn().mockResolvedValue(undefined);
    const { rerender } = render(<BankTransferRefundWorkflow status="UnderReview" onApprove={onApprove} onRecordSent={onRecordSent} onConfirmReceived={onConfirmReceived} />);
    fireEvent.click(screen.getByRole('button', { name: 'Approve refund' }));
    await waitFor(() => expect(onApprove).toHaveBeenCalledOnce());
    rerender(<BankTransferRefundWorkflow status="Approved" onApprove={onApprove} onRecordSent={onRecordSent} onConfirmReceived={onConfirmReceived} />);
    fireEvent.change(screen.getByRole('textbox'), { target: { value: 'BANK-123' } });
    fireEvent.click(screen.getByRole('button', { name: 'Record transfer sent' }));
    await waitFor(() => expect(onRecordSent).toHaveBeenCalledWith('BANK-123'));
    rerender(<BankTransferRefundWorkflow status="Processing" onApprove={onApprove} onRecordSent={onRecordSent} onConfirmReceived={onConfirmReceived} />);
    fireEvent.click(screen.getByRole('button', { name: 'Confirm transfer received' }));
    await waitFor(() => expect(onConfirmReceived).toHaveBeenCalledOnce());
  });
});

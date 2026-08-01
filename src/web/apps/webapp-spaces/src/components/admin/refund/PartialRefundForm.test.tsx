import { fireEvent, render, screen, waitFor } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { PartialRefundForm } from './PartialRefundForm';

describe('PartialRefundForm', () => {
  it('requires a valid amount and reason before confirmation', () => {
    render(<PartialRefundForm remainingBalance={50} currency="USD" onSubmit={vi.fn()} />);
    fireEvent.click(screen.getByRole('button', { name: 'Create partial refund' }));
    expect(screen.getByText('Enter a refund amount.')).toBeInTheDocument();
  });

  it('shows confirmation and submits the trimmed values', async () => {
    const onSubmit = vi.fn().mockResolvedValue(undefined);
    render(<PartialRefundForm remainingBalance={50} currency="USD" onSubmit={onSubmit} />);
    const [amountInput, reasonInput] = screen.getAllByRole('textbox');
    fireEvent.change(amountInput, { target: { value: '12.50' } });
    fireEvent.change(reasonInput, { target: { value: '  goodwill  ' } });
    fireEvent.click(screen.getByRole('button', { name: 'Create partial refund' }));
    expect(screen.getByText(/Confirm a partial refund of 12.50 USD/)).toBeInTheDocument();
    fireEvent.click(screen.getByRole('button', { name: 'Confirm partial refund' }));
    await waitFor(() => expect(onSubmit).toHaveBeenCalledWith(12.5, 'goodwill', expect.any(String)));
  });
});

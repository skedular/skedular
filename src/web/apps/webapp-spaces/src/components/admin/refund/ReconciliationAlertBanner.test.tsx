import { fireEvent, render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { ReconciliationAlertBanner } from './ReconciliationAlertBanner';

describe('ReconciliationAlertBanner', () => {
  it('does not render when there are no pending reconciliations', () => {
    render(<ReconciliationAlertBanner count={0} onOpenQueue={vi.fn()} />);
    expect(screen.queryByRole('alert')).not.toBeInTheDocument();
  });

  it('uses singular/plural copy and opens the queue', () => {
    const onOpenQueue = vi.fn();
    const { rerender } = render(<ReconciliationAlertBanner count={1} onOpenQueue={onOpenQueue} />);
    expect(screen.getByText('1 refund require reconciliation.')).toBeInTheDocument();
    fireEvent.click(screen.getByRole('button', { name: 'Open queue' }));
    expect(onOpenQueue).toHaveBeenCalledOnce();
    rerender(<ReconciliationAlertBanner count={2} onOpenQueue={onOpenQueue} />);
    expect(screen.getByText('2 refunds require reconciliation.')).toBeInTheDocument();
  });
});

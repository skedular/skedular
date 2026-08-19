import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it } from 'vitest';
import FieldHelp from './field-help';

describe('FieldHelp', () => {
  it('reveals contextual guidance when activated', async () => {
    const user = userEvent.setup();

    render(<FieldHelp label="Cadence">This controls when a recurring offer renews.</FieldHelp>);

    await user.click(screen.getByRole('button', { name: 'Help for Cadence' }));

    expect(screen.getByText('Cadence')).toBeInTheDocument();
    expect(screen.getByText('This controls when a recurring offer renews.')).toBeInTheDocument();
  });
});

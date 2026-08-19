import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';
import { useState } from 'react';
import DurationInput from './duration-input';

describe('DurationInput', () => {
  it('defaults to hours while showing the saved minute value', () => {
    render(<DurationInput label="Minimum duration" value="90" onChange={vi.fn()} required />);

    expect(screen.getByText('Minimum duration *')).toBeInTheDocument();
    expect(screen.getByRole('spinbutton')).toHaveValue(1.5);
    expect(screen.getByText('Saved as 90 minutes')).toBeInTheDocument();
    expect(screen.getByRole('switch', { name: 'Minimum duration unit' })).toBeChecked();
  });

  it('converts hour input to minutes', async () => {
    const user = userEvent.setup();
    const onChange = vi.fn();

    render(<DurationInput label="Booking duration" value="60" onChange={onChange} />);

    const input = screen.getByRole('spinbutton');
    await user.clear(input);
    await user.type(input, '1.5');

    expect(onChange).toHaveBeenLastCalledWith('90');
  });

  it('switches to minutes for precise entry and preserves the saved value', async () => {
    const user = userEvent.setup();
    const onChange = vi.fn();
    const ControlledDuration = () => {
      const [value, setValue] = useState('90');
      return (
        <DurationInput
          label="Booking duration"
          value={value}
          onChange={(nextValue) => {
            onChange(nextValue);
            setValue(nextValue);
          }}
        />
      );
    };

    render(<ControlledDuration />);

    await user.click(screen.getByRole('switch', { name: 'Booking duration unit' }));
    expect(screen.getByRole('spinbutton')).toHaveValue(90);
    expect(screen.getByRole('switch', { name: 'Booking duration unit' })).not.toBeChecked();

    await user.clear(screen.getByRole('spinbutton'));
    await user.type(screen.getByRole('spinbutton'), '45');
    expect(onChange).toHaveBeenLastCalledWith('45');
  });

  it('handles empty and non-numeric values without inventing a duration', async () => {
    const user = userEvent.setup();
    const onChange = vi.fn();
    const ControlledDuration = () => {
      const [value, setValue] = useState<string | undefined>('60');
      return (
        <DurationInput
          label="Duration"
          value={value}
          onChange={(nextValue) => {
            onChange(nextValue);
            setValue(nextValue);
          }}
        />
      );
    };

    render(<ControlledDuration />);

    expect(screen.getByRole('spinbutton')).toHaveValue(1);
    expect(screen.getByText('Saved as 60 minutes')).toBeInTheDocument();

    await user.clear(screen.getByRole('spinbutton'));
    expect(onChange).toHaveBeenLastCalledWith('');
  });

  it('disables both entry controls when disabled', () => {
    render(<DurationInput label="Duration" value="60" onChange={vi.fn()} disabled />);

    expect(screen.getByRole('spinbutton')).toBeDisabled();
    expect(screen.getByRole('switch', { name: 'Duration unit' })).toBeDisabled();
  });
});

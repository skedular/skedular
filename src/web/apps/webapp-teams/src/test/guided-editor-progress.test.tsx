import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';
import { GuidedEditorProgress } from '@skedular/ui';

describe('GuidedEditorProgress', () => {
  it('renders the editor title and all steps', () => {
    render(
      <GuidedEditorProgress
        title="Create Product"
        description="Move through the editor in steps."
        activeStepId="basics"
        onStepChange={() => undefined}
        steps={[
          { id: 'basics', title: 'Basics' },
          { id: 'offers', title: 'Offers' },
          { id: 'review', title: 'Review' },
        ]}
      />,
    );

    expect(screen.getByText('Create Product')).toBeInTheDocument();
    expect(screen.getByText('Move through the editor in steps.')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Basics' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Offers' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Review' })).toBeInTheDocument();
  });

  it('calls onStepChange when another step is selected', async () => {
    const user = userEvent.setup();
    const onStepChange = vi.fn();

    render(
      <GuidedEditorProgress
        title="Create Product"
        activeStepId="basics"
        onStepChange={onStepChange}
        steps={[
          { id: 'basics', title: 'Basics' },
          { id: 'offers', title: 'Offers' },
        ]}
      />,
    );

    await user.click(screen.getByRole('button', { name: 'Offers' }));

    expect(onStepChange).toHaveBeenCalledWith('offers');
  });
});

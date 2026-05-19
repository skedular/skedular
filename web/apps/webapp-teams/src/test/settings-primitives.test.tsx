import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it } from 'vitest';
import { EditorActionBar, SettingsSectionCard, StickyReviewRail } from '@skedular/ui';

describe('settings primitives', () => {
  it('renders a settings section card with title, description, and content', () => {
    render(
      <SettingsSectionCard title="Payments" description="Describe how customers pay for this offer.">
        <div>Payment fields</div>
      </SettingsSectionCard>,
    );

    expect(screen.getByText('Payments')).toBeInTheDocument();
    expect(screen.getByText('Describe how customers pay for this offer.')).toBeInTheDocument();
    expect(screen.getByText('Payment fields')).toBeInTheDocument();
  });

  it('renders a sticky review rail with summary content', () => {
    render(
      <StickyReviewRail title="Review rail" description="Keep the product story visible while editing.">
        <div>Summary card</div>
      </StickyReviewRail>,
    );

    expect(screen.getByText('Review rail')).toBeInTheDocument();
    expect(screen.getByText('Keep the product story visible while editing.')).toBeInTheDocument();
    expect(screen.getByText('Summary card')).toBeInTheDocument();
  });

  it('renders a generic editor action bar with primary and secondary actions', async () => {
    const user = userEvent.setup();
    let secondaryClicked = false;

    render(
      <EditorActionBar
        primaryAction="Save resource"
        secondaryActions={
          <button
            onClick={() => {
              secondaryClicked = true;
            }}
            type="button"
          >
            Cancel
          </button>
        }
      />,
    );

    expect(screen.getByRole('button', { name: 'Save resource' })).toHaveAttribute('type', 'submit');

    await user.click(screen.getByRole('button', { name: 'Cancel' }));

    expect(secondaryClicked).toBe(true);
  });
});

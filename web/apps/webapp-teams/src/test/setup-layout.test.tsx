import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { SetupFeatureCard, SetupSplitLayout } from '@skedular/ui';

describe('setup layout primitives', () => {
  it('renders a setup split layout with aside and main content', () => {
    render(
      <SetupSplitLayout
        asideTitle="Left panel"
        asideDescription="Context for setup"
        asideChildren={<div>Aside feature</div>}
        mainTitle="Create location"
        mainDescription="Primary editor content"
      >
        <div>Main form</div>
      </SetupSplitLayout>,
    );

    expect(screen.getByText('Left panel')).toBeInTheDocument();
    expect(screen.getByText('Context for setup')).toBeInTheDocument();
    expect(screen.getByText('Aside feature')).toBeInTheDocument();
    expect(screen.getByText('Create location')).toBeInTheDocument();
    expect(screen.getByText('Primary editor content')).toBeInTheDocument();
    expect(screen.getByText('Main form')).toBeInTheDocument();
  });

  it('renders a setup feature card with icon, title, and description', () => {
    render(<SetupFeatureCard icon={<span>i</span>} title="Booking access" description="Explain the setup benefit clearly." />);

    expect(screen.getByText('i')).toBeInTheDocument();
    expect(screen.getByText('Booking access')).toBeInTheDocument();
    expect(screen.getByText('Explain the setup benefit clearly.')).toBeInTheDocument();
  });
});

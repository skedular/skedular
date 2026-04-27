'use client';

import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import BodyIconTypography from '../body-icon-typography';
import CaptionIconTypography from '../caption-icon-typography';
import ErrorTypography from '../error-typography';

describe('ErrorTypography', () => {
  it('renders the error message', () => {
    render(<ErrorTypography errorMessage="Something went wrong" />);
    expect(screen.getByText('Something went wrong')).toBeInTheDocument();
  });

  it('renders nothing when errorMessage is null', () => {
    const { container } = render(<ErrorTypography errorMessage={null} />);
    expect(container.firstChild).toBeNull();
  });

  it('renders nothing when errorMessage is undefined', () => {
    const { container } = render(<ErrorTypography errorMessage={undefined} />);
    expect(container.firstChild).toBeNull();
  });
});

describe('BodyIconTypography', () => {
  it('renders a label', () => {
    render(<BodyIconTypography label="Hello body" />);
    expect(screen.getByText('Hello body')).toBeInTheDocument();
  });

  it('renders a start element', () => {
    render(<BodyIconTypography label="text" startElement={<span>icon</span>} />);
    expect(screen.getByText('icon')).toBeInTheDocument();
  });
});

describe('CaptionIconTypography', () => {
  it('renders a label', () => {
    render(<CaptionIconTypography label="Caption text" />);
    expect(screen.getByText('Caption text')).toBeInTheDocument();
  });
});

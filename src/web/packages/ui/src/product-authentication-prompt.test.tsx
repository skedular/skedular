import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import ProductAuthenticationPrompt from './product-authentication-prompt';

describe('ProductAuthenticationPrompt', () => {
  it('renders the product message and authentication actions', () => {
    render(<ProductAuthenticationPrompt title="Welcome to Skedular Spaces" description="Authentication is required." signInHref="/signin" signUpHref="/signup" />);

    expect(screen.getByText('Welcome to Skedular Spaces')).toBeInTheDocument();
    expect(screen.getByText('Authentication is required.')).toBeInTheDocument();
    expect(screen.getByRole('link', { name: 'Sign in' })).toHaveAttribute('href', '/signin');
    expect(screen.getByRole('link', { name: 'Create account' })).toHaveAttribute('href', '/signup');
  });
});

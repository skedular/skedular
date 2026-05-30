import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import CustomOrganizationAuthPage from './custom-organization-auth-page';

describe('CustomOrganizationAuthPage', () => {
  it('renders organization branding and posts sign in through custom WorkOS password auth', () => {
    const { container } = render(
      <CustomOrganizationAuthPage mode="sign-in" organizationName="Mapp" organizationLogoUrl="https://cdn.example.com/mapp.png" returnTo="/marketplace/bookings" />,
    );

    expect(screen.getByRole('img', { name: 'Mapp logo' })).toBeInTheDocument();
    expect(screen.getByRole('heading', { name: 'Sign in' })).toBeInTheDocument();
    expect(screen.getByRole('textbox', { name: /email/i })).toBeRequired();
    expect(container.querySelector('input[name="password"]')).toBeRequired();
    expect(screen.getByRole('button', { name: 'Sign in' })).toHaveAttribute('type', 'submit');
    expect(screen.getByRole('link', { name: 'Continue with Google' })).toHaveAttribute('href', '/api/auth/oauth/google/start?returnTo=%2Fmarketplace%2Fbookings&mode=sign-in');
    expect(screen.getByRole('link', { name: 'Continue with Microsoft' })).toHaveAttribute(
      'href',
      '/api/auth/oauth/microsoft/start?returnTo=%2Fmarketplace%2Fbookings&mode=sign-in',
    );
    expect(screen.getByDisplayValue('/marketplace/bookings')).toHaveAttribute('name', 'returnTo');
  });

  it('renders sign up copy and alternate sign in link', () => {
    const { container } = render(<CustomOrganizationAuthPage mode="sign-up" organizationName="Mapp" />);

    expect(screen.getByRole('heading', { name: 'Create account' })).toBeInTheDocument();
    expect(container.querySelector('input[name="confirmPassword"]')).toBeRequired();
    expect(screen.getByRole('link', { name: 'Sign in' })).toHaveAttribute('href', '/auth/signin');
  });

  it('renders authentication errors', () => {
    render(<CustomOrganizationAuthPage mode="sign-in" error="invalid_credentials" />);

    expect(screen.getByText('The email or password is incorrect.')).toBeInTheDocument();
  });
});

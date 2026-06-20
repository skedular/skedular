import { render, screen, waitFor } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import RootPage from './page';

const { loadQueryMock, useAuthMock, useQueryLoaderMock } = vi.hoisted(() => ({
  loadQueryMock: vi.fn(),
  useAuthMock: vi.fn(),
  useQueryLoaderMock: vi.fn(),
}));

vi.mock('@workos-inc/authkit-nextjs/components', () => ({
  useAuth: useAuthMock,
}));

vi.mock('react-relay', () => ({
  useQueryLoader: useQueryLoaderMock,
}));

vi.mock('@/components/noOrganizationLanding', () => ({
  NoOrganizationLandingPageRootQuery: {},
  NoOrganizationLandingContent: () => <div>Authenticated Teams content</div>,
}));

vi.mock('@/components/rootShell', () => ({
  NoOrganizationRootShell: ({ children }: React.PropsWithChildren) => <div data-testid="authenticated-shell">{children}</div>,
  UnauthenticatedRootShell: ({ children }: React.PropsWithChildren) => <div data-testid="unauthenticated-shell">{children}</div>,
}));

describe('Teams root page authentication boundary', () => {
  beforeEach(() => {
    loadQueryMock.mockReset();
    useAuthMock.mockReset();
    useQueryLoaderMock.mockReset();
    useQueryLoaderMock.mockReturnValue([{}, loadQueryMock]);
  });

  it('renders the unauthenticated page without mounting a Relay query', () => {
    useAuthMock.mockReturnValue({ loading: false, user: null });

    render(<RootPage />);

    expect(screen.getByTestId('unauthenticated-shell')).toBeInTheDocument();
    expect(screen.getByText('Welcome to Skedular Teams')).toBeInTheDocument();
    expect(useQueryLoaderMock).not.toHaveBeenCalled();
    expect(loadQueryMock).not.toHaveBeenCalled();
  });

  it('mounts the authenticated shell and query only for a signed-in user', async () => {
    useAuthMock.mockReturnValue({ loading: false, user: { id: 'user-1' } });

    render(<RootPage />);

    expect(screen.getByTestId('authenticated-shell')).toBeInTheDocument();
    expect(screen.getByText('Authenticated Teams content')).toBeInTheDocument();
    await waitFor(() => expect(loadQueryMock).toHaveBeenCalledTimes(1));
  });
});

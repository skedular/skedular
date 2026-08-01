import { render, waitFor } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import RootPage from '../refunds/page';

const { commitResolveMock, loadQueryMock, useKnownParamsMock, useMutationMock, usePreloadedQueryMock, useQueryLoaderMock } = vi.hoisted(() => ({
  commitResolveMock: vi.fn(),
  loadQueryMock: vi.fn(),
  useKnownParamsMock: vi.fn(),
  useMutationMock: vi.fn(),
  usePreloadedQueryMock: vi.fn(),
  useQueryLoaderMock: vi.fn(),
}));

vi.mock('@skedular/shared', async (importOriginal) => {
  const actual = await importOriginal<typeof import('@skedular/shared')>();
  return {
    ...actual,
    RelayError: () => null,
    startOfDay: () => ({
      startOf: () => ({ toISOString: () => '2026-07-31T00:00:00.000Z' }),
      endOf: () => ({ toISOString: () => '2026-08-01T23:59:59.999Z' }),
      subtract: () => ({ startOf: () => ({ toISOString: () => '2026-07-01T00:00:00.000Z' }) }),
      toISOString: () => '2026-08-01T00:00:00.000Z',
    }),
    toRootError: (error: unknown) => error,
    useKnownParams: useKnownParamsMock,
    useIntegratedPlatform: () => ({ integratedPlatform: undefined }),
  };
});

vi.mock('react-relay', () => ({
  graphql: vi.fn(),
  useMutation: useMutationMock,
  usePreloadedQuery: usePreloadedQueryMock,
  useQueryLoader: useQueryLoaderMock,
}));

vi.mock('next/navigation', () => ({
  useRouter: () => ({ push: vi.fn() }),
}));

vi.mock('@/components/rootShell', () => ({
  RootShell: ({ children }: React.PropsWithChildren) => <div>{children}</div>,
}));

vi.mock('@/components/organization/organizationAdmin', () => ({
  OrganizationAdmin: () => <div>Organization admin</div>,
}));

vi.mock('@/components/admin/refund/RefundQueue', () => ({
  RefundQueue: ({
    onRefundPageChange,
    onExternalFilterChange,
    onExternalPageChange,
  }: {
    onRefundPageChange?: (direction: 'next' | 'previous') => void;
    onExternalFilterChange?: (provider: string | null, status: string | null) => void;
    onExternalPageChange?: (direction: 'next' | 'previous') => void;
  }) => (
    <>
      <button onClick={() => onRefundPageChange?.('next')}>Next page</button>
      <button onClick={() => onExternalFilterChange?.('XERO', 'Resolved')}>Apply external filters</button>
      <button onClick={() => onExternalPageChange?.('next')}>Next external page</button>
    </>
  ),
}));

const rootData = {
  organization: { id: 'org-1', name: 'Spaces organization' },
  marketplaceRefundQueue: {
    pageInfo: {
      hasNextPage: true,
      hasPreviousPage: false,
      startCursor: 'refund-start',
      endCursor: 'refund-next',
    },
    edges: [],
  },
  marketplaceExternalRefundReconciliations: {
    pageInfo: {
      hasNextPage: true,
      hasPreviousPage: false,
      startCursor: 'cursor-start',
      endCursor: 'cursor-next',
    },
    edges: [],
  },
  marketplaceRefunds: [],
};

describe('Spaces refund management Relay query', () => {
  beforeEach(() => {
    commitResolveMock.mockReset();
    loadQueryMock.mockReset();
    useKnownParamsMock.mockReset();
    useMutationMock.mockReset();
    usePreloadedQueryMock.mockReset();
    useQueryLoaderMock.mockReset();

    useKnownParamsMock.mockReturnValue({ organizationCustomDomain: 'spaces.example' });
    useQueryLoaderMock.mockReturnValue([{}, loadQueryMock]);
    useMutationMock.mockReturnValue([commitResolveMock]);
    usePreloadedQueryMock.mockReturnValue(rootData);
  });

  it('reloads Relay with filter variables and the current page cursor', async () => {
    render(<RootPage />);

    await waitFor(() =>
      expect(loadQueryMock).toHaveBeenNthCalledWith(
        1,
        {
          organizationCustomDomain: 'spaces.example',
          externalFirst: 50,
          externalStatus: 'Open',
          refundFirst: 50,
          refundRequestedAtFrom: '2026-07-01T00:00:00.000Z',
          refundRequestedAtTo: '2026-08-01T23:59:59.999Z',
        },
        { fetchPolicy: 'store-and-network' },
      ),
    );
  });
});

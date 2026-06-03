import { describe, expect, it, vi } from 'vitest';

const mockLocalFont = vi.fn(({ variable }: { variable: string; src: unknown }) => ({
  variable,
  className: variable,
}));

vi.mock('next/font/local', () => ({ default: mockLocalFont }));

vi.mock('next/headers', () => ({
  headers: vi.fn().mockResolvedValue({ get: () => null }),
}));

vi.mock('@skedular/shared', () => ({
  getProductAppDefinition: vi.fn(() => ({ id: 'webapp' })),
}));

vi.mock('./auth/host-utils', () => ({
  isOrganizationCustomDomainHost: vi.fn(() => false),
}));

vi.mock('./client-root-layout', () => ({
  default: ({ children }: { children: React.ReactNode }) => <div data-testid="client-root">{children}</div>,
}));

vi.mock('./fonts.css', () => ({}));

describe('RootLayout', () => {
  it('registers localFont with --font-barlow variable', async () => {
    await import('./layout');
    const barlowCall = mockLocalFont.mock.calls.find((call) => (call[0] as { variable?: string })?.variable === '--font-barlow');
    expect(barlowCall).toBeDefined();
  });

  it('has no .ttf path references in the barlow font config', async () => {
    await import('./layout');
    const barlowCall = mockLocalFont.mock.calls.find((call) => (call[0] as { variable?: string })?.variable === '--font-barlow');
    if (barlowCall) {
      const arg = barlowCall[0] as { src: Array<{ path: string }> };
      arg.src.forEach((s) => expect(s.path).not.toMatch(/\.ttf$/));
    }
  });
});

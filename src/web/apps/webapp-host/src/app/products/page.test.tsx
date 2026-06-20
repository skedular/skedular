import { describe, expect, it, vi } from 'vitest';
import Page from './page';

const { redirect } = vi.hoisted(() => ({
  redirect: vi.fn(),
}));

vi.mock('next/navigation', () => ({
  redirect,
}));

describe('products legacy route redirect', () => {
  it('redirects to locations index', () => {
    Page();
    expect(redirect).toHaveBeenCalledWith('/locations');
  });
});

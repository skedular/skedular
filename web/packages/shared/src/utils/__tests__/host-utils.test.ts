import { describe, expect, it } from 'vitest';
import { getHostFromUrl, getOrganizationCustomDomainFromHost, isOrganizationCustomDomainHost, normalizeHost } from '../host-utils';

describe('host-utils', () => {
  it('normalizes host casing, ports, and trailing dots', () => {
    expect(normalizeHost('Trial.Spaces.Skedular.App:3000.')).toBe('trial.spaces.skedular.app');
    expect(getHostFromUrl('https://Teams.Skedular.App/callback')).toBe('teams.skedular.app');
  });

  it('does not treat root app hosts as organization custom domains', () => {
    expect(getOrganizationCustomDomainFromHost('skedular.app')).toBeNull();
    expect(getOrganizationCustomDomainFromHost('staging.skedular.app')).toBeNull();
    expect(getOrganizationCustomDomainFromHost('spaces.skedular.app')).toBeNull();
    expect(getOrganizationCustomDomainFromHost('staging.teams.skedular.app')).toBeNull();
  });

  it('reads the organization label before the product app host label', () => {
    expect(getOrganizationCustomDomainFromHost('trial.spaces.skedular.app')).toBe('trial');
    expect(getOrganizationCustomDomainFromHost('trial.teams.skedular.app')).toBe('trial');
    expect(getOrganizationCustomDomainFromHost('trial.staging.spaces.skedular.app')).toBe('trial');
    expect(getOrganizationCustomDomainFromHost('trial.staging.teams.skedular.app')).toBe('trial');
  });

  it('uses configured registered app hosts as the organization boundary', () => {
    expect(getOrganizationCustomDomainFromHost('spaces.custom.company.test', ['https://spaces.custom.company.test'])).toBeNull();
    expect(getOrganizationCustomDomainFromHost('trial.spaces.custom.company.test', ['https://spaces.custom.company.test'])).toBe('trial');
    expect(getOrganizationCustomDomainFromHost('trial.teams.custom.company.test', ['teams.custom.company.test'])).toBe('trial');
  });

  it('keeps the legacy first-label organization custom domain for direct subdomains', () => {
    expect(getOrganizationCustomDomainFromHost('trial.skedular.app')).toBe('trial');
    expect(getOrganizationCustomDomainFromHost('trial.staging.skedular.app')).toBe('trial');
  });

  it('reports whether a host resolves to an organization custom domain', () => {
    expect(isOrganizationCustomDomainHost('spaces.skedular.app')).toBe(false);
    expect(isOrganizationCustomDomainHost('trial.spaces.skedular.app')).toBe(true);
  });
});

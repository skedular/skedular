import { describe, expect, it } from 'vitest';
import { createAppShellModel, getOrganisationEmptyStateCopy } from '../app-shell-model';

describe('app-shell-model', () => {
  it('creates a Teams shell model with private organization scope', () => {
    const shell = createAppShellModel({ appId: 'webapp-teams' });

    expect(shell.title).toBe('Skedular Teams');
    expect(shell.organisationTypes).toEqual(['private']);
  });

  it('creates a Spaces shell model with marketplace organization scope', () => {
    const shell = createAppShellModel({ appId: 'webapp-spaces' });

    expect(shell.title).toBe('Skedular Spaces');
    expect(shell.organisationTypes).toEqual(['marketplace']);
  });

  it('returns app-specific empty state copy', () => {
    expect(getOrganisationEmptyStateCopy('webapp-teams').title).toContain('private organizations');
    expect(getOrganisationEmptyStateCopy('webapp-spaces').title).toContain('co-working organizations');
  });
});

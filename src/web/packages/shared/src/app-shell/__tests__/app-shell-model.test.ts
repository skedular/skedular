import { describe, expect, it } from 'vitest';
import { createAppShellModel, getOrganisationEmptyStateCopy } from '../app-shell-model';

describe('app-shell-model', () => {
  it('creates a Teams shell model with private organisation scope', () => {
    const shell = createAppShellModel({ appId: 'webapp-teams' });

    expect(shell.title).toBe('WebApp Teams');
    expect(shell.organisationTypes).toEqual(['private']);
  });

  it('creates a Spaces shell model with marketplace organisation scope', () => {
    const shell = createAppShellModel({ appId: 'webapp-spaces' });

    expect(shell.title).toBe('WebApp Spaces');
    expect(shell.organisationTypes).toEqual(['marketplace']);
  });

  it('returns app-specific empty state copy', () => {
    expect(getOrganisationEmptyStateCopy('webapp-teams').title).toContain('private organisations');
    expect(getOrganisationEmptyStateCopy('webapp-spaces').title).toContain('co-working organisations');
  });
});

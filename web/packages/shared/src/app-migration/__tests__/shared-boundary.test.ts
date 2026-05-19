import { describe, expect, it } from 'vitest';
import { filterOrganisationsByType } from '../../app-shell';

describe('shared boundary guards', () => {
  it('keeps app-specific organisation ownership outside the neutral filter helper', () => {
    const functionSource = filterOrganisationsByType.toString();

    expect(functionSource).not.toContain('webapp-teams');
    expect(functionSource).not.toContain('webapp-spaces');
    expect(functionSource).not.toContain('PRIVATE');
    expect(functionSource).not.toContain('MARKETPLACE');
  });
});

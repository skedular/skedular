import { describe, expect, it } from 'vitest';
import { filterOrganisationsByType, type SelectableOrganisationSummary } from '../organisation-selection';

type TestOrganisation = SelectableOrganisationSummary<'PRIVATE' | 'MARKETPLACE' | 'INDIVIDUAL'>;

const organisations: TestOrganisation[] = [
  { id: 'private-1', name: 'Private Workspace', customDomain: 'private-workspace', type: 'PRIVATE' },
  { id: 'marketplace-1', name: 'Co-working Space', customDomain: 'coworking-space', type: 'MARKETPLACE' },
  { id: 'individual-1', name: 'Individual Workspace', customDomain: 'individual-workspace', type: 'INDIVIDUAL' },
];

describe('shared organisation selection helpers', () => {
  it('filters by caller-provided organisation types', () => {
    expect(filterOrganisationsByType(organisations, ['PRIVATE'])).toEqual([organisations[0]]);
    expect(filterOrganisationsByType(organisations, ['MARKETPLACE'])).toEqual([organisations[1]]);
  });
});

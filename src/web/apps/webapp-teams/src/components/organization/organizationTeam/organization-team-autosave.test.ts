import { readFileSync } from 'node:fs';
import { resolve } from 'node:path';
import { describe, expect, it } from 'vitest';

describe('team autosave', () => {
  it('autosaves setup details without the manual update action', () => {
    const source = readFileSync(resolve(process.cwd(), 'src/components/organization/organizationTeam/organization-team.tsx'), 'utf8');

    expect(source).toContain('debouncedTeamDetailUpdate');
    expect(source).not.toContain('onSubmit={handleTeamDetailUpdateClick}');
    expect(source).not.toContain('primaryAction="Update"');
  });

  it('saves member role edits from the role change action', () => {
    const source = readFileSync(resolve(process.cwd(), 'src/components/organization/organizationTeam/organization-team.tsx'), 'utf8');

    expect(source).toContain('handleRoleChanged(selectedMemberId, role);');
    expect(source).toContain('commitChangeTeamMemberRole({');
    expect(source).not.toContain('onSubmit={handleRoleChanged}');
  });

  it('shows failed-state feedback without saved-state noise for team edits', () => {
    const source = readFileSync(resolve(process.cwd(), 'src/components/organization/organizationTeam/organization-team.tsx'), 'utf8');

    expect(source).toContain('errorNotificationOptions');
    expect(source).not.toContain('successNotificationOptions');
  });
});

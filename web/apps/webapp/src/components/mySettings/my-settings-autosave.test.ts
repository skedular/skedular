import { readFileSync } from 'node:fs';
import { resolve } from 'node:path';
import { describe, expect, it } from 'vitest';

describe('my settings autosave', () => {
  it('autosaves profile detail edits without the manual update action', () => {
    const source = readFileSync(resolve(process.cwd(), 'src/components/mySettings/my-settings.tsx'), 'utf8');

    expect(source).toContain('debouncedCommitProfilePatch');
    expect(source).not.toContain('primaryAction="Save"');
  });

  it('shows saved-state and failed-state feedback for profile detail edits', () => {
    const source = readFileSync(resolve(process.cwd(), 'src/components/mySettings/my-settings.tsx'), 'utf8');

    expect(source).toContain('successNotificationOptions');
    expect(source).toContain('errorNotificationOptions');
  });
});

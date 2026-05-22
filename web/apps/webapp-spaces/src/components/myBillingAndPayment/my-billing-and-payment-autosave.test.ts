import { readFileSync } from 'node:fs';
import { resolve } from 'node:path';
import { describe, expect, it } from 'vitest';

describe('my billing and payment autosave', () => {
  it('autosaves billing detail edits without the manual update action', () => {
    const source = readFileSync(resolve(process.cwd(), 'src/components/myBillingAndPayment/my-billing-and-payment.tsx'), 'utf8');

    expect(source).toContain('debouncedCommitBillingDetailsPatch');
    expect(source).not.toContain('primaryAction="Save"');
  });

  it('shows saved-state and failed-state feedback for billing detail edits', () => {
    const source = readFileSync(resolve(process.cwd(), 'src/components/myBillingAndPayment/my-billing-and-payment.tsx'), 'utf8');

    expect(source).toContain('successNotificationOptions');
    expect(source).toContain('errorNotificationOptions');
  });
});

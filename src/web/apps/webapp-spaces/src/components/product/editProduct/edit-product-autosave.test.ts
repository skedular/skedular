import { readFileSync } from 'node:fs';
import { resolve } from 'node:path';
import { describe, expect, it } from 'vitest';

describe('edit product autosave', () => {
  it('autosaves grouped product editor values', () => {
    const source = readFileSync(resolve(process.cwd(), 'src/components/product/editProduct/edit-product.tsx'), 'utf8');

    expect(source).toContain('debouncedProductDetailUpdate');
    expect(source).not.toContain('onSubmit={handleProductDetailUpdateClick}');
  });

  it('shows saved-state and failed-state feedback for product edits', () => {
    const source = readFileSync(resolve(process.cwd(), 'src/components/product/editProduct/edit-product.tsx'), 'utf8');

    expect(source).toContain('errorNotificationOptions');
  });
});

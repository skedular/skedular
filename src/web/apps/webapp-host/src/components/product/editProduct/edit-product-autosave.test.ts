import { readFileSync } from 'node:fs';
import { resolve } from 'node:path';
import { describe, expect, it } from 'vitest';

describe('edit product autosave', () => {
  it('autosaves grouped product editor values', () => {
    const source = readFileSync(resolve(process.cwd(), 'src/components/product/editProduct/edit-product.tsx'), 'utf8');

    expect(source).toContain('debouncedProductDetailUpdate');
    expect(source).not.toContain('onSubmit={handleProductDetailUpdateClick}');
  });

  it('validates only the fields included in the autosave patch', () => {
    const source = readFileSync(resolve(process.cwd(), 'src/components/product/editProduct/edit-product.tsx'), 'utf8');

    expect(source).toContain('getValidProductPatchFields');
    expect(source).toContain('productDetailsSchema.validateSyncAt(formField, productDetails)');
    expect(source).toContain('fieldsToUpdate: validFieldsToUpdate');
    expect(source).not.toContain('productDetailsSchema.isValidSync');
  });

  it('does not treat regenerated pricing option form ids as autosave changes', () => {
    const source = readFileSync(resolve(process.cwd(), 'src/components/product/editProduct/edit-product.tsx'), 'utf8');

    expect(source).toContain('getComparableProductFieldValue');
    expect(source).toContain('productDetails.pricingOptions.map(({ id, ...pricingOption }) =>');
    expect(source).toContain('void id');
    expect(source).toContain('getComparableProductFieldValue(left, f)');
    expect(source).toContain('getComparableProductFieldValue(right, f)');
  });

  it('shows saved-state and failed-state feedback for product edits', () => {
    const source = readFileSync(resolve(process.cwd(), 'src/components/product/editProduct/edit-product.tsx'), 'utf8');

    expect(source).toContain('errorNotificationOptions');
  });
});

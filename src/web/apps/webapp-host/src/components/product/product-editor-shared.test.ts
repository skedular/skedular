import { createPricingOption, productSchema, sanitizeWeeklyRequiredDays } from './product-editor-shared';
import { describe, expect, it } from 'vitest';

describe('weekly price day-selection editor state', () => {
  it('initializes the exact weekly day count as optional', () => {
    expect(createPricingOption(15).requiredDaysPerWeek).toBe('');
  });

  it('keeps only one whole-number digit in the weekly required-day field', () => {
    expect(sanitizeWeeklyRequiredDays('-1.5abc23')).toBe('1');
  });

  it('accepts an exact count within the available weekly days', async () => {
    await expect(
      productSchema(30).validateAt('pricingOptions[0].requiredDaysPerWeek', {
        pricingOptions: [{ cadence: 'WEEKLY', availableDays: ['MONDAY', 'TUESDAY'], requiredDaysPerWeek: '2' }],
      }),
    ).resolves.toBe('2');
  });

  it('rejects a count greater than the available weekly days', async () => {
    await expect(
      productSchema(30).validateAt('pricingOptions[0].requiredDaysPerWeek', {
        pricingOptions: [{ cadence: 'WEEKLY', availableDays: ['MONDAY', 'TUESDAY'], requiredDaysPerWeek: '3' }],
      }),
    ).rejects.toThrow('Set the required number of selected days');
  });

  it('treats an empty available-day list as all seven weekdays', async () => {
    await expect(
      productSchema(30).validateAt('pricingOptions[0].requiredDaysPerWeek', {
        pricingOptions: [{ cadence: 'WEEKLY', availableDays: [], requiredDaysPerWeek: '7' }],
      }),
    ).resolves.toBe('7');
  });

  it('rejects a weekly count on nonweekly pricing', async () => {
    await expect(
      productSchema(30).validateAt('pricingOptions[0].requiredDaysPerWeek', {
        pricingOptions: [{ cadence: 'MONTHLY', availableDays: [], requiredDaysPerWeek: '1' }],
      }),
    ).rejects.toThrow('Set the required number of selected days');
  });

  it('allows separate weekly price tiers with different required day counts', async () => {
    const weeklyPricingOption = (requiredDaysPerWeek: string) => ({
      ...createPricingOption(15),
      cadence: 'WEEKLY',
      price: '100',
      minDurationMinutes: '30',
      maxDurationMinutes: '60',
      billingMode: 'UPFRONT',
      acceptedPaymentMethods: ['CARD'],
      availableDays: ['MONDAY', 'TUESDAY', 'WEDNESDAY'],
      requiredDaysPerWeek,
    });

    await expect(
      productSchema(30).validateAt('pricingOptions', {
        pricingOptions: [weeklyPricingOption('2'), weeklyPricingOption('3')],
      }),
    ).resolves.toHaveLength(2);
  });
});

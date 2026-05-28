import { describe, expect, it } from 'vitest';
import {
  convertStringToLowercaseExceptFirstLetter,
  decodeBase64,
  encodeBase64,
  formatPriceForDisplay,
  getMonthlyPricingCadenceMonthCount,
  stringCollectionToString,
  stringToMultiLines,
  toFixed,
} from '../constants';

describe('encodeBase64 / decodeBase64', () => {
  it('round-trips a plain string', () => {
    const original = 'hello world';
    expect(decodeBase64(encodeBase64(original))).toBe(original);
  });

  it('encodes to a different string than the original', () => {
    expect(encodeBase64('hello')).not.toBe('hello');
  });
});

describe('convertStringToLowercaseExceptFirstLetter', () => {
  it('capitalises the first letter and lowercases the rest', () => {
    expect(convertStringToLowercaseExceptFirstLetter('hELLO')).toBe('Hello');
  });

  it('returns empty string for null', () => {
    expect(convertStringToLowercaseExceptFirstLetter(null)).toBe('');
  });

  it('returns empty string for undefined', () => {
    expect(convertStringToLowercaseExceptFirstLetter(undefined)).toBe('');
  });
});

describe('toFixed', () => {
  it('rounds to 2 decimal places when fractionDigits is 2', () => {
    expect(toFixed(1.234, 2)).toBe(1.23);
  });

  it('rounds to the specified number of digits', () => {
    expect(toFixed(1.12345, 3)).toBe(1.123);
  });

  it('returns an integer when fractionDigits is 0', () => {
    expect(toFixed(1.9, 0)).toBe(2);
  });
});

describe('stringToMultiLines', () => {
  it('splits newline-separated string into trimmed lines', () => {
    expect(stringToMultiLines('a\n b\nc')).toEqual(['a', 'b', 'c']);
  });

  it('returns empty array for null', () => {
    expect(stringToMultiLines(null)).toEqual([]);
  });

  it('returns empty array for undefined', () => {
    expect(stringToMultiLines(undefined)).toEqual([]);
  });
});

describe('stringCollectionToString', () => {
  it('joins array with newlines', () => {
    expect(stringCollectionToString(['a', 'b', 'c'])).toBe('a\nb\nc');
  });

  it('returns empty string for null', () => {
    expect(stringCollectionToString(null)).toBe('');
  });
});

describe('getMonthlyPricingCadenceMonthCount', () => {
  it('returns 12 for YEARLY cadence', () => {
    expect(getMonthlyPricingCadenceMonthCount('YEARLY')).toBe(12);
  });

  it('returns 3 for QUARTERLY cadence', () => {
    expect(getMonthlyPricingCadenceMonthCount('QUARTERLY')).toBe(3);
  });

  it('returns null for unknown cadence', () => {
    expect(getMonthlyPricingCadenceMonthCount('WEEKLY')).toBeNull();
  });

  it('returns null for null input', () => {
    expect(getMonthlyPricingCadenceMonthCount(null)).toBeNull();
  });
});

describe('formatPriceForDisplay', () => {
  it('formats a whole-number price', () => {
    const result = formatPriceForDisplay('USD', 50);
    expect(result).toContain('50');
  });

  it('handles string amount', () => {
    const result = formatPriceForDisplay('USD', '29.99');
    expect(result).toContain('29.99');
  });
});

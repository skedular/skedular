import { describe, expect, it } from 'vitest';
import { getCustomerAvatarLetters, getCustomerFullName, getCustomerShortName } from '../name-utils';

describe('getCustomerShortName', () => {
  it('returns empty string when nameDetails is null', () => {
    expect(getCustomerShortName(null)).toBe('');
  });

  it('returns empty string when nameDetails is undefined', () => {
    expect(getCustomerShortName(undefined)).toBe('');
  });

  it('returns givenName when present', () => {
    expect(getCustomerShortName({ givenName: 'Alice', familyName: 'Smith' })).toBe('Alice');
  });

  it('returns middleName when givenName is absent', () => {
    expect(getCustomerShortName({ middleName: 'Marie' })).toBe('Marie');
  });

  it('returns name when only name is set', () => {
    expect(getCustomerShortName({ name: 'Acme Corp' })).toBe('Acme Corp');
  });
});

describe('getCustomerFullName', () => {
  it('returns empty string when nameDetails is null', () => {
    expect(getCustomerFullName(null)).toBe('');
  });

  it('returns name when present', () => {
    expect(getCustomerFullName({ name: 'Acme Corp' })).toBe('Acme Corp');
  });

  it('returns givenName + familyName when both present', () => {
    expect(getCustomerFullName({ givenName: 'Alice', familyName: 'Smith' })).toBe('Alice Smith');
  });

  it('returns givenName alone when familyName is absent', () => {
    expect(getCustomerFullName({ givenName: 'Alice' })).toBe('Alice');
  });

  it('returns familyName alone when givenName is absent', () => {
    expect(getCustomerFullName({ familyName: 'Smith' })).toBe('Smith');
  });

  it('returns middleName as fallback', () => {
    expect(getCustomerFullName({ middleName: 'Marie' })).toBe('Marie');
  });
});

describe('getCustomerAvatarLetters', () => {
  it('returns empty string when nameDetails is null', () => {
    expect(getCustomerAvatarLetters(null)).toBe('');
  });

  it('returns initials from givenName and familyName', () => {
    expect(getCustomerAvatarLetters({ givenName: 'Alice', familyName: 'Smith' })).toBe('AS');
  });

  it('returns letters from name words when no given/family name', () => {
    expect(getCustomerAvatarLetters({ name: 'Acme Corp Ltd' })).toBe('ACL');
  });

  it('returns empty string when no usable name fields', () => {
    expect(getCustomerAvatarLetters({})).toBe('');
  });
});

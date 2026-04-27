export type NameDetails = {
  name?: string | null;
  givenName?: string | null;
  middleName?: string | null;
  familyName?: string | null;
};

export const getCustomerShortName = (nameDetails?: NameDetails | null) => {
  if (!nameDetails) {
    return '';
  }

  if (nameDetails.givenName) {
    return nameDetails.givenName;
  }

  if (nameDetails.middleName) {
    return nameDetails.middleName;
  }

  if (nameDetails.familyName) {
    return nameDetails.middleName;
  }

  if (nameDetails.name) {
    return nameDetails.name;
  }

  return '';
};

export const getCustomerFullName = (nameDetails?: NameDetails | null) => {
  if (!nameDetails) {
    return '';
  }

  if (nameDetails.name) {
    return nameDetails.name;
  }

  if (nameDetails.givenName && nameDetails.familyName) {
    return `${nameDetails.givenName} ${nameDetails.familyName}`;
  }

  if (nameDetails.givenName && !nameDetails.familyName) {
    return nameDetails.givenName;
  }

  if (!nameDetails.givenName && nameDetails.familyName) {
    return nameDetails.familyName;
  }

  if (nameDetails.middleName) {
    return nameDetails.middleName;
  }

  return '';
};

export const getCustomerAvatarLetters = (nameDetails?: NameDetails | null) => {
  if (!nameDetails) {
    return '';
  }

  let avatarLetters = '';

  if (nameDetails) {
    if (nameDetails.givenName && nameDetails.familyName) {
      avatarLetters = `${nameDetails.givenName[0]}${nameDetails.familyName[0]}`;
    } else if (nameDetails.name && typeof nameDetails.name[0] !== 'undefined') {
      avatarLetters = nameDetails.name.split(' ').reduce((acc, val) => acc + val[0], '');
    } else {
      avatarLetters = '';
    }
  } else {
    avatarLetters = '';
  }

  return avatarLetters;
};

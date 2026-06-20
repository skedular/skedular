export type CreateListingFormValues = {
  locationName: string;
  addressLine1: string;
  city: string;
  country: string;
  timezone: string;
};

export const validateCreateListing = (values: CreateListingFormValues) => {
  const errors: string[] = [];
  if (!values.locationName.trim()) errors.push('Location name is required.');
  if (!values.addressLine1.trim()) errors.push('Street address is required.');
  if (!values.country.trim()) errors.push('Country is required.');

  return errors;
};

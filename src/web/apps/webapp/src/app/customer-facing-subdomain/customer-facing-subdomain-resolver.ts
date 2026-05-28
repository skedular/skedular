import type { CustomerFacingEntryPoint, CustomerFacingEntryPointInput } from './customer-facing-entry-point';

export const resolveCustomerFacingEntryPoint = ({ isCustomDomain, organizationType }: CustomerFacingEntryPointInput): CustomerFacingEntryPoint => {
  if (!isCustomDomain) {
    return 'public-discovery';
  }

  if (organizationType === 'private') {
    return 'private-organisation-subdomain';
  }

  return 'co-working-subdomain';
};

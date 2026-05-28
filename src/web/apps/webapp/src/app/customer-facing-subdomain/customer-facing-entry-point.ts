export type CustomerFacingOrganisationType = 'marketplace' | 'private';

export type CustomerFacingEntryPoint = 'public-discovery' | 'co-working-subdomain' | 'private-organisation-subdomain';

export type CustomerFacingEntryPointInput = {
  isCustomDomain: boolean;
  organizationType?: CustomerFacingOrganisationType;
};

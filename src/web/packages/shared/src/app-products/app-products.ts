export const productAppIds = ['webapp', 'webapp-teams', 'webapp-spaces'] as const;

export type ProductAppId = (typeof productAppIds)[number];

export const organisationTypes = ['private', 'marketplace'] as const;

export type OrganisationType = (typeof organisationTypes)[number];

export const customerEntryTypes = ['root', 'marketplace-subdomain', 'private-organisation-subdomain'] as const;

export type CustomerEntryType = (typeof customerEntryTypes)[number];

export type ProductAppDefinition = {
  id: ProductAppId;
  name: string;
  shortName: string;
  purpose: string;
  allowedOrganisationTypes: readonly OrganisationType[];
  customerEntryTypes: readonly CustomerEntryType[];
};

export const productAppDefinitions = {
  webapp: {
    id: 'webapp',
    name: 'Skedular',
    shortName: 'Skedular',
    purpose: 'Customer-facing public discovery and customer organization entry points.',
    allowedOrganisationTypes: [],
    customerEntryTypes,
  },
  'webapp-spaces': {
    id: 'webapp-spaces',
    name: 'Skedular Spaces',
    shortName: 'Spaces',
    purpose: 'Marketplace and co-working organization operator workflows.',
    allowedOrganisationTypes: ['marketplace'],
    customerEntryTypes: [],
  },
  'webapp-teams': {
    id: 'webapp-teams',
    name: 'Skedular Teams',
    shortName: 'Teams',
    purpose: 'Private organization and team workflows.',
    allowedOrganisationTypes: ['private'],
    customerEntryTypes: [],
  },
} as const satisfies Record<ProductAppId, ProductAppDefinition>;

export const isProductAppId = (value: string): value is ProductAppId => productAppIds.includes(value as ProductAppId);

export const isOrganisationType = (value: string): value is OrganisationType => organisationTypes.includes(value as OrganisationType);

export const isCustomerEntryType = (value: string): value is CustomerEntryType => customerEntryTypes.includes(value as CustomerEntryType);

export const getProductAppDefinition = (appId: ProductAppId): ProductAppDefinition => productAppDefinitions[appId];

export const getAllowedOrganisationTypes = (appId: ProductAppId): readonly OrganisationType[] => getProductAppDefinition(appId).allowedOrganisationTypes;

export const canSelectOrganisationTypeInApp = (appId: ProductAppId, organisationType: OrganisationType): boolean => getAllowedOrganisationTypes(appId).includes(organisationType);

export const hasMarketplaceConcepts = (appId: ProductAppId): boolean => appId !== 'webapp-teams';

export const hasPrivateOrganisationConcepts = (appId: ProductAppId): boolean => appId !== 'webapp-spaces';

export const getCustomerEntryTypes = (appId: ProductAppId): readonly CustomerEntryType[] => getProductAppDefinition(appId).customerEntryTypes;

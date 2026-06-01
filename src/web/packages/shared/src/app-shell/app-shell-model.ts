import { getAllowedOrganisationTypes, getProductAppDefinition, type OrganisationType, type ProductAppId } from '../app-products';

export type AppShellNavigationItem = {
  label: string;
  href: string;
  appId?: ProductAppId;
  disabled?: boolean;
};

export type AppShellModel = {
  appId: ProductAppId;
  title: string;
  description: string;
  organisationTypes: readonly OrganisationType[];
  navigationItems: readonly AppShellNavigationItem[];
  reviewNote?: string;
};

export type CreateAppShellModelInput = {
  appId: ProductAppId;
  title?: string;
  description?: string;
  navigationItems?: readonly AppShellNavigationItem[];
  reviewNote?: string;
};

export const createAppShellModel = ({ appId, title, description, navigationItems = [], reviewNote }: CreateAppShellModelInput): AppShellModel => {
  const appDefinition = getProductAppDefinition(appId);

  return {
    appId,
    title: title ?? appDefinition.name,
    description: description ?? appDefinition.purpose,
    organisationTypes: getAllowedOrganisationTypes(appId),
    navigationItems,
    reviewNote,
  };
};

export const getOrganisationEmptyStateCopy = (appId: ProductAppId): { title: string; description: string; actionLabel: string } => {
  if (appId === 'webapp-teams') {
    return {
      title: 'No private organizations available',
      description: 'Create or join a private organization before using Teams workflows.',
      actionLabel: 'Create private organization',
    };
  }

  if (appId === 'webapp-spaces') {
    return {
      title: 'No co-working organizations available',
      description: 'Create or join a marketplace organization before using Spaces workflows.',
      actionLabel: 'Create co-working organization',
    };
  }

  return {
    title: 'No organization selected',
    description: 'Customer-facing discovery remains available without selecting an organization.',
    actionLabel: 'Browse locations',
  };
};

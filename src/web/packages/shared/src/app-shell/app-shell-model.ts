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
      title: 'No private organisations available',
      description: 'Create or join a private organisation before using Teams workflows.',
      actionLabel: 'Create private organisation',
    };
  }

  if (appId === 'webapp-spaces') {
    return {
      title: 'No co-working organisations available',
      description: 'Create or join a marketplace organisation before using Spaces workflows.',
      actionLabel: 'Create co-working organisation',
    };
  }

  return {
    title: 'No organisation selected',
    description: 'Customer-facing discovery remains available without selecting an organisation.',
    actionLabel: 'Browse locations',
  };
};

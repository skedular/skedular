import { getProductAppDefinition, productAppIds, type ProductAppId } from '../app-products';

export type AppSwitcherDestinationAvailability = 'available' | 'current' | 'missing-url' | 'invalid-url';

export type AppSwitcherConfiguration = {
  currentAppId: ProductAppId;
  destinations: Partial<Record<ProductAppId, string | null | undefined>>;
};

export type AppSwitcherDestination = {
  appId: ProductAppId;
  displayName: string;
  shortName: string;
  href?: string;
  isCurrent: boolean;
  availability: AppSwitcherDestinationAvailability;
};

export type AppSwitcherModel = {
  currentAppId: ProductAppId;
  destinations: readonly AppSwitcherDestination[];
  availableDestinationCount: number;
  hasSwitchTargets: boolean;
};

const normalizeDestinationUrl = (rawUrl: string | null | undefined): string | undefined => {
  if (!rawUrl?.trim()) {
    return undefined;
  }

  try {
    const url = new URL(rawUrl.trim());
    if (url.protocol !== 'http:' && url.protocol !== 'https:') {
      return undefined;
    }

    return url.href;
  } catch {
    return undefined;
  }
};

export const createAppSwitcherModel = ({ currentAppId, destinations }: AppSwitcherConfiguration): AppSwitcherModel => {
  const resolvedDestinations = productAppIds.map<AppSwitcherDestination>((appId) => {
    const appDefinition = getProductAppDefinition(appId);
    const isCurrent = appId === currentAppId;
    const rawUrl = destinations[appId];
    const href = normalizeDestinationUrl(rawUrl);
    const availability: AppSwitcherDestinationAvailability = isCurrent ? 'current' : !rawUrl?.trim() ? 'missing-url' : href ? 'available' : 'invalid-url';

    return {
      appId,
      displayName: appDefinition.name,
      shortName: appDefinition.shortName,
      href,
      isCurrent,
      availability,
    };
  });

  const availableDestinationCount = resolvedDestinations.filter((destination) => destination.availability === 'available').length;

  return {
    currentAppId,
    destinations: resolvedDestinations,
    availableDestinationCount,
    hasSwitchTargets: availableDestinationCount > 0,
  };
};

export const getAvailableAppSwitcherDestinations = (model: AppSwitcherModel): readonly AppSwitcherDestination[] =>
  model.destinations.filter((destination) => destination.availability === 'available');

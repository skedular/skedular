const localHosts = new Set(['', 'localhost', '127.0.0.1']);

const defaultRegisteredHosts = [
  'skedular.app',
  'staging.skedular.app',
  'www.skedular.app',
  'spaces.skedular.app',
  'staging.spaces.skedular.app',
  'teams.skedular.app',
  'staging.teams.skedular.app',
];

export const normalizeHost = (value: string | null | undefined) => (value ?? '').split(':')[0]?.replace(/\.$/, '').toLowerCase() ?? '';

const getHostLabels = (host: string) => host.split('.').filter((label) => label.length > 0);

export const getHostFromUrl = (value: string | null | undefined) => {
  if (!value) {
    return '';
  }

  try {
    return normalizeHost(new URL(value).host);
  } catch {
    return normalizeHost(value);
  }
};

const getRegisteredHosts = (registeredHosts: readonly (string | null | undefined)[] = []) =>
  [...registeredHosts.map(getHostFromUrl), getHostFromUrl(process.env.NEXT_PUBLIC_SITE_URL), ...defaultRegisteredHosts.map(getHostFromUrl)]
    .filter((host) => host.length > 0)
    .filter((host, index, hosts) => hosts.indexOf(host) === index);

const getRegisteredHostMatch = (host: string, registeredHosts: readonly (string | null | undefined)[] = []) => {
  const hostLabels = getHostLabels(host);

  return getRegisteredHosts(registeredHosts)
    .map((registeredHost) => getHostLabels(registeredHost))
    .filter((registeredHostLabels) => hostLabels.length >= registeredHostLabels.length)
    .sort((left, right) => right.length - left.length)
    .find((registeredHostLabels) => registeredHostLabels.every((label, index) => hostLabels[hostLabels.length - registeredHostLabels.length + index] === label));
};

export const getOrganizationCustomDomainFromHost = (value: string | null | undefined, registeredHosts: readonly (string | null | undefined)[] = []) => {
  const host = normalizeHost(value);

  if (localHosts.has(host)) {
    return null;
  }

  const registeredHostLabels = getRegisteredHostMatch(host, registeredHosts);

  if (!registeredHostLabels) {
    return getHostLabels(host)[0] || null;
  }

  const hostLabels = getHostLabels(host);
  const prefixLabels = hostLabels.slice(0, hostLabels.length - registeredHostLabels.length);

  if (prefixLabels.length === 0) {
    return null;
  }

  return prefixLabels[prefixLabels.length - 1] || null;
};

export const isOrganizationCustomDomainHost = (value: string | null | undefined, registeredHosts: readonly (string | null | undefined)[] = []) =>
  getOrganizationCustomDomainFromHost(value, registeredHosts) !== null;

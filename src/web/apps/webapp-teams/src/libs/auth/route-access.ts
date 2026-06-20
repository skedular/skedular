const unauthenticatedPaths = new Set(['/', '/callback', '/signin', '/signup']);

export const isUnauthenticatedPath = (pathname: string) => unauthenticatedPaths.has(pathname) || pathname.startsWith('/auth/');

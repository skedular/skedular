// force-dynamic prevents Next.js from statically prerendering this route at
// build time. The shell components for these custom-domain storefront pages
// resolve the organisation by reading window.location.hostname at runtime
// (e.g. acme.skedular.app → 'acme'). During static prerendering window is
// undefined, so organizationCustomDomain would be '' and the guard inside
// UnauthenticatedOrganizationStoreFrontRootShell would throw. The actual
// SSR suppression is done in client-page.tsx via { ssr: false }.
export const dynamic = 'force-dynamic';

export { default } from './client-page';

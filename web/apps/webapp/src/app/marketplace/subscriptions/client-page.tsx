'use client';

import dynamic from 'next/dynamic';

// ssr: false ensures the shell is never rendered server-side. These
// storefront pages live under a custom subdomain (e.g. acme.skedular.app)
// with no [organizationCustomDomain] segment in the URL path, so the
// organization is resolved from window.location.hostname at runtime.
// That value is unavailable in Node.js, which would cause the
// organizationCustomDomain guard to throw. See page.tsx for why
// force-dynamic is also required.
const SubscriptionsPage = dynamic(() => import('@/rootPages/marketplace/subscriptions/page'), { ssr: false });

export default SubscriptionsPage;

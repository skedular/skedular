import { AppShellLayout, PageHeaderPanel } from '@skedular/ui';

const PrivateOrganizationSubdomain = () => (
  <div data-customer-facing-entry="private-organisation-subdomain">
    <AppShellLayout
      appName="WebApp"
      title="Private organisation"
      description="Customer-facing private organisation access."
      reviewNote="Review shell only. Data-backed private organisation storefront is not wired yet."
    >
      <PageHeaderPanel title="Private organisation" description="This entry point will show customer-facing private organisation content without exposing other organisations." />
    </AppShellLayout>
  </div>
);

export default PrivateOrganizationSubdomain;

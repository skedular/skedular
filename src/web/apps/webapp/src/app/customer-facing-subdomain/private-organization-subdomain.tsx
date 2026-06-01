import { AppShellLayout, PageHeaderPanel } from '@skedular/ui';

const PrivateOrganizationSubdomain = () => (
  <div data-customer-facing-entry="private-organisation-subdomain">
    <AppShellLayout
      appName="Skedular"
      title="Private organization"
      description="Customer-facing private organization access."
      reviewNote="Review shell only. Data-backed private organization storefront is not wired yet."
    >
      <PageHeaderPanel title="Private organization" description="This entry point will show customer-facing private organization content without exposing other organizations." />
    </AppShellLayout>
  </div>
);

export default PrivateOrganizationSubdomain;

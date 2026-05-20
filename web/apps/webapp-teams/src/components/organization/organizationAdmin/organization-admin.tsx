import { BodyIconTypography, LeadIconTypography, StackColumn } from '@skedular/ui';
import type { organizationAdmin_query$key } from '@/queries/__generated__/organizationAdmin_query.graphql';
import Box from '@mui/material/Box';
import { PageHeaderPanel } from '@skedular/ui';
import { memo, useEffect, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
import { defaultPadding } from '@skedular/ui';
import { useSearchParams } from 'next/navigation';
import OrganizationAdminBillingPaymentSection from './organization-admin-billing-payment-section';
import OrganizationAdminManageOrganizationSection from './organization-admin-manage-organization-section';
import OrganizationAdminPhysicalAddressSection from './organization-admin-physical-address-section';
import OrganizationAdminSectionNav, { OrganizationAdminSection } from './organization-admin-section-nav';
import OrganizationAdminSetupSection from './organization-admin-setup-section';
import OrganizationAdminSsoSection from './organization-admin-sso-section';
import OrganizationAdminSubscriptionsSection from './organization-admin-subscriptions-section';
import OrganizationAdminTagsSection from './organization-admin-tags-section';
import OrganizationAdminZonesSection from './organization-admin-zones-section';

type Props = {
  rootDataRelay: organizationAdmin_query$key;
  organizationCustomDomain: string;
};

const validSections: OrganizationAdminSection[] = [
  'setup',
  'physical-address-setup',
  'billing-payment-setup',
  'sso-setup',
  'zones-setup',
  'tags-setup',
  'subscriptions',
  'manage-organization',
];

const getActiveSection = (value: string | null): OrganizationAdminSection => {
  if (value && validSections.includes(value as OrganizationAdminSection)) {
    return value as OrganizationAdminSection;
  }

  return 'setup';
};

const OrganizationAdmin = ({ rootDataRelay, organizationCustomDomain }: Props) => {
  const rootData = useFragment<organizationAdmin_query$key>(
    graphql`
      fragment organizationAdmin_query on Query {
        organization(customDomain: $organizationCustomDomain) {
          name
          listingMetadata {
            title
          }
          marketplaceListingMetadata {
            title
          }
        }
      }
    `,
    rootDataRelay,
  );
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const activeSection = useMemo(() => getActiveSection(section), [section]);
  const [stickyTop, setStickyTop] = useState(0);

  useEffect(() => {
    const updateStickyTop = () => {
      setStickyTop(document.querySelector('.app-bar')?.clientHeight ?? 0);
    };

    updateStickyTop();
    window.addEventListener('resize', updateStickyTop);

    return () => {
      window.removeEventListener('resize', updateStickyTop);
    };
  }, []);

  const organization = rootData.organization;

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 0, sm: 1, md: 2 }, pb: defaultPadding }}>
      <StackColumn
        sx={{
          width: '100%',
          maxWidth: 1200,
          mx: 'auto',
          pt: { xs: 1, sm: 1, md: 2 },
          backgroundColor: 'transparent',
          gap: 2,
        }}
      >
        <PageHeaderPanel
          eyebrow="Organization admin"
          title={organization?.name ?? 'Organization settings'}
          description="Manage identity, billing, addresses, access, tags, subscriptions, and lifecycle controls."
        >
          <StackColumn spacing={0.5}>
            <LeadIconTypography label="Settings & controls" />
            <BodyIconTypography
              label={organization?.listingMetadata?.title || organization?.marketplaceListingMetadata?.title || 'Billing, address, identity, tags, and subscriptions'}
            />
          </StackColumn>
        </PageHeaderPanel>

        <OrganizationAdminSectionNav activeSection={activeSection} organizationCustomDomain={organizationCustomDomain} stickyTop={stickyTop} />

        {activeSection === 'setup' && <OrganizationAdminSetupSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'physical-address-setup' && <OrganizationAdminPhysicalAddressSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'billing-payment-setup' && <OrganizationAdminBillingPaymentSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'sso-setup' && <OrganizationAdminSsoSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'zones-setup' && <OrganizationAdminZonesSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'tags-setup' && <OrganizationAdminTagsSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'subscriptions' && <OrganizationAdminSubscriptionsSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'manage-organization' && <OrganizationAdminManageOrganizationSection organizationCustomDomain={organizationCustomDomain} />}
      </StackColumn>
    </Box>
  );
};

export default memo(OrganizationAdmin);

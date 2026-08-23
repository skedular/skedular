import { getOrganizationIntegrationsBaseLink } from '@/components/links';
import OrganizationMarketplaceSetupLoader from '@/components/organization/organizationMarketplaceSetup/organization-marketplace-setup-loader';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { useIntegratedPlatform } from '@skedular/shared';
import { defaultPadding, PageHeaderPanel, StackColumn, StackRow } from '@skedular/ui';
import NextLink from 'next/link';
import { memo, useMemo } from 'react';
import { useSearchParams } from 'next/navigation';

type Props = { organizationCustomDomain: string };

const OrganizationIntegration = ({ organizationCustomDomain }: Props) => {
  const searchParams = useSearchParams();
  const { integratedPlatform } = useIntegratedPlatform();
  const tab = searchParams.get('tab');
  const activeTab = useMemo(() => (tab === 'xero-setup' || tab === 'stripe-connect-accounts-setup' ? tab : 'stripe-connect-accounts-setup'), [tab]);
  const integrationsBaseLink = getOrganizationIntegrationsBaseLink(integratedPlatform, organizationCustomDomain);
  const tabs = ['stripe-connect-accounts-setup'] as const;

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 0, sm: 1, md: 2 }, pb: defaultPadding }}>
      <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', pt: { xs: 1, sm: 1, md: 2 }, gap: 2 }}>
        <PageHeaderPanel eyebrow="Integrations" title="Integrations" description="Manage connections to the external systems used by this organization." />
        <StackRow sx={{ overflowX: 'auto', gap: 1, p: 1, border: 1, borderColor: 'divider', borderRadius: 4, bgcolor: 'background.paper' }}>
          {tabs.map((item) => (
            <Button
              key={item}
              component={NextLink}
              href={`${integrationsBaseLink.split('?')[0]}?tab=${item}`}
              variant={activeTab === item ? 'contained' : 'outlined'}
              color="primary"
              sx={{ flexShrink: 0, borderRadius: 999, px: 2, textTransform: 'none' }}
            >
              Stripe
            </Button>
          ))}
        </StackRow>
        <Box sx={{ width: '100%' }}>
          <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />
        </Box>
      </StackColumn>
    </Box>
  );
};

export default memo(OrganizationIntegration);

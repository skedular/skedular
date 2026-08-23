import { getOrganizationIntegrationsBaseLink } from '@/components/links';
import OrganizationMarketplaceSetupLoader from '@/components/organization/organizationMarketplaceSetup/organization-marketplace-setup-loader';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { useIntegratedPlatform } from '@skedular/shared';
import { defaultPadding, PageHeaderPanel, StackColumn, StackRow } from '@skedular/ui';
import NextLink from 'next/link';
import { memo } from 'react';
import { useSearchParams } from 'next/navigation';

type Props = { organizationCustomDomain: string };
const OrganizationIntegration = ({ organizationCustomDomain }: Props) => {
  const searchParams = useSearchParams();
  const { integratedPlatform } = useIntegratedPlatform();
  const tab = searchParams.get('tab');
  const activeTab = tab === 'xero-setup' || tab === 'stripe-connect-accounts-setup' ? tab : 'xero-setup';
  const integrationsBaseLink = getOrganizationIntegrationsBaseLink(integratedPlatform, organizationCustomDomain);
  const tabs = ['stripe-connect-accounts-setup', 'xero-setup'] as const;
  const labels = { 'xero-setup': 'Xero', 'stripe-connect-accounts-setup': 'Stripe' };
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
              {labels[item]}
            </Button>
          ))}
        </StackRow>
        <Box
          sx={{
            width: '100%',
            borderRadius: 4,
            border: 1,
            borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
            bgcolor: (theme) => (theme.palette.mode === 'light' ? 'common.white' : theme.palette.background.paper),
            boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 12px 32px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
            overflow: 'hidden',
          }}
        >
          <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />
        </Box>
      </StackColumn>
    </Box>
  );
};
export default memo(OrganizationIntegration);

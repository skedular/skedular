import { getOrganizationIntegrationsBaseLink } from '@/components/links';
import OrganizationMarketplaceSetupLoader from '@/components/organization/organizationMarketplaceSetup/organization-marketplace-setup-loader';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import ExpandMoreRoundedIcon from '@mui/icons-material/ExpandMoreRounded';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import { useIntegratedPlatform } from '@skedular/shared';
import { defaultPadding, PageHeaderPanel, StackColumn } from '@skedular/ui';
import NextLink from 'next/link';
import { memo, useState } from 'react';
import { useSearchParams } from 'next/navigation';

type Props = { organizationCustomDomain: string };
const OrganizationIntegration = ({ organizationCustomDomain }: Props) => {
  const searchParams = useSearchParams();
  const { integratedPlatform } = useIntegratedPlatform();
  const theme = useTheme();
  const isMobileIntegrationNav = useMediaQuery(theme.breakpoints.down('sm'), { noSsr: true });
  const [integrationMenuAnchor, setIntegrationMenuAnchor] = useState<HTMLElement | null>(null);
  const tab = searchParams.get('tab');
  const activeTab = tab === 'xero-setup' || tab === 'stripe-connect-accounts-setup' ? tab : 'xero-setup';
  const integrationsBaseLink = getOrganizationIntegrationsBaseLink(integratedPlatform, organizationCustomDomain);
  const tabs = ['stripe-connect-accounts-setup', 'xero-setup'] as const;
  const labels = { 'xero-setup': 'Xero', 'stripe-connect-accounts-setup': 'Stripe' };
  const renderIntegrationTabs = () =>
    isMobileIntegrationNav ? (
      <Box sx={{ borderTop: 1, borderColor: 'divider', pt: 1.5 }}>
        <Button
          fullWidth
          variant="outlined"
          color="inherit"
          onClick={(event) => setIntegrationMenuAnchor(event.currentTarget)}
          aria-haspopup="menu"
          aria-expanded={integrationMenuAnchor ? 'true' : undefined}
          aria-controls={integrationMenuAnchor ? 'organization-integration-sections-menu' : undefined}
          endIcon={<ExpandMoreRoundedIcon />}
          sx={{ justifyContent: 'space-between', minHeight: 48, borderRadius: 2.5, px: 2, textTransform: 'none' }}
        >
          {`Section: ${labels[activeTab]}`}
        </Button>
        <Menu anchorEl={integrationMenuAnchor} open={Boolean(integrationMenuAnchor)} onClose={() => setIntegrationMenuAnchor(null)} id="organization-integration-sections-menu">
          {tabs.map((item) => (
            <MenuItem
              key={item}
              component={NextLink}
              href={`${integrationsBaseLink.split('?')[0]}?tab=${item}`}
              selected={activeTab === item}
              onClick={() => setIntegrationMenuAnchor(null)}
            >
              {labels[item]}
            </MenuItem>
          ))}
        </Menu>
      </Box>
    ) : (
      <Tabs
        value={activeTab}
        variant="scrollable"
        scrollButtons="auto"
        aria-label="Integration sections"
        sx={{ mb: -2, borderTop: 1, borderColor: 'divider', '& .MuiTabs-indicator': { height: 3, borderRadius: '3px 3px 0 0' } }}
      >
        {tabs.map((item) => (
          <Tab
            key={item}
            value={item}
            component={NextLink}
            href={`${integrationsBaseLink.split('?')[0]}?tab=${item}`}
            label={labels[item]}
            disableRipple
            sx={{
              minWidth: 112,
              minHeight: 52,
              px: 2.5,
              textTransform: 'none',
              color: 'text.secondary',
              fontWeight: 500,
              '&.Mui-selected': { color: 'primary.main', fontWeight: 600 },
              '&:hover': { color: 'text.primary', backgroundColor: 'action.hover' },
            }}
          />
        ))}
      </Tabs>
    );

  return (
    <Box
      sx={{
        width: '100%',
        maxWidth: '100vw',
        minWidth: 0,
        display: 'flex',
        justifyContent: 'center',
        overflowX: 'hidden',
        boxSizing: 'border-box',
        px: { xs: 0, sm: 1, md: 2 },
        pb: defaultPadding,
      }}
    >
      <StackColumn sx={{ width: '100%', maxWidth: 1200, minWidth: 0, mx: 'auto', overflowX: 'hidden', pt: { xs: 1, sm: 1, md: 2 }, gap: 2 }}>
        <PageHeaderPanel
          eyebrow="Integrations"
          title="Integrations"
          description="Manage connections to the external systems used by this organization."
          sx={{ width: '100%', minWidth: 0, maxWidth: '100%' }}
        >
          {renderIntegrationTabs()}
        </PageHeaderPanel>
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

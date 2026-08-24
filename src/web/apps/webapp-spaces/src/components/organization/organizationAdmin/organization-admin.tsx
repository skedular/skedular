import {
  getOrganizationAdminCustomTagsBaseLink,
  getOrganizationAdminManageOrganizationBaseLink,
  getOrganizationAdminPhysicalAddressBaseLink,
  getOrganizationAdminSetupBaseLink,
  getOrganizationAdminSetupBillingCycleBaseLink,
  getOrganizationAdminSetupMarketplaceListingBaseLink,
  getOrganizationAdminSsoSettingsBaseLink,
  getOrganizationAdminSubscriptionsBaseLink,
  getOrganizationAdminTaxDetailsBaseLink,
  getOrganizationAdminZonesBaseLink,
  getOrganizationIntegrationsBaseLink,
  getOrganizationMarketplaceSetupBankAccountsBaseLink,
  getOrganizationMarketplaceSetupProductTagsBaseLink,
  getOrganizationMarketplaceSetupStripeConnectAccountsBaseLink,
  getOrganizationMarketplaceSetupXeroBaseLink,
} from '@/components/links';
import OrganizationMarketplaceSetupLoader from '@/components/organization/organizationMarketplaceSetup/organization-marketplace-setup-loader';
import type { organizationAdmin_query$key } from '@/queries/__generated__/organizationAdmin_query.graphql';
import ExpandMoreRoundedIcon from '@mui/icons-material/ExpandMoreRounded';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import { useIntegratedPlatform } from '@skedular/shared';
import { BodyIconTypography, defaultPadding, LeadIconTypography, PageHeaderPanel, StackColumn } from '@skedular/ui';
import NextLink from 'next/link';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { memo, PropsWithChildren, useEffect, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
import OrganizationAdminBillingPaymentSection from './organization-admin-billing-payment-section';
import OrganizationAdminManageOrganizationSection from './organization-admin-manage-organization-section';
import OrganizationAdminPhysicalAddressSection from './organization-admin-physical-address-section';
import OrganizationAdminSetupSection from './organization-admin-setup-section';
import OrganizationAdminSsoSection from './organization-admin-sso-section';
import OrganizationAdminSubscriptionsSection from './organization-admin-subscriptions-section';
import OrganizationAdminTagsSection from './organization-admin-tags-section';
import OrganizationAdminTaxDetailsSection from './organization-admin-tax-details-section';
import OrganizationAdminZonesSection from './organization-admin-zones-section';

type Props = {
  rootDataRelay: organizationAdmin_query$key;
  organizationCustomDomain: string;
  tagsGroupsMode?: boolean;
};

type EditorSectionProps = { title: string; description: string; summary: string; expanded: boolean; onChange: () => void };
const EditorSection = ({ title, description, summary, expanded, onChange, children }: PropsWithChildren<EditorSectionProps>) => (
  <Accordion
    disableGutters
    elevation={0}
    expanded={expanded}
    onChange={onChange}
    sx={{ margin: 0, border: 1, borderColor: 'divider', borderRadius: '16px !important', overflow: 'hidden', '&::before': { display: 'none' } }}
  >
    <AccordionSummary expandIcon={<ExpandMoreRoundedIcon />} sx={{ px: 2.5, py: 0.75, minHeight: 72, '& .MuiAccordionSummary-content': { my: 1 } }}>
      <StackColumn spacing={0.35}>
        <LeadIconTypography label={title} />
        <BodyIconTypography label={expanded ? description : summary} />
      </StackColumn>
    </AccordionSummary>
    <AccordionDetails sx={{ borderTop: 1, borderColor: 'divider', p: { xs: 2, sm: 2.5 } }}>{children}</AccordionDetails>
  </Accordion>
);

type OrganizationAdminSection =
  | 'setup'
  | 'marketplace-listing'
  | 'billing-cycle'
  | 'xero-setup'
  | 'stripe-connect-accounts-setup'
  | 'bank-accounts-setup'
  | 'product-tags-setup'
  | 'physical-address-setup'
  | 'sso-setup'
  | 'tax-details-setup'
  | 'zones-setup'
  | 'tags-setup'
  | 'subscriptions'
  | 'manage-organization';

const validSections: OrganizationAdminSection[] = [
  'setup',
  'marketplace-listing',
  'billing-cycle',
  'xero-setup',
  'stripe-connect-accounts-setup',
  'bank-accounts-setup',
  'product-tags-setup',
  'physical-address-setup',
  'sso-setup',
  'tax-details-setup',
  'zones-setup',
  'tags-setup',
  'subscriptions',
  'manage-organization',
];

const getActiveSection = (value: string | null): OrganizationAdminSection | null => {
  if (value && validSections.includes(value as OrganizationAdminSection)) {
    return value as OrganizationAdminSection;
  }

  return null;
};

const sectionLabels: Record<OrganizationAdminSection, string> = {
  setup: 'Organisation profile',
  'marketplace-listing': 'Marketplace listing',
  'billing-cycle': 'Billing cadence',
  'xero-setup': 'Xero',
  'stripe-connect-accounts-setup': 'Stripe',
  'bank-accounts-setup': 'Bank accounts',
  'product-tags-setup': 'Booking groups',
  'physical-address-setup': 'Address',
  'sso-setup': 'SSO',
  'tax-details-setup': 'Tax',
  'zones-setup': 'Zones',
  'tags-setup': 'Tags',
  subscriptions: 'Subscriptions',
  'manage-organization': 'Manage organisation',
};

const OrganizationAdmin = ({ rootDataRelay, organizationCustomDomain, tagsGroupsMode = false }: Props) => {
  const rootData = useFragment<organizationAdmin_query$key>(
    graphql`
      fragment organizationAdmin_query on Query {
        organization(customDomain: $organizationCustomDomain) {
          name
          physicalAddress {
            formattedAddress
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
  const pathname = usePathname();
  const { integratedPlatform } = useIntegratedPlatform();
  const integrationsMode = pathname.endsWith('/integrations');
  const adminTab = searchParams.get('tab') ?? 'profile';
  const theme = useTheme();
  const isMobileAdminNav = useMediaQuery(theme.breakpoints.down('sm'), { noSsr: true });
  const [adminTabsMenuAnchor, setAdminTabsMenuAnchor] = useState<HTMLElement | null>(null);
  const section = integrationsMode ? searchParams.get('tab') : searchParams.get('section');
  const activeSection = useMemo(() => getActiveSection(section) ?? (integrationsMode ? 'xero-setup' : null), [integrationsMode, section]);
  const router = useRouter();
  useEffect(() => {
    if (section === 'billing-payment-setup') router.replace('?tab=profile&section=billing-details');
    if (section === 'subscriptions') router.replace('?tab=profile&section=plan');
    if (section === 'billing-cycle') router.replace('?tab=profile&section=billing-cadence');
    if (section === 'setup' && searchParams.get('profileSection')) router.replace(`?tab=profile&section=${searchParams.get('profileSection')}`);
  }, [router, section, searchParams]);
  const profileSection = section ?? 'presentation';
  const setExpandedProfileSection = (section: string) => {
    const params = new URLSearchParams(searchParams.toString());
    params.set('tab', 'profile');
    if (section) params.set('section', section);
    else params.delete('section');
    router.replace(`${pathname}?${params.toString()}`, { scroll: false });
  };
  const expandedProfileSection = profileSection;

  const organization = rootData.organization;
  const sectionLinks: Record<OrganizationAdminSection, string> = {
    setup: getOrganizationAdminSetupBaseLink(integratedPlatform, organizationCustomDomain),
    'physical-address-setup': getOrganizationAdminPhysicalAddressBaseLink(integratedPlatform, organizationCustomDomain),
    'tax-details-setup': getOrganizationAdminTaxDetailsBaseLink(integratedPlatform, organizationCustomDomain),
    'marketplace-listing': getOrganizationAdminSetupMarketplaceListingBaseLink(integratedPlatform, organizationCustomDomain),
    'product-tags-setup': getOrganizationMarketplaceSetupProductTagsBaseLink(integratedPlatform, organizationCustomDomain),
    'zones-setup': getOrganizationAdminZonesBaseLink(integratedPlatform, organizationCustomDomain),
    'tags-setup': getOrganizationAdminCustomTagsBaseLink(integratedPlatform, organizationCustomDomain),
    'billing-cycle': getOrganizationAdminSetupBillingCycleBaseLink(integratedPlatform, organizationCustomDomain),
    'xero-setup': getOrganizationMarketplaceSetupXeroBaseLink(integratedPlatform, organizationCustomDomain),
    'stripe-connect-accounts-setup': getOrganizationMarketplaceSetupStripeConnectAccountsBaseLink(integratedPlatform, organizationCustomDomain),
    'bank-accounts-setup': getOrganizationMarketplaceSetupBankAccountsBaseLink(integratedPlatform, organizationCustomDomain),
    'sso-setup': getOrganizationAdminSsoSettingsBaseLink(integratedPlatform, organizationCustomDomain),
    subscriptions: getOrganizationAdminSubscriptionsBaseLink(integratedPlatform, organizationCustomDomain),
    'manage-organization': getOrganizationAdminManageOrganizationBaseLink(integratedPlatform, organizationCustomDomain),
  };
  const integrationsBaseLink = getOrganizationIntegrationsBaseLink(integratedPlatform, organizationCustomDomain);

  const renderAdminTabs = () => {
    const tabs = [
      ['profile', 'Profile'],
      ['bank-accounts', 'Bank Accounts'],
      ['operations', 'Operations'],
    ];

    return isMobileAdminNav ? (
      <Box sx={{ borderTop: 1, borderColor: 'divider', pt: 1.5 }}>
        <Button
          fullWidth
          variant="outlined"
          color="inherit"
          onClick={(event) => setAdminTabsMenuAnchor(event.currentTarget)}
          aria-haspopup="menu"
          aria-expanded={adminTabsMenuAnchor ? 'true' : undefined}
          aria-controls={adminTabsMenuAnchor ? 'organization-admin-sections-menu' : undefined}
          endIcon={<ExpandMoreRoundedIcon />}
          sx={{ justifyContent: 'space-between', minHeight: 48, borderRadius: 2.5, px: 2, textTransform: 'none' }}
        >
          {`Section: ${tabs.find(([key]) => key === adminTab)?.[1] ?? 'Profile'}`}
        </Button>
        <Menu anchorEl={adminTabsMenuAnchor} open={Boolean(adminTabsMenuAnchor)} onClose={() => setAdminTabsMenuAnchor(null)} id="organization-admin-sections-menu">
          {tabs.map(([key, label]) => (
            <MenuItem
              key={key}
              component={NextLink}
              href={`${pathname}?tab=${key}${key === 'profile' ? '&section=presentation' : ''}`}
              selected={adminTab === key}
              onClick={() => setAdminTabsMenuAnchor(null)}
            >
              {label}
            </MenuItem>
          ))}
        </Menu>
      </Box>
    ) : (
      <Tabs
        value={adminTab}
        variant="scrollable"
        scrollButtons="auto"
        aria-label="Organization admin sections"
        sx={{ mb: -2, borderTop: 1, borderColor: 'divider', '& .MuiTabs-indicator': { height: 3, borderRadius: '3px 3px 0 0' } }}
      >
        {tabs.map(([key, label]) => (
          <Tab
            key={key}
            value={key}
            component={NextLink}
            href={`${pathname}?tab=${key}${key === 'profile' ? '&section=presentation' : ''}`}
            label={label}
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
  };

  const renderTagsGroupsTabs = () => {
    const tabs = ['tags-setup', 'zones-setup', 'product-tags-setup'] as OrganizationAdminSection[];

    return isMobileAdminNav ? (
      <Box sx={{ borderTop: 1, borderColor: 'divider', pt: 1.5 }}>
        <Button
          fullWidth
          variant="outlined"
          color="inherit"
          onClick={(event) => setAdminTabsMenuAnchor(event.currentTarget)}
          aria-haspopup="menu"
          aria-expanded={adminTabsMenuAnchor ? 'true' : undefined}
          aria-controls={adminTabsMenuAnchor ? 'organization-tags-groups-sections-menu' : undefined}
          endIcon={<ExpandMoreRoundedIcon />}
          sx={{ justifyContent: 'space-between', minHeight: 48, borderRadius: 2.5, px: 2, textTransform: 'none' }}
        >
          {`Section: ${activeSection ? sectionLabels[activeSection] : sectionLabels['tags-setup']}`}
        </Button>
        <Menu anchorEl={adminTabsMenuAnchor} open={Boolean(adminTabsMenuAnchor)} onClose={() => setAdminTabsMenuAnchor(null)} id="organization-tags-groups-sections-menu">
          {tabs.map((item) => (
            <MenuItem key={item} component={NextLink} href={sectionLinks[item]} selected={activeSection === item} onClick={() => setAdminTabsMenuAnchor(null)}>
              {sectionLabels[item]}
            </MenuItem>
          ))}
        </Menu>
      </Box>
    ) : (
      <Tabs
        value={activeSection ?? false}
        variant="scrollable"
        scrollButtons="auto"
        aria-label="Tags and groups sections"
        sx={{ mb: -2, borderTop: 1, borderColor: 'divider', '& .MuiTabs-indicator': { height: 3, borderRadius: '3px 3px 0 0' } }}
      >
        {tabs.map((item) => (
          <Tab
            key={item}
            value={item}
            component={NextLink}
            href={sectionLinks[item]}
            label={sectionLabels[item]}
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
  };

  const renderOrganizationSummary = () => (
    <StackColumn sx={{ position: { md: 'sticky' }, top: { md: 16 }, alignSelf: 'flex-start' }}>
      <Card variant="outlined" sx={{ borderRadius: 3, width: '100%' }}>
        <CardContent>
          <StackColumn spacing={1.25}>
            <BodyIconTypography label="Summary" />
            <LeadIconTypography label={organization?.name ?? 'Organization'} />
            <BodyIconTypography label={organization?.physicalAddress?.formattedAddress ?? 'No physical address added'} />
          </StackColumn>
        </CardContent>
      </Card>
    </StackColumn>
  );

  const renderIntegrationTabs = () => {
    const tabs = ['xero-setup', 'stripe-connect-accounts-setup'] as OrganizationAdminSection[];

    return isMobileAdminNav ? (
      <Box sx={{ borderTop: 1, borderColor: 'divider', pt: 1.5 }}>
        <Button
          fullWidth
          variant="outlined"
          color="inherit"
          onClick={(event) => setAdminTabsMenuAnchor(event.currentTarget)}
          aria-haspopup="menu"
          aria-expanded={adminTabsMenuAnchor ? 'true' : undefined}
          aria-controls={adminTabsMenuAnchor ? 'organization-integration-sections-menu' : undefined}
          endIcon={<ExpandMoreRoundedIcon />}
          sx={{ justifyContent: 'space-between', minHeight: 48, borderRadius: 2.5, px: 2, textTransform: 'none' }}
        >
          {`Section: ${sectionLabels[activeSection as OrganizationAdminSection]}`}
        </Button>
        <Menu anchorEl={adminTabsMenuAnchor} open={Boolean(adminTabsMenuAnchor)} onClose={() => setAdminTabsMenuAnchor(null)} id="organization-integration-sections-menu">
          {tabs.map((item) => (
            <MenuItem
              key={item}
              component={NextLink}
              href={`${integrationsBaseLink.split('?')[0]}?tab=${item}`}
              selected={activeSection === item}
              onClick={() => setAdminTabsMenuAnchor(null)}
            >
              {sectionLabels[item]}
            </MenuItem>
          ))}
        </Menu>
      </Box>
    ) : (
      <Tabs
        value={activeSection ?? false}
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
            label={sectionLabels[item]}
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
  };

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
      <StackColumn
        sx={{
          width: '100%',
          maxWidth: 1200,
          minWidth: 0,
          mx: 'auto',
          overflowX: 'hidden',
          pt: { xs: 1, sm: 1, md: 2 },
          backgroundColor: 'transparent',
          gap: 2,
        }}
      >
        <PageHeaderPanel
          eyebrow={integrationsMode ? 'Integrations' : tagsGroupsMode ? 'Tags & Groups' : 'Organisation admin'}
          title={integrationsMode ? 'Integrations' : tagsGroupsMode ? 'Shared tags, zones & booking groups' : (organization?.name ?? 'Organisation settings')}
          description={
            integrationsMode
              ? 'Manage connections to the external systems used by this organization.'
              : tagsGroupsMode
                ? 'Manage tags, zones, and booking groups used across this organization.'
                : activeSection
                  ? `Editing ${sectionLabels[activeSection].toLocaleLowerCase()}.`
                  : 'Choose the area you want to configure for this marketplace organisation.'
          }
          sx={{ width: '100%', minWidth: 0, maxWidth: '100%' }}
        >
          {tagsGroupsMode && renderTagsGroupsTabs()}
          {integrationsMode && activeSection && renderIntegrationTabs()}
          {!activeSection && !tagsGroupsMode && !integrationsMode && renderAdminTabs()}
        </PageHeaderPanel>

        {!activeSection && !tagsGroupsMode && adminTab === 'profile' && (
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'minmax(0, 1fr) 300px' }, gap: { xs: 2, md: 3 }, alignItems: 'start' }}>
            <StackColumn spacing={1.5}>
              <OrganizationAdminSetupSection key={expandedProfileSection} organizationCustomDomain={organizationCustomDomain} />
              <EditorSection
                title="Marketplace listing"
                description="Manage the public listing details shown for this organization in the marketplace."
                summary="Marketplace listing details"
                expanded={expandedProfileSection === 'marketplace-listing'}
                onChange={() => setExpandedProfileSection(expandedProfileSection === 'marketplace-listing' ? '' : 'marketplace-listing')}
              >
                <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />
              </EditorSection>
              <EditorSection
                title="Billing cadence"
                description="Manage the billing cycle and default invoice payment terms for marketplace bookings."
                summary="Marketplace invoice settings"
                expanded={expandedProfileSection === 'billing-cadence'}
                onChange={() => setExpandedProfileSection(expandedProfileSection === 'billing-cadence' ? '' : 'billing-cadence')}
              >
                <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded section="billing-cycle" />
              </EditorSection>
              <EditorSection
                title="Physical address"
                description="Update the organization address used for internal records and operational context."
                summary="Organization address"
                expanded={expandedProfileSection === 'physical-address'}
                onChange={() => setExpandedProfileSection(expandedProfileSection === 'physical-address' ? '' : 'physical-address')}
              >
                <OrganizationAdminPhysicalAddressSection organizationCustomDomain={organizationCustomDomain} embedded />
              </EditorSection>
              <EditorSection
                title="Billing details"
                description="Manage the billing recipient and legal address used for invoices."
                summary="Invoice billing details"
                expanded={expandedProfileSection === 'billing-details'}
                onChange={() => setExpandedProfileSection(expandedProfileSection === 'billing-details' ? '' : 'billing-details')}
              >
                <OrganizationAdminBillingPaymentSection organizationCustomDomain={organizationCustomDomain} section="billing-details" />
              </EditorSection>
              <EditorSection
                title="Tax details"
                description="Manage the tax registration and rate used for organization billing."
                summary="Tax registration and rate"
                expanded={expandedProfileSection === 'tax-details'}
                onChange={() => setExpandedProfileSection(expandedProfileSection === 'tax-details' ? '' : 'tax-details')}
              >
                <OrganizationAdminTaxDetailsSection organizationCustomDomain={organizationCustomDomain} />
              </EditorSection>
              <EditorSection
                title="SSO"
                description="Configure enterprise sign-in and identity federation for organization members."
                summary="Enterprise sign-in settings"
                expanded={expandedProfileSection === 'sso'}
                onChange={() => setExpandedProfileSection(expandedProfileSection === 'sso' ? '' : 'sso')}
              >
                <OrganizationAdminSsoSection organizationCustomDomain={organizationCustomDomain} />
              </EditorSection>
              <EditorSection
                title="Plan"
                description="Manage the organization subscription tier and available features."
                summary="Organization subscription plan"
                expanded={expandedProfileSection === 'plan'}
                onChange={() => setExpandedProfileSection(expandedProfileSection === 'plan' ? '' : 'plan')}
              >
                <OrganizationAdminSubscriptionsSection organizationCustomDomain={organizationCustomDomain} />
              </EditorSection>
            </StackColumn>
            {renderOrganizationSummary()}
          </Box>
        )}
        {!activeSection && !tagsGroupsMode && adminTab === 'bank-accounts' && (
          <Box sx={{ width: '100%' }}>
            <Box
              sx={{
                borderRadius: 4,
                border: 1,
                borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
                bgcolor: (theme) => (theme.palette.mode === 'light' ? 'common.white' : theme.palette.background.paper),
                boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 12px 32px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
                overflow: 'hidden',
              }}
            >
              <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded section="bank-accounts-setup" />
            </Box>
          </Box>
        )}
        {!activeSection && !tagsGroupsMode && adminTab === 'operations' && (
          <Box sx={{ width: '100%' }}>
            <OrganizationAdminManageOrganizationSection organizationCustomDomain={organizationCustomDomain} />
          </Box>
        )}
        {activeSection === 'marketplace-listing' && <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />}
        {activeSection === 'billing-cycle' && <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />}
        {integrationsMode && (activeSection === 'xero-setup' || activeSection === 'stripe-connect-accounts-setup') && (
          <Box sx={{ border: 1, borderColor: 'divider', borderRadius: 4, bgcolor: 'background.paper', overflow: 'hidden' }}>
            <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />
          </Box>
        )}
        {!integrationsMode && activeSection === 'xero-setup' && <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />}
        {!integrationsMode && activeSection === 'stripe-connect-accounts-setup' && (
          <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />
        )}
        {activeSection === 'bank-accounts-setup' && <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />}
        {activeSection === 'product-tags-setup' && (
          <Box
            sx={{
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
        )}
        {activeSection === 'physical-address-setup' && <OrganizationAdminPhysicalAddressSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'sso-setup' && <OrganizationAdminSsoSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'tax-details-setup' && <OrganizationAdminTaxDetailsSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'zones-setup' && <OrganizationAdminZonesSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'tags-setup' && <OrganizationAdminTagsSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'subscriptions' && <OrganizationAdminSubscriptionsSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'manage-organization' && <OrganizationAdminManageOrganizationSection organizationCustomDomain={organizationCustomDomain} />}
      </StackColumn>
    </Box>
  );
};

export default memo(OrganizationAdmin);

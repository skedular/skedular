import { getOrganizationIntegrationsBaseLink } from '@/components/links';
import OrganizationMarketplaceSetupLoader from '@/components/organization/organizationMarketplaceSetup/organization-marketplace-setup-loader';
import type { organizationSettings_query$key } from '@/queries/__generated__/organizationSettings_query.graphql';
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
import OrganizationSettingsBillingPaymentSection from './organization-settings-billing-payment-section';
import OrganizationSettingsManageOrganizationSection from './organization-settings-manage-organization-section';
import OrganizationSettingsPhysicalAddressSection from './organization-settings-physical-address-section';
import OrganizationSettingsSetupSection from './organization-settings-setup-section';
import OrganizationSettingsSsoSection from './organization-settings-sso-section';
import OrganizationSettingsSubscriptionsSection from './organization-settings-subscriptions-section';
import OrganizationSettingsTaxDetailsSection from './organization-settings-tax-details-section';

type Props = {
  rootDataRelay: organizationSettings_query$key;
  organizationCustomDomain: string;
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

type OrganizationSettingsSection = 'setup' | 'marketplace-listing' | 'stripe-connect-accounts-setup' | 'physical-address-setup' | 'tax-details-setup' | 'manage-organization';

const validSections: OrganizationSettingsSection[] = [
  'setup',
  'marketplace-listing',
  'stripe-connect-accounts-setup',
  'physical-address-setup',
  'tax-details-setup',
  'manage-organization',
];

const getActiveSection = (value: string | null): OrganizationSettingsSection | null => {
  if (value && validSections.includes(value as OrganizationSettingsSection)) {
    return value as OrganizationSettingsSection;
  }

  return null;
};

const sectionLabels: Record<OrganizationSettingsSection, string> = {
  setup: 'Organization profile',
  'marketplace-listing': 'Marketplace listing',
  'stripe-connect-accounts-setup': 'Stripe',
  'physical-address-setup': 'Address',
  'tax-details-setup': 'Tax',
  'manage-organization': 'Manage organization',
};

const OrganizationSettings = ({ rootDataRelay, organizationCustomDomain }: Props) => {
  const rootData = useFragment<organizationSettings_query$key>(
    graphql`
      fragment organizationSettings_query on Query {
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
  const activeSection = useMemo(() => getActiveSection(section) ?? (integrationsMode ? 'stripe-connect-accounts-setup' : null), [integrationsMode, section]);
  const router = useRouter();
  useEffect(() => {
    if (section === 'marketplace-listing') router.replace('?tab=profile&section=marketplace-listing');
    if (section === 'billing-payment-setup') router.replace('?tab=profile&section=billing-details');
    if (section === 'subscriptions') router.replace('?tab=profile&section=plan');
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
  const integrationsBaseLink = getOrganizationIntegrationsBaseLink(integratedPlatform, organizationCustomDomain);

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

  const renderAdminTabs = () => {
    const tabs = [
      ['profile', 'Profile'],
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
          aria-controls={adminTabsMenuAnchor ? 'organization-settings-sections-menu' : undefined}
          endIcon={<ExpandMoreRoundedIcon />}
          sx={{ justifyContent: 'space-between', minHeight: 48, borderRadius: 2.5, px: 2, textTransform: 'none' }}
        >
          {`Section: ${tabs.find(([key]) => key === adminTab)?.[1] ?? 'Profile'}`}
        </Button>
        <Menu anchorEl={adminTabsMenuAnchor} open={Boolean(adminTabsMenuAnchor)} onClose={() => setAdminTabsMenuAnchor(null)} id="organization-settings-sections-menu">
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
        aria-label="Organization settings sections"
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

  const renderIntegrationTabs = () => {
    const tab = 'stripe-connect-accounts-setup' as OrganizationSettingsSection;

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
          {`Section: ${sectionLabels[tab]}`}
        </Button>
        <Menu anchorEl={adminTabsMenuAnchor} open={Boolean(adminTabsMenuAnchor)} onClose={() => setAdminTabsMenuAnchor(null)} id="organization-integration-sections-menu">
          <MenuItem component={NextLink} href={`${integrationsBaseLink.split('?')[0]}?tab=${tab}`} selected={activeSection === tab} onClick={() => setAdminTabsMenuAnchor(null)}>
            {sectionLabels[tab]}
          </MenuItem>
        </Menu>
      </Box>
    ) : (
      <Tabs
        value={activeSection ?? false}
        aria-label="Integration sections"
        sx={{ mb: -2, borderTop: 1, borderColor: 'divider', '& .MuiTabs-indicator': { height: 3, borderRadius: '3px 3px 0 0' } }}
      >
        <Tab
          value={tab}
          component={NextLink}
          href={`${integrationsBaseLink.split('?')[0]}?tab=${tab}`}
          label={sectionLabels[tab]}
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
          eyebrow={integrationsMode ? 'Integrations' : 'Organization settings'}
          title={integrationsMode ? 'Integrations' : (organization?.name ?? 'Organization settings')}
          description={
            integrationsMode
              ? 'Manage connections to the external systems used by this organization.'
              : activeSection
                ? `Editing ${sectionLabels[activeSection].toLocaleLowerCase()}.`
                : 'Choose the area you want to configure for this Host organization.'
          }
        >
          {integrationsMode && activeSection && renderIntegrationTabs()}
          {!activeSection && !integrationsMode && renderAdminTabs()}
        </PageHeaderPanel>

        {!activeSection && adminTab === 'profile' && (
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'minmax(0, 1fr) 300px' }, gap: { xs: 2, md: 3 }, alignItems: 'start' }}>
            <StackColumn spacing={1.5}>
              <OrganizationSettingsSetupSection organizationCustomDomain={organizationCustomDomain} />
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
                title="Physical address"
                description="Update the organization address used for internal records and operational context."
                summary="Organization address"
                expanded={expandedProfileSection === 'physical-address'}
                onChange={() => setExpandedProfileSection(expandedProfileSection === 'physical-address' ? '' : 'physical-address')}
              >
                <OrganizationSettingsPhysicalAddressSection organizationCustomDomain={organizationCustomDomain} embedded />
              </EditorSection>
              <EditorSection
                title="Billing details"
                description="Manage the billing recipient and legal address used for invoices."
                summary="Invoice billing details"
                expanded={expandedProfileSection === 'billing-details'}
                onChange={() => setExpandedProfileSection(expandedProfileSection === 'billing-details' ? '' : 'billing-details')}
              >
                <OrganizationSettingsBillingPaymentSection organizationCustomDomain={organizationCustomDomain} section="billing-details" />
              </EditorSection>
              <EditorSection
                title="Tax details"
                description="Manage the tax registration and rate used for organization billing."
                summary="Tax registration and rate"
                expanded={expandedProfileSection === 'tax-details'}
                onChange={() => setExpandedProfileSection(expandedProfileSection === 'tax-details' ? '' : 'tax-details')}
              >
                <OrganizationSettingsTaxDetailsSection organizationCustomDomain={organizationCustomDomain} />
              </EditorSection>
              <EditorSection
                title="SSO"
                description="Configure enterprise sign-in and identity federation for organization members."
                summary="Enterprise sign-in settings"
                expanded={expandedProfileSection === 'sso'}
                onChange={() => setExpandedProfileSection(expandedProfileSection === 'sso' ? '' : 'sso')}
              >
                <OrganizationSettingsSsoSection organizationCustomDomain={organizationCustomDomain} />
              </EditorSection>
              <EditorSection
                title="Plan"
                description="Manage the organization subscription tier and available features."
                summary="Organization subscription plan"
                expanded={expandedProfileSection === 'plan'}
                onChange={() => setExpandedProfileSection(expandedProfileSection === 'plan' ? '' : 'plan')}
              >
                <OrganizationSettingsSubscriptionsSection organizationCustomDomain={organizationCustomDomain} />
              </EditorSection>
            </StackColumn>
            {renderOrganizationSummary()}
          </Box>
        )}
        {!activeSection && adminTab === 'operations' && (
          <Box sx={{ width: '100%' }}>
            <OrganizationSettingsManageOrganizationSection organizationCustomDomain={organizationCustomDomain} />
          </Box>
        )}
        {activeSection === 'marketplace-listing' && <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />}
        {activeSection === 'stripe-connect-accounts-setup' && <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />}
        {activeSection === 'physical-address-setup' && <OrganizationSettingsPhysicalAddressSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'tax-details-setup' && <OrganizationSettingsTaxDetailsSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'manage-organization' && <OrganizationSettingsManageOrganizationSection organizationCustomDomain={organizationCustomDomain} />}
      </StackColumn>
    </Box>
  );
};

export default memo(OrganizationSettings);

import {
  getOrganizationAdminCustomTagsBaseLink,
  getOrganizationAdminManageOrganizationBaseLink,
  getOrganizationAdminPhysicalAddressBaseLink,
  getOrganizationAdminSetupBaseLink,
  getOrganizationAdminSsoSettingsBaseLink,
  getOrganizationAdminSubscriptionsBaseLink,
  getOrganizationAdminZonesBaseLink,
} from '@/components/links';
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
import { BodyIconTypography, defaultPadding, LeadIconTypography, PageHeaderPanel, StackColumn, StackRow } from '@skedular/ui';
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

type OrganizationAdminSection = 'setup' | 'physical-address-setup' | 'sso-setup' | 'zones-setup' | 'tags-setup' | 'subscriptions' | 'manage-organization';

const validSections: OrganizationAdminSection[] = ['setup', 'physical-address-setup', 'sso-setup', 'zones-setup', 'tags-setup', 'subscriptions', 'manage-organization'];

const getActiveSection = (value: string | null): OrganizationAdminSection | null => {
  if (value && validSections.includes(value as OrganizationAdminSection)) {
    return value as OrganizationAdminSection;
  }

  return null;
};

const sectionLabels: Record<OrganizationAdminSection, string> = {
  setup: 'Organisation profile',
  'physical-address-setup': 'Address',
  'sso-setup': 'SSO',
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
        }
      }
    `,
    rootDataRelay,
  );
  const searchParams = useSearchParams();
  const pathname = usePathname();
  const { integratedPlatform } = useIntegratedPlatform();
  const adminTab = searchParams.get('tab') ?? 'profile';
  const theme = useTheme();
  const isMobileAdminNav = useMediaQuery(theme.breakpoints.down('sm'), { noSsr: true });
  const [adminTabsMenuAnchor, setAdminTabsMenuAnchor] = useState<HTMLElement | null>(null);
  const section = searchParams.get('section');
  const activeSection = useMemo(() => getActiveSection(section), [section]);
  const router = useRouter();
  useEffect(() => {
    if (section === 'billing-payment-setup') router.replace('?tab=profile&section=billing-details');
    if (section === 'subscriptions') router.replace('?tab=profile&section=plan');
    if (section === 'setup' && searchParams.get('profileSection')) {
      router.replace(`?tab=profile&section=${searchParams.get('profileSection')}`);
    }
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
    'sso-setup': getOrganizationAdminSsoSettingsBaseLink(integratedPlatform, organizationCustomDomain),
    'zones-setup': getOrganizationAdminZonesBaseLink(integratedPlatform, organizationCustomDomain),
    'tags-setup': getOrganizationAdminCustomTagsBaseLink(integratedPlatform, organizationCustomDomain),
    subscriptions: getOrganizationAdminSubscriptionsBaseLink(integratedPlatform, organizationCustomDomain),
    'manage-organization': getOrganizationAdminManageOrganizationBaseLink(integratedPlatform, organizationCustomDomain),
  };
  const renderAdminTabs = () =>
    isMobileAdminNav ? (
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
          {`Section: ${adminTab === 'profile' ? 'Profile' : 'Operations'}`}
        </Button>
        <Menu anchorEl={adminTabsMenuAnchor} open={Boolean(adminTabsMenuAnchor)} onClose={() => setAdminTabsMenuAnchor(null)} id="organization-admin-sections-menu">
          {[
            ['profile', 'Profile'],
            ['operations', 'Operations'],
          ].map(([key, label]) => (
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
        sx={{
          mb: -2,
          borderTop: 1,
          borderColor: 'divider',
          '& .MuiTabs-indicator': { height: 3, borderRadius: '3px 3px 0 0' },
        }}
      >
        {[
          ['profile', 'Profile'],
          ['operations', 'Operations'],
        ].map(([key, label]) => (
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

  const renderTagsGroupsTabs = () => (
    <StackRow
      sx={{
        overflowX: 'auto',
        gap: 1,
        p: 1,
        border: 1,
        borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
        borderRadius: 4,
        bgcolor: (theme) => (theme.palette.mode === 'light' ? 'common.white' : theme.palette.background.paper),
        boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : 'none'),
      }}
    >
      {(['tags-setup', 'zones-setup'] as OrganizationAdminSection[]).map((item) => (
        <Button
          key={item}
          component={NextLink}
          href={sectionLinks[item]}
          variant={activeSection === item ? 'contained' : 'outlined'}
          color="primary"
          sx={{ borderRadius: 999, px: 2, textTransform: 'none', whiteSpace: 'nowrap' }}
        >
          {sectionLabels[item]}
        </Button>
      ))}
    </StackRow>
  );

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
          eyebrow={tagsGroupsMode ? 'Tags & Groups' : 'Organisation admin'}
          title={tagsGroupsMode ? 'Shared tags & zones' : (organization?.name ?? 'Organisation settings')}
          description={
            tagsGroupsMode
              ? 'Manage tags and zones used across this organization.'
              : activeSection
                ? `Editing ${sectionLabels[activeSection].toLocaleLowerCase()}.`
                : 'Choose the area you want to configure for this organisation.'
          }
          sx={{ width: '100%', minWidth: 0, maxWidth: '100%' }}
        >
          {!activeSection && !tagsGroupsMode && renderAdminTabs()}
        </PageHeaderPanel>

        {tagsGroupsMode && renderTagsGroupsTabs()}
        {!activeSection && !tagsGroupsMode && adminTab === 'profile' && (
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'minmax(0, 1fr) 300px' }, gap: { xs: 2, md: 3 }, alignItems: 'start' }}>
            <StackColumn spacing={1.5}>
              <OrganizationAdminSetupSection key={expandedProfileSection} organizationCustomDomain={organizationCustomDomain} />
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
        {!activeSection && !tagsGroupsMode && adminTab === 'operations' && (
          <Box sx={{ width: '100%' }}>
            <OrganizationAdminManageOrganizationSection organizationCustomDomain={organizationCustomDomain} />
          </Box>
        )}
        {activeSection === 'physical-address-setup' && <OrganizationAdminPhysicalAddressSection organizationCustomDomain={organizationCustomDomain} />}
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

import {
  getOrganizationAdminBaseLink,
  getOrganizationAdminCustomTagsBaseLink,
  getOrganizationAdminManageOrganizationBaseLink,
  getOrganizationAdminPhysicalAddressBaseLink,
  getOrganizationAdminSetupBaseLink,
  getOrganizationAdminSsoSettingsBaseLink,
  getOrganizationAdminSetupMarketplaceListingBaseLink,
  getOrganizationAdminSubscriptionsBaseLink,
  getOrganizationAdminTaxDetailsBaseLink,
  getOrganizationAdminZonesBaseLink,
  getOrganizationBaseLink,
  getOrganizationIntegrationsBaseLink,
  getOrganizationMarketplaceSetupBankAccountsBaseLink,
  getOrganizationMarketplaceSetupBillingCycleBaseLink,
  getOrganizationMarketplaceSetupProductTagsBaseLink,
  getOrganizationMarketplaceSetupStripeConnectAccountsBaseLink,
  getOrganizationMarketplaceSetupXeroBaseLink,
} from '@/components/links';
import OrganizationMarketplaceSetupLoader from '@/components/organization/organizationMarketplaceSetup/organization-marketplace-setup-loader';
import type { organizationAdmin_query$key } from '@/queries/__generated__/organizationAdmin_query.graphql';
import Box from '@mui/material/Box';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Divider from '@mui/material/Divider';
import ExpandMoreRoundedIcon from '@mui/icons-material/ExpandMoreRounded';
import { useIntegratedPlatform } from '@skedular/shared';
import { BodyIconTypography, defaultPadding, LeadIconTypography, PageHeaderPanel, StackColumn, StackRow } from '@skedular/ui';
import NextLink from 'next/link';
import { memo, PropsWithChildren, useEffect, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
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
  const section = integrationsMode ? searchParams.get('tab') : searchParams.get('section');
  const activeSection = useMemo(() => getActiveSection(section) ?? (integrationsMode ? 'xero-setup' : null), [integrationsMode, section]);
  const router = useRouter();
  useEffect(() => {
    if (section === 'billing-payment-setup') router.replace('?section=setup&profileSection=billing-details');
    if (section === 'subscriptions') router.replace('?section=setup&profileSection=plan');
  }, [router, section]);
  const profileSection = searchParams.get('profileSection') ?? 'presentation';
  const setExpandedProfileSection = (section: string) => {
    const params = new URLSearchParams(searchParams.toString());
    params.set('section', 'setup');
    if (section) params.set('profileSection', section);
    else params.delete('profileSection');
    router.replace(`?${params.toString()}`, { scroll: false });
  };
  const expandedProfileSection = profileSection;

  const organization = rootData.organization;
  const adminBaseLink = tagsGroupsMode
    ? `${getOrganizationBaseLink(integratedPlatform, organizationCustomDomain)}/tags-groups`
    : getOrganizationAdminBaseLink(integratedPlatform, organizationCustomDomain);
  const sectionLinks: Record<OrganizationAdminSection, string> = {
    setup: getOrganizationAdminSetupBaseLink(integratedPlatform, organizationCustomDomain),
    'physical-address-setup': getOrganizationAdminPhysicalAddressBaseLink(integratedPlatform, organizationCustomDomain),
    'tax-details-setup': getOrganizationAdminTaxDetailsBaseLink(integratedPlatform, organizationCustomDomain),
    'marketplace-listing': getOrganizationAdminSetupMarketplaceListingBaseLink(integratedPlatform, organizationCustomDomain),
    'product-tags-setup': getOrganizationMarketplaceSetupProductTagsBaseLink(integratedPlatform, organizationCustomDomain),
    'zones-setup': getOrganizationAdminZonesBaseLink(integratedPlatform, organizationCustomDomain),
    'tags-setup': getOrganizationAdminCustomTagsBaseLink(integratedPlatform, organizationCustomDomain),
    'billing-cycle': getOrganizationMarketplaceSetupBillingCycleBaseLink(integratedPlatform, organizationCustomDomain),
    'xero-setup': getOrganizationMarketplaceSetupXeroBaseLink(integratedPlatform, organizationCustomDomain),
    'stripe-connect-accounts-setup': getOrganizationMarketplaceSetupStripeConnectAccountsBaseLink(integratedPlatform, organizationCustomDomain),
    'bank-accounts-setup': getOrganizationMarketplaceSetupBankAccountsBaseLink(integratedPlatform, organizationCustomDomain),
    'sso-setup': getOrganizationAdminSsoSettingsBaseLink(integratedPlatform, organizationCustomDomain),
    subscriptions: getOrganizationAdminSubscriptionsBaseLink(integratedPlatform, organizationCustomDomain),
    'manage-organization': getOrganizationAdminManageOrganizationBaseLink(integratedPlatform, organizationCustomDomain),
  };
  const integrationsBaseLink = getOrganizationIntegrationsBaseLink(integratedPlatform, organizationCustomDomain);
  const adminCards = [
    {
      title: 'Profile',
      description: 'Organisation identity, physical address, and tax details.',
      sections: ['setup'] satisfies OrganizationAdminSection[],
    },
    {
      title: 'Billing & Payouts',
      description: 'Billing cadence, bank accounts, and payment methods.',
      sections: ['billing-cycle', 'bank-accounts-setup'] satisfies OrganizationAdminSection[],
    },
    {
      title: 'Operations',
      description: 'Subscriptions and organisation lifecycle controls.',
      sections: ['manage-organization'] satisfies OrganizationAdminSection[],
    },
  ];

  const renderOverview = () => (
    <Box
      sx={{
        display: 'grid',
        gridTemplateColumns: { xs: '1fr', md: 'repeat(2, minmax(0, 1fr))' },
        gap: 2,
      }}
    >
      {adminCards.map((card) => {
        const primarySection = card.sections[0];

        return (
          <Card
            key={card.title}
            variant="outlined"
            sx={{
              borderRadius: 3,
              borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
              boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 12px 32px rgba(15, 23, 42, 0.06)' : theme.shadows[1]),
              overflow: 'hidden',
            }}
          >
            <CardContent>
              <StackColumn spacing={1.5}>
                <StackColumn spacing={0.5}>
                  <LeadIconTypography label={card.title} />
                  <BodyIconTypography label={card.description} />
                </StackColumn>
                <Divider />
                <StackRow sx={{ flexWrap: 'wrap', gap: 1 }}>
                  {card.sections.map((item) => (
                    <Button
                      key={item}
                      component={NextLink}
                      href={sectionLinks[item]}
                      variant="outlined"
                      size="small"
                      sx={{
                        borderRadius: 999,
                        textTransform: 'none',
                        fontWeight: 700,
                        ...(item === primarySection
                          ? {
                              bgcolor: (theme) => (theme.palette.mode === 'light' ? 'grey.900' : 'grey.100'),
                              borderColor: (theme) => (theme.palette.mode === 'light' ? 'grey.900' : 'grey.100'),
                              color: (theme) => (theme.palette.mode === 'light' ? 'common.white' : 'grey.900'),
                              '&:hover': {
                                bgcolor: (theme) => (theme.palette.mode === 'light' ? 'grey.800' : 'common.white'),
                                borderColor: (theme) => (theme.palette.mode === 'light' ? 'grey.800' : 'common.white'),
                              },
                            }
                          : {
                              bgcolor: (theme) => (theme.palette.mode === 'light' ? 'common.white' : 'transparent'),
                              borderColor: (theme) => (theme.palette.mode === 'light' ? 'grey.400' : 'grey.500'),
                              color: 'text.primary',
                              '&:hover': {
                                bgcolor: (theme) => (theme.palette.mode === 'light' ? 'grey.50' : 'rgba(255, 255, 255, 0.08)'),
                                borderColor: 'text.primary',
                              },
                            }),
                      }}
                    >
                      {sectionLabels[item]}
                    </Button>
                  ))}
                </StackRow>
              </StackColumn>
            </CardContent>
          </Card>
        );
      })}
    </Box>
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
      {(['tags-setup', 'zones-setup', 'product-tags-setup'] as OrganizationAdminSection[]).map((item) => (
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

  const renderIntegrationTabs = () => (
    <StackRow sx={{ overflowX: 'auto', gap: 1, p: 1, border: 1, borderColor: 'divider', borderRadius: 4, bgcolor: 'background.paper' }}>
      {(['xero-setup', 'stripe-connect-accounts-setup'] as OrganizationAdminSection[]).map((item) => (
        <Button
          key={item}
          component={NextLink}
          href={`${integrationsBaseLink.split('?')[0]}?tab=${item}`}
          variant={activeSection === item ? 'contained' : 'outlined'}
          color="primary"
          sx={{ flexShrink: 0, borderRadius: 999, px: 2, textTransform: 'none', whiteSpace: 'nowrap' }}
        >
          {sectionLabels[item]}
        </Button>
      ))}
    </StackRow>
  );

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
        >
          <StackColumn spacing={0.5}>
            {!integrationsMode && activeSection && !tagsGroupsMode ? (
              <Button
                component={NextLink}
                href={adminBaseLink}
                variant="outlined"
                sx={{
                  alignSelf: 'flex-start',
                  borderRadius: 999,
                  textTransform: 'none',
                  fontWeight: 700,
                  borderColor: (theme) => (theme.palette.mode === 'light' ? 'grey.500' : 'grey.400'),
                  color: 'text.primary',
                  '&:hover': {
                    bgcolor: (theme) => (theme.palette.mode === 'light' ? 'grey.50' : 'rgba(255, 255, 255, 0.08)'),
                    borderColor: 'text.primary',
                  },
                }}
              >
                Back to admin
              </Button>
            ) : !tagsGroupsMode && !integrationsMode ? (
              <>
                <LeadIconTypography label="Marketplace controls" />
                <BodyIconTypography label={organization?.marketplaceListingMetadata?.title || organization?.name || 'Listing, billing, payouts, tags, and subscriptions'} />
              </>
            ) : null}
          </StackColumn>
        </PageHeaderPanel>

        {tagsGroupsMode && renderTagsGroupsTabs()}
        {!activeSection && !tagsGroupsMode && renderOverview()}
        {['xero-setup', 'stripe-connect-accounts-setup'].includes(activeSection ?? '') && renderIntegrationTabs()}
        {activeSection === 'setup' && (
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'minmax(0, 1fr) 300px' }, gap: { xs: 2, md: 3 }, alignItems: 'start' }}>
            <StackColumn spacing={1.5}>
              <OrganizationAdminSetupSection organizationCustomDomain={organizationCustomDomain} />
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
                <OrganizationAdminPhysicalAddressSection organizationCustomDomain={organizationCustomDomain} />
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
        {activeSection === 'product-tags-setup' && <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />}
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

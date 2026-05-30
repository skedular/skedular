import {
  getOrganizationAdminBaseLink,
  getOrganizationAdminBillingAndPaymentBaseLink,
  getOrganizationAdminCustomTagsBaseLink,
  getOrganizationAdminManageOrganizationBaseLink,
  getOrganizationAdminPhysicalAddressBaseLink,
  getOrganizationAdminSetupBaseLink,
  getOrganizationAdminSsoSettingsBaseLink,
  getOrganizationAdminSubscriptionsBaseLink,
  getOrganizationAdminTaxDetailsBaseLink,
  getOrganizationAdminZonesBaseLink,
  getOrganizationMarketplaceSetupBankAccountsBaseLink,
  getOrganizationMarketplaceSetupBillingCycleBaseLink,
  getOrganizationMarketplaceSetupMarketplaceListingBaseLink,
  getOrganizationMarketplaceSetupProductTagsBaseLink,
  getOrganizationMarketplaceSetupStripeConnectAccountsBaseLink,
  getOrganizationMarketplaceSetupXeroBaseLink,
} from '@/components/links';
import OrganizationMarketplaceSetupLoader from '@/components/organization/organizationMarketplaceSetup/organization-marketplace-setup-loader';
import type { organizationAdmin_query$key } from '@/queries/__generated__/organizationAdmin_query.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Divider from '@mui/material/Divider';
import { useIntegratedPlatform } from '@skedular/shared';
import { BodyIconTypography, defaultPadding, LeadIconTypography, PageHeaderPanel, StackColumn, StackRow } from '@skedular/ui';
import NextLink from 'next/link';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';
import { useSearchParams } from 'next/navigation';
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
};

type OrganizationAdminSection =
  | 'setup'
  | 'marketplace-listing'
  | 'billing-cycle'
  | 'xero-setup'
  | 'stripe-connect-accounts-setup'
  | 'bank-accounts-setup'
  | 'product-tags-setup'
  | 'physical-address-setup'
  | 'billing-payment-setup'
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
  'billing-payment-setup',
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
  'product-tags-setup': 'Product tags',
  'physical-address-setup': 'Address',
  'billing-payment-setup': 'Payment methods',
  'sso-setup': 'SSO',
  'tax-details-setup': 'Tax',
  'zones-setup': 'Zones',
  'tags-setup': 'Tags',
  subscriptions: 'Subscriptions',
  'manage-organization': 'Manage organisation',
};

const OrganizationAdmin = ({ rootDataRelay, organizationCustomDomain }: Props) => {
  const rootData = useFragment<organizationAdmin_query$key>(
    graphql`
      fragment organizationAdmin_query on Query {
        organization(customDomain: $organizationCustomDomain) {
          name
          marketplaceListingMetadata {
            title
          }
        }
      }
    `,
    rootDataRelay,
  );
  const searchParams = useSearchParams();
  const { integratedPlatform } = useIntegratedPlatform();
  const section = searchParams.get('section');
  const activeSection = useMemo(() => getActiveSection(section), [section]);

  const organization = rootData.organization;
  const adminBaseLink = getOrganizationAdminBaseLink(integratedPlatform, organizationCustomDomain);
  const sectionLinks: Record<OrganizationAdminSection, string> = {
    setup: getOrganizationAdminSetupBaseLink(integratedPlatform, organizationCustomDomain),
    'physical-address-setup': getOrganizationAdminPhysicalAddressBaseLink(integratedPlatform, organizationCustomDomain),
    'tax-details-setup': getOrganizationAdminTaxDetailsBaseLink(integratedPlatform, organizationCustomDomain),
    'marketplace-listing': getOrganizationMarketplaceSetupMarketplaceListingBaseLink(integratedPlatform, organizationCustomDomain),
    'product-tags-setup': getOrganizationMarketplaceSetupProductTagsBaseLink(integratedPlatform, organizationCustomDomain),
    'zones-setup': getOrganizationAdminZonesBaseLink(integratedPlatform, organizationCustomDomain),
    'tags-setup': getOrganizationAdminCustomTagsBaseLink(integratedPlatform, organizationCustomDomain),
    'billing-cycle': getOrganizationMarketplaceSetupBillingCycleBaseLink(integratedPlatform, organizationCustomDomain),
    'xero-setup': getOrganizationMarketplaceSetupXeroBaseLink(integratedPlatform, organizationCustomDomain),
    'stripe-connect-accounts-setup': getOrganizationMarketplaceSetupStripeConnectAccountsBaseLink(integratedPlatform, organizationCustomDomain),
    'bank-accounts-setup': getOrganizationMarketplaceSetupBankAccountsBaseLink(integratedPlatform, organizationCustomDomain),
    'billing-payment-setup': getOrganizationAdminBillingAndPaymentBaseLink(integratedPlatform, organizationCustomDomain),
    'sso-setup': getOrganizationAdminSsoSettingsBaseLink(integratedPlatform, organizationCustomDomain),
    subscriptions: getOrganizationAdminSubscriptionsBaseLink(integratedPlatform, organizationCustomDomain),
    'manage-organization': getOrganizationAdminManageOrganizationBaseLink(integratedPlatform, organizationCustomDomain),
  };
  const adminCards = [
    {
      title: 'Profile',
      description: 'Organisation identity, physical address, and tax details.',
      sections: ['setup', 'physical-address-setup', 'tax-details-setup'] satisfies OrganizationAdminSection[],
    },
    {
      title: 'Marketplace',
      description: 'Listing content, customer-facing product tags, zones, and tags.',
      sections: ['marketplace-listing', 'product-tags-setup', 'zones-setup', 'tags-setup'] satisfies OrganizationAdminSection[],
    },
    {
      title: 'Billing & Payouts',
      description: 'Billing cadence, Xero, Stripe, bank accounts, and payment methods.',
      sections: ['billing-cycle', 'xero-setup', 'stripe-connect-accounts-setup', 'bank-accounts-setup', 'billing-payment-setup'] satisfies OrganizationAdminSection[],
    },
    {
      title: 'Access',
      description: 'Single sign-on and identity-provider controls.',
      sections: ['sso-setup'] satisfies OrganizationAdminSection[],
    },
    {
      title: 'Operations',
      description: 'Subscriptions and organisation lifecycle controls.',
      sections: ['subscriptions', 'manage-organization'] satisfies OrganizationAdminSection[],
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
          eyebrow="Organisation admin"
          title={organization?.name ?? 'Organisation settings'}
          description={activeSection ? `Editing ${sectionLabels[activeSection].toLocaleLowerCase()}.` : 'Choose the area you want to configure for this marketplace organisation.'}
        >
          <StackColumn spacing={0.5}>
            {activeSection ? (
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
            ) : (
              <>
                <LeadIconTypography label="Marketplace controls" />
                <BodyIconTypography label={organization?.marketplaceListingMetadata?.title || organization?.name || 'Listing, billing, payouts, tags, and subscriptions'} />
              </>
            )}
          </StackColumn>
        </PageHeaderPanel>

        {!activeSection && renderOverview()}
        {activeSection === 'setup' && <OrganizationAdminSetupSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'marketplace-listing' && <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />}
        {activeSection === 'billing-cycle' && <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />}
        {activeSection === 'xero-setup' && <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />}
        {activeSection === 'stripe-connect-accounts-setup' && <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />}
        {activeSection === 'bank-accounts-setup' && <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />}
        {activeSection === 'product-tags-setup' && <OrganizationMarketplaceSetupLoader organizationCustomDomain={organizationCustomDomain} embedded />}
        {activeSection === 'physical-address-setup' && <OrganizationAdminPhysicalAddressSection organizationCustomDomain={organizationCustomDomain} />}
        {activeSection === 'billing-payment-setup' && <OrganizationAdminBillingPaymentSection organizationCustomDomain={organizationCustomDomain} />}
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

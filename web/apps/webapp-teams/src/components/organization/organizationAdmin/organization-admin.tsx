import {
  getOrganizationAdminBaseLink,
  getOrganizationAdminBillingAndPaymentBaseLink,
  getOrganizationAdminCustomTagsBaseLink,
  getOrganizationAdminManageOrganizationBaseLink,
  getOrganizationAdminPhysicalAddressBaseLink,
  getOrganizationAdminSetupBaseLink,
  getOrganizationAdminSsoSettingsBaseLink,
  getOrganizationAdminSubscriptionsBaseLink,
  getOrganizationAdminZonesBaseLink,
} from '@/components/links';
import type { organizationAdmin_query$key } from '@/queries/__generated__/organizationAdmin_query.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Divider from '@mui/material/Divider';
import { useIntegratedPlatrform } from '@skedular/shared';
import { BodyIconTypography, defaultPadding, LeadIconTypography, PageHeaderPanel, StackColumn, StackRow } from '@skedular/ui';
import NextLink from 'next/link';
import { useSearchParams } from 'next/navigation';
import { memo, useMemo } from 'react';
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
};

type OrganizationAdminSection = 'setup' | 'physical-address-setup' | 'billing-payment-setup' | 'sso-setup' | 'zones-setup' | 'tags-setup' | 'subscriptions' | 'manage-organization';

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

const getActiveSection = (value: string | null): OrganizationAdminSection | null => {
  if (value && validSections.includes(value as OrganizationAdminSection)) {
    return value as OrganizationAdminSection;
  }

  return null;
};

const sectionLabels: Record<OrganizationAdminSection, string> = {
  setup: 'Organisation profile',
  'physical-address-setup': 'Address',
  'billing-payment-setup': 'Payment methods',
  'sso-setup': 'SSO',
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
          listingMetadata {
            title
          }
        }
      }
    `,
    rootDataRelay,
  );
  const searchParams = useSearchParams();
  const { integratedPlatrform } = useIntegratedPlatrform();
  const section = searchParams.get('section');
  const activeSection = useMemo(() => getActiveSection(section), [section]);

  const organization = rootData.organization;
  const adminBaseLink = getOrganizationAdminBaseLink(integratedPlatrform, organizationCustomDomain);
  const sectionLinks: Record<OrganizationAdminSection, string> = {
    setup: getOrganizationAdminSetupBaseLink(integratedPlatrform, organizationCustomDomain),
    'physical-address-setup': getOrganizationAdminPhysicalAddressBaseLink(integratedPlatrform, organizationCustomDomain),
    'billing-payment-setup': getOrganizationAdminBillingAndPaymentBaseLink(integratedPlatrform, organizationCustomDomain),
    'sso-setup': getOrganizationAdminSsoSettingsBaseLink(integratedPlatrform, organizationCustomDomain),
    'zones-setup': getOrganizationAdminZonesBaseLink(integratedPlatrform, organizationCustomDomain),
    'tags-setup': getOrganizationAdminCustomTagsBaseLink(integratedPlatrform, organizationCustomDomain),
    subscriptions: getOrganizationAdminSubscriptionsBaseLink(integratedPlatrform, organizationCustomDomain),
    'manage-organization': getOrganizationAdminManageOrganizationBaseLink(integratedPlatrform, organizationCustomDomain),
  };
  const adminCards = [
    {
      title: 'Profile',
      description: 'Organisation identity and physical address.',
      sections: ['setup', 'physical-address-setup'] satisfies OrganizationAdminSection[],
    },
    {
      title: 'Team Controls',
      description: 'Zones and tags used to organise access and preferences.',
      sections: ['zones-setup', 'tags-setup'] satisfies OrganizationAdminSection[],
    },
    {
      title: 'Billing',
      description: 'Payment methods and subscription controls.',
      sections: ['billing-payment-setup', 'subscriptions'] satisfies OrganizationAdminSection[],
    },
    {
      title: 'Access',
      description: 'Single sign-on and identity-provider controls.',
      sections: ['sso-setup'] satisfies OrganizationAdminSection[],
    },
    {
      title: 'Operations',
      description: 'Organisation lifecycle controls.',
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
          description={activeSection ? `Editing ${sectionLabels[activeSection].toLocaleLowerCase()}.` : 'Choose the area you want to configure for this organisation.'}
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
                <LeadIconTypography label="Settings & controls" />
                <BodyIconTypography label={organization?.listingMetadata?.title || 'Billing, address, identity, tags, and subscriptions'} />
              </>
            )}
          </StackColumn>
        </PageHeaderPanel>

        {!activeSection && renderOverview()}
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

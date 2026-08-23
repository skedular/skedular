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
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, PropsWithChildren, useMemo } from 'react';
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
          physicalAddress {
            formattedAddress
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
  const router = useRouter();
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
  const adminBaseLink = getOrganizationAdminBaseLink(integratedPlatform, organizationCustomDomain);
  const sectionLinks: Record<OrganizationAdminSection, string> = {
    setup: getOrganizationAdminSetupBaseLink(integratedPlatform, organizationCustomDomain),
    'physical-address-setup': getOrganizationAdminPhysicalAddressBaseLink(integratedPlatform, organizationCustomDomain),
    'billing-payment-setup': getOrganizationAdminBillingAndPaymentBaseLink(integratedPlatform, organizationCustomDomain),
    'sso-setup': getOrganizationAdminSsoSettingsBaseLink(integratedPlatform, organizationCustomDomain),
    'zones-setup': getOrganizationAdminZonesBaseLink(integratedPlatform, organizationCustomDomain),
    'tags-setup': getOrganizationAdminCustomTagsBaseLink(integratedPlatform, organizationCustomDomain),
    subscriptions: getOrganizationAdminSubscriptionsBaseLink(integratedPlatform, organizationCustomDomain),
    'manage-organization': getOrganizationAdminManageOrganizationBaseLink(integratedPlatform, organizationCustomDomain),
  };
  const adminCards = [
    {
      title: 'Profile',
      description: 'Organisation identity and physical address.',
      sections: ['setup'] satisfies OrganizationAdminSection[],
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
                <BodyIconTypography label="Billing, address, identity, tags, and subscriptions" />
              </>
            )}
          </StackColumn>
        </PageHeaderPanel>

        {!activeSection && renderOverview()}
        {activeSection === 'setup' && (
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'minmax(0, 1fr) 300px' }, gap: { xs: 2, md: 3 }, alignItems: 'start' }}>
            <StackColumn spacing={1.5}>
              <OrganizationAdminSetupSection organizationCustomDomain={organizationCustomDomain} />
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
                title="SSO"
                description="Configure enterprise sign-in and identity federation for organization members."
                summary="Enterprise sign-in settings"
                expanded={expandedProfileSection === 'sso'}
                onChange={() => setExpandedProfileSection(expandedProfileSection === 'sso' ? '' : 'sso')}
              >
                <OrganizationAdminSsoSection organizationCustomDomain={organizationCustomDomain} />
              </EditorSection>
            </StackColumn>
            {renderOrganizationSummary()}
          </Box>
        )}
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

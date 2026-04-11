import {
  getOrganizationAdminBillingAndPaymentBaseLink,
  getOrganizationAdminCustomTagsBaseLink,
  getOrganizationAdminManageOrganizationBaseLink,
  getOrganizationAdminPhysicalAddressBaseLink,
  getOrganizationAdminSetupBaseLink,
  getOrganizationAdminSsoSettingsBaseLink,
  getOrganizationAdminSubscriptionsBaseLink,
  getOrganizationAdminTaxDetailsBaseLink,
  getOrganizationAdminZonesBaseLink,
} from '@/components/links';
import { useIntegratedPlatrform } from '@/libs/providers';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import NextLink from 'next/link';
import { memo, useState, type MouseEvent } from 'react';

export type OrganizationAdminSection =
  | 'setup'
  | 'physical-address-setup'
  | 'billing-payment-setup'
  | 'sso-setup'
  | 'tax-details-setup'
  | 'zones-setup'
  | 'tags-setup'
  | 'subscriptions'
  | 'manage-organization';

type Props = {
  activeSection: OrganizationAdminSection;
  organizationCustomDomain: string;
  stickyTop?: number;
};

const sectionLabels: Record<OrganizationAdminSection, string> = {
  setup: 'Setup',
  'physical-address-setup': 'Address',
  'billing-payment-setup': 'Billing',
  'sso-setup': 'SSO',
  'tax-details-setup': 'Tax',
  'zones-setup': 'Zones',
  'tags-setup': 'Tags',
  subscriptions: 'Subscriptions',
  'manage-organization': 'Manage',
};

const OrganizationAdminSectionNav = ({ activeSection, organizationCustomDomain, stickyTop = 0 }: Props) => {
  const { integratedPlatrform } = useIntegratedPlatrform();
  const theme = useTheme();
  const isCompactNav = useMediaQuery(theme.breakpoints.down('md'), { noSsr: true });
  const [menuAnchor, setMenuAnchor] = useState<HTMLElement | null>(null);
  const sectionLinks: Record<OrganizationAdminSection, string> = {
    setup: getOrganizationAdminSetupBaseLink(integratedPlatrform, organizationCustomDomain),
    'physical-address-setup': getOrganizationAdminPhysicalAddressBaseLink(integratedPlatrform, organizationCustomDomain),
    'billing-payment-setup': getOrganizationAdminBillingAndPaymentBaseLink(integratedPlatrform, organizationCustomDomain),
    'sso-setup': getOrganizationAdminSsoSettingsBaseLink(integratedPlatrform, organizationCustomDomain),
    'tax-details-setup': getOrganizationAdminTaxDetailsBaseLink(integratedPlatrform, organizationCustomDomain),
    'zones-setup': getOrganizationAdminZonesBaseLink(integratedPlatrform, organizationCustomDomain),
    'tags-setup': getOrganizationAdminCustomTagsBaseLink(integratedPlatrform, organizationCustomDomain),
    subscriptions: getOrganizationAdminSubscriptionsBaseLink(integratedPlatrform, organizationCustomDomain),
    'manage-organization': getOrganizationAdminManageOrganizationBaseLink(integratedPlatrform, organizationCustomDomain),
  };

  const handleOpenMenu = (event: MouseEvent<HTMLElement>) => {
    setMenuAnchor(event.currentTarget);
  };

  const handleCloseMenu = () => {
    setMenuAnchor(null);
  };

  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: { xs: 'column', md: 'row' },
        px: { xs: 2, sm: 3 },
        py: 2,
        borderBottom: 1,
        borderColor: 'divider',
        bgcolor: 'background.paper',
        position: 'sticky',
        top: stickyTop,
        zIndex: 2,
      }}
    >
      {isCompactNav ? (
        <>
          <Button
            variant="contained"
            color="primary"
            onClick={handleOpenMenu}
            aria-haspopup="menu"
            aria-expanded={menuAnchor ? 'true' : undefined}
            aria-controls={menuAnchor ? 'organization-admin-sections-menu' : undefined}
            sx={{
              justifyContent: 'space-between',
              borderRadius: 999,
              px: 2,
              textTransform: 'none',
            }}
          >
            {`Section: ${sectionLabels[activeSection]}`}
          </Button>

          <Menu anchorEl={menuAnchor} open={Boolean(menuAnchor)} onClose={handleCloseMenu} id="organization-admin-sections-menu">
            {(Object.keys(sectionLabels) as OrganizationAdminSection[]).map((section) => (
              <MenuItem key={section} component={NextLink} href={sectionLinks[section]} selected={activeSection === section} onClick={handleCloseMenu}>
                {sectionLabels[section]}
              </MenuItem>
            ))}
          </Menu>
        </>
      ) : (
        <Box
          sx={{
            display: 'flex',
            gap: 1,
            overflowX: 'auto',
            flex: '1 1 0%',
            minWidth: 0,
            scrollbarWidth: 'none',
            '&::-webkit-scrollbar': {
              display: 'none',
            },
          }}
        >
          {(Object.keys(sectionLabels) as OrganizationAdminSection[]).map((section) => (
            <Button
              key={section}
              component={NextLink}
              href={sectionLinks[section]}
              variant={activeSection === section ? 'contained' : 'text'}
              color={activeSection === section ? 'primary' : 'inherit'}
              sx={{
                flexShrink: 0,
                borderRadius: 999,
                px: 2,
                textTransform: 'none',
                whiteSpace: 'nowrap',
              }}
            >
              {sectionLabels[section]}
            </Button>
          ))}
        </Box>
      )}
    </Box>
  );
};

export default memo(OrganizationAdminSectionNav);

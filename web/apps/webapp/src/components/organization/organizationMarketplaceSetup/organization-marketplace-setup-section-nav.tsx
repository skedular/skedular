import {
  getOrganizationMarketplaceSetupBankAccountsBaseLink,
  getOrganizationMarketplaceSetupBillingCycleBaseLink,
  getOrganizationMarketplaceSetupMarketplaceListingBaseLink,
  getOrganizationMarketplaceSetupProductTagsBaseLink,
  getOrganizationMarketplaceSetupStripeConnectAccountsBaseLink,
  getOrganizationMarketplaceSetupXeroBaseLink,
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

export type OrganizationMarketplaceSetupSection =
  | 'marketplace-listing'
  | 'billing-cycle'
  | 'xero-setup'
  | 'stripe-connect-accounts-setup'
  | 'bank-accounts-setup'
  | 'product-tags-setup';

type Props = {
  activeSection: OrganizationMarketplaceSetupSection;
  organizationCustomDomain: string;
  stickyTop?: number;
};

const sectionLabels: Record<OrganizationMarketplaceSetupSection, string> = {
  'marketplace-listing': 'Listing',
  'billing-cycle': 'Billing',
  'xero-setup': 'Xero',
  'stripe-connect-accounts-setup': 'Stripe',
  'bank-accounts-setup': 'Bank Accounts',
  'product-tags-setup': 'Product Tags',
};

const OrganizationMarketplaceSetupSectionNav = ({ activeSection, organizationCustomDomain, stickyTop = 0 }: Props) => {
  const { integratedPlatrform } = useIntegratedPlatrform();
  const theme = useTheme();
  const isCompactNav = useMediaQuery(theme.breakpoints.down('md'), { noSsr: true });
  const [menuAnchor, setMenuAnchor] = useState<HTMLElement | null>(null);
  const sectionLinks: Record<OrganizationMarketplaceSetupSection, string> = {
    'marketplace-listing': getOrganizationMarketplaceSetupMarketplaceListingBaseLink(integratedPlatrform, organizationCustomDomain),
    'billing-cycle': getOrganizationMarketplaceSetupBillingCycleBaseLink(integratedPlatrform, organizationCustomDomain),
    'xero-setup': getOrganizationMarketplaceSetupXeroBaseLink(integratedPlatrform, organizationCustomDomain),
    'stripe-connect-accounts-setup': getOrganizationMarketplaceSetupStripeConnectAccountsBaseLink(integratedPlatrform, organizationCustomDomain),
    'bank-accounts-setup': getOrganizationMarketplaceSetupBankAccountsBaseLink(integratedPlatrform, organizationCustomDomain),
    'product-tags-setup': getOrganizationMarketplaceSetupProductTagsBaseLink(integratedPlatrform, organizationCustomDomain),
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
        border: 1,
        borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
        borderRadius: 4,
        bgcolor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.88)' : theme.palette.background.paper),
        boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : theme.shadows[1]),
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
            aria-controls={menuAnchor ? 'organization-marketplace-setup-sections-menu' : undefined}
            sx={{
              justifyContent: 'space-between',
              borderRadius: 999,
              px: 2,
              textTransform: 'none',
            }}
          >
            {`Section: ${sectionLabels[activeSection]}`}
          </Button>

          <Menu anchorEl={menuAnchor} open={Boolean(menuAnchor)} onClose={handleCloseMenu} id="organization-marketplace-setup-sections-menu">
            {(Object.keys(sectionLabels) as OrganizationMarketplaceSetupSection[]).map((section) => (
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
          {(Object.keys(sectionLabels) as OrganizationMarketplaceSetupSection[]).map((section) => (
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

export default memo(OrganizationMarketplaceSetupSectionNav);

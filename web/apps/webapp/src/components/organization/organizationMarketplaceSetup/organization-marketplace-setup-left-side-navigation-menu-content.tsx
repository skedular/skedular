import { BodyIconTypography } from '@/components/commons';
import { BankAccountIcon, BillingIcon, EditIcon, ProductTagIcon, StripeConnectAccountIcon } from '@/components/icons';
import {
  getOrganizationMarketplaceSetupBankAccountsBaseLink,
  getOrganizationMarketplaceSetupBillingCycleBaseLink,
  getOrganizationMarketplaceSetupMarketplaceListingBaseLink,
  getOrganizationMarketplaceSetupProductTagsBaseLink,
  getOrganizationMarketplaceSetupStripeConnectAccountsBaseLink,
} from '@/components/links';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import {
  getSelectedListItemBorderRadius,
  sandstone,
  secondDrawerCollapsedDrawerWidth,
  secondDrawerCollapsedDrawerWidthPx,
  secondDrawerExpandedDrawerWidth,
  secondDrawerExpandedDrawerWidthPx,
} from '@/libs/theme';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import NextLink from 'next/link';
import { usePathname, useSearchParams } from 'next/navigation';
import { memo, useContext } from 'react';

type Props = {
  organizationUniqueAlphanumericName: string;
  collapsed?: boolean;
  hideIcons?: boolean;
};

const OrganizationMarketplaceSetupLeftSideNavigationMenuContent = ({ organizationUniqueAlphanumericName, collapsed, hideIcons }: Props) => {
  const { integratedPlatrform } = useIntegratedPlatrform();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const paletteMode = useContext(PaletteModeContext);
  const maxWidth = collapsed ? secondDrawerCollapsedDrawerWidth : secondDrawerExpandedDrawerWidth;
  const styles = {
    width: maxWidth,
    borderRadius: 4,
    marginLeft: 1,
    marginRight: 1,
    transition: 'border-radius 0.3s ease, width 0.3s ease',
    '&:hover': {
      width: maxWidth,
      borderRadius: 4,
      marginLeft: 1,
      marginRight: 1,
      transition: 'none',
    },
    '&.Mui-selected': {
      width: maxWidth,
      borderRadius: 4,
      marginLeft: 1,
      marginRight: 1,
      backgroundColor: sandstone,
      '&:hover': {
        width: maxWidth,
        borderRadius: 4,
        marginLeft: 1,
        marginRight: 1,
        backgroundColor: sandstone,
      },
    },
  };

  const fullPath = `${pathname}?${searchParams.toString()}`;
  const marketplaceListingLink = getOrganizationMarketplaceSetupMarketplaceListingBaseLink(integratedPlatrform, organizationUniqueAlphanumericName);
  const billingCycleLink = getOrganizationMarketplaceSetupBillingCycleBaseLink(integratedPlatrform, organizationUniqueAlphanumericName);
  const stripeConnectAccountsLink = getOrganizationMarketplaceSetupStripeConnectAccountsBaseLink(integratedPlatrform, organizationUniqueAlphanumericName);
  const bankAccountsLink = getOrganizationMarketplaceSetupBankAccountsBaseLink(integratedPlatrform, organizationUniqueAlphanumericName);
  const productTagsLink = getOrganizationMarketplaceSetupProductTagsBaseLink(integratedPlatrform, organizationUniqueAlphanumericName);

  return (
    <List
      sx={{
        backgroundColor: (theme) => theme.palette.background.paper,
        borderRight: 1,
        borderColor: (theme) => theme.palette.divider,
        paddingTop: { xs: 1, sm: 1, md: 3 },
        height: '100vh',
        position: 'fixed',
        width: collapsed ? secondDrawerCollapsedDrawerWidthPx : secondDrawerExpandedDrawerWidthPx,
      }}
    >
      <ListItem disablePadding>
        <Link component={NextLink} href={marketplaceListingLink}>
          <ListItemButton selected={fullPath === marketplaceListingLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === marketplaceListingLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <EditIcon color="inherit" />} invertDefaultColor={fullPath === marketplaceListingLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Marketplace Listing"
                startElement={!hideIcons && <EditIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === marketplaceListingLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={billingCycleLink}>
          <ListItemButton selected={fullPath === billingCycleLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === billingCycleLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <BillingIcon color="inherit" />} invertDefaultColor={fullPath === billingCycleLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Billing Cycle"
                startElement={!hideIcons && <BillingIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === billingCycleLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={stripeConnectAccountsLink}>
          <ListItemButton
            selected={fullPath === stripeConnectAccountsLink}
            sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === stripeConnectAccountsLink) }}
          >
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <StripeConnectAccountIcon color="inherit" />}
                invertDefaultColor={fullPath === stripeConnectAccountsLink && paletteMode === 'dark'}
              />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Stripe Connect Account"
                startElement={!hideIcons && <StripeConnectAccountIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === stripeConnectAccountsLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={bankAccountsLink}>
          <ListItemButton selected={fullPath === bankAccountsLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === bankAccountsLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <BankAccountIcon color="inherit" />} invertDefaultColor={fullPath === bankAccountsLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Bank Account"
                startElement={!hideIcons && <BankAccountIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === bankAccountsLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={productTagsLink}>
          <ListItemButton selected={fullPath === productTagsLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === productTagsLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <ProductTagIcon color="inherit" />} invertDefaultColor={fullPath === productTagsLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Product Tag"
                startElement={!hideIcons && <ProductTagIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === productTagsLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>
    </List>
  );
};

export default memo(OrganizationMarketplaceSetupLeftSideNavigationMenuContent);

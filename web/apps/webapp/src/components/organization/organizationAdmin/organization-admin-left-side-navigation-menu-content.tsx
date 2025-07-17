import { BodyIconTypography } from '@/components/commons';
import { AddressIcon, BillingAndPaymentIcon, CustomTagIcon, EditIcon, SsoSettingsIcon, SubscriptionsIcon, TaxDetailsIcon } from '@/components/icons';
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
  organizationId: string;
  collapsed?: boolean;
  hideIcons?: boolean;
};

const OrganizationAdminLeftSideNavigationMenuContent = ({ organizationId, collapsed, hideIcons }: Props) => {
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
  const setupLink = getOrganizationAdminSetupBaseLink(integratedPlatrform, organizationId);
  const physcialAddressLink = getOrganizationAdminPhysicalAddressBaseLink(integratedPlatrform, organizationId);
  const billingAndPaymentLink = getOrganizationAdminBillingAndPaymentBaseLink(integratedPlatrform, organizationId);
  const ssoSettingsLink = getOrganizationAdminSsoSettingsBaseLink(integratedPlatrform, organizationId);
  const taxDetailsLink = getOrganizationAdminTaxDetailsBaseLink(integratedPlatrform, organizationId);
  const zonesLink = getOrganizationAdminZonesBaseLink(integratedPlatrform, organizationId);
  const customTagsLink = getOrganizationAdminCustomTagsBaseLink(integratedPlatrform, organizationId);
  const subscriptionsLink = getOrganizationAdminSubscriptionsBaseLink(integratedPlatrform, organizationId);
  const manageOrganizationLink = getOrganizationAdminManageOrganizationBaseLink(integratedPlatrform, organizationId);

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
        <Link component={NextLink} href={setupLink}>
          <ListItemButton selected={fullPath === setupLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === setupLink) }}>
            {collapsed && <BodyIconTypography startElement={!hideIcons && <EditIcon color="inherit" />} invertDefaultColor={fullPath === setupLink && paletteMode === 'dark'} />}
            {!collapsed && (
              <BodyIconTypography
                label="Organization Setup"
                startElement={!hideIcons && <EditIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === setupLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={physcialAddressLink}>
          <ListItemButton selected={fullPath === physcialAddressLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === physcialAddressLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <AddressIcon color="inherit" />} invertDefaultColor={fullPath === physcialAddressLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Physical Address"
                startElement={!hideIcons && <AddressIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === physcialAddressLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={billingAndPaymentLink}>
          <ListItemButton selected={fullPath === billingAndPaymentLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === billingAndPaymentLink) }}>
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <BillingAndPaymentIcon color="inherit" />}
                invertDefaultColor={fullPath === billingAndPaymentLink && paletteMode === 'dark'}
              />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Billing & Payment"
                startElement={!hideIcons && <BillingAndPaymentIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === billingAndPaymentLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={ssoSettingsLink}>
          <ListItemButton selected={fullPath === ssoSettingsLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === ssoSettingsLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <SsoSettingsIcon color="inherit" />} invertDefaultColor={fullPath === ssoSettingsLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="SSO Setup"
                startElement={!hideIcons && <SsoSettingsIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === ssoSettingsLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={taxDetailsLink}>
          <ListItemButton selected={fullPath === taxDetailsLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === taxDetailsLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <TaxDetailsIcon color="inherit" />} invertDefaultColor={fullPath === taxDetailsLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Tax Details Setup"
                startElement={!hideIcons && <TaxDetailsIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === taxDetailsLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={zonesLink}>
          <ListItemButton selected={fullPath === zonesLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === zonesLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <CustomTagIcon color="inherit" />} invertDefaultColor={fullPath === zonesLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Zone Setup"
                startElement={!hideIcons && <CustomTagIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === zonesLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={customTagsLink}>
          <ListItemButton selected={fullPath === customTagsLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === customTagsLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <CustomTagIcon color="inherit" />} invertDefaultColor={fullPath === customTagsLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Tag Setup"
                startElement={!hideIcons && <CustomTagIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === customTagsLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={subscriptionsLink}>
          <ListItemButton selected={fullPath === subscriptionsLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === subscriptionsLink) }}>
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <SubscriptionsIcon color="inherit" />}
                invertDefaultColor={fullPath === subscriptionsLink && paletteMode === 'dark'}
              />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Subscriptions"
                startElement={!hideIcons && <SubscriptionsIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === subscriptionsLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={manageOrganizationLink}>
          <ListItemButton selected={fullPath === manageOrganizationLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === manageOrganizationLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <EditIcon color="inherit" />} invertDefaultColor={fullPath === manageOrganizationLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Manage Organization"
                startElement={!hideIcons && <EditIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === manageOrganizationLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>
    </List>
  );
};

export default memo(OrganizationAdminLeftSideNavigationMenuContent);

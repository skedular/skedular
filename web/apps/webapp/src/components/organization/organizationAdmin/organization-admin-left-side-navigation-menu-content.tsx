import { BodyIconTypography } from '@/components/commons';
import { BillingAndPaymentIcon, CustomTagIcon, EditIcon, LocationTagIcon, ProductTagIcon, SSOIcon, SubscriptionsIcon } from '@/components/icons';
import {
  getOrganizationAdminBillingAndPaymentBaseLink,
  getOrganizationAdminCustomTagsBaseLink,
  getOrganizationAdminLocationTagsBaseLink,
  getOrganizationAdminManageOrganizationBaseLink,
  getOrganizationAdminProductTagsBaseLink,
  getOrganizationAdminSetupBaseLink,
  getOrganizationAdminSSOBaseLink,
  getOrganizationAdminSubscriptionsBaseLink,
  getOrganizationAdminZonesBaseLink,
} from '@/components/links';
import { PaletteModeContext } from '@/libs/providers';
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
  const setupLink = getOrganizationAdminSetupBaseLink(organizationId);
  const billingAndPaymentLink = getOrganizationAdminBillingAndPaymentBaseLink(organizationId);
  const ssoLink = getOrganizationAdminSSOBaseLink(organizationId);
  const zonesLink = getOrganizationAdminZonesBaseLink(organizationId);
  const customTagsLink = getOrganizationAdminCustomTagsBaseLink(organizationId);
  const productTagsLink = getOrganizationAdminProductTagsBaseLink(organizationId);
  const locationTagsLink = getOrganizationAdminLocationTagsBaseLink(organizationId);
  const subscriptionsLink = getOrganizationAdminSubscriptionsBaseLink(organizationId);
  const manageOrganizationLink = getOrganizationAdminManageOrganizationBaseLink(organizationId);

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
        <Link component={NextLink} href={ssoLink}>
          <ListItemButton selected={fullPath === ssoLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === ssoLink) }}>
            {collapsed && <BodyIconTypography startElement={!hideIcons && <SSOIcon color="inherit" />} invertDefaultColor={fullPath === ssoLink && paletteMode === 'dark'} />}
            {!collapsed && (
              <BodyIconTypography
                label="SSO Setup"
                startElement={!hideIcons && <SSOIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === ssoLink && paletteMode === 'dark'}
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
        <Link component={NextLink} href={productTagsLink}>
          <ListItemButton selected={fullPath === productTagsLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === productTagsLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <ProductTagIcon color="inherit" />} invertDefaultColor={fullPath === productTagsLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Product Tag Setup"
                startElement={!hideIcons && <ProductTagIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === productTagsLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={locationTagsLink}>
          <ListItemButton selected={fullPath === locationTagsLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === locationTagsLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <LocationTagIcon color="inherit" />} invertDefaultColor={fullPath === locationTagsLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Location Tag Setup"
                startElement={!hideIcons && <LocationTagIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === locationTagsLink && paletteMode === 'dark'}
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

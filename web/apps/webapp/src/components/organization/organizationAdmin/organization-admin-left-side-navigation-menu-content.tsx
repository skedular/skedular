import {
  getModernOrganizationAdminBillingAndPaymentBaseLink,
  getModernOrganizationAdminCustomTagsBaseLink,
  getModernOrganizationAdminSetupBaseLink,
  getModernOrganizationAdminSSOBaseLink,
  getModernOrganizationAdminSubscriptionsBaseLink,
  getModernOrganizationAdminZonesBaseLink,
} from '@/components/organization';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { BillingAndPaymentIcon, CustomTagIcon, EditIcon, SSOIcon, SubscriptionsIcon, ZoneIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { getSelectedListItemBorderRadius, sandstone } from '@repo/shared/libs/theme';
import NextLink from 'next/link';
import { usePathname, useSearchParams } from 'next/navigation';
import { memo, useContext } from 'react';
import { collapsedDrawerWidth, collapsedDrawerWidthPx, expandedDrawerWidth, expandedDrawerWidthPx } from './commons';

type Props = {
  organizationId: string;
  collapsed?: boolean;
  hideIcons?: boolean;
};

const OrganizationAdminLeftSideNavigationMenuContent = ({ organizationId, collapsed, hideIcons }: Props) => {
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const paletteMode = useContext(PaletteModeContext);
  const maxWidth = collapsed ? collapsedDrawerWidth : expandedDrawerWidth;
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
  const setupLink = getModernOrganizationAdminSetupBaseLink(organizationId);
  const billingAndPaymentLink = getModernOrganizationAdminBillingAndPaymentBaseLink(organizationId);
  const ssoLink = getModernOrganizationAdminSSOBaseLink(organizationId);
  const zonesLink = getModernOrganizationAdminZonesBaseLink(organizationId);
  const customTagsLink = getModernOrganizationAdminCustomTagsBaseLink(organizationId);
  const subscriptionsLink = getModernOrganizationAdminSubscriptionsBaseLink(organizationId);

  return (
    <List
      sx={{
        backgroundColor: (theme) => theme.palette.background.paper,
        borderRight: 1,
        borderColor: (theme) => theme.palette.divider,
        paddingTop: { xs: 1, sm: 1, md: 3 },
        height: '100vh',
        position: 'fixed',
        width: collapsed ? collapsedDrawerWidthPx : expandedDrawerWidthPx,
      }}
    >
      <ListItem disablePadding>
        <Link component={NextLink} href={setupLink}>
          <ListItemButton selected={fullPath === setupLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === setupLink) }}>
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <EditIcon color="inherit" />}
                invertDefaultColor={fullPath === setupLink && paletteMode === 'dark'}
              />
            )}
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
          <ListItemButton
            selected={fullPath === billingAndPaymentLink}
            sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === billingAndPaymentLink) }}
          >
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
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <SSOIcon color="inherit" />}
                invertDefaultColor={fullPath === ssoLink && paletteMode === 'dark'}
              />
            )}
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
              <BodyIconTypography
                startElement={!hideIcons && <ZoneIcon color="inherit" />}
                invertDefaultColor={fullPath === zonesLink && paletteMode === 'dark'}
              />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Zone Setup"
                startElement={!hideIcons && <ZoneIcon excludeTooltip color="inherit" />}
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
          <ListItemButton
            selected={fullPath === customTagsLink}
            sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === customTagsLink) }}
          >
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <CustomTagIcon color="inherit" />}
                invertDefaultColor={fullPath === customTagsLink && paletteMode === 'dark'}
              />
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
          <ListItemButton
            selected={fullPath === subscriptionsLink}
            sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === subscriptionsLink) }}
          >
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
    </List>
  );
};

export default memo(OrganizationAdminLeftSideNavigationMenuContent);

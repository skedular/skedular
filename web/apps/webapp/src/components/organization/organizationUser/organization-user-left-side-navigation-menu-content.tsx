import { BodyIconTypography } from '@/components/commons';
import { BillingAndPaymentIcon, EditIcon, ProfileIcon } from '@/components/icons';
import {
  getOrganizationUserBillingAndPaymentBaseLink,
  getOrganizationUserManageBaseLink,
  getOrganizationUserManageTeamsBaseLink,
  getOrganizationUserProfileBaseLink,
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
import type { organizationUserLeftSideNavigationMenuContent_query$key } from '@/queries/__generated__/organizationUserLeftSideNavigationMenuContent_query.graphql';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import NextLink from 'next/link';
import { usePathname, useSearchParams } from 'next/navigation';
import { memo, useContext } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizationUserLeftSideNavigationMenuContent_query$key;
  organizationUniqueAlphanumericName: string;
  customerId: string;
  collapsed?: boolean;
  hideIcons?: boolean;
};

const OrganizationUserLeftSideNavigationMenuContent = ({ rootDataRelay, organizationUniqueAlphanumericName, customerId, collapsed, hideIcons }: Props) => {
  const rootData = useFragment<organizationUserLeftSideNavigationMenuContent_query$key>(
    graphql`
      fragment organizationUserLeftSideNavigationMenuContent_query on Query {
        me {
          id
        }
      }
    `,
    rootDataRelay,
  );

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
  const porofileLink = getOrganizationUserProfileBaseLink(integratedPlatrform, organizationUniqueAlphanumericName, customerId);
  const manageTeamsLink = getOrganizationUserManageTeamsBaseLink(integratedPlatrform, organizationUniqueAlphanumericName, customerId);
  const billingAndPaymentLink = getOrganizationUserBillingAndPaymentBaseLink(integratedPlatrform, organizationUniqueAlphanumericName, customerId);
  const manageUserLink = getOrganizationUserManageBaseLink(integratedPlatrform, organizationUniqueAlphanumericName, customerId);

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
        <Link component={NextLink} href={porofileLink}>
          <ListItemButton selected={fullPath === porofileLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === porofileLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <ProfileIcon color="inherit" />} invertDefaultColor={fullPath === porofileLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Profile"
                startElement={!hideIcons && <ProfileIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === porofileLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={manageTeamsLink}>
          <ListItemButton selected={fullPath === manageTeamsLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === manageTeamsLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <EditIcon color="inherit" />} invertDefaultColor={fullPath === manageTeamsLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Manage Teams"
                startElement={!hideIcons && <EditIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === manageTeamsLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      {customerId === rootData.me?.id && (
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
      )}

      <ListItem disablePadding>
        <Link component={NextLink} href={manageUserLink}>
          <ListItemButton selected={fullPath === manageUserLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === manageUserLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <EditIcon color="inherit" />} invertDefaultColor={fullPath === manageUserLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Manage User"
                startElement={!hideIcons && <EditIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === manageUserLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>
    </List>
  );
};

export default memo(OrganizationUserLeftSideNavigationMenuContent);

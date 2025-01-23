import { getOrganizationUserManageBaseLink, getOrganizationUserManageTeamsBaseLink, getOrganizationUserProfileBaseLink } from '@/components/links';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { EditIcon, ProfileIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { getSelectedListItemBorderRadius, sandstone } from '@repo/shared/libs/theme';
import NextLink from 'next/link';
import { usePathname, useSearchParams } from 'next/navigation';
import { memo, useContext } from 'react';
import { collapsedDrawerWidth, collapsedDrawerWidthPx, expandedDrawerWidth, expandedDrawerWidthPx } from './commons';

type Props = {
  organizationId: string;
  customerId: string;
  collapsed?: boolean;
  hideIcons?: boolean;
};

const OrganizationUserLeftSideNavigationMenuContent = ({ organizationId, customerId, collapsed, hideIcons }: Props) => {
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
  const porofileLink = getOrganizationUserProfileBaseLink(organizationId, customerId);
  const manageTeamsLink = getOrganizationUserManageTeamsBaseLink(organizationId, customerId);
  const manageUserLink = getOrganizationUserManageBaseLink(organizationId, customerId);

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
        <Link component={NextLink} href={porofileLink}>
          <ListItemButton
            selected={fullPath === porofileLink}
            sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === porofileLink) }}
          >
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <ProfileIcon color="inherit" />}
                invertDefaultColor={fullPath === porofileLink && paletteMode === 'dark'}
              />
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
          <ListItemButton
            selected={fullPath === manageTeamsLink}
            sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === manageTeamsLink) }}
          >
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <EditIcon color="inherit" />}
                invertDefaultColor={fullPath === manageTeamsLink && paletteMode === 'dark'}
              />
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

      <ListItem disablePadding>
        <Link component={NextLink} href={manageUserLink}>
          <ListItemButton
            selected={fullPath === manageUserLink}
            sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === manageUserLink) }}
          >
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <EditIcon color="inherit" />}
                invertDefaultColor={fullPath === manageUserLink && paletteMode === 'dark'}
              />
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

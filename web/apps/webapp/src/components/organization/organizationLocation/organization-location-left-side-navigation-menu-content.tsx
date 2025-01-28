import {
  getOrganizationLocationManageDesksBaseLink,
  getOrganizationLocationManageLocationBaseLink,
  getOrganizationLocationManageRoomsBaseLink,
  getOrganizationLocationSetupBaseLink,
} from '@/components/links';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { DeskIcon, EditIcon, RoomIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import {
  getSelectedListItemBorderRadius,
  sandstone,
  secondDrawerCollapsedDrawerWidth,
  secondDrawerCollapsedDrawerWidthPx,
  secondDrawerExpandedDrawerWidth,
  secondDrawerExpandedDrawerWidthPx,
} from '@repo/shared/libs/theme';
import NextLink from 'next/link';
import { usePathname, useSearchParams } from 'next/navigation';
import { memo, useContext } from 'react';

type Props = {
  organizationId: string;
  locationId: string;
  collapsed?: boolean;
  hideIcons?: boolean;
};

const OrganizationLocationLeftSideNavigationMenuContent = ({ organizationId, locationId, collapsed, hideIcons }: Props) => {
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
  const setupLink = getOrganizationLocationSetupBaseLink(organizationId, locationId);
  const manageDesksLink = getOrganizationLocationManageDesksBaseLink(organizationId, locationId);
  const manageRoomsLink = getOrganizationLocationManageRoomsBaseLink(organizationId, locationId);
  const manageLocationLink = getOrganizationLocationManageLocationBaseLink(organizationId, locationId);

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
                label="Location Setup"
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
        <Link component={NextLink} href={manageDesksLink}>
          <ListItemButton selected={fullPath === manageDesksLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === manageDesksLink) }}>
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <DeskIcon color="inherit" excludeTooltip />}
                invertDefaultColor={fullPath === manageDesksLink && paletteMode === 'dark'}
              />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Manage Desks"
                startElement={!hideIcons && <DeskIcon color="inherit" excludeTooltip />}
                spacing={3}
                invertDefaultColor={fullPath === manageDesksLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={manageRoomsLink}>
          <ListItemButton selected={fullPath === manageRoomsLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === manageRoomsLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <RoomIcon color="inherit" />} invertDefaultColor={fullPath === manageRoomsLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Manage Rooms"
                startElement={!hideIcons && <RoomIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === manageRoomsLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={manageLocationLink}>
          <ListItemButton selected={fullPath === manageLocationLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === manageLocationLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <EditIcon color="inherit" />} invertDefaultColor={fullPath === manageLocationLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Manage Location"
                startElement={!hideIcons && <EditIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === manageLocationLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>
    </List>
  );
};

export default memo(OrganizationLocationLeftSideNavigationMenuContent);

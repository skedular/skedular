import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { EditIcon, MembersIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { getSelectedListItemBorderRadius, sandstone } from '@repo/shared/libs/theme';
import {
  getModernOrganizationTeamLocationBaseLink,
  getModernOrganizationTeamMembersBaseLink,
  getModernOrganizationTeamSetupBaseLink,
} from 'components/organization';
import { memo, useContext } from 'react';
import { useLocation, useSearchParams } from 'react-router-dom';
import { collapsedDrawerWidth, collapsedDrawerWidthPx, expandedDrawerWidth, expandedDrawerWidthPx } from './commons';

type Props = {
  organizationId: string;
  teamId: string;
  collapsed?: boolean;
  hideIcons?: boolean;
};

const OrganizationTeamLeftSideNavigationMenuContent = ({ organizationId, teamId, collapsed, hideIcons }: Props) => {
  const location = useLocation();
  const pathname = location.pathname;
  const [searchParams] = useSearchParams();
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
  const setupLink = getModernOrganizationTeamSetupBaseLink(organizationId, teamId);
  const locationLink = getModernOrganizationTeamLocationBaseLink(organizationId, teamId);
  const memberesLink = getModernOrganizationTeamMembersBaseLink(organizationId, teamId);

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
        <Link href={setupLink}>
          <ListItemButton selected={fullPath === setupLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === setupLink) }}>
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <EditIcon color="inherit" />}
                invertDefaultColor={fullPath === setupLink && paletteMode === 'dark'}
              />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Team Setup"
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
        <Link href={locationLink}>
          <ListItemButton
            selected={fullPath === locationLink}
            sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === locationLink) }}
          >
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <MembersIcon color="inherit" />}
                invertDefaultColor={fullPath === locationLink && paletteMode === 'dark'}
              />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Location Settings"
                startElement={!hideIcons && <MembersIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === locationLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link href={memberesLink}>
          <ListItemButton
            selected={fullPath === memberesLink}
            sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === memberesLink) }}
          >
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <MembersIcon color="inherit" />}
                invertDefaultColor={fullPath === memberesLink && paletteMode === 'dark'}
              />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Team Members"
                startElement={!hideIcons && <MembersIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === memberesLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>
    </List>
  );
};

export default memo(OrganizationTeamLeftSideNavigationMenuContent);

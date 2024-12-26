import { IconButton } from '@mui/material';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import { BodyIconTypography } from '@repo/shared/components/commons';
import {
  CollpaseDrawerIcon,
  HomeIcon,
  LocationIcon,
  NotificationsIcon,
  OrganizationIcon,
  SettingsIcon,
  TeamIcon,
} from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { getSelectedListItemBorderRadius, sandstone, selectedListItemPaddings } from '@repo/shared/libs/theme';
import Image from 'next/image';
import NextLink from 'next/link';
import { usePathname } from 'next/navigation';
import { memo, useContext } from 'react';
import { collapsedDrawerWidth, expandedDrawerWidth } from './commons';

type Props = {
  collapsed?: boolean;
  toggleCollapse?: (collapsed: boolean) => void;
  enableCollapseButton?: boolean;
  hideIcons?: boolean;
};

const OldLeftSideNavigationMenu = ({ collapsed, enableCollapseButton, toggleCollapse, hideIcons }: Props) => {
  const pathName = usePathname();
  const paletteMode = useContext(PaletteModeContext);
  const maxWidth = collapsed ? collapsedDrawerWidth : expandedDrawerWidth;
  const logoUrl =
    paletteMode === 'dark'
      ? collapsed
        ? '/images/skedular-icon-inverse.svg'
        : '/images/skedular-logo-inverse.svg'
      : collapsed
        ? '/images/skedular-icon-primary.svg'
        : '/images/skedular-logo-primary.svg';
  const originalWidth = 779;
  const originalHeight = 163;
  const widthPercentage = ((maxWidth - 70) * 100) / originalWidth;
  const heightPercentage = ((maxWidth - 30) * 100) / originalWidth;
  const width = collapsed ? 30 : (originalWidth * widthPercentage) / 100;
  const height = collapsed ? 30 : (originalHeight * heightPercentage) / 100;
  const styles = {
    width: maxWidth - 30,
    marginLeft: 2,
    marginRight: 2,
    transition: 'border-radius 0.3s ease, width 0.3s ease',
    '&:hover': {
      borderRadius: 4,
      width: maxWidth - 30,
      marginLeft: 2,
      marginRight: 2,
      transition: 'none',
    },
    '&.Mui-selected': {
      width: maxWidth - 30,
      backgroundColor: sandstone,
      '&:hover': {
        backgroundColor: sandstone,
      },
    },
    ...selectedListItemPaddings,
  };

  const handleCollpaseClicked = () => {
    if (toggleCollapse) {
      toggleCollapse(true);
    }
  };

  const handleExpandClicked = () => {
    if (toggleCollapse) {
      toggleCollapse(false);
    }
  };

  return (
    <>
      {enableCollapseButton && !collapsed && (
        <IconButton
          sx={{
            position: 'absolute',
            top: 0,
            right: 0,
            transform: 'translate(0%, 80%)',
            zIndex: (theme) => theme.zIndex.drawer + 1,
          }}
          size="small"
          onClick={handleCollpaseClicked}
        >
          <CollpaseDrawerIcon fontSize="small" />
        </IconButton>
      )}

      <List>
        <ListItem
          disablePadding
          sx={{ width: collapsed ? undefined : maxWidth - 30, justifyContent: 'center', marginLeft: 0, paddingBottom: { xs: 1, sm: 1, md: 5 } }}
          onClick={handleExpandClicked}
        >
          <Image src={logoUrl} width={width} height={height} alt="Skedular" />
        </ListItem>

        <ListItem disablePadding>
          <Link component={NextLink} href="/">
            <ListItemButton selected={pathName === '/'} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName === '/') }}>
              {collapsed && (
                <BodyIconTypography
                  startElement={!hideIcons && <HomeIcon color="inherit" />}
                  invertDefaultColor={pathName === '/' && paletteMode === 'dark'}
                />
              )}
              {!collapsed && (
                <BodyIconTypography
                  label="Home"
                  startElement={!hideIcons && <HomeIcon excludeTooltip color="inherit" />}
                  spacing={3}
                  invertDefaultColor={pathName === '/' && paletteMode === 'dark'}
                />
              )}
            </ListItemButton>
          </Link>
        </ListItem>

        <ListItem disablePadding>
          <Link component={NextLink} href="/organizations">
            <ListItemButton
              selected={pathName.startsWith('/organizations')}
              sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith('/organizations')) }}
            >
              {collapsed && (
                <BodyIconTypography
                  startElement={!hideIcons && <OrganizationIcon color="inherit" />}
                  invertDefaultColor={pathName.startsWith('/organizations') && paletteMode === 'dark'}
                />
              )}
              {!collapsed && (
                <BodyIconTypography
                  label="Organizations"
                  startElement={!hideIcons && <OrganizationIcon excludeTooltip color="inherit" />}
                  spacing={3}
                  invertDefaultColor={pathName.startsWith('/organizations') && paletteMode === 'dark'}
                />
              )}
            </ListItemButton>
          </Link>
        </ListItem>

        <ListItem disablePadding>
          <Link component={NextLink} href="/locations">
            <ListItemButton
              selected={pathName.startsWith('/locations')}
              sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith('/locations')) }}
            >
              {collapsed && (
                <BodyIconTypography
                  startElement={!hideIcons && <LocationIcon color="inherit" />}
                  invertDefaultColor={pathName.startsWith('/locations') && paletteMode === 'dark'}
                />
              )}
              {!collapsed && (
                <BodyIconTypography
                  label="Locations"
                  startElement={!hideIcons && <LocationIcon excludeTooltip color="inherit" />}
                  spacing={3}
                  invertDefaultColor={pathName.startsWith('/locations') && paletteMode === 'dark'}
                />
              )}
            </ListItemButton>
          </Link>
        </ListItem>

        <ListItem disablePadding>
          <Link component={NextLink} href="/teams">
            <ListItemButton
              selected={pathName.startsWith('/teams')}
              sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith('/teams')) }}
            >
              {collapsed && (
                <BodyIconTypography
                  startElement={!hideIcons && <TeamIcon color="inherit" />}
                  invertDefaultColor={pathName.startsWith('/teams') && paletteMode === 'dark'}
                />
              )}
              {!collapsed && (
                <BodyIconTypography
                  label="Teams"
                  startElement={!hideIcons && <TeamIcon excludeTooltip color="inherit" />}
                  spacing={3}
                  invertDefaultColor={pathName.startsWith('/teams') && paletteMode === 'dark'}
                />
              )}
            </ListItemButton>
          </Link>
        </ListItem>

        <ListItem disablePadding>
          <Link component={NextLink} href="/notifications">
            <ListItemButton
              selected={pathName.startsWith('/notifications')}
              sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith('/notifications')) }}
            >
              {collapsed && (
                <BodyIconTypography
                  startElement={!hideIcons && <NotificationsIcon color="inherit" />}
                  invertDefaultColor={pathName.startsWith('/notifications') && paletteMode === 'dark'}
                />
              )}
              {!collapsed && (
                <BodyIconTypography
                  label="Notifications"
                  startElement={!hideIcons && <NotificationsIcon excludeTooltip color="inherit" />}
                  spacing={3}
                  invertDefaultColor={pathName.startsWith('/notifications') && paletteMode === 'dark'}
                />
              )}
            </ListItemButton>
          </Link>
        </ListItem>

        <ListItem disablePadding>
          <Link component={NextLink} href="/settings">
            <ListItemButton
              selected={pathName.startsWith('/settings')}
              sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith('/settings')) }}
            >
              {collapsed && (
                <BodyIconTypography
                  startElement={!hideIcons && <SettingsIcon color="inherit" />}
                  invertDefaultColor={pathName.startsWith('/settings') && paletteMode === 'dark'}
                />
              )}
              {!collapsed && (
                <BodyIconTypography
                  label="Settings"
                  startElement={!hideIcons && <SettingsIcon excludeTooltip color="inherit" />}
                  spacing={3}
                  invertDefaultColor={pathName.startsWith('/settings') && paletteMode === 'dark'}
                />
              )}
            </ListItemButton>
          </Link>
        </ListItem>
      </List>
    </>
  );
};

export default memo(OldLeftSideNavigationMenu);

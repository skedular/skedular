import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { HomeIcon, LocationIcon, NotificationsIcon, OrganizationIcon, SettingsIcon, TeamIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { sandstone } from '@repo/shared/libs/theme';
import Image from 'next/image';
import NextLink from 'next/link';
import { usePathname } from 'next/navigation';
import { memo, useContext } from 'react';

type Props = {
  onReloadRequired: () => void;
  maxWidth: number;
  showIconsOnly?: boolean;
  hideIcons?: boolean;
};

const OldLeftSideNavigationMenu = ({ maxWidth, showIconsOnly, hideIcons }: Props) => {
  const pathName = usePathname();
  const paletteMode = useContext(PaletteModeContext);
  const logoUrl =
    paletteMode === 'dark'
      ? showIconsOnly
        ? '/images/skedular-icon-inverse.svg'
        : '/images/skedular-logo-inverse.svg'
      : showIconsOnly
        ? '/images/skedular-icon-primary.svg'
        : '/images/skedular-logo-primary.svg';
  const originalWidth = 779;
  const originalHeight = 163;
  const percentage = ((maxWidth - 30) * 100) / originalWidth;
  const width = showIconsOnly ? 30 : (originalWidth * percentage) / 100;
  const height = showIconsOnly ? 30 : (originalHeight * percentage) / 100;
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
      backgroundColor: sandstone,
      '&:hover': {
        backgroundColor: sandstone,
      },
    },
  };

  return (
    <List>
      <ListItem disablePadding sx={{ justifyContent: 'center', marginBottom: 3 }}>
        <Image src={logoUrl} width={width} height={height} alt="Skedular" />
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href="/">
          <ListItemButton selected={pathName === '/'} sx={{ ...styles, borderRadius: pathName === '/' ? 4 : 0 }}>
            {showIconsOnly && (
              <BodyIconTypography
                startElement={!hideIcons && <HomeIcon excludeTooltip color="inherit" />}
                invertDefaultColor={pathName === '/' && paletteMode === 'dark'}
              />
            )}
            {!showIconsOnly && (
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
            sx={{ ...styles, borderRadius: pathName.startsWith('/organizations') ? 4 : 0 }}
          >
            {showIconsOnly && (
              <BodyIconTypography
                startElement={!hideIcons && <OrganizationIcon excludeTooltip color="inherit" />}
                invertDefaultColor={pathName.startsWith('/organizations') && paletteMode === 'dark'}
              />
            )}
            {!showIconsOnly && (
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
          <ListItemButton selected={pathName.startsWith('/locations')} sx={{ ...styles, borderRadius: pathName.startsWith('/locations') ? 4 : 0 }}>
            {showIconsOnly && (
              <BodyIconTypography
                startElement={!hideIcons && <LocationIcon excludeTooltip color="inherit" />}
                invertDefaultColor={pathName.startsWith('/locations') && paletteMode === 'dark'}
              />
            )}
            {!showIconsOnly && (
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
          <ListItemButton selected={pathName.startsWith('/teams')} sx={{ ...styles, borderRadius: pathName.startsWith('/teams') ? 4 : 0 }}>
            {showIconsOnly && (
              <BodyIconTypography
                startElement={!hideIcons && <TeamIcon excludeTooltip color="inherit" />}
                invertDefaultColor={pathName.startsWith('/teams') && paletteMode === 'dark'}
              />
            )}
            {!showIconsOnly && (
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
            sx={{ ...styles, borderRadius: pathName.startsWith('/notifications') ? 4 : 0 }}
          >
            {showIconsOnly && (
              <BodyIconTypography
                startElement={!hideIcons && <NotificationsIcon excludeTooltip color="inherit" />}
                invertDefaultColor={pathName.startsWith('/notifications') && paletteMode === 'dark'}
              />
            )}
            {!showIconsOnly && (
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
          <ListItemButton selected={pathName.startsWith('/settings')} sx={{ ...styles, borderRadius: pathName.startsWith('/settings') ? 4 : 0 }}>
            {showIconsOnly && (
              <BodyIconTypography
                startElement={!hideIcons && <SettingsIcon excludeTooltip color="inherit" />}
                invertDefaultColor={pathName.startsWith('/settings') && paletteMode === 'dark'}
              />
            )}
            {!showIconsOnly && (
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
  );
};

export default memo(OldLeftSideNavigationMenu);

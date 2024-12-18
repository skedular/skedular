import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { HomeIcon, LocationIcon, NotificationsIcon, OrganizationIcon, SettingsIcon, TeamIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import Image from 'next/image';
import NextLink from 'next/link';
import { usePathname } from 'next/navigation';
import { memo, useContext } from 'react';

type Props = {
  onReloadRequired: () => void;
  maxWidth: number;
  showIconsOnly?: boolean;
};

const OldLeftSideNavigationMenu = ({ maxWidth, showIconsOnly }: Props) => {
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
    transition: 'border-radius 0.3s ease, width 0.3s ease',
    '&:hover': {
      borderRadius: 4,
      width: maxWidth - 30,
      marginLeft: 2,
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
            {showIconsOnly && <HomeIcon excludeTooltip />}
            {!showIconsOnly && <BodyIconTypography label="Home" startElement={<HomeIcon excludeTooltip />} spacing={3} />}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href="/organizations">
          <ListItemButton selected={pathName === '/organizations'} sx={{ ...styles, borderRadius: pathName === '/organizations' ? 4 : 0 }}>
            {showIconsOnly && <OrganizationIcon excludeTooltip />}
            {!showIconsOnly && <BodyIconTypography label="Organizations" startElement={<OrganizationIcon excludeTooltip />} spacing={3} />}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href="/locations">
          <ListItemButton selected={pathName === '/locations'} sx={{ ...styles, borderRadius: pathName === '/locations' ? 4 : 0 }}>
            {showIconsOnly && <LocationIcon excludeTooltip />}
            {!showIconsOnly && <BodyIconTypography label="Locations" startElement={<LocationIcon excludeTooltip />} spacing={3} />}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href="/teams">
          <ListItemButton selected={pathName === '/teams'} sx={{ ...styles, borderRadius: pathName === '/teams' ? 4 : 0 }}>
            {showIconsOnly && <TeamIcon excludeTooltip />}
            {!showIconsOnly && <BodyIconTypography label="Teams" startElement={<TeamIcon excludeTooltip />} spacing={3} />}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href="/notifications">
          <ListItemButton selected={pathName === '/notifications'} sx={{ ...styles, borderRadius: pathName === '/notifications' ? 4 : 0 }}>
            {showIconsOnly && <NotificationsIcon excludeTooltip />}
            {!showIconsOnly && <BodyIconTypography label="Notifications" startElement={<NotificationsIcon excludeTooltip />} spacing={3} />}{' '}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href="/settings">
          <ListItemButton selected={pathName === '/settings'} sx={{ ...styles, borderRadius: pathName === '/settings' ? 4 : 0 }}>
            {showIconsOnly && <SettingsIcon excludeTooltip />}
            {!showIconsOnly && <BodyIconTypography label="Settings" startElement={<SettingsIcon excludeTooltip />} spacing={3} />}
          </ListItemButton>
        </Link>
      </ListItem>
    </List>
  );
};

export default memo(OldLeftSideNavigationMenu);

import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import { DashboardIcon, LocationIcon, NotificationsIcon, OrganizationIcon, SettingsIcon, TeamIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import Image from 'next/image';
import NextLink from 'next/link';
import { memo, useContext } from 'react';

type Props = {
  maxWidth: number;
};

const LeftSideNavigationMenu = ({ maxWidth }: Props) => {
  const paletteMode = useContext(PaletteModeContext);
  const logoUrl = paletteMode === 'dark' ? '/images/skedular-logo-inverse.svg' : '/images/skedular-logo-primary.svg';
  const originalWidth = 779;
  const originalHeight = 163;
  const percentage = ((maxWidth - 30) * 100) / originalWidth;
  const width = (originalWidth * percentage) / 100;
  const height = (originalHeight * percentage) / 100;

  return (
    <List>
      <ListItem disablePadding sx={{ justifyContent: 'center' }}>
        <Image src={logoUrl} width={width} height={height} alt="Skedular" />
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href="/">
          <ListItemButton>
            <ListItemIcon>
              <DashboardIcon />
            </ListItemIcon>
            <ListItemText>Dashboard</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href="/organizations">
          <ListItemButton>
            <ListItemIcon>
              <OrganizationIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Organizations</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href="/locations">
          <ListItemButton>
            <ListItemIcon>
              <LocationIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Locations</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href="/teams">
          <ListItemButton>
            <ListItemIcon>
              <TeamIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Teams</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href="/notifications">
          <ListItemButton>
            <ListItemIcon>
              <NotificationsIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Notifications</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href="/settings">
          <ListItemButton>
            <ListItemIcon>
              <SettingsIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Settings</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>
    </List>
  );
};

export default memo(LeftSideNavigationMenu);

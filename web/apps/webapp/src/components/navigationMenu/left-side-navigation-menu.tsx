import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import { DashboardIcon, LocationIcon, NotificationsIcon, OrganizationIcon, SettingsIcon, TeamIcon } from '@repo/shared/components/icons';
import { Logo } from '@repo/shared/components/logo';
import NextLink from 'next/link';
import { memo } from 'react';

const LeftSideNavigationMenu = () => {
  return (
    <List>
      <ListItem disablePadding>
        <Logo />
      </ListItem>

      <Link component={NextLink} href="/">
        <ListItem disablePadding>
          <ListItemButton>
            <ListItemIcon>
              <DashboardIcon />
            </ListItemIcon>
            <ListItemText>Dashboard</ListItemText>
          </ListItemButton>
        </ListItem>
      </Link>

      <Link component={NextLink} href="/organizations">
        <ListItem disablePadding>
          <ListItemButton>
            <ListItemIcon>
              <OrganizationIcon />
            </ListItemIcon>
            <ListItemText>Organizations</ListItemText>
          </ListItemButton>
        </ListItem>
      </Link>

      <Link component={NextLink} href="/locations">
        <ListItem disablePadding>
          <ListItemButton>
            <ListItemIcon>
              <LocationIcon />
            </ListItemIcon>
            <ListItemText>Locations</ListItemText>
          </ListItemButton>
        </ListItem>
      </Link>

      <Link component={NextLink} href="/teams">
        <ListItem disablePadding>
          <ListItemButton>
            <ListItemIcon>
              <TeamIcon />
            </ListItemIcon>
            <ListItemText>Teams</ListItemText>
          </ListItemButton>
        </ListItem>
      </Link>

      <Link component={NextLink} href="/notifications">
        <ListItem disablePadding>
          <ListItemButton>
            <ListItemIcon>
              <NotificationsIcon />
            </ListItemIcon>
            <ListItemText>Notifications</ListItemText>
          </ListItemButton>
        </ListItem>
      </Link>

      <Link component={NextLink} href="/settings">
        <ListItem disablePadding>
          <ListItemButton>
            <ListItemIcon>
              <SettingsIcon />
            </ListItemIcon>
            <ListItemText>Settings</ListItemText>
          </ListItemButton>
        </ListItem>
      </Link>
    </List>
  );
};

export default memo(LeftSideNavigationMenu);

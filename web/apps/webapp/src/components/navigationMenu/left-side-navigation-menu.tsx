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

const LeftSideNavigationMenu = () => (
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
            <OrganizationIcon excludeTooltip />
          </ListItemIcon>
          <ListItemText>Organizations</ListItemText>
        </ListItemButton>
      </ListItem>
    </Link>

    <Link component={NextLink} href="/locations">
      <ListItem disablePadding>
        <ListItemButton>
          <ListItemIcon>
            <LocationIcon excludeTooltip />
          </ListItemIcon>
          <ListItemText>Locations</ListItemText>
        </ListItemButton>
      </ListItem>
    </Link>

    <Link component={NextLink} href="/teams">
      <ListItem disablePadding>
        <ListItemButton>
          <ListItemIcon>
            <TeamIcon excludeTooltip />
          </ListItemIcon>
          <ListItemText>Teams</ListItemText>
        </ListItemButton>
      </ListItem>
    </Link>

    <Link component={NextLink} href="/notifications">
      <ListItem disablePadding>
        <ListItemButton>
          <ListItemIcon>
            <NotificationsIcon excludeTooltip />
          </ListItemIcon>
          <ListItemText>Notifications</ListItemText>
        </ListItemButton>
      </ListItem>
    </Link>

    <Link component={NextLink} href="/settings">
      <ListItem disablePadding>
        <ListItemButton>
          <ListItemIcon>
            <SettingsIcon excludeTooltip />
          </ListItemIcon>
          <ListItemText>Settings</ListItemText>
        </ListItemButton>
      </ListItem>
    </Link>
  </List>
);

export default memo(LeftSideNavigationMenu);

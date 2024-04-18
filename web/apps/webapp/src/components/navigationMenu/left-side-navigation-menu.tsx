import { CalendarIcon, LocationIcon, NotificationsIcon, OrganizationIcon, SettingsIcon, TeamIcon } from '@repo/shared/components/icons';
import Divider from '@mui/material/Divider';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import Link from 'next/link';
import { memo } from 'react';

const LeftSideNavigationMenu = () => {
  return (
    <List>
      <Link href="/">
        <ListItem disablePadding>
          <ListItemButton>
            <ListItemIcon>
              <CalendarIcon />
            </ListItemIcon>
            <ListItemText>Calendar</ListItemText>
          </ListItemButton>
        </ListItem>
      </Link>

      <Link href="/organization">
        <ListItem disablePadding>
          <ListItemButton>
            <ListItemIcon>
              <OrganizationIcon />
            </ListItemIcon>
            <ListItemText>Organizations</ListItemText>
          </ListItemButton>
        </ListItem>
      </Link>

      <Link href="/location">
        <ListItem disablePadding>
          <ListItemButton>
            <ListItemIcon>
              <LocationIcon />
            </ListItemIcon>
            <ListItemText>Locations</ListItemText>
          </ListItemButton>
        </ListItem>
      </Link>

      <Link href="/team">
        <ListItem disablePadding>
          <ListItemButton>
            <ListItemIcon>
              <TeamIcon />
            </ListItemIcon>
            <ListItemText>Teams</ListItemText>
          </ListItemButton>
        </ListItem>
      </Link>

      <Link href="/notification">
        <ListItem disablePadding>
          <ListItemButton>
            <ListItemIcon>
              <NotificationsIcon />
            </ListItemIcon>
            <ListItemText>Notifications</ListItemText>
          </ListItemButton>
        </ListItem>
      </Link>

      <Link href="/settings">
        <ListItem disablePadding>
          <ListItemButton>
            <ListItemIcon>
              <SettingsIcon />
            </ListItemIcon>
            <ListItemText>Settings</ListItemText>
          </ListItemButton>
        </ListItem>
        <Divider />
      </Link>
    </List>
  );
};

export default memo(LeftSideNavigationMenu);

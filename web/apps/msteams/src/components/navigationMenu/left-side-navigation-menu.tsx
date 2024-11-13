import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import { HomeIcon, LocationIcon, NotificationsIcon, SettingsIcon, TeamIcon } from '@repo/shared/components/icons';
import { memo } from 'react';
import { useParams } from 'react-router-dom';

const LeftSideNavigationMenu = () => {
  const { organizationId } = useParams();
  let finalOrganizationId = '';

  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] === 'undefined') {
      throw new Error('organizationId is required');
    }

    finalOrganizationId = organizationId[0];
  } else {
    throw new Error('organizationId is required');
  }

  return (
    <List>
      <ListItem disablePadding>
        <Link href={`/organizations/${finalOrganizationId}`}>
          <ListItemButton>
            <ListItemIcon>
              <HomeIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Home</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link href={`/organizations/${finalOrganizationId}/locations`}>
          <ListItemButton>
            <ListItemIcon>
              <LocationIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Locations</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link href={`/organizations/${finalOrganizationId}/teams`}>
          <ListItemButton>
            <ListItemIcon>
              <TeamIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Teams</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link href={`/${finalOrganizationId}/notifications`}>
          <ListItemButton>
            <ListItemIcon>
              <NotificationsIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Notifications</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link href={`/${finalOrganizationId}/settings`}>
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

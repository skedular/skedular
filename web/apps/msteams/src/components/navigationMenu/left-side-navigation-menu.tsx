import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import { BookingIcon, LocationIcon, NotificationsIcon, SettingsIcon, TeamIcon } from '@repo/shared/components/icons';
import { getOrganizationLocationsBaseLink, getOrganizationTeamsBaseLink } from 'components/organization/organization-link';
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
      <Link href={`/organizations/${finalOrganizationId}`}>
        <ListItem disablePadding>
          <ListItemButton>
            <ListItemIcon>
              <BookingIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Bookings</ListItemText>
          </ListItemButton>
        </ListItem>
      </Link>

      <Link href={getOrganizationLocationsBaseLink(finalOrganizationId)}>
        <ListItem disablePadding>
          <ListItemButton>
            <ListItemIcon>
              <LocationIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Locations</ListItemText>
          </ListItemButton>
        </ListItem>
      </Link>

      <Link href={getOrganizationTeamsBaseLink(finalOrganizationId)}>
        <ListItem disablePadding>
          <ListItemButton>
            <ListItemIcon>
              <TeamIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Teams</ListItemText>
          </ListItemButton>
        </ListItem>
      </Link>

      <Link href={`/${finalOrganizationId}/notifications`}>
        <ListItem disablePadding>
          <ListItemButton>
            <ListItemIcon>
              <NotificationsIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Notifications</ListItemText>
          </ListItemButton>
        </ListItem>
      </Link>

      <Link href={`/${finalOrganizationId}/settings`}>
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
};

export default memo(LeftSideNavigationMenu);

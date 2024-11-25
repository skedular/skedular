import Box from '@mui/material/Box';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import { HomeIcon, LocationIcon, NotificationsIcon, SettingsIcon, TeamIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { memo, useContext } from 'react';
import { useLocation, useParams } from 'react-router-dom';

type Props = {
  onReloadRequired: () => void;
  maxWidth: number;
};

const LeftSideNavigationMenu = ({ maxWidth }: Props) => {
  const location = useLocation();
  const pathName = location.pathname;
  const paletteMode = useContext(PaletteModeContext);
  const logoUrl = paletteMode === 'dark' ? '/images/skedular-logo-inverse.svg' : '/images/skedular-logo-primary.svg';
  const originalWidth = 779;
  const originalHeight = 163;
  const percentage = ((maxWidth - 30) * 100) / originalWidth;
  const width = (originalWidth * percentage) / 100;
  const height = (originalHeight * percentage) / 100;
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
      <ListItem disablePadding sx={{ justifyContent: 'center', marginBottom: 3 }}>
        <Box component="img" sx={{ width, height }} alt="Skedular" src={logoUrl} />
      </ListItem>

      <ListItem disablePadding>
        <Link href={`/organizations/${finalOrganizationId}`}>
          <ListItemButton
            selected={pathName === `/organizations/${finalOrganizationId}`}
            sx={{ ...styles, borderRadius: pathName === `/organizations/${finalOrganizationId}` ? 4 : 0 }}
          >
            <ListItemIcon>
              <HomeIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Home</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link href={`/organizations/${finalOrganizationId}/locations`}>
          <ListItemButton
            selected={pathName === `/organizations/${finalOrganizationId}/locations`}
            sx={{ ...styles, borderRadius: pathName === `/organizations/${finalOrganizationId}/locations` ? 4 : 0 }}
          >
            <ListItemIcon>
              <LocationIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Locations</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link href={`/organizations/${finalOrganizationId}/teams`}>
          <ListItemButton
            selected={pathName === `/organizations/${finalOrganizationId}/teams`}
            sx={{ ...styles, borderRadius: pathName === `/organizations/${finalOrganizationId}/teams` ? 4 : 0 }}
          >
            <ListItemIcon>
              <TeamIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Teams</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link href={`/${finalOrganizationId}/notifications`}>
          <ListItemButton
            selected={pathName === `/${finalOrganizationId}/notifications`}
            sx={{ ...styles, borderRadius: pathName === `/${finalOrganizationId}/notifications` ? 4 : 0 }}
          >
            <ListItemIcon>
              <NotificationsIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Notifications</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link href={`/${finalOrganizationId}/settings`}>
          <ListItemButton
            selected={pathName === `/${finalOrganizationId}/settings`}
            sx={{ ...styles, borderRadius: pathName === `/${finalOrganizationId}/settings` ? 4 : 0 }}
          >
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

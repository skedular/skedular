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
import { graphql, useFragment } from 'react-relay';
import { useParams } from 'react-router-dom';
import type { leftSideNavigationMenu_query$key } from './__generated__/leftSideNavigationMenu_query.graphql';

type Props = {
  rootDataRelay: leftSideNavigationMenu_query$key;
  onReloadRequired: () => void;
  maxWidth: number;
};

const LeftSideNavigationMenu = ({ rootDataRelay, maxWidth }: Props) => {
  const rootData = useFragment<leftSideNavigationMenu_query$key>(
    graphql`
      fragment leftSideNavigationMenu_query on Query {
        organization(id: $organizationId) {
          canModify
        }
      }
    `,
    rootDataRelay,
  );

  const paletteMode = useContext(PaletteModeContext);
  const logoUrl = paletteMode === 'dark' ? '/images/skedular-logo-inverse.svg' : '/images/skedular-logo-primary.svg';
  const originalWidth = 779;
  const originalHeight = 163;
  const percentage = ((maxWidth - 30) * 100) / originalWidth;
  const width = (originalWidth * percentage) / 100;
  const height = (originalHeight * percentage) / 100;
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

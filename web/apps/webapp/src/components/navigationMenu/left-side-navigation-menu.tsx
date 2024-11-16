import type { leftSideNavigationMenu_query$key } from '@/queries/__generated__/leftSideNavigationMenu_query.graphql';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import { HomeIcon, LocationIcon, NotificationsIcon, OrganizationIcon, SettingsIcon, TeamIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import Image from 'next/image';
import NextLink from 'next/link';
import { memo, useContext } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: leftSideNavigationMenu_query$key;
  onReloadRequired: () => void;
  maxWidth: number;
};

const LeftSideNavigationMenu = ({ rootDataRelay, maxWidth }: Props) => {
  const rootData = useFragment<leftSideNavigationMenu_query$key>(
    graphql`
      fragment leftSideNavigationMenu_query on Query {
        organization(id: $organizationId) @include(if: $organizationExists) {
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

  return (
    <List>
      <ListItem disablePadding sx={{ justifyContent: 'center' }}>
        <Image src={logoUrl} width={width} height={height} alt="Skedular" />
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href="/">
          <ListItemButton>
            <ListItemIcon>
              <HomeIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Home</ListItemText>
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

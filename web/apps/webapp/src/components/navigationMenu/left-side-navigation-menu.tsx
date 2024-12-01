import {
  getModernOrganizationLocationsBaseLink,
  getModernOrganizationTeamsBaseLink,
  getOrganizationBaseLink,
} from '@/components/organization/organization-link';
import type { leftSideNavigationMenu_query$key } from '@/queries/__generated__/leftSideNavigationMenu_query.graphql';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import { DeskIcon, HomeIcon, LocationIcon, SettingsIcon, TeamIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import Image from 'next/image';
import NextLink from 'next/link';
import { usePathname } from 'next/navigation';
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
          id
          canModify
        }
      }
    `,
    rootDataRelay,
  );

  const pathName = usePathname();
  const paletteMode = useContext(PaletteModeContext);
  const logoUrl = paletteMode === 'dark' ? '/images/skedular-logo-inverse.svg' : '/images/skedular-logo-primary.svg';
  const originalWidth = 779;
  const originalHeight = 163;
  const percentage = ((maxWidth - 30) * 100) / originalWidth;
  const width = (originalWidth * percentage) / 100;
  const height = (originalHeight * percentage) / 100;

  if (!rootData?.organization) {
    return <></>;
  }

  const organizationBaseLink = getOrganizationBaseLink(rootData.organization.id);
  const organizationLocationsBaseLink = getModernOrganizationLocationsBaseLink(rootData.organization.id);
  const organizationTeamsBaseLink = getModernOrganizationTeamsBaseLink(rootData.organization.id);
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
        <Link component={NextLink} href={organizationBaseLink}>
          <ListItemButton selected={pathName === organizationBaseLink} sx={{ ...styles, borderRadius: pathName === organizationBaseLink ? 4 : 0 }}>
            <ListItemIcon>
              <HomeIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Home</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={organizationLocationsBaseLink}>
          <ListItemButton
            selected={pathName === organizationLocationsBaseLink}
            sx={{ ...styles, borderRadius: pathName === organizationLocationsBaseLink ? 4 : 0 }}
          >
            <ListItemIcon>
              <LocationIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Locations</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={organizationTeamsBaseLink}>
          <ListItemButton
            selected={pathName === organizationTeamsBaseLink}
            sx={{ ...styles, borderRadius: pathName === organizationTeamsBaseLink ? 4 : 0 }}
          >
            <ListItemIcon>
              <TeamIcon excludeTooltip />
            </ListItemIcon>
            <ListItemText>Teams</ListItemText>
          </ListItemButton>
        </Link>
      </ListItem>

      {rootData.organization.canModify && (
        <ListItem disablePadding>
          <Link component={NextLink} href="/notifications">
            <ListItemButton selected={pathName === '/notifications'} sx={{ ...styles, borderRadius: pathName === '/notifications' ? 4 : 0 }}>
              <ListItemIcon>
                <DeskIcon excludeTooltip />
              </ListItemIcon>
              <ListItemText>Manage Seats</ListItemText>
            </ListItemButton>
          </Link>
        </ListItem>
      )}

      {rootData.organization.canModify && (
        <ListItem disablePadding>
          <Link component={NextLink} href="/settings">
            <ListItemButton selected={pathName === '/settings'} sx={{ ...styles, borderRadius: pathName === '/settings' ? 4 : 0 }}>
              <ListItemIcon>
                <SettingsIcon excludeTooltip />
              </ListItemIcon>
              <ListItemText>Admin</ListItemText>
            </ListItemButton>
          </Link>
        </ListItem>
      )}
    </List>
  );
};

export default memo(LeftSideNavigationMenu);

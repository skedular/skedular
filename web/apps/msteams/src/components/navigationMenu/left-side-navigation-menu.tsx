import Box from '@mui/material/Box';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { HomeIcon, LocationIcon, NotificationsIcon, SettingsIcon, TeamIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { sandstone } from '@repo/shared/libs/theme';
import { memo, useContext } from 'react';
import { useLocation, useParams } from 'react-router-dom';

type Props = {
  onReloadRequired: () => void;
  maxWidth: number;
  showIconsOnly?: boolean;
};

const LeftSideNavigationMenu = ({ maxWidth, showIconsOnly }: Props) => {
  const location = useLocation();
  const pathName = location.pathname;
  const paletteMode = useContext(PaletteModeContext);
  const logoUrl =
    paletteMode === 'dark'
      ? showIconsOnly
        ? '/images/skedular-icon-inverse.svg'
        : '/images/skedular-logo-inverse.svg'
      : showIconsOnly
        ? '/images/skedular-icon-primary.svg'
        : '/images/skedular-logo-primary.svg';
  const originalWidth = 779;
  const originalHeight = 163;
  const percentage = ((maxWidth - 30) * 100) / originalWidth;
  const width = showIconsOnly ? 30 : (originalWidth * percentage) / 100;
  const height = showIconsOnly ? 30 : (originalHeight * percentage) / 100;
  const styles = {
    width: maxWidth - 30,
    marginLeft: 2,
    transition: 'border-radius 0.3s ease, width 0.3s ease',
    '&:hover': {
      borderRadius: 4,
      width: maxWidth - 30,
      marginLeft: 2,
      transition: 'none',
    },
    '&.Mui-selected': {
      backgroundColor: sandstone,
      '&:hover': {
        backgroundColor: sandstone,
      },
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
            {showIconsOnly && <HomeIcon excludeTooltip color="inherit" />}
            {!showIconsOnly && (
              <BodyIconTypography
                label="Home"
                startElement={<HomeIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={pathName === `/organizations/${finalOrganizationId}` && paletteMode === 'dark'}
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link href={`/organizations/${finalOrganizationId}/locations`}>
          <ListItemButton
            selected={pathName.startsWith(`/organizations/${finalOrganizationId}/locations`)}
            sx={{ ...styles, borderRadius: pathName.startsWith(`/organizations/${finalOrganizationId}/locations`) ? 4 : 0 }}
          >
            {showIconsOnly && <LocationIcon excludeTooltip color="inherit" />}
            {!showIconsOnly && (
              <BodyIconTypography
                label="Locations"
                startElement={<LocationIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={pathName.startsWith(`/organizations/${finalOrganizationId}/locations`) && paletteMode === 'dark'}
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link href={`/organizations/${finalOrganizationId}/teams`}>
          <ListItemButton
            selected={pathName.startsWith(`/organizations/${finalOrganizationId}/teams`)}
            sx={{ ...styles, borderRadius: pathName.startsWith(`/organizations/${finalOrganizationId}/teams`) ? 4 : 0 }}
          >
            {showIconsOnly && <TeamIcon excludeTooltip color="inherit" />}
            {!showIconsOnly && (
              <BodyIconTypography
                label="Teams"
                startElement={<TeamIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={pathName.startsWith(`/organizations/${finalOrganizationId}/teams`) && paletteMode === 'dark'}
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link href={`/${finalOrganizationId}/notifications`}>
          <ListItemButton
            selected={pathName.startsWith(`/${finalOrganizationId}/notifications`)}
            sx={{ ...styles, borderRadius: pathName.startsWith(`/${finalOrganizationId}/notifications`) ? 4 : 0 }}
          >
            {showIconsOnly && <NotificationsIcon excludeTooltip color="inherit" />}
            {!showIconsOnly && (
              <BodyIconTypography
                label="Notifications"
                startElement={<NotificationsIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={pathName.startsWith(`/${finalOrganizationId}/notifications`) && paletteMode === 'dark'}
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link href={`/${finalOrganizationId}/settings`}>
          <ListItemButton
            selected={pathName.startsWith(`/${finalOrganizationId}/settings`)}
            sx={{ ...styles, borderRadius: pathName.startsWith(`/${finalOrganizationId}/settings`) ? 4 : 0 }}
          >
            {showIconsOnly && <SettingsIcon excludeTooltip color="inherit" />}
            {!showIconsOnly && (
              <BodyIconTypography
                label="Settings"
                startElement={<SettingsIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={pathName.startsWith(`/${finalOrganizationId}/settings`) && paletteMode === 'dark'}
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>
    </List>
  );
};

export default memo(LeftSideNavigationMenu);

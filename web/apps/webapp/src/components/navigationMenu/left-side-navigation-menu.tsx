import {
  getModernOrganizationLocationsBaseLink,
  getModernOrganizationMembersBaseLink,
  getModernOrganizationTeamsBaseLink,
  getOrganizationBaseLink,
} from '@/components/organization/organization-link';
import type { leftSideNavigationMenu_query$key } from '@/queries/__generated__/leftSideNavigationMenu_query.graphql';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import Box from '@mui/system/Box';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { HomeIcon, LocationIcon, MembersIcon, SettingsIcon, TeamIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { sandstone } from '@repo/shared/libs/theme';
import Image from 'next/image';
import NextLink from 'next/link';
import { usePathname } from 'next/navigation';
import { memo, useContext } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: leftSideNavigationMenu_query$key;
  onReloadRequired: () => void;
  maxWidth: number;
  showIconsOnly?: boolean;
  hideIcons?: boolean;
};

const LeftSideNavigationMenu = ({ rootDataRelay, maxWidth, showIconsOnly, hideIcons }: Props) => {
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
    marginRight: 2,
    transition: 'border-radius 0.3s ease, width 0.3s ease',
    '&:hover': {
      borderRadius: 4,
      width: maxWidth - 30,
      marginLeft: 2,
      marginRight: 2,
      transition: 'none',
    },
    '&.Mui-selected': {
      backgroundColor: sandstone,
      '&:hover': {
        backgroundColor: sandstone,
      },
    },
  };

  if (!rootData?.organization) {
    return <></>;
  }

  const organizationBaseLink = getOrganizationBaseLink(rootData.organization.id);
  const organizationLocationsBaseLink = getModernOrganizationLocationsBaseLink(rootData.organization.id);
  const organizationTeamsBaseLink = getModernOrganizationTeamsBaseLink(rootData.organization.id);
  const organizationMembersBaseLink = getModernOrganizationMembersBaseLink(rootData.organization.id);

  return (
    <List>
      <Box sx={{ paddingBottom: { xs: 1, sm: 1, md: 1 } }} />

      <ListItem disablePadding sx={{ justifyContent: 'center', marginBottom: 3 }}>
        <Image src={logoUrl} width={width} height={height} alt="Skedular" />
      </ListItem>

      <Box sx={{ paddingBottom: { xs: 1, sm: 1, md: 3 } }} />

      <ListItem disablePadding>
        <Link component={NextLink} href={organizationBaseLink}>
          <ListItemButton
            selected={pathName === organizationBaseLink}
            sx={{ ...styles, borderRadius: pathName === organizationBaseLink ? 4 : 0, paddingRight: 5 }}
          >
            {showIconsOnly && (
              <BodyIconTypography
                startElement={!hideIcons && <HomeIcon excludeTooltip color="inherit" />}
                invertDefaultColor={pathName === organizationBaseLink && paletteMode === 'dark'}
              />
            )}
            {!showIconsOnly && (
              <BodyIconTypography
                label="Home"
                startElement={!hideIcons && <HomeIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={pathName === organizationBaseLink && paletteMode === 'dark'}
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={organizationLocationsBaseLink}>
          <ListItemButton
            selected={pathName.startsWith(organizationLocationsBaseLink)}
            sx={{ ...styles, borderRadius: pathName.startsWith(organizationLocationsBaseLink) ? 4 : 0, paddingRight: 5 }}
          >
            {showIconsOnly && (
              <BodyIconTypography
                startElement={!hideIcons && <LocationIcon excludeTooltip color="inherit" />}
                invertDefaultColor={pathName.startsWith(organizationLocationsBaseLink) && paletteMode === 'dark'}
              />
            )}
            {!showIconsOnly && (
              <BodyIconTypography
                label="Locations"
                startElement={!hideIcons && <LocationIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={pathName.startsWith(organizationLocationsBaseLink) && paletteMode === 'dark'}
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={organizationTeamsBaseLink}>
          <ListItemButton
            selected={pathName.startsWith(organizationTeamsBaseLink)}
            sx={{ ...styles, borderRadius: pathName.startsWith(organizationTeamsBaseLink) ? 4 : 0, paddingRight: 5 }}
          >
            {showIconsOnly && (
              <BodyIconTypography
                startElement={!hideIcons && <TeamIcon excludeTooltip color="inherit" />}
                invertDefaultColor={pathName.startsWith(organizationTeamsBaseLink) && paletteMode === 'dark'}
              />
            )}
            {!showIconsOnly && (
              <BodyIconTypography
                label="Teams"
                startElement={!hideIcons && <TeamIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={pathName.startsWith(organizationTeamsBaseLink) && paletteMode === 'dark'}
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={organizationMembersBaseLink}>
          <ListItemButton
            selected={pathName.startsWith(organizationMembersBaseLink)}
            sx={{ ...styles, borderRadius: pathName.startsWith(organizationMembersBaseLink) ? 4 : 0, paddingRight: 5 }}
          >
            {showIconsOnly && (
              <BodyIconTypography
                startElement={!hideIcons && <MembersIcon excludeTooltip color="inherit" />}
                invertDefaultColor={pathName.startsWith(organizationMembersBaseLink) && paletteMode === 'dark'}
              />
            )}
            {!showIconsOnly && (
              <BodyIconTypography
                label="Members"
                startElement={!hideIcons && <MembersIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={pathName.startsWith(organizationMembersBaseLink) && paletteMode === 'dark'}
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      {rootData.organization.canModify && (
        <ListItem disablePadding>
          <Link component={NextLink} href="/settings">
            <ListItemButton
              selected={pathName.startsWith('/settings')}
              sx={{ ...styles, borderRadius: pathName.startsWith('/settings') ? 4 : 0, paddingRight: 5 }}
            >
              {showIconsOnly && (
                <BodyIconTypography
                  startElement={!hideIcons && <SettingsIcon excludeTooltip color="inherit" />}
                  invertDefaultColor={pathName.startsWith('/settings') && paletteMode === 'dark'}
                />
              )}
              {!showIconsOnly && (
                <BodyIconTypography
                  label="Admin"
                  startElement={!hideIcons && <SettingsIcon excludeTooltip color="inherit" />}
                  spacing={3}
                  invertDefaultColor={pathName.startsWith('/settings') && paletteMode === 'dark'}
                />
              )}
            </ListItemButton>
          </Link>
        </ListItem>
      )}
    </List>
  );
};

export default memo(LeftSideNavigationMenu);

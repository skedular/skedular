import { BodyIconTypography } from '@/components/commons';
import { LocationIcon, OrganizationIcon } from '@/components/icons';
import { getOrganizationAnalyticsBaseLink, getOrganizationLocationsAnalyticsLocationsBaseLink } from '@/components/links';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import {
  getSelectedListItemBorderRadius,
  sandstone,
  secondDrawerCollapsedDrawerWidth,
  secondDrawerCollapsedDrawerWidthPx,
  secondDrawerExpandedDrawerWidth,
  secondDrawerExpandedDrawerWidthPx,
} from '@/libs/theme';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import NextLink from 'next/link';
import { usePathname, useSearchParams } from 'next/navigation';
import { memo, useContext } from 'react';

type Props = {
  organizationUniqueAlphanumericName: string;
  collapsed?: boolean;
  hideIcons?: boolean;
};

const OrganizationAnalyticsLeftSideNavigationMenuContent = ({ organizationUniqueAlphanumericName, collapsed, hideIcons }: Props) => {
  const { integratedPlatrform } = useIntegratedPlatrform();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const paletteMode = useContext(PaletteModeContext);
  const maxWidth = collapsed ? secondDrawerCollapsedDrawerWidth : secondDrawerExpandedDrawerWidth;
  const styles = {
    width: maxWidth,
    borderRadius: 4,
    marginLeft: 1,
    marginRight: 1,
    transition: 'border-radius 0.3s ease, width 0.3s ease',
    '&:hover': {
      width: maxWidth,
      borderRadius: 4,
      marginLeft: 1,
      marginRight: 1,
      transition: 'none',
    },
    '&.Mui-selected': {
      width: maxWidth,
      borderRadius: 4,
      marginLeft: 1,
      marginRight: 1,
      backgroundColor: sandstone,
      '&:hover': {
        width: maxWidth,
        borderRadius: 4,
        marginLeft: 1,
        marginRight: 1,
        backgroundColor: sandstone,
      },
    },
  };

  const fullPath = `${pathname}?${searchParams.toString()}`;
  const organizatinAnalyticsLink = getOrganizationAnalyticsBaseLink(integratedPlatrform, organizationUniqueAlphanumericName);
  const locationsAnalyticsLink = getOrganizationLocationsAnalyticsLocationsBaseLink(integratedPlatrform, organizationUniqueAlphanumericName);

  return (
    <List
      sx={{
        backgroundColor: (theme) => theme.palette.background.paper,
        borderRight: 1,
        borderColor: (theme) => theme.palette.divider,
        paddingTop: { xs: 1, sm: 1, md: 3 },
        height: '100vh',
        position: 'fixed',
        width: collapsed ? secondDrawerCollapsedDrawerWidthPx : secondDrawerExpandedDrawerWidthPx,
      }}
    >
      <ListItem disablePadding>
        <Link component={NextLink} href={organizatinAnalyticsLink}>
          <ListItemButton selected={fullPath === organizatinAnalyticsLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === organizatinAnalyticsLink) }}>
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <OrganizationIcon color="inherit" />}
                invertDefaultColor={fullPath === organizatinAnalyticsLink && paletteMode === 'dark'}
              />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Organization"
                startElement={!hideIcons && <OrganizationIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === organizatinAnalyticsLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={locationsAnalyticsLink}>
          <ListItemButton selected={fullPath === locationsAnalyticsLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === locationsAnalyticsLink) }}>
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <LocationIcon color="inherit" />}
                invertDefaultColor={fullPath === locationsAnalyticsLink && paletteMode === 'dark'}
              />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Locations"
                startElement={!hideIcons && <LocationIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === locationsAnalyticsLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>
    </List>
  );
};

export default memo(OrganizationAnalyticsLeftSideNavigationMenuContent);

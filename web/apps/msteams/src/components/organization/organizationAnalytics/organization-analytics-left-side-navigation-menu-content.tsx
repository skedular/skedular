import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { LocationIcon, OrganizationIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { getSelectedListItemBorderRadius, sandstone } from '@repo/shared/libs/theme';
import { getModernOrganizationAdminSetupBaseLink } from 'components/organization';
import { memo, useContext } from 'react';
import { useLocation, useSearchParams } from 'react-router-dom';
import { collapsedDrawerWidth, collapsedDrawerWidthPx, expandedDrawerWidth, expandedDrawerWidthPx } from './commons';

type Props = {
  organizationId: string;
  collapsed?: boolean;
  hideIcons?: boolean;
};

const OrganizationAnalyticsLeftSideNavigationMenuContent = ({ organizationId, collapsed, hideIcons }: Props) => {
  const location = useLocation();
  const pathname = location.pathname;
  const [searchParams] = useSearchParams();
  const paletteMode = useContext(PaletteModeContext);
  const maxWidth = collapsed ? collapsedDrawerWidth : expandedDrawerWidth;
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
  const organizatinAnalyticsLink = getModernOrganizationAdminSetupBaseLink(organizationId);
  const locationsAnalyticsLink = getModernOrganizationAdminSetupBaseLink(organizationId);

  return (
    <List
      sx={{
        backgroundColor: (theme) => theme.palette.background.paper,
        borderRight: 1,
        borderColor: (theme) => theme.palette.divider,
        paddingTop: { xs: 1, sm: 1, md: 3 },
        height: '100vh',
        position: 'fixed',
        width: collapsed ? collapsedDrawerWidthPx : expandedDrawerWidthPx,
      }}
    >
      <ListItem disablePadding>
        <Link href={organizatinAnalyticsLink}>
          <ListItemButton
            selected={fullPath === organizatinAnalyticsLink}
            sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === organizatinAnalyticsLink) }}
          >
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
        <Link href={locationsAnalyticsLink}>
          <ListItemButton
            selected={fullPath === locationsAnalyticsLink}
            sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === locationsAnalyticsLink) }}
          >
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

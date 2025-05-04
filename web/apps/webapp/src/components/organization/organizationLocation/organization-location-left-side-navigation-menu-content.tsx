import { BodyIconTypography } from '@/components/commons';
import { EditIcon, OpeningHoursIcon, ResourceIcon } from '@/components/icons';
import {
  getOrganizationLocationManageLocationBaseLink,
  getOrganizationLocationManageResourcesBaseLink,
  getOrganizationLocationOpeningHoursBaseLink,
  getOrganizationLocationSetupBaseLink,
} from '@/components/links';
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
  organizationId: string;
  locationId: string;
  collapsed?: boolean;
  hideIcons?: boolean;
};

const OrganizationLocationLeftSideNavigationMenuContent = ({ organizationId, locationId, collapsed, hideIcons }: Props) => {
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
  const setupLink = getOrganizationLocationSetupBaseLink(integratedPlatrform, organizationId, locationId);
  const openingHoursLink = getOrganizationLocationOpeningHoursBaseLink(integratedPlatrform, organizationId, locationId);
  const manageResourcesLink = getOrganizationLocationManageResourcesBaseLink(integratedPlatrform, organizationId, locationId);
  const manageLocationLink = getOrganizationLocationManageLocationBaseLink(integratedPlatrform, organizationId, locationId);

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
        <Link component={NextLink} href={setupLink}>
          <ListItemButton selected={fullPath === setupLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === setupLink) }}>
            {collapsed && <BodyIconTypography startElement={!hideIcons && <EditIcon color="inherit" />} invertDefaultColor={fullPath === setupLink && paletteMode === 'dark'} />}
            {!collapsed && (
              <BodyIconTypography
                label="Location Setup"
                startElement={!hideIcons && <EditIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === setupLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={openingHoursLink}>
          <ListItemButton selected={fullPath === openingHoursLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === openingHoursLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <OpeningHoursIcon color="inherit" />} invertDefaultColor={fullPath === openingHoursLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Opening Hours"
                startElement={!hideIcons && <OpeningHoursIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === openingHoursLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={manageResourcesLink}>
          <ListItemButton selected={fullPath === manageResourcesLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === manageResourcesLink) }}>
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <ResourceIcon color="inherit" excludeTooltip />}
                invertDefaultColor={fullPath === manageResourcesLink && paletteMode === 'dark'}
              />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Manage Resources"
                startElement={!hideIcons && <ResourceIcon color="inherit" excludeTooltip />}
                spacing={3}
                invertDefaultColor={fullPath === manageResourcesLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>

      <ListItem disablePadding>
        <Link component={NextLink} href={manageLocationLink}>
          <ListItemButton selected={fullPath === manageLocationLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(fullPath === manageLocationLink) }}>
            {collapsed && (
              <BodyIconTypography startElement={!hideIcons && <EditIcon color="inherit" />} invertDefaultColor={fullPath === manageLocationLink && paletteMode === 'dark'} />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Manage Location"
                startElement={!hideIcons && <EditIcon color="inherit" />}
                spacing={3}
                invertDefaultColor={fullPath === manageLocationLink && paletteMode === 'dark'}
                noWrap
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>
    </List>
  );
};

export default memo(OrganizationLocationLeftSideNavigationMenuContent);

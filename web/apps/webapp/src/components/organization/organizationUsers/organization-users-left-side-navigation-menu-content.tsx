import { BodyIconTypography } from '@/components/commons';
import { MembersIcon } from '@/components/icons';
import { getOrganizationUsersBaseLink } from '@/components/links';
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
import { usePathname } from 'next/navigation';
import { memo, useContext } from 'react';

type Props = {
  organizationCustomDomain: string;
  collapsed?: boolean;
  hideIcons?: boolean;
};

const OrganizationUsersLeftSideNavigationMenuContent = ({ organizationCustomDomain, collapsed, hideIcons }: Props) => {
  const { integratedPlatrform } = useIntegratedPlatrform();
  const pathName = usePathname();
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

  const memberesLink = getOrganizationUsersBaseLink(integratedPlatrform, organizationCustomDomain);

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
        <Link component={NextLink} href={memberesLink}>
          <ListItemButton selected={pathName === memberesLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName === memberesLink) }}>
            {collapsed && (
              <BodyIconTypography
                startElement={!hideIcons && <MembersIcon excludeTooltip color="inherit" />}
                invertDefaultColor={pathName === memberesLink && paletteMode === 'dark'}
              />
            )}
            {!collapsed && (
              <BodyIconTypography
                label="Users"
                startElement={!hideIcons && <MembersIcon excludeTooltip color="inherit" />}
                spacing={3}
                invertDefaultColor={pathName === memberesLink && paletteMode === 'dark'}
              />
            )}
          </ListItemButton>
        </Link>
      </ListItem>
    </List>
  );
};

export default memo(OrganizationUsersLeftSideNavigationMenuContent);

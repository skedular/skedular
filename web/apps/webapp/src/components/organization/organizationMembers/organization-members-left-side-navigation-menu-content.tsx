import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { MembersIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { getSelectedListItemBorderRadius, sandstone } from '@repo/shared/libs/theme';
import NextLink from 'next/link';
import { usePathname } from 'next/navigation';
import { memo, useContext } from 'react';
import { getModernOrganizationMembersBaseLink } from '../';
import { collapsedDrawerWidth, expandedDrawerWidth } from './commons';

type Props = {
  organizationId: string;
  collapsed?: boolean;
  hideIcons?: boolean;
};

const OrganizationMembersLeftSideNavigationMenuContent = ({ organizationId, collapsed, hideIcons }: Props) => {
  const pathName = usePathname();
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

  const memberesLink = getModernOrganizationMembersBaseLink(organizationId);

  return (
    <List
      sx={{
        backgroundColor: (theme) => theme.palette.background.paper,
        borderRight: 1,
        borderColor: (theme) => theme.palette.divider,
        paddingTop: { xs: 1, sm: 1, md: 3 },
        height: '100vh',
      }}
    >
      <ListItem disablePadding>
        <Link component={NextLink} href={memberesLink}>
          <ListItemButton
            selected={pathName === memberesLink}
            sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName === memberesLink) }}
          >
            <ListItemButton
              selected={pathName === memberesLink}
              sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName === memberesLink) }}
            >
              {collapsed && (
                <BodyIconTypography
                  startElement={!hideIcons && <MembersIcon excludeTooltip color="inherit" />}
                  invertDefaultColor={pathName === memberesLink && paletteMode === 'dark'}
                />
              )}
              {!collapsed && (
                <BodyIconTypography
                  label="Members"
                  startElement={!hideIcons && <MembersIcon excludeTooltip color="inherit" />}
                  spacing={3}
                  invertDefaultColor={pathName === memberesLink && paletteMode === 'dark'}
                />
              )}
            </ListItemButton>
          </ListItemButton>
        </Link>
      </ListItem>
    </List>
  );
};

export default memo(OrganizationMembersLeftSideNavigationMenuContent);

import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { sandstone } from '@repo/shared/libs/theme';
import NextLink from 'next/link';
import { usePathname } from 'next/navigation';
import { memo, useContext } from 'react';
import { getModernOrganizationMembersBaseLink } from '../organization-link';

type Props = {
  organizationId: string;
  maxWidth: number;
};

const OrganizationMembersLeftSideNavigationMenuContent = ({ organizationId, maxWidth }: Props) => {
  const pathName = usePathname();
  const paletteMode = useContext(PaletteModeContext);

  const styles = {
    width: maxWidth,
    marginLeft: 2,
    marginRight: 2,
    transition: 'border-radius 0.3s ease, width 0.3s ease',
    '&:hover': {
      borderRadius: 4,
      width: maxWidth,
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
          <ListItemButton selected={pathName === memberesLink} sx={{ ...styles, borderRadius: pathName === memberesLink ? 4 : 0, paddingRight: 5 }}>
            <BodyIconTypography label="Members" invertDefaultColor={pathName === memberesLink && paletteMode === 'dark'} />
          </ListItemButton>
        </Link>
      </ListItem>
    </List>
  );
};

export default memo(OrganizationMembersLeftSideNavigationMenuContent);

import { BodyIconTypography } from '@skedular/ui';
import { CollpaseDrawerIcon, SignInIcon, SignUpIcon } from '@/components/icons';
import { getSignInLink, getSignUpLink } from '@/components/links';
import { PaletteModeContext } from '@skedular/shared';
import { getSelectedListItemBorderRadius, sandstone, selectedListItemPaddings } from '@skedular/ui';
import Box from '@mui/material/Box';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import Image from 'next/image';
import NextLink from 'next/link';
import { usePathname } from 'next/navigation';
import { memo, useContext } from 'react';
import { secondDrawerCollapsedDrawerWidth, secondDrawerExpandedDrawerWidth } from './commons';

type Props = {
  collapsed?: boolean;
  toggleCollapse?: (collapsed: boolean) => void;
  enableCollapseButton?: boolean;
  hideIcons?: boolean;
};

const UnauthenticatedLeftSideNavigationMenuContent = ({ collapsed, enableCollapseButton, toggleCollapse, hideIcons }: Props) => {
  const pathName = usePathname();
  const paletteMode = useContext(PaletteModeContext);
  const maxWidth = collapsed ? secondDrawerCollapsedDrawerWidth : secondDrawerExpandedDrawerWidth;
  const logoUrl =
    paletteMode === 'dark'
      ? collapsed
        ? '/images/skedular-icon-inverse.svg'
        : '/images/skedular-logo-inverse.svg'
      : collapsed
        ? '/images/skedular-icon-primary.svg'
        : '/images/skedular-logo-primary.svg';
  const originalWidth = 779;
  const originalHeight = 163;
  const widthPercentage = ((maxWidth - 70) * 100) / originalWidth;
  const heightPercentage = ((maxWidth - 30) * 100) / originalWidth;
  const width = collapsed ? 30 : (originalWidth * widthPercentage) / 100;
  const height = collapsed ? 30 : (originalHeight * heightPercentage) / 100;
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
      width: maxWidth - 30,
      backgroundColor: sandstone,
      '&:hover': {
        backgroundColor: sandstone,
      },
    },
    ...selectedListItemPaddings,
  };

  const handleCollpaseClicked = () => {
    if (toggleCollapse) {
      toggleCollapse(true);
    }
  };

  const handleExpandClicked = () => {
    if (toggleCollapse) {
      toggleCollapse(false);
    }
  };

  const signInLink = getSignInLink();
  const signUpLink = getSignUpLink();

  return (
    <>
      <Box>
        {enableCollapseButton && !collapsed && (
          <IconButton
            sx={{
              position: 'absolute',
              top: 0,
              right: 0,
              transform: 'translate(0%, 80%)',
              zIndex: (theme) => theme.zIndex.drawer + 1,
            }}
            size="small"
            onClick={handleCollpaseClicked}
          >
            <CollpaseDrawerIcon fontSize="small" />
          </IconButton>
        )}

        <List>
          <ListItem
            disablePadding
            sx={{ width: collapsed ? undefined : maxWidth - 30, justifyContent: 'center', marginLeft: 0, paddingBottom: { xs: 1, sm: 1, md: 5 } }}
            onClick={handleExpandClicked}
          >
            <Image src={logoUrl} width={width} height={height} unoptimized alt="Skedular" />
          </ListItem>

          <ListItem disablePadding>
            <Link component={NextLink} href={signInLink}>
              <ListItemButton selected={pathName === signInLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName === signInLink) }}>
                {collapsed && (
                  <BodyIconTypography startElement={!hideIcons && <SignInIcon color="inherit" />} invertDefaultColor={pathName === signInLink && paletteMode === 'dark'} />
                )}
                {!collapsed && (
                  <BodyIconTypography
                    label="Sign In"
                    startElement={!hideIcons && <SignInIcon color="inherit" />}
                    spacing={3}
                    invertDefaultColor={pathName === signInLink && paletteMode === 'dark'}
                  />
                )}
              </ListItemButton>
            </Link>
          </ListItem>

          <ListItem disablePadding>
            <Link component={NextLink} href={signUpLink}>
              <ListItemButton selected={pathName === signUpLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName === signUpLink) }}>
                {collapsed && (
                  <BodyIconTypography startElement={!hideIcons && <SignUpIcon color="inherit" />} invertDefaultColor={pathName === signUpLink && paletteMode === 'dark'} />
                )}
                {!collapsed && (
                  <BodyIconTypography
                    label="Sign Up"
                    startElement={!hideIcons && <SignUpIcon color="inherit" />}
                    spacing={3}
                    invertDefaultColor={pathName === signUpLink && paletteMode === 'dark'}
                  />
                )}
              </ListItemButton>
            </Link>
          </ListItem>
        </List>
      </Box>
    </>
  );
};

export default memo(UnauthenticatedLeftSideNavigationMenuContent);

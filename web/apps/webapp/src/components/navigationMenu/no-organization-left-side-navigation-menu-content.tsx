import { BodyIconTypography } from '@/components/commons';
import { BillingAndPaymentIcon, CollpaseDrawerIcon, HomeIcon, NotificationsIcon, SettingsIcon } from '@/components/icons';
import { getBillingAndPaymentLink, getNotificationsLink, getRootLink, getSettingsLink } from '@/components/links';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { getSelectedListItemBorderRadius, sandstone, selectedListItemPaddings } from '@/libs/theme';
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

const NoOrganizationLeftSideNavigationMenuContent = ({ collapsed, enableCollapseButton, toggleCollapse, hideIcons }: Props) => {
  const { integratedPlatrform } = useIntegratedPlatrform();
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

  const homeLink = getRootLink(integratedPlatrform);
  const notificationsLink = getNotificationsLink(integratedPlatrform);
  const billingAndPaymentLink = getBillingAndPaymentLink(integratedPlatrform);
  const settingsBaseLink = getSettingsLink(integratedPlatrform);

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
            <Image src={logoUrl} width={width} height={height} alt="Skedular" />
          </ListItem>

          <ListItem disablePadding>
            <Link component={NextLink} href={homeLink}>
              <ListItemButton selected={pathName === homeLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName === homeLink) }}>
                {collapsed && <BodyIconTypography startElement={!hideIcons && <HomeIcon color="inherit" />} invertDefaultColor={pathName === homeLink && paletteMode === 'dark'} />}
                {!collapsed && (
                  <BodyIconTypography
                    label="Home"
                    startElement={!hideIcons && <HomeIcon excludeTooltip color="inherit" />}
                    spacing={3}
                    invertDefaultColor={pathName === homeLink && paletteMode === 'dark'}
                  />
                )}
              </ListItemButton>
            </Link>
          </ListItem>

          <ListItem disablePadding>
            <Link component={NextLink} href={notificationsLink}>
              <ListItemButton selected={pathName === notificationsLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName === notificationsLink) }}>
                {collapsed && (
                  <BodyIconTypography
                    startElement={!hideIcons && <NotificationsIcon color="inherit" />}
                    invertDefaultColor={pathName === notificationsLink && paletteMode === 'dark'}
                  />
                )}
                {!collapsed && (
                  <BodyIconTypography
                    label="Notifications"
                    startElement={!hideIcons && <NotificationsIcon excludeTooltip color="inherit" />}
                    spacing={3}
                    invertDefaultColor={pathName === notificationsLink && paletteMode === 'dark'}
                  />
                )}
              </ListItemButton>
            </Link>
          </ListItem>

          <ListItem disablePadding>
            <Link component={NextLink} href={billingAndPaymentLink}>
              <ListItemButton selected={pathName === billingAndPaymentLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName === billingAndPaymentLink) }}>
                {collapsed && (
                  <BodyIconTypography
                    startElement={!hideIcons && <BillingAndPaymentIcon color="inherit" />}
                    invertDefaultColor={pathName === billingAndPaymentLink && paletteMode === 'dark'}
                  />
                )}
                {!collapsed && (
                  <BodyIconTypography
                    label="Billing & Payment"
                    startElement={!hideIcons && <BillingAndPaymentIcon color="inherit" />}
                    spacing={3}
                    invertDefaultColor={pathName === billingAndPaymentLink && paletteMode === 'dark'}
                  />
                )}
              </ListItemButton>
            </Link>
          </ListItem>

          <ListItem disablePadding>
            <Link component={NextLink} href={settingsBaseLink}>
              <ListItemButton selected={pathName === settingsBaseLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName === settingsBaseLink) }}>
                {collapsed && (
                  <BodyIconTypography startElement={!hideIcons && <SettingsIcon color="inherit" />} invertDefaultColor={pathName === settingsBaseLink && paletteMode === 'dark'} />
                )}
                {!collapsed && (
                  <BodyIconTypography
                    label="Settings"
                    startElement={!hideIcons && <SettingsIcon excludeTooltip color="inherit" />}
                    spacing={3}
                    invertDefaultColor={pathName === settingsBaseLink && paletteMode === 'dark'}
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

export default memo(NoOrganizationLeftSideNavigationMenuContent);

import { BodyIconTypography, SmallIconTypography, StackColumn } from '@/components/commons';
import {
  AnalyticsIcon,
  BookingIcon,
  CollpaseDrawerIcon,
  HomeIcon,
  LocationIcon,
  MarketplaceIcon,
  MembersIcon,
  SettingsIcon,
  SetupMarketplaceIcon,
  TeamIcon,
  UpgradeIcon,
} from '@/components/icons';
import {
  getOrganizationAdminSetupBaseLink,
  getOrganizationAdminSubscriptionsBaseLink,
  getOrganizationAnalyticsBaseLink,
  getOrganizationBaseLink,
  getOrganizationBookingsBaseLink,
  getOrganizationLocationsBaseLink,
  getOrganizationMarketplaceBaseLink,
  getOrganizationMarketplaceSetupBaseLink,
  getOrganizationTeamsBaseLink,
  getOrganizationUsersBaseLink,
} from '@/components/links';
import { InvitePeopleToJoinOrganizationButton } from '@/components/organization/invitePeopleToJoinOrganization';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { coal, defaultPadding, emerald, getSelectedListItemBorderRadius, sandstone, selectedListItemPaddings } from '@/libs/theme';
import type { leftSideNavigationMenuContent_query$key } from '@/queries/__generated__/leftSideNavigationMenuContent_query.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import Image from 'next/image';
import NextLink from 'next/link';
import { useParams, usePathname } from 'next/navigation';
import { memo, useContext } from 'react';
import { graphql, useFragment } from 'react-relay';
import { secondDrawerCollapsedDrawerWidth, secondDrawerExpandedDrawerWidth } from './commons';

type Props = {
  rootDataRelay: leftSideNavigationMenuContent_query$key;
  collapsed?: boolean;
  toggleCollapse?: (collapsed: boolean) => void;
  enableCollapseButton?: boolean;
  hideIcons?: boolean;
};

const LeftSideNavigationMenuContent = ({ rootDataRelay, collapsed, enableCollapseButton, toggleCollapse, hideIcons }: Props) => {
  const rootData = useFragment<leftSideNavigationMenuContent_query$key>(
    graphql`
      fragment leftSideNavigationMenuContent_query on Query {
        organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) @include(if: $organizationExists) {
          id
          uniqueAlphanumericName
          type {
            type
          }
          canModify
          canViewAnalytics
          activeOffering {
            free
            earlyBird
          }
        }
      }
    `,
    rootDataRelay,
  );

  const { integratedPlatrform } = useIntegratedPlatrform();
  const pathName = usePathname();
  const paletteMode = useContext(PaletteModeContext);
  const { organizationUniqueAlphanumericName } = useParams();
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

  let finalOrganizationUniqueAlphanumericName = '';

  if (typeof organizationUniqueAlphanumericName === 'string') {
    finalOrganizationUniqueAlphanumericName = organizationUniqueAlphanumericName;
  } else if (Array.isArray(organizationUniqueAlphanumericName)) {
    if (typeof organizationUniqueAlphanumericName[0] === 'undefined') {
      throw new Error('organizationUniqueAlphanumericName is required');
    }

    finalOrganizationUniqueAlphanumericName = organizationUniqueAlphanumericName[0];
  }

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

  if (!rootData?.organization) {
    return null;
  }

  const organizationBaseLink = getOrganizationBaseLink(integratedPlatrform, rootData.organization.uniqueAlphanumericName!);
  const organizationBookingsBaseLink = getOrganizationBookingsBaseLink(integratedPlatrform, rootData.organization.uniqueAlphanumericName!);
  const organizationLocationsBaseLink = getOrganizationLocationsBaseLink(integratedPlatrform, rootData.organization.uniqueAlphanumericName!);
  const organizationTeamsBaseLink = getOrganizationTeamsBaseLink(integratedPlatrform, rootData.organization.uniqueAlphanumericName!);
  const organizationMembersBaseLink = getOrganizationUsersBaseLink(integratedPlatrform, rootData.organization.uniqueAlphanumericName!);
  const organizationAnalyticsSetupBaseLink = getOrganizationAnalyticsBaseLink(integratedPlatrform, rootData.organization.uniqueAlphanumericName!);
  const organizationMarketplaceSetupBaseLink = getOrganizationMarketplaceSetupBaseLink(integratedPlatrform, rootData.organization.uniqueAlphanumericName!);
  const organizationMarketplaceBaseLink = getOrganizationMarketplaceBaseLink(integratedPlatrform, rootData.organization.uniqueAlphanumericName!);
  const organizationAdminSetupBaseLink = getOrganizationAdminSetupBaseLink(integratedPlatrform, rootData.organization.uniqueAlphanumericName!);

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
            <Link component={NextLink} href={organizationBaseLink}>
              <ListItemButton selected={pathName === organizationBaseLink} sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName === organizationBaseLink) }}>
                {collapsed && (
                  <BodyIconTypography startElement={!hideIcons && <HomeIcon color="inherit" />} invertDefaultColor={pathName === organizationBaseLink && paletteMode === 'dark'} />
                )}
                {!collapsed && (
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
            <Link component={NextLink} href={organizationBookingsBaseLink}>
              <ListItemButton
                selected={pathName === organizationBookingsBaseLink}
                sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName === organizationBookingsBaseLink) }}
              >
                {collapsed && (
                  <BodyIconTypography
                    startElement={!hideIcons && <BookingIcon color="inherit" />}
                    invertDefaultColor={pathName === organizationBookingsBaseLink && paletteMode === 'dark'}
                  />
                )}
                {!collapsed && (
                  <BodyIconTypography
                    label="Bookings"
                    startElement={!hideIcons && <BookingIcon excludeTooltip color="inherit" />}
                    spacing={3}
                    invertDefaultColor={pathName === organizationBookingsBaseLink && paletteMode === 'dark'}
                  />
                )}
              </ListItemButton>
            </Link>
          </ListItem>

          <ListItem disablePadding>
            <Link component={NextLink} href={organizationLocationsBaseLink}>
              <ListItemButton
                selected={pathName.startsWith(organizationLocationsBaseLink)}
                sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith(organizationLocationsBaseLink)) }}
              >
                {collapsed && (
                  <BodyIconTypography
                    startElement={!hideIcons && <LocationIcon color="inherit" />}
                    invertDefaultColor={pathName.startsWith(organizationLocationsBaseLink) && paletteMode === 'dark'}
                  />
                )}
                {!collapsed && (
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
                sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith(organizationTeamsBaseLink)) }}
              >
                {collapsed && (
                  <BodyIconTypography
                    startElement={!hideIcons && <TeamIcon color="inherit" />}
                    invertDefaultColor={pathName.startsWith(organizationTeamsBaseLink) && paletteMode === 'dark'}
                  />
                )}
                {!collapsed && (
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
                sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith(organizationMembersBaseLink)) }}
              >
                {collapsed && (
                  <BodyIconTypography
                    startElement={!hideIcons && <MembersIcon color="inherit" />}
                    invertDefaultColor={pathName.startsWith(organizationMembersBaseLink) && paletteMode === 'dark'}
                  />
                )}
                {!collapsed && (
                  <BodyIconTypography
                    label="Users"
                    startElement={!hideIcons && <MembersIcon excludeTooltip color="inherit" />}
                    spacing={3}
                    invertDefaultColor={pathName.startsWith(organizationMembersBaseLink) && paletteMode === 'dark'}
                  />
                )}
              </ListItemButton>
            </Link>
          </ListItem>

          {rootData.organization.type.type === 'MARKETPLACE' && (
            <ListItem disablePadding>
              <Link component={NextLink} href={organizationMarketplaceBaseLink}>
                <ListItemButton
                  selected={pathName.startsWith(organizationMarketplaceBaseLink)}
                  sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith(organizationMarketplaceBaseLink)) }}
                >
                  {collapsed && (
                    <BodyIconTypography
                      startElement={!hideIcons && <SetupMarketplaceIcon color="inherit" />}
                      invertDefaultColor={pathName.startsWith(organizationMarketplaceBaseLink) && paletteMode === 'dark'}
                    />
                  )}
                  {!collapsed && (
                    <BodyIconTypography
                      label="Marketplace"
                      startElement={!hideIcons && <SetupMarketplaceIcon color="inherit" />}
                      spacing={3}
                      invertDefaultColor={pathName.startsWith(organizationMarketplaceBaseLink) && paletteMode === 'dark'}
                    />
                  )}
                </ListItemButton>
              </Link>
            </ListItem>
          )}

          {rootData.organization.canModify && rootData.organization.type.type === 'MARKETPLACE' && (
            <ListItem disablePadding>
              <Link component={NextLink} href={organizationMarketplaceSetupBaseLink}>
                <ListItemButton
                  selected={pathName.startsWith(organizationMarketplaceSetupBaseLink)}
                  sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith(organizationMarketplaceSetupBaseLink)) }}
                >
                  {collapsed && (
                    <BodyIconTypography
                      startElement={!hideIcons && <MarketplaceIcon color="inherit" />}
                      invertDefaultColor={pathName.startsWith(organizationMarketplaceSetupBaseLink) && paletteMode === 'dark'}
                    />
                  )}
                  {!collapsed && (
                    <BodyIconTypography
                      label="Setup Marketplace"
                      startElement={!hideIcons && <MarketplaceIcon color="inherit" />}
                      spacing={3}
                      invertDefaultColor={pathName.startsWith(organizationMarketplaceSetupBaseLink) && paletteMode === 'dark'}
                    />
                  )}
                </ListItemButton>
              </Link>
            </ListItem>
          )}

          {rootData.organization.canModify && (
            <ListItem disablePadding>
              <Link component={NextLink} href={organizationAdminSetupBaseLink}>
                <ListItemButton
                  selected={pathName.startsWith(organizationAdminSetupBaseLink)}
                  sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith(organizationAdminSetupBaseLink)) }}
                >
                  {collapsed && (
                    <BodyIconTypography
                      startElement={!hideIcons && <SettingsIcon color="inherit" />}
                      invertDefaultColor={pathName.startsWith(organizationAdminSetupBaseLink) && paletteMode === 'dark'}
                    />
                  )}
                  {!collapsed && (
                    <BodyIconTypography
                      label="Admin"
                      startElement={!hideIcons && <SettingsIcon excludeTooltip color="inherit" />}
                      spacing={3}
                      invertDefaultColor={pathName.startsWith(organizationAdminSetupBaseLink) && paletteMode === 'dark'}
                    />
                  )}
                </ListItemButton>
              </Link>
            </ListItem>
          )}

          {rootData.organization.canViewAnalytics && (
            <ListItem disablePadding>
              <Link component={NextLink} href={organizationAnalyticsSetupBaseLink}>
                <ListItemButton
                  selected={pathName.startsWith(organizationAnalyticsSetupBaseLink)}
                  sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith(organizationAnalyticsSetupBaseLink)) }}
                >
                  {collapsed && (
                    <BodyIconTypography
                      startElement={!hideIcons && <AnalyticsIcon color="inherit" />}
                      invertDefaultColor={pathName.startsWith(organizationAnalyticsSetupBaseLink) && paletteMode === 'dark'}
                    />
                  )}
                  {!collapsed && (
                    <BodyIconTypography
                      label="Analytics"
                      startElement={!hideIcons && <AnalyticsIcon color="inherit" />}
                      spacing={3}
                      invertDefaultColor={pathName.startsWith(organizationAnalyticsSetupBaseLink) && paletteMode === 'dark'}
                    />
                  )}
                </ListItemButton>
              </Link>
            </ListItem>
          )}
        </List>
      </Box>

      {!collapsed && finalOrganizationUniqueAlphanumericName && (
        <>
          <Box sx={{ flexGrow: 1 }} />
          <Box sx={{ backgroundColor: paletteMode === 'dark' ? emerald : coal, position: 'absolute', bottom: 0, width: '100%' }}>
            <StackColumn sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', padding: defaultPadding }}>
              {rootData.organization.activeOffering && rootData.organization.activeOffering.free && !rootData.organization.activeOffering.earlyBird && (
                <Button
                  href={getOrganizationAdminSubscriptionsBaseLink(integratedPlatrform, finalOrganizationUniqueAlphanumericName)}
                  variant="contained"
                  color="secondary"
                  sx={{ textTransform: 'none', paddingTop: 1, paddingBottom: 1, width: 210 }}
                >
                  <BodyIconTypography label="Upgrade Plan" endElement={<UpgradeIcon fontSize="medium" />} color="inherit" />
                </Button>
              )}

              <InvitePeopleToJoinOrganizationButton
                variant="contained"
                organizationUniqueAlphanumericName={finalOrganizationUniqueAlphanumericName}
                label="Invite Teammates"
                size="medium"
                sx={{ backgroundColor: paletteMode === 'dark' ? coal : emerald, paddingTop: 1, paddingBottom: 1, width: 210 }}
                color={paletteMode === 'dark' ? sandstone : coal}
              />
              <SmallIconTypography label="Add teammates to your organization" invertDefaultColor />
            </StackColumn>
          </Box>
        </>
      )}
    </>
  );
};

export default memo(LeftSideNavigationMenuContent);

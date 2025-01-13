import { InvitePeopleToJoinOrganizationButton } from '@/components/organization/invitePeopleToJoinOrganization';
import {
  getModernOrganizationAdminSetupBaseLink,
  getModernOrganizationAdminSubscriptionsBaseLink,
  getModernOrganizationLocationsBaseLink,
  getModernOrganizationMembersBaseLink,
  getModernOrganizationNotificationsBaseLink,
  getModernOrganizationTeamsBaseLink,
  getOrganizationBaseLink,
} from '@/components/organization/organization-link';
import type { modernLeftSideNavigationMenuContent_query$key } from '@/queries/__generated__/modernLeftSideNavigationMenuContent_query.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import { BodyIconTypography, SmallIconTypography, StackColumn } from '@repo/shared/components/commons';
import {
  CollpaseDrawerIcon,
  HomeIcon,
  LocationIcon,
  MembersIcon,
  NotificationsIcon,
  SettingsIcon,
  TeamIcon,
  UpgradeIcon,
} from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { coal, defaultPadding, emerald, getSelectedListItemBorderRadius, sandstone, selectedListItemPaddings } from '@repo/shared/libs/theme';
import Image from 'next/image';
import NextLink from 'next/link';
import { useParams, usePathname } from 'next/navigation';
import { memo, useContext } from 'react';
import { graphql, useFragment } from 'react-relay';
import { collapsedDrawerWidth, expandedDrawerWidth } from './commons';

type Props = {
  rootDataRelay: modernLeftSideNavigationMenuContent_query$key;
  collapsed?: boolean;
  toggleCollapse?: (collapsed: boolean) => void;
  enableCollapseButton?: boolean;
  hideIcons?: boolean;
};

const ModernLeftSideNavigationMenuContent = ({ rootDataRelay, collapsed, enableCollapseButton, toggleCollapse, hideIcons }: Props) => {
  const rootData = useFragment<modernLeftSideNavigationMenuContent_query$key>(
    graphql`
      fragment modernLeftSideNavigationMenuContent_query on Query {
        organization(id: $organizationId) @include(if: $organizationExists) {
          id
          canModify
        }
        organizationDesksAvailability(where: { organizationId: $organizationId, date: $todayDate }) @include(if: $organizationExists) {
          desksCount
          availableDesksCount
        }
      }
    `,
    rootDataRelay,
  );

  const pathName = usePathname();
  const paletteMode = useContext(PaletteModeContext);
  const { organizationId } = useParams();
  const maxWidth = collapsed ? collapsedDrawerWidth : expandedDrawerWidth;
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

  let finalOrganizationId = '';

  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] === 'undefined') {
      throw new Error('organizationId is required');
    }

    finalOrganizationId = organizationId[0];
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
    return <></>;
  }

  const organizationBaseLink = getOrganizationBaseLink(rootData.organization.id);
  const organizationLocationsBaseLink = getModernOrganizationLocationsBaseLink(rootData.organization.id);
  const organizationTeamsBaseLink = getModernOrganizationTeamsBaseLink(rootData.organization.id);
  const organizationMembersBaseLink = getModernOrganizationMembersBaseLink(rootData.organization.id);
  const notificationsBaseLink = getModernOrganizationNotificationsBaseLink(rootData.organization.id);
  const organizationAdminSetupBaseLink = getModernOrganizationAdminSetupBaseLink(rootData.organization.id);

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
              <ListItemButton
                selected={pathName === organizationBaseLink}
                sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName === organizationBaseLink) }}
              >
                {collapsed && (
                  <BodyIconTypography
                    startElement={!hideIcons && <HomeIcon color="inherit" />}
                    invertDefaultColor={pathName === organizationBaseLink && paletteMode === 'dark'}
                  />
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
                    label="Members"
                    startElement={!hideIcons && <MembersIcon excludeTooltip color="inherit" />}
                    spacing={3}
                    invertDefaultColor={pathName.startsWith(organizationMembersBaseLink) && paletteMode === 'dark'}
                  />
                )}
              </ListItemButton>
            </Link>
          </ListItem>

          <ListItem disablePadding>
            <Link component={NextLink} href={notificationsBaseLink}>
              <ListItemButton
                selected={pathName.startsWith(notificationsBaseLink)}
                sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith(notificationsBaseLink)) }}
              >
                {collapsed && (
                  <BodyIconTypography
                    startElement={!hideIcons && <NotificationsIcon color="inherit" />}
                    invertDefaultColor={pathName.startsWith(notificationsBaseLink) && paletteMode === 'dark'}
                  />
                )}
                {!collapsed && (
                  <BodyIconTypography
                    label="Notifications"
                    startElement={!hideIcons && <NotificationsIcon excludeTooltip color="inherit" />}
                    spacing={3}
                    invertDefaultColor={pathName.startsWith(notificationsBaseLink) && paletteMode === 'dark'}
                  />
                )}
              </ListItemButton>
            </Link>
          </ListItem>

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
        </List>
      </Box>

      {!collapsed && finalOrganizationId && (
        <>
          <Box sx={{ flexGrow: 1 }} />
          <Box sx={{ backgroundColor: paletteMode === 'dark' ? emerald : coal, position: 'absolute', bottom: 0, width: '100%' }}>
            <StackColumn sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', padding: defaultPadding }}>
              <Button
                href={getModernOrganizationAdminSubscriptionsBaseLink(finalOrganizationId)}
                variant="contained"
                color="secondary"
                sx={{ textTransform: 'none', paddingTop: 1, paddingBottom: 1, width: 210 }}
              >
                <BodyIconTypography label="Upgrade Plan" endElement={<UpgradeIcon fontSize="medium" />} color="inherit" />
              </Button>

              {rootData.organizationDesksAvailability && (
                <SmallIconTypography
                  label={`${rootData.organizationDesksAvailability.availableDesksCount}/${rootData.organizationDesksAvailability.desksCount} seats available`}
                  invertDefaultColor
                />
              )}

              <InvitePeopleToJoinOrganizationButton
                variant="contained"
                organizationId={finalOrganizationId}
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

export default memo(ModernLeftSideNavigationMenuContent);

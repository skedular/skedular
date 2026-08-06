import { AnalyticsIcon, BillingAndPaymentIcon, BillingIcon, CollpaseDrawerIcon, HomeIcon, LocationIcon, MembersIcon, RefundIcon, SettingsIcon } from '@/components/icons';
import {
  getOrganizationAdminBaseLink,
  getOrganizationAnalyticsBaseLink,
  getOrganizationBaseLink,
  getOrganizationLocationsBaseLink,
  getOrganizationPaymentsBaseLink,
  getOrganizationRefundsBaseLink,
  getOrganizationSubscriptionsBaseLink,
  getOrganizationUsersBaseLink,
} from '@/components/links';
import { InvitePeopleToJoinOrganizationButton } from '@/components/organization/invitePeopleToJoinOrganization';
import type { leftSideNavigationMenuContent_query$key } from '@/queries/__generated__/leftSideNavigationMenuContent_query.graphql';
import Box from '@mui/material/Box';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import { PaletteModeContext, useIntegratedPlatform, useKnownParams } from '@skedular/shared';
import {
  BodyIconTypography,
  coal,
  defaultPadding,
  emerald,
  getSelectedListItemBorderRadius,
  sandstone,
  selectedListItemPaddings,
  SmallIconTypography,
  StackColumn,
} from '@skedular/ui';
import Image from 'next/image';
import NextLink from 'next/link';
import { usePathname } from 'next/navigation';
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
        organization(customDomain: $organizationCustomDomain) {
          id
          customDomain
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

  const { integratedPlatform } = useIntegratedPlatform();
  const pathName = usePathname();
  const paletteMode = useContext(PaletteModeContext);
  const { organizationCustomDomain } = useKnownParams();
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

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
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

  const organizationBaseLink = getOrganizationBaseLink(integratedPlatform, rootData.organization.customDomain!);
  const organizationLocationsBaseLink = getOrganizationLocationsBaseLink(integratedPlatform, rootData.organization.customDomain!);
  const organizationMembersBaseLink = getOrganizationUsersBaseLink(integratedPlatform, rootData.organization.customDomain!);
  const organizationPaymentsBaseLink = getOrganizationPaymentsBaseLink(integratedPlatform, rootData.organization.customDomain!);
  const organizationRefundsBaseLink = getOrganizationRefundsBaseLink(integratedPlatform, rootData.organization.customDomain!);
  const organizationSubscriptionsBaseLink = getOrganizationSubscriptionsBaseLink(integratedPlatform, rootData.organization.customDomain!);
  const organizationAnalyticsSetupBaseLink = getOrganizationAnalyticsBaseLink(integratedPlatform, rootData.organization.customDomain!);
  const organizationAdminBaseLink = getOrganizationAdminBaseLink(integratedPlatform, rootData.organization.customDomain!);

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
            <Link component={NextLink} href={organizationSubscriptionsBaseLink}>
              <ListItemButton
                selected={pathName.startsWith(organizationSubscriptionsBaseLink)}
                sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith(organizationSubscriptionsBaseLink)) }}
              >
                {collapsed ? (
                  <BodyIconTypography
                    startElement={!hideIcons && <BillingIcon color="inherit" />}
                    invertDefaultColor={pathName.startsWith(organizationSubscriptionsBaseLink) && paletteMode === 'dark'}
                  />
                ) : (
                  <BodyIconTypography
                    label="Purchases"
                    startElement={!hideIcons && <BillingIcon color="inherit" />}
                    spacing={3}
                    invertDefaultColor={pathName.startsWith(organizationSubscriptionsBaseLink) && paletteMode === 'dark'}
                  />
                )}
              </ListItemButton>
            </Link>
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

          <ListItem disablePadding>
            <Link component={NextLink} href={organizationPaymentsBaseLink}>
              <ListItemButton
                selected={pathName.startsWith(organizationPaymentsBaseLink)}
                sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith(organizationPaymentsBaseLink)) }}
              >
                {collapsed ? (
                  <BodyIconTypography
                    startElement={!hideIcons && <BillingAndPaymentIcon color="inherit" />}
                    invertDefaultColor={pathName.startsWith(organizationPaymentsBaseLink) && paletteMode === 'dark'}
                  />
                ) : (
                  <BodyIconTypography
                    label="Payments"
                    startElement={!hideIcons && <BillingAndPaymentIcon color="inherit" />}
                    spacing={3}
                    invertDefaultColor={pathName.startsWith(organizationPaymentsBaseLink) && paletteMode === 'dark'}
                  />
                )}
              </ListItemButton>
            </Link>
          </ListItem>

          {rootData.organization.canModify && (
            <ListItem disablePadding>
              <Link component={NextLink} href={organizationRefundsBaseLink}>
                <ListItemButton
                  selected={pathName.startsWith(organizationRefundsBaseLink)}
                  sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith(organizationRefundsBaseLink)) }}
                >
                  {collapsed ? (
                    <BodyIconTypography
                      startElement={!hideIcons && <RefundIcon color="inherit" />}
                      invertDefaultColor={pathName.startsWith(organizationRefundsBaseLink) && paletteMode === 'dark'}
                    />
                  ) : (
                    <BodyIconTypography
                      label="Refunds"
                      startElement={!hideIcons && <RefundIcon color="inherit" />}
                      spacing={3}
                      invertDefaultColor={pathName.startsWith(organizationRefundsBaseLink) && paletteMode === 'dark'}
                    />
                  )}
                </ListItemButton>
              </Link>
            </ListItem>
          )}

          {rootData.organization.canModify && (
            <ListItem disablePadding>
              <Link component={NextLink} href={organizationAdminBaseLink}>
                <ListItemButton
                  selected={pathName.startsWith(organizationAdminBaseLink)}
                  sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathName.startsWith(organizationAdminBaseLink)) }}
                >
                  {collapsed && (
                    <BodyIconTypography
                      startElement={!hideIcons && <SettingsIcon color="inherit" />}
                      invertDefaultColor={pathName.startsWith(organizationAdminBaseLink) && paletteMode === 'dark'}
                    />
                  )}
                  {!collapsed && (
                    <BodyIconTypography
                      label="Admin"
                      startElement={!hideIcons && <SettingsIcon excludeTooltip color="inherit" />}
                      spacing={3}
                      invertDefaultColor={pathName.startsWith(organizationAdminBaseLink) && paletteMode === 'dark'}
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

      {!collapsed && organizationCustomDomain && (
        <>
          <Box sx={{ flexGrow: 1 }} />
          <Box sx={{ backgroundColor: paletteMode === 'dark' ? emerald : coal, position: 'absolute', bottom: 0, width: '100%' }}>
            <StackColumn sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', padding: defaultPadding }}>
              <InvitePeopleToJoinOrganizationButton
                variant="contained"
                organizationCustomDomain={organizationCustomDomain}
                label="Invite People"
                size="medium"
                sx={{ backgroundColor: paletteMode === 'dark' ? coal : emerald, paddingTop: 1, paddingBottom: 1, width: 210 }}
                color={paletteMode === 'dark' ? sandstone : coal}
              />
              <SmallIconTypography label="Add people to your organization" invertDefaultColor />
            </StackColumn>
          </Box>
        </>
      )}
    </>
  );
};

export default memo(LeftSideNavigationMenuContent);

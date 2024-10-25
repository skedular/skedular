import { NewFeedbackDialog } from '@/components/feedback';
import type { appBar_query$key } from '@/queries/__generated__/appBar_query.graphql';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import Breadcrumbs from '@mui/material/Breadcrumbs';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Stack from '@mui/material/Stack';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { FeedbackIcon, LogoutIcon, SettingsIcon } from '@repo/shared/components/icons';
import { BreadcrumpsContext, PaletteModeContext, UpdatePaletteModeContext } from '@repo/shared/libs/providers';
import { getCustomerFullName } from '@repo/shared/libs/utils';
import { signOut } from 'next-auth/react';
import NextLink from 'next/link';
import { memo, useContext, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: appBar_query$key;
  onReloadRequired: () => void;
  breadcrumbs?: AppBarBreadcrumbs;
};

type AppBarBreadcrumbsItem = {
  href: string;
  label: string;
  icon?: React.ReactNode;
};

export type AppBarBreadcrumbs = {
  items?: AppBarBreadcrumbsItem[];
  lastItemLabel?: string;
  lastItemIcon?: React.ReactNode;
};

const AppBar = ({ rootDataRelay, breadcrumbs }: Props) => {
  const rootData = useFragment<appBar_query$key>(
    graphql`
      fragment appBar_query on Query {
        me {
          email {
            email
            verified
          }
          givenName
          middleName
          familyName
          photoUrl
        }
        ...newFeedbackDialog_query
      }
    `,
    rootDataRelay,
  );

  const paletteMode = useContext(PaletteModeContext);
  const breadcrumpsContext = useContext(BreadcrumpsContext);
  const updatePaletteMode = useContext(UpdatePaletteModeContext);
  const [profileOpenAnchorEl, setProfileOpenAnchorEl] = useState<null | HTMLElement>(null);
  const [submitFeedbackDialogOpen, setSubmitFeedbackDialogOpen] = useState(false);

  const handleProfileMenuOpenClick = (event: React.MouseEvent<HTMLElement>) => {
    setProfileOpenAnchorEl(event.currentTarget);
  };

  const handleProfileMenuCloseClick = () => {
    setProfileOpenAnchorEl(null);
  };

  const handleSignOutClick = () => {
    setProfileOpenAnchorEl(null);
    signOut();
  };

  const handleSubmitFeedbackSendClick = () => {
    setSubmitFeedbackDialogOpen(false);
  };

  const handleSubmitFeedbackCancelClick = () => {
    setSubmitFeedbackDialogOpen(false);
  };

  const breadcrumpsLinks = useMemo(() => {
    if (!breadcrumbs?.items) {
      return [];
    }

    return breadcrumbs.items.map((item) => {
      return breadcrumpsContext.has(item.href)
        ? {
            icon: item.icon,
            href: item.href,
            label: breadcrumpsContext.get(item.href)!,
          }
        : item;
    });
  }, [breadcrumbs?.items, breadcrumpsContext]);

  const lastBreadcrumps = useMemo(() => {
    if (!breadcrumbs?.lastItemLabel) {
      return undefined;
    }

    const label = breadcrumpsContext.has(breadcrumbs?.lastItemLabel) ? breadcrumpsContext.get(breadcrumbs?.lastItemLabel) : breadcrumbs.lastItemLabel;
    const icon = breadcrumbs?.lastItemIcon;

    return [icon, label];
  }, [breadcrumbs?.lastItemLabel, breadcrumbs?.lastItemIcon, breadcrumpsContext]);

  return (
    <>
      <Stack direction="row" sx={{ alignItems: 'center', justifyContent: 'space-between', width: '100%', paddingLeft: 1, paddingRight: 1 }}>
        <Breadcrumbs maxItems={5}>
          {breadcrumpsLinks?.map(({ href, icon, label }, index) => (
            <Link component={NextLink} key={index} underline="hover" href={href}>
              {icon && <>{icon}</>}
              {!icon && label && <Typography>{label}</Typography>}
            </Link>
          ))}
          {lastBreadcrumps && lastBreadcrumps[0] && lastBreadcrumps[0]}
          {lastBreadcrumps && !lastBreadcrumps[0] && lastBreadcrumps[1] && <Typography>{lastBreadcrumps[1]}</Typography>}
        </Breadcrumbs>

        <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
          <Tooltip title="Send us feedback">
            <IconButton sx={{ ml: 1 }} onClick={() => setSubmitFeedbackDialogOpen(true)}>
              <FeedbackIcon />
            </IconButton>
          </Tooltip>

          {paletteMode === 'dark' && (
            <IconButton sx={{ ml: 1 }} onClick={() => updatePaletteMode('light')}>
              <LightModeIcon />
            </IconButton>
          )}

          {paletteMode === 'light' && (
            <IconButton sx={{ ml: 1 }} onClick={() => updatePaletteMode('dark')}>
              <DarkModeIcon />
            </IconButton>
          )}

          <IconButton onClick={handleProfileMenuOpenClick}>
            <CustomerAvatar
              name={{
                name: null,
                givenName: rootData.me?.givenName,
                middleName: rootData.me?.middleName,
                familyName: rootData.me?.familyName,
              }}
              photo={{
                url: rootData.me?.photoUrl,
              }}
            />
          </IconButton>

          <Menu
            sx={{ mt: 4 }}
            anchorEl={profileOpenAnchorEl}
            anchorOrigin={{
              vertical: 'top',
              horizontal: 'right',
            }}
            keepMounted
            transformOrigin={{
              vertical: 'top',
              horizontal: 'right',
            }}
            open={Boolean(profileOpenAnchorEl)}
            onClose={handleProfileMenuCloseClick}
          >
            <MenuItem>
              <Stack direction="column">
                <Stack direction="column">
                  <Typography variant="body1">Signed in as</Typography>
                  <Typography variant="body1">
                    {getCustomerFullName({
                      name: null,
                      givenName: rootData.me?.givenName,
                      middleName: rootData.me?.middleName,
                      familyName: rootData.me?.familyName,
                    })}
                  </Typography>
                  {rootData.me?.email && <Typography variant="body1">{rootData.me?.email.email}</Typography>}
                </Stack>
              </Stack>
            </MenuItem>

            <Divider />

            <MenuItem>
              <Link component={NextLink} href="/settings" color="inherit">
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                  <SettingsIcon fontSize="small" />
                  <Typography textAlign="center">Settings</Typography>
                </Stack>
              </Link>
            </MenuItem>

            <Divider />

            <MenuItem onClick={handleSignOutClick}>
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                <LogoutIcon fontSize="small" />
                <Typography textAlign="center">Sign out</Typography>
              </Stack>
            </MenuItem>
          </Menu>
        </Stack>
      </Stack>

      <NewFeedbackDialog
        rootDataRelay={rootData}
        isDialogOpen={submitFeedbackDialogOpen}
        onSendClicked={handleSubmitFeedbackSendClick}
        onCancelClicked={handleSubmitFeedbackCancelClick}
      />
    </>
  );
};

export default memo(AppBar);

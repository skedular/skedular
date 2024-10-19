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
import { FeedbackIcon, SettingsIcon } from '@repo/shared/components/icons';
import { BreadcrumpsContext, PaletteModeContext, UpdatePaletteModeContext } from '@repo/shared/libs/providers';
import { getCustomerFullName } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { NewFeedbackDialog } from 'components/feedback';
import { memo, useContext, useMemo, useState } from 'react';
import { useFragment } from 'react-relay';
import type { appBar_query$key } from './__generated__/appBar_query.graphql';

type Props = {
  rootDataRelay: appBar_query$key;
  onReloadRequired: () => void;
  breadcrumbs?: appBarBreadcrumbs;
};

type appBarBreadcrumbsItem = {
  label: string;
  href: string;
};

export type appBarBreadcrumbs = {
  items?: appBarBreadcrumbsItem[];
  lastItemLabel?: string;
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

  const handleSubmitFeedbackSendClick = () => {
    setSubmitFeedbackDialogOpen(false);
  };

  const handleSubmitFeedbackCancelClick = () => {
    setSubmitFeedbackDialogOpen(false);
  };

  const breadcrumpsLinks = useMemo<appBarBreadcrumbsItem[]>(() => {
    if (!breadcrumbs?.items) {
      return [];
    }

    return breadcrumbs.items.map((item) => {
      return breadcrumpsContext.has(item.href)
        ? {
            href: item.href,
            label: breadcrumpsContext.get(item.href)!,
          }
        : item;
    });
  }, [breadcrumbs?.items, breadcrumpsContext]);

  const lastBreadcrumpsLabel = useMemo<string | undefined>(() => {
    if (!breadcrumbs?.lastItemLabel) {
      return undefined;
    }

    return breadcrumpsContext.has(breadcrumbs?.lastItemLabel) ? breadcrumpsContext.get(breadcrumbs?.lastItemLabel) : breadcrumbs.lastItemLabel;
  }, [breadcrumbs?.lastItemLabel, breadcrumpsContext]);

  return (
    <>
      <Stack direction="row" sx={{ alignItems: 'center', justifyContent: 'space-between', width: '100%', paddingLeft: 1, paddingRight: 1 }}>
        <Breadcrumbs maxItems={5}>
          {breadcrumpsLinks?.map(({ href, label }, index) => (
            <Link key={index} underline="hover" href={href}>
              <Typography>{label}</Typography>
            </Link>
          ))}
          {lastBreadcrumpsLabel && <Typography>{lastBreadcrumpsLabel}</Typography>}
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
              <Link href="/settings" color="inherit">
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                  <SettingsIcon fontSize="small" />
                  <Typography textAlign="center">Settings</Typography>
                </Stack>
              </Link>
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

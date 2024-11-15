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
import { getCustomerFullName, localNow, toLongDateTime } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { NewFeedbackDialog } from 'components/feedback';
import { memo, useState } from 'react';
import { useFragment } from 'react-relay';
import { useInterval } from 'usehooks-ts';
import type { appBar_query$key } from './__generated__/appBar_query.graphql';

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

const AppBar = ({ rootDataRelay }: Props) => {
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

  const [currentTime, setCurrentTime] = useState(localNow());
  const [profileOpenAnchorEl, setProfileOpenAnchorEl] = useState<null | HTMLElement>(null);
  const [submitFeedbackDialogOpen, setSubmitFeedbackDialogOpen] = useState(false);

  useInterval(() => {
    setCurrentTime(localNow());
  }, 1000);

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

  const customerName = getCustomerFullName({
    name: null,
    givenName: rootData.me?.givenName,
    middleName: rootData.me?.middleName,
    familyName: rootData.me?.familyName,
  });

  return (
    <>
      <Stack
        direction="row"
        sx={{ alignItems: 'center', justifyContent: 'space-between', width: '100%', paddingLeft: 1, paddingRight: 1, flexWrap: 'wrap' }}
      >
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <Typography variant="h6" sx={{ display: { xs: 'none', sm: 'block' } }}>{`Welcome ${customerName}`}</Typography>
        </Stack>

        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <Typography variant="h6" sx={{ display: { xs: 'none', sm: 'block' } }}>{`${toLongDateTime(currentTime)}`}</Typography>
          <Divider orientation="vertical" flexItem />
          <Tooltip title="Send us feedback">
            <IconButton sx={{ ml: 1 }} onClick={() => setSubmitFeedbackDialogOpen(true)}>
              <FeedbackIcon />
            </IconButton>
          </Tooltip>

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
                  <Typography variant="body1">{customerName}</Typography>
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

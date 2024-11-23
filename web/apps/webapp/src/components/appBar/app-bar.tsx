import { NewFeedbackDialog } from '@/components/feedback';
import { getOrganizationAddLink, getOrganizationBaseLink } from '@/components/organization/organization-link';
import { SelectedOrganizationContext, UpdateSelectedOrganizationContext } from '@/libs/providers';
import type { appBar_query$key } from '@/queries/__generated__/appBar_query.graphql';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import Divider from '@mui/material/Divider';
import FormControl from '@mui/material/FormControl';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { CustomerAvatar, OrganizationAvatar } from '@repo/shared/components/avatars';
import { AddIcon, FeedbackIcon, LogoutIcon, NotificationsIcon, SettingsIcon } from '@repo/shared/components/icons';
import { PaletteModeContext, UpdatePaletteModeContext } from '@repo/shared/libs/providers';
import { getCustomerFullName, localNow, toLongDateTime } from '@repo/shared/libs/utils';
import { signOut } from 'next-auth/react';
import NextLink from 'next/link';
import { useParams, useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
import { useInterval } from 'usehooks-ts';

type Props = {
  rootDataRelay: appBar_query$key;
  onReloadRequired: () => void;
};

const createOrganizationId = '76eZvntIX6YA5FboBJlRk';

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
        myOrganizations {
          id
          logoUrl
          name
          canModify
          canViewAnalytics
        }
        ...newFeedbackDialog_query
      }
    `,
    rootDataRelay,
  );

  const router = useRouter();
  const { organizationId } = useParams();
  const [currentTime, setCurrentTime] = useState(localNow());
  const selectedOrganization = useContext(SelectedOrganizationContext);
  const updateSelectedOrganization = useContext(UpdateSelectedOrganizationContext);
  const paletteMode = useContext(PaletteModeContext);
  const updatePaletteMode = useContext(UpdatePaletteModeContext);
  const [profileOpenAnchorEl, setProfileOpenAnchorEl] = useState<null | HTMLElement>(null);
  const [submitFeedbackDialogOpen, setSubmitFeedbackDialogOpen] = useState(false);

  let finalOrganizationId = '';
  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] !== 'undefined') {
      finalOrganizationId = organizationId[0];
    }
  }

  const [selectedOrganizationId, setSelectedOrganizationId] = useState<string | undefined>(() => {
    if (finalOrganizationId && rootData.myOrganizations && rootData.myOrganizations.some((item) => item.id === finalOrganizationId)) {
      return finalOrganizationId;
    }

    if (selectedOrganization && rootData.myOrganizations && rootData.myOrganizations.some((item) => item.id === selectedOrganization)) {
      return selectedOrganization;
    }

    return rootData.myOrganizations && rootData.myOrganizations.length > 0 ? rootData.myOrganizations[0]?.id : undefined;
  });

  useInterval(() => setCurrentTime(localNow()), 1000);

  const handleSelectedOrganizationChange = (event: SelectChangeEvent) => {
    const id = event.target.value as string;

    if (id === createOrganizationId) {
      router.push(getOrganizationAddLink());
    } else {
      setSelectedOrganizationId(id);
      updateSelectedOrganization(id);

      router.push(getOrganizationBaseLink(id));
    }
  };

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

  const handleSubmitFeedbackClicked = () => {
    setProfileOpenAnchorEl(null);
    setSubmitFeedbackDialogOpen(true);
  };

  const handleSubmitFeedbackSendClick = () => {
    setSubmitFeedbackDialogOpen(false);
  };

  const handleSubmitFeedbackCancelClick = () => {
    setSubmitFeedbackDialogOpen(false);
  };

  const handleDarkThemeClicked = () => {
    updatePaletteMode('dark');
  };

  const handleLightThemeClicked = () => {
    updatePaletteMode('light');
  };

  if (!rootData.myOrganizations) {
    return <></>;
  }

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
          <>
            {rootData.myOrganizations.length > 0 && (
              <FormControl sx={{ width: { xs: '100%', sm: 300 } }}>
                <Select
                  value={selectedOrganizationId}
                  onChange={handleSelectedOrganizationChange}
                  sx={{
                    '& fieldset': {
                      border: 0,
                      borderRight: 1,
                      borderColor: 'divider',
                      borderRadius: 0,
                    },
                  }}
                >
                  {rootData.myOrganizations.map((organization) => (
                    <MenuItem key={organization.id} value={organization.id}>
                      <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                        <OrganizationAvatar name={{ name: organization.name }} photo={{ url: organization.logoUrl }} />
                        <Stack direction="column">
                          <Typography variant="h5">{organization.name}</Typography>
                          <Typography variant="body2">Organization</Typography>
                        </Stack>
                      </Stack>
                    </MenuItem>
                  ))}

                  <Divider />

                  <MenuItem value={createOrganizationId}>
                    <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                      <AddIcon fontSize="large" />
                      <Stack direction="column">
                        <Typography variant="h6">Create Organization</Typography>
                      </Stack>
                    </Stack>
                  </MenuItem>
                </Select>
              </FormControl>
            )}
          </>

          <Typography variant="h6" sx={{ display: { xs: 'none', sm: 'block' } }}>{`Welcome ${customerName}`}</Typography>
        </Stack>

        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <Typography variant="h6" sx={{ display: { xs: 'none', sm: 'block' } }}>
            {`${toLongDateTime(currentTime)}`}
          </Typography>
          <Divider orientation="vertical" flexItem />

          <IconButton sx={{ ml: 1 }}>
            <NotificationsIcon />
          </IconButton>

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
                  <Typography variant="h6">{customerName}</Typography>
                  {rootData.me?.email && <Typography variant="body1">{rootData.me?.email.email}</Typography>}
                </Stack>
              </Stack>
            </MenuItem>

            <Divider />

            <MenuItem>
              <Link component={NextLink} href="/settings" color="inherit">
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                  <SettingsIcon fontSize="medium" />
                  <Typography textAlign="center">Settings</Typography>
                </Stack>
              </Link>
            </MenuItem>

            {paletteMode === 'dark' && (
              <MenuItem onClick={handleLightThemeClicked}>
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                  <LightModeIcon fontSize="medium" />
                  <Typography textAlign="center">Dark Mode</Typography>
                </Stack>
              </MenuItem>
            )}

            {paletteMode === 'light' && (
              <MenuItem onClick={handleDarkThemeClicked}>
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                  <DarkModeIcon fontSize="medium" />
                  <Typography textAlign="center">Light Mode</Typography>
                </Stack>
              </MenuItem>
            )}

            <Divider />

            <MenuItem onClick={handleSubmitFeedbackClicked}>
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                <FeedbackIcon fontSize="medium" />
                <Typography textAlign="center">Send us feedback</Typography>
              </Stack>
            </MenuItem>

            <Divider />

            <MenuItem onClick={handleSignOutClick}>
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                <LogoutIcon fontSize="medium" />
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

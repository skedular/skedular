import { CustomerAvatar } from '@/components/customer';
import { NewFeedbackDialog } from '@/components/feedback';
import type { mainRootLayout_query$key } from '@/queries/__generated__/mainRootLayout_query.graphql';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import MuiAppBar, { AppBarProps as MuiAppBarProps } from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import CssBaseline from '@mui/material/CssBaseline';
import Divider from '@mui/material/Divider';
import Drawer from '@mui/material/Drawer';
import IconButton from '@mui/material/IconButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Stack from '@mui/material/Stack';
import Toolbar from '@mui/material/Toolbar';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { styled, useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import { FeedbackIcon, LogoutIcon, MenuIcon, SettingsIcon } from '@repo/shared/components/icons';
import { Logo } from '@repo/shared/components/logo';
import { ColorModeContext } from '@repo/shared/libs/providers';
import { getCustomerFullName } from '@repo/shared/libs/utils';
import { signOut } from 'next-auth/react';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

const drawerWidth = 250;

const Main = styled('div', { shouldForwardProp: (prop) => prop !== 'open' })<{
  open?: boolean;
}>(({ theme, open }) => ({
  flexGrow: 1,
  padding: theme.spacing(3),
  transition: theme.transitions.create('margin', {
    easing: theme.transitions.easing.sharp,
    duration: theme.transitions.duration.leavingScreen,
  }),
  marginLeft: `-${drawerWidth}px`,
  ...(open && {
    transition: theme.transitions.create('margin', {
      easing: theme.transitions.easing.easeOut,
      duration: theme.transitions.duration.enteringScreen,
    }),
    marginLeft: 0,
  }),
}));

interface AppBarProps extends MuiAppBarProps {
  open?: boolean;
}

const AppBar = styled(MuiAppBar, {
  shouldForwardProp: (prop) => prop !== 'open',
})<AppBarProps>(({ theme, open }) => ({
  transition: theme.transitions.create(['margin', 'width'], {
    easing: theme.transitions.easing.sharp,
    duration: theme.transitions.duration.leavingScreen,
  }),
  ...(open && {
    width: `calc(100% - ${drawerWidth}px)`,
    marginLeft: `${drawerWidth}px`,
    transition: theme.transitions.create(['margin', 'width'], {
      easing: theme.transitions.easing.easeOut,
      duration: theme.transitions.duration.enteringScreen,
    }),
  }),
}));

const DrawerHeader = styled('div')(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  padding: theme.spacing(0, 1),
  // necessary for content to be below app bar
  ...theme.mixins.toolbar,
  justifyContent: 'flex-end',
}));

type Props = {
  rootDataRelay: mainRootLayout_query$key;
  children: React.ReactNode;
  leftSideContent: React.JSX.Element;
  rightSideContent?: React.JSX.Element;
};

const MainRootLayout = ({ rootDataRelay, children, leftSideContent, rightSideContent }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment mainRootLayout_query on Query {
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

  const theme = useTheme();
  const colorMode = useContext(ColorModeContext);
  const matchMobileView = useMediaQuery(theme.breakpoints.down('sm'));
  const router = useRouter();
  const [leftDraweropen, setLeftDrawerOpen] = useState(!matchMobileView);
  const [rightDraweropen, setRightDrawerOpen] = useState(false);
  const [anchorElNav, setAnchorElNav] = useState<null | HTMLElement>(null);
  const [profileOpenAnchorEl, setProfileOpenAnchorEl] = useState<null | HTMLElement>(null);

  const [submitFeedbackDialogOpen, setSubmitFeedbackDialogOpen] = useState(false);

  const handleLeftDrawerOpen = () => {
    setLeftDrawerOpen(true);
  };

  const handleLeftDrawerClose = () => {
    setLeftDrawerOpen(false);
  };

  const handleRightDrawerOpen = () => {
    setRightDrawerOpen(true);
  };

  const handleRightDrawerClose = () => {
    setRightDrawerOpen(false);
  };

  const handleCloseLeftDrawer = () => {
    setAnchorElNav(null);
  };

  const handleProfileMenuOpenClick = (event: React.MouseEvent<HTMLElement>) => {
    setProfileOpenAnchorEl(event.currentTarget);
  };

  const handleProfileMenuCloseClick = () => {
    setProfileOpenAnchorEl(null);
  };

  const handleSettingsClick = () => {
    setProfileOpenAnchorEl(null);
    router.push('/settings');
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

  const email = rootData.me?.email;

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <CssBaseline />
        <AppBar position="fixed" open={leftDraweropen} sx={{ background: theme.palette.background.paper }}>
          <Toolbar>
            <IconButton
              aria-label="open drawer"
              onClick={handleLeftDrawerOpen}
              edge="start"
              sx={{ mr: 2, ...(leftDraweropen && { display: 'none' }) }}
            >
              <MenuIcon />
            </IconButton>
            {!leftDraweropen && <Logo />}
            <Box sx={{ flex: 1 }}>
              <Menu
                id="menu-appbar"
                anchorEl={anchorElNav}
                anchorOrigin={{
                  vertical: 'bottom',
                  horizontal: 'left',
                }}
                keepMounted
                transformOrigin={{
                  vertical: 'top',
                  horizontal: 'left',
                }}
                open={Boolean(anchorElNav)}
                onClose={handleCloseLeftDrawer}
                sx={{
                  display: { xs: 'block', md: 'none' },
                }}
              />
            </Box>

            <Box>
              <Tooltip title="Send us feedback">
                <IconButton sx={{ ml: 1 }} onClick={() => setSubmitFeedbackDialogOpen(true)}>
                  <FeedbackIcon />
                </IconButton>
              </Tooltip>
              <IconButton sx={{ ml: 1 }} onClick={colorMode.toggleColorMode}>
                {theme.palette.mode === 'dark' ? <LightModeIcon /> : <DarkModeIcon />}
              </IconButton>
              <IconButton onClick={handleProfileMenuOpenClick} sx={{ p: 0 }}>
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

              {rightSideContent && (
                <IconButton
                  aria-label="open drawer"
                  onClick={handleRightDrawerOpen}
                  edge="end"
                  sx={{ mr: 2, ...(rightDraweropen && { display: 'none' }) }}
                >
                  <MenuIcon />
                </IconButton>
              )}

              <Menu
                sx={{ mt: '45px' }}
                id="menu-appbar"
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
                <MenuItem onClick={handleProfileMenuCloseClick}>
                  <Stack spacing={2}>
                    <ListItemText
                      primary={
                        <Typography variant="body1" style={{ color: 'primary' }}>
                          Signed in as
                        </Typography>
                      }
                    />
                    <ListItemText
                      primary={
                        <Typography variant="body2" style={{ color: 'primary' }}>
                          {getCustomerFullName({
                            name: null,
                            givenName: rootData.me?.givenName,
                            middleName: rootData.me?.middleName,
                            familyName: rootData.me?.familyName,
                          })}
                        </Typography>
                      }
                    />

                    {email && (
                      <ListItemText
                        primary={
                          <Typography variant="body2" style={{ color: 'primary' }}>
                            {email.email}
                          </Typography>
                        }
                      />
                    )}
                  </Stack>
                </MenuItem>

                <Divider />

                <MenuItem onClick={handleSettingsClick}>
                  <ListItemIcon>
                    <SettingsIcon fontSize="small" />
                  </ListItemIcon>
                  <Typography textAlign="center">Settings</Typography>
                </MenuItem>

                <Divider />

                <MenuItem onClick={handleSignOutClick}>
                  <ListItemIcon>
                    <LogoutIcon fontSize="small" />
                  </ListItemIcon>
                  <Typography textAlign="center">Sign out</Typography>
                </MenuItem>
              </Menu>
            </Box>
          </Toolbar>
        </AppBar>

        <Drawer
          sx={{
            width: drawerWidth,
            flexShrink: 0,
            '& .MuiDrawer-paper': {
              width: drawerWidth,
              boxSizing: 'border-box',
            },
          }}
          variant="persistent"
          anchor="left"
          open={leftDraweropen}
        >
          <DrawerHeader>
            <Box sx={{ flex: 1 }}>
              <Logo />
            </Box>
            <IconButton onClick={handleLeftDrawerClose}>
              <ChevronLeftIcon />
            </IconButton>
          </DrawerHeader>
          <Divider />
          {leftSideContent}
        </Drawer>

        {rightSideContent && (
          <Drawer
            sx={{
              width: drawerWidth,
              flexShrink: 0,
              '& .MuiDrawer-paper': {
                width: drawerWidth,
                boxSizing: 'border-box',
              },
            }}
            variant="persistent"
            anchor="right"
            open={rightDraweropen}
          >
            <DrawerHeader>
              <IconButton onClick={handleRightDrawerClose}>
                <ChevronRightIcon />
              </IconButton>
            </DrawerHeader>
            <Divider />
            {rightSideContent}
          </Drawer>
        )}
        <Main open={leftDraweropen}>{children}</Main>
      </Box>
      <NewFeedbackDialog
        rootDataRelay={rootData}
        isDialogOpen={submitFeedbackDialogOpen}
        onSendClicked={handleSubmitFeedbackSendClick}
        onCancelClicked={handleSubmitFeedbackCancelClick}
      />
    </>
  );
};

export default memo(MainRootLayout);

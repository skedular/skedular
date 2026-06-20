'use client';

import { createHostAppSwitcherModel } from '@/app/app-switcher-config';
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import AddBusinessIcon from '@mui/icons-material/AddBusiness';
import ApartmentIcon from '@mui/icons-material/Apartment';
import BookOnlineIcon from '@mui/icons-material/BookOnline';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import DashboardIcon from '@mui/icons-material/Dashboard';
import LightModeIcon from '@mui/icons-material/LightMode';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import MenuIcon from '@mui/icons-material/Menu';
import PaymentsIcon from '@mui/icons-material/Payments';
import SettingsIcon from '@mui/icons-material/Settings';
import AppBar from '@mui/material/AppBar';
import Avatar from '@mui/material/Avatar';
import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import Drawer from '@mui/material/Drawer';
import IconButton from '@mui/material/IconButton';
import List from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Select from '@mui/material/Select';
import Toolbar from '@mui/material/Toolbar';
import { PaletteModeContext, SelectedPaletteModeContext, UpdatePaletteModeContext } from '@skedular/shared';
import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, SmallIconTypography } from '@skedular/ui';
import AppSwitcher from '@skedular/ui/app-shell/app-switcher';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import Link from 'next/link';
import { usePathname, useRouter } from 'next/navigation';
import { useContext, useMemo, useState, type PropsWithChildren } from 'react';

const drawerWidth = 248;

const navigation = [
  { label: 'Overview', href: '/dashboard', icon: DashboardIcon },
  { label: 'Locations', href: '/locations', icon: LocationOnIcon },
  { label: 'Bookings', href: '/bookings', icon: BookOnlineIcon },
  { label: 'Payments', href: '/commissions', icon: PaymentsIcon },
  { label: 'Organization', href: '/organization', icon: ApartmentIcon },
  { label: 'Settings', href: '/settings', icon: SettingsIcon },
] as const;

const Navigation = ({ close }: { close?: () => void }) => {
  const pathname = usePathname();

  return (
    <Box sx={{ height: '100%', bgcolor: 'background.paper' }}>
      <Toolbar sx={{ px: 2.5 }}>
        <LeadIconTypography label="Skedular Host" />
      </Toolbar>
      <Divider />
      <List sx={{ p: 1.5 }}>
        {navigation.map(({ label, href, icon: Icon }) => {
          const active = pathname === href || pathname.startsWith(`${href}/`);

          return (
            <ListItemButton key={href} component={Link} href={href} selected={active} onClick={close} sx={{ borderRadius: 2, mb: 0.5 }}>
              <ListItemIcon sx={{ minWidth: 40 }}>
                <Icon fontSize="small" />
              </ListItemIcon>
              <ListItemText primary={label} />
            </ListItemButton>
          );
        })}
      </List>
    </Box>
  );
};

const DashboardLayout = ({ children }: PropsWithChildren) => {
  const router = useRouter();
  const { user, signOut } = useAuth();
  const paletteMode = useContext(PaletteModeContext);
  const selectedPaletteMode = useContext(SelectedPaletteModeContext);
  const updatePaletteMode = useContext(UpdatePaletteModeContext);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [profileAnchor, setProfileAnchor] = useState<HTMLElement | null>(null);
  const [themeAnchor, setThemeAnchor] = useState<HTMLElement | null>(null);
  const appSwitcher = useMemo(() => createHostAppSwitcherModel({ logConfiguration: false }), []);
  const displayName = user?.firstName || user?.email || 'Host';

  const selectTheme = (mode: 'light' | 'dark' | 'system') => {
    updatePaletteMode(mode);
    setThemeAnchor(null);
  };

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh', bgcolor: 'background.default' }}>
      <Box component="nav" sx={{ width: { md: drawerWidth }, flexShrink: { md: 0 } }} aria-label="Host workspace navigation">
        <Drawer
          variant="temporary"
          open={drawerOpen}
          onClose={() => setDrawerOpen(false)}
          ModalProps={{ keepMounted: true }}
          sx={{ display: { xs: 'block', md: 'none' }, '& .MuiDrawer-paper': { width: drawerWidth } }}
        >
          <Navigation close={() => setDrawerOpen(false)} />
        </Drawer>
        <Drawer variant="permanent" open sx={{ display: { xs: 'none', md: 'block' }, '& .MuiDrawer-paper': { width: drawerWidth, boxSizing: 'border-box' } }}>
          <Navigation />
        </Drawer>
      </Box>

      <Box sx={{ flexGrow: 1, minWidth: 0 }}>
        <AppBar position="sticky" elevation={0} sx={{ bgcolor: 'background.paper', color: 'text.primary', borderBottom: 1, borderColor: 'divider' }}>
          <Toolbar>
            <IconButton aria-label="Open navigation" onClick={() => setDrawerOpen(true)} sx={{ display: { md: 'none' }, mr: 1 }}>
              <MenuIcon />
            </IconButton>

            <Select
              displayEmpty
              defaultValue="host"
              onChange={(event) => {
                if (event.target.value === 'create') router.push('/onboarding');
              }}
              sx={{ maxWidth: { xs: 190, sm: 300 }, '& fieldset': { border: 0 } }}
            >
              <MenuItem value="host">
                <BodyIconTypography label="Host organization" startElement={<ApartmentIcon fontSize="small" />} />
              </MenuItem>
              <Divider />
              <MenuItem value="create">
                <BodyIconTypography label="Create organization" startElement={<AddBusinessIcon fontSize="small" />} />
              </MenuItem>
            </Select>

            <Box sx={{ flexGrow: 1 }} />
            <BodyIconTypography label={`Welcome ${displayName}`} sx={{ display: { xs: 'none', lg: 'block' }, mr: 2 }} />
            <IconButton aria-label="Choose theme" onClick={(event) => setThemeAnchor(event.currentTarget)} sx={{ display: { xs: 'none', sm: 'inline-flex' }, mr: 0.5 }}>
              {paletteMode === 'dark' ? <DarkModeIcon /> : <LightModeIcon />}
            </IconButton>
            <Menu anchorEl={themeAnchor} open={Boolean(themeAnchor)} onClose={() => setThemeAnchor(null)}>
              {(['light', 'dark', 'system'] as const).map((mode) => (
                <MenuItem key={mode} selected={selectedPaletteMode === mode} onClick={() => selectTheme(mode)}>
                  {mode[0].toUpperCase() + mode.slice(1)}
                </MenuItem>
              ))}
            </Menu>

            <IconButton aria-label="Open profile menu" onClick={(event) => setProfileAnchor(event.currentTarget)}>
              <Avatar sx={{ width: 34, height: 34 }} src={user?.profilePictureUrl ?? undefined}>
                <AccountCircleIcon />
              </Avatar>
            </IconButton>
            <Menu anchorEl={profileAnchor} open={Boolean(profileAnchor)} onClose={() => setProfileAnchor(null)} anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}>
              <MenuItem disabled>
                <Box>
                  <LeadIconTypography label={displayName} />
                  <CaptionIconTypography label={user?.email} />
                </Box>
              </MenuItem>
              <Divider />
              <AppSwitcher model={appSwitcher} buttonMode="menu-item" />
              <Divider />
              <MenuItem component={Link} href="/organization" onClick={() => setProfileAnchor(null)}>
                <SmallIconTypography label="Organization settings" startElement={<SettingsIcon fontSize="small" />} />
              </MenuItem>
              <MenuItem
                onClick={async () => {
                  setProfileAnchor(null);
                  await signOut({ returnTo: '/' });
                }}
              >
                Sign out
              </MenuItem>
            </Menu>
          </Toolbar>
        </AppBar>

        <Box component="main" sx={{ px: { xs: 2, sm: 3, lg: 4 }, py: { xs: 2, md: 3 } }}>
          {children}
        </Box>
      </Box>
    </Box>
  );
};

export default DashboardLayout;

import { BodyIconTypography, PushToRight } from '@skedular/ui';
import { HamburgerMenuIcon, SystemModeIcon } from '@/components/icons';
import { getSignInLink, getSignUpLink } from '@/components/links';
import { UnauthenticatedMobileLeftSideNavigationMenu } from '@/components/navigationMenu';
import { PaletteModeContext, SelectedPaletteModeContext, UpdatePaletteModeContext } from '@skedular/shared';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import MuiAppBar from '@mui/material/AppBar';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Toolbar from '@mui/material/Toolbar';
import Box from '@mui/system/Box';
import Image from 'next/image';
import { memo, useContext, useState } from 'react';

const UnauthenticatedAppBar = () => {
  const selectedThemeMode = useContext(SelectedPaletteModeContext);
  const paletteMode = useContext(PaletteModeContext);
  const updatePaletteMode = useContext(UpdatePaletteModeContext);
  const [mobileDrawerOpen, setMobileDrawerOpen] = useState(false);
  const [themeMenuAnchorEl, setThemeMenuAnchorEl] = useState<null | HTMLElement>(null);

  const logoUrl = paletteMode === 'dark' ? '/images/skedular-logo-inverse.svg' : '/images/skedular-logo-primary.svg';
  const originalWidth = 779;
  const originalHeight = 163;
  const maxWidth = 300;
  const widthPercentage = ((maxWidth - 70) * 100) / originalWidth;
  const heightPercentage = ((maxWidth - 30) * 100) / originalWidth;
  const width = (originalWidth * widthPercentage) / 100;
  const height = (originalHeight * heightPercentage) / 100;

  const signInLink = getSignInLink();
  const signUpLink = getSignUpLink();

  const handleThemeMenuOpenClick = (event: React.MouseEvent<HTMLElement>) => {
    setThemeMenuAnchorEl(event.currentTarget);
  };

  const handleThemeMenuCloseClick = () => {
    setThemeMenuAnchorEl(null);
  };

  const handleThemeModeSelected = (mode: 'light' | 'dark' | 'system') => {
    updatePaletteMode(mode);
    handleThemeMenuCloseClick();
  };

  const toggleMobileDrawerOpen = (newOpen: boolean) => () => {
    setMobileDrawerOpen(newOpen);
  };
  const selectedThemeIcon =
    selectedThemeMode === 'light' ? <LightModeIcon fontSize="small" /> : selectedThemeMode === 'dark' ? <DarkModeIcon fontSize="small" /> : <SystemModeIcon fontSize="small" />;

  return (
    <MuiAppBar position="sticky" className="app-bar">
      <Toolbar
        sx={{
          backgroundColor: (theme) => theme.palette.background.paper,
          borderBottom: paletteMode === 'dark' ? 1 : undefined,
          borderColor: (theme) => theme.palette.divider,
        }}
      >
        <Image src={logoUrl} width={width} height={height} unoptimized alt="Skedular" />

        <PushToRight />
        <IconButton
          onClick={handleThemeMenuOpenClick}
          sx={{
            border: 1,
            borderColor: (theme) => theme.palette.divider,
            borderRadius: 3,
            width: 40,
            height: 40,
            color: (theme) => theme.palette.text.primary,
            '&:hover': {
              backgroundColor: (theme) => theme.palette.action.hover,
            },
          }}
        >
          {selectedThemeIcon}
        </IconButton>

        <Menu
          anchorEl={themeMenuAnchorEl}
          open={Boolean(themeMenuAnchorEl)}
          onClose={handleThemeMenuCloseClick}
          anchorOrigin={{
            vertical: 'bottom',
            horizontal: 'right',
          }}
          transformOrigin={{
            vertical: 'top',
            horizontal: 'right',
          }}
          sx={{ mt: 1 }}
        >
          <MenuItem selected={selectedThemeMode === 'light'} onClick={() => handleThemeModeSelected('light')}>
            <BodyIconTypography startElement={<LightModeIcon fontSize="small" />} label="Light" spacing={2} />
          </MenuItem>
          <MenuItem selected={selectedThemeMode === 'dark'} onClick={() => handleThemeModeSelected('dark')}>
            <BodyIconTypography startElement={<DarkModeIcon fontSize="small" />} label="Dark" spacing={2} />
          </MenuItem>
          <MenuItem selected={selectedThemeMode === 'system'} onClick={() => handleThemeModeSelected('system')}>
            <BodyIconTypography startElement={<SystemModeIcon fontSize="small" />} label="System" spacing={2} />
          </MenuItem>
        </Menu>

        <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
          <IconButton component="a" href={signInLink}>
            <Button component="span" variant="contained" fullWidth sx={{ textTransform: 'none' }}>
              <BodyIconTypography label="Sign In" />
            </Button>
          </IconButton>
        </Box>

        <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
          <IconButton component="a" href={signUpLink}>
            <Button component="span" variant="contained" fullWidth sx={{ textTransform: 'none' }} color="secondary">
              <BodyIconTypography label="Sign Up" invertDefaultColor={paletteMode === 'dark'} />
            </Button>
          </IconButton>
        </Box>

        <IconButton onClick={toggleMobileDrawerOpen(true)} sx={{ display: { xs: 'block', sm: 'none' } }}>
          <HamburgerMenuIcon />
        </IconButton>

        <UnauthenticatedMobileLeftSideNavigationMenu open={mobileDrawerOpen} toggleDrawer={toggleMobileDrawerOpen} />
      </Toolbar>
    </MuiAppBar>
  );
};

export default memo(UnauthenticatedAppBar);

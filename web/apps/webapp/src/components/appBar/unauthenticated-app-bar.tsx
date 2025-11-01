import { BodyIconTypography, PushToRight } from '@/components/commons';
import { HamburgerMenuIcon } from '@/components/icons';
import { UnauthenticatedMobileLeftSideNavigationMenu } from '@/components/navigationMenu';
import { PaletteModeContext, UpdatePaletteModeContext } from '@/libs/providers';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import MuiAppBar from '@mui/material/AppBar';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import Toolbar from '@mui/material/Toolbar';
import Box from '@mui/system/Box';
import Image from 'next/image';
import type { JSX } from 'react';
import { memo, useContext, useState } from 'react';

type Props = {
  showBreadcrumps?: boolean;
  breadcrumbs?: React.ReactNode | JSX.Element;
};

const UnauthenticatedAppBar = ({ showBreadcrumps, breadcrumbs }: Props) => {
  const paletteMode = useContext(PaletteModeContext);
  const updatePaletteMode = useContext(UpdatePaletteModeContext);
  const [mobileDrawerOpen, setMobileDrawerOpen] = useState(false);

  const logoUrl = paletteMode === 'dark' ? '/images/skedular-logo-inverse.svg' : '/images/skedular-logo-primary.svg';
  const originalWidth = 779;
  const originalHeight = 163;
  const maxWidth = 300;
  const widthPercentage = ((maxWidth - 70) * 100) / originalWidth;
  const heightPercentage = ((maxWidth - 30) * 100) / originalWidth;
  const width = (originalWidth * widthPercentage) / 100;
  const height = (originalHeight * heightPercentage) / 100;

  const handleDarkThemeClicked = () => {
    updatePaletteMode('dark');
  };

  const handleLightThemeClicked = () => {
    updatePaletteMode('light');
  };

  const toggleMobileDrawerOpen = (newOpen: boolean) => () => {
    setMobileDrawerOpen(newOpen);
  };

  return (
    <MuiAppBar position="sticky" className="app-bar">
      <Toolbar
        sx={{
          backgroundColor: (theme) => theme.palette.background.paper,
          borderBottom: paletteMode === 'dark' ? 1 : undefined,
          borderColor: (theme) => theme.palette.divider,
        }}
      >
        <Image src={logoUrl} width={width} height={height} alt="Skedular" />
        {showBreadcrumps && <>{breadcrumbs}</>}

        <PushToRight />
        {paletteMode === 'dark' && (
          <IconButton onClick={handleLightThemeClicked}>
            <DarkModeIcon />
          </IconButton>
        )}

        {paletteMode === 'light' && (
          <IconButton onClick={handleDarkThemeClicked}>
            <LightModeIcon />
          </IconButton>
        )}

        <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
          <IconButton component="a" href="/signin">
            <Button component="span" variant="contained" fullWidth sx={{ textTransform: 'none' }}>
              <BodyIconTypography label="Sign In" />
            </Button>
          </IconButton>
        </Box>

        <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
          <IconButton component="a" href="/signup">
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

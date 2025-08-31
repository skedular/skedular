import { getSignInUrlAction } from '@/components/authActions';
import { BodyIconTypography, PushToRight } from '@/components/commons';
import { PaletteModeContext, UpdatePaletteModeContext } from '@/libs/providers';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import MuiAppBar from '@mui/material/AppBar';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import Toolbar from '@mui/material/Toolbar';
import Image from 'next/image';
import { useRouter } from 'next/navigation';
import type { JSX } from 'react';
import { memo, useContext, useEffect, useState } from 'react';

type Props = {
  showBreadcrumps?: boolean;
  breadcrumbs?: React.ReactNode | JSX.Element;
};

const UnauthenticatedAppBar = ({ showBreadcrumps, breadcrumbs }: Props) => {
  const paletteMode = useContext(PaletteModeContext);
  const updatePaletteMode = useContext(UpdatePaletteModeContext);
  const [signInUrl, setSignInUrl] = useState('');
  const router = useRouter();

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

  const handleSignIn = () => {
    router.push(signInUrl);
  };

  useEffect(() => {
    async function loadSignInUrl() {
      setSignInUrl(await getSignInUrlAction());
    }

    loadSignInUrl();
  }, []);

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

        <IconButton onClick={handleSignIn}>
          <Button variant={'contained'} fullWidth sx={{ textTransform: 'none' }}>
            <BodyIconTypography label={'Sign In'} />
          </Button>
        </IconButton>
      </Toolbar>
    </MuiAppBar>
  );
};

export default memo(UnauthenticatedAppBar);

import { BodyIconTypography, LeadIconTypography, PushToRight } from '@/components/commons';
import { HamburgerMenuIcon } from '@/components/icons';
import { getSignInLink, getSignUpLink } from '@/components/links';
import { UnauthenticatedMobileLeftSideNavigationMenu } from '@/components/navigationMenu';
import { PaletteModeContext, UpdatePaletteModeContext } from '@/libs/providers';
import type { unauthenticatedOrganizationStoreFrontAppBar_query$key } from '@/queries/__generated__/unauthenticatedOrganizationStoreFrontAppBar_query.graphql';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import MuiAppBar from '@mui/material/AppBar';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import Toolbar from '@mui/material/Toolbar';
import Box from '@mui/system/Box';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: unauthenticatedOrganizationStoreFrontAppBar_query$key;
};

const UnauthenticatedOrganizationStoreFrontAppBar = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<unauthenticatedOrganizationStoreFrontAppBar_query$key>(
    graphql`
      fragment unauthenticatedOrganizationStoreFrontAppBar_query on Query {
        organizationPublic(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
          name
        }
      }
    `,
    rootDataRelay,
  );

  const paletteMode = useContext(PaletteModeContext);
  const updatePaletteMode = useContext(UpdatePaletteModeContext);
  const [mobileDrawerOpen, setMobileDrawerOpen] = useState(false);

  const signInLink = getSignInLink();
  const signUpLink = getSignUpLink();

  const handleDarkThemeClicked = () => {
    updatePaletteMode('dark');
  };

  const handleLightThemeClicked = () => {
    updatePaletteMode('light');
  };

  const toggleMobileDrawerOpen = (newOpen: boolean) => () => {
    setMobileDrawerOpen(newOpen);
  };

  if (!rootData.organizationPublic) {
    return null;
  }

  return (
    <MuiAppBar position="sticky" className="app-bar">
      <Toolbar
        sx={{
          backgroundColor: (theme) => theme.palette.background.paper,
          borderBottom: paletteMode === 'dark' ? 1 : undefined,
          borderColor: (theme) => theme.palette.divider,
        }}
      >
        <LeadIconTypography label={rootData.organizationPublic?.name} />

        <PushToRight />
        {paletteMode === 'dark' && (
          <IconButton onClick={handleLightThemeClicked}>
            <LightModeIcon />
          </IconButton>
        )}

        {paletteMode === 'light' && (
          <IconButton onClick={handleDarkThemeClicked}>
            <DarkModeIcon />
          </IconButton>
        )}

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

export default memo(UnauthenticatedOrganizationStoreFrontAppBar);

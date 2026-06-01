import { SystemModeIcon } from '@/components/icons';
import { getSignInLink, getSignUpLink } from '@/components/links';
import type { unauthenticatedOrganizationStoreFrontAppBar_query$key } from '@/queries/__generated__/unauthenticatedOrganizationStoreFrontAppBar_query.graphql';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import MuiAppBar from '@mui/material/AppBar';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import IconButton from '@mui/material/IconButton';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Toolbar from '@mui/material/Toolbar';
import Box from '@mui/system/Box';
import { SelectedPaletteModeContext, UpdatePaletteModeContext } from '@skedular/shared';
import { BodyIconTypography, LeadIconTypography, PushToRight } from '@skedular/ui';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: unauthenticatedOrganizationStoreFrontAppBar_query$key;
};

const UnauthenticatedOrganizationStoreFrontAppBar = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<unauthenticatedOrganizationStoreFrontAppBar_query$key>(
    graphql`
      fragment unauthenticatedOrganizationStoreFrontAppBar_query on Query {
        organizationPublic(customDomain: $organizationCustomDomain) {
          name
        }
      }
    `,
    rootDataRelay,
  );

  const selectedThemeMode = useContext(SelectedPaletteModeContext);
  const updatePaletteMode = useContext(UpdatePaletteModeContext);
  const [themeMenuAnchorEl, setThemeMenuAnchorEl] = useState<null | HTMLElement>(null);

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

  const handleOrganizationHomeClick = () => {
    window.location.href = window.location.origin;
  };

  const selectedThemeIcon =
    selectedThemeMode === 'light' ? <LightModeIcon fontSize="small" /> : selectedThemeMode === 'dark' ? <DarkModeIcon fontSize="small" /> : <SystemModeIcon fontSize="small" />;

  if (!rootData.organizationPublic) {
    return null;
  }

  return (
    <MuiAppBar
      position="sticky"
      className="app-bar"
      elevation={0}
      sx={{
        backgroundColor: (theme) => theme.palette.background.default,
        backdropFilter: 'blur(10px)',
        borderBottom: 1,
        borderColor: (theme) => theme.palette.divider,
      }}
    >
      <Container maxWidth="xl">
        <Toolbar
          disableGutters
          sx={{
            minHeight: 'unset !important',
            py: 2.5,
          }}
        >
          <Box
            onClick={handleOrganizationHomeClick}
            sx={{
              cursor: 'pointer',
              borderRadius: 2,
              px: 0.5,
              py: 0.25,
              ml: -0.5,
              transition: 'background-color 120ms ease',
              '&:hover': {
                backgroundColor: (theme) => theme.palette.action.hover,
              },
            }}
          >
            <LeadIconTypography
              label={rootData.organizationPublic?.name}
              fontWeight={600}
              sx={{
                letterSpacing: '-0.03em',
                fontSize: {
                  xs: '1.25rem',
                  sm: '1.5rem',
                },
              }}
            />
          </Box>

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

          <Box sx={{ display: 'flex', alignItems: 'center', gap: { xs: 1, sm: 1.5 }, ml: { xs: 1, sm: 2 } }}>
            <Button
              component="a"
              href={signInLink}
              variant="outlined"
              sx={{
                textTransform: 'none',
                borderRadius: '24px',
                px: { xs: 1.5, sm: 3 },
                py: 1,
                borderColor: (theme) => theme.palette.divider,
                color: (theme) => theme.palette.text.primary,
                backgroundColor: (theme) => theme.palette.background.paper,
                fontWeight: 500,
                fontSize: '0.9375rem',
                '&:hover': {
                  borderColor: (theme) => theme.palette.divider,
                  backgroundColor: (theme) => theme.palette.action.hover,
                },
              }}
            >
              Sign in
            </Button>

            <Button
              component="a"
              href={signUpLink}
              variant="contained"
              color="secondary"
              sx={{
                textTransform: 'none',
                borderRadius: '24px',
                display: { xs: 'none', sm: 'inline-flex' },
                px: { xs: 1.5, sm: 3 },
                py: 1,
                fontWeight: 500,
                fontSize: '0.9375rem',
              }}
            >
              Sign up
            </Button>
          </Box>
        </Toolbar>
      </Container>
    </MuiAppBar>
  );
};

export default memo(UnauthenticatedOrganizationStoreFrontAppBar);

import { Navigation } from '@/components/navigation';
import CloseIcon from '@mui/icons-material/Close';
import ExitToAppIcon from '@mui/icons-material/ExitToApp';
import LoginIcon from '@mui/icons-material/Login';
import AppBar, { AppBarProps as MuiAppBarProps } from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import { styled, useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import { MenuIcon } from '@repo/shared/components/icons';
import { SlackIconButton } from '@repo/shared/components/slackButtons';
import NextLink from 'next/link';
import { memo, useState } from 'react';

const StyledAppBar = styled(AppBar)<MuiAppBarProps>(({ theme }) => ({
  transition: theme.transitions.create(['margin', 'width'], {
    easing: theme.transitions.easing.sharp,
    duration: theme.transitions.duration.leavingScreen,
  }),
  background: theme.palette.background.paper,
}));

const Header = () => {
  const [visibleMenu, setVisibleMenu] = useState<boolean>(false);
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

  return (
    <StyledAppBar position="fixed">
      <Container>
        <Box
          sx={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
          }}
        >
          <Box sx={{ ml: 'auto', display: { xs: 'inline-flex', md: 'none' } }}>
            <IconButton onClick={() => setVisibleMenu(!visibleMenu)}>
              <MenuIcon />
            </IconButton>
          </Box>
          <Box
            sx={{
              width: '100%',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'space-between',
              flexDirection: { xs: 'column', md: 'row' },

              transition: (theme) => theme.transitions.create(['top']),
              ...(isMobile && {
                py: 6,
                backgroundColor: 'background.paper',
                zIndex: 'appBar',
                position: 'fixed',
                height: { xs: '100vh', md: 'auto' },
                top: visibleMenu ? 0 : '-120vh',
                left: 0,
              }),
            }}
          >
            <Navigation />
            <Stack direction="row">
              <SlackIconButton />
              <Link component={NextLink} href="https://app.unityhub.io">
                <Button variant="outlined" sx={{ marginLeft: 1 }} size="small" startIcon={<LoginIcon />}>
                  Login
                </Button>
              </Link>
              <Link component={NextLink} href="https://app.unityhub.io">
                <Button variant="contained" sx={{ marginLeft: 1 }} size="small" startIcon={<ExitToAppIcon />}>
                  Sign up
                </Button>
              </Link>
            </Stack>
            {visibleMenu && isMobile && (
              <IconButton
                sx={{
                  position: 'fixed',
                  top: 10,
                  right: 10,
                }}
                onClick={() => setVisibleMenu(!visibleMenu)}
              >
                <CloseIcon />
              </IconButton>
            )}
          </Box>
        </Box>
      </Container>
    </StyledAppBar>
  );
};

export default memo(Header);

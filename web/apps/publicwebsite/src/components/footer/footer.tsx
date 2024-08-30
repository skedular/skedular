import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import Grid from '@mui/material/Grid2';
import Typography from '@mui/material/Typography';
import { memo } from 'react';
import FooterNavigation from './footer-navigation';
import FooterSocialLinks from './footer-social-links';

const Footer = () => {
  return (
    <Box
      component="footer"
      sx={{
        backgroundColor: 'primary.main',
        py: { xs: 6, md: 10 },
        color: 'primary.contrastText',
      }}
    >
      <Container>
        <Grid container spacing={1}>
          <Grid sx={{ sx: 12, md: 5 }}>
            <Box sx={{ width: { xs: '100%', md: 360 }, mb: { xs: 3, md: 0 } }}>
              <Typography component="h2" variant="h2" sx={{ mb: 2 }}>
                UnityHub
              </Typography>
              <Typography variant="subtitle1" sx={{ letterSpacing: 1, mb: 2 }}>
                Transform your hybrid office with one tool to book desks and boost attendance, all in Slack and Web platforms.
              </Typography>
              <FooterSocialLinks />
            </Box>
          </Grid>
          <Grid sx={{ sx: 12, md: 7 }}>
            <FooterNavigation />
          </Grid>
        </Grid>
      </Container>
    </Box>
  );
};

export default memo(Footer);

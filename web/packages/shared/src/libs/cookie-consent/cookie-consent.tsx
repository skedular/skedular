import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import Grid from '@mui/material/Grid2';
import Snackbar from '@mui/material/Snackbar';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { hasCookie, setCookie } from 'cookies-next';
import { memo, useEffect, useState } from 'react';
import { optOutCookieName } from './constants';

const CookieConsent = () => {
  const [showCookieConsent, setShowCookieConsent] = useState(false);

  useEffect(() => {
    setShowCookieConsent(!hasCookie(optOutCookieName));
  }, []);

  const handleAccept = () => {
    setShowCookieConsent(false);
    setCookie('__opt_out', 'no', {});
  };

  const handleDecline = () => {
    setShowCookieConsent(false);
    setCookie('__opt_out', 'yes', {});
  };

  const action = (
    <>
      <Container>
        <Grid>
          <Typography variant="subtitle1">
            This site uses cookies to improve and customise your browsing experience and for analytics and metrics about our visitors. By continuing
            to use this site, you consent to the use of cookies. To find out more, see our privacy policy at https://unityhub.io/privacy-policy. If
            you decline, your information won’t be tracked when you visit this website. A single cookie will be used in your browser to remember your
            preference not to be tracked.
          </Typography>
        </Grid>
        <Grid>
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
            <Button variant="contained" onClick={handleAccept} sx={{ margin: 1 }}>
              OK - continue browsing
            </Button>
            <Button variant="outlined" onClick={handleDecline} sx={{ margin: 1 }}>
              Decline
            </Button>
          </Stack>
        </Grid>
      </Container>
    </>
  );

  return <Snackbar open={showCookieConsent} action={action} anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }} />;
};

export default memo(CookieConsent);

'use client';

import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import Grid from '@mui/material/Grid';
import Snackbar from '@mui/material/Snackbar';
import { SmallIconTypography, StackRow } from '@skedular/ui';
import { hasCookie, setCookie } from 'cookies-next';
import { memo, useMemo, useState } from 'react';
import { optOutCookieName } from './constants';

const CookieConsent = () => {
  const initialConsent = useMemo(() => {
    if (typeof document === 'undefined') {
      return false;
    }

    return !hasCookie(optOutCookieName);
  }, []);

  const [showCookieConsent, setShowCookieConsent] = useState(initialConsent);

  const handleAccept = () => {
    setShowCookieConsent(false);
    setCookie(optOutCookieName, 'no', {});
  };

  const handleDecline = () => {
    setShowCookieConsent(false);
    setCookie(optOutCookieName, 'yes', {});
  };

  const action = (
    <Container>
      <Grid>
        <SmallIconTypography label="This site uses cookies to improve and customise your browsing experience and for analytics and metrics about our visitors. By continuing to use this site, you consent to the use of cookies. To find out more, see our privacy policy at https://getskedular.com/privacy-policy. If you decline, your information won't be tracked when you visit this website. A single cookie will be used in your browser to remember your preference not to be tracked." />
      </Grid>
      <Grid>
        <StackRow>
          <Button variant="contained" onClick={handleAccept} sx={{ margin: 1 }}>
            OK – continue browsing
          </Button>
          <Button variant="outlined" onClick={handleDecline} sx={{ margin: 1 }}>
            Decline
          </Button>
        </StackRow>
      </Grid>
    </Container>
  );

  return <Snackbar open={showCookieConsent} action={action} anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }} />;
};

export default memo(CookieConsent);

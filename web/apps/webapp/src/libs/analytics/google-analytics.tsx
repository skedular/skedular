import { optOutCookieName } from '@/libs/cookie-consent';
import { GoogleAnalytics as GA } from '@next/third-parties/google';
import { getCookie, hasCookie } from 'cookies-next';
import { useEffect, useState } from 'react';

type Props = {
  ignoreOptOutCookie: boolean;
  forceOverride: boolean;
};

const GoogleAnalytics = ({ ignoreOptOutCookie, forceOverride }: Props) => {
  const [shouldUseAnalytics, setShouldUseAnalytics] = useState(false);

  useEffect(() => {
    if (forceOverride) {
      setShouldUseAnalytics(true);

      return;
    }

    if (ignoreOptOutCookie) {
      setShouldUseAnalytics(true);

      return;
    }

    if (hasCookie(optOutCookieName)) {
      setShouldUseAnalytics(getCookie(optOutCookieName) === 'no');
    } else {
      setShouldUseAnalytics(true);
    }
  }, [ignoreOptOutCookie, forceOverride]);

  return <>{shouldUseAnalytics && process.env.NEXT_PUBLIC_GOOGLE_ANALYTICS_MEASUREMENT_ID && <GA gaId={process.env.NEXT_PUBLIC_GOOGLE_ANALYTICS_MEASUREMENT_ID} />}</>;
};

export default GoogleAnalytics;

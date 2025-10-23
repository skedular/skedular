import { optOutCookieName } from '@/libs/cookie-consent';
import { GoogleAnalytics as GA } from '@next/third-parties/google';
import { getCookie, hasCookie } from 'cookies-next';
import { useMemo } from 'react';

type Props = {
  ignoreOptOutCookie: boolean;
  forceOverride: boolean;
};

const GoogleAnalytics = ({ ignoreOptOutCookie, forceOverride }: Props) => {
  const shouldUseAnalytics = useMemo(() => {
    if (forceOverride || ignoreOptOutCookie) {
      return true;
    }

    if (hasCookie(optOutCookieName)) {
      return getCookie(optOutCookieName) === 'no';
    }

    return true;
  }, [ignoreOptOutCookie, forceOverride]);

  return <>{shouldUseAnalytics && process.env.NEXT_PUBLIC_GOOGLE_ANALYTICS_MEASUREMENT_ID && <GA gaId={process.env.NEXT_PUBLIC_GOOGLE_ANALYTICS_MEASUREMENT_ID} />}</>;
};

export default GoogleAnalytics;

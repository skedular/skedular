import { GoogleTagManager as GTM } from '@next/third-parties/google';
import { getCookie, hasCookie } from 'cookies-next';
import { useEffect, useState } from 'react';
import { optOutCookieName } from '../cookie-consent';

type Props = {
  ignoreOptOutCookie: boolean;
  forceOverride: boolean;
};

const GoogleTagManager = ({ ignoreOptOutCookie, forceOverride }: Props) => {
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

  return (
    <>
      {shouldUseAnalytics && process.env.NEXT_PUBLIC_GOOGLE_TAG_MANAGER_CONTAINER_ID && (
        <GTM gtmId={process.env.NEXT_PUBLIC_GOOGLE_TAG_MANAGER_CONTAINER_ID} />
      )}
    </>
  );
};

export default GoogleTagManager;

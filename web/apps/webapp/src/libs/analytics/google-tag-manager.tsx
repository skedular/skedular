import { optOutCookieName } from '@/libs/cookie-consent';
import { GoogleTagManager as GTM } from '@next/third-parties/google';
import { getCookie, hasCookie } from 'cookies-next';
import { useMemo } from 'react';

type Props = {
  ignoreOptOutCookie: boolean;
  forceOverride: boolean;
};

const GoogleTagManager = ({ ignoreOptOutCookie, forceOverride }: Props) => {
  const shouldUseAnalytics = useMemo(() => {
    if (forceOverride || ignoreOptOutCookie) {
      return true;
    }

    if (hasCookie(optOutCookieName)) {
      return getCookie(optOutCookieName) === 'no';
    }

    return true;
  }, [ignoreOptOutCookie, forceOverride]);

  return <>{shouldUseAnalytics && process.env.NEXT_PUBLIC_GOOGLE_TAG_MANAGER_CONTAINER_ID && <GTM gtmId={process.env.NEXT_PUBLIC_GOOGLE_TAG_MANAGER_CONTAINER_ID} />}</>;
};

export default GoogleTagManager;

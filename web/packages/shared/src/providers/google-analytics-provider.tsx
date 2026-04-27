'use client';

import { sendGTMEvent } from '@next/third-parties/google';
import { getCookie, hasCookie } from 'cookies-next';
import { usePathname } from 'next/navigation';
import { useEffect, useMemo } from 'react';
import { optOutCookieName } from '../cookie-consent/constants';

type Props = {
  ignoreOptOutCookie: boolean;
  forceOverride: boolean;
  googleTagManagerContainerId: string;
};

const GoogleAnalyticsProvider = ({ ignoreOptOutCookie, forceOverride, googleTagManagerContainerId }: Props) => {
  const pathname = usePathname();
  const shouldUseAnalytics = useMemo(() => {
    if (forceOverride || ignoreOptOutCookie) {
      return true;
    }

    if (hasCookie(optOutCookieName)) {
      return getCookie(optOutCookieName) === 'no';
    }

    return true;
  }, [ignoreOptOutCookie, forceOverride]);

  useEffect(() => {
    if (shouldUseAnalytics && googleTagManagerContainerId) {
      sendGTMEvent({
        event: 'page_view',
        page: window.location.href,
      });
    }
  }, [shouldUseAnalytics, pathname, googleTagManagerContainerId]);

  return null;
};

export default GoogleAnalyticsProvider;

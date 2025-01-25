import { optOutCookieName } from '@/libs/cookie-consent';
import { sendGTMEvent } from '@next/third-parties/google';
import { getCookie, hasCookie } from 'cookies-next';
import { usePathname } from 'next/navigation';
import { useEffect, useState } from 'react';

type Props = {
  ignoreOptOutCookie: boolean;
  forceOverride: boolean;
  googleTagManagerContainerId: string;
};

const GoogleAnalyticsProvider = ({ ignoreOptOutCookie, forceOverride, googleTagManagerContainerId }: Props) => {
  const [shouldUseAnalytics, setShouldUseAnalytics] = useState(false);
  const pathname = usePathname();

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

  useEffect(() => {
    if (shouldUseAnalytics && googleTagManagerContainerId) {
      sendGTMEvent({
        event: 'page_view',
        page: window.location.href,
      });
    }
  }, [shouldUseAnalytics, pathname, googleTagManagerContainerId]);

  return <></>;
};

export default GoogleAnalyticsProvider;

'use client';

import { getCookie, hasCookie } from 'cookies-next';
import LogRocket from 'logrocket';
import setupLogRocketReact from 'logrocket-react';
import { useEffect, useState } from 'react';
import { optOutCookieName } from '../cookie-consent';

type Props = {
  children?: React.ReactNode;
  ignoreOptOutCookie: boolean;
  forceOverride: boolean;
  logRocketAppId: string | undefined;
};

const LogRocketProvider = ({ children, ignoreOptOutCookie, forceOverride, logRocketAppId }: Props) => {
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

  useEffect(() => {
    // only initialize when in the browser
    if (shouldUseAnalytics && typeof window !== 'undefined' && logRocketAppId) {
      LogRocket.init(logRocketAppId);
      setupLogRocketReact(LogRocket);
    }
  }, [shouldUseAnalytics, logRocketAppId]);

  return <>{children}</>;
};

export default LogRocketProvider;

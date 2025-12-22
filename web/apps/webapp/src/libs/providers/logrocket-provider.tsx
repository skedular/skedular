import { optOutCookieName } from '@/libs/cookie-consent';
import { getCookie, hasCookie } from 'cookies-next';
import LogRocket from 'logrocket';
import setupLogRocketReact from 'logrocket-react';
import { useEffect, useMemo } from 'react';

type Props = {
  ignoreOptOutCookie: boolean;
  forceOverride: boolean;
  logRocketAppId?: string;
};

const LogRocketProvider = ({ ignoreOptOutCookie, forceOverride, logRocketAppId }: Props) => {
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
    if (!shouldUseAnalytics || typeof window === 'undefined' || !logRocketAppId) {
      return;
    }

    LogRocket.init(logRocketAppId);
    setupLogRocketReact();
  }, [shouldUseAnalytics, logRocketAppId]);

  return null;
};

export default LogRocketProvider;

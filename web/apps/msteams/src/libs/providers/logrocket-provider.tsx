import LogRocket from 'logrocket';
import setupLogRocketReact from 'logrocket-react';
import { useEffect } from 'react';

type Props = {
  logRocketAppId: string | undefined;
};

const LogRocketProvider = ({ logRocketAppId }: Props) => {
  useEffect(() => {
    // only initialize when in the browser
    if (typeof window !== 'undefined' && logRocketAppId) {
      LogRocket.init(logRocketAppId);
      setupLogRocketReact(LogRocket);
    }
  }, [logRocketAppId]);

  return <></>;
};

export default LogRocketProvider;

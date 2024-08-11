import LogRocket from 'logrocket';
import setupLogRocketReact from 'logrocket-react';
import { useEffect } from 'react';

type Props = {
  children?: React.ReactNode;
  logRocketAppId: string | undefined;
};

const LogRocketProvider = ({ children, logRocketAppId }: Props) => {
  useEffect(() => {
    // only initialize when in the browser
    if (typeof window !== 'undefined' && logRocketAppId) {
      LogRocket.init(logRocketAppId);
      setupLogRocketReact(LogRocket);
    }
  }, [logRocketAppId]);

  return <>{children}</>;
};

export default LogRocketProvider;

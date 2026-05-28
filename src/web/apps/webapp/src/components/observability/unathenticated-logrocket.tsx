import LogRocket from 'logrocket';
import { memo, useEffect } from 'react';

const UnauthenticatedLogRocket = () => {
  useEffect(() => {
    LogRocket.identify('unauthenticated', {});
  }, []);

  return null;
};

export default memo(UnauthenticatedLogRocket);

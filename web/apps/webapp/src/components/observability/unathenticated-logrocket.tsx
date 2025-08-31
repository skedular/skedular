import LogRocket from 'logrocket';
import { memo, useEffect } from 'react';

const UnauthenticatedLogRocket = () => {
  useEffect(() => {
    LogRocket.identify('unauthenticated', {});
  }, []);

  return <></>;
};

export default memo(UnauthenticatedLogRocket);

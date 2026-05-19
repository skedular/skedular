import { memo } from 'react';
import UnauthenticatedLogRocket from './unathenticated-logrocket';

const UnathenticatedObservability = () => <>{process.env.NEXT_PUBLIC_LOGROCKET_APP_ID && <UnauthenticatedLogRocket />}</>;

export default memo(UnathenticatedObservability);

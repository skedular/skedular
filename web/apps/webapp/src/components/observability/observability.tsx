import type { observability_query$key } from '@/queries/__generated__/observability_query.graphql';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';
import LogRocket from './logrocket';

type Props = {
  rootDataRelay: observability_query$key;
  onReloadRequired: () => void;
};

const Observability = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<observability_query$key>(
    graphql`
      fragment observability_query on Query {
        ...logrocket_query
      }
    `,
    rootDataRelay,
  );

  return <>{process.env.NEXT_PUBLIC_LOGROCKET_APP_ID && <LogRocket rootDataRelay={rootData} />}</>;
};

export default memo(Observability);

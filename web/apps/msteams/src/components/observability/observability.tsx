import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';
import type { observability_query$key } from './__generated__/observability_query.graphql';
import LogRocket from './logrocket';

type Props = {
  rootDataRelay: observability_query$key;
};

const Observability = ({ rootDataRelay }: Props) => {
  const rootData = useFragment(
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

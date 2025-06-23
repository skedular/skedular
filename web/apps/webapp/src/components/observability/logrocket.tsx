import type { logrocket_query$key } from '@/queries/__generated__/logrocket_query.graphql';
import LogRocket from 'logrocket';
import { memo, useEffect } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: logrocket_query$key;
};

const LogRocketComponent = ({ rootDataRelay }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment logrocket_query on Query {
        me {
          id
          email
          title
          givenName
          middleName
          familyName
        }
      }
    `,
    rootDataRelay,
  );

  useEffect(() => {
    LogRocket.identify(rootData.me?.id, {
      email: rootData.me?.email ?? '',
      title: rootData.me?.title ?? '',
      givenName: rootData.me?.givenName ?? '',
      middleName: rootData.me?.middleName ?? '',
      familyName: rootData.me?.familyName ?? '',
    });
  }, [rootData]);

  return <></>;
};

export default memo(LogRocketComponent);

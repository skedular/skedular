import graphql from 'babel-plugin-relay/macro';
import LogRocket from 'logrocket';
import { memo, useEffect } from 'react';
import { useFragment } from 'react-relay';
import type { logrocket_query$key } from './__generated__/logrocket_query.graphql';

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
    if (!rootData.me) {
      return;
    }

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

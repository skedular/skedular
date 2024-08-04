import graphql from 'babel-plugin-relay/macro';
import { TeamsFxContext } from 'libs/providers';
import { memo, useContext, useEffect } from 'react';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { appHome_rootQuery } from './__generated__/appHome_rootQuery.graphql';

const RootQuery = graphql`
  query appHome_rootQuery {
    msTeamsVersion {
      major
    }
  }
`;

type Props = {
  queryReference: PreloadedQuery<appHome_rootQuery, Record<string, unknown>>;
};

const Home = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<appHome_rootQuery>(RootQuery, queryReference);
  const { themeString } = useContext(TeamsFxContext);

  return (
    <div className={themeString === 'default' ? 'light' : themeString === 'dark' ? 'dark' : 'contrast'}>
      <h1>Testing home page</h1>
      {rootData.msTeamsVersion.major}
    </div>
  );
};

const MemoHome = memo(Home);

const HomeWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<appHome_rootQuery>(RootQuery);

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery]);

  if (queryReference == null) {
    return <></>;
  }

  return <MemoHome queryReference={queryReference} />;
};

export default memo(HomeWithRelay);

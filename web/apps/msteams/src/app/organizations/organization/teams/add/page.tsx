import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { RootShell } from 'components/rootShell';
import { AddTeam } from 'components/team/addTeam';
import { memo, useCallback, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { useParams } from 'react-router-dom';
import type { pageAddOrganizationTeam_rootQuery } from './__generated__/pageAddOrganizationTeam_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<pageAddOrganizationTeam_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageAddOrganizationTeam_rootQuery {
    teamCustomerRecordSynced
    ...rootShell_query
  }
`;

const AddTeamPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageAddOrganizationTeam_rootQuery>(RootQuery, queryReference);
  const { organizationId } = useParams();
  let finalOrganizationId = '';

  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] === 'undefined') {
      throw new Error('organizationId is required');
    }

    finalOrganizationId = organizationId[0];
  } else {
    throw new Error('organizationId is required');
  }
  const areAdditionalCustomerRecordsSync = useCallback(() => rootData?.teamCustomerRecordSynced, [rootData?.teamCustomerRecordSynced]);

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequired={onReloadRequired}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.teamCustomerRecordSynced]}
    >
      <AddTeam organizationId={finalOrganizationId} />
    </RootShell>
  );
};

const MemoAddTeamPage = memo(AddTeamPage);

const AddTeamPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageAddOrganizationTeam_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReload(triggerReload + 1);
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoAddTeamPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(AddTeamPageWithRelay);

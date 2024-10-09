import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { Organization } from 'components/organization/organizationPage';
import { RootShell } from 'components/rootShell';
import { memo, useCallback, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { useParams } from 'react-router-dom';
import type { pageOrganization_rootQuery } from './__generated__/pageOrganization_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<pageOrganization_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageOrganization_rootQuery {
    organizationCustomerRecordSynced
    ...rootShell_query
  }
`;

const OrganizationPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageOrganization_rootQuery>(RootQuery, queryReference);
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
  const areAdditionalCustomerRecordsSync = useCallback(
    () => rootData?.organizationCustomerRecordSynced,
    [rootData?.organizationCustomerRecordSynced],
  );

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequired={onReloadRequired}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.organizationCustomerRecordSynced]}
    >
      <Organization organizationId={finalOrganizationId} />
    </RootShell>
  );
};

const MemoOrganizationPage = memo(OrganizationPage);

const OrganizationPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganization_rootQuery>(RootQuery);
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
      <MemoOrganizationPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationPageWithRelay);

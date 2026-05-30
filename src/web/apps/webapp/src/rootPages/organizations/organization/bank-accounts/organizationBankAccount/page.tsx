import { EditBankAccount } from '@/components/bankAccount/editBanktAccount';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@skedular/shared';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationBankAccount_rootQuery } from '@/queries/__generated__/pageOrganizationBankAccount_rootQuery.graphql';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import useKnownParams from '@/hooks/use-known-params';

const RootQuery = graphql`
  query pageOrganizationBankAccount_rootQuery($organizationBankAccountId: String!) {
    organizationBankAccount(id: $organizationBankAccountId) {
      name
    }
    ...editBankAccount_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationBankAccount_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationBankAccount_rootQuery>(RootQuery, queryReference);

  if (!rootData.organizationBankAccount) {
    return null;
  }

  return (
    <RootShell>
      <EditBankAccount rootDataRelay={rootData} onReloadRequired={onReloadRequired} />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationBankAccount_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationBankAccountId } = useKnownParams();
  if (!organizationBankAccountId) {
    throw new Error('organizationBankAccountId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationBankAccountId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationBankAccountId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);

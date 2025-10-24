import { EditBankAccount } from '@/components/bankAccount/editBanktAccount';
import { BodyIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationBankAccount_rootQuery } from '@/queries/__generated__/pageOrganizationBankAccount_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { useParams, useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

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
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

  if (!rootData.organizationBankAccount) {
    return null;
  }

  const breadcrumbs = (
    <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
      <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
        {'< back'}
      </Button>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <Breadcrumbs>
          <BodyIconTypography label="Bank Account" />
          <BodyIconTypography label={rootData.organizationBankAccount.name} />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <EditBankAccount rootDataRelay={rootData} onReloadRequired={onReloadRequired} />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationBankAccount_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationBankAccountId } = useParams();
  let finalOrganizationBankAccountId = '';

  if (typeof organizationBankAccountId === 'string') {
    finalOrganizationBankAccountId = organizationBankAccountId;
  } else if (Array.isArray(organizationBankAccountId)) {
    if (typeof organizationBankAccountId[0] === 'undefined') {
      throw new Error('organizationBankAccountId is required');
    }

    finalOrganizationBankAccountId = organizationBankAccountId[0];
  } else {
    throw new Error('organizationBankAccountId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationBankAccountId: finalOrganizationBankAccountId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, finalOrganizationBankAccountId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);

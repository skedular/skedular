import { BodyIconTypography, StackColumn } from '@skedular/ui';
import { Loading } from '@/components/loading';
import { OrganizationUser } from '@/components/organization/organizationUser';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { getCustomerFullName } from '@skedular/shared';
import type { pageOrganizationUser_rootQuery } from '@/queries/__generated__/pageOrganizationUser_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  queryReference: PreloadedQuery<pageOrganizationUser_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  customerId: string;
};

const RootQuery = graphql`
  query pageOrganizationUser_rootQuery($organizationCustomDomain: String!, $customerId: String!, $teamsSortingValues: [TeamOrderInput!]) {
    customer(id: $customerId) {
      name
      givenName
      middleName
      familyName
    }
    ...organizationUser_query
  }
`;

const UserPage = ({ queryReference, onReloadRequired, organizationCustomDomain, customerId }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationUser_rootQuery>(RootQuery, queryReference);
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

  const breadcrumbs = (
    <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
      <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
        {'< back'}
      </Button>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <Breadcrumbs>
          <BodyIconTypography label="User" />
          <BodyIconTypography label={getCustomerFullName(rootData.customer)} />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <OrganizationUser rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} customerId={customerId} />
    </RootShell>
  );
};

const MemoUserPage = memo(UserPage);

const UserPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationUser_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationCustomDomain, customerId } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  if (!customerId) {
    throw new Error('customerId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        customerId,
        teamsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationCustomDomain, customerId]);

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
      <MemoUserPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} customerId={customerId} />
    </ErrorBoundary>
  );
};

export default memo(UserPageWithRelay);

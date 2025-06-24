import { BodyIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import { OrganizationUser } from '@/components/organization/organizationUser';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { getCustomerFullName } from '@/libs/utils';
import type { pageOrganizationUser_rootQuery } from '@/queries/__generated__/pageOrganizationUser_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { useParams, useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<pageOrganizationUser_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  customerId: string;
};

const RootQuery = graphql`
  query pageOrganizationUser_rootQuery($organizationId: String!, $customerId: String!, $teamsSortingValues: [TeamOrderInput!]) {
    customer(id: $customerId) {
      name
      givenName
      middleName
      familyName
    }
    ...organizationUser_query
  }
`;

const UserPage = ({ queryReference, onReloadRequired, organizationId, customerId }: Props) => {
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
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <OrganizationUser rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationId={organizationId} customerId={customerId} />
    </RootShell>
  );
};

const MemoUserPage = memo(UserPage);

const UserPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationUser_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationId, customerId } = useParams();
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

  let finalCustomerId = '';

  if (typeof customerId === 'string') {
    finalCustomerId = customerId;
  } else if (Array.isArray(customerId)) {
    if (typeof customerId[0] === 'undefined') {
      throw new Error('customerId is required');
    }

    finalCustomerId = customerId[0];
  } else {
    throw new Error('customerId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationId: finalOrganizationId,
        customerId: finalCustomerId,
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
  }, [loadQuery, triggerReloadId, finalOrganizationId, finalCustomerId]);

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
      <MemoUserPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={finalOrganizationId} customerId={finalCustomerId} />
    </ErrorBoundary>
  );
};

export default memo(UserPageWithRelay);

'use client';

import { OrganizationAnalytics } from '@/components/organization/organizationAnalytics';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationAnalytics_rootQuery } from '@/queries/__generated__/pageOrganizationAnalytics_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { BodyIconTypography, StackColumn } from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { nanoid } from 'nanoid';
import { useParams, useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageOrganizationAnalytics_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query pageOrganizationAnalytics_rootQuery($organizationId: String!, $locationsSortingValues: [LocationOrderInput!]) {
    organization(id: $organizationId) {
      name
    }
    ...organizationAnalytics_query
  }
`;

const AnalyticsPage = ({ queryReference, onReloadRequired, organizationId }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationAnalytics_rootQuery>(RootQuery, queryReference);
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
          <BodyIconTypography label="Analytics" />
          <BodyIconTypography label={rootData.organization?.name} />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <OrganizationAnalytics rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationId={organizationId} />
    </RootShell>
  );
};

const MemoAnalyticsPage = memo(AnalyticsPage);

const AnalyticsPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationAnalytics_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();
  const { organizationId, teamId } = useParams();
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

  useEffect(() => {
    loadQuery(
      {
        organizationId: finalOrganizationId,
        locationsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, finalOrganizationId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoAnalyticsPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={finalOrganizationId} />
    </ErrorBoundary>
  );
};

export default memo(AnalyticsPageWithRelay);

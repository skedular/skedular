'use client';

import { EditDesk } from '@/components/desk/editDesk';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationLocationDesk_rootQuery } from '@/queries/__generated__/pageOrganizationLocationDesk_rootQuery.graphql';
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

const RootQuery = graphql`
  query pageOrganizationLocationDesk_rootQuery(
    $organizationId: String!
    $deskId: String!
    $multipleChoicesCustomTagsSortingValues: [OrganizationTagOrderInput!]
    $multipleChoicesZonesSortingValues: [OrganizationTagOrderInput!]
  ) {
    desk(id: $deskId) {
      name
    }
    ...editDesk_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationLocationDesk_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  deskId: string;
};

const LocationPage = ({ queryReference, onReloadRequired, organizationId, deskId }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationLocationDesk_rootQuery>(RootQuery, queryReference);
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

  if (!rootData.desk) {
    return <></>;
  }

  const breadcrumbs = (
    <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
      <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
        {'< back'}
      </Button>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <Breadcrumbs>
          <BodyIconTypography label="Desk Settings" />
          <BodyIconTypography label={rootData.desk.name} />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <EditDesk rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationId={organizationId} />
    </RootShell>
  );
};

const MemoLocationPage = memo(LocationPage);

const LocationPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationLocationDesk_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();
  const { organizationId, deskId } = useParams();
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

  let finalDeskId = '';

  if (typeof deskId === 'string') {
    finalDeskId = deskId;
  } else if (Array.isArray(deskId)) {
    if (typeof deskId[0] === 'undefined') {
      throw new Error('deskId is required');
    }

    finalDeskId = deskId[0];
  } else {
    throw new Error('deskId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationId: finalOrganizationId,
        deskId: finalDeskId,
        multipleChoicesCustomTagsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        multipleChoicesZonesSortingValues: [
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
  }, [loadQuery, triggerReloadId, finalOrganizationId, finalDeskId]);

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
      <MemoLocationPage
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationId={finalOrganizationId}
        deskId={finalDeskId}
      />
    </ErrorBoundary>
  );
};

export default memo(LocationPageWithRelay);

'use client';

import { Location } from '@/components/location/locationPage';
import { OrganizationLocation } from '@/components/organization/organizationLocation';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationLocation_rootQuery } from '@/queries/__generated__/pageOrganizationLocation_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { BodyIconTypography, StackColumn } from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { SwitchToModernUIContext } from '@repo/shared/libs/providers';
import { nanoid } from 'nanoid';
import { useParams, useRouter } from 'next/navigation';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

const RootQuery = graphql`
  query pageOrganizationLocation_rootQuery(
    $organizationId: String!
    $locationId: String!
    $deskNameSearchText: String
    $zonesSortingValues: [OrganizationTagOrderInput!]
    $customTagsSortingValues: [OrganizationTagOrderInput!]
    $deskZoneIds: [String!]
    $deskCustomTagIds: [String!]
  ) {
    location(id: $locationId) {
      name
    }
    ...organizationLocation_query
    ...organizationLocation_desks_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationLocation_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  locationId: string;
};

const LocationPage = ({ queryReference, onReloadRequired, organizationId, locationId }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationLocation_rootQuery>(RootQuery, queryReference);
  const switchToModernUI = useContext(SwitchToModernUIContext);
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

  if (switchToModernUI) {
    const breadcrumbs = (
      <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
        <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
          {'< back'}
        </Button>
        <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
          <Breadcrumbs>
            <BodyIconTypography label="Location Settings" />
            <BodyIconTypography label={rootData.location?.name} />
          </Breadcrumbs>
        </Box>
      </StackColumn>
    );

    return (
      <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
        <OrganizationLocation
          rootDataRelay={rootData}
          rootDataDesksRelay={rootData}
          onReloadRequired={onReloadRequired}
          organizationId={organizationId}
          locationId={locationId}
        />
      </RootShell>
    );
  }

  return (
    <RootShell>
      <Location organizationId={organizationId} locationId={locationId} />
    </RootShell>
  );
};

const MemoLocationPage = memo(LocationPage);

const LocationPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationLocation_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();
  const { organizationId, locationId } = useParams();
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

  let finalLocationId = '';

  if (typeof locationId === 'string') {
    finalLocationId = locationId;
  } else if (Array.isArray(locationId)) {
    if (typeof locationId[0] === 'undefined') {
      throw new Error('locationId is required');
    }

    finalLocationId = locationId[0];
  } else {
    throw new Error('locationId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationId: finalOrganizationId,
        locationId: finalLocationId,
        zonesSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        customTagsSortingValues: [
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
  }, [loadQuery, triggerReloadId, finalOrganizationId, finalLocationId]);

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
        locationId={finalLocationId}
      />
    </ErrorBoundary>
  );
};

export default memo(LocationPageWithRelay);

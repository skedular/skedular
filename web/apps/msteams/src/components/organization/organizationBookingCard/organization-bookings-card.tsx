import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Skeleton from '@mui/material/Skeleton';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { OrganizationLink } from 'components/organization';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { organizationBookingsCard_rootQuery } from './__generated__/organizationBookingsCard_rootQuery.graphql';
import OrganizationMembersBookings from './organization-members-bookings';

type Props = {
  queryReference: PreloadedQuery<organizationBookingsCard_rootQuery, Record<string, unknown>>;
  organizationId: string;
  organizationName?: string;
  organizationsConnectionIds: string[];
  hideRemoveOrganizationOption?: boolean;
};

const RootQuery = graphql`
  query organizationBookingsCard_rootQuery(
    $peopleSortingValues: [OrganizationMemberOrderInput!]!
    $organizationId: String!
    $locationId: String!
    $teamId: String!
    $from: DateTime!
    $to: DateTime!
  ) {
    ...organizationMembersBookings_query
  }
`;

const OrganizationBookingsCard = ({
  queryReference,
  organizationId,
  organizationName,
  organizationsConnectionIds,
  hideRemoveOrganizationOption,
}: Props) => {
  const rootData = usePreloadedQuery<organizationBookingsCard_rootQuery>(RootQuery, queryReference);

  return (
    <OrganizationMembersBookings
      rootDataRelay={rootData}
      organizationId={organizationId}
      organizationName={organizationName}
      organizationsConnectionIds={organizationsConnectionIds}
      hideRemoveOrganizationOption={hideRemoveOrganizationOption}
    />
  );
};

const MemoOrganizationBookingsCard = memo(OrganizationBookingsCard);

type RelayProps = {
  organizationId: string;
  organizationName?: string;
  organizationsConnectionIds: string[];
  hideRemoveOrganizationOption?: boolean;
};

const OrganizationBookingsWithRelay = ({
  organizationId,
  organizationName,
  organizationsConnectionIds,
  hideRemoveOrganizationOption,
}: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationBookingsCard_rootQuery>(RootQuery);

  useEffect(() => {
    const startDate = startOfDay();
    const endDate = startDate.add(1, 'week');

    loadQuery(
      {
        peopleSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        organizationId,
        locationId: '',
        teamId: '',
        from: startDate.toISOString(),
        to: endDate.toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationId]);

  if (!queryReference) {
    return (
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader title={<OrganizationLink id={organizationId} name={organizationName} />} />
        <CardContent>
          <Skeleton variant="rounded" width={470} height={350} />
        </CardContent>
      </Card>
    );
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOrganizationBookingsCard
        queryReference={queryReference}
        organizationId={organizationId}
        organizationName={organizationName}
        organizationsConnectionIds={organizationsConnectionIds}
        hideRemoveOrganizationOption={hideRemoveOrganizationOption}
      />
    </ErrorBoundary>
  );
};

export default memo(OrganizationBookingsWithRelay);

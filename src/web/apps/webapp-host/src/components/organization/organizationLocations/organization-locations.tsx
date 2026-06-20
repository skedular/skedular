import { RelayError, toRootError } from '@skedular/shared';
import { Loading } from '@/components/loading';
import { NewLocationButton } from '@/components/location/addLocation';
import type { organizationLocations_rootQuery } from '@/queries/__generated__/organizationLocations_rootQuery.graphql';
import Box from '@mui/system/Box';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, type PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import LocationCard from './location-card';
import OrganizationLocationsPageShell from './organization-locations-page-shell';

type Props = {
  queryReference: PreloadedQuery<organizationLocations_rootQuery, Record<string, unknown>>;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query organizationLocations_rootQuery($organizationCustomDomain: String!, $locationsSortingValues: [LocationOrderInput!]) {
    organization(customDomain: $organizationCustomDomain) {
      canModify
    }
    ...newLocationButton_query
    ...locationCard_query
    locations(first: 100, where: { organizationCustomDomain: $organizationCustomDomain }, orderBy: $locationsSortingValues) @connection(key: "organizationLocations_locations") {
      __id
      edges {
        node {
          id
          ...locationCard_LocationDetails
        }
      }
    }
  }
`;

const OrganizationLocations = ({ queryReference, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<organizationLocations_rootQuery>(RootQuery, queryReference);
  const locations = rootData.locations.edges.map(({ node }) => node);
  const connectionIds = [rootData.locations.__id];
  const actions = rootData.organization?.canModify ? <NewLocationButton rootDataRelay={rootData} organizationCustomDomain={organizationCustomDomain} /> : null;

  return (
    <OrganizationLocationsPageShell actions={actions} isEmpty={locations.length === 0}>
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: { xs: '1fr', sm: 'repeat(auto-fit, minmax(320px, 360px))' },
          gap: 2,
          alignItems: 'stretch',
          justifyContent: 'start',
        }}
      >
        {locations.map((location) => (
          <LocationCard key={location.id} rootDataRelay={rootData} locationDetailsRelay={location} connectionIds={connectionIds} />
        ))}
      </Box>
    </OrganizationLocationsPageShell>
  );
};

const MemoOrganizationLocations = memo(OrganizationLocations);

const OrganizationLocationsWithRelay = ({ organizationCustomDomain }: { organizationCustomDomain: string }) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationLocations_rootQuery>(RootQuery);

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        locationsSortingValues: [{ direction: 'ASCENDING', field: 'NAME' }],
      },
      { fetchPolicy: 'store-and-network' },
    );
  }, [loadQuery, organizationCustomDomain]);

  if (!queryReference) return <Loading />;

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoOrganizationLocations queryReference={queryReference} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationLocationsWithRelay);

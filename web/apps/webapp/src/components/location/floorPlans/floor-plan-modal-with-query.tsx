import { Loading } from '@/components/loading';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { memo } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, graphql } from 'react-relay';
import FloorPlanModal from './floor-plan-modal';

type Props = {
  queryReference: PreloadedQuery<any, Record<string, unknown>>;
  organizationId: string;
  locationId: string;
  locationName: string;
  isOpen: boolean;
  onClose: () => void;
  onBookResource?: (resourceId: string) => void;
  platform?: string;
};

const FloorPlanModalQuery = graphql`
  query floorPlanModalWithQuery_Query($locationId: String!, $organizationId: String!, $dateFromToGetAvailableResources: DateTime!, $dateUntilToGetAvailableResources: DateTime!) {
    ...floorPlanModal_query @arguments(locationId: $locationId)
  }
`;

const FloorPlanModalWithQuery = ({ queryReference, organizationId, locationId, locationName, isOpen, onClose, onBookResource, platform = 'web' }: Props) => {
  const data = usePreloadedQuery(FloorPlanModalQuery, queryReference);

  return (
    <FloorPlanModal
      rootDataRelay={data}
      organizationId={organizationId}
      locationId={locationId}
      locationName={locationName}
      isOpen={isOpen}
      onClose={onClose}
      onBookResource={onBookResource}
      platform={platform}
    />
  );
};

const MemoFloorPlanModalWithQuery = memo(FloorPlanModalWithQuery);

export default function FloorPlanModalWithQueryWrapper(props: Props) {
  if (!props.queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoFloorPlanModalWithQuery {...props} />
    </ErrorBoundary>
  );
}

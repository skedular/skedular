import type { organizationOfferingTab_rootQuery } from '@/queries/__generated__/organizationOfferingTab_rootQuery.graphql';
import Stack from '@mui/material/Stack';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { nanoid } from 'nanoid';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import OrganizationAvailableOfferings from './organization-available-offerings';
import OrganizationOffering from './organization-offering';

type Props = {
  queryReference: PreloadedQuery<organizationOfferingTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query organizationOfferingTab_rootQuery($organizationId: String!) {
    organization(id: $organizationId) {
      id
      availableOfferings {
        code
      }
    }
    ...organizationOffering_query
    ...organizationAvailableOfferings_query
  }
`;

const OrganizationOfferingTab = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<organizationOfferingTab_rootQuery>(RootQuery, queryReference);

  return (
    <Stack spacing={1}>
      <OrganizationOffering rootDataRelay={rootData} onReloadRequired={onReloadRequired} />
      {rootData.organization?.availableOfferings && rootData.organization?.availableOfferings.length > 0 && (
        <OrganizationAvailableOfferings rootDataRelay={rootData} onReloadRequired={onReloadRequired} />
      )}
    </Stack>
  );
};

const MemoOrganizationOfferingTab = memo(OrganizationOfferingTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
};

const OrganizationOfferingTabWithRelay = ({ onReloadRequired, organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationOfferingTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOrganizationOfferingTab queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationOfferingTabWithRelay);

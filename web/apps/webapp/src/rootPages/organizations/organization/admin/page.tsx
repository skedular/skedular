import { BodyIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import { OrganizationAdmin } from '@/components/organization/organizationAdmin';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationAdmin_rootQuery } from '@/queries/__generated__/pageOrganizationAdmin_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { useParams, useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<pageOrganizationAdmin_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
};

const RootQuery = graphql`
  query pageOrganizationAdmin_rootQuery($organizationUniqueAlphanumericName: String!, $zoneNameSearchText: String, $customTagNameSearchText: String) {
    organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
      name
    }
    ...organizationAdmin_query
    ...organizationAdmin_organization_query
    ...organizationAdmin_zones_query
    ...organizationAdmin_customTags_query
  }
`;

const RootPage = ({ queryReference, onReloadRequired, organizationUniqueAlphanumericName }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationAdmin_rootQuery>(RootQuery, queryReference);
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
          <BodyIconTypography label="Admin" />
          <BodyIconTypography label={rootData.organization?.name} />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <OrganizationAdmin
        rootDataRelay={rootData}
        rootDataOrganizationRelay={rootData}
        rootDataZonesRelay={rootData}
        rootDataCustomTagsRelay={rootData}
        onReloadRequired={onReloadRequired}
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
      />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationAdmin_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationUniqueAlphanumericName } = useParams();
  let finalOrganizationUniqueAlphanumericName = '';

  if (typeof organizationUniqueAlphanumericName === 'string') {
    finalOrganizationUniqueAlphanumericName = organizationUniqueAlphanumericName;
  } else if (Array.isArray(organizationUniqueAlphanumericName)) {
    if (typeof organizationUniqueAlphanumericName[0] === 'undefined') {
      throw new Error('organizationUniqueAlphanumericName is required');
    }

    finalOrganizationUniqueAlphanumericName = organizationUniqueAlphanumericName[0];
  } else {
    throw new Error('organizationUniqueAlphanumericName is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationUniqueAlphanumericName: finalOrganizationUniqueAlphanumericName,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, finalOrganizationUniqueAlphanumericName]);

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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationUniqueAlphanumericName={finalOrganizationUniqueAlphanumericName} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);

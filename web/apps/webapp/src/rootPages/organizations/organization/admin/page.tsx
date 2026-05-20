import { BodyIconTypography, StackColumn } from '@skedular/ui';
import { Loading } from '@/components/loading';
import { OrganizationAdmin } from '@/components/organization/organizationAdmin';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationAdmin_rootQuery } from '@/queries/__generated__/pageOrganizationAdmin_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  queryReference: PreloadedQuery<pageOrganizationAdmin_rootQuery, Record<string, unknown>>;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query pageOrganizationAdmin_rootQuery($organizationCustomDomain: String!) {
    organization(customDomain: $organizationCustomDomain) {
      name
    }
    ...organizationAdmin_query
  }
`;

const RootPage = ({ queryReference, organizationCustomDomain }: Props) => {
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
      <OrganizationAdmin rootDataRelay={rootData} organizationCustomDomain={organizationCustomDomain} />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationAdmin_rootQuery>(RootQuery);
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationCustomDomain]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoRootPage queryReference={queryReference} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);

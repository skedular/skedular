import { BodyIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import { OrganizationMarketplaceSetup } from '@/components/organization/organizationMarketplaceSetup';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { useKnownParams } from '@/libs/providers';
import type { pageOrganizationMarketplaceSetup_rootQuery } from '@/queries/__generated__/pageOrganizationMarketplaceSetup_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<pageOrganizationMarketplaceSetup_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query pageOrganizationMarketplaceSetup_rootQuery(
    $organizationCustomDomain: String!
    $productTagNameSearchText: String
    $organizationStripeConnectAccountNameSearchText: String
    $organizationBankAccountNameSearchText: String
  ) {
    organization(customDomain: $organizationCustomDomain) {
      name
    }
    ...organizationMarketplaceSetup_query
    ...organizationMarketplaceSetup_productTags_query
    ...organizationMarketplaceSetup_organizationStripeConnectAccounts_query
    ...organizationMarketplaceSetup_organizationBankAccounts_query
  }
`;

const RootPage = ({ queryReference, onReloadRequired, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationMarketplaceSetup_rootQuery>(RootQuery, queryReference);
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
          <BodyIconTypography label="Marketplace Setup" />
          <BodyIconTypography label={rootData.organization?.name} />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <OrganizationMarketplaceSetup
        rootDataRelay={rootData}
        rootDataProductTagsRelay={rootData}
        rootDataOrganizationStripeConnectAccountsRelay={rootData}
        rootDataOrganizationBankAccountsRelay={rootData}
        onReloadRequired={onReloadRequired}
        organizationCustomDomain={organizationCustomDomain}
      />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationMarketplaceSetup_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
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
  }, [loadQuery, triggerReloadId, organizationCustomDomain]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);

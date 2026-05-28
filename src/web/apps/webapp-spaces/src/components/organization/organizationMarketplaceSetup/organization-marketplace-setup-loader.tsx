import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import type { pageOrganizationMarketplaceSetup_rootQuery } from '@/queries/__generated__/pageOrganizationMarketplaceSetup_rootQuery.graphql';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import OrganizationMarketplaceSetup from './organization-marketplace-setup';

type Props = {
  organizationCustomDomain: string;
  embedded?: boolean;
};

type InnerProps = Props & {
  queryReference: PreloadedQuery<pageOrganizationMarketplaceSetup_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
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

const OrganizationMarketplaceSetupLoaderContent = ({ queryReference, onReloadRequired, organizationCustomDomain, embedded }: InnerProps) => {
  const rootData = usePreloadedQuery<pageOrganizationMarketplaceSetup_rootQuery>(RootQuery, queryReference);

  return (
    <OrganizationMarketplaceSetup
      rootDataRelay={rootData}
      rootDataProductTagsRelay={rootData}
      rootDataOrganizationStripeConnectAccountsRelay={rootData}
      rootDataOrganizationBankAccountsRelay={rootData}
      onReloadRequired={onReloadRequired}
      organizationCustomDomain={organizationCustomDomain}
      embedded={embedded}
    />
  );
};

const MemoOrganizationMarketplaceSetupLoaderContent = memo(OrganizationMarketplaceSetupLoaderContent);

const OrganizationMarketplaceSetupLoader = ({ organizationCustomDomain, embedded }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationMarketplaceSetup_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

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
      <MemoOrganizationMarketplaceSetupLoaderContent
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationCustomDomain={organizationCustomDomain}
        embedded={embedded}
      />
    </ErrorBoundary>
  );
};

export default memo(OrganizationMarketplaceSetupLoader);

import { PaletteModeContext, RelayError, toRootError, useIntegratedPlatform } from '@skedular/shared';
import OrganizationAvatar from '@/components/avatars/organization-avatar';
import { LeadIconTypography, SmallIconTypography } from '@skedular/ui';
import { getOrganizationBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';

import { coal, emerald } from '@skedular/ui';
import type { pageOrganizationSsoSignin_rootQuery } from '@/queries/__generated__/pageOrganizationSsoSignin_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import { useSearchParams } from 'next/navigation';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  queryReference: PreloadedQuery<pageOrganizationSsoSignin_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageOrganizationSsoSignin_rootQuery($organizationCustomDomain: String!, $redirectUrl: String!) {
    organization(customDomain: $organizationCustomDomain) {
      logoUrl
      name
      ssoLoginUrl(redirectUrl: $redirectUrl)
    }
  }
`;

const RootPage = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationSsoSignin_rootQuery>(RootQuery, queryReference);
  const paletteMode = useContext(PaletteModeContext);

  if (!rootData.organization) {
    return null;
  }

  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
      <Card sx={{ textAlign: 'center', padding: 4, borderRadius: 3, maxWidth: 400, backgroundColor: paletteMode === 'dark' ? emerald : coal }}>
        <CardContent sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center' }}>
          <OrganizationAvatar name={{ name: rootData.organization?.name }} photo={{ url: rootData.organization?.logoUrl }} />
          <LeadIconTypography label={`Single sign-on to ${rootData.organization?.name}`} invertDefaultColor sx={{ marginTop: 2 }} />
          <SmallIconTypography label={`Authenticate your account by logging into ${rootData.organization?.name}'s single sign-on provider.`} invertDefaultColor />
          <Button variant="contained" href={rootData.organization.ssoLoginUrl} fullWidth sx={{ marginTop: 2 }}>
            Continue
          </Button>
        </CardContent>
      </Card>
    </Box>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationSsoSignin_rootQuery>(RootQuery);
  const { integratedPlatform } = useIntegratedPlatform();
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const searchParams = useSearchParams();
  const redirectUrl = searchParams.get('redirectUrl');
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        redirectUrl: redirectUrl ?? getOrganizationBaseLink(integratedPlatform, organizationCustomDomain),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationCustomDomain, redirectUrl, integratedPlatform]);

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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);

'use client';

import OrganizationAvatar from '@/components/avatars/organization-avatar';
import { LeadIconTypography, SmallIconTypography } from '@/components/commons';
import { getOrganizationBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { PaletteModeContext } from '@/libs/providers';
import { coal, emerald } from '@/libs/theme';
import type { pageOrganizationSsoSignin_rootQuery } from '@/queries/__generated__/pageOrganizationSsoSignin_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import { nanoid } from 'nanoid';
import { useParams, useSearchParams } from 'next/navigation';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageOrganizationSsoSignin_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query pageOrganizationSsoSignin_rootQuery($organizationId: String!, $redirectUrl: String!) {
    organization(id: $organizationId) {
      logoUrl
      name
    }
    ssoLoginUrl(id: $organizationId, redirectUrl: $redirectUrl)
  }
`;

const OrganizationSigninPage = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationSsoSignin_rootQuery>(RootQuery, queryReference);
  const paletteMode = useContext(PaletteModeContext);

  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
      <Card sx={{ textAlign: 'center', padding: 4, borderRadius: 3, maxWidth: 400, backgroundColor: paletteMode === 'dark' ? emerald : coal }}>
        <CardContent sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center' }}>
          <OrganizationAvatar name={{ name: rootData.organization?.name }} photo={{ url: rootData.organization?.logoUrl }} />
          <LeadIconTypography label={`Single sign-on to ${rootData.organization?.name}`} invertDefaultColor sx={{ marginTop: 2 }} />
          <SmallIconTypography label={`Authenticate your account by logging into ${rootData.organization?.name}'s single sign-on provider.`} invertDefaultColor />
          <Button variant="contained" href={rootData.ssoLoginUrl} fullWidth sx={{ marginTop: 2 }}>
            Continue
          </Button>
        </CardContent>
      </Card>
    </Box>
  );
};

const MemoOrganizationSigninPage = memo(OrganizationSigninPage);

const OrganizationSigninPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationSsoSignin_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();
  const { organizationId } = useParams();
  const searchParams = useSearchParams();
  const redirectUrl = searchParams.get('redirectUrl');
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

  useEffect(() => {
    loadQuery(
      {
        organizationId: finalOrganizationId,
        redirectUrl: redirectUrl ?? getOrganizationBaseLink(finalOrganizationId),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, finalOrganizationId, redirectUrl]);

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
      <MemoOrganizationSigninPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={finalOrganizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationSigninPageWithRelay);

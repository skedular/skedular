import { OrganizationAvatar } from '@/components/avatars';
import { NewIcon } from '@/components/icons';
import { getOrganizationBaseLink, getOrganizationSetupLink } from '@/components/links';
import type { noOrganizationLandingContent_query$key } from '@/queries/__generated__/noOrganizationLandingContent_query.graphql';
import type { noOrganizationLandingPage_rootQuery } from '@/queries/__generated__/noOrganizationLandingPage_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActionArea from '@mui/material/CardActionArea';
import CardContent from '@mui/material/CardContent';
import { useIntegratedPlatform } from '@skedular/shared';
import { BodyIconTypography, LeadIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { useRouter } from 'next/navigation';
import { memo, useEffect } from 'react';
import { graphql, type PreloadedQuery, useFragment, usePreloadedQuery } from 'react-relay';

type Props = {
  queryRef: PreloadedQuery<noOrganizationLandingPage_rootQuery>;
};

export const NoOrganizationLandingPageRootQuery = graphql`
  query noOrganizationLandingPage_rootQuery {
    me {
      id
      isOnboardingDone
    }
    ...noOrganizationLandingContent_query
  }
`;

const QueryFragment = graphql`
  fragment noOrganizationLandingContent_query on Query {
    myOrganizations(types: [MARKETPLACE]) {
      name
      uniqueId
      customDomain
      logoUrl
    }
  }
`;

const hashUserId = (userId?: string | null) => {
  if (!userId) {
    return undefined;
  }

  let hash = 0;
  for (let index = 0; index < userId.length; index += 1) {
    hash = (hash << 5) - hash + userId.charCodeAt(index);
    hash |= 0;
  }

  return Math.abs(hash).toString(16);
};

const getLandingState = (orgCount: number) => {
  if (orgCount === 0) {
    return 'no-orgs';
  }

  return orgCount === 1 ? 'single-org' : 'multi-org';
};

const AddOrganizationButton = ({ href }: { href: string }) => (
  <Button href={href} variant="text" sx={{ alignSelf: 'flex-start' }}>
    <LeadIconTypography label="Add Organization" endElement={<NewIcon fontSize="large" />} />
  </Button>
);

const NoOrganizationLandingContent = ({ queryRef }: Props) => {
  const rootData = usePreloadedQuery<noOrganizationLandingPage_rootQuery>(NoOrganizationLandingPageRootQuery, queryRef);
  const queryData = useFragment<noOrganizationLandingContent_query$key>(QueryFragment, rootData);
  const { integratedPlatform } = useIntegratedPlatform();
  const router = useRouter();
  const organizations = queryData.myOrganizations;
  const organizationCount = organizations.length;
  const landingState = getLandingState(organizationCount);
  const createOrganizationLink = getOrganizationSetupLink(integratedPlatform);

  useEffect(() => {
    console.info('org_landing_state', {
      event: 'org_landing_state',
      state: landingState,
      orgCount: organizationCount,
      userId: hashUserId(rootData.me?.id),
    });
  }, [landingState, organizationCount, rootData.me?.id]);

  if (!rootData.me?.isOnboardingDone) {
    return null;
  }

  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'flex-start', width: '100%' }}>
      <StackColumn sx={{ maxWidth: 760, width: '100%', p: { xs: 2, md: 4 } }}>
        {organizationCount === 0 ? (
          <Card variant="outlined">
            <CardContent>
              <StackColumn>
                <LeadIconTypography label="Create your first workspace organization" />
                <BodyIconTypography label="Spaces is for coworking and individual organizations, locations, products, resources, bookings, and operational workflows." />
                <AddOrganizationButton href={createOrganizationLink} />
              </StackColumn>
            </CardContent>
          </Card>
        ) : (
          <StackColumn>
            <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center', gap: 2, flexWrap: 'wrap' }}>
              <LeadIconTypography label={organizationCount === 1 ? 'Select your workspace organization' : 'Select a workspace organization'} />
              <AddOrganizationButton href={createOrganizationLink} />
            </StackRow>
            {organizations.map((organization) => {
              const organizationLink = getOrganizationBaseLink(integratedPlatform, organization.customDomain ?? organization.uniqueId);

              return (
                <Card variant="outlined" key={organization.uniqueId}>
                  <CardActionArea onClick={() => router.push(organizationLink)}>
                    <CardContent>
                      <StackRow sx={{ alignItems: 'center', gap: 2 }}>
                        <OrganizationAvatar size="large" name={{ name: organization.name }} photo={{ url: organization.logoUrl }} />
                        <LeadIconTypography label={organization.name} />
                      </StackRow>
                    </CardContent>
                  </CardActionArea>
                </Card>
              );
            })}
          </StackColumn>
        )}
      </StackColumn>
    </Box>
  );
};

export default memo(NoOrganizationLandingContent);

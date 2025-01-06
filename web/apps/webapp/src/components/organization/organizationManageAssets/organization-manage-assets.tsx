import type { organizationManageAssets_deskTypes_query$key } from '@/queries/__generated__/organizationManageAssets_deskTypes_query.graphql';
import type { organizationManageAssets_deskTypes_refetchableFragment } from '@/queries/__generated__/organizationManageAssets_deskTypes_refetchableFragment.graphql';
import type { organizationManageAssets_rootQuery } from '@/queries/__generated__/organizationManageAssets_rootQuery.graphql';
import type { organizationManageAssets_zones_query$key } from '@/queries/__generated__/organizationManageAssets_zones_query.graphql';
import type { organizationManageAssets_zones_refetchableFragment } from '@/queries/__generated__/organizationManageAssets_zones_refetchableFragment.graphql';
import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import { BodyIconTypography, SectionIconTypography, StackColumn, StackColumnWithSaveExitCancelAppBar } from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { defaultPadding } from '@repo/shared/libs/theme';
import { nanoid } from 'nanoid';
import { useSearchParams } from 'next/navigation';
import { memo, useEffect, useRef, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { expandedDrawerWidthPx } from './commons';
import OrganizationManageAssetsLeftSideNavigationMenuContent from './organization-manage-assets-left-side-navigation-menu-content';

type Props = {
  queryReference: PreloadedQuery<organizationManageAssets_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationManageAssets_rootQuery($organizationId: String!, $zoneNameSearchText: String) {
    ...organizationManageAssets_zones_query
    ...organizationManageAssets_deskTypes_query
  }
`;

const OrganizationManageAssets = ({ queryReference, organizationId }: Props) => {
  const rootData = usePreloadedQuery<organizationManageAssets_rootQuery>(RootQuery, queryReference);
  const [rootDataZones, refetchZones] = useRefetchableFragment<
    organizationManageAssets_zones_refetchableFragment,
    organizationManageAssets_zones_query$key
  >(
    graphql`
      fragment organizationManageAssets_zones_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationManageAssets_zones_refetchableFragment") {
        zones(first: $count, after: $cursor, where: { organizationId: $organizationId, nameContains: $zoneNameSearchText })
          @connection(key: "organizationManageAssets_zones") {
          __id
          totalCount
          edges {
            node {
              id
              name
            }
          }
        }
      }
    `,
    rootData,
  );

  const [rootDataDeskTypes, refetchDeskTypes] = useRefetchableFragment<
    organizationManageAssets_deskTypes_refetchableFragment,
    organizationManageAssets_deskTypes_query$key
  >(
    graphql`
      fragment organizationManageAssets_deskTypes_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationManageAssets_deskTypes_refetchableFragment") {
        deskTypes(first: $count, after: $cursor, where: { organizationId: $organizationId, nameContains: $zoneNameSearchText })
          @connection(key: "organizationManageAssets_deskTypes") {
          __id
          totalCount
          edges {
            node {
              id
              name
            }
          }
        }
      }
    `,
    rootData,
  );

  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});

  useEffect(() => {
    if (!section || section === 'zones-setup') {
      return;
    }

    const element = sectionRefs.current[section];
    if (!element) {
      return;
    }

    const appBarHeight = document.querySelector('.app-bar')?.clientHeight || 0;
    const elementTop = element.getBoundingClientRect().top + window.scrollY;
    window.scrollTo({
      top: elementTop - appBarHeight,
      behavior: 'smooth',
    });
  }, [section]);

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationManageAssetsLeftSideNavigationMenuContent organizationId={organizationId} hideIcons />
        <Box sx={{ marginLeft: expandedDrawerWidthPx, flexGrow: 1 }}>
          <StackColumnWithSaveExitCancelAppBar label="Manage Assets" hideCancel hideSaveAndExit>
            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['zones-setup'] = divElement;
              }}
            >
              <SectionIconTypography label="Zones Setup" />
              <BodyIconTypography label="Edit your organization zones details" />
              <Divider />
            </StackColumn>

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['desk-types-setup'] = divElement;
              }}
            >
              <SectionIconTypography label="Desk Types Setup" />
              <BodyIconTypography label="Edit your organization desk types details" />
              <Divider />
            </StackColumn>
          </StackColumnWithSaveExitCancelAppBar>
        </Box>
      </Box>
    </>
  );
};

const MemoOrganizationManageAssets = memo(OrganizationManageAssets);

type RelayProps = {
  organizationId: string;
};

const OrganizationManageAssetsWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationManageAssets_rootQuery>(RootQuery);
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
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOrganizationManageAssets queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationManageAssetsWithRelay);

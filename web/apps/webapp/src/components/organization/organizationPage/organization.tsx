import type { organization_rootQuery } from '@/queries/__generated__/organization_rootQuery.graphql';
import Stack from '@mui/material/Stack';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import Typography from '@mui/material/Typography';
import { OrganizationAvatar } from '@repo/shared/components/avatars';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { getCurrentCompleteUrl } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { useRouter, useSearchParams } from 'next/navigation';
import { useSnackbar } from 'notistack';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import OrganizationAboutTab from './organization-about-tab';
import OrganizationAnalyticsTab from './organization-analytics-tab';
import OrganizationBillingTab from './organization-billing-tab';
import OrganizationBookingsTab from './organization-bookings-tab';
import OrganizationLocationsTab from './organization-locations-tab';
import OrganizationOfferingTab from './organization-offering-tab';
import OrganizationPeopleTab from './organization-people-tab';
import OrganizationTeamsTab from './organization-teams-tab';

type Props = {
  queryReference: PreloadedQuery<organization_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organization_rootQuery($organizationId: String!) {
    organization(id: $organizationId) {
      id
      name
      logoUrl
      canModify
      canViewAnalytics
    }
  }
`;

const Organization = ({ queryReference, onReloadRequired, organizationId }: Props) => {
  const rootData = usePreloadedQuery<organization_rootQuery>(RootQuery, queryReference);
  const searchParams = useSearchParams();
  const tab = searchParams.get('tab');
  const router = useRouter();
  const addPaymentMethodStatus = searchParams.get('add-payment-method-status');
  const { enqueueSnackbar } = useSnackbar();
  let initialTabIndex = 0;

  useEffect(() => {
    if (addPaymentMethodStatus === 'failed') {
      enqueueSnackbar('Failed to add payment method', {
        variant: 'error',
        anchorOrigin,
      });
    } else if (addPaymentMethodStatus === 'added') {
    }
  }, [addPaymentMethodStatus, enqueueSnackbar]);

  if (tab === 'bookings') {
    initialTabIndex = 0;
  } else if (tab === 'about') {
    initialTabIndex = 1;
  } else if (tab === 'people') {
    initialTabIndex = 2;
  } else if (tab === 'locations') {
    initialTabIndex = 3;
  } else if (tab === 'teams') {
    initialTabIndex = 4;
  } else if (tab === 'offering') {
    initialTabIndex = 5;
  } else if (tab === 'billing') {
    initialTabIndex = 6;
  } else if (tab === 'analytics') {
    initialTabIndex = 7;
  }

  const [tabIndex, setTabIndex] = useState(initialTabIndex);

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabIndex(newValue);

    let tab = '';

    if (newValue === 0) {
      tab = 'bookings';
    } else if (newValue === 1) {
      tab = 'about';
    } else if (newValue === 2) {
      tab = 'people';
    } else if (newValue === 3) {
      tab = 'locations';
    } else if (newValue === 4) {
      tab = 'teams';
    } else if (newValue === 5) {
      tab = 'offering';
    } else if (newValue === 6) {
      tab = 'billing';
    } else if (newValue === 7) {
      tab = 'analytics';
    }

    if (tab) {
      router.push(`${getCurrentCompleteUrl()}?tab=${tab}`);
    }
  };

  if (!rootData.organization) {
    return null;
  }

  return (
    <Stack direction="column" spacing={1}>
      <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
        <OrganizationAvatar name={{ name: rootData.organization?.name }} photo={{ url: rootData.organization?.logoUrl }} sx={{ marginBottom: 1 }} />
        <Typography variant="h6">{rootData.organization?.name}</Typography>
      </Stack>

      <Tabs value={tabIndex} onChange={handleTabChange}>
        <Tab label="Bookings" />
        <Tab label="About" />
        <Tab label="People" />
        <Tab label="Locations" />
        <Tab label="Teams" />
        {rootData.organization.canModify && <Tab label="Offering" />}
        {rootData.organization.canModify && <Tab label="Billing" />}
        {rootData.organization.canViewAnalytics && <Tab label="Analytics" />}
      </Tabs>

      <>
        {tabIndex === 0 && <OrganizationBookingsTab onReloadRequired={onReloadRequired} organizationId={organizationId} />}
        {tabIndex === 1 && <OrganizationAboutTab onReloadRequired={onReloadRequired} organizationId={organizationId} />}
        {tabIndex === 2 && <OrganizationPeopleTab onReloadRequired={onReloadRequired} organizationId={organizationId} />}
        {tabIndex === 3 && <OrganizationLocationsTab onReloadRequired={onReloadRequired} organizationId={organizationId} />}
        {tabIndex === 4 && <OrganizationTeamsTab onReloadRequired={onReloadRequired} organizationId={organizationId} />}
        {tabIndex === 5 && rootData.organization.canModify && (
          <OrganizationOfferingTab onReloadRequired={onReloadRequired} organizationId={organizationId} />
        )}
        {tabIndex === 6 && rootData.organization.canModify && (
          <OrganizationBillingTab onReloadRequired={onReloadRequired} organizationId={organizationId} />
        )}
        {tabIndex === 7 && rootData.organization.canViewAnalytics && (
          <OrganizationAnalyticsTab onReloadRequired={onReloadRequired} organizationId={organizationId} />
        )}
      </>
    </Stack>
  );
};

const MemoOrganization = memo(Organization);

type RelayProps = {
  organizationId: string;
};

const OrganizationWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organization_rootQuery>(RootQuery);
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
      <MemoOrganization queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationWithRelay);

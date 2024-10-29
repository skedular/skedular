import { Bookings } from '@/components/booking/bookingsPage';
import { getOrganizationBaseLink } from '@/components/organization';
import type { organization_rootQuery } from '@/queries/__generated__/organization_rootQuery.graphql';
import Stack from '@mui/material/Stack';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import Typography from '@mui/material/Typography';
import { OrganizationAvatar } from '@repo/shared/components/avatars';
import { Loading } from '@repo/shared/components/loading';
import { NotificationContent, errorNotificationOptions } from '@repo/shared/components/notification';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { PaletteModeContext, UpdateBreadcrumpsContext } from '@repo/shared/libs/providers';
import { getCurrentCompleteUrl } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import OrganizationAboutTab from './organization-about-tab';
import OrganizationAnalyticsTab from './organization-analytics-tab';
import OrganizationBillingTab from './organization-billing-tab';
import OrganizationLocationsTab from './organization-locations-tab';
import OrganizationMembersTab from './organization-members-tab';
import OrganizationOfferingTab from './organization-offering-tab';
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
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const searchParams = useSearchParams();
  const tab = searchParams.get('tab');
  const router = useRouter();
  const addPaymentMethodStatus = searchParams.get('add-payment-method-status');
  const updateBreadcrumps = useContext(UpdateBreadcrumpsContext);
  let initialTabIndex = 0;

  useEffect(() => {
    if (addPaymentMethodStatus === 'failed') {
      themedToast(<NotificationContent content={`Failed to add payment method`} />, errorNotificationOptions);
    } else if (addPaymentMethodStatus === 'added') {
    }
  }, [addPaymentMethodStatus, themedToast]);

  useEffect(() => {
    if (!rootData.organization) {
      return;
    }

    updateBreadcrumps(new Map([[getOrganizationBaseLink(rootData.organization.id), rootData.organization?.name!]]));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rootData.organization]);

  if (tab === 'bookings') {
    initialTabIndex = 0;
  } else if (tab === 'about') {
    initialTabIndex = 1;
  } else if (tab === 'members') {
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

  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    setTabIndex(newValue);

    let tab = '';

    if (newValue === 0) {
      tab = 'bookings';
    } else if (newValue === 1) {
      tab = 'about';
    } else if (newValue === 2) {
      tab = 'members';
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
    <>
      <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
        <OrganizationAvatar name={{ name: rootData.organization?.name }} photo={{ url: rootData.organization?.logoUrl }} sx={{ marginBottom: 1 }} />
        <Typography variant="h6">{rootData.organization?.name}</Typography>
      </Stack>

      <Tabs value={tabIndex} onChange={handleTabChange}>
        <Tab label="Bookings" />
        <Tab label="About" />
        <Tab label="Members" />
        <Tab label="Locations" />
        <Tab label="Teams" />
        {rootData.organization.canModify && <Tab label="Offering" />}
        {rootData.organization.canModify && <Tab label="Billing" />}
        {rootData.organization.canViewAnalytics && <Tab label="Analytics" />}
      </Tabs>

      {tabIndex === 0 && <Bookings onReloadRequired={onReloadRequired} organizationId={organizationId} />}
      {tabIndex === 1 && <OrganizationAboutTab onReloadRequired={onReloadRequired} organizationId={organizationId} />}
      {tabIndex === 2 && <OrganizationMembersTab onReloadRequired={onReloadRequired} organizationId={organizationId} />}
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

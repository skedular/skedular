import { OldBookings } from '@/components/booking';
import type { oldOrganization_rootQuery } from '@/queries/__generated__/oldOrganization_rootQuery.graphql';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import { OrganizationAvatar } from '@repo/shared/components/avatars';
import { LeadIconTypography } from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import { NotificationContent, errorNotificationOptions, successNotificationOptions } from '@repo/shared/components/notification';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { PaletteModeContext } from '@repo/shared/libs/providers';
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
import OrganizationCustomTagsTab from './organization-custom-tags-tab';
import OrganizationLocationsTab from './organization-locations-tab';
import OrganizationMembersTab from './organization-members-tab';
import OrganizationOfferingTab from './organization-offering-tab';
import OrganizationTeamsTab from './organization-teams-tab';
import OrganizationZonesTab from './organization-zones-tab';

type Props = {
  queryReference: PreloadedQuery<oldOrganization_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query oldOrganization_rootQuery($organizationId: String!) {
    organization(id: $organizationId) {
      id
      name
      logoUrl
      canModify
      canViewAnalytics
    }
  }
`;

const OldOrganization = ({ queryReference, onReloadRequired, organizationId }: Props) => {
  const rootData = usePreloadedQuery<oldOrganization_rootQuery>(RootQuery, queryReference);
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const searchParams = useSearchParams();
  const tab = searchParams.get('tab');
  const router = useRouter();
  const addPaymentMethodStatus = searchParams.get('add-payment-method-status');
  let initialTabIndex = 0;

  useEffect(() => {
    if (addPaymentMethodStatus === 'failed') {
      themedToast(<NotificationContent content={`Failed to add payment method`} />, errorNotificationOptions);
    } else if (addPaymentMethodStatus === 'added') {
      themedToast(<NotificationContent content={`Payment method added.`} />, successNotificationOptions);
    }
  }, [addPaymentMethodStatus, themedToast]);

  let tabCount = 0;
  const bookingTabIndex = tabCount++;
  const aboutTabIndex = tabCount++;
  const membersTabIndex = tabCount++;
  const locationTabIndex = tabCount++;
  const teamTabIndex = tabCount++;
  const customTagsTabIndex = tabCount++;
  const zonesTabIndex = tabCount++;
  const offeringTabIndex = rootData.organization?.canModify ? tabCount++ : -1;
  const billingTabIndex = rootData.organization?.canModify ? tabCount++ : -1;
  const analyticsTabIndex = rootData.organization?.canViewAnalytics ? tabCount++ : -1;

  if (tab === 'bookings') {
    initialTabIndex = bookingTabIndex;
  } else if (tab === 'about') {
    initialTabIndex = aboutTabIndex;
  } else if (tab === 'members') {
    initialTabIndex = membersTabIndex;
  } else if (tab === 'locations') {
    initialTabIndex = locationTabIndex;
  } else if (tab === 'teams') {
    initialTabIndex = teamTabIndex;
  } else if (tab === 'customTags') {
    initialTabIndex = customTagsTabIndex;
  } else if (tab === 'zones') {
    initialTabIndex = zonesTabIndex;
  } else if (tab === 'offering') {
    initialTabIndex = offeringTabIndex;
  } else if (tab === 'billing') {
    initialTabIndex = billingTabIndex;
  } else if (tab === 'analytics') {
    initialTabIndex = analyticsTabIndex;
  }

  const [tabIndex, setTabIndex] = useState(initialTabIndex);

  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    setTabIndex(newValue);

    let tab = '';

    if (newValue === bookingTabIndex) {
      tab = 'bookings';
    } else if (newValue === aboutTabIndex) {
      tab = 'about';
    } else if (newValue === membersTabIndex) {
      tab = 'members';
    } else if (newValue === locationTabIndex) {
      tab = 'locations';
    } else if (newValue === teamTabIndex) {
      tab = 'teams';
    } else if (newValue === customTagsTabIndex) {
      tab = 'customTags';
    } else if (newValue === zonesTabIndex) {
      tab = 'zones';
    } else if (newValue === offeringTabIndex) {
      tab = 'offering';
    } else if (newValue === billingTabIndex) {
      tab = 'billing';
    } else if (newValue === analyticsTabIndex) {
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
      <LeadIconTypography
        label={rootData.organization?.name}
        startElement={
          <OrganizationAvatar name={{ name: rootData.organization?.name }} photo={{ url: rootData.organization?.logoUrl }} sx={{ marginBottom: 1 }} />
        }
      />

      <Tabs value={tabIndex} onChange={handleTabChange}>
        <Tab label="Bookings" />
        <Tab label="About" />
        <Tab label="Members" />
        <Tab label="Locations" />
        <Tab label="Teams" />
        <Tab label="Tags" />
        <Tab label="Zones" />
        {rootData.organization.canModify && <Tab label="Offering" />}
        {rootData.organization.canModify && <Tab label="Billing" />}
        {rootData.organization.canViewAnalytics && <Tab label="Analytics" />}
      </Tabs>

      {tabIndex === bookingTabIndex && <OldBookings onReloadRequired={onReloadRequired} organizationId={organizationId} />}
      {tabIndex === aboutTabIndex && <OrganizationAboutTab onReloadRequired={onReloadRequired} organizationId={organizationId} />}
      {tabIndex === membersTabIndex && <OrganizationMembersTab onReloadRequired={onReloadRequired} organizationId={organizationId} />}
      {tabIndex === locationTabIndex && <OrganizationLocationsTab onReloadRequired={onReloadRequired} organizationId={organizationId} />}
      {tabIndex === teamTabIndex && <OrganizationTeamsTab onReloadRequired={onReloadRequired} organizationId={organizationId} />}
      {tabIndex === customTagsTabIndex && <OrganizationCustomTagsTab onReloadRequired={onReloadRequired} organizationId={organizationId} />}
      {tabIndex === zonesTabIndex && <OrganizationZonesTab onReloadRequired={onReloadRequired} organizationId={organizationId} />}
      {tabIndex === offeringTabIndex && rootData.organization.canModify && (
        <OrganizationOfferingTab onReloadRequired={onReloadRequired} organizationId={organizationId} />
      )}
      {tabIndex === billingTabIndex && rootData.organization.canModify && (
        <OrganizationBillingTab onReloadRequired={onReloadRequired} organizationId={organizationId} />
      )}
      {tabIndex === analyticsTabIndex && rootData.organization.canViewAnalytics && (
        <OrganizationAnalyticsTab onReloadRequired={onReloadRequired} organizationId={organizationId} />
      )}
    </>
  );
};

const MemoOldOrganization = memo(OldOrganization);

type RelayProps = {
  organizationId: string;
};

const OldOrganizationWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<oldOrganization_rootQuery>(RootQuery);
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
      <MemoOldOrganization queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(OldOrganizationWithRelay);

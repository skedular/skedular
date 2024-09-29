import type { organizationPageQuery } from '@/queries/__generated__/organizationPageQuery.graphql';
import type { organizationPage_query$key } from '@/queries/__generated__/organizationPage_query.graphql';
import Stack from '@mui/material/Stack';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import Typography from '@mui/material/Typography';
import { OrganizationAvatar } from '@repo/shared/components/avatars';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { getCurrentCompleteUrl } from '@repo/shared/libs/utils';
import { useRouter, useSearchParams } from 'next/navigation';
import { useSnackbar } from 'notistack';
import { memo, useEffect, useState, useTransition } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';
import OrganizationAboutTab from './organization-about-tab';
import OrganizationAnalyticsTab from './organization-analytics-tab';
import OrganizationBillingTab from './organization-billing-tab';
import OrganizationBookingsTab from './organization-bookings-tab';
import OrganizationLocationsTab from './organization-locations-tab';
import OrganizationOfferingTab from './organization-offering-tab';
import OrganizationPeopleTab from './organization-people-tab';
import OrganizationTeamsTab from './organization-teams-tab';

type Props = {
  rootDataRelay: organizationPage_query$key;
  organizationId: string;
};

const Organization = ({ rootDataRelay, organizationId }: Props) => {
  const [rootData, refetch] = useRefetchableFragment<organizationPageQuery, organizationPage_query$key>(
    graphql`
      fragment organizationPage_query on Query @refetchable(queryName: "organizationPageQuery") {
        organization(id: $organizationId) {
          id
          name
          logoUrl
          canModify
          canViewAnalytics
        }
        ...organizationAboutTab_query
        ...organizationBookingsTab_query
        ...organizationMultipleChoicesIndustries_query
        ...organizationPeopleTab_query
        ...organizationLocationsTab_query
        ...organizationTeamsTab_query
        ...organizationBillingTab_query
        ...organizationOfferingTab_query
      }
    `,
    rootDataRelay,
  );

  const [, startTransition] = useTransition();
  const searchParams = useSearchParams();
  const tab = searchParams.get('tab');
  const router = useRouter();
  const addPaymentMethodStatus = searchParams.get('add-payment-method-status');
  const { enqueueSnackbar } = useSnackbar();

  useEffect(() => {
    if (addPaymentMethodStatus === 'failed') {
      enqueueSnackbar('Failed to add payment method', {
        variant: 'error',
        anchorOrigin,
      });
    } else if (addPaymentMethodStatus === 'added') {
    }
  }, [addPaymentMethodStatus, enqueueSnackbar]);

  let initialTabIndex = 0;

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

  const handleRefetch = () => {
    startTransition(() => {
      refetch({ organizationId: rootData.organization?.id }, { fetchPolicy: 'store-and-network' });
    });
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
        {tabIndex === 0 && <OrganizationBookingsTab rootDataRelay={rootData} organizationId={organizationId} />}
        {tabIndex === 1 && <OrganizationAboutTab rootDataRelay={rootData} />}
        {tabIndex === 2 && <OrganizationPeopleTab rootDataRelay={rootData} />}
        {tabIndex === 3 && <OrganizationLocationsTab rootDataRelay={rootData} />}
        {tabIndex === 4 && <OrganizationTeamsTab rootDataRelay={rootData} />}
        {tabIndex === 5 && rootData.organization.canModify && <OrganizationOfferingTab rootDataRelay={rootData} onRefetchRequired={handleRefetch} />}
        {tabIndex === 6 && rootData.organization.canModify && <OrganizationBillingTab rootDataRelay={rootData} onRefetchRequired={handleRefetch} />}
        {tabIndex === 7 && rootData.organization.canViewAnalytics && <OrganizationAnalyticsTab organizationId={organizationId} />}
      </>
    </Stack>
  );
};

export default memo(Organization);

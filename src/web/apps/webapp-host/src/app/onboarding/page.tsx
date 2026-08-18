'use client';

import ApartmentIcon from '@mui/icons-material/Apartment';
import { NotificationContent } from '@/components/notification';
import type { pageHostOnboardingMutation } from '@/queries/__generated__/pageHostOnboardingMutation.graphql';
import type { pageHostOnboardingQuery } from '@/queries/__generated__/pageHostOnboardingQuery.graphql';
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Stack from '@mui/material/Stack';
import TextField from '@mui/material/TextField';
import { getRelayErrorMessage } from '@skedular/shared';
import { BodyIconTypography, MediumHeadingIconTypography, SetupFeatureCard, SetupSplitLayout } from '@skedular/ui';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { graphql, useLazyLoadQuery, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

const OnboardingPage = () => {
  const router = useRouter();
  const data = useLazyLoadQuery<pageHostOnboardingQuery>(
    graphql`
      query pageHostOnboardingQuery {
        activeOrganizationTermsOfUse {
          id
        }
        myOrganizations(types: [HOST]) {
          uniqueId
        }
      }
    `,
    {},
  );
  const [commit, inFlight] = useMutation<pageHostOnboardingMutation>(graphql`
    mutation pageHostOnboardingMutation($input: AddOrganizationInput!) {
      addOrganization(input: $input) {
        organization {
          id
          name
          customDomain
        }
      }
    }
  `);
  const [name, setName] = useState('');
  const [customDomain, setCustomDomain] = useState('');

  useEffect(() => {
    if (data.myOrganizations.length > 0) router.replace('/dashboard');
  }, [data.myOrganizations.length, router]);

  if (data.myOrganizations.length > 0) return null;

  const submit = () => {
    const normalizedName = name.trim();
    if (normalizedName.length < 3) {
      toast.error(<NotificationContent content="Organization name must be at least three characters long." />);
      return;
    }

    commit({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: uuid(),
          name: normalizedName,
          customDomain: customDomain.trim() || null,
          type: 'HOST',
          agreedToTermsOfUse: true,
          termsOfUseId: data.activeOrganizationTermsOfUse.id,
          industrySubCategoryIds: [],
          refundNotificationEmails: [],
          billingCycle: 'MONTHLY',
          invoiceDueInDays: 7,
        },
      },
      onCompleted: (_, errors) => {
        if (errors?.length) {
          toast.error(<NotificationContent content={`We couldn't create your Host organization. ${getRelayErrorMessage(errors)}`} />);
          return;
        }
        router.push('/locations/create');
      },
      onError: (error) => {
        toast.error(<NotificationContent content={`We couldn't create your Host organization. ${error.message}`} />);
      },
    });
  };

  return (
    <SetupSplitLayout
      asideTitle="Set up Skedular Host"
      asideDescription="Create your organization, add each place once, and offer it with hourly, daily, monthly, or contract pricing."
      asideChildren={
        <>
          <SetupFeatureCard
            icon={<LocationOnIcon color="primary" />}
            title="One place, one hidden resource"
            description="Skedular creates the booking resource and product tag behind the scenes."
          />
          <SetupFeatureCard
            icon={<CalendarMonthIcon color="primary" />}
            title="Use the existing booking engine"
            description="Availability and conflicts are handled using the same reliable flow as Skedular Spaces."
          />
          <SetupFeatureCard
            icon={<ApartmentIcon color="primary" />}
            title="Host-focused administration"
            description="Manage locations and products without resource, floor-plan, or coworking-space complexity."
          />
        </>
      }
    >
      <Stack spacing={3} sx={{ maxWidth: 640 }}>
        <MediumHeadingIconTypography label="Create your Host organization" />
        <BodyIconTypography label="This is the account that owns your listings and receives booking payouts." />
        <TextField label="Organization name" value={name} onChange={(event) => setName(event.target.value)} required fullWidth />
        <TextField
          label="Organization URL"
          value={customDomain}
          onChange={(event) => setCustomDomain(event.target.value.toLowerCase().replace(/[^a-z0-9-]/g, ''))}
          helperText="Optional. Use letters, numbers, and hyphens."
          fullWidth
        />
        <Button variant="contained" size="large" disabled={inFlight} onClick={submit} startIcon={inFlight ? <CircularProgress size={18} /> : undefined}>
          Create organization
        </Button>
      </Stack>
    </SetupSplitLayout>
  );
};

export default OnboardingPage;

import { RelayError, endOfWeek, startOfDay, startOfWeek, toRootError } from '@skedular/shared';
import { NewBookingButton } from '@/components/booking/addBooking';
import { Bookings } from '@/components/booking/bookings';
import { GettingStarted } from '@/components/gettingStarted';
import { GridContainer, StackColumn } from '@skedular/ui';
import { WeekRangePicker } from '@/components/datePickers';
import { Loading } from '@/components/loading';
import { LocationSelector } from '@/components/location/locationSelector';
import { OrganizationUserSelector } from '@/components/organization/organizationUserSelector';
import OperatorMarketplaceBookingDialog from '@/components/booking/operator-marketplace-booking-dialog';
import Button from '@mui/material/Button';

import type { organizationBookings_rootQuery } from '@/queries/__generated__/organizationBookings_rootQuery.graphql';
import Box from '@mui/system/Box';
import { Dayjs } from 'dayjs';
import { useSearchParams } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<organizationBookings_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  customerId?: string | null;
  locationId?: string | null;
  defaultStartWeek: Dayjs;
};

const RootQuery = graphql`
  query organizationBookings_rootQuery(
    $organizationCustomDomain: String!
    $customerId: String!
    $locationIds: [String!]!
    $customerIds: [String!]!
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaTo: DateTime!
    $locationsSortingValues: [LocationOrderInput!]
    $peopleNameSearchText: String
    $organizationMembersSortingValues: [OrganizationMemberOrderInput!]
  ) {
    organization(customDomain: $organizationCustomDomain) {
      id
      name
    }
    products(where: { organizationCustomDomains: [$organizationCustomDomain], includeInactive: false }) {
      edges {
        node {
          id
          latestProductVersionId
          listingMetadata {
            title
          }
          pricingOptions {
            id
            listingMetadata {
              title
            }
            fulfillmentType
          }
        }
      }
    }
    entitlementsByCustomer(customerId: $customerId) {
      id
      pricingId
      availableQuantity
      expiresAt
    }
    marketplaceBookingSubscriptionCancellationModes {
      type
      name
    }
    marketplaceBookingSubscriptions(first: 100, where: { organizationCustomDomain: $organizationCustomDomain }) {
      edges {
        node {
          id
          recurringBookings {
            id
          }
        }
      }
    }
    myLocations(organizationCustomDomain: $organizationCustomDomain) {
      id
      name
      organization {
        id
        name
      }
    }
    ...organizationUserSelector_organizationMembers_query
    ...locationSelector_allLocations_query
    ...gettingStarted_query
    ...bookings_query
    ...bookings_bookings_query
  }
`;

const OrganizationBookings = ({ queryReference, onReloadRequired, organizationCustomDomain, customerId, locationId, defaultStartWeek }: Props) => {
  const rootData = usePreloadedQuery<organizationBookings_rootQuery>(RootQuery, queryReference);
  const [today] = useState(startOfDay());
  const [startWeek, setStartWeek] = useState(defaultStartWeek);
  const [endWeek, setEndWeek] = useState(endOfWeek(defaultStartWeek).add(-1, 'milliseconds'));
  const [customerIds, setCustomerIds] = useState<string[]>(customerId ? [customerId] : []);
  const [locationIds, setLocationIds] = useState<string[]>(locationId ? [locationId] : []);
  const [operatorDialogOpen, setOperatorDialogOpen] = useState(false);

  const handleWeehChanged = (date: Dayjs) => {
    setStartWeek(date);
    setEndWeek(endOfWeek(date).add(-1, 'milliseconds'));
  };

  const handlCustomerChanged = (id?: string) => {
    setCustomerIds(id ? [id] : []);
  };

  const handlLocationChanged = (id?: string) => {
    setLocationIds(id ? [id] : []);
  };

  if (!rootData.myLocations) {
    return null;
  }

  if (!organizationCustomDomain) {
    return null;
  }

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center' }}>
      <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', pt: { xs: 1, sm: 1, md: 2 } }} spacing={2}>
        <GettingStarted rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} />
        <Bookings
          rootDataRelay={rootData}
          rootDataBookingRelay={rootData}
          organizationCustomDomain={organizationCustomDomain}
          from={startWeek}
          to={endWeek}
          locationIds={locationIds}
          customerIds={customerIds}
          toolbar={
            <GridContainer spacing={1}>
              <OrganizationUserSelector rootDataOrganizationMembersRelay={rootData} onChange={handlCustomerChanged} defaultValue={customerId} />
              <LocationSelector rootDataRelay={rootData} onChange={handlLocationChanged} defaultValue={locationId} />
              <WeekRangePicker defaultStartWeek={startWeek} onWeekChanged={handleWeehChanged} />
              {customerId && (
                <Button variant="contained" onClick={() => setOperatorDialogOpen(true)}>
                  Marketplace booking
                </Button>
              )}
            </GridContainer>
          }
          hasTopInset={false}
          actions={
            <>
              <NewBookingButton onReloadRequired={onReloadRequired} defaultDate={today} organizationCustomDomain={organizationCustomDomain} />
              {customerId && (
                <OperatorMarketplaceBookingDialog
                  open={operatorDialogOpen}
                  organizationCustomDomain={organizationCustomDomain}
                  customerId={customerId}
                  products={rootData.products.edges.map(({ node: product }) => ({
                    id: product.id,
                    latestProductVersionId: product.latestProductVersionId,
                    title: product.listingMetadata?.title,
                    pricingOptions: product.pricingOptions.map((pricing) => ({ id: pricing.id, title: pricing.listingMetadata?.title, fulfillmentType: pricing.fulfillmentType })),
                  }))}
                  entitlements={rootData.entitlementsByCustomer}
                  onClose={() => setOperatorDialogOpen(false)}
                  onCompleted={() => {
                    setOperatorDialogOpen(false);
                    onReloadRequired();
                  }}
                />
              )}
            </>
          }
        />
      </StackColumn>
    </Box>
  );
};

const MemoOrganizationBookings = memo(OrganizationBookings);

type RelayProps = {
  organizationCustomDomain: string;
  customerId?: string | null;
  locationId?: string | null;
};

const ModernOrganizationWithRelay = ({ organizationCustomDomain }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationBookings_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const [, startTransition] = useTransition();
  const [startWeek] = useState(startOfWeek());
  const searchParams = useSearchParams();
  const customerId = searchParams.get('customerId');
  const locationId = searchParams.get('locationId');

  useEffect(() => {
    const bookingsSearchCriteriaFrom = startWeek.toISOString();
    const bookingsSearchCriteriaTo = endOfWeek(startWeek).add(-1, 'milliseconds').toISOString();

    loadQuery(
      {
        organizationCustomDomain,
        customerId: customerId ?? '',
        bookingsSearchCriteriaFrom,
        bookingsSearchCriteriaTo,
        locationIds: locationId ? [locationId] : [],
        customerIds: customerId ? [customerId] : [],
        locationsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        organizationMembersSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload, startWeek, organizationCustomDomain, locationId, customerId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReload(triggerReload + 1);
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoOrganizationBookings
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationCustomDomain={organizationCustomDomain}
        customerId={customerId}
        locationId={locationId}
        defaultStartWeek={startWeek}
      />
    </ErrorBoundary>
  );
};

export default memo(ModernOrganizationWithRelay);

import { getMarketplaceBookingDetailsLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { NotificationContent } from '@/components/notification';
import { isDateAvailableForPrice } from '@/components/marketplaceProduct/available-days';
import { SmallResourceSelectionHelp } from '@/components/marketplaceProductBooking/modify-marketplace-booking-dialog';
import type { entitlementBookingPage_addMarketplaceBookingMutation } from '@/queries/__generated__/entitlementBookingPage_addMarketplaceBookingMutation.graphql';
import type { entitlementBookingPage_availableResourcesQuery } from '@/queries/__generated__/entitlementBookingPage_availableResourcesQuery.graphql';
import type { entitlementBookingPage_rootQuery } from '@/queries/__generated__/entitlementBookingPage_rootQuery.graphql';
import Alert from '@mui/material/Alert';
import Autocomplete from '@mui/material/Autocomplete';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import TextField from '@mui/material/TextField';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { TimeRangePicker } from '@mui/x-date-pickers-pro/TimeRangePicker';
import type { DateRange } from '@mui/x-date-pickers-pro/models';
import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@skedular/ui';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import type { Dayjs } from 'dayjs';
import { useRouter } from 'next/navigation';
import { memo, useMemo, useState } from 'react';
import { graphql, useLazyLoadQuery, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

dayjs.extend(utc);

type Props = { entitlementId: string };
type ResourceOption = { id: string; name: string; available: boolean };
type LocationOption = { id: string; name: string };

const RootQuery = graphql`
  query entitlementBookingPage_rootQuery($entitlementId: String!) {
    me {
      id
    }
    entitlement(id: $entitlementId) {
      id
      productId
      pricingId
      organizationCustomDomain
      availableQuantity
      grantedQuantity
      activatesAt
      expiresAt
      status
      restrictions {
        productVersionId
        availableDays
        minDurationMinutes
        maxDurationMinutes
        numberOfResourcesToBook
      }
    }
  }
`;

const AvailableResourcesQuery = graphql`
  query entitlementBookingPage_availableResourcesQuery($organizationCustomDomain: String!, $productId: String!, $locationId: String, $from: DateTime!, $until: DateTime!) {
    marketplaceLocations(where: { productIds: [$productId] }) {
      edges {
        node {
          id
          name
        }
      }
    }
    availableResources(where: { organizationCustomDomain: $organizationCustomDomain, productId: $productId, locationId: $locationId, from: $from, until: $until }) {
      resource {
        id
        name
      }
      location {
        uniqueId
        name
      }
    }
  }
`;

const EntitlementBookingPage = ({ entitlementId }: Props) => {
  const data = useLazyLoadQuery<entitlementBookingPage_rootQuery>(RootQuery, { entitlementId }, { fetchPolicy: 'network-only' });
  const router = useRouter();
  const entitlement = data.entitlement;
  if (!entitlement || !data.me) return <Loading />;

  return <EntitlementBookingForm entitlement={entitlement} meId={data.me.id} router={router} />;
};

type EntitlementBookingFormProps = {
  entitlement: NonNullable<entitlementBookingPage_rootQuery['response']['entitlement']>;
  meId: string;
  router: ReturnType<typeof useRouter>;
};

const EntitlementBookingForm = ({ entitlement, meId, router }: EntitlementBookingFormProps) => {
  const entitlementStartDate = dayjs.utc(entitlement.activatesAt).startOf('day');
  const entitlementExpiresAt = dayjs.utc(entitlement.expiresAt);
  const entitlementLastBookableDate = entitlementExpiresAt.subtract(1, 'millisecond').startOf('day');
  const [selectedDate, setSelectedDate] = useState<Dayjs | null>(() => {
    const today = dayjs.utc().startOf('day');
    if (entitlementStartDate.isAfter(today)) return entitlementStartDate;
    return entitlementLastBookableDate.isBefore(today) ? entitlementLastBookableDate : today;
  });
  const [timeRange, setTimeRange] = useState<DateRange<Dayjs>>([dayjs.utc().hour(9).minute(0).second(0), dayjs.utc().hour(10).minute(0).second(0)]);
  const [selectedLocationId, setSelectedLocationId] = useState<string>();
  const [notes, setNotes] = useState('');
  const [selectedResourceIds, setSelectedResourceIds] = useState<string[]>([]);
  const [commitBooking, isInFlight] = useMutation<entitlementBookingPage_addMarketplaceBookingMutation>(graphql`
    mutation entitlementBookingPage_addMarketplaceBookingMutation($input: AddMarketplaceBookingInput!) {
      addMarketplaceBooking(input: $input) {
        booking {
          id
        }
      }
    }
  `);
  const bookingDate = selectedDate?.utc(true);
  const range = useMemo(() => {
    const [startTime, endTime] = timeRange;
    const from = bookingDate && startTime ? bookingDate.hour(startTime.hour()).minute(startTime.minute()).second(0) : dayjs.utc('invalid');
    const until = bookingDate && endTime ? bookingDate.hour(endTime.hour()).minute(endTime.minute()).second(0) : dayjs.utc('invalid');
    return { from, until, valid: from.isValid() && until.isValid() && until.isAfter(from) };
  }, [bookingDate, timeRange]);

  const resources = useLazyLoadQuery<entitlementBookingPage_availableResourcesQuery>(
    AvailableResourcesQuery,
    {
      organizationCustomDomain: entitlement.organizationCustomDomain,
      productId: entitlement.productId,
      locationId: selectedLocationId ?? null,
      from: range.from.toISOString(),
      until: range.until.toISOString(),
    },
    { fetchPolicy: 'network-only' },
  );

  const restrictions = entitlement.restrictions;
  const maximumResourceCount = restrictions?.numberOfResourcesToBook ?? 1;
  const hasRestrictions = restrictions != null;
  const selectedDateAvailable =
    hasRestrictions &&
    selectedDate &&
    !selectedDate.isBefore(entitlementStartDate, 'day') &&
    !selectedDate.isAfter(entitlementLastBookableDate, 'day') &&
    isDateAvailableForPrice(selectedDate, restrictions.availableDays);
  const resourceOptions = useMemo<ReadonlyArray<ResourceOption>>(
    () =>
      selectedDateAvailable && range.valid
        ? resources.availableResources.map(({ resource }) => ({
            ...resource,
            available: true,
          }))
        : [],
    [range.valid, resources.availableResources, selectedDateAvailable],
  );
  const filterResource = createFilterOptions<ResourceOption>();
  const locations = useMemo<ReadonlyArray<LocationOption>>(
    () => (selectedDateAvailable && range.valid ? resources.marketplaceLocations.edges.map(({ node }) => node) : []),
    [range.valid, resources.marketplaceLocations.edges, selectedDateAvailable],
  );
  const filterLocation = createFilterOptions<LocationOption>();

  const canBook =
    hasRestrictions &&
    entitlement.status === 'ACTIVE' &&
    entitlement.availableQuantity > 0 &&
    selectedDateAvailable &&
    range.valid &&
    !range.from.isBefore(entitlementStartDate) &&
    !range.until.isAfter(entitlementExpiresAt) &&
    entitlementExpiresAt.isAfter(dayjs.utc());
  const submit = () => {
    if (!canBook || !restrictions) return;
    commitBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: uuid(),
          customerIds: [meId],
          from: range.from.toISOString(),
          until: range.until.toISOString(),
          organizationCustomDomains: [entitlement.organizationCustomDomain],
          organizationIds: [],
          teamIds: [],
          resourceIds: selectedResourceIds,
          category: 'WORKING_FROM_COWORKING_SPACE',
          paymentMethod: 'CARD',
          invoiceEmailList: [],
          quantity: 1,
          productVersionId: restrictions.productVersionId,
          pricingId: entitlement.pricingId,
          entitlementId: entitlement.id,
          checkoutReturnUrl: null,
          notes,
        },
      },
      onCompleted: (response) => {
        const bookingId = response.addMarketplaceBooking.booking?.id;
        if (bookingId) router.push(getMarketplaceBookingDetailsLink(undefined, true, '', bookingId));
      },
      onError: (error) => {
        toast.error(<NotificationContent content={error.message} />);
      },
    });
  };

  return (
    <Box sx={{ maxWidth: 820, mx: 'auto', py: { xs: 3, md: 5 }, px: 2 }}>
      <StackColumn spacing={2.5}>
        <div>
          <CaptionIconTypography label="Your entitlement" sx={{ textTransform: 'uppercase', opacity: 0.7 }} />
          <LeadIconTypography label="Book with credits" sx={{ mt: 0.5 }} />
          <BodyIconTypography label="This booking uses one credit from this entitlement. No payment is required." sx={{ mt: 0.75, opacity: 0.8 }} />
        </div>
        <Card>
          <CardContent>
            <StackColumn spacing={1}>
              <SubtitleIconTypography label={`${entitlement.availableQuantity} of ${entitlement.grantedQuantity} credits available`} />
              <BodyIconTypography label={`Expires ${dayjs(entitlement.expiresAt).format('MMM D, YYYY')}`} sx={{ opacity: 0.75 }} />
              {!hasRestrictions ? <Alert severity="error">This entitlement is missing its booking restrictions and cannot be used until it is refreshed.</Alert> : null}
              {entitlement.restrictions?.availableDays.length ? (
                <BodyIconTypography label={`Available on ${entitlement.restrictions.availableDays.join(', ')}`} sx={{ opacity: 0.75 }} />
              ) : null}
            </StackColumn>
          </CardContent>
        </Card>
        <Card>
          <CardContent>
            <StackColumn spacing={2}>
              <SubtitleIconTypography label="Choose a date and time" />
              <DatePicker
                label="Date"
                value={selectedDate}
                onChange={(value) =>
                  setSelectedDate(
                    value && value.isBefore(entitlementStartDate, 'day')
                      ? entitlementStartDate
                      : value && value.isAfter(entitlementLastBookableDate, 'day')
                        ? entitlementLastBookableDate
                        : value,
                  )
                }
                disablePast
                minDate={entitlementStartDate}
                maxDate={entitlementLastBookableDate}
                shouldDisableDate={(value) =>
                  !hasRestrictions ||
                  value.isBefore(entitlementStartDate, 'day') ||
                  value.isAfter(entitlementLastBookableDate, 'day') ||
                  !isDateAvailableForPrice(value, restrictions?.availableDays)
                }
                slotProps={{
                  textField: {
                    helperText: entitlement.restrictions?.availableDays.length ? `Available on ${entitlement.restrictions.availableDays.join(', ')}.` : undefined,
                  },
                }}
              />
              <TimeRangePicker value={timeRange} onChange={setTimeRange} />
              {!range.valid ? <Alert severity="warning">Choose an end time after the start time.</Alert> : null}
              {!selectedDateAvailable ? <Alert severity="warning">This entitlement cannot be used on the selected date.</Alert> : null}
              {locations.length > 0 ? (
                <Autocomplete
                  options={locations}
                  value={locations.find((location) => location.id === selectedLocationId) ?? null}
                  getOptionLabel={(location) => location.name}
                  filterOptions={filterLocation}
                  renderOption={(props, option) => (
                    <li {...props} key={option.id}>
                      {option.name}
                    </li>
                  )}
                  renderInput={(params) => <TextField {...params} label="Location" helperText="Choose a location with resources eligible for this product." />}
                  onChange={(_, location) => {
                    setSelectedLocationId(location?.id);
                    setSelectedResourceIds([]);
                  }}
                />
              ) : null}
              <StackColumn spacing={0.5}>
                <BodyIconTypography label={`Resources (${selectedResourceIds.length}/${maximumResourceCount})`} />
                <SmallResourceSelectionHelp maximumResourceCount={maximumResourceCount} />
                {resourceOptions.length > 0 ? (
                  <Autocomplete
                    multiple={maximumResourceCount !== 1}
                    options={resourceOptions}
                    value={
                      maximumResourceCount === 1
                        ? (resourceOptions.find((resource) => selectedResourceIds.includes(resource.id)) ?? null)
                        : resourceOptions.filter((resource) => selectedResourceIds.includes(resource.id))
                    }
                    getOptionLabel={(resource) => (resource.available ? resource.name : `${resource.name} (unavailable)`)}
                    getOptionDisabled={(resource) => !resource.available && !selectedResourceIds.includes(resource.id)}
                    filterOptions={filterResource}
                    renderOption={(props, option) => (
                      <li {...props} key={option.id}>
                        {option.available ? option.name : `${option.name} (unavailable)`}
                      </li>
                    )}
                    onChange={(_, selected) => {
                      const selectedResources = Array.isArray(selected) ? selected : selected ? [selected] : [];
                      setSelectedResourceIds(selectedResources.map((resource) => resource.id).slice(-maximumResourceCount));
                    }}
                    renderInput={(params) => (
                      <TextField {...params} label="Resources" helperText={`Select up to ${maximumResourceCount} resource${maximumResourceCount === 1 ? '' : 's'}.`} />
                    )}
                  />
                ) : (
                  <Alert severity="warning">No eligible resources are available for selection.</Alert>
                )}
              </StackColumn>
              <TextField label="Notes" value={notes} onChange={(event) => setNotes(event.target.value)} multiline minRows={2} fullWidth helperText="Optional notes for the host" />
              <StackRow sx={{ width: '100%', justifyContent: 'flex-end', gap: 1 }}>
                <Button variant="outlined" onClick={() => router.back()} disabled={isInFlight} sx={{ textTransform: 'none' }}>
                  Cancel
                </Button>
                <Button variant="contained" onClick={submit} disabled={!canBook || isInFlight} sx={{ textTransform: 'none' }}>
                  {isInFlight ? 'Booking...' : 'Book'}
                </Button>
              </StackRow>
            </StackColumn>
          </CardContent>
        </Card>
      </StackColumn>
    </Box>
  );
};

export default memo(EntitlementBookingPage);

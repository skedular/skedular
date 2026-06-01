import { BodyIconTypography, CaptionIconTypography, MediumHeadingIconTypography, StackRow, SubtitleIconTypography } from '@skedular/ui';
import { getMarketplaceLocationFloorPlansLink, getMarketplaceLocationLink } from '@/components/links';
import { useIntegratedPlatform } from '@skedular/shared';
import type { guestStoreFrontLocationsStrip_query$data, guestStoreFrontLocationsStrip_query$key } from '@/queries/__generated__/guestStoreFrontLocationsStrip_query.graphql';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: guestStoreFrontLocationsStrip_query$key;
  onLocationChange?: (locationId: string) => void;
};

const WEEKDAY_KEYS = ['sunday', 'monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday'] as const;

type WeekdayKey = (typeof WEEKDAY_KEYS)[number];
type LocationNode = guestStoreFrontLocationsStrip_query$data['marketplaceLocations']['edges'][number]['node'];
type DayOpeningHours = LocationNode['openingHours']['weekOpeningHours']['monday'];

const parseTimeToMinutes = (value: string | null | undefined) => {
  if (!value) {
    return null;
  }

  const parsed = /^(\d{1,2})(?::(\d{1,2}))?$/.exec(value.trim());

  if (!parsed) {
    return null;
  }

  const hour = Number(parsed[1]);
  const minute = Number(parsed[2] ?? '0');

  if (!Number.isInteger(hour) || !Number.isInteger(minute) || hour < 0 || hour > 23 || minute < 0 || minute > 59) {
    return null;
  }

  return hour * 60 + minute;
};

const formatMinutes = (minutes: number) => {
  const hour24 = Math.floor(minutes / 60);
  const minute = minutes % 60;
  const suffix = hour24 >= 12 ? 'PM' : 'AM';
  const hour12 = hour24 % 12 === 0 ? 12 : hour24 % 12;

  return `${hour12}:${minute.toString().padStart(2, '0')} ${suffix}`;
};

const getWeekdayKeyInTimezone = (timezone: string | null | undefined): WeekdayKey => {
  try {
    const weekday = new Intl.DateTimeFormat('en-US', { weekday: 'long', timeZone: timezone ?? 'UTC' }).format(new Date()).toLowerCase();

    return WEEKDAY_KEYS.includes(weekday as WeekdayKey) ? (weekday as WeekdayKey) : 'monday';
  } catch {
    return 'monday';
  }
};

const getCurrentMinuteInTimezone = (timezone: string | null | undefined) => {
  try {
    const parts = new Intl.DateTimeFormat('en-US', {
      timeZone: timezone ?? 'UTC',
      hour: '2-digit',
      minute: '2-digit',
      hour12: false,
    }).formatToParts(new Date());

    const hour = Number(parts.find((part) => part.type === 'hour')?.value ?? '0');
    const minute = Number(parts.find((part) => part.type === 'minute')?.value ?? '0');

    return hour * 60 + minute;
  } catch {
    return 0;
  }
};

const getOpenState = (timezone: string | null | undefined, dayOpeningHours: DayOpeningHours) => {
  if (dayOpeningHours.closed) {
    return {
      isOpenNow: false,
      label: 'Closed today',
    };
  }

  if (dayOpeningHours.openAllDay) {
    return {
      isOpenNow: true,
      label: 'Open 24 hours',
    };
  }

  const fromMinutes = parseTimeToMinutes(dayOpeningHours.from);
  const untilMinutes = parseTimeToMinutes(dayOpeningHours.until);

  if (fromMinutes === null || untilMinutes === null) {
    return {
      isOpenNow: false,
      label: 'Hours unavailable',
    };
  }

  const nowMinutes = getCurrentMinuteInTimezone(timezone);
  const isOvernight = untilMinutes < fromMinutes;
  const isOpenNow = isOvernight ? nowMinutes >= fromMinutes || nowMinutes < untilMinutes : nowMinutes >= fromMinutes && nowMinutes < untilMinutes;

  return {
    isOpenNow,
    label: isOpenNow ? `Open until ${formatMinutes(untilMinutes)}` : `Hours ${formatMinutes(fromMinutes)} - ${formatMinutes(untilMinutes)}`,
  };
};

const GuestStoreFrontLocationsStrip = ({ rootDataRelay, onLocationChange }: Props) => {
  const router = useRouter();
  const { integratedPlatform } = useIntegratedPlatform();
  const rootData = useFragment<guestStoreFrontLocationsStrip_query$key>(
    graphql`
      fragment guestStoreFrontLocationsStrip_query on Query {
        marketplaceLocations(where: { organizationCustomDomain: $organizationCustomDomain }) {
          totalCount
          edges {
            node {
              id
              name
              timezone
              floorPlanCount
              physicalAddress {
                formattedAddress
              }
              openingHours {
                weekOpeningHours {
                  monday {
                    closed
                    openAllDay
                    from
                    until
                  }
                  tuesday {
                    closed
                    openAllDay
                    from
                    until
                  }
                  wednesday {
                    closed
                    openAllDay
                    from
                    until
                  }
                  thursday {
                    closed
                    openAllDay
                    from
                    until
                  }
                  friday {
                    closed
                    openAllDay
                    from
                    until
                  }
                  saturday {
                    closed
                    openAllDay
                    from
                    until
                  }
                  sunday {
                    closed
                    openAllDay
                    from
                    until
                  }
                }
              }
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [selectedLocationId, setSelectedLocationId] = useState('');
  const locations = useMemo(() => rootData.marketplaceLocations.edges.map((item) => item.node), [rootData.marketplaceLocations.edges]);
  const selectedLocation = useMemo(() => locations.find((location) => location.id === selectedLocationId) ?? null, [locations, selectedLocationId]);

  useEffect(() => {
    if (!onLocationChange) {
      return;
    }

    onLocationChange(selectedLocationId);
  }, [onLocationChange, selectedLocationId]);

  if (rootData.marketplaceLocations.totalCount === 0) {
    return null;
  }

  const selectedLocationOpenState = selectedLocation
    ? getOpenState(selectedLocation.timezone, selectedLocation.openingHours.weekOpeningHours[getWeekdayKeyInTimezone(selectedLocation.timezone)])
    : null;

  return (
    <Box sx={{ mb: { xs: 3, md: 4 }, minWidth: 0 }}>
      <MediumHeadingIconTypography label="Locations" sx={{ mb: 0.75 }} />
      <BodyIconTypography label="Filter products by location." sx={{ opacity: 0.8, mb: 1.5 }} />

      <StackRow sx={{ overflowX: 'auto', pb: 0.5, flexWrap: 'nowrap', alignItems: 'center', gap: 1, scrollbarWidth: 'thin' }}>
        <Box
          component="button"
          type="button"
          onClick={() => setSelectedLocationId('')}
          sx={{
            border: 1,
            borderColor: (theme) => (!selectedLocationId ? theme.palette.primary.main : theme.palette.divider),
            backgroundColor: (theme) => (!selectedLocationId ? theme.palette.action.selected : theme.palette.background.paper),
            borderRadius: 999,
            px: 1.5,
            py: 0.9,
            cursor: 'pointer',
            flex: '0 0 auto',
            color: 'text.primary',
          }}
        >
          <SubtitleIconTypography label="All locations" />
        </Box>

        {locations.map((location) => {
          const isActive = location.id === selectedLocationId;
          const weekdayKey = getWeekdayKeyInTimezone(location.timezone);
          const openState = getOpenState(location.timezone, location.openingHours.weekOpeningHours[weekdayKey]);

          return (
            <Box
              key={location.id}
              component="button"
              type="button"
              onClick={() => setSelectedLocationId((current) => (current === location.id ? '' : location.id))}
              sx={{
                border: 1,
                borderColor: (theme) => (isActive ? theme.palette.primary.main : theme.palette.divider),
                backgroundColor: (theme) => (isActive ? theme.palette.action.selected : theme.palette.background.paper),
                borderRadius: 999,
                px: 1.5,
                py: 0.75,
                cursor: 'pointer',
                flex: '0 0 auto',
                color: 'text.primary',
              }}
            >
              <StackRow spacing={1} sx={{ flexWrap: 'nowrap', alignItems: 'center' }}>
                <SubtitleIconTypography label={location.name} />
                <Chip
                  size="small"
                  label={openState.isOpenNow ? 'Open' : 'Closed'}
                  color={openState.isOpenNow ? 'success' : 'default'}
                  variant={openState.isOpenNow ? 'filled' : 'outlined'}
                />
              </StackRow>
            </Box>
          );
        })}
      </StackRow>

      {selectedLocation && selectedLocationOpenState ? (
        <Box
          sx={{
            mt: 1.5,
            p: 1.5,
            border: 1,
            borderColor: (theme) => theme.palette.divider,
            borderRadius: 2,
            bgcolor: (theme) => theme.palette.background.paper,
          }}
        >
          <StackRow sx={{ justifyContent: 'space-between', gap: 1.5, alignItems: 'center' }}>
            <Box sx={{ minWidth: 0 }}>
              <CaptionIconTypography label={selectedLocation.physicalAddress?.formattedAddress ?? 'Address not available'} sx={{ opacity: 0.8 }} />
              <CaptionIconTypography label={selectedLocationOpenState.label} sx={{ opacity: 0.9, mt: 0.25 }} />
            </Box>
            <StackRow spacing={1} sx={{ flexWrap: 'nowrap' }}>
              {selectedLocation.floorPlanCount > 0 ? (
                <Button
                  variant="contained"
                  size="small"
                  onClick={() => router.push(getMarketplaceLocationFloorPlansLink(integratedPlatform, selectedLocation.id))}
                  sx={{ textTransform: 'none', borderRadius: 2, whiteSpace: 'nowrap' }}
                >
                  Floor plan
                </Button>
              ) : null}
              <Button
                variant="outlined"
                size="small"
                onClick={() => router.push(getMarketplaceLocationLink(integratedPlatform, selectedLocation.id))}
                sx={{ textTransform: 'none', borderRadius: 2, whiteSpace: 'nowrap' }}
              >
                Details
              </Button>
            </StackRow>
          </StackRow>
        </Box>
      ) : null}
    </Box>
  );
};

export default memo(GuestStoreFrontLocationsStrip);

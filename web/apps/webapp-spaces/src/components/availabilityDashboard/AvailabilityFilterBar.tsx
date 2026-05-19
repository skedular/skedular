'use client';

import { LocationAvatar } from '@/components/avatars';
import { DayPicker } from '@/components/datePickers';
import { DefaultSelect } from '@/components/styled';
import type { AvailabilityFilterBar_locations$key } from '@/queries/__generated__/AvailabilityFilterBar_locations.graphql';
import type { AvailabilityFilterBar_statuses$key } from '@/queries/__generated__/AvailabilityFilterBar_statuses.graphql';
import Checkbox from '@mui/material/Checkbox';
import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import Box from '@mui/system/Box';
import { BodyIconTypography, CollectionToolbar, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@skedular/ui';
import dayjs from 'dayjs';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

export type AvailabilityFilters = {
  date: string;
  locationIds?: string[];
  statuses?: string[];
};

type Props = {
  filters: AvailabilityFilters;
  locationsRef: AvailabilityFilterBar_locations$key;
  statusesRef: AvailabilityFilterBar_statuses$key;
  onChange: (filters: AvailabilityFilters) => void;
  isPending?: boolean;
};

// Inline sub-component for the location multi-select so it can use its own fragment.
const LocationFilter = memo(function LocationFilter({
  locationsRef,
  values,
  onChange,
}: {
  locationsRef: AvailabilityFilterBar_locations$key;
  values: string[];
  onChange: (ids: string[]) => void;
}) {
  const data = useFragment(
    graphql`
      fragment AvailabilityFilterBar_locations on Query {
        locations(where: { organizationCustomDomain: $organizationCustomDomain }, orderBy: $locationsSortingValues) {
          edges {
            node {
              id
              name
            }
          }
        }
      }
    `,
    locationsRef,
  );

  const locations = useMemo(() => data.locations.edges.map(({ node }) => node), [data.locations]);

  return (
    <DefaultSelect
      multiple
      value={values}
      onChange={(e) => {
        const val = e.target.value;
        onChange(typeof val === 'string' ? [val] : (val as string[]));
      }}
      size="small"
      displayEmpty
      renderValue={(selectedIds) => {
        const ids = selectedIds as string[];
        const label =
          ids.length === 0
            ? 'All'
            : ids.length === 1
              ? (locations.find((l) => l.id === ids[0])?.name ?? ids[0])
              : `${locations.find((l) => l.id === ids[0])?.name ?? ids[0]} +${ids.length - 1}`;
        return (
          <StackRow>
            <LeadIconTypography label="Location" />
            <Divider orientation="vertical" flexItem />
            <PushToRight />
            <SmallIconTypography label={label} />
          </StackRow>
        );
      }}
    >
      {locations.map((loc) => (
        <MenuItem key={loc.id} value={loc.id}>
          <Checkbox checked={values.includes(loc.id)} size="small" />
          <BodyIconTypography startElement={<LocationAvatar name={{ name: loc.name }} size="small" />} label={loc.name} />
        </MenuItem>
      ))}
    </DefaultSelect>
  );
});

// Inline sub-component for the status multi-select so it can use its own fragment.
const StatusFilter = memo(function StatusFilter({
  statusesRef,
  values,
  onChange,
}: {
  statusesRef: AvailabilityFilterBar_statuses$key;
  values: string[];
  onChange: (statuses: string[]) => void;
}) {
  const statuses = useFragment<AvailabilityFilterBar_statuses$key>(
    graphql`
      fragment AvailabilityFilterBar_statuses on ResourceAvailabilityClassificationDetails @relay(plural: true) {
        type
        name
      }
    `,
    statusesRef,
  );

  return (
    <DefaultSelect
      multiple
      value={values}
      onChange={(e) => {
        const val = e.target.value;
        onChange(typeof val === 'string' ? [val] : (val as string[]));
      }}
      size="small"
      displayEmpty
      renderValue={(selectedTypes) => {
        const types = selectedTypes as string[];
        const label =
          types.length === 0
            ? 'All'
            : types.length === 1
              ? (statuses.find((s) => s.type === types[0])?.name ?? types[0])
              : `${statuses.find((s) => s.type === types[0])?.name ?? types[0]} +${types.length - 1}`;
        return (
          <StackRow>
            <LeadIconTypography label="Status" />
            <Divider orientation="vertical" flexItem />
            <PushToRight />
            <SmallIconTypography label={label} />
          </StackRow>
        );
      }}
    >
      {statuses.map((s) => (
        <MenuItem key={s.type} value={s.type}>
          <Checkbox checked={values.includes(s.type)} size="small" />
          <BodyIconTypography label={s.name} />
        </MenuItem>
      ))}
    </DefaultSelect>
  );
});

const AvailabilityFilterBar = ({ filters, locationsRef, statusesRef, onChange, isPending }: Props) => (
  <CollectionToolbar
    filters={
      <Box aria-busy={isPending || undefined} sx={{ display: 'flex', flexWrap: 'wrap', gap: 1.5, alignItems: 'center', opacity: isPending ? 0.6 : 1, transition: 'opacity 0.2s' }}>
        <DayPicker defaultDate={dayjs(filters.date)} onDateChanged={(d) => onChange({ ...filters, date: d.format('YYYY-MM-DD') })} />
        <LocationFilter locationsRef={locationsRef} values={filters.locationIds ?? []} onChange={(locationIds) => onChange({ ...filters, locationIds })} />
        <StatusFilter statusesRef={statusesRef} values={filters.statuses ?? []} onChange={(statuses) => onChange({ ...filters, statuses })} />
      </Box>
    }
  />
);

export default memo(AvailabilityFilterBar);

import { LocationAvatar } from '@/components/avatars';
import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@/components/commons';
import { LocationIcon } from '@/components/icons';
import { DefaultSelect } from '@/components/styled';
import type { locationSelector_allLocations_query$key } from '@/queries/__generated__/locationSelector_allLocations_query.graphql';
import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import { SelectChangeEvent } from '@mui/material/Select';
import { memo, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: locationSelector_allLocations_query$key;
  onChange: (id?: string) => void;
  defaultValue?: string | null;
};

const allId = 'kkigMVsUXwi2YMSSrXv7i';

const LocationSelector = ({ rootDataRelay, onChange, defaultValue }: Props) => {
  const rootData = useFragment<locationSelector_allLocations_query$key>(
    graphql`
      fragment locationSelector_allLocations_query on Query {
        locations(where: { organizationId: $organizationId }, orderBy: $locationsSortingValues) {
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
    rootDataRelay,
  );

  const [id, setId] = useState<string>(defaultValue ?? allId);
  const allItems = useMemo(() => (rootData.locations?.edges ? rootData.locations.edges.map(({ node }) => node) : []), [rootData.locations]);

  const handleChanged = (event: SelectChangeEvent<unknown>) => {
    const id = event.target.value as string;

    setId(id);
    onChange(id === allId ? undefined : id);
  };

  return (
    <DefaultSelect
      value={id}
      onChange={handleChanged}
      size="small"
      renderValue={(selectedId) => {
        const selectedItem = allItems.find((item) => item.id === selectedId);
        if (selectedItem) {
          return (
            <StackRow>
              <LeadIconTypography label="Location" startElement={<LocationIcon />} />
              <Divider orientation="vertical" flexItem />
              <PushToRight />
              <SmallIconTypography label={selectedItem.name} />
            </StackRow>
          );
        }

        return (
          <StackRow>
            <LeadIconTypography label="Location" startElement={<LocationIcon />} />
            <Divider orientation="vertical" flexItem />
            <PushToRight />
            <SmallIconTypography label="All" />
          </StackRow>
        );
      }}
    >
      <MenuItem value={allId}>
        <BodyIconTypography label="All" />
      </MenuItem>

      {allItems.map((item) => (
        <MenuItem key={item.id} value={item.id}>
          <BodyIconTypography startElement={<LocationAvatar name={{ name: item.name }} size="small" />} label={item.name} />
        </MenuItem>
      ))}
    </DefaultSelect>
  );
};

export default memo(LocationSelector);

import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import { SelectChangeEvent } from '@mui/material/Select';
import { LocationAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography, DropdownSelect, LeadIconTypography, PushToRight, StackRow } from '@repo/shared/components/commons';
import { LocationIcon } from '@repo/shared/components/icons';
import graphql from 'babel-plugin-relay/macro';
import { memo, useMemo, useState } from 'react';
import { useFragment } from 'react-relay';
import type { locationSelector_allLocations_query$key } from './__generated__/locationSelector_allLocations_query.graphql';

type Props = {
  rootDataRelay: locationSelector_allLocations_query$key;
  onChange: (id?: string) => void;
};

const allId = 'kkigMVsUXwi2YMSSrXv7i';

const LocationSelector = ({ rootDataRelay, onChange }: Props) => {
  const rootData = useFragment<locationSelector_allLocations_query$key>(
    graphql`
      fragment locationSelector_allLocations_query on Query {
        locations(where: { organizationId: $organizationId }) {
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

  const [id, setId] = useState<string>(allId);
  const allItems = useMemo(() => (rootData.locations?.edges ? rootData.locations.edges.map(({ node }) => node) : []), [rootData.locations]);

  const handleChanged = (event: SelectChangeEvent<unknown>) => {
    const id = event.target.value as string;

    setId(id);
    onChange(id === allId ? undefined : id);
  };

  return (
    <DropdownSelect
      value={id}
      onChange={handleChanged}
      size="small"
      renderValue={(selectedId) => {
        const selectedItem = allItems.find((item) => item.id === selectedId);
        if (selectedItem) {
          return (
            <StackRow>
              <LeadIconTypography label="Location" icon={<LocationIcon />} />
              <Divider orientation="vertical" flexItem />
              <PushToRight />
              <BodyIconTypography label={selectedItem.name} />
            </StackRow>
          );
        }

        return (
          <StackRow>
            <LeadIconTypography label="Location" icon={<LocationIcon />} />
            <Divider orientation="vertical" flexItem />
            <PushToRight />
            <BodyIconTypography label="All" />
          </StackRow>
        );
      }}
    >
      <MenuItem value={allId}>
        <BodyIconTypography label="All" />
      </MenuItem>

      {allItems.map((item) => (
        <MenuItem key={item.id} value={item.id}>
          <LeadIconTypography icon={<LocationAvatar name={{ name: item.name }} size="small" />} label={item.name} />
        </MenuItem>
      ))}
    </DropdownSelect>
  );
};

export default memo(LocationSelector);

import type { zoneSelector_allZones_query$key } from '@/queries/__generated__/zoneSelector_allZones_query.graphql';
import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import { SelectChangeEvent } from '@mui/material/Select';
import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@repo/shared/components/commons';
import { ZoneIcon } from '@repo/shared/components/icons';
import { DefaultSelect } from '@repo/shared/components/styled';
import { memo, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: zoneSelector_allZones_query$key;
  onChange: (id?: string) => void;
};

const allId = 'kkigMVsUXwi2YMSSrXv7i';

const ZoneSelector = ({ rootDataRelay, onChange }: Props) => {
  const rootData = useFragment<zoneSelector_allZones_query$key>(
    graphql`
      fragment zoneSelector_allZones_query on Query {
        zones(where: { organizationId: $organizationId }, orderBy: $zonesSortingValues) {
          __id
          totalCount
          edges {
            node {
              id
              name
              color
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [id, setId] = useState<string>(allId);
  const allItems = useMemo(() => (rootData.zones?.edges ? rootData.zones.edges.map(({ node }) => node) : []), [rootData.zones]);

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
            <StackRow sx={{ alignItems: 'center' }}>
              <LeadIconTypography label="Zones" startElement={<ZoneIcon />} />
              <Divider orientation="vertical" flexItem />
              <PushToRight />
              <SmallIconTypography label={selectedItem.name} />
            </StackRow>
          );
        }

        return (
          <StackRow sx={{ alignItems: 'center' }}>
            <LeadIconTypography label="Zones" startElement={<ZoneIcon />} />
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
          <BodyIconTypography label={item.name} />
        </MenuItem>
      ))}
    </DefaultSelect>
  );
};

export default memo(ZoneSelector);

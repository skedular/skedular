import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@skedular/ui';
import { FloorPlanIcon } from '@/components/icons';
import { DefaultSelect } from '@/components/styled';
import type { floorPlanSelector_allFloorPlans_query$key } from '@/queries/__generated__/floorPlanSelector_allFloorPlans_query.graphql';
import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import { SelectChangeEvent } from '@mui/material/Select';
import { memo, useEffect, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: floorPlanSelector_allFloorPlans_query$key;
  onChange: (id?: string) => void;
};

const FloorPlanSelector = ({ rootDataRelay, onChange }: Props) => {
  const rootData = useFragment<floorPlanSelector_allFloorPlans_query$key>(
    graphql`
      fragment floorPlanSelector_allFloorPlans_query on Query {
        floorPlans(where: { locationId: $locationId }, orderBy: $floorPlansSortingValues) {
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

  const allItems = useMemo(() => (rootData.floorPlans?.edges ? rootData.floorPlans.edges.map(({ node }) => node) : []), [rootData.floorPlans]);
  const defaultId = allItems.length > 0 ? allItems[0].id : null;
  const [userSelectedId, setUserSelectedId] = useState<string | null>(null);
  const id = userSelectedId ?? defaultId;

  useEffect(() => {
    if (defaultId !== null) {
      onChange(defaultId);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [defaultId]);

  const handleChanged = (event: SelectChangeEvent<unknown>) => {
    const newId = event.target.value as string;

    setUserSelectedId(newId);
    onChange(newId);
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
              <LeadIconTypography label="Floor Plan" startElement={<FloorPlanIcon />} />
              <Divider orientation="vertical" flexItem />
              <PushToRight />
              <SmallIconTypography label={selectedItem.name} />
            </StackRow>
          );
        }

        return (
          <StackRow sx={{ alignItems: 'center' }}>
            <LeadIconTypography label="Floor Plan" startElement={<FloorPlanIcon />} />
            <Divider orientation="vertical" flexItem />
            <PushToRight />
            <SmallIconTypography label="All" />
          </StackRow>
        );
      }}
    >
      {allItems.map((item) => (
        <MenuItem key={item.id} value={item.id}>
          <BodyIconTypography label={item.name} />
        </MenuItem>
      ))}
    </DefaultSelect>
  );
};

export default memo(FloorPlanSelector);

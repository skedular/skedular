import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@/components/commons';
import { ResourceIcon } from '@/components/icons';
import { DefaultSelect } from '@/components/styled';
import type { resourceTypeSelector_allResourceTypes_query$key } from '@/queries/__generated__/resourceTypeSelector_allResourceTypes_query.graphql';
import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import { SelectChangeEvent } from '@mui/material/Select';
import { memo, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: resourceTypeSelector_allResourceTypes_query$key;
  onChange: (id?: string) => void;
};

const allId = 'kkigMVsUXwi2YMSSrXv7i';

const ResourceTypeSelector = ({ rootDataRelay, onChange }: Props) => {
  const rootData = useFragment<resourceTypeSelector_allResourceTypes_query$key>(
    graphql`
      fragment resourceTypeSelector_allResourceTypes_query on Query {
        resourceTypes {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const allItems = useMemo(() => rootData.resourceTypes, [rootData.resourceTypes]);
  const [id, setId] = useState<string>(allId);

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
        const selectedItem = allItems.find((item) => item.type === selectedId);
        if (selectedItem) {
          return (
            <StackRow sx={{ alignItems: 'center' }}>
              <LeadIconTypography label="Resource Type" startElement={<ResourceIcon />} />
              <Divider orientation="vertical" flexItem />
              <PushToRight />
              <SmallIconTypography label={selectedItem.name} />
            </StackRow>
          );
        }

        return (
          <StackRow sx={{ alignItems: 'center' }}>
            <LeadIconTypography label="Resource Type" startElement={<ResourceIcon />} />
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
        <MenuItem key={item.type} value={item.type}>
          <BodyIconTypography label={item.name} />
        </MenuItem>
      ))}
    </DefaultSelect>
  );
};

export default memo(ResourceTypeSelector);

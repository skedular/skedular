import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@skedular/ui';
import { CustomTagIcon } from '@/components/icons';
import { DefaultSelect } from '@/components/styled';
import type { customTagSelector_allCustomTags_query$key } from '@/queries/__generated__/customTagSelector_allCustomTags_query.graphql';
import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import { SelectChangeEvent } from '@mui/material/Select';
import { memo, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: customTagSelector_allCustomTags_query$key;
  onChange: (id?: string) => void;
};

const allId = 'kkigMVsUXwi2YMSSrXv7i';

const CustomTagSelector = ({ rootDataRelay, onChange }: Props) => {
  const rootData = useFragment<customTagSelector_allCustomTags_query$key>(
    graphql`
      fragment customTagSelector_allCustomTags_query on Query {
        organization(customDomain: $organizationCustomDomain) {
          customTags(orderBy: $customTagsSortingValues) {
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
      }
    `,
    rootDataRelay,
  );

  const allItems = useMemo(() => (rootData.organization ? rootData.organization.customTags.edges.map(({ node }) => node) : []), [rootData.organization]);
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
        const selectedItem = allItems.find((item) => item.id === selectedId);
        if (selectedItem) {
          return (
            <StackRow sx={{ alignItems: 'center' }}>
              <LeadIconTypography label="Tags" startElement={<CustomTagIcon />} />
              <Divider orientation="vertical" flexItem />
              <PushToRight />
              <SmallIconTypography label={selectedItem.name} />
            </StackRow>
          );
        }

        return (
          <StackRow sx={{ alignItems: 'center' }}>
            <LeadIconTypography label="Tags" startElement={<CustomTagIcon />} />
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

export default memo(CustomTagSelector);

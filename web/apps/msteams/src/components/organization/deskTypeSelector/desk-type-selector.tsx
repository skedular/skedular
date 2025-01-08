import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import { SelectChangeEvent } from '@mui/material/Select';
import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@repo/shared/components/commons';
import { DeskTypeIcon } from '@repo/shared/components/icons';
import { DefaultSelect } from '@repo/shared/components/styled';
import graphql from 'babel-plugin-relay/macro';
import { memo, useMemo, useState } from 'react';
import { useFragment } from 'react-relay';
import type { deskTypeSelector_allDeskTypes_query$key } from './__generated__/deskTypeSelector_allDeskTypes_query.graphql';

type Props = {
  rootDataRelay: deskTypeSelector_allDeskTypes_query$key;
  onChange: (id?: string) => void;
};

const allId = 'kkigMVsUXwi2YMSSrXv7i';

const DeskTypeSelector = ({ rootDataRelay, onChange }: Props) => {
  const rootData = useFragment<deskTypeSelector_allDeskTypes_query$key>(
    graphql`
      fragment deskTypeSelector_allDeskTypes_query on Query {
        deskTypes(where: { organizationId: $organizationId }, orderBy: $deskTypesSortingValues) {
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
  const allItems = useMemo(() => (rootData.deskTypes?.edges ? rootData.deskTypes.edges.map(({ node }) => node) : []), [rootData.deskTypes]);

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
              <LeadIconTypography label="Desk Types" startElement={<DeskTypeIcon />} />
              <Divider orientation="vertical" flexItem />
              <PushToRight />
              <SmallIconTypography label={selectedItem.name} />
            </StackRow>
          );
        }

        return (
          <StackRow sx={{ alignItems: 'center' }}>
            <LeadIconTypography label="Desk Types" startElement={<DeskTypeIcon />} />
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

export default memo(DeskTypeSelector);

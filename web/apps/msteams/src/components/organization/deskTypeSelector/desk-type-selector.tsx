import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { DeskIcon } from '@repo/shared/components/icons';
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
        deskTypes(where: { organizationId: $organizationId }) {
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

  const handleSelectedChanged = (event: SelectChangeEvent) => {
    const id = event.target.value as string;

    setId(id);
    onChange(id === allId ? undefined : id);
  };

  return (
    <Select
      value={id}
      onChange={handleSelectedChanged}
      sx={{
        '& .MuiOutlinedInput-notchedOutline': {
          borderRadius: 4,
        },
        width: {
          xs: '100%',
          sm: 'min(100%, 250px)',
        },
      }}
      size="small"
      renderValue={(selectedId) => {
        const selectedItem = allItems.find((item) => item.id === selectedId);
        if (selectedItem) {
          return (
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <DeskIcon />
              <Typography variant="h6">Desks</Typography>
              <Divider orientation="vertical" flexItem />
              <Typography variant="body1">{selectedItem.name}</Typography>
            </Stack>
          );
        }

        return (
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <DeskIcon />
            <Typography variant="h6">Desks</Typography>
            <Divider orientation="vertical" flexItem />
            <Typography variant="body1">All</Typography>
          </Stack>
        );
      }}
    >
      <MenuItem value={allId}>
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <Typography variant="h6">All</Typography>
        </Stack>
      </MenuItem>

      {allItems.map((location) => (
        <MenuItem key={location.id} value={location.id}>
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">{location.name}</Typography>
          </Stack>
        </MenuItem>
      ))}
    </Select>
  );
};

export default memo(DeskTypeSelector);

import Badge from '@mui/material/Badge';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { memo, useState } from 'react';
import { CollapseIcon, DeskTypeIcon, MoreItemsIcon } from '../icons';
import type { DeskTypeDetails } from './desk-type';
import DeskType from './desk-type';

type Props = {
  deskTypes: readonly DeskTypeDetails[];
  deskTypeTotalDisplayLimit?: number;
};

const preferredDeskTypesTotalDisplayLimit = 2;

const DeskTypesLine = ({ deskTypes, deskTypeTotalDisplayLimit = preferredDeskTypesTotalDisplayLimit }: Props) => {
  const [showAll, setShowAll] = useState(false);
  const limit = deskTypeTotalDisplayLimit <= 0 ? preferredDeskTypesTotalDisplayLimit : deskTypeTotalDisplayLimit;
  const deskTypesToDisplay = showAll ? deskTypes : deskTypes.slice(0, limit);
  const deskTypesToDisplayInBadge = showAll ? [] : deskTypes.slice(limit);

  const handleExpand = () => {
    setShowAll(true);
  };

  const handleCollapse = () => {
    setShowAll(false);
  };

  return (
    <>
      {deskTypes.length === 0 && (
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <DeskTypeIcon />
          <Typography>N/A</Typography>
        </Stack>
      )}

      {deskTypes.length !== 0 && (
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <DeskTypeIcon />
          {deskTypesToDisplay.map((deskType) => (
            <DeskType key={deskType.id} deskType={deskType} maxWidth={100} />
          ))}

          {deskTypes.length - limit > 0 && showAll && (
            <Tooltip title="Collapse">
              <Button size="small" onClick={handleCollapse}>
                <CollapseIcon />
              </Button>
            </Tooltip>
          )}

          {deskTypesToDisplayInBadge.length !== 0 && (
            <Tooltip title={deskTypesToDisplayInBadge.map((item) => item.name).join(', ')}>
              <Button size="small" onClick={handleExpand}>
                <Badge badgeContent={deskTypesToDisplayInBadge.length} color="info">
                  <MoreItemsIcon />
                </Badge>
              </Button>
            </Tooltip>
          )}
        </Stack>
      )}
    </>
  );
};

export default memo(DeskTypesLine);

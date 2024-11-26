import Badge from '@mui/material/Badge';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { memo, useState } from 'react';
import { CollapseIcon, MoreItemsIcon, ZoneIcon } from '../icons';
import type { ZoneDetails } from './zone';
import Zone from './zone';

type Props = {
  zones: readonly ZoneDetails[];
  zoneTotalDisplayLimit?: number;
};

const preferredZonesTotalDisplayLimit = 2;

const ZonesLine = ({ zones, zoneTotalDisplayLimit = preferredZonesTotalDisplayLimit }: Props) => {
  const [showAll, setShowAll] = useState(false);
  const limit = zoneTotalDisplayLimit <= 0 ? preferredZonesTotalDisplayLimit : zoneTotalDisplayLimit;
  const zonesToDisplay = showAll ? zones : zones.slice(0, limit);
  const zonesToDisplayInBadge = showAll ? [] : zones.slice(limit);

  const handleExpand = () => {
    setShowAll(true);
  };

  const handleCollapse = () => {
    setShowAll(false);
  };

  return (
    <>
      {zones.length === 0 && (
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <ZoneIcon />
          <Typography>N/A</Typography>
        </Stack>
      )}

      {zones.length !== 0 && (
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <ZoneIcon />
          {zonesToDisplay.map((zone) => (
            <Zone key={zone.id} zone={zone} maxWidth={100} />
          ))}

          {zones.length - limit > 0 && showAll && (
            <Tooltip title="Collapse">
              <Button size="small" onClick={handleCollapse}>
                <CollapseIcon />
              </Button>
            </Tooltip>
          )}

          {zonesToDisplayInBadge.length !== 0 && (
            <Tooltip title={zonesToDisplayInBadge.map((item) => item.name).join(', ')}>
              <Button size="small" onClick={handleExpand}>
                <Badge badgeContent={zonesToDisplayInBadge.length} color="info">
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

export default memo(ZonesLine);

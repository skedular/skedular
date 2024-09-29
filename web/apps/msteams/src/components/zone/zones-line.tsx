import Badge from '@mui/material/Badge';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Stack from '@mui/material/Stack';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { CollapseIcon, MoreItemsIcon, ZoneIcon } from '@repo/shared/components/icons';
import { memo, useState } from 'react';

export type Zone = {
  id: string;
  name: string;
};

type Props = {
  zones: ReadonlyArray<Zone>;
  zoneTotalDisplayLimit?: number;
};

const preferredZonesTotalDisplayLimit = 2;
const maxChipTextWidthToDisplay = 100;

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
          <Typography>No zone</Typography>
        </Stack>
      )}

      {zones.length !== 0 && (
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <ZoneIcon />
          {zonesToDisplay.map(({ id, name }) => (
            <Tooltip key={id} title={name}>
              <Chip label={name} sx={{ maxWidth: maxChipTextWidthToDisplay }} />
            </Tooltip>
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

import Badge from '@mui/material/Badge';
import Button from '@mui/material/Button';
import Tooltip from '@mui/material/Tooltip';
import { memo, useState } from 'react';
import { BodyIconTypography, StackRow } from '../commons';
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
      {deskTypes.length === 0 && <BodyIconTypography startElement={<DeskTypeIcon />} label="N/A" />}

      {deskTypes.length !== 0 && (
        <StackRow sx={{ alignItems: 'center' }}>
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
        </StackRow>
      )}
    </>
  );
};

export default memo(DeskTypesLine);

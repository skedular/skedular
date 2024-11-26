import Chip from '@mui/material/Chip';
import Tooltip from '@mui/material/Tooltip';
import { memo } from 'react';
import { stringToColor } from '../../libs/utils';

export type ZoneDetails = {
  id: string;
  name?: string | null | undefined;
};

type Props = {
  zone: ZoneDetails;
  maxWidth?: number;
};

const Zone = ({ zone, maxWidth }: Props) => (
  <Tooltip title={zone.name}>
    <Chip label={`#${zone.name}`} sx={{ maxWidth, bgcolor: stringToColor(zone.id) }} />
  </Tooltip>
);

export default memo(Zone);

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
  showFullName?: boolean;
};

const Zone = ({ zone, showFullName }: Props) => (
  <Tooltip title={zone.name}>
    <Chip label={`#${zone.name}`} sx={{ maxWidth: showFullName ? undefined : 100, backgroundColor: stringToColor(zone.id) }} />
  </Tooltip>
);

export default memo(Zone);

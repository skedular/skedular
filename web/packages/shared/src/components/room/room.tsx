import Chip from '@mui/material/Chip';
import Tooltip from '@mui/material/Tooltip';
import { memo } from 'react';
import { stringToColor } from '../../libs/utils';

export type RoomDetails = {
  id: string;
  name?: string | null | undefined;
  color?: string | null | undefined;
};

type Props = {
  room: RoomDetails;
  showFullName?: boolean;
};

const Room = ({ room, showFullName }: Props) => (
  <Tooltip title={room.name}>
    <Chip label={`${room.name}`} sx={{ maxWidth: showFullName ? undefined : 100, backgroundColor: room.color ?? stringToColor(room.id) }} />
  </Tooltip>
);

export default memo(Room);

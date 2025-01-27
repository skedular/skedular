import Chip from '@mui/material/Chip';
import Tooltip from '@mui/material/Tooltip';
import { memo } from 'react';
import { stringToColor } from '../../libs/utils';

export type DeskDetails = {
  id: string;
  name?: string | null | undefined;
  color?: string | null | undefined;
};

type Props = {
  desk: DeskDetails;
  showFullName?: boolean;
};

const Desk = ({ desk, showFullName }: Props) => (
  <Tooltip title={desk.name}>
    <Chip label={`${desk.name}`} sx={{ maxWidth: showFullName ? undefined : 100, backgroundColor: desk.color ?? stringToColor(desk.id) }} />
  </Tooltip>
);

export default memo(Desk);

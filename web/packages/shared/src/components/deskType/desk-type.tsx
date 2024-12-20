import Chip from '@mui/material/Chip';
import Tooltip from '@mui/material/Tooltip';
import { memo } from 'react';
import { stringToColor } from '../../libs/utils';

export type DeskTypeDetails = {
  id: string;
  name?: string | null | undefined;
};

type Props = {
  deskType: DeskTypeDetails;
  maxWidth?: number;
};

const DeskType = ({ deskType, maxWidth }: Props) => (
  <Tooltip title={deskType.name}>
    <Chip label={`#${deskType.name}`} sx={{ maxWidth, backgroundColor: stringToColor(deskType.id) }} />
  </Tooltip>
);

export default memo(DeskType);

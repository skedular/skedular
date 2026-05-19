import { stringToColor } from '@skedular/shared';
import Chip from '@mui/material/Chip';
import Tooltip from '@mui/material/Tooltip';
import { memo } from 'react';

export type CustomTagDetails = {
  id: string;
  name?: string | null | undefined;
  color?: string | null | undefined;
};

type Props = {
  customTag: CustomTagDetails;
  showFullName?: boolean;
};

const CustomTag = ({ customTag, showFullName }: Props) => (
  <Tooltip title={customTag.name}>
    <Chip label={`#${customTag.name}`} sx={{ maxWidth: showFullName ? undefined : 100, backgroundColor: customTag.color ?? stringToColor(customTag.id) }} />
  </Tooltip>
);

export default memo(CustomTag);

import { stringToColor } from '@/libs/utils';
import Chip from '@mui/material/Chip';
import Tooltip from '@mui/material/Tooltip';
import { memo } from 'react';

export type ResourceDetails = {
  id: string;
  name?: string | null | undefined;
  color?: string | null | undefined;
};

type Props = {
  resource: ResourceDetails;
  showFullName?: boolean;
};

const Resource = ({ resource, showFullName }: Props) => (
  <Tooltip title={resource.name}>
    <Chip label={`${resource.name}`} sx={{ maxWidth: showFullName ? undefined : 100, backgroundColor: resource.color ?? stringToColor(resource.id) }} />
  </Tooltip>
);

export default memo(Resource);

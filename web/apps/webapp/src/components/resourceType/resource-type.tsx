import { stringToColor } from '@/libs/utils';
import Chip from '@mui/material/Chip';
import Tooltip from '@mui/material/Tooltip';
import { memo } from 'react';

export type ResourceTypeDetails = {
  id: string;
  name?: string | null | undefined;
  color?: string | null | undefined;
};

type Props = {
  resourceType: ResourceTypeDetails;
  showFullName?: boolean;
};

const ResourceType = ({ resourceType, showFullName }: Props) => (
  <Tooltip title={resourceType.name}>
    <Chip label={`${resourceType.name}`} sx={{ maxWidth: showFullName ? undefined : 100, backgroundColor: resourceType.color ?? stringToColor(resourceType.id) }} />
  </Tooltip>
);

export default memo(ResourceType);

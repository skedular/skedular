import { stringToColor } from '@/libs/utils';
import Chip from '@mui/material/Chip';
import Tooltip from '@mui/material/Tooltip';
import { memo } from 'react';

export type LocationTagDetails = {
  id: string;
  name?: string | null | undefined;
  color?: string | null | undefined;
};

type Props = {
  locationTag: LocationTagDetails;
  showFullName?: boolean;
};

const LocationTag = ({ locationTag, showFullName }: Props) => (
  <Tooltip title={locationTag.name}>
    <Chip label={`#${locationTag.name}`} sx={{ maxWidth: showFullName ? undefined : 100, backgroundColor: locationTag.color ?? stringToColor(locationTag.id) }} />
  </Tooltip>
);

export default memo(LocationTag);

import { stringToColor } from '@/libs/utils';
import Chip from '@mui/material/Chip';
import Tooltip from '@mui/material/Tooltip';
import { memo } from 'react';

export type ProductTagDetails = {
  id: string;
  name?: string | null | undefined;
  color?: string | null | undefined;
};

type Props = {
  productTag: ProductTagDetails;
  showFullName?: boolean;
};

const ProductTag = ({ productTag, showFullName }: Props) => (
  <Tooltip title={productTag.name}>
    <Chip label={`#${productTag.name}`} sx={{ maxWidth: showFullName ? undefined : 100, backgroundColor: productTag.color ?? stringToColor(productTag.id) }} />
  </Tooltip>
);

export default memo(ProductTag);

import { GridContainer, SmallIconTypography, StackRow } from '@skedular/ui';
import { ProductTagIcon } from '@/components/icons';
import Chip from '@mui/material/Chip';
import Tooltip from '@mui/material/Tooltip';
import Grid from '@mui/material/Grid';
import type { SxProps, Theme } from '@mui/system';
import { memo } from 'react';
import type { ProductTagDetails } from './product-tag';
import ProductTag from './product-tag';

type Props = {
  sx?: SxProps<Theme>;
  productTags: readonly ProductTagDetails[];
  hideIcon?: boolean;
  hideNAText?: boolean;
};

const maxItemToDisplay = 2;

const ProductTags = ({ sx, productTags, hideIcon, hideNAText }: Props) => {
  if (productTags.length === 0) {
    return hideNAText ? null : <SmallIconTypography label="N/A" startElement={!hideIcon && <ProductTagIcon />} sx={sx} />;
  }

  const visibleItems = productTags.slice(0, maxItemToDisplay);
  const extraItems = productTags.slice(maxItemToDisplay);

  return (
    <StackRow sx={sx}>
      <GridContainer spacing={1}>
        {!hideIcon && (
          <Grid>
            <ProductTagIcon />
          </Grid>
        )}
        {visibleItems.map((productTag) => (
          <Grid key={productTag.id}>
            <ProductTag productTag={productTag} />
          </Grid>
        ))}
        {extraItems.length > 0 && (
          <Grid>
            <Tooltip title={extraItems.map((item) => item.name).join(', ')}>
              <Chip label={`+${extraItems.length}`} />
            </Tooltip>
          </Grid>
        )}
      </GridContainer>
    </StackRow>
  );
};

export default memo(ProductTags);

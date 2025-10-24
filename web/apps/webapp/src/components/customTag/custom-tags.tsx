import { GridContainer, SmallIconTypography, StackRow } from '@/components/commons';
import { CustomTagIcon } from '@/components/icons';
import { Chip, Tooltip } from '@mui/material';
import Grid from '@mui/material/Grid';
import type { SxProps, Theme } from '@mui/system';
import { memo } from 'react';
import type { CustomTagDetails } from './custom-tag';
import CustomTag from './custom-tag';

type Props = {
  sx?: SxProps<Theme>;
  customTags: readonly CustomTagDetails[];
  hideIcon?: boolean;
  hideNAText?: boolean;
};

const maxItemToDisplay = 2;

const CustomTags = ({ sx, customTags, hideIcon, hideNAText }: Props) => {
  if (customTags.length === 0) {
    return hideNAText ? null : <SmallIconTypography label="N/A" startElement={!hideIcon && <CustomTagIcon />} sx={sx} />;
  }

  const visibleItems = customTags.slice(0, maxItemToDisplay);
  const extraItems = customTags.slice(maxItemToDisplay);

  return (
    <StackRow sx={sx}>
      <GridContainer spacing={1}>
        {!hideIcon && (
          <Grid>
            <CustomTagIcon />
          </Grid>
        )}
        {visibleItems.map((customTag) => (
          <Grid key={customTag.id}>
            <CustomTag customTag={customTag} />
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

export default memo(CustomTags);

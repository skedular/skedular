import { GridContainer, SmallIconTypography, StackRow } from '@/components/commons';
import { LocationTagIcon } from '@/components/icons';
import { Chip, Tooltip } from '@mui/material';
import Grid from '@mui/material/Grid';
import type { SxProps, Theme } from '@mui/system';
import { memo } from 'react';
import type { LocationTagDetails } from './location-tag';
import LocationTag from './location-tag';

type Props = {
  sx?: SxProps<Theme>;
  locationTags: readonly LocationTagDetails[];
  hideIcon?: boolean;
  hideNAText?: boolean;
};

const maxItemToDisplay = 2;

const LocationTags = ({ sx, locationTags, hideIcon, hideNAText }: Props) => {
  if (locationTags.length === 0) {
    return hideNAText ? null : <SmallIconTypography label="N/A" startElement={!hideIcon && <LocationTagIcon />} sx={sx} />;
  }

  const visibleItems = locationTags.slice(0, maxItemToDisplay);
  const extraItems = locationTags.slice(maxItemToDisplay);

  return (
    <StackRow sx={sx}>
      <GridContainer spacing={1}>
        {!hideIcon && (
          <Grid>
            <LocationTagIcon />
          </Grid>
        )}
        {visibleItems.map((locationTag) => (
          <Grid key={locationTag.id}>
            <LocationTag locationTag={locationTag} />
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

export default memo(LocationTags);

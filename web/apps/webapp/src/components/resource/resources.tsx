import { GridContainer, SmallIconTypography, StackRow } from '@/components/commons';
import { ResourceIcon } from '@/components/icons';
import { Chip, Tooltip } from '@mui/material';
import Grid from '@mui/material/Grid2';
import type { SxProps, Theme } from '@mui/system';
import { memo } from 'react';
import type { ResourceDetails } from './resource';
import Resource from './resource';

type Props = {
  sx?: SxProps<Theme>;
  Resources: readonly ResourceDetails[];
  hideIcon?: boolean;
};

const maxItemToDisplay = 2;

const Resources = ({ sx, Resources, hideIcon }: Props) => {
  if (Resources.length === 0) {
    return <SmallIconTypography label="N/A" startElement={!hideIcon && <ResourceIcon />} sx={sx} />;
  }

  const visibleItems = Resources.slice(0, maxItemToDisplay);
  const extraItems = Resources.slice(maxItemToDisplay);

  return (
    <StackRow sx={sx}>
      <GridContainer spacing={1}>
        {!hideIcon && (
          <Grid>
            <ResourceIcon />
          </Grid>
        )}
        {visibleItems.map((resource) => (
          <Grid key={resource.id}>
            <Resource key={resource.id} resource={resource} />
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

export default memo(Resources);

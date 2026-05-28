import type { ResourceDayViewList_result$key } from '@/queries/__generated__/ResourceDayViewList_result.graphql';
import Box from '@mui/material/Box';
import type { SxProps, Theme } from '@mui/system';
import { SubtitleIconTypography } from '@skedular/ui';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';
import ResourceDayViewCard from './ResourceDayViewCard';

type Props = {
  resultRef: ResourceDayViewList_result$key;
};

const surfaceSx: SxProps<Theme> = {
  borderRadius: 4,
  border: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.88)' : theme.palette.background.paper),
  boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : '0 1px 3px rgba(0, 0, 0, 0.24)'),
};

const ResourceDayViewList = ({ resultRef }: Props) => {
  const data = useFragment<ResourceDayViewList_result$key>(
    graphql`
      fragment ResourceDayViewList_result on ResourceDayViewConnection {
        subscriptionKey
        items {
          resourceId
          ...ResourceDayViewCard_resourceDayView
        }
      }
    `,
    resultRef,
  );

  if (data.items.length === 0) {
    return (
      <Box sx={{ ...surfaceSx, px: 3, py: 4 }}>
        <SubtitleIconTypography label="No resources found for the selected filters." />
      </Box>
    );
  }

  return (
    <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', md: '1fr 1fr 1fr' }, gap: 2, alignItems: 'stretch' }} aria-label="Resource availability list">
      {data.items.map((item) => (
        <ResourceDayViewCard key={item.resourceId} resourceDayViewRef={item} />
      ))}
    </Box>
  );
};

export default memo(ResourceDayViewList);

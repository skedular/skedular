import { BodyIconTypography, SmallHeadingIconTypography, StackColumn } from '@/components/commons';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import type { PropsWithChildren } from 'react';
import { memo } from 'react';

type Props = {
  title?: string;
  description?: string;
};

const RightSidePanel = ({ children, title, description }: PropsWithChildren<Props>) => (
  <Grid sx={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
    <Box sx={{ padding: 2, height: '100%' }}>
      <StackColumn>
        {title && <SmallHeadingIconTypography label={title} />}
        {description && <BodyIconTypography label={description} sx={{ paddingTop: 1, paddingBottom: 1 }} />}
        {children}
      </StackColumn>
    </Box>
  </Grid>
);

export default memo(RightSidePanel);

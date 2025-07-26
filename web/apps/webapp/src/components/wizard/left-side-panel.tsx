import { BodyIconTypography, SmallHeadingIconTypography, StackColumn } from '@/components/commons';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import type { PropsWithChildren } from 'react';
import { memo } from 'react';

type Props = {
  title?: string;
  description?: string;
};

const LeftSidePanel = ({ children, title, description }: PropsWithChildren<Props>) => (
  <Grid sx={{ height: '100%', width: '40%' }}>
    <Box sx={{ padding: 2, background: 'linear-gradient(0deg, #74d77eff 0%, #cab9ffff 100%)', height: '100%' }}>
      <StackColumn>
        {title && <SmallHeadingIconTypography label={title} />}
        {description && <BodyIconTypography label={description} sx={{ paddingTop: 1, paddingBottom: 1 }} />}
        {children}
      </StackColumn>
    </Box>
  </Grid>
);

export default memo(LeftSidePanel);

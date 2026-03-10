import { UnauthenticatedAppBar } from '@/components/appBar';
import { UnathenticatedObservability } from '@/components/observability';
import Box from '@mui/material/Box';
import CssBaseline from '@mui/material/CssBaseline';
import type { PropsWithChildren } from 'react';
import { memo } from 'react';

const UnauthenticatedRootShell = ({ children }: PropsWithChildren) => (
  <>
    <UnathenticatedObservability />
    <Box sx={{ display: 'flex' }}>
      <CssBaseline enableColorScheme />
      <Box sx={{ flexGrow: 1 }}>
        <UnauthenticatedAppBar />
        {children}
      </Box>
    </Box>
  </>
);

export default memo(UnauthenticatedRootShell);

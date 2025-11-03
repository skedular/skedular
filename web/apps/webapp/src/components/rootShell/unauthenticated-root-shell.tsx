import { UnauthenticatedAppBar } from '@/components/appBar';
import { UnathenticatedObservability } from '@/components/observability';
import Box from '@mui/material/Box';
import CssBaseline from '@mui/material/CssBaseline';
import type { JSX, PropsWithChildren } from 'react';
import { memo } from 'react';

type Props = {
  showBreadcrumps?: boolean;
  breadcrumbs?: React.ReactNode | JSX.Element;
};

const UnauthenticatedRootShell = ({ children, showBreadcrumps, breadcrumbs }: PropsWithChildren<Props>) => (
  <>
    <UnathenticatedObservability />
    <Box sx={{ display: 'flex' }}>
      <CssBaseline enableColorScheme />
      <Box sx={{ flexGrow: 1 }}>
        <UnauthenticatedAppBar showBreadcrumps={showBreadcrumps} breadcrumbs={breadcrumbs} />
        {children}
      </Box>
    </Box>
  </>
);

export default memo(UnauthenticatedRootShell);

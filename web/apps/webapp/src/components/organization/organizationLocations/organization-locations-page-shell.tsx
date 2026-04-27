import { CollectionToolbar, StackColumn, SubtitleIconTypography } from '@skedular/ui';
import { defaultPadding } from '@skedular/ui';
import type { SxProps, Theme } from '@mui/system';
import Box from '@mui/system/Box';
import { PageHeaderPanel } from '@skedular/ui';
import type { ReactNode } from 'react';

type Props = {
  actions?: ReactNode;
  toolbar?: ReactNode;
  isEmpty: boolean;
  emptyMessage?: string;
  children?: ReactNode;
};

const surfaceSx: SxProps<Theme> = {
  borderRadius: 4,
  border: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.88)' : theme.palette.background.paper),
  boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : '0 1px 3px rgba(0, 0, 0, 0.24)'),
};

const OrganizationLocationsPageShell = ({ actions, toolbar, isEmpty, emptyMessage = 'No locations yet', children }: Props) => (
  <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 0, sm: 1, md: 2 }, pb: defaultPadding }}>
    <StackColumn sx={{ width: '100%', maxWidth: 1120, mx: 'auto', pt: { xs: 1, sm: 1, md: 2 } }} spacing={2}>
      <PageHeaderPanel title="Locations" description="Manage bookable spaces, availability context, and contact details for each location." />

      <CollectionToolbar filters={toolbar} actions={actions} />

      {isEmpty ? (
        <Box sx={{ ...surfaceSx, px: 3, py: 4 }}>
          <SubtitleIconTypography label={emptyMessage} />
        </Box>
      ) : (
        children
      )}
    </StackColumn>
  </Box>
);

export default OrganizationLocationsPageShell;

'use client';

import type { SxProps, Theme } from '@mui/system';
import Box from '@mui/system/Box';
import type { ReactNode } from 'react';
import { defaultPadding } from '../theme/layout';
import CollectionToolbar from '../commons/collection-toolbar';
import PageHeaderPanel from '../page-header-panel';
import StackColumn from '../stack-column';
import SubtitleIconTypography from '../typography/subtitle-icon-typography';

type Props = {
  title: ReactNode;
  description: ReactNode;
  actions?: ReactNode;
  toolbar?: ReactNode;
  isEmpty: boolean;
  emptyMessage: ReactNode;
  children?: ReactNode;
  contentMode?: 'plain' | 'grid';
  sx?: SxProps<Theme>;
};

const surfaceSx: SxProps<Theme> = {
  borderRadius: 4,
  border: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.88)' : theme.palette.background.paper),
  boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : '0 1px 3px rgba(0, 0, 0, 0.24)'),
};

const gridSx: SxProps<Theme> = {
  display: 'grid',
  gridTemplateColumns: {
    xs: '1fr',
    sm: 'repeat(auto-fit, minmax(320px, 360px))',
  },
  gap: 2,
  alignItems: 'stretch',
  justifyContent: 'start',
};

const ManagementPageShell = ({ title, description, actions, toolbar, isEmpty, emptyMessage, children, contentMode = 'plain', sx }: Props) => (
  <Box
    sx={[
      {
        width: '100%',
        display: 'flex',
        justifyContent: 'center',
        px: { xs: 0, sm: 1, md: 2 },
        pb: defaultPadding,
      },
      ...(sx != null ? (Array.isArray(sx) ? sx : [sx]) : []),
    ]}
  >
    <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', pt: { xs: 1, sm: 1, md: 2 } }} spacing={2}>
      <PageHeaderPanel title={title} description={description} />

      <CollectionToolbar filters={toolbar} actions={actions} />

      {isEmpty ? (
        <Box sx={{ ...surfaceSx, px: 3, py: 4 }}>
          <SubtitleIconTypography label={emptyMessage} />
        </Box>
      ) : contentMode === 'grid' ? (
        <Box sx={gridSx}>{children}</Box>
      ) : (
        children
      )}
    </StackColumn>
  </Box>
);

export default ManagementPageShell;

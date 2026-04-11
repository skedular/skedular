import { StackColumn, SubtitleIconTypography } from '@/components/commons';
import { defaultPadding } from '@/libs/theme';
import { PageHeaderPanel } from '@skedular/ui';
import Box from '@mui/system/Box';
import type { ReactNode } from 'react';

type Props = {
  actions?: ReactNode;
  isEmpty: boolean;
  children?: ReactNode;
};

const OrganizationProductsPageShell = ({ actions, isEmpty, children }: Props) => (
  <StackColumn sx={{ width: '100%', padding: defaultPadding }} spacing={2}>
    <PageHeaderPanel title="Products" description="Create and manage the bookable offers customers can purchase." actions={actions} />

    {isEmpty ? (
      <Box
        sx={{
          borderRadius: 4,
          border: 1,
          borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
          backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.88)' : theme.palette.background.paper),
          boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : '0 1px 3px rgba(0, 0, 0, 0.24)'),
          px: 3,
          py: 4,
        }}
      >
        <SubtitleIconTypography label="No products yet" />
      </Box>
    ) : (
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: {
            xs: '1fr',
            sm: 'repeat(auto-fit, minmax(320px, 360px))',
          },
          gap: 2,
          alignItems: 'stretch',
          justifyContent: 'start',
        }}
      >
        {children}
      </Box>
    )}
  </StackColumn>
);

export default OrganizationProductsPageShell;

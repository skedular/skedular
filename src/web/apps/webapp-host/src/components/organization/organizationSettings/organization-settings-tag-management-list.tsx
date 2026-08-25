import { LeadIconTypography, SmallIconTypography, compactManagementIconButtonSx } from '@skedular/ui';
import { EllipseMenuIcon } from '@/components/icons';
import Box from '@mui/material/Box';
import Checkbox from '@mui/material/Checkbox';
import IconButton from '@mui/material/IconButton';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import type { ReactNode } from 'react';
import { memo } from 'react';

export type OrganizationSettingsTagManagementListItem = {
  id: string;
  name: string;
  description: string | null | undefined;
};

type Props = {
  items: OrganizationSettingsTagManagementListItem[];
  emptyTitle: string;
  emptyDescription: string;
  selectedIds: string[];
  onToggleSelected: (id: string) => void;
  onOpenMoreActions: (id: string, target: HTMLElement) => void;
  onOpenItem?: (id: string) => void;
  renderPrimary: (item: OrganizationSettingsTagManagementListItem) => ReactNode;
  variant?: 'panel' | 'plain';
};

const OrganizationSettingsTagManagementList = ({
  items,
  emptyTitle,
  emptyDescription,
  selectedIds,
  onToggleSelected,
  onOpenMoreActions,
  onOpenItem,
  renderPrimary,
  variant = 'panel',
}: Props) => {
  if (items.length === 0) {
    return (
      <Box
        sx={
          variant === 'plain'
            ? {
                py: 3,
              }
            : {
                border: 1,
                borderStyle: 'dashed',
                borderColor: 'divider',
                borderRadius: 3,
                p: 3,
                backgroundColor: 'background.paper',
              }
        }
      >
        <LeadIconTypography label={emptyTitle} />
        <SmallIconTypography label={emptyDescription} />
      </Box>
    );
  }

  return (
    <TableContainer component={Box} sx={{ border: 0, overflow: 'hidden', bgcolor: 'transparent' }}>
      <Table size="small" aria-label="Management list" sx={{ minWidth: 520 }}>
        <TableHead>
          <TableRow
            sx={{
              '& th': { px: 1.5, py: 1.25, fontWeight: 700, textTransform: 'uppercase', letterSpacing: '0.04em', color: 'text.primary', borderBottom: 1, borderColor: 'divider' },
            }}
          >
            <TableCell padding="checkbox" />
            <TableCell>Name</TableCell>
            <TableCell sx={{ display: { xs: 'none', md: 'table-cell' } }}>Description</TableCell>
            <TableCell align="right" />
          </TableRow>
        </TableHead>
        <TableBody>
          {items.map((item) => {
            const isSelected = selectedIds.includes(item.id);

            return (
              <TableRow
                key={item.id}
                hover
                selected={isSelected}
                tabIndex={onOpenItem ? 0 : undefined}
                onClick={() => onOpenItem?.(item.id)}
                onKeyDown={(event) => {
                  if (onOpenItem && (event.key === 'Enter' || event.key === ' ')) {
                    event.preventDefault();
                    onOpenItem(item.id);
                  }
                }}
                sx={{ cursor: onOpenItem ? 'pointer' : undefined, '& td': { px: 1.5, py: 1.25, borderBottom: 1, borderColor: 'divider' } }}
              >
                <TableCell padding="checkbox" onClick={(event) => event.stopPropagation()}>
                  <Checkbox checked={isSelected} onChange={() => onToggleSelected(item.id)} slotProps={{ input: { 'aria-label': `Select ${item.name}` } }} />
                </TableCell>
                <TableCell sx={{ minWidth: 160, fontWeight: 600, '& .MuiTypography-root': { fontWeight: 600 } }}>{renderPrimary(item)}</TableCell>
                <TableCell sx={{ display: { xs: 'none', md: 'table-cell' }, maxWidth: 360, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                  {item.description ? <SmallIconTypography label={item.description} /> : <SmallIconTypography label="—" />}
                </TableCell>
                <TableCell align="right" onClick={(event) => event.stopPropagation()}>
                  <IconButton
                    onClick={(event: React.MouseEvent<HTMLElement>) => onOpenMoreActions(item.id, event.currentTarget)}
                    aria-label={`More actions for ${item.name}`}
                    sx={compactManagementIconButtonSx}
                  >
                    <EllipseMenuIcon />
                  </IconButton>
                </TableCell>
              </TableRow>
            );
          })}
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default memo(OrganizationSettingsTagManagementList);

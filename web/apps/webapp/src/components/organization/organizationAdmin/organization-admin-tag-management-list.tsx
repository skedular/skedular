import { LeadIconTypography, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { EllipseMenuIcon } from '@/components/icons';
import Box from '@mui/material/Box';
import Checkbox from '@mui/material/Checkbox';
import IconButton from '@mui/material/IconButton';
import type { ReactNode } from 'react';
import { memo } from 'react';

export type OrganizationAdminTagManagementListItem = {
  id: string;
  name: string;
  description: string | null | undefined;
};

type Props = {
  items: OrganizationAdminTagManagementListItem[];
  emptyTitle: string;
  emptyDescription: string;
  selectedIds: string[];
  onToggleSelected: (id: string) => void;
  onOpenMoreActions: (id: string, target: HTMLElement) => void;
  renderPrimary: (item: OrganizationAdminTagManagementListItem) => ReactNode;
};

const OrganizationAdminTagManagementList = ({ items, emptyTitle, emptyDescription, selectedIds, onToggleSelected, onOpenMoreActions, renderPrimary }: Props) => {
  if (items.length === 0) {
    return (
      <Box
        sx={{
          border: 1,
          borderStyle: 'dashed',
          borderColor: 'divider',
          borderRadius: 3,
          p: 3,
          backgroundColor: 'background.paper',
        }}
      >
        <LeadIconTypography label={emptyTitle} />
        <SmallIconTypography label={emptyDescription} />
      </Box>
    );
  }

  return (
    <StackColumn spacing={1}>
      {items.map((item) => {
        const isSelected = selectedIds.includes(item.id);

        return (
          <Box
            key={item.id}
            sx={{
              border: 1,
              borderColor: isSelected ? 'primary.main' : 'divider',
              borderRadius: 2.5,
              px: 1,
              py: 0.75,
              backgroundColor: isSelected ? 'action.selected' : 'background.paper',
              boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 2px 10px rgba(15, 23, 42, 0.04)' : theme.shadows[1]),
            }}
          >
            <StackRow sx={{ alignItems: 'center', gap: 1, flexWrap: 'nowrap', minWidth: 0 }}>
              <Checkbox checked={isSelected} onChange={() => onToggleSelected(item.id)} slotProps={{ input: { 'aria-label': `Select ${item.name}` } }} />

              <Box sx={{ flexShrink: 0 }}>{renderPrimary(item)}</Box>

              <Box sx={{ minWidth: 0, flex: '1 1 auto', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                {item.description ? <SmallIconTypography label={item.description} /> : null}
              </Box>

              <IconButton
                onClick={(event: React.MouseEvent<HTMLElement>) => {
                  onOpenMoreActions(item.id, event.currentTarget);
                }}
                aria-label={`More actions for ${item.name}`}
              >
                <EllipseMenuIcon />
              </IconButton>
            </StackRow>
          </Box>
        );
      })}
    </StackColumn>
  );
};

export default memo(OrganizationAdminTagManagementList);

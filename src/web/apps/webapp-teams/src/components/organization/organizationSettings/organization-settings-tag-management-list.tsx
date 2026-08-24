import { LeadIconTypography, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { EllipseMenuIcon } from '@/components/icons';
import Box from '@mui/material/Box';
import Checkbox from '@mui/material/Checkbox';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import type { Theme } from '@mui/material/styles';
import type { SxProps } from '@mui/system';
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
    <StackColumn spacing={1}>
      {items.map((item, itemIndex) => {
        const isSelected = selectedIds.includes(item.id);
        const rowSx: SxProps<Theme> =
          variant === 'plain'
            ? {
                py: 1.25,
                px: isSelected ? 1 : 0,
                backgroundColor: isSelected ? 'action.selected' : 'transparent',
                borderRadius: isSelected ? 2 : 0,
              }
            : {
                border: 1,
                borderColor: isSelected ? 'primary.main' : 'divider',
                borderRadius: 2.5,
                px: 1,
                py: 0.75,
                backgroundColor: isSelected ? 'action.selected' : 'background.paper',
                boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 2px 10px rgba(15, 23, 42, 0.04)' : theme.shadows[1]),
              };

        return (
          <StackColumn key={item.id} spacing={0}>
            {variant === 'plain' && itemIndex > 0 ? <Divider /> : null}
            <Box sx={rowSx}>
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
          </StackColumn>
        );
      })}
    </StackColumn>
  );
};

export default memo(OrganizationSettingsTagManagementList);

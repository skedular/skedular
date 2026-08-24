import { LeadIconTypography, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { DeleteIcon, EllipseMenuIcon } from '@/components/icons';
import { ResourceType } from '@/components/resourceType';
import { Zones } from '@/components/zone';
import { compactManagementIconButtonSx, defaultGridActionPadding } from '@skedular/ui';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Checkbox from '@mui/material/Checkbox';
import Chip from '@mui/material/Chip';
import IconButton from '@mui/material/IconButton';
import { memo } from 'react';

export type ResourceManagementListItem = {
  id: string;
  resourceName: string;
  resourceType: {
    id: string;
    name: string | null | undefined;
    color: string | null | undefined;
  };
  customTags: {
    id: string;
    name: string | null | undefined;
    color: string | null | undefined;
  }[];
  zones: {
    id: string;
    name: string | null | undefined;
    color: string | null | undefined;
  }[];
  isActive: boolean;
  isPreferred: boolean;
  capacity: number;
};

type Props = {
  items: ResourceManagementListItem[];
  selectedIds: string[];
  onToggleSelected: (resourceId: string) => void;
  onOpenResource: (resourceId: string) => void;
  onOpenMoreActions: (resourceId: string, target: HTMLElement) => void;
  onDeactivateSelected: (resourceIds: string[]) => void;
  onActivateSelected: (resourceIds: string[]) => void;
  onDeleteSelected: (resourceIds: string[]) => void;
};

const OrganizationLocationResourceManagementList = ({
  items,
  selectedIds,
  onToggleSelected,
  onOpenResource,
  onOpenMoreActions,
  onDeactivateSelected,
  onActivateSelected,
  onDeleteSelected,
}: Props) => {
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
        <LeadIconTypography label="No resources found" />
        <SmallIconTypography label="Adjust the filters or add a new resource for this location." />
      </Box>
    );
  }

  return (
    <StackColumn spacing={0}>
      {selectedIds.length > 0 && (
        <Box
          sx={{
            backgroundColor: 'background.paper',
            padding: defaultGridActionPadding,
            border: 1,
            borderColor: (theme) => theme.palette.divider,
            borderRadius: 2,
          }}
        >
          <StackRow sx={{ alignItems: 'center', flexWrap: 'wrap', gap: 1 }}>
            <SmallIconTypography label={`${selectedIds.length} resource${selectedIds.length === 1 ? '' : 's'} selected`} />
            <Box sx={{ flexGrow: 1 }} />
            <Button size="medium" variant="contained" color="secondary" onClick={() => onDeactivateSelected(selectedIds)}>
              Deactivate
            </Button>
            <Button size="medium" variant="contained" color="secondary" onClick={() => onActivateSelected(selectedIds)}>
              Activate
            </Button>
            <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={() => onDeleteSelected(selectedIds)} sx={{ textTransform: 'none' }}>
              Remove
            </Button>
          </StackRow>
        </Box>
      )}

      <Box
        sx={{
          display: { xs: 'none', lg: 'grid' },
          gridTemplateColumns: '40px minmax(180px, 1.4fr) 100px 80px minmax(180px, 1fr) 40px',
          gap: 1,
          alignItems: 'center',
          px: 0.5,
          py: 0.75,
          color: 'text.secondary',
          typography: 'caption',
          fontWeight: 700,
          textTransform: 'uppercase',
          letterSpacing: '0.04em',
        }}
      >
        <Box />
        <Box>Name</Box>
        <Box>Capacity</Box>
        <Box>Type</Box>
        <Box>Zone</Box>
        <Box />
      </Box>

      {items.map((item) => {
        const isSelected = selectedIds.includes(item.id);

        return (
          <Box
            key={item.id}
            role="button"
            tabIndex={0}
            onClick={() => onOpenResource(item.id)}
            onKeyDown={(event) => {
              if (event.key === 'Enter' || event.key === ' ') {
                event.preventDefault();
                onOpenResource(item.id);
              }
            }}
            sx={{
              borderBottom: 1,
              borderColor: 'divider',
              px: 0.5,
              py: 1,
              backgroundColor: isSelected ? 'action.selected' : 'background.paper',
              '&:last-child': { borderBottom: 0 },
              '&:hover': { backgroundColor: isSelected ? 'action.selected' : 'action.hover' },
              '&:focus-visible': { outline: '2px solid', outlineColor: 'primary.main', outlineOffset: -2 },
            }}
          >
            <StackColumn spacing={1}>
              <Box
                sx={{
                  display: 'grid',
                  gridTemplateColumns: { xs: '40px minmax(0, 1fr) auto', lg: '40px minmax(180px, 1.4fr) 100px 80px minmax(180px, 1fr) 40px' },
                  gap: 1,
                  alignItems: 'center',
                  minWidth: 0,
                }}
              >
                <Checkbox
                  checked={isSelected}
                  onClick={(event) => event.stopPropagation()}
                  onChange={() => onToggleSelected(item.id)}
                  slotProps={{ input: { 'aria-label': `Select ${item.resourceName}` } }}
                />

                <StackRow sx={{ gap: 0.75, minWidth: 0, overflow: 'hidden', whiteSpace: 'nowrap' }}>
                  {item.isActive ? (
                    <Box component="span" sx={{ width: 8, height: 8, flex: '0 0 auto', borderRadius: '50%', bgcolor: 'success.main' }} />
                  ) : (
                    <Box component="span" sx={{ width: 8, height: 8, flex: '0 0 auto', borderRadius: '50%', bgcolor: 'text.disabled' }} />
                  )}
                  <Box sx={{ minWidth: 0, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                    <LeadIconTypography label={item.resourceName} />
                  </Box>
                </StackRow>

                <Chip size="small" label={`${item.capacity} ${item.capacity === 1 ? 'person' : 'people'}`} sx={{ display: { xs: 'none', lg: 'inline-flex' } }} />

                <Box sx={{ display: { xs: 'none', lg: 'block' }, minWidth: 0, overflow: 'hidden' }}>
                  <ResourceType resourceType={item.resourceType} showFullName />
                </Box>
                <StackRow sx={{ display: { xs: 'none', lg: 'flex' }, gap: 0.5, minWidth: 0, overflow: 'hidden', whiteSpace: 'nowrap' }}>
                  {item.zones.length > 0 ? <Zones zones={item.zones} hideIcon hideNAText /> : <SmallIconTypography label="—" />}
                </StackRow>

                <IconButton
                  onClick={(event: React.MouseEvent<HTMLElement>) => {
                    event.stopPropagation();
                    onOpenMoreActions(item.id, event.currentTarget);
                  }}
                  aria-label={`More actions for ${item.resourceName}`}
                  sx={compactManagementIconButtonSx}
                >
                  <EllipseMenuIcon />
                </IconButton>

                <StackRow sx={{ gridColumn: '2 / -1', display: { xs: 'flex', lg: 'none' }, gap: 0.5, flexWrap: 'wrap' }}>
                  <ResourceType resourceType={item.resourceType} showFullName />
                  {item.zones.length > 0 && <Zones zones={item.zones} hideIcon hideNAText />}
                  <Chip size="small" label={`${item.capacity} ${item.capacity === 1 ? 'person' : 'people'}`} />
                </StackRow>
              </Box>
            </StackColumn>
          </Box>
        );
      })}
    </StackColumn>
  );
};

export default memo(OrganizationLocationResourceManagementList);

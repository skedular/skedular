import { BodyIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow } from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { DeleteIcon, EllipseMenuIcon } from '@/components/icons';
import { ProductTags } from '@/components/productTag';
import { ResourceType } from '@/components/resourceType';
import { Zones } from '@/components/zone';
import {
  compactManagementActionButtonSx,
  compactManagementIconButtonSx,
  compactManagementNeutralChipSx,
  compactManagementWarningChipSx,
  defaultGridActionPadding,
} from '@/libs/theme';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Checkbox from '@mui/material/Checkbox';
import Chip from '@mui/material/Chip';
import Collapse from '@mui/material/Collapse';
import IconButton from '@mui/material/IconButton';
import { memo, useState } from 'react';

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
  productTags: {
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
  const [expandedIds, setExpandedIds] = useState<string[]>([]);

  const handleToggleExpanded = (resourceId: string) => {
    setExpandedIds((current) => (current.includes(resourceId) ? current.filter((id) => id !== resourceId) : current.concat(resourceId)));
  };

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
    <StackColumn spacing={1.5}>
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

      {items.map((item) => {
        const isSelected = selectedIds.includes(item.id);
        const isExpanded = expandedIds.includes(item.id);
        const hasMetadata = item.zones.length > 0 || item.customTags.length > 0 || item.productTags.length > 0;

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
            <StackColumn spacing={1}>
              <StackRow sx={{ alignItems: 'center', gap: 1, flexWrap: 'nowrap', minWidth: 0 }}>
                <Checkbox checked={isSelected} onChange={() => onToggleSelected(item.id)} slotProps={{ input: { 'aria-label': `Select ${item.resourceName}` } }} />

                <StackRow sx={{ gap: 0.75, alignItems: 'center', flex: '1 1 auto', minWidth: 0, overflow: 'hidden', whiteSpace: 'nowrap' }}>
                  <Box sx={{ minWidth: 0, maxWidth: 220, overflow: 'hidden', textOverflow: 'ellipsis' }}>
                    <LeadIconTypography label={item.resourceName} />
                  </Box>
                  <ResourceType resourceType={item.resourceType} showFullName />
                  {hasMetadata && (
                    <>
                      {item.zones.length > 0 && <Zones zones={item.zones} hideIcon hideNAText />}
                      {item.customTags.length > 0 && <CustomTags customTags={item.customTags} hideIcon hideNAText />}
                      {item.productTags.length > 0 && <ProductTags productTags={item.productTags} hideIcon hideNAText />}
                    </>
                  )}
                </StackRow>

                <StackRow sx={{ gap: 0.75, ml: 'auto', alignItems: 'center', flexWrap: 'nowrap', flexShrink: 0 }}>
                  <Chip size="small" label={`Capacity ${item.capacity}`} />
                  {item.isActive ? (
                    <Chip size="small" label="Active" sx={compactManagementNeutralChipSx} />
                  ) : (
                    <Chip size="small" label="Inactive" sx={compactManagementWarningChipSx} />
                  )}
                  <Button variant="text" onClick={() => onOpenResource(item.id)} sx={compactManagementActionButtonSx}>
                    Open
                  </Button>
                  <Button variant="text" onClick={() => handleToggleExpanded(item.id)} sx={compactManagementActionButtonSx}>
                    {isExpanded ? 'Hide details' : 'Details'}
                  </Button>
                  <IconButton
                    onClick={(event: React.MouseEvent<HTMLElement>) => {
                      onOpenMoreActions(item.id, event.currentTarget);
                    }}
                    aria-label={`More actions for ${item.resourceName}`}
                    sx={compactManagementIconButtonSx}
                  >
                    <EllipseMenuIcon />
                  </IconButton>
                </StackRow>
              </StackRow>

              <Collapse in={isExpanded} timeout="auto" unmountOnExit>
                <Box
                  sx={{
                    display: 'grid',
                    gridTemplateColumns: { xs: '1fr', lg: 'repeat(3, minmax(0, 1fr))' },
                    gap: 1.25,
                    pt: 0.5,
                  }}
                >
                  <Box
                    sx={{
                      borderRadius: 2,
                      border: 1,
                      borderColor: 'divider',
                      p: 1.25,
                      backgroundColor: 'background.default',
                    }}
                  >
                    <BodyIconTypography label="Zones" />
                    <Zones zones={item.zones} hideIcon hideNAText={false} sx={{ pt: 1 }} />
                  </Box>
                  <Box
                    sx={{
                      borderRadius: 2,
                      border: 1,
                      borderColor: 'divider',
                      p: 1.25,
                      backgroundColor: 'background.default',
                    }}
                  >
                    <BodyIconTypography label="Custom tags" />
                    <CustomTags customTags={item.customTags} hideIcon hideNAText={false} sx={{ pt: 1 }} />
                  </Box>
                  <Box
                    sx={{
                      borderRadius: 2,
                      border: 1,
                      borderColor: 'divider',
                      p: 1.25,
                      backgroundColor: 'background.default',
                    }}
                  >
                    <BodyIconTypography label="Product tags" />
                    <ProductTags productTags={item.productTags} hideIcon hideNAText={false} sx={{ pt: 1 }} />
                  </Box>
                </Box>
              </Collapse>
            </StackColumn>
          </Box>
        );
      })}
    </StackColumn>
  );
};

export default memo(OrganizationLocationResourceManagementList);

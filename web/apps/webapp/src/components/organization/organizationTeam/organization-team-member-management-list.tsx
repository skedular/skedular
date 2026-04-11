import { CustomerAvatar } from '@/components/avatars';
import { BodyIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow } from '@/components/commons';
import { DeleteIcon, EllipseMenuIcon } from '@/components/icons';
import {
  compactManagementActionButtonSx,
  compactManagementIconButtonSx,
  compactManagementNeutralChipSx,
  compactManagementWarningChipSx,
  defaultButtonStyle,
  defaultGridActionPadding,
} from '@/libs/theme';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Checkbox from '@mui/material/Checkbox';
import Chip from '@mui/material/Chip';
import Collapse from '@mui/material/Collapse';
import IconButton from '@mui/material/IconButton';
import { memo, useState } from 'react';

export type OrganizationTeamMemberManagementListItem = {
  id: string;
  customer: {
    id: string;
    givenName?: string | null;
    middleName?: string | null;
    familyName?: string | null;
    name?: string | null;
    photoUrl?: string | null;
    phoneNumber?: string | null;
  };
  name: string;
  email: string | null | undefined;
  phoneNumber: string | null | undefined;
  role: string | null | undefined;
  isActive: boolean;
};

type Props = {
  items: OrganizationTeamMemberManagementListItem[];
  selectedIds: string[];
  onToggleSelected: (memberId: string) => void;
  onOpenChangeRole: (memberId: string, target: HTMLElement) => void;
  onOpenMoreActions: (memberId: string, target: HTMLElement) => void;
  onDeactivateSelected: (memberIds: string[]) => void;
  onActivateSelected: (memberIds: string[]) => void;
  onRemoveSelected: (memberIds: string[]) => void;
};

const OrganizationTeamMemberManagementList = ({
  items,
  selectedIds,
  onToggleSelected,
  onOpenChangeRole,
  onOpenMoreActions,
  onDeactivateSelected,
  onActivateSelected,
  onRemoveSelected,
}: Props) => {
  const [expandedIds, setExpandedIds] = useState<string[]>([]);

  const handleToggleExpanded = (memberId: string) => {
    setExpandedIds((current) => (current.includes(memberId) ? current.filter((id) => id !== memberId) : current.concat(memberId)));
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
        <LeadIconTypography label="No team members found" />
        <SmallIconTypography label="Adjust the search or add a new team member to this team." />
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
            <SmallIconTypography label={`${selectedIds.length} member${selectedIds.length === 1 ? '' : 's'} selected`} />
            <Box sx={{ flexGrow: 1 }} />
            <Button size="medium" variant="contained" color="secondary" onClick={() => onDeactivateSelected(selectedIds)} sx={defaultButtonStyle}>
              Deactivate Member
            </Button>
            <Button size="medium" variant="contained" color="secondary" onClick={() => onActivateSelected(selectedIds)} sx={defaultButtonStyle}>
              Activate Member
            </Button>
            <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={() => onRemoveSelected(selectedIds)} sx={{ textTransform: 'none' }}>
              Remove Member
            </Button>
          </StackRow>
        </Box>
      )}

      {items.map((item) => {
        const isSelected = selectedIds.includes(item.id);
        const isExpanded = expandedIds.includes(item.id);

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
              <StackRow sx={{ alignItems: 'center', gap: 1, minWidth: 0, flexWrap: 'nowrap' }}>
                <Checkbox checked={isSelected} onChange={() => onToggleSelected(item.id)} slotProps={{ input: { 'aria-label': `Select ${item.name}` } }} />

                <CustomerAvatar name={item.customer} photo={{ url: item.customer.photoUrl }} size="medium" />

                <StackColumn sx={{ minWidth: 0, flex: '1 1 auto' }} spacing={0.35}>
                  <StackRow sx={{ alignItems: 'center', gap: 1, minWidth: 0, flexWrap: 'wrap' }}>
                    <Box sx={{ minWidth: 0, maxWidth: 280, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                      <LeadIconTypography label={item.name} />
                    </Box>
                    <Chip size="small" label={item.isActive ? 'Active' : 'Inactive'} sx={item.isActive ? compactManagementNeutralChipSx : compactManagementWarningChipSx} />
                  </StackRow>
                  <StackRow sx={{ gap: 1, flexWrap: 'wrap' }}>
                    <SmallIconTypography label={`Role: ${item.role || 'Not assigned'}`} sx={{ color: 'text.secondary', fontWeight: 600 }} />
                    {item.email ? <SmallIconTypography label={item.email} /> : null}
                    {item.phoneNumber ? <SmallIconTypography label={item.phoneNumber} /> : null}
                  </StackRow>
                </StackColumn>

                <StackRow sx={{ gap: 0.75, ml: 'auto', alignItems: 'center', flexWrap: 'nowrap', flexShrink: 0 }}>
                  <Button
                    variant="text"
                    onClick={(event: React.MouseEvent<HTMLElement>) => {
                      onOpenChangeRole(item.id, event.currentTarget);
                    }}
                    sx={compactManagementActionButtonSx}
                  >
                    Change role
                  </Button>
                  <Button variant="text" onClick={() => handleToggleExpanded(item.id)} sx={compactManagementActionButtonSx}>
                    {isExpanded ? 'Hide details' : 'Details'}
                  </Button>
                  <IconButton
                    onClick={(event: React.MouseEvent<HTMLElement>) => {
                      onOpenMoreActions(item.id, event.currentTarget);
                    }}
                    aria-label={`More actions for ${item.name}`}
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
                    gridTemplateColumns: { xs: '1fr', md: 'repeat(3, minmax(0, 1fr))' },
                    gap: 1.25,
                    pt: 0.5,
                  }}
                >
                  <Box sx={{ borderRadius: 2, border: 1, borderColor: 'divider', p: 1.25, backgroundColor: 'background.default' }}>
                    <BodyIconTypography label="Contact" />
                    <StackColumn spacing={0.75} sx={{ pt: 1 }}>
                      <SmallIconTypography label={item.email || 'No email'} />
                      <SmallIconTypography label={item.phoneNumber || 'No phone number'} />
                    </StackColumn>
                  </Box>
                  <Box sx={{ borderRadius: 2, border: 1, borderColor: 'divider', p: 1.25, backgroundColor: 'background.default' }}>
                    <BodyIconTypography label="Role" />
                    <StackColumn spacing={0.75} sx={{ pt: 1 }}>
                      <SmallIconTypography label={item.role || 'No role'} />
                    </StackColumn>
                  </Box>
                  <Box sx={{ borderRadius: 2, border: 1, borderColor: 'divider', p: 1.25, backgroundColor: 'background.default' }}>
                    <BodyIconTypography label="Status" />
                    <StackColumn spacing={0.75} sx={{ pt: 1 }}>
                      <SmallIconTypography label={item.isActive ? 'Active' : 'Inactive'} />
                    </StackColumn>
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

export default memo(OrganizationTeamMemberManagementList);

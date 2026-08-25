import { CustomerAvatar } from '@/components/avatars';
import { DeleteIcon, EditIcon, EllipseMenuIcon } from '@/components/icons';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Checkbox from '@mui/material/Checkbox';
import IconButton from '@mui/material/IconButton';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Tooltip from '@mui/material/Tooltip';
import { compactManagementIconButtonSx, defaultButtonStyle, defaultGridActionPadding, LeadIconTypography, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { memo } from 'react';

export type OrganizationUserManagementListItem = {
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
  teams: string[];
  role: string | null | undefined;
  statusName: string;
  isActive: boolean;
};

type Props = {
  items: OrganizationUserManagementListItem[];
  selectedIds: string[];
  onToggleSelected: (memberId: string) => void;
  onOpenProfile: (memberId: string) => void;
  onOpenChangeRole: (memberId: string, target: HTMLElement) => void;
  onOpenMoreActions: (memberId: string, target: HTMLElement) => void;
  onDeactivateSelected: (memberIds: string[]) => void;
  onActivateSelected: (memberIds: string[]) => void;
  onRemoveSelected: (memberIds: string[]) => void;
};

const OrganizationUserManagementList = ({
  items,
  selectedIds,
  onToggleSelected,
  onOpenProfile,
  onOpenChangeRole,
  onOpenMoreActions,
  onDeactivateSelected,
  onActivateSelected,
  onRemoveSelected,
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
        <LeadIconTypography label="No users found" />
        <SmallIconTypography label="Adjust the filters or invite a new person to the organization." />
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
            <SmallIconTypography label={`${selectedIds.length} user${selectedIds.length === 1 ? '' : 's'} selected`} />
            <Box sx={{ flexGrow: 1 }} />
            <Button size="medium" variant="contained" color="secondary" onClick={() => onDeactivateSelected(selectedIds)} sx={defaultButtonStyle}>
              Deactivate User
            </Button>
            <Button size="medium" variant="contained" color="secondary" onClick={() => onActivateSelected(selectedIds)} sx={defaultButtonStyle}>
              Activate User
            </Button>
            <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={() => onRemoveSelected(selectedIds)} sx={{ textTransform: 'none' }}>
              Remove User
            </Button>
          </StackRow>
        </Box>
      )}

      <TableContainer component={Box} sx={{ overflowX: 'auto' }}>
        <Table size="small" aria-label="Organization users" sx={{ minWidth: 640 }}>
          <TableHead>
            <TableRow sx={{ '& th': { fontWeight: 700, textTransform: 'uppercase', letterSpacing: '0.04em', color: 'text.primary', borderBottom: 1, borderColor: 'divider' } }}>
              <TableCell padding="checkbox" />
              <TableCell>Name</TableCell>
              <TableCell>Role</TableCell>
              <TableCell>Contact</TableCell>
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
                  tabIndex={0}
                  onClick={() => onOpenProfile(item.id)}
                  onKeyDown={(event) => {
                    if (event.key === 'Enter' || event.key === ' ') {
                      event.preventDefault();
                      onOpenProfile(item.id);
                    }
                  }}
                  sx={{ cursor: 'pointer', '& td': { py: 1.25, borderBottom: 1, borderColor: 'divider' } }}
                >
                  <TableCell padding="checkbox" onClick={(event) => event.stopPropagation()}>
                    <Checkbox checked={isSelected} onChange={() => onToggleSelected(item.id)} slotProps={{ input: { 'aria-label': `Select ${item.name}` } }} />
                  </TableCell>
                  <TableCell sx={{ minWidth: 300 }}>
                    <StackRow sx={{ alignItems: 'center', gap: 1, minWidth: 0 }}>
                      <CustomerAvatar name={item.customer} photo={{ url: item.customer.photoUrl }} size="medium" />
                      <StackColumn spacing={0.35} sx={{ minWidth: 0 }}>
                        <StackRow sx={{ alignItems: 'center', gap: 1, minWidth: 0, flexWrap: 'wrap' }}>
                          <Box component="span" sx={{ width: 8, height: 8, flex: '0 0 auto', borderRadius: '50%', bgcolor: item.isActive ? 'success.main' : 'text.disabled' }} />
                          <LeadIconTypography label={item.name} sx={{ fontWeight: 600 }} />
                        </StackRow>
                      </StackColumn>
                    </StackRow>
                  </TableCell>
                  <TableCell sx={{ display: { xs: 'none', md: 'table-cell' }, fontWeight: 600 }}>{item.role || 'Not assigned'}</TableCell>
                  <TableCell sx={{ display: { xs: 'none', lg: 'table-cell' }, maxWidth: 280, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                    {item.email || item.phoneNumber || '—'}
                  </TableCell>
                  <TableCell align="right" onClick={(event) => event.stopPropagation()}>
                    <StackRow sx={{ gap: 0.75, justifyContent: 'flex-end', alignItems: 'center', flexWrap: 'nowrap' }}>
                      <Tooltip title="Change role">
                        <IconButton
                          onClick={(event: React.MouseEvent<HTMLElement>) => {
                            onOpenChangeRole(item.id, event.currentTarget);
                          }}
                          aria-label={`Change role for ${item.name}`}
                          sx={compactManagementIconButtonSx}
                        >
                          <EditIcon />
                        </IconButton>
                      </Tooltip>
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
                  </TableCell>
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
      </TableContainer>
    </StackColumn>
  );
};

export default memo(OrganizationUserManagementList);

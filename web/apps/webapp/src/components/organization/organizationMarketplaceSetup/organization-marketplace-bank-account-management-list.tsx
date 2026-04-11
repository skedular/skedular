import { BodyIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow } from '@/components/commons';
import { DeleteIcon, EllipseMenuIcon } from '@/components/icons';
import { defaultGridActionPadding, emerald } from '@/libs/theme';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Checkbox from '@mui/material/Checkbox';
import Chip from '@mui/material/Chip';
import Collapse from '@mui/material/Collapse';
import IconButton from '@mui/material/IconButton';
import { memo, useState } from 'react';

export type OrganizationMarketplaceBankAccountManagementListItem = {
  id: string;
  name: string;
  bankName: string;
  accountHolderName: string;
  accountNumber: string;
  country: string;
  isDefault: boolean;
};

type Props = {
  items: OrganizationMarketplaceBankAccountManagementListItem[];
  selectedIds: string[];
  onToggleSelected: (accountId: string) => void;
  onOpenAccount: (accountId: string) => void;
  onOpenMoreActions: (accountId: string, target: HTMLElement) => void;
  onRemoveSelected: (accountIds: string[]) => void;
};

const OrganizationMarketplaceBankAccountManagementList = ({ items, selectedIds, onToggleSelected, onOpenAccount, onOpenMoreActions, onRemoveSelected }: Props) => {
  const [expandedIds, setExpandedIds] = useState<string[]>([]);

  const handleToggleExpanded = (accountId: string) => {
    setExpandedIds((current) => (current.includes(accountId) ? current.filter((id) => id !== accountId) : current.concat(accountId)));
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
        <LeadIconTypography label="No bank accounts found" />
        <SmallIconTypography label="Add a payout bank account to route marketplace transfers." />
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
            <SmallIconTypography label={`${selectedIds.length} bank account${selectedIds.length === 1 ? '' : 's'} selected`} />
            <Box sx={{ flexGrow: 1 }} />
            <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={() => onRemoveSelected(selectedIds)} sx={{ textTransform: 'none' }}>
              Remove Bank Account
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
              <StackRow sx={{ alignItems: 'center', gap: 1, flexWrap: 'nowrap', minWidth: 0 }}>
                <Checkbox checked={isSelected} onChange={() => onToggleSelected(item.id)} slotProps={{ input: { 'aria-label': `Select ${item.name}` } }} />

                <StackColumn sx={{ minWidth: 0, flex: '1 1 auto' }} spacing={0.35}>
                  <StackRow sx={{ alignItems: 'center', gap: 1, minWidth: 0, flexWrap: 'wrap' }}>
                    <Box sx={{ minWidth: 0, maxWidth: 260, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                      <LeadIconTypography label={item.name} />
                    </Box>
                    {item.isDefault ? (
                      <Chip
                        size="small"
                        label="Default"
                        sx={{
                          backgroundColor: `${emerald}22`,
                          color: emerald,
                        }}
                      />
                    ) : null}
                  </StackRow>
                  <StackRow sx={{ gap: 1, flexWrap: 'wrap' }}>
                    <SmallIconTypography label={item.bankName} />
                    <SmallIconTypography label={item.accountHolderName} />
                    <SmallIconTypography label={item.country} />
                  </StackRow>
                </StackColumn>

                <StackRow sx={{ gap: 0.75, ml: 'auto', alignItems: 'center', flexWrap: 'nowrap', flexShrink: 0 }}>
                  <Button variant="text" onClick={() => onOpenAccount(item.id)} sx={{ textTransform: 'none' }}>
                    Open
                  </Button>
                  <Button variant="text" onClick={() => handleToggleExpanded(item.id)} sx={{ textTransform: 'none', minWidth: 0 }}>
                    {isExpanded ? 'Hide details' : 'Details'}
                  </Button>
                  <IconButton
                    onClick={(event: React.MouseEvent<HTMLElement>) => {
                      onOpenMoreActions(item.id, event.currentTarget);
                    }}
                    aria-label={`More actions for ${item.name}`}
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
                  <Box
                    sx={{
                      borderRadius: 2,
                      border: 1,
                      borderColor: 'divider',
                      p: 1.25,
                      backgroundColor: 'background.default',
                    }}
                  >
                    <BodyIconTypography label="Bank" />
                    <StackColumn spacing={0.75} sx={{ pt: 1 }}>
                      <SmallIconTypography label={item.bankName || 'N/A'} />
                      <SmallIconTypography label={item.country || 'N/A'} />
                    </StackColumn>
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
                    <BodyIconTypography label="Account holder" />
                    <StackColumn spacing={0.75} sx={{ pt: 1 }}>
                      <SmallIconTypography label={item.accountHolderName || 'N/A'} />
                    </StackColumn>
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
                    <BodyIconTypography label="Account number" />
                    <StackColumn spacing={0.75} sx={{ pt: 1 }}>
                      <SmallIconTypography label={item.accountNumber || 'N/A'} />
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

export default memo(OrganizationMarketplaceBankAccountManagementList);

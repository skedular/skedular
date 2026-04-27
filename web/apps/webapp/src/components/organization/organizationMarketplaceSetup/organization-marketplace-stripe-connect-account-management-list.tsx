import { BodyIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { DeleteIcon, EllipseMenuIcon } from '@/components/icons';
import { CompleteOnboardStripeConnectAccountButton } from '@/components/stripeConnectAccount';
import { compactManagementActionButtonSx, compactManagementIconButtonSx, compactManagementNeutralChipSx, defaultGridActionPadding } from '@skedular/ui';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Checkbox from '@mui/material/Checkbox';
import Chip from '@mui/material/Chip';
import Collapse from '@mui/material/Collapse';
import IconButton from '@mui/material/IconButton';
import { memo, useState } from 'react';

export type OrganizationMarketplaceStripeConnectAccountManagementListItem = {
  id: string;
  name: string;
  companyName: string | null | undefined;
  country: string;
  defaultCurrency: string | null | undefined;
  businessType: string | null | undefined;
  website: string | null | undefined;
  supportLink: string | null | undefined;
  contactEmail: string | null | undefined;
  contactPhone: string | null | undefined;
  chargesEnabled: boolean;
  payoutsEnabled: boolean;
  detailsSubmitted: boolean;
  isAuthorized: boolean;
  isDefault: boolean;
  requiresOnboarding: boolean;
  onboardingUrl: string;
};

type Props = {
  items: OrganizationMarketplaceStripeConnectAccountManagementListItem[];
  selectedIds: string[];
  onToggleSelected: (accountId: string) => void;
  onOpenAccount: (accountId: string) => void;
  onOpenMoreActions: (accountId: string, target: HTMLElement) => void;
  onRemoveSelected: (accountIds: string[]) => void;
};

const OrganizationMarketplaceStripeConnectAccountManagementList = ({ items, selectedIds, onToggleSelected, onOpenAccount, onOpenMoreActions, onRemoveSelected }: Props) => {
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
        <LeadIconTypography label="No Stripe accounts found" />
        <SmallIconTypography label="Connect a Stripe account to configure marketplace payouts and onboarding." />
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
            <SmallIconTypography label={`${selectedIds.length} Stripe account${selectedIds.length === 1 ? '' : 's'} selected`} />
            <Box sx={{ flexGrow: 1 }} />
            <Button size="medium" variant="contained" color="warning" startIcon={<DeleteIcon />} onClick={() => onRemoveSelected(selectedIds)} sx={{ textTransform: 'none' }}>
              Remove Stripe Connect Account
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
                    {item.isDefault ? <Chip size="small" label="Default" sx={compactManagementNeutralChipSx} /> : null}
                    {item.defaultCurrency ? <Chip size="small" label={item.defaultCurrency} /> : null}
                  </StackRow>
                  <StackRow sx={{ gap: 1, flexWrap: 'wrap' }}>
                    {item.companyName ? <SmallIconTypography label={item.companyName} /> : null}
                    <SmallIconTypography label={item.country || 'N/A'} />
                    {item.businessType ? <SmallIconTypography label={item.businessType} /> : null}
                  </StackRow>
                </StackColumn>

                <StackRow sx={{ gap: 0.75, ml: 'auto', alignItems: 'center', flexWrap: 'nowrap', flexShrink: 0 }}>
                  {item.requiresOnboarding ? <CompleteOnboardStripeConnectAccountButton onboardingUrl={item.onboardingUrl} variant="contained" size="small" /> : null}
                  <Button variant="text" onClick={() => onOpenAccount(item.id)} sx={compactManagementActionButtonSx}>
                    Open
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
                    <BodyIconTypography label="Business profile" />
                    <StackColumn spacing={0.75} sx={{ pt: 1 }}>
                      <SmallIconTypography label={item.companyName || 'N/A'} />
                      <SmallIconTypography label={item.businessType || 'N/A'} />
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
                    <BodyIconTypography label="Support & contact" />
                    <StackColumn spacing={0.75} sx={{ pt: 1 }}>
                      <SmallIconTypography label={item.contactEmail || 'No contact email'} />
                      <SmallIconTypography label={item.contactPhone || 'No contact phone'} />
                      <SmallIconTypography label={item.supportLink || 'No support link'} />
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
                    <BodyIconTypography label="Connection status" />
                    <StackColumn spacing={0.75} sx={{ pt: 1 }}>
                      <SmallIconTypography label={`Authorized: ${item.isAuthorized ? 'Yes' : 'No'}`} />
                      <SmallIconTypography label={`Details submitted: ${item.detailsSubmitted ? 'Yes' : 'No'}`} />
                      <SmallIconTypography label={`Charges enabled: ${item.chargesEnabled ? 'Yes' : 'No'}`} />
                      <SmallIconTypography label={`Payouts enabled: ${item.payoutsEnabled ? 'Yes' : 'No'}`} />
                      <SmallIconTypography label={item.website || 'No website'} />
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

export default memo(OrganizationMarketplaceStripeConnectAccountManagementList);

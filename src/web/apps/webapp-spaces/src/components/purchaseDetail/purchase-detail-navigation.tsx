'use client';

import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { useMemo, useState, type MouseEvent } from 'react';

export type PurchaseDetailTab = 'overview' | 'billing' | 'refunds' | 'bookings';

type Props = {
  hasLinkedBookings?: boolean;
};

const tabs: Array<{ id: PurchaseDetailTab; label: string }> = [
  { id: 'overview', label: 'Overview' },
  { id: 'billing', label: 'Billing periods' },
  { id: 'refunds', label: 'Refunds' },
  { id: 'bookings', label: 'Linked bookings' },
];

export const PurchaseDetailNavigation = ({ hasLinkedBookings = true }: Props) => {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'), { noSsr: true });
  const [menuAnchor, setMenuAnchor] = useState<HTMLElement | null>(null);
  const activeTab: PurchaseDetailTab =
    searchParams.get('tab') === 'bookings' ? 'bookings' : searchParams.get('tab') === 'billing' ? 'billing' : searchParams.get('tab') === 'refunds' ? 'refunds' : 'overview';
  const visibleTabs = useMemo(() => (hasLinkedBookings ? tabs : tabs.filter((tab) => tab.id !== 'bookings')), [hasLinkedBookings]);

  const selectTab = (tab: PurchaseDetailTab) => {
    const nextParams = new URLSearchParams(searchParams.toString());
    nextParams.set('tab', tab);
    nextParams.delete('section');
    const query = nextParams.toString();
    router.replace(`${pathname}${query ? `?${query}` : ''}`, { scroll: false });
    document.getElementById(`purchase-section-${tab}`)?.scrollIntoView({ behavior: 'smooth', block: 'start' });
  };

  const handleOpenMenu = (event: MouseEvent<HTMLElement>) => setMenuAnchor(event.currentTarget);
  const handleCloseMenu = () => setMenuAnchor(null);

  return (
    <Box sx={{ borderTop: 1, borderColor: 'divider', pt: { xs: 1.5, md: 0 } }}>
      {isMobile ? (
        <>
          <Button
            fullWidth
            variant="outlined"
            color="inherit"
            onClick={handleOpenMenu}
            aria-haspopup="menu"
            aria-expanded={menuAnchor ? 'true' : undefined}
            aria-controls={menuAnchor ? 'purchase-detail-sections-menu' : undefined}
            sx={{ justifyContent: 'space-between', minHeight: 48, borderRadius: 2.5, px: 2, textTransform: 'none' }}
          >
            {`Tab: ${visibleTabs.find((tab) => tab.id === activeTab)?.label ?? 'Overview'}`}
          </Button>
          <Menu anchorEl={menuAnchor} open={Boolean(menuAnchor)} onClose={handleCloseMenu} id="purchase-detail-sections-menu">
            {visibleTabs.map((tab) => (
              <MenuItem
                key={tab.id}
                selected={activeTab === tab.id}
                onClick={() => {
                  handleCloseMenu();
                  selectTab(tab.id);
                }}
              >
                {tab.label}
              </MenuItem>
            ))}
          </Menu>
        </>
      ) : (
        <Tabs
          value={activeTab}
          onChange={(_, value: PurchaseDetailTab) => selectTab(value)}
          variant="scrollable"
          scrollButtons="auto"
          aria-label="Purchase detail sections"
          sx={{ mb: -2, '& .MuiTabs-indicator': { height: 3, borderRadius: '3px 3px 0 0' } }}
        >
          {visibleTabs.map((tab) => (
            <Tab
              key={tab.id}
              value={tab.id}
              label={tab.label}
              disableRipple
              sx={{
                minWidth: 112,
                minHeight: 52,
                px: 2.5,
                textTransform: 'none',
                whiteSpace: 'nowrap',
                color: 'text.secondary',
                fontWeight: 500,
                '&.Mui-selected': { color: 'primary.main', fontWeight: 600 },
                '&:hover': { color: 'text.primary', backgroundColor: 'action.hover' },
              }}
            />
          ))}
        </Tabs>
      )}
    </Box>
  );
};

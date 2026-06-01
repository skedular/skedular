'use client';

import AppsIcon from '@mui/icons-material/Apps';
import type { ButtonProps } from '@mui/material/Button';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import IconButton from '@mui/material/IconButton';
import ListItemText from '@mui/material/ListItemText';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Stack from '@mui/material/Stack';
import type { SxProps, Theme } from '@mui/system';
import { useId, useMemo, useState } from 'react';

export type AppSwitcherDestinationAvailability = 'available' | 'current' | 'missing-url' | 'invalid-url';

export type AppSwitcherDestination = {
  appId: string;
  displayName: string;
  shortName: string;
  href?: string;
  isCurrent: boolean;
  availability: AppSwitcherDestinationAvailability;
};

export type AppSwitcherModel = {
  currentAppId: string;
  destinations: readonly AppSwitcherDestination[];
  availableDestinationCount: number;
  hasSwitchTargets: boolean;
};

type Props = {
  model: AppSwitcherModel;
  buttonSx?: SxProps<Theme>;
  buttonVariant?: ButtonProps['variant'];
  buttonMode?: 'button' | 'icon' | 'menu-item';
  onDestinationSelect?: (destination: AppSwitcherDestination) => void;
};

const AppSwitcher = ({ model, buttonSx, buttonVariant = 'outlined', buttonMode = 'button', onDestinationSelect }: Props) => {
  const menuId = useId();
  const [anchorElement, setAnchorElement] = useState<HTMLElement | null>(null);
  const activeDestinations = useMemo(() => model.destinations.filter((destination) => destination.availability === 'available'), [model.destinations]);
  const currentDestination = model.destinations.find((destination) => destination.isCurrent);

  if (!model.hasSwitchTargets || activeDestinations.length === 0) {
    return null;
  }

  const isOpen = anchorElement != null;
  const buttonAriaLabel = 'Switch app';

  return (
    <>
      {buttonMode === 'menu-item' ? (
        <MenuItem aria-controls={isOpen ? menuId : undefined} aria-expanded={isOpen} aria-haspopup="menu" onClick={(event) => setAnchorElement(event.currentTarget)}>
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
            <AppsIcon fontSize="small" />
            <ListItemText primary={buttonAriaLabel} />
          </Stack>
        </MenuItem>
      ) : buttonMode === 'icon' ? (
        <IconButton
          aria-controls={isOpen ? menuId : undefined}
          aria-expanded={isOpen}
          aria-haspopup="menu"
          aria-label={buttonAriaLabel}
          onClick={(event) => setAnchorElement(event.currentTarget)}
          size="small"
          sx={buttonSx}
        >
          <AppsIcon fontSize="small" />
        </IconButton>
      ) : (
        <Button
          aria-controls={isOpen ? menuId : undefined}
          aria-expanded={isOpen}
          aria-haspopup="menu"
          onClick={(event) => setAnchorElement(event.currentTarget)}
          startIcon={<AppsIcon fontSize="small" />}
          variant={buttonVariant}
          size="small"
          sx={buttonSx}
        >
          {buttonAriaLabel}
        </Button>
      )}
      <Menu id={menuId} anchorEl={anchorElement} open={isOpen} onClose={() => setAnchorElement(null)} slotProps={{ list: { 'aria-label': 'Skedular app switcher' } }}>
        {currentDestination ? (
          <MenuItem disabled>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', minWidth: 220 }}>
              <ListItemText primary={currentDestination.displayName} secondary="Current app" />
              <Chip label="Current" size="small" />
            </Stack>
          </MenuItem>
        ) : null}
        {activeDestinations.map((destination) => (
          <MenuItem
            key={destination.appId}
            component="a"
            href={destination.href}
            onClick={() => {
              onDestinationSelect?.(destination);
              console.info({
                event: 'web_app_switcher_selection',
                currentAppId: model.currentAppId,
                destinationAppId: destination.appId,
              });
              setAnchorElement(null);
            }}
          >
            <ListItemText primary={destination.displayName} secondary="Open app" />
          </MenuItem>
        ))}
      </Menu>
    </>
  );
};

export default AppSwitcher;

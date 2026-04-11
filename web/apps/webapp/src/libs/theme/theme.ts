import type { CSSProperties } from '@mui/material/styles';
import { createTheme as createMuiTheme, PaletteMode, Theme } from '@mui/material/styles';
import type { ResponsiveStyleValue, SxProps } from '@mui/system';
import { gridClasses } from '@mui/x-data-grid';
import getDesignTokens, { coal } from './theme-primitives';

export const defaultButtonStyle: SxProps<Theme> = {
  backgroundColor: 'white',
  borderColor: coal,
  borderWidth: 1,
  borderStyle: 'solid',
  textTransform: 'none',
};

export const compactManagementActionButtonSx: SxProps<Theme> = {
  textTransform: 'none',
  minWidth: 0,
  color: 'text.primary',
  fontWeight: 600,
  px: 1.25,
  py: 0.5,
  minHeight: 32,
  borderRadius: 2,
  border: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.16)' : theme.palette.divider),
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(248, 250, 252, 0.96)' : theme.palette.background.paper),
  boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 1px 2px rgba(15, 23, 42, 0.06)' : 'none'),
  transition: 'background-color 120ms ease, border-color 120ms ease, box-shadow 120ms ease, transform 120ms ease',
  '&:hover': {
    backgroundColor: (theme) => theme.palette.action.hover,
    borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.28)' : theme.palette.text.secondary),
    boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 3px 10px rgba(15, 23, 42, 0.08)' : 'none'),
  },
  '&:focus-visible': {
    outline: '2px solid rgba(15, 23, 42, 0.35)',
    outlineOffset: 2,
  },
};

export const compactManagementIconButtonSx: SxProps<Theme> = {
  width: 32,
  height: 32,
  borderRadius: 2,
  border: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.16)' : theme.palette.divider),
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(248, 250, 252, 0.96)' : theme.palette.background.paper),
  color: 'text.primary',
  boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 1px 2px rgba(15, 23, 42, 0.06)' : 'none'),
  transition: 'background-color 120ms ease, border-color 120ms ease, box-shadow 120ms ease',
  '&:hover': {
    backgroundColor: (theme) => theme.palette.action.hover,
    borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.28)' : theme.palette.text.secondary),
    boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 3px 10px rgba(15, 23, 42, 0.08)' : 'none'),
  },
  '&:focus-visible': {
    outline: '2px solid rgba(15, 23, 42, 0.35)',
    outlineOffset: 2,
  },
};

export const compactManagementNeutralChipSx: SxProps<Theme> = {
  border: 1,
  borderRadius: 999,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.14)' : theme.palette.divider),
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.05)' : theme.palette.action.hover),
  color: 'text.primary',
  fontWeight: 600,
};

export const compactManagementWarningChipSx: SxProps<Theme> = {
  border: 1,
  borderRadius: 999,
  borderColor: 'rgba(239, 68, 68, 0.24)',
  backgroundColor: 'rgba(239, 68, 68, 0.08)',
  color: 'error.main',
  fontWeight: 600,
};

export const defaultPadding: ResponsiveStyleValue<CSSProperties['paddingTop']> = { xs: 1, sm: 1, md: 3 };
export const defaultGridActionPadding: ResponsiveStyleValue<CSSProperties['paddingTop']> = { xs: 1, sm: 1, md: 2 };
export const maxScreenWidth = 1700;

export const defaultGridStyle: SxProps<Theme> = {
  border: 'none',
  [`& .${gridClasses.cell}`]: {
    border: 'none',
  },
  [`& .${gridClasses.row}`]: {
    borderRadius: 2,
    backgroundColor: (theme) => theme.palette.background.paper,
    border: 'none',
  },
};

export const defaultOldGridStyle: SxProps<Theme> = {
  border: 'none',
  [`& .${gridClasses.cell}`]: {
    border: 'none',
  },
  [`& .${gridClasses.row}`]: {
    borderRadius: 2,
    backgroundColor: (theme) => theme.palette.background.paper,
    border: 'none',
  },
};

export const selectedListItemPaddings = { paddingRight: 5 };
export const getSelectedListItemBorderRadius = (selected: boolean): number => (selected ? 4 : 0);

const createTheme = (mode: PaletteMode): Theme => createMuiTheme(getDesignTokens(mode));

export const secondDrawerExpandedDrawerWidth = 210;
export const secondDrawerCollapsedDrawerWidth = 80;
export const secondDrawerExpandedDrawerWidthPx = '230px';
export const secondDrawerCollapsedDrawerWidthPx = '100px';

export default createTheme;

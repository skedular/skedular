import type { GridRowSelectionModel } from '@mui/x-data-grid';

export { default as MuiXLicense } from './muix-license';

export const defaultGridRowSelectionModelValue: GridRowSelectionModel = { type: 'include', ids: new Set() };

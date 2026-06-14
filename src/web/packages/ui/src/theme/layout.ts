import type { ResponsiveStyleValue } from '@mui/system';
import type { CSSProperties } from 'react';

export const defaultPadding: ResponsiveStyleValue<CSSProperties['paddingTop']> = { xs: 1, sm: 1, md: 3 };
export const defaultGridActionPadding: ResponsiveStyleValue<CSSProperties['paddingTop']> = { xs: 1, sm: 1, md: 2 };
export const maxScreenWidth = 1700;

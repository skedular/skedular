import { createTheme, PaletteMode, Shadows } from '@mui/material/styles';

const barlowFontFamily = ['Barlow'].join(',');
const interFontFamily = ['Inter'].join(',');
const defaultTheme = createTheme();
const customShadows: Shadows = [...defaultTheme.shadows];

export const coal = 'rgb(30, 33, 48)';
export const emerald = 'rgb(116, 215, 126)';
export const sandstone = 'rgb(239, 238, 235)';
export const violet = 'rgb(202,185,255)';
export const aqua = 'rgb(161,217,232)';
export const subbeam = 'rgb(249,214,110)';
export const flame = 'rgb(254,147,111)';

// emerald
export const brand1 = {
  50: 'hsl(126, 55%, 48%)',
  100: 'hsl(126, 55%, 48%)',
  200: 'hsl(126, 55%, 48%)',
  300: 'hsl(126, 55%, 48%)',
  400: 'hsl(126, 55%, 48%)',
  500: 'hsl(126, 55%, 48%)',
  600: 'hsl(126, 55%, 48%)',
  700: 'hsl(126, 55%, 48%)',
  800: 'hsl(126, 55%, 48%)',
  900: 'hsl(126, 55%, 48%)',
};

// sandstone
export const brand2 = {
  50: 'hsl(45, 11%, 92%)',
  100: 'hsl(45, 11%, 92%)',
  200: 'hsl(45, 11%, 92%)',
  300: 'hsl(45, 11%, 92%)',
  400: 'hsl(45, 11%, 92%)',
  500: 'hsl(45, 11%, 92%)',
  600: 'hsl(45, 11%, 92%)',
  700: 'hsl(45, 11%, 92%)',
  800: 'hsl(45, 11%, 92%)',
  900: 'hsl(45, 11%, 92%)',
};

// coal
export const gray = {
  50: 'hsl(230, 23%, 15%)',
  100: 'hsl(230, 23%, 15%)',
  200: 'hsl(220, 23%, 15%)',
  300: 'hsl(220, 23%, 15%)',
  400: 'hsl(220, 23%, 15%)',
  500: 'hsl(220, 23%, 15%)',
  600: 'hsl(220, 23%, 15%)',
  700: 'hsl(220, 23%, 15%)',
  800: 'hsl(220, 23%, 15%)',
  900: 'hsl(220, 23%, 15%)',
};

// flame
export const warning = {
  50: 'hsl(45, 92%, 70%)',
  100: 'hsl(45, 92%, 70%)',
  200: 'hsl(45, 92%, 70%)',
  300: 'hsl(45, 92%, 70%)',
  400: 'hsl(45, 92%, 70%)',
  500: 'hsl(45, 92%, 70%)',
  600: 'hsl(45, 92%, 70%)',
  700: 'hsl(45, 92%, 70%)',
  800: 'hsl(45, 92%, 70%)',
  900: 'hsl(45, 92%, 70%)',
};

// flame
export const error = {
  50: 'hsl(15, 98%, 71%)',
  100: 'hsl(15, 98%, 71%)',
  200: 'hsl(15, 98%, 71%)',
  300: 'hsl(15, 98%, 71%)',
  400: 'hsl(15, 98%, 71%)',
  500: 'hsl(15, 98%, 71%)',
  600: 'hsl(15, 98%, 71%)',
  700: 'hsl(15, 98%, 71%)',
  800: 'hsl(15, 98%, 71%)',
  900: 'hsl(15, 98%, 71%)',
};

const getDesignTokens = (mode: PaletteMode) => {
  customShadows[1] =
    mode === 'dark'
      ? 'hsla(220, 30%, 5%, 0.7) 0px 4px 16px 0px, hsla(220, 25%, 10%, 0.8) 0px 8px 16px -5px'
      : 'hsla(220, 30%, 5%, 0.07) 0px 4px 16px 0px, hsla(220, 25%, 10%, 0.07) 0px 8px 16px -5px';

  return {
    cssVariables: true,
    palette: {
      mode,
      primaryAction: {
        light: brand1[300],
        main: brand1[400],
        dark: brand1[800],
        ...(mode === 'dark' && {
          light: brand1[400],
          main: brand1[500],
          dark: brand1[700],
        }),
      },
      secondaryAction: {
        light: brand2[300],
        main: brand2[400],
        dark: brand2[800],
        ...(mode === 'dark' && {
          light: brand2[400],
          main: brand2[500],
          dark: brand2[700],
        }),
      },
      grey: {
        ...(mode === 'dark' ? brand2 : gray),
      },
      background: {
        default: mode === 'dark' ? coal : sandstone,
        paper: mode === 'dark' ? coal : defaultTheme.palette.background.paper,
      },
      text: {
        primary: mode === 'dark' ? sandstone : coal,
        secondary: mode === 'dark' ? sandstone : coal,
        warning: mode === 'dark' ? sandstone : coal,
      },
    },
    shape: {
      borderRadius: 8,
    },
    shadows: customShadows,
    typography: {
      interFontFamily,
      h1: {
        fontSize: defaultTheme.typography.pxToRem(52),
        fontWeight: 800,
        lineHeight: 1.2,
        letterSpacing: -0.5,
        fontFamily: barlowFontFamily,
      },
      h2: {
        fontSize: defaultTheme.typography.pxToRem(40),
        fontWeight: 800,
        lineHeight: 1.2,
        fontFamily: barlowFontFamily,
      },
      h3: {
        fontSize: defaultTheme.typography.pxToRem(34),
        fontWeight: 800,
        lineHeight: 1.2,
        fontFamily: barlowFontFamily,
      },
      h4: {
        fontSize: defaultTheme.typography.pxToRem(28),
        fontWeight: 700,
        lineHeight: 1.5,
        fontFamily: barlowFontFamily,
      },
      h5: {
        fontSize: defaultTheme.typography.pxToRem(24),
        fontWeight: 700,
        fontFamily: barlowFontFamily,
      },
      h6: {
        fontSize: defaultTheme.typography.pxToRem(18),
        fontWeight: 700,
        fontFamily: interFontFamily,
      },
      subtitle1: {
        fontSize: defaultTheme.typography.pxToRem(18),
        fontWeight: 600,
        fontFamily: interFontFamily,
      },
      subtitle2: {
        fontSize: defaultTheme.typography.pxToRem(16),
        fontWeight: 400,
        fontFamily: interFontFamily,
      },
      body1: {
        fontSize: defaultTheme.typography.pxToRem(16),
        fontWeight: 500,
        fontFamily: interFontFamily,
      },
      body2: {
        fontSize: defaultTheme.typography.pxToRem(14),
        fontWeight: 400,
        fontFamily: interFontFamily,
      },
      caption: {
        fontSize: defaultTheme.typography.pxToRem(14),
        fontWeight: 400,
        fontFamily: interFontFamily,
      },
    },
    components: {
      MuiTypography: {
        styleOverrides: {
          root: {
            color: mode === 'dark' ? sandstone : coal,
          },
        },
      },
      MuiLink: {
        styleOverrides: {
          root: {
            textDecoration: 'none',
            '&:hover': {
              textDecoration: 'none',
            },
          },
        },
      },
      MuiCard: {
        styleOverrides: {
          root: {
            boxShadow: 'none',
          },
        },
      },
      MuiPaper: {
        styleOverrides: {
          root: {
            boxShadow: 'none',
          },
        },
      },
      MuiCardHeader: {
        styleOverrides: {
          root: {
            backgroundColor: mode === 'dark' ? sandstone : coal,
          },
        },
      },
    },
  };
};

export default getDesignTokens;

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
export const sunbeam = 'rgb(249,214,110)';
export const flame = 'rgb(254,147,111)';

// emerald
export const brand1 = {
  50: 'hsl(126, 62%, 95%)',
  100: 'hsl(126, 58%, 90%)',
  200: 'hsl(126, 56%, 82%)',
  300: 'hsl(126, 55%, 73%)',
  400: 'hsl(126, 55%, 65%)',
  500: 'hsl(126, 55%, 48%)',
  600: 'hsl(126, 52%, 41%)',
  700: 'hsl(126, 48%, 34%)',
  800: 'hsl(126, 44%, 27%)',
  900: 'hsl(126, 40%, 20%)',
};

// sandstone
export const brand2 = {
  50: 'hsl(45, 25%, 98%)',
  100: 'hsl(45, 18%, 96%)',
  200: 'hsl(45, 14%, 94%)',
  300: 'hsl(45, 11%, 92%)',
  400: 'hsl(45, 10%, 88%)',
  500: 'hsl(45, 9%, 82%)',
  600: 'hsl(45, 8%, 72%)',
  700: 'hsl(45, 8%, 60%)',
  800: 'hsl(45, 8%, 48%)',
  900: 'hsl(45, 8%, 36%)',
};

// coal
export const gray = {
  50: 'hsl(225, 20%, 96%)',
  100: 'hsl(226, 20%, 91%)',
  200: 'hsl(227, 18%, 83%)',
  300: 'hsl(228, 17%, 72%)',
  400: 'hsl(229, 16%, 58%)',
  500: 'hsl(230, 15%, 45%)',
  600: 'hsl(230, 17%, 33%)',
  700: 'hsl(230, 19%, 24%)',
  800: 'hsl(230, 21%, 18%)',
  900: 'hsl(230, 23%, 15%)',
};

// sunbeam
export const warning = {
  50: 'hsl(45, 95%, 96%)',
  100: 'hsl(45, 94%, 91%)',
  200: 'hsl(45, 93%, 84%)',
  300: 'hsl(45, 92%, 77%)',
  400: 'hsl(45, 92%, 70%)',
  500: 'hsl(45, 85%, 60%)',
  600: 'hsl(45, 78%, 51%)',
  700: 'hsl(45, 70%, 43%)',
  800: 'hsl(45, 64%, 35%)',
  900: 'hsl(45, 58%, 28%)',
};

// flame
export const error = {
  50: 'hsl(16, 100%, 96%)',
  100: 'hsl(16, 97%, 90%)',
  200: 'hsl(16, 95%, 82%)',
  300: 'hsl(16, 94%, 73%)',
  400: 'hsl(16, 94%, 64%)',
  500: 'hsl(16, 88%, 55%)',
  600: 'hsl(16, 80%, 47%)',
  700: 'hsl(16, 72%, 40%)',
  800: 'hsl(16, 64%, 33%)',
  900: 'hsl(16, 56%, 27%)',
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
      primary: {
        light: brand1[300],
        main: brand1[400],
        dark: brand1[800],
        ...(mode === 'dark' && {
          light: brand1[400],
          main: brand1[500],
          dark: brand1[700],
        }),
      },
      secondary: {
        light: brand2[300],
        main: brand2[400],
        dark: brand2[800],
        ...(mode === 'dark' && {
          light: brand2[400],
          main: brand2[500],
          dark: brand2[700],
        }),
      },
      warning: {
        main: warning[500],
        light: warning[300],
        dark: warning[700],
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
        fontWeight: 700,
        lineHeight: 1.2,
        letterSpacing: -0.5,
        fontFamily: barlowFontFamily,
      },
      h2: {
        fontSize: defaultTheme.typography.pxToRem(40),
        fontWeight: 700,
        lineHeight: 1.2,
        fontFamily: barlowFontFamily,
      },
      h3: {
        fontSize: defaultTheme.typography.pxToRem(34),
        fontWeight: 600,
        lineHeight: 1.2,
        fontFamily: barlowFontFamily,
      },
      h4: {
        fontSize: defaultTheme.typography.pxToRem(28),
        fontWeight: 500,
        lineHeight: 1.4,
        fontFamily: interFontFamily,
      },
      h5: {
        fontSize: defaultTheme.typography.pxToRem(24),
        fontWeight: 600,
        fontFamily: barlowFontFamily,
      },
      h6: {
        fontSize: defaultTheme.typography.pxToRem(18),
        fontWeight: 500,
        fontFamily: interFontFamily,
      },
      subtitle1: {
        fontSize: defaultTheme.typography.pxToRem(18),
        fontWeight: 500,
        fontFamily: interFontFamily,
      },
      subtitle2: {
        fontSize: defaultTheme.typography.pxToRem(16),
        fontWeight: 400,
        fontFamily: interFontFamily,
      },
      body1: {
        fontSize: defaultTheme.typography.pxToRem(16),
        fontWeight: 400,
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
      MuiButton: {
        styleOverrides: {
          root: {
            borderRadius: 16,
          },
        },
      },
      MuiTextField: {
        styleOverrides: {
          root: {
            maxWidth: 600,
            ['& .MuiOutlinedInput-root']: {
              backgroundColor: mode === 'dark' ? coal : defaultTheme.palette.background.paper,
              borderRadius: 12,
            },
          },
        },
      },
    },
  };
};

export default getDesignTokens;

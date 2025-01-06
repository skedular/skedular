import AppBar from '@mui/material/AppBar';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import Toolbar from '@mui/material/Toolbar';
import { Theme } from '@mui/material/styles';
import type { SxProps } from '@mui/system';
import { ResponsiveStyleValue } from '@mui/system';
import { useContext, type PropsWithChildren } from 'react';
import { PaletteModeContext } from '../../libs/providers';
import { coal, defaultPadding, maxScreenWidth, sandstone } from '../../libs/theme';
import PushToRight from './push-to-right';
import SmallHeadingIconTypography from './small-heading-icon-typography';
import SmallIconTypography from './small-icon-typography';
import StackColumn from './stack-column';
import StackRow from './stack-row';

interface AnyObject {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  [key: string]: any;
}

type Props = {
  sx?: SxProps<Theme>;
  spacing?: ResponsiveStyleValue<number | string>;
  label: string;
  onSubmit?: (event?: Partial<Pick<React.SyntheticEvent, 'preventDefault' | 'stopPropagation'>>) => Promise<AnyObject | undefined> | undefined;
  onCancel?: () => void;
  hideCancel?: boolean;
  hideSaveAndExit?: boolean;
  useChildrenPadding?: boolean;
};

const StackColumnWithSaveExitCancelAppBar = ({
  children,
  sx,
  spacing,
  label,
  onSubmit,
  onCancel,
  hideCancel,
  hideSaveAndExit,
  useChildrenPadding,
}: PropsWithChildren<Props>) => {
  const paletteMode = useContext(PaletteModeContext);
  const childrenSx = useChildrenPadding ? { maxWidth: maxScreenWidth, padding: defaultPadding } : { maxWidth: maxScreenWidth };

  return (
    <Stack
      direction="column"
      spacing={spacing === undefined ? 1 : spacing}
      sx={{ padding: 0, ...sx }}
      component="form"
      noValidate
      onSubmit={onSubmit}
    >
      <AppBar position="sticky">
        <Toolbar
          sx={{
            backgroundColor: paletteMode === 'dark' ? sandstone : coal,
            borderBottom: paletteMode === 'dark' ? 1 : undefined,
            borderColor: (theme) => theme.palette.divider,
          }}
        >
          <SmallHeadingIconTypography label={label} invertDefaultColor />

          <PushToRight />
          {!hideCancel && (
            <StackRow>
              <Button
                sx={{
                  border: 1,
                  borderColor: paletteMode === 'dark' ? coal : sandstone,
                  textTransform: 'none',
                  '&:hover': {
                    backgroundColor: 'inherit',
                  },
                }}
                variant="contained"
                color="inherit"
                onClick={onCancel}
              >
                <SmallIconTypography label="Cancel" invertDefaultColor />
              </Button>

              {!hideSaveAndExit && (
                <Button variant="contained" color="primary" type="submit" sx={{ textTransform: 'none' }}>
                  <SmallIconTypography label="Save & Exit" />
                </Button>
              )}
            </StackRow>
          )}
        </Toolbar>
      </AppBar>

      <StackColumn sx={childrenSx}>{children}</StackColumn>
    </Stack>
  );
};

export default StackColumnWithSaveExitCancelAppBar;

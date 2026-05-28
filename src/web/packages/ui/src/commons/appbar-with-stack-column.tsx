'use client';

import AppBar from '@mui/material/AppBar';
import Button from '@mui/material/Button';
import Toolbar from '@mui/material/Toolbar';
import { useContext, type PropsWithChildren } from 'react';
import StackColumn from '../stack-column';
import StackRow from '../stack-row';
import { PaletteModeContext } from '../theme/palette-mode-context';
import { defaultPadding } from '../theme/theme';
import { coal, sandstone } from '../theme/theme-primitives';
import SmallHeadingIconTypography from '../typography/small-heading-icon-typography';
import SmallIconTypography from '../typography/small-icon-typography';
import PushToRight from './push-to-right';

type Props = {
  label: string;
  onClose?: () => void;
  hideClose?: boolean;
};

const AppBarWithStackColumn = ({ children, label, onClose, hideClose }: PropsWithChildren<Props>) => {
  const paletteMode = useContext(PaletteModeContext);

  return (
    <StackColumn>
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
          <StackRow>
            {!hideClose && (
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
                onClick={onClose}
              >
                <SmallIconTypography label="Close" invertDefaultColor />
              </Button>
            )}
          </StackRow>
        </Toolbar>
      </AppBar>

      <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', paddingBottom: defaultPadding }}>{children}</StackColumn>
    </StackColumn>
  );
};

export default AppBarWithStackColumn;

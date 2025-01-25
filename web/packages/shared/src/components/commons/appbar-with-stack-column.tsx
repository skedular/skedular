import AppBar from '@mui/material/AppBar';
import Button from '@mui/material/Button';
import Toolbar from '@mui/material/Toolbar';
import { useContext, type PropsWithChildren } from 'react';
import { PaletteModeContext } from '../../libs/providers';
import { coal, defaultPadding, maxScreenWidth, sandstone } from '../../libs/theme';
import PushToRight from './push-to-right';
import SmallHeadingIconTypography from './small-heading-icon-typography';
import SmallIconTypography from './small-icon-typography';
import StackColumn from './stack-column';
import StackRow from './stack-row';

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

      <StackColumn sx={{ maxWidth: maxScreenWidth, paddingBottom: defaultPadding }}>{children}</StackColumn>
    </StackColumn>
  );
};

export default AppBarWithStackColumn;

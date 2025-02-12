import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle } from '@/libs/theme';
import Button from '@mui/material/Button';
import DialogActions from '@mui/material/DialogActions';
import type { SxProps, Theme } from '@mui/system';
import { useContext } from 'react';
import BodyIconTypography from './body-icon-typography';

type Props = {
  sx?: SxProps<Theme>;
  onPrimaryClicked?: () => void;
  onSecondaryClicked?: () => void;
  primaryLabel: string;
  secondaryLabel?: string;
  hideSecondary?: boolean;
  disabled?: boolean;
};

const TwoButtonsDialogActions = ({ sx, onPrimaryClicked, onSecondaryClicked, primaryLabel, secondaryLabel, hideSecondary, disabled }: Props) => {
  const paletteMode = useContext(PaletteModeContext);

  return (
    <DialogActions sx={sx}>
      {!hideSecondary && (
        <Button variant="contained" onClick={onSecondaryClicked} color="secondary" disabled={disabled} sx={defaultButtonStyle}>
          <BodyIconTypography label={secondaryLabel} invertDefaultColor={paletteMode === 'dark'} />
        </Button>
      )}

      <Button variant="contained" type={onPrimaryClicked ? undefined : 'submit'} onClick={onPrimaryClicked} color="primary" disabled={disabled} sx={{ textTransform: 'none' }}>
        <BodyIconTypography label={primaryLabel} invertDefaultColor />
      </Button>
    </DialogActions>
  );
};

export default TwoButtonsDialogActions;

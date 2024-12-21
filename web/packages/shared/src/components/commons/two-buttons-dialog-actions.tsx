import Button from '@mui/material/Button';
import DialogActions from '@mui/material/DialogActions';
import { useContext } from 'react';
import { PaletteModeContext } from '../../libs/providers';
import BodyIconTypography from './body-icon-typography';

type Props = {
  onPrimaryClicked?: () => void;
  onSecondaryClicked?: () => void;
  primaryLabel: string;
  secondaryLabel?: string;
  hideSecondary?: boolean;
  disabled?: boolean;
};

const TwoButtonsDialogActions = ({ onPrimaryClicked, onSecondaryClicked, primaryLabel, secondaryLabel, hideSecondary, disabled }: Props) => {
  const paletteMode = useContext(PaletteModeContext);

  return (
    <DialogActions>
      {!hideSecondary && (
        <Button variant="contained" onClick={onSecondaryClicked} color="secondary" disabled={disabled}>
          <BodyIconTypography label={secondaryLabel} invertDefaultColor={paletteMode === 'dark'} />
        </Button>
      )}

      <Button variant="contained" type={onPrimaryClicked ? undefined : 'submit'} onClick={onPrimaryClicked} color="primary" disabled={disabled}>
        <BodyIconTypography label={primaryLabel} invertDefaultColor />
      </Button>
    </DialogActions>
  );
};

export default TwoButtonsDialogActions;

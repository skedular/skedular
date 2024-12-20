import Button from '@mui/material/Button';
import CardActions from '@mui/material/CardActions';
import { useContext } from 'react';
import { PaletteModeContext } from '../../libs/providers';
import BodyIconTypography from './body-icon-typography';

declare module '@mui/material/styles' {
  interface Palette {
    primaryAction: Palette['primary'];
    secondaryAction: Palette['primary'];
  }
  interface PaletteOptions {
    primaryAction?: PaletteOptions['primary'];
    secondaryAction?: PaletteOptions['primary'];
  }
}

type Props = {
  onPrimaryClicked?: () => void;
  onSecondaryClicked?: () => void;
  primaryLabel: string;
  secondaryLabel?: string;
  hideSecondary?: boolean;
  disabled?: boolean;
};

const TwoButtonsCardActions = ({ onPrimaryClicked, onSecondaryClicked, primaryLabel, secondaryLabel, hideSecondary, disabled }: Props) => {
  const paletteMode = useContext(PaletteModeContext);

  return (
    <CardActions sx={{ justifyContent: 'flex-end' }}>
      {!hideSecondary && (
        <Button
          variant="contained"
          onClick={onSecondaryClicked}
          sx={{ backgroundColor: (theme) => theme.palette.secondaryAction.main }}
          disabled={disabled}
        >
          <BodyIconTypography label={secondaryLabel} invertDefaultColor={paletteMode === 'dark'} />
        </Button>
      )}

      <Button
        variant="contained"
        type={onPrimaryClicked ? undefined : 'submit'}
        sx={{ backgroundColor: (theme) => theme.palette.primaryAction.main }}
        onClick={onPrimaryClicked}
        disabled={disabled}
      >
        <BodyIconTypography label={primaryLabel} invertDefaultColor />
      </Button>
    </CardActions>
  );
};

export default TwoButtonsCardActions;

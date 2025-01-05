import Button from '@mui/material/Button';
import CardActions from '@mui/material/CardActions';
import type { SxProps, Theme } from '@mui/system';
import { useContext } from 'react';
import { PaletteModeContext } from '../../libs/providers';
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

const TwoButtonsCardActions = ({ sx, onPrimaryClicked, onSecondaryClicked, primaryLabel, secondaryLabel, hideSecondary, disabled }: Props) => {
  const paletteMode = useContext(PaletteModeContext);

  return (
    <CardActions sx={{ justifyContent: 'flex-end', ...sx }}>
      {!hideSecondary && (
        <Button variant="contained" onClick={onSecondaryClicked} color="secondary" disabled={disabled}>
          <BodyIconTypography label={secondaryLabel} invertDefaultColor={paletteMode === 'dark'} />
        </Button>
      )}

      <Button variant="contained" type={onPrimaryClicked ? undefined : 'submit'} onClick={onPrimaryClicked} color="primary" disabled={disabled}>
        <BodyIconTypography label={primaryLabel} invertDefaultColor />
      </Button>
    </CardActions>
  );
};

export default TwoButtonsCardActions;

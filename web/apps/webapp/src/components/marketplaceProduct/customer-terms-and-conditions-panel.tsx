import { BodyIconTypography, CaptionIconTypography, StackColumn } from '@skedular/ui';
import OpenInNewIcon from '@mui/icons-material/OpenInNew';
import Checkbox from '@mui/material/Checkbox';
import FormControlLabel from '@mui/material/FormControlLabel';
import Link from '@mui/material/Link';
import Box from '@mui/system/Box';
import { memo } from 'react';

type Props = {
  accepted?: boolean;
  onAcceptedChange?: (accepted: boolean) => void;
  termsAndConditionsUrl: string | null | undefined;
};

const CustomerTermsAndConditionsPanel = ({ accepted = false, onAcceptedChange, termsAndConditionsUrl }: Props) => {
  if (!termsAndConditionsUrl) {
    return null;
  }

  return (
    <Box
      sx={{
        p: 1.5,
        borderRadius: 2.5,
        border: 1,
        borderColor: (theme) => theme.palette.divider,
        bgcolor: (theme) => theme.palette.action.hover,
      }}
    >
      <StackColumn spacing={1}>
        <CaptionIconTypography label="Terms and conditions" sx={{ letterSpacing: '0.05em', textTransform: 'uppercase', opacity: 0.68 }} />
        <BodyIconTypography label="Review this coworking space's terms and conditions before you continue with this booking or plan." sx={{ opacity: 0.82, fontSize: '0.95rem' }} />
        <Link
          href={termsAndConditionsUrl}
          target="_blank"
          rel="noreferrer"
          underline="hover"
          sx={{
            width: 'fit-content',
            display: 'inline-flex',
            alignItems: 'center',
            gap: 0.75,
            fontWeight: 600,
            color: 'text.primary',
            textDecorationColor: 'currentColor',
            opacity: 0.88,
          }}
        >
          Open the space terms and conditions
          <OpenInNewIcon sx={{ fontSize: '0.95rem' }} />
        </Link>
        {onAcceptedChange ? (
          <FormControlLabel
            control={
              <Checkbox
                checked={accepted}
                onChange={(event) => onAcceptedChange(event.target.checked)}
                slotProps={{ input: { 'aria-label': 'Accept the space terms and conditions' } }}
                size="small"
              />
            }
            label="I have read and accept this space's terms and conditions."
            sx={{
              alignItems: 'center',
              m: 0,
              gap: 0.5,
              '.MuiFormControlLabel-label': {
                lineHeight: 1.35,
                fontSize: '0.95rem',
              },
            }}
          />
        ) : null}
      </StackColumn>
    </Box>
  );
};

export default memo(CustomerTermsAndConditionsPanel);

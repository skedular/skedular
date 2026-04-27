import { GridContainer, SmallIconTypography, StackRow } from '@skedular/ui';
import { ContactPhoneIcon } from '@/components/icons';
import Grid from '@mui/material/Grid';
import type { SxProps, Theme } from '@mui/system';
import { memo } from 'react';
import ContactPhone from './contact-phone';

type Props = {
  sx?: SxProps<Theme>;
  contactPhones: readonly string[];
  hideIcon?: boolean;
  hideNAText?: boolean;
};

const ContactPhones = ({ sx, contactPhones, hideIcon, hideNAText }: Props) => {
  if (contactPhones.length === 0) {
    return hideNAText ? null : <SmallIconTypography label="N/A" startElement={!hideIcon && <ContactPhoneIcon />} sx={sx} />;
  }

  return (
    <StackRow sx={sx}>
      <GridContainer spacing={1}>
        {!hideIcon && (
          <Grid>
            <ContactPhoneIcon />
          </Grid>
        )}
        {contactPhones.map((contactPhone, index) => (
          <Grid key={index}>
            <ContactPhone contactPhone={contactPhone} />
          </Grid>
        ))}
      </GridContainer>
    </StackRow>
  );
};

export default memo(ContactPhones);

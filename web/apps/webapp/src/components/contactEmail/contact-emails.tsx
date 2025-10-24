import { GridContainer, SmallIconTypography, StackRow } from '@/components/commons';
import { ContactEmailIcon } from '@/components/icons';
import Grid from '@mui/material/Grid';
import type { SxProps, Theme } from '@mui/system';
import { memo } from 'react';
import ContactEmail from './contact-email';

type Props = {
  sx?: SxProps<Theme>;
  contactEmails: readonly string[];
  hideIcon?: boolean;
  hideNAText?: boolean;
};

const ContactPeople = ({ sx, contactEmails, hideIcon, hideNAText }: Props) => {
  if (contactEmails.length === 0) {
    return hideNAText ? null : <SmallIconTypography label="N/A" startElement={!hideIcon && <ContactEmailIcon />} sx={sx} />;
  }

  return (
    <StackRow sx={sx}>
      <GridContainer spacing={1}>
        {!hideIcon && (
          <Grid>
            <ContactEmailIcon />
          </Grid>
        )}
        {contactEmails.map((contactEmail, index) => (
          <Grid key={index}>
            <ContactEmail contactEmail={contactEmail} />
          </Grid>
        ))}
      </GridContainer>
    </StackRow>
  );
};

export default memo(ContactPeople);

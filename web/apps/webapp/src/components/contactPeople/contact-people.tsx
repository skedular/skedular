import { GridContainer, SmallIconTypography, StackRow } from '@/components/commons';
import { ContactPeopleIcon } from '@/components/icons';
import Grid from '@mui/material/Grid';
import type { SxProps, Theme } from '@mui/system';
import { memo } from 'react';
import ContactPerson from './contact-person';

type Props = {
  sx?: SxProps<Theme>;
  contactPeople: readonly string[];
  hideIcon?: boolean;
  hideNAText?: boolean;
};

const ContactPeople = ({ sx, contactPeople, hideIcon, hideNAText }: Props) => {
  if (contactPeople.length === 0) {
    return hideNAText ? <></> : <SmallIconTypography label="N/A" startElement={!hideIcon && <ContactPeopleIcon />} sx={sx} />;
  }

  return (
    <StackRow sx={sx}>
      <GridContainer spacing={1}>
        {!hideIcon && (
          <Grid>
            <ContactPeopleIcon />
          </Grid>
        )}
        {contactPeople.map((contactPerson, index) => (
          <Grid key={index}>
            <ContactPerson contactPerson={contactPerson} />
          </Grid>
        ))}
      </GridContainer>
    </StackRow>
  );
};

export default memo(ContactPeople);

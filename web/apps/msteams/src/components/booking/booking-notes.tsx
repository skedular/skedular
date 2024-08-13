import { TextField } from 'mui-rff';
import { memo } from 'react';

type Props = {
  name: string;
  required?: boolean;
};

const BookingNotes = ({ name, required }: Props) => {
  return (
    <TextField
      label="Notes"
      name={name}
      required={required}
      helperText="e.g. I will be half an hour late this morning"
      sx={{
        minWidth: 300,
        maxWidth: 300,
        textAlign: 'center',
      }}
    />
  );
};

export default memo(BookingNotes);

import { DatePicker } from 'mui-rff';
import { memo } from 'react';

type Props = {
  name: string;
  required?: boolean;
};

const BookingDate = ({ name, required }: Props) => {
  return (
    <DatePicker
      label="Date"
      name={name}
      required={required}
      sx={{
        minWidth: 300,
        maxWidth: 300,
        textAlign: 'center',
      }}
    />
  );
};

export default memo(BookingDate);

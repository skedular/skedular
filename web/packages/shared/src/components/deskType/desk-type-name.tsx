import { TextField } from 'mui-rff';
import { memo } from 'react';

type Props = {
  name: string;
  required?: boolean;
};

const ZoneName = ({ name, required }: Props) => (
  <TextField
    label="Name"
    name={name}
    required={required}
    helperText="Add your desk type name"
    sx={{
      minWidth: 300,
      maxWidth: 300,
      textAlign: 'center',
    }}
  />
);

export default memo(ZoneName);

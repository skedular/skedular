import { TextField } from 'mui-rff';
import { memo } from 'react';

type Props = {
  name: string;
  required?: boolean;
};

const DeskName = ({ name, required }: Props) => {
  return (
    <TextField
      label="Name"
      name={name}
      required={required}
      helperText="Add your desk name"
      sx={{
        minWidth: 300,
        maxWidth: 300,
        textAlign: 'center',
      }}
    />
  );
};

export default memo(DeskName);

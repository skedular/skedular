import Typography from '@mui/material/Typography';

type Props = {
  errorMessage?: string | null | undefined;
};

const ErrorTypography = ({ errorMessage }: Props) => {
  if (!errorMessage) {
    return <></>;
  }

  return (
    <Typography variant="caption" color="error.main">
      {errorMessage}
    </Typography>
  );
};

export default ErrorTypography;

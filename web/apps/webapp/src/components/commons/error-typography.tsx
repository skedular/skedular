import Typography from '@mui/material/Typography';
import type { CSSProperties } from '@mui/material/styles';

type Props = {
  errorMessage?: string | null | undefined;
  fontWeight?: CSSProperties['fontWeight'];
};

const ErrorTypography = ({ errorMessage, fontWeight }: Props) => {
  if (!errorMessage) {
    return <></>;
  }

  return (
    <Typography variant="caption" color="error.main" fontWeight={fontWeight}>
      {errorMessage}
    </Typography>
  );
};

export default ErrorTypography;

import Typography from '@mui/material/Typography';
import type { CSSProperties } from '@mui/material/styles';

type Props = {
  errorMessage?: string | null | undefined;
  fontWeight?: CSSProperties['fontWeight'];
};

const ErrorTypography = ({ errorMessage, fontWeight }: Props) => {
  if (!errorMessage) {
    return null;
  }

  return (
    <Typography variant="caption" color="error.main" sx={{ fontWeight }}>
      {errorMessage}
    </Typography>
  );
};

export default ErrorTypography;

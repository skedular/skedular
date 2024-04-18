import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import { memo } from 'react';

interface Props {
  title: string;
}

const FooterSectionTitle = ({ title }: Props) => {
  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        mb: 2,
      }}
    >
      <Typography component="p" variant="h5" sx={{ color: 'primary.contrastText', fontWeight: '700' }}>
        {title}
      </Typography>
    </Box>
  );
};

export default memo(FooterSectionTitle);

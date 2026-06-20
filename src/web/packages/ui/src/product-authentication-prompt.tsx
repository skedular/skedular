'use client';

import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import { alpha } from '@mui/material/styles';
import BodyIconTypography from './typography/body-icon-typography';
import LeadIconTypography from './typography/lead-icon-typography';
import StackColumn from './stack-column';

type Props = {
  description: string;
  signInHref: string;
  signUpHref: string;
  title: string;
};

const ProductAuthenticationPrompt = ({ description, signInHref, signUpHref, title }: Props) => (
  <Box
    component="main"
    sx={{
      minHeight: 'calc(100dvh - 64px)',
      display: 'grid',
      placeItems: 'center',
      px: 2,
      py: { xs: 5, md: 8 },
      background: (theme) => `radial-gradient(circle at 50% 0%, ${alpha(theme.palette.primary.main, 0.09)}, transparent 42%)`,
    }}
  >
    <Card
      sx={{
        width: '100%',
        maxWidth: 560,
        borderRadius: 5,
        border: 1,
        borderColor: 'divider',
        boxShadow: (theme) => `0 24px 64px ${alpha(theme.palette.common.black, theme.palette.mode === 'dark' ? 0.32 : 0.1)}`,
      }}
    >
      <CardContent sx={{ p: { xs: 3, sm: 4, md: 5 }, textAlign: 'center', '&:last-child': { pb: { xs: 3, sm: 4, md: 5 } } }}>
        <Box
          sx={{
            width: 80,
            height: 80,
            mx: 'auto',
            mb: 3,
            borderRadius: '50%',
            display: 'grid',
            placeItems: 'center',
            color: 'primary.main',
            bgcolor: (theme) => alpha(theme.palette.primary.main, 0.09),
          }}
        >
          <LockOutlinedIcon sx={{ fontSize: 38 }} />
        </Box>

        <LeadIconTypography label={title} sx={{ fontSize: { xs: '1.8rem', md: '2.2rem' }, lineHeight: 1.15 }} />
        <BodyIconTypography label={description} sx={{ mt: 1.5, mx: 'auto', maxWidth: 440, opacity: 0.76, lineHeight: 1.7 }} />

        <StackColumn spacing={1.5} sx={{ mt: 4, mx: 'auto', maxWidth: 400 }}>
          <Button href={signInHref} variant="contained" size="large" fullWidth sx={{ py: 1.5, borderRadius: 3, textTransform: 'none' }}>
            Sign in
          </Button>
          <Button href={signUpHref} variant="outlined" size="large" fullWidth sx={{ py: 1.5, borderRadius: 3, borderWidth: 2, textTransform: 'none' }}>
            Create account
          </Button>
        </StackColumn>
      </CardContent>
    </Card>
  </Box>
);

export default ProductAuthenticationPrompt;

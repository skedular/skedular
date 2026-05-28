import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackColumn } from '@skedular/ui';
import { getSignInLink, getSignUpLink } from '@/components/links';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Container from '@mui/material/Container';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { memo, useMemo } from 'react';

const MarketplaceProductSubscriptionDetailsSignIn = () => {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();

  const returnTo = useMemo(() => {
    const query = searchParams.toString();
    return query ? `${pathname}?${query}` : pathname;
  }, [pathname, searchParams]);

  return (
    <Box
      sx={{
        minHeight: '100vh',
        pb: 8,
        background:
          'radial-gradient(circle at top left, rgba(23, 93, 175, 0.14), transparent 28%), radial-gradient(circle at top right, rgba(255, 159, 67, 0.12), transparent 22%)',
      }}
    >
      <Container maxWidth="md" sx={{ pt: { xs: 3, md: 5 } }}>
        <Card sx={{ borderRadius: 5, boxShadow: '0 18px 64px rgba(7, 22, 41, 0.14)' }}>
          <CardContent sx={{ p: { xs: 3, md: 5 }, textAlign: 'center' }}>
            <CaptionIconTypography label="Subscription access" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
            <LeadIconTypography label="Sign in to view this subscription" sx={{ mt: 1 }} />
            <BodyIconTypography
              label="You’ll need an account to review payment status, open the checkout link, and return to this subscription later."
              sx={{ mt: 1.5, opacity: 0.76, maxWidth: 560, mx: 'auto' }}
            />

            <StackColumn spacing={1.25} sx={{ mt: 3, alignItems: 'center' }}>
              <Button variant="contained" onClick={() => router.push(`${getSignInLink()}?returnTo=${encodeURIComponent(returnTo)}`)}>
                Sign in
              </Button>
              <Button variant="text" onClick={() => router.push(`${getSignUpLink()}?returnTo=${encodeURIComponent(returnTo)}`)}>
                Create account
              </Button>
            </StackColumn>
          </CardContent>
        </Card>
      </Container>
    </Box>
  );
};

export default memo(MarketplaceProductSubscriptionDetailsSignIn);

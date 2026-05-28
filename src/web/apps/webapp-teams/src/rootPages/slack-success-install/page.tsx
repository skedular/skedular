import { BodyIconTypography } from '@skedular/ui';
import { PaletteModeContext } from '@skedular/shared';
import { coal, emerald, sandstone } from '@skedular/ui';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import { useSearchParams } from 'next/navigation';
import { memo, Suspense, useContext } from 'react';

const SlackSuccessInstall = () => {
  const paletteMode = useContext(PaletteModeContext);
  const searchParams = useSearchParams();
  const app = searchParams.get('app');

  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
      <Card sx={{ textAlign: 'center', padding: 4, borderRadius: 3, maxWidth: 400, backgroundColor: paletteMode === 'dark' ? emerald : coal }}>
        <CardContent>
          <CheckCircleIcon sx={{ color: 'white', fontSize: 48 }} />
          <BodyIconTypography label="Installation Successful" invertDefaultColor />
          <Button
            href={`https://slack.com/app_redirect?app=${app}`}
            variant="contained"
            fullWidth
            sx={{ mt: 3, backgroundColor: paletteMode === 'dark' ? coal : emerald, textTransform: 'none' }}
          >
            <BodyIconTypography label="Start using Skedular" sx={{ color: paletteMode === 'dark' ? sandstone : coal }} />
          </Button>
        </CardContent>
      </Card>
    </Box>
  );
};

const RootPage = () => (
  <Suspense>
    <SlackSuccessInstall />
  </Suspense>
);

export default memo(RootPage);

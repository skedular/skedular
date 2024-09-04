'use client';

import { PublicMainRootLayout } from '@/components/layouts';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Grid from '@mui/material/Grid2';
import Typography from '@mui/material/Typography';
import { memo } from 'react';

const Pricing = () => {
  return (
    <PublicMainRootLayout>
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'space-between',
          alignItems: 'left',
          p: '3rem',
        }}
      >
        <Grid container sx={{ justifyContent: 'center' }}>
          <Grid
            sx={{
              marginLeft: 2,
              minWidth: 300,
              maxWidth: 300,
              minHeight: 300,
              maxHeight: 300,
            }}
          >
            <Card variant="outlined">
              <CardHeader title="Basic" sx={{ backgroundColor: 'lightblue' }} />
              <CardContent>
                <Typography variant="h4">Free</Typography>
                <Typography variant="h5">For up to 10 users</Typography>
                <Typography sx={{ marginTop: 2 }}>Ideal for trying out UnityHub with a small team.</Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid
            sx={{
              marginLeft: 2,
              minWidth: 300,
              maxWidth: 300,
              minHeight: 300,
              maxHeight: 300,
            }}
          >
            <Card variant="outlined">
              <CardHeader title="Pay-as-you-go" sx={{ backgroundColor: 'lightblue' }} />
              <CardContent>
                <Typography variant="h4">Pay as you go</Typography>
                <Typography variant="h5">$3.00 USD per user per month</Typography>
                <Typography sx={{ marginTop: 2 }}>Enjoy all features, paying solely for active users each month when they book.</Typography>
              </CardContent>
            </Card>
          </Grid>

          {/* <Grid
            sx={{
              marginLeft: 2,
              minWidth: 300,
              maxWidth: 300,
              minHeight: 300,
              maxHeight: 300,
            }}
          >
            <Card variant="outlined">
              <CardHeader title="Early bird" sx={{ backgroundColor: 'lightblue' }} />
              <CardContent>
                <Typography variant="h4">Free</Typography>
                <Typography sx={{ marginTop: 2 }}>
                  You&apos;ll unlock access to all features, and in exchange, we&apos;d greatly appreciate your feedback.
                </Typography>
              </CardContent>
            </Card>
          </Grid> */}
        </Grid>
      </Box>
    </PublicMainRootLayout>
  );
};

export default memo(Pricing);

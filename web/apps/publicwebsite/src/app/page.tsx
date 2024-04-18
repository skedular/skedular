'use client';

import { PublicMainRootLayout } from '@/components/layouts';
import { SlackButton } from '@repo/shared/components/slackButtons';
import CheckIcon from '@mui/icons-material/Check';
import TryIcon from '@mui/icons-material/Try';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import Image from 'next/image';
import Link from 'next/link';
import { memo } from 'react';

const Home = () => {
  return (
    <PublicMainRootLayout>
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'space-between',
          alignItems: 'left',
          p: '4rem',
        }}
      >
        <Grid container sx={{ justifyContent: 'center' }}>
          <Grid item>
            <Image src="/images/screenshots/slack-mobile.jpg" width={180} height={370} alt="Slack Mobile" style={{ borderRadius: '10px' }} />
          </Grid>
          <Grid item sx={{ marginLeft: 10 }} />
          <Grid item>
            <Image src="/images/screenshots/web.png" width={528} height={267} alt="Slack Mobile" style={{ borderRadius: '10px' }} />
          </Grid>
          <Grid item sx={{ marginLeft: 10 }} />
          <Grid item sx={{ maxWidth: 400 }}>
            <Typography variant="h2">Discover the ideal days for office attendance, seamlessly integrated with Slack and Web platforms.</Typography>
            <Grid item sx={{ marginTop: 3 }} />
            <Typography variant="body1">
              Empower teams to navigate hybrid work arrangements with ease, balancing office collaboration with remote productivity.
            </Typography>
            <Grid item sx={{ marginTop: 3 }} />
            <Typography variant="body1">
              Efficiently manage office space utilization and expenses through workplace scheduling and desk booking solutions.
            </Typography>
            <Grid item sx={{ marginTop: 3 }} />
            <Typography variant="body1">
              UnityHub keeps your team informed about office occupancy and activities, directly accessible via Slack and Web interfaces.
            </Typography>
            <Grid item sx={{ marginTop: 3 }} />
            <Stack direction="row">
              <CheckIcon />
              <Typography variant="body1">No credit card required. Start your journey with us for free.</Typography>
            </Stack>
            <Grid item sx={{ marginTop: 3 }} />

            <Stack direction="row">
              <SlackButton />
              <Link href="https://app.unityhub.io">
                <Button variant="contained" sx={{ marginLeft: 2, borderRadius: '50px' }} size="large" startIcon={<TryIcon />}>
                  Try for free
                </Button>
              </Link>
            </Stack>
          </Grid>
        </Grid>
      </Box>
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'space-between',
          alignItems: 'left',
          p: '1rem',
        }}
      >
        <Stack direction="row">
          <Grid container sx={{ justifyContent: 'center' }}>
            <Grid item sx={{ maxWidth: 650 }}>
              <Typography variant="h2">Reserve your desk effortlessly with just one click.</Typography>
              <Grid item sx={{ marginTop: 3 }} />
              <Typography variant="body1">
                Sure, you could opt for a complex desk booking system featuring interactive maps and a lengthy many steps booking process. But does
                your team have the time for that every day? If not, chances are they won&apos;t bother using it.
              </Typography>
              <Grid item sx={{ marginTop: 3 }} />
              <Typography variant="body1">
                UnityHub simplifies the process by eliminating unnecessary clutter and remembering your preferred seating arrangements. Now, you can
                book your spot with a single click directly within Slack & Web. You can even set it to autopilot for added convenience.
              </Typography>
            </Grid>
            <Grid item sx={{ maxWidth: 650 }}>
              <Typography variant="h2">Share who&apos;s planning to be present.</Typography>
              <Grid item sx={{ marginTop: 3 }} />
              <Typography variant="body1">
                If your team members aren&apos;t aware of who&apos;s coming into the office, they might opt to stay home (after all, nobody enjoys
                commuting to an empty workspace). This could leave your office feeling underutilized, despite the costs involved.
              </Typography>
              <Grid item sx={{ marginTop: 3 }} />
              <Typography variant="body1">
                UnityHub simplifies this by broadcasting the list of employees expected in the office each day via a dedicated channel in Slack & Web.
                This way, everyone stays informed and connected, fostering a more vibrant office culture while ensuring optimal space utilization.
              </Typography>
            </Grid>
          </Grid>
        </Stack>
      </Box>

      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'space-between',
          alignItems: 'left',
          p: '1rem',
        }}
      >
        <Stack direction="row">
          <Grid container sx={{ justifyContent: 'center' }}>
            <Grid item sx={{ maxWidth: 650 }}>
              <Typography variant="h2">Intelligent office alerts.</Typography>
              <Grid item sx={{ marginTop: 3 }} />
              <Typography variant="body1">
                With UnityHub, receive smart notifications that inform employees when their manager, team leader, or key contacts are planning to be
                in the office. This feature helps team members coordinate their schedules effectively, ensuring seamless alignment and collaboration.
              </Typography>
              <Grid item sx={{ marginTop: 3 }} />
            </Grid>
            <Grid item sx={{ maxWidth: 650 }}>
              <Typography variant="h2">Office Utilization Insights.</Typography>
              <Grid item sx={{ marginTop: 3 }} />
              <Typography variant="body1">
                Your office space is a significant expense, and it&apos;s crucial to ensure you&apos;re optimizing its usage to avoid unnecessary
                costs.
              </Typography>
              <Grid item sx={{ marginTop: 3 }} />
              <Typography variant="body1">
                With UnityHub, gain insights into daily attendance numbers, popular days, and individual attendance trends. This data empowers you to
                right-size your office space according to your actual needs, helping you make informed decisions to optimize costs effectively.
              </Typography>
            </Grid>
          </Grid>
        </Stack>
      </Box>

      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'space-between',
          alignItems: 'left',
          p: '1rem',
        }}
      >
        <Stack direction="row">
          <Grid container sx={{ justifyContent: 'center' }}>
            <Grid item sx={{ maxWidth: 650 }}>
              <Typography variant="h2">All Integrated with Slack and Web platforms.</Typography>
              <Grid item sx={{ marginTop: 3 }} />
              <Typography variant="body1">
                Unlike other desk booking tools that require your employees to download yet another app, UnityHub integrates seamlessly with their
                existing workflow in Slack. This integration ensures high adoption rates among your team members, eliminating the need for separate
                applications and the associated costs of low usage.
              </Typography>
            </Grid>
          </Grid>
        </Stack>
      </Box>
    </PublicMainRootLayout>
  );
};

export default memo(Home);

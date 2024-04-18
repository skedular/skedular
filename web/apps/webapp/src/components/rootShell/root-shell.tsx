import { MainRootLayout } from '@/components/layouts';
import { LeftSideNavigationMenu } from '@/components/navigationMenu';
import { Observability } from '@/components/observability';
import type { rootShell_query$key } from '@/queries/__generated__/rootShell_query.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import { LogoutIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import { MuiXLicense } from '@repo/shared/libs/mui';
import { signOut } from 'next-auth/react';
import { memo, useEffect, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: rootShell_query$key;
  title?: string | null;
  children: React.ReactNode;
  onReloadRequire: () => void;
  areAdditionalCustomerRecordsSync: () => boolean;
  additionalCustomerRecords: any[];
  rightSideContent?: React.JSX.Element;
};

const maxRetryAttemptsToReload = 20;

const RootShell = ({
  rootDataRelay,
  children,
  onReloadRequire,
  areAdditionalCustomerRecordsSync,
  additionalCustomerRecords,
  rightSideContent,
}: Props) => {
  const rootData = useFragment(
    graphql`
      fragment rootShell_query on Query {
        me {
          id
        }
        ...observability_query
        ...mainRootLayout_query
      }
    `,
    rootDataRelay,
  );

  const [reloadCount, setReloadCount] = useState(0);

  useEffect(() => {
    if (reloadCount === maxRetryAttemptsToReload || (rootData.me && areAdditionalCustomerRecordsSync())) {
      return;
    }

    const intervalId = setInterval(() => {
      onReloadRequire();
      setReloadCount(reloadCount + 1);
    }, 3000);

    return () => {
      clearInterval(intervalId);
    };
  }, [rootDataRelay, rootData.me, reloadCount, onReloadRequire, areAdditionalCustomerRecordsSync, ...additionalCustomerRecords]); // eslint-disable-line react-hooks/exhaustive-deps

  if (!rootDataRelay) {
    return null;
  }

  const handleSignOutClick = () => {
    signOut();
  };

  if (reloadCount === maxRetryAttemptsToReload) {
    return (
      <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
        <Typography variant="h2">
          There was an issue activating your account. Kindly sign out and then sign back in to resolve the problem.
        </Typography>
        <Button variant="contained" startIcon={<LogoutIcon />} onClick={handleSignOutClick}>
          Sign out
        </Button>
      </Box>
    );
  }

  if (!rootData.me || !areAdditionalCustomerRecordsSync()) {
    return <Loading message="Kindly hold on as we proceed to activate your account..." />;
  }

  return (
    <>
      <Observability rootDataRelay={rootData} />
      <MainRootLayout rootDataRelay={rootData} leftSideContent={<LeftSideNavigationMenu />} rightSideContent={rightSideContent}>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'column',
            justifyContent: 'space-between',
            alignItems: 'left',
            p: '6rem',
          }}
        >
          {children}
        </Box>
      </MainRootLayout>
      <MuiXLicense />
    </>
  );
};

export default memo(RootShell);

import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import { Loading } from '@repo/shared/components/loading';
import { MuiXLicense } from '@repo/shared/libs/mui';
import graphql from 'babel-plugin-relay/macro';
import { MainRootLayout } from 'components/layouts';
import { LeftSideNavigationMenu } from 'components/navigationMenu';
import { Observability } from 'components/observability';
import { memo, useEffect, useState } from 'react';
import { useFragment } from 'react-relay';
import type { rootShell_query$key } from './__generated__/rootShell_query.graphql';

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
        isAzureTenantInstalled
        azureTenantAdminConsentUrl
        ...observability_query
      }
    `,
    rootDataRelay,
  );

  const [reloadCount, setReloadCount] = useState(0);

  useEffect(() => {
    if (!rootData.isAzureTenantInstalled) {
      return;
    }

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

  const handleInstallClicked = () => {
    window.open(rootData.azureTenantAdminConsentUrl);
  };

  if (!rootData.isAzureTenantInstalled) {
    return (
      <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
        <Typography variant="h2">
          Your administrator needs to install UnityHub for you. This is a one-time setup. Please click the button below to start the installation.
        </Typography>
        <Button variant="contained" onClick={handleInstallClicked}>
          Install
        </Button>
      </Box>
    );
  }

  if (reloadCount === maxRetryAttemptsToReload) {
    return (
      <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
        <Typography variant="h2">There was an issue activating your account.</Typography>
      </Box>
    );
  }

  if (!rootData.me || !areAdditionalCustomerRecordsSync()) {
    return <Loading message="Kindly hold on as we proceed to activate your account..." />;
  }

  return (
    <>
      <Observability rootDataRelay={rootData} />
      <MainRootLayout leftSideContent={<LeftSideNavigationMenu />} rightSideContent={rightSideContent}>
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

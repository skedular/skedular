import type { customerSettings_rootQuery } from '@/queries/__generated__/customerSettings_rootQuery.graphql';
import Stack from '@mui/material/Stack';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { getCurrentCompleteUrl } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import CustomerSettingsPersonalTab from './customer-settings-personal-tab';

type Props = {
  queryReference: PreloadedQuery<customerSettings_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query customerSettings_rootQuery {
    me {
      id
      name
      givenName
      middleName
      familyName
      photoUrl
    }
    ...customerSettingsPersonalTab_query
  }
`;

const CustomerSettings = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<customerSettings_rootQuery>(RootQuery, queryReference);
  const searchParams = useSearchParams();
  const tab = searchParams.get('tab');
  const router = useRouter();
  let initialTabIndex = 0;

  if (tab === 'personal') {
    initialTabIndex = 0;
  }

  const [tabIndex, setTabIndex] = useState(initialTabIndex);

  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    setTabIndex(newValue);

    let tab = '';

    if (newValue === 0) {
      tab = 'personal';
    }

    if (tab) {
      router.push(`${getCurrentCompleteUrl()}?tab=${tab}`);
    }
  };

  if (!rootData.me) {
    return null;
  }

  return (
    <>
      <Stack direction="column" spacing={1}>
        <CustomerAvatar name={rootData.me} photo={{ url: rootData.me.photoUrl }} sx={{ marginBottom: 1 }} />
      </Stack>
      <Tabs value={tabIndex} onChange={handleTabChange}>
        <Tab label="Personal" />
      </Tabs>
      <Stack direction="column" spacing={1}>
        {tabIndex === 0 && <CustomerSettingsPersonalTab rootDataRelay={rootData} />}
      </Stack>
    </>
  );
};

const MemoCustomerSettings = memo(CustomerSettings);

const CustomerSettingsWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<customerSettings_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoCustomerSettings queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(CustomerSettingsWithRelay);

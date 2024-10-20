import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { nanoid } from 'nanoid';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { useSearchParams } from 'react-router-dom';
import type { customerSettings_rootQuery } from './__generated__/customerSettings_rootQuery.graphql';
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
  const [searchParams, setSearchParams] = useSearchParams();
  const tab = searchParams.get('tab');
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
      setSearchParams({ tab });
    }
  };

  if (!rootData.me) {
    return null;
  }

  return (
    <>
      <CustomerAvatar name={rootData.me} photo={{ url: rootData.me.photoUrl }} sx={{ marginBottom: 1 }} />
      <Tabs value={tabIndex} onChange={handleTabChange}>
        <Tab label="Personal" />
      </Tabs>
      {tabIndex === 0 && <CustomerSettingsPersonalTab rootDataRelay={rootData} />}
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

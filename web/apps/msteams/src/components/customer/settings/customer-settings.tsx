import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import graphql from 'babel-plugin-relay/macro';
import { CustomerAvatar } from 'components/customer';
import { memo, useState } from 'react';
import { useFragment } from 'react-relay';
import { useLocation, useNavigate, useSearchParams } from 'react-router-dom';
import type { customerSettingsPage_query$key } from './__generated__/customerSettingsPage_query.graphql';
import CustomerSettingsPersonalTab from './customer-settings-personal-tab';

type Props = {
  rootDataRelay: customerSettingsPage_query$key;
};

const CustomerSettings = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<customerSettingsPage_query$key>(
    graphql`
      fragment customerSettingsPage_query on Query {
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
    `,
    rootDataRelay,
  );

  const [searchParams] = useSearchParams();
  const location = useLocation();
  const tab = searchParams.get('tab');
  const navigate = useNavigate();

  let initialTabIndex = 0;

  if (tab === 'personal') {
    initialTabIndex = 0;
  }

  const [tabIndex, setTabIndex] = useState(initialTabIndex);

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabIndex(newValue);

    let tab = '';

    if (newValue === 0) {
      tab = 'personal';
    }

    if (tab) {
      navigate(`${location.pathname}?tab=${tab}`);
    }
  };

  if (!rootData.me) {
    return null;
  }

  return (
    <Box sx={{ display: 'flex' }}>
      <Stack direction="column">
        <Stack direction="column">
          <CustomerAvatar
            name={{
              name: rootData.me.name,
              givenName: rootData.me.givenName,
              middleName: rootData.me.middleName,
              familyName: rootData.me.familyName,
            }}
            photo={{
              url: rootData.me.photoUrl,
            }}
            sx={{ marginBottom: 1 }}
          />
        </Stack>
        <Tabs value={tabIndex} onChange={handleTabChange}>
          <Tab label="Personal" />
        </Tabs>
        <Stack direction="column">{tabIndex === 0 && <CustomerSettingsPersonalTab rootDataRelay={rootData} />}</Stack>
      </Stack>
    </Box>
  );
};

export default memo(CustomerSettings);

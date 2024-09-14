import type { organizationsNavigationMenu_query$key } from '@/queries/__generated__/organizationsNavigationMenu_query.graphql';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import { memo, useEffect, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizationsNavigationMenu_query$key;
};

const localStorageSelectedOrganizationKey = 'selectedOrganization';

const OrganizationsNavigationMenu = ({ rootDataRelay }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment organizationsNavigationMenu_query on Query {
        me {
          defaultOrganization {
            uniqueId
          }
        }
        myOrganizations {
          id
          logoUrl
          name
          canModify
          canDelete
          canViewAnalytics
        }
      }
    `,
    rootDataRelay,
  );

  const [selectedOrganization, setSelectedOrganization] = useState<string | undefined>(rootData.me?.defaultOrganization?.uniqueId);

  useEffect(() => {
    const savedSelectedOrganization = localStorage.getItem(localStorageSelectedOrganizationKey) as string | null;
    if (savedSelectedOrganization) {
      const matchedOrgnization = rootData.myOrganizations.find((organization) => organization.id === savedSelectedOrganization);
      if (matchedOrgnization) {
        setSelectedOrganization(matchedOrgnization.id);

        return;
      }
    }

    const matchedOrgnization = rootData.myOrganizations.find((organization) => organization.id === rootData.me?.defaultOrganization?.uniqueId);
    if (matchedOrgnization) {
      setSelectedOrganization(matchedOrgnization.id);
    } else {
      setSelectedOrganization(undefined);
    }
  }, [rootData.me?.defaultOrganization?.uniqueId, rootData.myOrganizations]);

  const handleSelectedOrganizationChange = (event: SelectChangeEvent) => {
    const selectedOrganizationId = event.target.value as string;
    setSelectedOrganization(selectedOrganizationId);
    localStorage.setItem(localStorageSelectedOrganizationKey, selectedOrganizationId);
  };

  return (
    <FormControl fullWidth>
      <InputLabel>Organization</InputLabel>
      <Select value={selectedOrganization} label="Organization" onChange={handleSelectedOrganizationChange}>
        {rootData.myOrganizations.map((organization) => (
          <MenuItem key={organization.id} value={organization.id}>
            {organization.name}
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  );
};

export default memo(OrganizationsNavigationMenu);

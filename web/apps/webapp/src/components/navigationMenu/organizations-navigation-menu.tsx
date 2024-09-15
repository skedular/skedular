import type { organizationsNavigationMenu_query$key } from '@/queries/__generated__/organizationsNavigationMenu_query.graphql';
import { Stack } from '@mui/material';
import Button from '@mui/material/Button';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import Typography from '@mui/material/Typography';
import { OrganizationAvatar } from '@repo/shared/components/avatars';
import { NewIcon } from '@repo/shared/components/icons';
import { memo, useEffect, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizationsNavigationMenu_query$key;
};

const localStorageSelectedOrganizationKey = 'selectedOrganization';
const newOrganizationMenuItemId = 'BuTrIsjIXhPeRwNx6SgCW';
const emptyOrganizatioMenuItemId = 'd1cEUZqsxQZDxIxJ0AgKc';

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
    if (savedSelectedOrganization === emptyOrganizatioMenuItemId) {
      setSelectedOrganization(undefined);

      return;
    }

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

    if (selectedOrganizationId === '') {
      localStorage.setItem(localStorageSelectedOrganizationKey, emptyOrganizatioMenuItemId);
      setSelectedOrganization(undefined);

      return;
    }

    if (selectedOrganizationId === newOrganizationMenuItemId) {
      return;
    }

    setSelectedOrganization(selectedOrganizationId);
    localStorage.setItem(localStorageSelectedOrganizationKey, selectedOrganizationId);
  };

  return (
    <FormControl fullWidth>
      <InputLabel>Organization</InputLabel>
      <Select value={selectedOrganization} label="Organization" onChange={handleSelectedOrganizationChange}>
        <MenuItem value="">
          <Typography>None</Typography>
        </MenuItem>
        <MenuItem value={newOrganizationMenuItemId}>
          <Button startIcon={<NewIcon />}>New</Button>
        </MenuItem>
        {rootData.myOrganizations.map((organization) => (
          <MenuItem key={organization.id} value={organization.id}>
            <Stack direction="row" spacing={1}>
              <OrganizationAvatar name={{ name: organization.name }} photo={{ url: organization.logoUrl }} size="small" />
              <Typography>{organization.name}</Typography>
            </Stack>
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  );
};

export default memo(OrganizationsNavigationMenu);

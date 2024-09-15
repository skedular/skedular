import type { organizationsNavigationMenu_query$key } from '@/queries/__generated__/organizationsNavigationMenu_query.graphql';
import { Stack } from '@mui/material';
import Button from '@mui/material/Button';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import MenuItem from '@mui/material/MenuItem';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import Typography from '@mui/material/Typography';
import { OrganizationAvatar } from '@repo/shared/components/avatars';
import { AnalyticsIcon, BillingAndPaymentIcon, MembersIcon, NewIcon, SettingsIcon } from '@repo/shared/components/icons';
import { SelectedOrganizationContext, UpdateSelectedOrganizationContext } from '@repo/shared/libs/providers';
import Link from 'next/link';
import { memo, useContext, useEffect, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizationsNavigationMenu_query$key;
};

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
          canViewAnalytics
        }
      }
    `,
    rootDataRelay,
  );

  const selectedOrganization = useContext(SelectedOrganizationContext);
  const updateSelectedOrganization = useContext(UpdateSelectedOrganizationContext);
  const [selectedOrganizationId, setSelectedOrganizationId] = useState<string | undefined>(rootData.me?.defaultOrganization?.uniqueId);

  useEffect(() => {
    if (selectedOrganization === emptyOrganizatioMenuItemId) {
      setSelectedOrganizationId(undefined);

      return;
    }

    if (selectedOrganization) {
      const matchedOrgnization = rootData.myOrganizations.find((organization) => organization.id === selectedOrganization);
      if (matchedOrgnization) {
        setSelectedOrganizationId(matchedOrgnization.id);

        return;
      }
    }

    const matchedOrgnization = rootData.myOrganizations.find((organization) => organization.id === rootData.me?.defaultOrganization?.uniqueId);
    if (matchedOrgnization) {
      setSelectedOrganizationId(matchedOrgnization.id);
    } else {
      setSelectedOrganizationId(undefined);
    }
  }, [selectedOrganization, rootData.me?.defaultOrganization?.uniqueId, rootData.myOrganizations]);

  const handleSelectedOrganizationChange = (event: SelectChangeEvent) => {
    const id = event.target.value as string;
    if (id === '') {
      updateSelectedOrganization(emptyOrganizatioMenuItemId);
      setSelectedOrganizationId(undefined);

      return;
    }

    if (id === newOrganizationMenuItemId) {
      return;
    }

    setSelectedOrganizationId(id);
    updateSelectedOrganization(id);
  };

  const matchedOrganization = rootData.myOrganizations.find((organization) => organization.id === selectedOrganizationId);

  return (
    <>
      <FormControl fullWidth>
        <InputLabel>Organization</InputLabel>
        <Select value={selectedOrganizationId} label="Organization" onChange={handleSelectedOrganizationChange}>
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
      {matchedOrganization && (
        <List>
          <Link href={`/organization/${selectedOrganizationId}/members`}>
            <ListItem disablePadding>
              <ListItemButton>
                <ListItemIcon>
                  <MembersIcon />
                </ListItemIcon>
                <ListItemText>Members</ListItemText>
              </ListItemButton>
            </ListItem>
          </Link>

          {matchedOrganization.canViewAnalytics && (
            <Link href={`/organization/${selectedOrganizationId}/analytics`}>
              <ListItem disablePadding>
                <ListItemButton>
                  <ListItemIcon>
                    <AnalyticsIcon />
                  </ListItemIcon>
                  <ListItemText>Analytics</ListItemText>
                </ListItemButton>
              </ListItem>
            </Link>
          )}

          {matchedOrganization.canModify && (
            <Link href={`/organization/${selectedOrganizationId}/settings`}>
              <ListItem disablePadding>
                <ListItemButton>
                  <ListItemIcon>
                    <SettingsIcon excludeTooltip={true} />
                  </ListItemIcon>
                  <ListItemText>Settings</ListItemText>
                </ListItemButton>
              </ListItem>
            </Link>
          )}

          {matchedOrganization.canModify && (
            <Link href={`/organization/${selectedOrganizationId}/settings`}>
              <ListItem disablePadding>
                <ListItemButton>
                  <ListItemIcon>
                    <BillingAndPaymentIcon />
                  </ListItemIcon>
                  <ListItemText>Billing and Payments</ListItemText>
                </ListItemButton>
              </ListItem>
            </Link>
          )}
        </List>
      )}
    </>
  );
};

export default memo(OrganizationsNavigationMenu);

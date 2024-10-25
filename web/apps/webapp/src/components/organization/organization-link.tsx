import { OrganizationBookingsCard } from '@/components/organization/organizationBookingCard';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { OrganizationIcon, ViewDetailsIcon } from '@repo/shared/components/icons';
import { DialogTransition } from '@repo/shared/components/transitions';
import NextLink from 'next/link';
import { memo, useState } from 'react';

type Props = {
  id: string;
  name?: string;
  excludeLink?: boolean;
  bookingsLink?: boolean;
  settingsLink?: boolean;
  peopleLink?: boolean;
  locationsLink?: boolean;
  teamsLink?: boolean;
  offeringLink?: boolean;
  billingLink?: boolean;
  analayticsLink?: boolean;
  enableViewDetails?: boolean;
  onReloadRequired?: () => void;
};

export const getOrganizationBaseLink = (id: string) => `/organizations/${id}`;
export const getOrganizationAddLink = () => `/organizations/add`;
export const getOrganizationBookingsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=bookings`;
export const getOrganizationSettingsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=about`;
export const getOrganizationMembersBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=memebrs`;
export const getOrganizationLocationsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=locations`;
export const getOrganizationTeamsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=teams`;
export const getOrganizationOfferingBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=offering`;
export const getOrganizationBillingBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=billing`;
export const getOrganizationAnalyticsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=analytics`;

const OrganizationLink = ({
  id,
  name,
  excludeLink,
  bookingsLink,
  settingsLink,
  peopleLink,
  locationsLink,
  teamsLink,
  offeringLink,
  billingLink,
  analayticsLink,
  enableViewDetails,
  onReloadRequired,
}: Props) => {
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  let href = '';
  if (bookingsLink) {
    href = getOrganizationBookingsBaseLink(id);
  } else if (settingsLink) {
    href = getOrganizationSettingsBaseLink(id);
  } else if (peopleLink) {
    href = getOrganizationMembersBaseLink(id);
  } else if (locationsLink) {
    href = getOrganizationLocationsBaseLink(id);
  } else if (teamsLink) {
    href = getOrganizationTeamsBaseLink(id);
  } else if (offeringLink) {
    href = getOrganizationOfferingBaseLink(id);
  } else if (billingLink) {
    href = getOrganizationBillingBaseLink(id);
  } else if (analayticsLink) {
    href = getOrganizationAnalyticsBaseLink(id);
  } else {
    href = getOrganizationBaseLink(id);
  }

  const handleViewDetailsClick = () => {
    setIsDialogOpen(true);
  };

  const handleViewDetailsCloseClick = () => {
    setIsDialogOpen(false);

    if (onReloadRequired) {
      onReloadRequired();
    }
  };

  return (
    <>
      <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
        <OrganizationIcon fontSize="small" color="primary" />
        {excludeLink && (
          <Typography variant="h6" color="primary">
            {name}
          </Typography>
        )}
        {!excludeLink && (
          <Link component={NextLink} href={href}>
            <Typography variant="h6" color="primary">
              {name}
            </Typography>
          </Link>
        )}
        {enableViewDetails && (
          <Button size="small" color="warning" onClick={handleViewDetailsClick}>
            <ViewDetailsIcon color="primary" />
          </Button>
        )}
      </Stack>
      <Dialog TransitionComponent={DialogTransition} open={isDialogOpen}>
        <DialogContent>
          <OrganizationBookingsCard organizationId={id} organizationName={name} organizationsConnectionIds={[]} />
        </DialogContent>
        <DialogActions>
          <Button color="primary" variant="contained" onClick={handleViewDetailsCloseClick}>
            Close
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default memo(OrganizationLink);

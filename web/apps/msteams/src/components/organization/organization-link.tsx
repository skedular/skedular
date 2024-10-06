import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { OrganizationIcon, ViewDetailsIcon } from '@repo/shared/components/icons';
import { OrganizationBookingsCard } from 'components/organization/organizationBookingCard';
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
};

export const getOrganizationBaseLink = (id: string) => `/organization/${id}`;
export const getOrganizationAddLink = () => `/organization/add`;
export const getOrganizationBookingsLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=bookings`;
export const getOrganizationSettingsLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=about`;
export const getOrganizationPeopleLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=people`;
export const getOrganizationLocationLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=locations`;
export const getOrganizationTeamsLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=teams`;
export const getOrganizationOfferingLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=offering`;
export const getOrganizationBillingLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=billing`;
export const getOrganizationAnalyticsLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=analytics`;

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
}: Props) => {
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  let href = '';
  if (bookingsLink) {
    href = getOrganizationBookingsLink(id);
  } else if (settingsLink) {
    href = getOrganizationSettingsLink(id);
  } else if (peopleLink) {
    href = getOrganizationPeopleLink(id);
  } else if (locationsLink) {
    href = getOrganizationLocationLink(id);
  } else if (teamsLink) {
    href = getOrganizationTeamsLink(id);
  } else if (offeringLink) {
    href = getOrganizationOfferingLink(id);
  } else if (billingLink) {
    href = getOrganizationBillingLink(id);
  } else if (analayticsLink) {
    href = getOrganizationAnalyticsLink(id);
  } else {
    href = getOrganizationBaseLink(id);
  }

  const handleViewDetailsClick = () => {
    setIsDialogOpen(true);
  };

  const handleViewDetailsCloseClick = () => {
    setIsDialogOpen(false);
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
          <Link href={href}>
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
      <Dialog open={isDialogOpen}>
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

import { LocationBookingsCard } from '@/components/location/locationBookingCard';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import Link from '@mui/material/Link';
import { LeadIconTypography, StackRow } from '@repo/shared/components/commons';
import { LocationIcon, ViewDetailsIcon } from '@repo/shared/components/icons';
import { DialogTransition } from '@repo/shared/components/transitions';
import { memo, useState } from 'react';

type Props = {
  organizationId?: string;
  organizationName?: string;
  id: string;
  name?: string;
  excludeLink?: boolean;
  bookingsLink?: boolean;
  settingsLink?: boolean;
  peopleLink?: boolean;
  zonesLink?: boolean;
  desksLink?: boolean;
  analayticsLink?: boolean;
  enableViewDetails?: boolean;
  onReloadRequired?: () => void;
};

export const getLocationBaseLink = (id: string, organizationId?: string) =>
  organizationId ? `/organizations/${organizationId}/locations/${id}` : `/locations/${id}`;
export const getLocationAddLink = (organizationId?: string) => (organizationId ? `/organizations/${organizationId}/locations/add` : `/locations/add`);
export const getLocationBookingsLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=bookings`;
export const getLocationSettingsLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=about`;
export const getLocationMembersLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=members`;
export const getLocationZonesLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=zones`;
export const getLocationDesksLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=desks`;
export const getLocationAnalyticsLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=analytics`;

const LocationLink = ({
  organizationId,
  organizationName,
  id,
  name,
  excludeLink,
  bookingsLink,
  settingsLink,
  peopleLink,
  zonesLink,
  desksLink,
  analayticsLink,
  enableViewDetails,
  onReloadRequired,
}: Props) => {
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  let href = '';
  if (bookingsLink) {
    href = getLocationBookingsLink(id, organizationId);
  } else if (settingsLink) {
    href = getLocationSettingsLink(id, organizationId);
  } else if (peopleLink) {
    href = getLocationMembersLink(id, organizationId);
  } else if (zonesLink) {
    href = getLocationZonesLink(id, organizationId);
  } else if (desksLink) {
    href = getLocationDesksLink(id, organizationId);
  } else if (analayticsLink) {
    href = getLocationAnalyticsLink(id, organizationId);
  } else {
    href = getLocationBaseLink(id, organizationId);
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
      <StackRow>
        {excludeLink && <LeadIconTypography color="primary" label={name} icon={<LocationIcon fontSize="small" color="primary" />} />}
        {!excludeLink && (
          <Link href={href}>
            <LeadIconTypography color="primary" label={name} icon={<LocationIcon fontSize="small" color="primary" />} />
          </Link>
        )}
        {enableViewDetails && (
          <Button size="small" color="warning" onClick={handleViewDetailsClick}>
            <ViewDetailsIcon color="primary" />
          </Button>
        )}
      </StackRow>
      <Dialog TransitionComponent={DialogTransition} open={isDialogOpen}>
        <DialogContent>
          <LocationBookingsCard
            organizationId={organizationId}
            organizationName={organizationName}
            locationId={id}
            locationName={name}
            locationsConnectionIds={[]}
          />
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

export default memo(LocationLink);

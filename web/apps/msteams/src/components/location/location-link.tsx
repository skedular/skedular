import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { LocationIcon } from '@repo/shared/components/icons';
import { memo } from 'react';

type Props = {
  organizationId?: string;
  id: string;
  name?: string;
  excludeLink?: boolean;
  bookingsLink?: boolean;
  settingsLink?: boolean;
  peopleLink?: boolean;
  zonesLink?: boolean;
  desksLink?: boolean;
  analayticsLink?: boolean;
};

export const getLocationBaseLink = (id: string, organizationId?: string) =>
  organizationId ? `/organization/${organizationId}/location/${id}` : `/location/${id}`;
export const getLocationAddLink = (organizationId?: string) => (organizationId ? `/organization/${organizationId}/location/add` : `/location/add`);
export const getLocationBookingsLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=bookings`;
export const getLocationSettingsLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=about`;
export const getLocationPeopleLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=people`;
export const getLocationZonesLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=zones`;
export const getLocationDesksLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=desks`;
export const getLocationAnalyticsLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=analytics`;

const LocationLink = ({
  organizationId,
  id,
  name,
  excludeLink,
  bookingsLink,
  settingsLink,
  peopleLink,
  zonesLink,
  desksLink,
  analayticsLink,
}: Props) => {
  let href = '';
  if (bookingsLink) {
    href = getLocationBookingsLink(id, organizationId);
  } else if (settingsLink) {
    href = getLocationSettingsLink(id, organizationId);
  } else if (peopleLink) {
    href = getLocationPeopleLink(id, organizationId);
  } else if (zonesLink) {
    href = getLocationZonesLink(id, organizationId);
  } else if (desksLink) {
    href = getLocationDesksLink(id, organizationId);
  } else if (analayticsLink) {
    href = getLocationAnalyticsLink(id, organizationId);
  } else {
    href = getLocationBaseLink(id, organizationId);
  }

  return (
    <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
      <LocationIcon fontSize="small" color="primary" />
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
    </Stack>
  );
};

export default memo(LocationLink);

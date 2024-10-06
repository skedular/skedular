import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { TeamIcon, ViewDetailsIcon } from '@repo/shared/components/icons';
import { TeamBookingsCard } from 'components/team/teamBookingCard';
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
  enableViewDetails?: boolean;
};

export const getTeamBaseLink = (id: string, organizationId?: string) =>
  organizationId ? `/organization/${organizationId}/team/${id}` : `/team/${id}`;
export const getTeamAddLink = (organizationId?: string) => (organizationId ? `/organization/${organizationId}/team/add` : `/team/add`);
export const getTeamBookingsLink = (id: string, organizationId?: string) => `${getTeamBaseLink(id, organizationId)}?tab=bookings`;
export const getTeamSettingsLink = (id: string, organizationId?: string) => `${getTeamBaseLink(id, organizationId)}?tab=about`;
export const getTeamPeopleLink = (id: string, organizationId?: string) => `${getTeamBaseLink(id, organizationId)}?tab=people`;

const TeamLink = ({ organizationId, organizationName, id, name, excludeLink, bookingsLink, settingsLink, peopleLink, enableViewDetails }: Props) => {
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  let href = '';
  if (bookingsLink) {
    href = getTeamBookingsLink(id, organizationId);
  } else if (settingsLink) {
    href = getTeamSettingsLink(id, organizationId);
  } else if (peopleLink) {
    href = getTeamPeopleLink(id, organizationId);
  } else {
    href = getTeamBaseLink(id, organizationId);
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
        <TeamIcon fontSize="small" color="primary" />
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
          <TeamBookingsCard organizationId={organizationId} organizationName={organizationName} teamId={id} teamName={name} teamsConnectionIds={[]} />
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

export default memo(TeamLink);

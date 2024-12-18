import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import Link from '@mui/material/Link';
import { LeadIconTypography, StackRow } from '@repo/shared/components/commons';
import { TeamIcon, ViewDetailsIcon } from '@repo/shared/components/icons';
import { DialogTransition } from '@repo/shared/components/transitions';
import { TeamBookingsCard } from 'components/team/teamBookingCard';
import { memo, useState } from 'react';

type Props = {
  organizationId: string;
  organizationName?: string;
  id: string;
  name?: string;
  excludeLink?: boolean;
  bookingsLink?: boolean;
  settingsLink?: boolean;
  peopleLink?: boolean;
  enableViewDetails?: boolean;
  onReloadRequired?: () => void;
};

export const getTeamBaseLink = (id: string, organizationId: string) => `/organizations/${organizationId}/teams/${id}`;
export const getTeamAddLink = (organizationId: string) => `/organizations/${organizationId}/teams/add`;
export const getTeamBookingsLink = (id: string, organizationId: string) => `${getTeamBaseLink(id, organizationId)}?tab=bookings`;
export const getTeamSettingsLink = (id: string, organizationId: string) => `${getTeamBaseLink(id, organizationId)}?tab=about`;
export const getTeamMembersLink = (id: string, organizationId: string) => `${getTeamBaseLink(id, organizationId)}?tab=members`;

const TeamLink = ({
  organizationId,
  organizationName,
  id,
  name,
  excludeLink,
  bookingsLink,
  settingsLink,
  peopleLink,
  enableViewDetails,
  onReloadRequired,
}: Props) => {
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  let href = '';
  if (bookingsLink) {
    href = getTeamBookingsLink(id, organizationId);
  } else if (settingsLink) {
    href = getTeamSettingsLink(id, organizationId);
  } else if (peopleLink) {
    href = getTeamMembersLink(id, organizationId);
  } else {
    href = getTeamBaseLink(id, organizationId);
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
        {excludeLink && <LeadIconTypography label={name} startElement={<TeamIcon fontSize="medium" excludeTooltip />} />}
        {!excludeLink && (
          <Link href={href}>
            <LeadIconTypography label={name} startElement={<TeamIcon fontSize="medium" excludeTooltip />} />
          </Link>
        )}
        {enableViewDetails && (
          <Button size="small" color="warning" onClick={handleViewDetailsClick}>
            <ViewDetailsIcon />
          </Button>
        )}
      </StackRow>
      <Dialog TransitionComponent={DialogTransition} open={isDialogOpen}>
        <DialogContent>
          <TeamBookingsCard organizationId={organizationId} organizationName={organizationName} teamId={id} teamName={name} teamsConnectionIds={[]} />
        </DialogContent>
        <DialogActions>
          <Button variant="contained" onClick={handleViewDetailsCloseClick}>
            Close
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default memo(TeamLink);

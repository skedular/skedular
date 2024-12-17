import { TeamBookingsCard } from '@/components/team/teamBookingCard';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import Link from '@mui/material/Link';
import { LeadIconTypography, StackRow } from '@repo/shared/components/commons';
import { TeamIcon, ViewDetailsIcon } from '@repo/shared/components/icons';
import { DialogTransition } from '@repo/shared/components/transitions';
import NextLink from 'next/link';
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
  onReloadRequired?: () => void;
};

export const getTeamBaseLink = (id: string, organizationId?: string) =>
  organizationId ? `/organizations/${organizationId}/teams/${id}` : `/teams/${id}`;
export const getTeamAddLink = (organizationId?: string) => (organizationId ? `/organizations/${organizationId}/teams/add` : `/teams/add`);
export const getTeamBookingsLink = (id: string, organizationId?: string) => `${getTeamBaseLink(id, organizationId)}?tab=bookings`;
export const getTeamSettingsLink = (id: string, organizationId?: string) => `${getTeamBaseLink(id, organizationId)}?tab=about`;
export const getTeamMembersLink = (id: string, organizationId?: string) => `${getTeamBaseLink(id, organizationId)}?tab=members`;

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
        {excludeLink && <LeadIconTypography color="primary" label={name} icon={<TeamIcon fontSize="small" color="primary" />} />}
        {!excludeLink && (
          <Link component={NextLink} href={href}>
            <LeadIconTypography color="primary" label={name} icon={<TeamIcon fontSize="small" color="primary" />} />
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

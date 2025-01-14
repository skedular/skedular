import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import Link from '@mui/material/Link';
import { LeadIconTypography, StackRow, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import { OrganizationIcon, ViewDetailsIcon } from '@repo/shared/components/icons';
import { DialogTransition } from '@repo/shared/components/transitions';
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
  onReloadRequired?: () => void;
};

export const getOrganizationBaseLink = (id: string) => `/organizations/${id}`;
export const getOrganizationAddLink = () => `/organizations/add`;

export const getModernOrganizationMembersBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/members`;

export const getModernOrganizationTeamsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/teams`;
export const getModernOrganizationTeamSetupBaseLink = (id: string, teamId: string) => `${getOrganizationBaseLink(id)}/teams/${teamId}?section=setup`;
export const getModernOrganizationTeamMembersBaseLink = (id: string, teamId: string) =>
  `${getOrganizationBaseLink(id)}/teams/${teamId}?section=members`;
export const getModernOrganizationTeamLocationBaseLink = (id: string, teamId: string) =>
  `${getOrganizationBaseLink(id)}/teams/${teamId}?section=location`;

export const getModernOrganizationBookingBaseLink = (id: string, bookingId: string) => `${getOrganizationBaseLink(id)}/bookings/${bookingId}`;

export const getModernOrganizationLocationsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/locations`;
export const getModernOrganizationLocationSetupBaseLink = (id: string, locationId: string) =>
  `${getOrganizationBaseLink(id)}/locations/${locationId}?section=setup`;
export const getModernOrganizationLocationManageDesksBaseLink = (id: string, locationId: string) =>
  `${getOrganizationBaseLink(id)}/locations/${locationId}?section=manage-desks`;

export const getModernOrganizationLocationDeskBaseLink = (id: string, locationId: string, deskId: string) =>
  `${getOrganizationBaseLink(id)}/locations/${locationId}/desks/${deskId}`;

export const getModernOrganizationAdminSetupBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=setup`;
export const getModernOrganizationAdminBillingAndPaymentBaseLink = (id: string) =>
  `${getOrganizationBaseLink(id)}/admin?section=billing-payment-setup`;
export const getModernOrganizationAdminSSOBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=sso-setup`;
export const getModernOrganizationAdminZonesBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=zones-setup`;
export const getModernOrganizationAdminCustomTagsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=tags-setup`;
export const getModernOrganizationAdminSubscriptionsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=subscriptions`;

export const getOrganizationBookingsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=bookings`;
export const getOrganizationSettingsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=about`;
export const getOrganizationMembersBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=members`;
export const getOrganizationLocationsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=locations`;
export const getOrganizationTeamsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=teams`;
export const getOrganizationOfferingBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=offering`;
export const getOrganizationBillingBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=billing`;
export const getOrganizationAnalyticsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=analytics`;

export const getModernOrganizationNotificationsBaseLink = (id: string) => `/organizations/${id}/notifications`;

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
    href = getModernOrganizationLocationsBaseLink(id);
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
      <StackRow>
        {excludeLink && <LeadIconTypography label={name} startElement={<OrganizationIcon fontSize="medium" excludeTooltip />} invertDefaultColor />}
        {!excludeLink && (
          <Link href={href}>
            <LeadIconTypography label={name} startElement={<OrganizationIcon fontSize="medium" excludeTooltip />} invertDefaultColor />
          </Link>
        )}
        {enableViewDetails && (
          <Button size="small" color="warning" onClick={handleViewDetailsClick}>
            <ViewDetailsIcon />
          </Button>
        )}
      </StackRow>
      <Dialog TransitionComponent={DialogTransition} open={isDialogOpen} fullWidth>
        <DialogContent>
          <OrganizationBookingsCard organizationId={id} organizationName={name} organizationsConnectionIds={[]} />
        </DialogContent>
        <TwoButtonsDialogActions onPrimaryClicked={handleViewDetailsCloseClick} primaryLabel="Close" hideSecondary />
      </Dialog>
    </>
  );
};

export default memo(OrganizationLink);

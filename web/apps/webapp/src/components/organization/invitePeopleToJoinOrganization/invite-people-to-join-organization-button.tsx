import Button from '@mui/material/Button';
import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@repo/shared/components/commons';
import { NewIcon } from '@repo/shared/components/icons';
import { memo, useState } from 'react';
import InvitePeopleToJoinOrganizationDialog from './invite-people-to-join-organization-dialog';

type Props = {
  organizationId: string;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const InvitePeopleToJoinOrganizationButton = ({ organizationId, fullWidth, label, hideIcon, variant, size }: Props) => {
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  const handleButtonClicked = () => {
    setIsDialogOpen(true);
  };

  const handleInviteClicked = () => {
    setIsDialogOpen(false);
  };

  const handleCancelClicked = () => {
    setIsDialogOpen(false);
  };

  return (
    <>
      <Button variant={variant ?? 'text'} onClick={handleButtonClicked} fullWidth={fullWidth} sx={{ textTransform: 'none' }}>
        {size === 'small' && (
          <SmallIconTypography label={label ?? 'Invite Members'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} />
        )}
        {size === 'medium' && (
          <BodyIconTypography label={label ?? 'Invite Members'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} />
        )}
        {(size === 'large' || !size) && (
          <LeadIconTypography label={label ?? 'Invite Members'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} />
        )}
      </Button>
      <InvitePeopleToJoinOrganizationDialog
        isDialogOpen={isDialogOpen}
        onInviteClicked={handleInviteClicked}
        onCancel={handleCancelClicked}
        organizationId={organizationId}
      />
    </>
  );
};

export default memo(InvitePeopleToJoinOrganizationButton);

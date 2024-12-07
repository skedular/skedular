import Button from '@mui/material/Button';
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
      <Button
        variant={variant ?? 'text'}
        size={size ?? 'large'}
        onClick={handleButtonClicked}
        fullWidth={fullWidth}
        endIcon={hideIcon ? null : <NewIcon />}
        sx={{ borderRadius: 4 }}
      >
        {label ?? 'Invite New Members'}
      </Button>
      <InvitePeopleToJoinOrganizationDialog
        isDialogOpen={isDialogOpen}
        onInviteClicked={handleInviteClicked}
        onCancelClicked={handleCancelClicked}
        organizationId={organizationId}
      />
    </>
  );
};

export default memo(InvitePeopleToJoinOrganizationButton);

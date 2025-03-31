import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@/components/commons';
import { InvitePeopleIcon } from '@/components/icons';
import Button from '@mui/material/Button';
import type { CSSProperties } from '@mui/material/styles';
import type { SxProps, Theme } from '@mui/system';
import { memo, useState } from 'react';
import InvitePeopleToJoinOrganizationDialog from './invite-people-to-join-organization-dialog';

type Props = {
  sx?: SxProps<Theme>;
  color?: CSSProperties['color'];
  organizationId: string;
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const InvitePeopleToJoinOrganizationButton = ({ sx, color, organizationId, fullWidth, label, hideIcon, variant, size }: Props) => {
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
      <Button variant={variant ?? 'text'} onClick={handleButtonClicked} fullWidth={fullWidth} sx={{ textTransform: 'none', ...sx }}>
        {size === 'small' && <SmallIconTypography label={label ?? 'Add a New User'} endElement={hideIcon ? null : <InvitePeopleIcon fontSize={size ?? 'small'} />} color={color} />}
        {size === 'medium' && (
          <BodyIconTypography label={label ?? 'Add a New User'} endElement={hideIcon ? null : <InvitePeopleIcon fontSize={size ?? 'medium'} />} color={color} />
        )}
        {(size === 'large' || !size) && (
          <LeadIconTypography label={label ?? 'Add a New User'} endElement={hideIcon ? null : <InvitePeopleIcon fontSize={size ?? 'large'} />} color={color} />
        )}
      </Button>
      <InvitePeopleToJoinOrganizationDialog isDialogOpen={isDialogOpen} onInviteClicked={handleInviteClicked} onCancel={handleCancelClicked} organizationId={organizationId} />
    </>
  );
};

export default memo(InvitePeopleToJoinOrganizationButton);

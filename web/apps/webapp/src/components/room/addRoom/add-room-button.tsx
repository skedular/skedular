import Button from '@mui/material/Button';
import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@repo/shared/components/commons';
import { NewIcon } from '@repo/shared/components/icons';
import { memo, useState } from 'react';
import AddRoomDialog from './add-room-dialog';

type Props = {
  onReloadRequired?: () => void;
  organizationId: string;
  locationId: string;
  connectionIds: string[];
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const AddRoomButton = ({ onReloadRequired, locationId, organizationId, connectionIds, fullWidth, label, hideIcon, variant, size }: Props) => {
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  const handleButtonClicked = () => {
    setIsDialogOpen(true);
  };

  const handleAddClicked = () => {
    setIsDialogOpen(false);

    if (onReloadRequired) {
      onReloadRequired();
    }
  };

  const handleCancelClicked = () => {
    setIsDialogOpen(false);
  };

  return (
    <>
      <Button variant={variant ?? 'text'} onClick={handleButtonClicked} fullWidth={fullWidth} sx={{ textTransform: 'none' }}>
        {size === 'small' && <SmallIconTypography label={label ?? 'Add Room'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} />}
        {size === 'medium' && <BodyIconTypography label={label ?? 'Add Room'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} />}
        {(size === 'large' || !size) && <LeadIconTypography label={label ?? 'Add Room'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} />}
      </Button>
      <AddRoomDialog
        onReloadRequired={onReloadRequired}
        organizationId={organizationId}
        locationId={locationId}
        connectionIds={connectionIds}
        isDialogOpen={isDialogOpen}
        onAddClicked={handleAddClicked}
        onCancel={handleCancelClicked}
      />
    </>
  );
};

export default memo(AddRoomButton);

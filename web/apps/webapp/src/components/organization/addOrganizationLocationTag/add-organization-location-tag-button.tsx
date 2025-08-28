import { BodyIconTypography, LeadIconTypography, SmallIconTypography } from '@/components/commons';
import { NewIcon } from '@/components/icons';
import Button from '@mui/material/Button';
import { memo, useState } from 'react';
import AddOrganizationLocationTagDialog from './add-organization-location-tag-dialog';

type Props = {
  onReloadRequired?: () => void;
  organizationUniqueAlphanumericName: string;
  connectionIds: string[];
  fullWidth?: boolean;
  label?: string;
  hideIcon?: boolean;
  variant?: 'text' | 'outlined' | 'contained';
  size?: 'small' | 'medium' | 'large';
};

const AddOrganizationLocationTagButton = ({ organizationUniqueAlphanumericName, onReloadRequired, connectionIds, fullWidth, label, hideIcon, variant, size }: Props) => {
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
        {size === 'small' && <SmallIconTypography label={label ?? 'Add Location Tag'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'small'} />} />}
        {size === 'medium' && <BodyIconTypography label={label ?? 'Add Location Tag'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'medium'} />} />}
        {(size === 'large' || !size) && <LeadIconTypography label={label ?? 'Add Location Tag'} endElement={hideIcon ? null : <NewIcon fontSize={size ?? 'large'} />} />}
      </Button>
      <AddOrganizationLocationTagDialog
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
        connectionIds={connectionIds}
        isDialogOpen={isDialogOpen}
        onAddClicked={handleAddClicked}
        onCancel={handleCancelClicked}
      />
    </>
  );
};

export default memo(AddOrganizationLocationTagButton);

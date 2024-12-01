import { getLocationAddLink } from '@/components/location';
import Button from '@mui/material/Button';
import { NewIcon } from '@repo/shared/components/icons';
import { memo } from 'react';

type Props = {
  organizationId?: string;
  fullWidth?: boolean;
};

const NewLocationButton = ({ organizationId, fullWidth }: Props) => (
  <Button href={getLocationAddLink(organizationId)} variant="text" size="large" fullWidth={fullWidth} endIcon={<NewIcon />}>
    Add Location
  </Button>
);

export default memo(NewLocationButton);

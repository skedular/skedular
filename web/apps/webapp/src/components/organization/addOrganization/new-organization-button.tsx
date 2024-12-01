import { getOrganizationAddLink } from '@/components/organization';
import Button from '@mui/material/Button';
import { NewIcon } from '@repo/shared/components/icons';
import { memo } from 'react';

type Props = {
  fullWidth?: boolean;
};

const NewOrganizationButton = ({ fullWidth }: Props) => (
  <Button href={getOrganizationAddLink()} variant="text" size="large" fullWidth={fullWidth} endIcon={<NewIcon />}>
    Add Organization
  </Button>
);

export default memo(NewOrganizationButton);

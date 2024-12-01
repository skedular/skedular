import Button from '@mui/material/Button';
import { NewIcon } from '@repo/shared/components/icons';
import { getTeamAddLink } from 'components/team';
import { memo } from 'react';

type Props = {
  organizationId: string;
  fullWidth?: boolean;
};

const NewTeamButton = ({ organizationId, fullWidth }: Props) => (
  <Button href={getTeamAddLink(organizationId)} variant="text" size="large" fullWidth={fullWidth} endIcon={<NewIcon />}>
    Create a Team
  </Button>
);

export default memo(NewTeamButton);

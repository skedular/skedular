import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import { BodyIconTypography, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { memo, type JSX } from 'react';

export type UserType = 'private';

type Props = {
  icon?: React.ReactNode | JSX.Element;
  title: string;
  subtitle: string;
  onClick: () => void;
};

const UserTypeCard = ({ icon, title, subtitle, onClick }: Props) => (
  <Card variant="outlined" sx={{ cursor: 'pointer', '&:hover': { borderColor: 'primary.main' }, padding: 2 }} onClick={onClick}>
    <CardContent>
      <StackRow>
        {icon}
        <StackColumn spacing={0}>
          <BodyIconTypography fontWeight="bold" label={title} />
          <SmallIconTypography label={subtitle} />
        </StackColumn>
      </StackRow>
    </CardContent>
  </Card>
);

export default memo(UserTypeCard);

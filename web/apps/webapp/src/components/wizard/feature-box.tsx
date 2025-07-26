import { BodyIconTypography, SmallIconTypography, StackRow } from '@/components/commons';
import StackColumn from '@/components/commons/stack-column';
import Card from '@mui/material/Card';
import { memo, type JSX } from 'react';

type Props = {
  icon?: React.ReactNode | JSX.Element;
  title: string;
  subtitle: string;
};

const FeatureBox = ({ icon, title, subtitle }: Props) => (
  <Card sx={{ backgroundColor: 'rgba(255,255,255,0.1)', color: 'white', padding: 2 }}>
    <StackRow>
      {icon}
      <StackColumn spacing={0}>
        <BodyIconTypography fontWeight="bold" label={title} />
        <SmallIconTypography label={subtitle} />
      </StackColumn>
    </StackRow>
  </Card>
);

export default memo(FeatureBox);

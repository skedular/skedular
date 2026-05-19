import Tooltip from '@mui/material/Tooltip';
import { memo } from 'react';

type Props = {
  tip: string;
  children: React.ReactElement;
};

const TooltipIcon = ({ tip, children }: Props) => {
  return <Tooltip title={tip}>{children}</Tooltip>;
};

export default memo(TooltipIcon);

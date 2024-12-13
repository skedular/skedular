import type { SxProps, Theme } from '@mui/system';
import { PropsWithChildren } from 'react';
import StackRow from './stack-row';

type Props = {
  sx?: SxProps<Theme>;
};

const StackRowFullWidth = ({ children, sx }: PropsWithChildren<Props>) => (
  <StackRow sx={{ justifyContent: 'space-between', width: '100%', alignItems: 'center', ...sx }}>{children}</StackRow>
);

export default StackRowFullWidth;

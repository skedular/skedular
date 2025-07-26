import DialogTitle from '@mui/material/DialogTitle';
import Divider from '@mui/material/Divider';
import type { CSSProperties } from '@mui/material/styles';
import type { SxProps, Theme } from '@mui/system';
import SectionIconTypography from './section-icon-typography';
import StackColumn from './stack-column';

type Props = {
  sx?: SxProps<Theme>;
  title: string;
  fontWeight?: CSSProperties['fontWeight'];
};

const DefaultDialogTitle = ({ sx, title, fontWeight }: Props) => (
  <DialogTitle sx={{ padding: 0, ...sx }}>
    <StackColumn>
      <SectionIconTypography label={title} sx={{ paddingRight: 2, paddingLeft: 2, paddingTop: 2, paddingBottom: 1 }} fontWeight={fontWeight} />
      <Divider />
    </StackColumn>
  </DialogTitle>
);

export default DefaultDialogTitle;

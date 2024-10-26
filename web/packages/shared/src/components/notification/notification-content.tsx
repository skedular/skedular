import Typography from '@mui/material/Typography';

type Props = {
  content?: string;
};

const Notification = ({ content }: Props) => <Typography variant="caption">{content}</Typography>;

export default Notification;

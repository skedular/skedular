import { SmallIconTypography } from '../commons';

type Props = {
  content?: string;
};

const Notification = ({ content }: Props) => <SmallIconTypography label={content} />;

export default Notification;

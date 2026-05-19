import { SmallIconTypography } from '@skedular/ui';

type Props = {
  content?: string;
};

const Notification = ({ content }: Props) => <SmallIconTypography label={content} />;

export default Notification;

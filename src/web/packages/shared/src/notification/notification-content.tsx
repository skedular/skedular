'use client';

import { SmallIconTypography } from '@skedular/ui';
import type { ToastOptions } from 'react-toastify';

type Props = {
  content?: string;
};

const NotificationContent = ({ content }: Props) => <SmallIconTypography label={content} />;

export default NotificationContent;

export const errorNotificationOptions: ToastOptions = {
  type: 'error',
  autoClose: false,
};

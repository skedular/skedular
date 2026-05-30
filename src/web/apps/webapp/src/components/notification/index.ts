// NotificationContent and errorNotificationOptions are re-exported from @skedular/shared
export { errorNotificationOptions, NotificationContent } from '@skedular/shared';
import type { ToastOptions } from 'react-toastify';

const successNotificationAutoCloseTimeout = 3000;

const infoNotificationOptions: ToastOptions = {
  type: 'info',
  autoClose: false,
};

const autoCloseErrorNotificationOptions: ToastOptions = {
  type: 'error',
  autoClose: successNotificationAutoCloseTimeout,
};

const successNotificationOptions: ToastOptions = {
  type: 'success',
  autoClose: successNotificationAutoCloseTimeout,
};

export { autoCloseErrorNotificationOptions, infoNotificationOptions, successNotificationAutoCloseTimeout, successNotificationOptions };

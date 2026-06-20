export { default as NotificationContent } from './notification-content';
import type { ToastOptions } from 'react-toastify';

const successNotificationAutoCloseTimeout = 3000;

const infoNotificationOptions: ToastOptions = {
  type: 'info',
  autoClose: false,
};

const errorNotificationOptions: ToastOptions = {
  type: 'error',
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

export { autoCloseErrorNotificationOptions, errorNotificationOptions, infoNotificationOptions, successNotificationAutoCloseTimeout, successNotificationOptions };

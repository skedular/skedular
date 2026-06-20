import pino from 'pino';

export default pino({
  name: 'webapp',
  level: process.env.LOG_LEVEL ?? 'info',
  base: {
    app: 'webapp',
  },
});

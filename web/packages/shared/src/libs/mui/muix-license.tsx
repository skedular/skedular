'use client';

import { generateLicense, LicenseInfo } from '@mui/x-license-pro';
import dayjs from 'dayjs';

LicenseInfo.setLicenseKey(
  generateLicense({
    expiryDate: dayjs().utc().add(1, 'year').toDate(),
    orderNumber: 'Test',
    licensingModel: 'subscription',
    scope: 'premium',
  }),
);

const MuiXLicense = () => {
  return null;
};

export default MuiXLicense;

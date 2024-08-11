'use client';

import { generateLicense, LicenseInfo } from '@mui/x-license-pro';
import dayjs from 'dayjs';
import { useEffect } from 'react';

const MuiXLicense = () => {
  useEffect(() => {
    LicenseInfo.setLicenseKey(
      generateLicense({
        expiryDate: dayjs().utc().add(1, 'year').toDate(),
        orderNumber: 'Test',
        licensingModel: 'subscription',
        scope: 'premium',
      }),
    );
  }, []);

  return null;
};

export default MuiXLicense;

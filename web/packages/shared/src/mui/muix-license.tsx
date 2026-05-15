'use client';

import { LicenseInfo } from '@mui/x-license';
import { base64Encode, md5 } from '@mui/x-license/internals';
import dayjs from 'dayjs';
import { useEffect } from 'react';

const generateLicense = ({
  expiryDate,
  licensingModel,
  orderNumber,
  scope,
}: {
  expiryDate: Date;
  licensingModel: 'annual' | 'perpetual' | 'subscription';
  orderNumber: string;
  scope: 'pro' | 'premium';
}) => {
  const license = `O=${orderNumber},E=${expiryDate.getTime()},S=${scope},LM=${licensingModel},PV=Q1-2026,KV=2`;
  const encodedLicense = base64Encode(license);

  return `${md5(encodedLicense)}${encodedLicense}`;
};

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

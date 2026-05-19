import Link from '@mui/material/Link';
import NextLink from 'next/link';
import { memo } from 'react';

type Props = {
  contactPhone: string;
};

const ContactPhone = ({ contactPhone }: Props) => (
  <Link component={NextLink} href={`tel:${contactPhone.trim().replace(/[^\d+]/g, '')}`}>
    {contactPhone}
  </Link>
);

export default memo(ContactPhone);

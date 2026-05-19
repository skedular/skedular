import Link from '@mui/material/Link';
import NextLink from 'next/link';
import { memo } from 'react';

type Props = {
  contactEmail: string;
};

const ContactEmail = ({ contactEmail }: Props) => (
  <Link component={NextLink} href={`mailto:${contactEmail.trim()}`}>
    {contactEmail}
  </Link>
);

export default memo(ContactEmail);

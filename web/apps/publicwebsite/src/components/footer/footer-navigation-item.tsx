import Link from '@mui/material/Link';
import Typography from '@mui/material/Typography';
import NextLink from 'next/link';
import { memo } from 'react';
import type { UrlObject } from 'url';

type Url = string | UrlObject;

interface NavigationItemProps {
  label: string;
  path: Url;
}

const NavigationItem = ({ label, path }: NavigationItemProps) => {
  return (
    <Link component={NextLink} href={path} passHref>
      <Typography variant="body2" sx={{ color: 'primary.contrastText' }}>
        {label}
      </Typography>
    </Link>
  );
};

export default memo(NavigationItem);

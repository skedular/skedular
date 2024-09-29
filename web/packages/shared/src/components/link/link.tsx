import MUILink from '@mui/material/Link';
import NextLink from 'next/link';
import { memo } from 'react';
import type { UrlObject } from 'url';

type Url = string | UrlObject;

type Props = {
  children: React.ReactNode;
  href: Url;
};

const Link = ({ children, href }: Props) => (
  <NextLink href={href}>
    <MUILink>{children}</MUILink>
  </NextLink>
);

export default memo(Link);

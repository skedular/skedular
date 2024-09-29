import MUILink from '@mui/material/Link';
import NextLink from 'next/link';
import { memo } from 'react';
import type { UrlObject } from 'url';

type Url = string | UrlObject;

type HTMLAttributeAnchorTarget = '_self' | '_blank' | '_parent' | '_top' | (string & {});

type Props = {
  children: React.ReactNode;
  href: Url;
  target?: HTMLAttributeAnchorTarget | undefined;
  rel?: string | undefined;
};

const Link = ({ children, href, target, rel }: Props) => (
  <NextLink href={href} target={target} rel={rel}>
    <MUILink>{children}</MUILink>
  </NextLink>
);

export default memo(Link);

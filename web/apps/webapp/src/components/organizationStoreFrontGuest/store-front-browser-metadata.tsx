'use client';

import { useLayoutEffect } from 'react';

const defaultIconHref = '/images/skedular-icon-primary.svg';

type Props = {
  organizationName: string;
  organizationLogoUrl?: string | null;
};

const StoreFrontBrowserMetadata = ({ organizationName, organizationLogoUrl }: Props) => {
  // useLayoutEffect runs synchronously after each React commit and before paint,
  // so the org name is always in the tab before the browser renders anything.
  useLayoutEffect(() => {
    document.title = organizationName;
  }, [organizationName]);

  useLayoutEffect(() => {
    // Remove all existing icon links (including any Next.js-injected ones) so
    // the browser is forced to use ours and cannot fall back to the Skedular icon.
    document.head.querySelectorAll('link[rel~="icon"]').forEach((el) => el.remove());
    const link = document.createElement('link');
    link.rel = 'icon';
    link.href = organizationLogoUrl ?? defaultIconHref;
    document.head.appendChild(link);
  }, [organizationLogoUrl]);

  return null;
};

export default StoreFrontBrowserMetadata;

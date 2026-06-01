'use client';

import { useLayoutEffect } from 'react';

const defaultIconHref = '/images/skedular-icon-primary.svg';
const storeFrontIconId = 'store-front-browser-metadata-icon';

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
    let link = document.getElementById(storeFrontIconId) as HTMLLinkElement | null;

    if (!link) {
      link = document.createElement('link') as HTMLLinkElement;
    }

    link.id = storeFrontIconId;
    link.setAttribute('rel', 'icon');
    link.setAttribute('href', organizationLogoUrl ?? defaultIconHref);

    if (!link.parentNode) {
      document.head.appendChild(link);
    }
  }, [organizationLogoUrl]);

  return null;
};

export default StoreFrontBrowserMetadata;

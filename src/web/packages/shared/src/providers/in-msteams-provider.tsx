'use client';

import { usePathname } from 'next/navigation';
import type { PropsWithChildren } from 'react';
import { createContext, useMemo } from 'react';

export const InMsTeamsContext = createContext<boolean>(false);

const InMsTeamsProvider = ({ children }: PropsWithChildren) => {
  const pathname = usePathname();
  const inMsTeams = useMemo(() => !!pathname && pathname.toLowerCase().startsWith('/msteams'), [pathname]);

  return <InMsTeamsContext.Provider value={inMsTeams}>{children}</InMsTeamsContext.Provider>;
};

export default InMsTeamsProvider;

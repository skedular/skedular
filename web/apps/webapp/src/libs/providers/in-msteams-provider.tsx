import type { PropsWithChildren } from 'react';
import { createContext } from 'react';

export const InMsTeamsContext = createContext<boolean>(typeof window !== 'undefined' && window.name === 'embedded-page-container');

const InMsTeamsProvider = ({ children }: PropsWithChildren) => {
  return <InMsTeamsContext.Provider value={typeof window !== 'undefined' && window.name === 'embedded-page-container'}>{children}</InMsTeamsContext.Provider>;
};

export default InMsTeamsProvider;

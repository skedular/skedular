import type { PropsWithChildren } from 'react';
import { createContext } from 'react';
import useIntegratedPlatrform from './integrated-paltform-hook';

export const InMsTeamsContext = createContext<boolean>(typeof window !== 'undefined' && window.name === 'embedded-page-container');

const InMsTeamsProvider = ({ children }: PropsWithChildren) => {
  const { integratedPlatrform } = useIntegratedPlatrform();

  return <InMsTeamsContext.Provider value={integratedPlatrform === 'msteams'}>{children}</InMsTeamsContext.Provider>;
};

export default InMsTeamsProvider;

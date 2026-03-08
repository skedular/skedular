import type { PropsWithChildren } from 'react';
import { createContext } from 'react';
import useIntegratedPlatrform from './integrated-platform-hook';

export const InMsTeamsContext = createContext<boolean>(false);

const InMsTeamsProvider = ({ children }: PropsWithChildren) => {
  const { integratedPlatrform } = useIntegratedPlatrform();

  return <InMsTeamsContext.Provider value={integratedPlatrform === 'msteams'}>{children}</InMsTeamsContext.Provider>;
};

export default InMsTeamsProvider;

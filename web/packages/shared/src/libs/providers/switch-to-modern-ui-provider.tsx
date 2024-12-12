import { createContext, PropsWithChildren, useEffect, useState } from 'react';
import { useLocalStorage } from 'usehooks-ts';

export const SwitchToModernUIContext = createContext<boolean>(false);
export const UpdateSwitchToModernUIContext = createContext<(state: boolean) => void>(() => {});

const SwitchToModernUIProvider = ({ children }: PropsWithChildren) => {
  const [persistedSwitchToModernUI, setPersistedSwitchToModernUI] = useLocalStorage<boolean>('switchToModernUI', false);
  const [selectedOrganization, setSwitchToModernUI] = useState<boolean>(false);

  useEffect(() => setSwitchToModernUI(persistedSwitchToModernUI ?? undefined), [persistedSwitchToModernUI]);

  const updateSwitchToModernUI = (state: boolean) => {
    setSwitchToModernUI(state);
    setPersistedSwitchToModernUI(state);
  };

  return (
    <SwitchToModernUIContext.Provider value={selectedOrganization}>
      <UpdateSwitchToModernUIContext.Provider value={updateSwitchToModernUI}>{children}</UpdateSwitchToModernUIContext.Provider>
    </SwitchToModernUIContext.Provider>
  );
};

export default SwitchToModernUIProvider;

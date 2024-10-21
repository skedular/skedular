import { createContext, useState } from 'react';
import { v4 as uuidv4 } from 'uuid';

export const GlobalReloadIdContext = createContext<string>(uuidv4());
export const UpdateGlobalReloadIdContext = createContext<() => void>(() => {});

type Props = {
  children: React.ReactNode;
};

const GlobalReloadIdProvider = ({ children }: Props) => {
  const [selectedGlobalReloadId, setGlobalReloadId] = useState<string>(uuidv4());
  const updateGlobalReloadId = () => {
    setGlobalReloadId(uuidv4());
  };

  return (
    <GlobalReloadIdContext.Provider value={selectedGlobalReloadId}>
      <UpdateGlobalReloadIdContext.Provider value={updateGlobalReloadId}>{children}</UpdateGlobalReloadIdContext.Provider>
    </GlobalReloadIdContext.Provider>
  );
};

export default GlobalReloadIdProvider;

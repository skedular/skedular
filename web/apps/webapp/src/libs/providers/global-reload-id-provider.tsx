import type { PropsWithChildren } from 'react';
import { createContext, useState } from 'react';
import { v7 as uuid } from 'uuid';

export const GlobalReloadIdContext = createContext<string>(uuid());
export const UpdateGlobalReloadIdContext = createContext<() => void>(() => {});

const GlobalReloadIdProvider = ({ children }: PropsWithChildren) => {
  const [selectedGlobalReloadId, setGlobalReloadId] = useState<string>(uuid());
  const updateGlobalReloadId = () => setGlobalReloadId(uuid());

  return (
    <GlobalReloadIdContext.Provider value={selectedGlobalReloadId}>
      <UpdateGlobalReloadIdContext.Provider value={updateGlobalReloadId}>{children}</UpdateGlobalReloadIdContext.Provider>
    </GlobalReloadIdContext.Provider>
  );
};

export default GlobalReloadIdProvider;

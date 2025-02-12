import { nanoid } from 'nanoid';
import type { PropsWithChildren } from 'react';
import { createContext, useState } from 'react';

export const GlobalReloadIdContext = createContext<string>(nanoid());
export const UpdateGlobalReloadIdContext = createContext<() => void>(() => {});

const GlobalReloadIdProvider = ({ children }: PropsWithChildren) => {
  const [selectedGlobalReloadId, setGlobalReloadId] = useState<string>(nanoid());
  const updateGlobalReloadId = () => setGlobalReloadId(nanoid());

  return (
    <GlobalReloadIdContext.Provider value={selectedGlobalReloadId}>
      <UpdateGlobalReloadIdContext.Provider value={updateGlobalReloadId}>{children}</UpdateGlobalReloadIdContext.Provider>
    </GlobalReloadIdContext.Provider>
  );
};

export default GlobalReloadIdProvider;

import { nanoid } from 'nanoid';
import { createContext, useState } from 'react';

export const GlobalReloadIdContext = createContext<string>(nanoid());
export const UpdateGlobalReloadIdContext = createContext<() => void>(() => {});

type Props = {
  children: React.ReactNode;
};

const GlobalReloadIdProvider = ({ children }: Props) => {
  const [selectedGlobalReloadId, setGlobalReloadId] = useState<string>(nanoid());
  const updateGlobalReloadId = () => {
    setGlobalReloadId(nanoid());
  };

  return (
    <GlobalReloadIdContext.Provider value={selectedGlobalReloadId}>
      <UpdateGlobalReloadIdContext.Provider value={updateGlobalReloadId}>{children}</UpdateGlobalReloadIdContext.Provider>
    </GlobalReloadIdContext.Provider>
  );
};

export default GlobalReloadIdProvider;

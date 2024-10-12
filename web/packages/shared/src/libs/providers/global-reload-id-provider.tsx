'use client';

import { createContext, useState } from 'react';
import { v4 as uuidv4 } from 'uuid';

export const SelectedGlobalReloadIdContext = createContext<string>(uuidv4());
export const UpdateSelectedGlobalReloadIdContext = createContext<() => void>(() => {});

type Props = {
  children: React.ReactNode;
};

const SelectedGlobalReloadIdProvider = ({ children }: Props) => {
  const [selectedGlobalReloadIdId, setSelectedGlobalReloadIdId] = useState<string>(uuidv4());
  const updateSelectedGlobalReloadId = () => {
    setSelectedGlobalReloadIdId(selectedGlobalReloadIdId);
  };

  return (
    <SelectedGlobalReloadIdContext.Provider value={selectedGlobalReloadIdId}>
      <UpdateSelectedGlobalReloadIdContext.Provider value={updateSelectedGlobalReloadId}>{children}</UpdateSelectedGlobalReloadIdContext.Provider>
    </SelectedGlobalReloadIdContext.Provider>
  );
};

export default SelectedGlobalReloadIdProvider;

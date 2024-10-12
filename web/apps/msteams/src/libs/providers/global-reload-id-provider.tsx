import { nanoid } from 'nanoid';
import { createContext, useState } from 'react';

export const SelectedGlobalReloadIdContext = createContext<string>(nanoid());
export const UpdateSelectedGlobalReloadIdContext = createContext<() => void>(() => {});

type Props = {
  children: React.ReactNode;
};

const SelectedGlobalReloadIdProvider = ({ children }: Props) => {
  const [selectedGlobalReloadIdId, setSelectedGlobalReloadIdId] = useState<string>(nanoid());
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

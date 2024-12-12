import { createContext, PropsWithChildren, useEffect, useState } from 'react';
import { useLocalStorage } from 'usehooks-ts';

export const SelectedOrganizationContext = createContext<string | undefined>(undefined);
export const UpdateSelectedOrganizationContext = createContext<(selectedOrganizationId: string) => void>(() => {});

const SelectedOrganizationProvider = ({ children }: PropsWithChildren) => {
  const [persistedSelectedOrganizationId, setPersistedSelectedOrganizationId] = useLocalStorage<string | undefined>(
    'selectedOrganizationId',
    undefined,
  );
  const [selectedOrganizationId, setSelectedOrganizationId] = useState<string | undefined>(undefined);

  useEffect(() => setSelectedOrganizationId(persistedSelectedOrganizationId ?? undefined), [persistedSelectedOrganizationId]);

  const updateSelectedOrganizationId = (selectedOrganizationId: string) => {
    setSelectedOrganizationId(selectedOrganizationId);
    setPersistedSelectedOrganizationId(selectedOrganizationId);
  };

  return (
    <SelectedOrganizationContext.Provider value={selectedOrganizationId}>
      <UpdateSelectedOrganizationContext.Provider value={updateSelectedOrganizationId}>{children}</UpdateSelectedOrganizationContext.Provider>
    </SelectedOrganizationContext.Provider>
  );
};

export default SelectedOrganizationProvider;

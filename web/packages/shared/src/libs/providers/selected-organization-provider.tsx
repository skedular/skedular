'use client';

import { createContext, useEffect, useState } from 'react';
import { useLocalStorage } from 'usehooks-ts';

export const SelectedOrganizationContext = createContext<string | null>(null);
export const UpdateSelectedOrganizationContext = createContext<(selectedOrganizationId: string | null) => void>(() => {});

type Props = {
  children: React.ReactNode;
};

const SelectedOrganizationProvider = ({ children }: Props) => {
  const [persistedSelectedOrganizationId, setPersistedSelectedOrganizationId] = useLocalStorage<string | null>('selected-organization', null);
  const [selectedOrganizationId, setSelectedOrganizationId] = useState<string | null>(null);

  useEffect(() => {
    let finalSelectedOrganizationId: string | null;
    if (persistedSelectedOrganizationId) {
      finalSelectedOrganizationId = persistedSelectedOrganizationId;
    } else {
      finalSelectedOrganizationId = null;
    }

    setSelectedOrganizationId(finalSelectedOrganizationId);
  }, [persistedSelectedOrganizationId]);

  const updateSelectedOrganization = (selectedOrganizationId: string | null) => {
    setSelectedOrganizationId(selectedOrganizationId);
    setPersistedSelectedOrganizationId(selectedOrganizationId);
  };

  return (
    <SelectedOrganizationContext.Provider value={selectedOrganizationId}>
      <UpdateSelectedOrganizationContext.Provider value={updateSelectedOrganization}>{children}</UpdateSelectedOrganizationContext.Provider>
    </SelectedOrganizationContext.Provider>
  );
};

export default SelectedOrganizationProvider;

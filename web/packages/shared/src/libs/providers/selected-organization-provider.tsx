'use client';

import { createContext, useEffect, useState } from 'react';
import { useLocalStorage } from 'usehooks-ts';

export const SelectedOrganizationContext = createContext<string | undefined>(undefined);
export const UpdateSelectedOrganizationContext = createContext<(selectedOrganizationId: string) => void>(() => {});

type Props = {
  children: React.ReactNode;
};

const SelectedOrganizationProvider = ({ children }: Props) => {
  const [persistedSelectedOrganizationId, setPersistedSelectedOrganizationId] = useLocalStorage<string | undefined>(
    'selected-organization',
    undefined,
  );
  const [selectedOrganizationId, setSelectedOrganizationId] = useState<string | undefined>();

  useEffect(() => {
    let finalSelectedOrganizationId: string | undefined;
    if (persistedSelectedOrganizationId) {
      finalSelectedOrganizationId = persistedSelectedOrganizationId;
    } else {
      finalSelectedOrganizationId = undefined;
    }

    setSelectedOrganizationId(finalSelectedOrganizationId);
  }, [persistedSelectedOrganizationId]);

  const updateSelectedOrganization = (selectedOrganizationId: string | undefined) => {
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

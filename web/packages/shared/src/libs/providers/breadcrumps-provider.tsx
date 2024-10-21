import { createContext, useState } from 'react';

export const BreadcrumpsContext = createContext<Map<string, string>>(new Map());
export const UpdateBreadcrumpsContext = createContext<(newEntries: Map<string, string>) => void>(() => {});

type Props = {
  children: React.ReactNode;
};

const BreadcrumpsProvider = ({ children }: Props) => {
  const [breadcrumps, setBreadcrumps] = useState<Map<string, string>>(new Map());
  const updateBreadcrump = (newEntries: Map<string, string>) => {
    setBreadcrumps(newEntries);
  };

  return (
    <BreadcrumpsContext.Provider value={breadcrumps}>
      <UpdateBreadcrumpsContext.Provider value={updateBreadcrump}>{children}</UpdateBreadcrumpsContext.Provider>
    </BreadcrumpsContext.Provider>
  );
};

export default BreadcrumpsProvider;

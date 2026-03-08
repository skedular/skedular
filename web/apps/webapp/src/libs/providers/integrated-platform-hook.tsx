import { usePathname } from 'next/navigation';
import { useMemo } from 'react';

const useIntegratedPlatrform = () => {
  const pathname = usePathname();
  const inMsTeams = useMemo(() => !!pathname && pathname.toLowerCase().startsWith('/msteams'), [pathname]);

  return {
    integratedPlatrform: inMsTeams ? 'msteams' : undefined,
  };
};

export default useIntegratedPlatrform;

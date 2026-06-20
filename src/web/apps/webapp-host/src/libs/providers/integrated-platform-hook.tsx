import { usePathname } from 'next/navigation';
import { useMemo } from 'react';

const useIntegratedPlatform = () => {
  const pathname = usePathname();
  const inMsTeams = useMemo(() => !!pathname && pathname.toLowerCase().startsWith('/msteams'), [pathname]);

  return {
    integratedPlatform: inMsTeams ? 'msteams' : undefined,
  };
};

export default useIntegratedPlatform;

import Typography from '@mui/material/Typography';
import { memo, useMemo } from 'react';
import NavigationRootLinkWithSubItems from './navigation-root-link-with-subitems';
import NavigationRootLinkWithoutSubItems from './navigation-root-link-without-subitems';
import { Navigation } from './navigation.data';

interface Props {
  navigation: Navigation;
}

const NavigationLink = ({ navigation }: Props) => {
  const component = useMemo(() => <Typography variant="h5">{navigation.label}</Typography>, [navigation.label]);

  return (
    <>
      {!navigation.subItems && <NavigationRootLinkWithoutSubItems navigation={navigation}>{component}</NavigationRootLinkWithoutSubItems>}
      {navigation.subItems && <NavigationRootLinkWithSubItems navigation={navigation}>{component}</NavigationRootLinkWithSubItems>}
    </>
  );
};

export default memo(NavigationLink);

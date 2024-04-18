import Box from '@mui/material/Box';
import { memo } from 'react';
import NavigationLink from './navigation-link';
import { appBarNavigations } from './navigation.data';

const Navigation = () => {
  return (
    <Box sx={{ display: 'flex', flexDirection: { xs: 'column', md: 'row' } }}>
      {appBarNavigations.map((navigation, index) => (
        <NavigationLink key={index} navigation={navigation} />
      ))}
    </Box>
  );
};

export default memo(Navigation);

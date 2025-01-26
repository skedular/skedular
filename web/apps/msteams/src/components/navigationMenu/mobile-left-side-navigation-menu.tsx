import Drawer, { drawerClasses } from '@mui/material/Drawer';
import { memo } from 'react';
import { secondDrawerExpandedDrawerWidth } from './commons';
import LeftSideNavigationMenuContent from './left-side-navigation-menu-content';

type Props = {
  open: boolean | undefined;
  toggleDrawer: (newOpen: boolean) => () => void;
};

const MobileLeftSideNavigationMenu = ({ open, toggleDrawer }: Props) => {
  return (
    <Drawer
      anchor="right"
      open={open}
      onClose={toggleDrawer(false)}
      sx={{
        zIndex: (theme) => theme.zIndex.drawer + 1,
        width: secondDrawerExpandedDrawerWidth + 10,
        [`& .${drawerClasses.paper}`]: {
          width: secondDrawerExpandedDrawerWidth + 10,
        },
      }}
    >
      <LeftSideNavigationMenuContent />
    </Drawer>
  );
};

export default memo(MobileLeftSideNavigationMenu);

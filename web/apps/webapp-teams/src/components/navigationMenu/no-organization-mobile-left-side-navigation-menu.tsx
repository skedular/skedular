import Drawer, { drawerClasses } from '@mui/material/Drawer';
import { memo } from 'react';
import { secondDrawerExpandedDrawerWidth } from './commons';
import NoOrganizationLeftSideNavigationMenuContent from './no-organization-left-side-navigation-menu-content';

type Props = {
  open: boolean | undefined;
  toggleDrawer: (newOpen: boolean) => () => void;
};

const MobileLeftSideNavigationMenu = ({ open, toggleDrawer }: Props) => (
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
    <NoOrganizationLeftSideNavigationMenuContent />
  </Drawer>
);

export default memo(MobileLeftSideNavigationMenu);

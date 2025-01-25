import Drawer, { drawerClasses } from '@mui/material/Drawer';
import { secondDrawerCollapsedDrawerWidth, secondDrawerExpandedDrawerWidth } from '@repo/shared/libs/theme';
import { memo, useState } from 'react';
import LeftSideNavigationMenuContent from './left-side-navigation-menu-content';

type Props = {
  collapsed?: boolean;
  hideIcons?: boolean;
};

const LeftSideNavigationMenu = ({ collapsed, hideIcons }: Props) => {
  const [isCollpased, setIsCollpased] = useState(collapsed);
  const drawerWidth = isCollpased ? secondDrawerCollapsedDrawerWidth : secondDrawerExpandedDrawerWidth;

  const toggleCollapse = (collapsed: boolean) => {
    setIsCollpased(collapsed);
  };

  return (
    <Drawer
      sx={{
        display: { xs: 'none', sm: 'block' },
        width: drawerWidth + 10,
        [`& .${drawerClasses.paper}`]: {
          width: drawerWidth + 10,
        },
      }}
      variant="permanent"
    >
      <LeftSideNavigationMenuContent collapsed={collapsed} enableCollapseButton toggleCollapse={toggleCollapse} hideIcons={hideIcons} />
    </Drawer>
  );
};

export default memo(LeftSideNavigationMenu);

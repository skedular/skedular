import Drawer, { drawerClasses } from '@mui/material/Drawer';
import { useLeftNavigationCollapsed } from '@skedular/shared';
import { memo } from 'react';
import { secondDrawerCollapsedDrawerWidth, secondDrawerExpandedDrawerWidth } from './commons';
import NoOrganizationLeftSideNavigationMenuContent from './no-organization-left-side-navigation-menu-content';

type Props = {
  hideIcons?: boolean;
};

const NoOrganizationLeftSideNavigationMenu = ({ hideIcons }: Props) => {
  const [isCollpased, setIsCollpased] = useLeftNavigationCollapsed();
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
      <NoOrganizationLeftSideNavigationMenuContent collapsed={isCollpased} enableCollapseButton toggleCollapse={toggleCollapse} hideIcons={hideIcons} />
    </Drawer>
  );
};

export default memo(NoOrganizationLeftSideNavigationMenu);

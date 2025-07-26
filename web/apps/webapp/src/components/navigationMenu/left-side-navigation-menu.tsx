import type { leftSideNavigationMenu_query$key } from '@/queries/__generated__/leftSideNavigationMenu_query.graphql';
import Drawer, { drawerClasses } from '@mui/material/Drawer';
import { memo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
import { secondDrawerCollapsedDrawerWidth, secondDrawerExpandedDrawerWidth } from './commons';
import LeftSideNavigationMenuContent from './left-side-navigation-menu-content';

type Props = {
  rootDataRelay: leftSideNavigationMenu_query$key;
  collapsed?: boolean;
  hideIcons?: boolean;
};

const LeftSideNavigationMenu = ({ rootDataRelay, collapsed, hideIcons }: Props) => {
  const rootData = useFragment<leftSideNavigationMenu_query$key>(
    graphql`
      fragment leftSideNavigationMenu_query on Query {
        ...leftSideNavigationMenuContent_query
      }
    `,
    rootDataRelay,
  );

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
      <LeftSideNavigationMenuContent rootDataRelay={rootData} collapsed={isCollpased} enableCollapseButton toggleCollapse={toggleCollapse} hideIcons={hideIcons} />
    </Drawer>
  );
};

export default memo(LeftSideNavigationMenu);

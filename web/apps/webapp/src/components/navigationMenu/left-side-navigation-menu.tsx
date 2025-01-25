import { getNotificationsBaseLink, getOrganizationAddLink } from '@/components/links';
import type { leftSideNavigationMenu_query$key } from '@/queries/__generated__/leftSideNavigationMenu_query.graphql';
import Drawer, { drawerClasses } from '@mui/material/Drawer';
import { secondDrawerCollapsedDrawerWidth, secondDrawerExpandedDrawerWidth } from '@repo/shared/libs/theme';
import { usePathname } from 'next/navigation';
import { memo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
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

  const pathName = usePathname();
  const [isCollpased, setIsCollpased] = useState(collapsed);
  const drawerWidth = isCollpased ? secondDrawerCollapsedDrawerWidth : secondDrawerExpandedDrawerWidth;

  const toggleCollapse = (collapsed: boolean) => {
    setIsCollpased(collapsed);
  };

  const organizationAddLink = getOrganizationAddLink();
  const notificationsLink = getNotificationsBaseLink();

  if (pathName === organizationAddLink || pathName === notificationsLink) {
    return <></>;
  }

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

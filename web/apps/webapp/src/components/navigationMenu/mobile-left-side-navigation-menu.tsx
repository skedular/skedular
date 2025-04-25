import { getMeLink, getNotificationsBaseLink, getOrganizationAddLink } from '@/components/links';
import type { mobileLeftSideNavigationMenu_query$key } from '@/queries/__generated__/mobileLeftSideNavigationMenu_query.graphql';
import Drawer, { drawerClasses } from '@mui/material/Drawer';
import { usePathname } from 'next/navigation';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';
import { secondDrawerExpandedDrawerWidth } from './commons';
import LeftSideNavigationMenuContent from './left-side-navigation-menu-content';

type Props = {
  rootDataRelay: mobileLeftSideNavigationMenu_query$key;
  open: boolean | undefined;
  toggleDrawer: (newOpen: boolean) => () => void;
};

const MobileLeftSideNavigationMenu = ({ rootDataRelay, open, toggleDrawer }: Props) => {
  const rootData = useFragment<mobileLeftSideNavigationMenu_query$key>(
    graphql`
      fragment mobileLeftSideNavigationMenu_query on Query {
        ...leftSideNavigationMenuContent_query
      }
    `,
    rootDataRelay,
  );

  const pathName = usePathname();
  const meLink = getMeLink();
  const organizationAddLink = getOrganizationAddLink();
  const notificationsLink = getNotificationsBaseLink();

  if (pathName === meLink || pathName === organizationAddLink || pathName === notificationsLink) {
    return <></>;
  }

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
      <LeftSideNavigationMenuContent rootDataRelay={rootData} />
    </Drawer>
  );
};

export default memo(MobileLeftSideNavigationMenu);

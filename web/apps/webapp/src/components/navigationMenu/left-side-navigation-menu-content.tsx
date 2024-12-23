import type { leftSideNavigationMenuContent_query$key } from '@/queries/__generated__/leftSideNavigationMenuContent_query.graphql';
import { SwitchToModernUIContext } from '@repo/shared/libs/providers';
import { memo, useContext } from 'react';
import { graphql, useFragment } from 'react-relay';
import ModernLeftSideNavigationMenuContent from './modern-left-side-navigation-menu-content';
import OldLeftSideNavigationMenu from './old-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: leftSideNavigationMenuContent_query$key;
  collapsed?: boolean;
  enableCollapseButton?: boolean;
  toggleCollapse?: (collapsed: boolean) => void;
  hideIcons?: boolean;
};

const LeftSideNavigationMenuContent = ({ rootDataRelay, collapsed, enableCollapseButton, toggleCollapse, hideIcons }: Props) => {
  const rootData = useFragment<leftSideNavigationMenuContent_query$key>(
    graphql`
      fragment leftSideNavigationMenuContent_query on Query {
        ...modernLeftSideNavigationMenuContent_query
      }
    `,
    rootDataRelay,
  );
  const switchToModernUI = useContext(SwitchToModernUIContext);

  return switchToModernUI ? (
    <ModernLeftSideNavigationMenuContent
      rootDataRelay={rootData}
      collapsed={collapsed}
      enableCollapseButton={enableCollapseButton}
      toggleCollapse={toggleCollapse}
      hideIcons={hideIcons}
    />
  ) : (
    <OldLeftSideNavigationMenu
      collapsed={collapsed}
      enableCollapseButton={enableCollapseButton}
      toggleCollapse={toggleCollapse}
      hideIcons={hideIcons}
    />
  );
};

export default memo(LeftSideNavigationMenuContent);

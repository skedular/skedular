import { SwitchToModernUIContext } from '@repo/shared/libs/providers';
import type { JSX } from 'react';
import { memo, useContext } from 'react';
import { graphql, useFragment } from 'react-relay';
import type { appBar_query$key } from './__generated__/appBar_query.graphql';
import ModernAppBar from './modern-app-bar';
import OldAppBar from './old-app-bar';

type Props = {
  rootDataRelay: appBar_query$key;
  hideWelcomeMessage?: boolean;
  showBreadcrumps?: boolean;
  breadcrumbs?: React.ReactNode | JSX.Element;
};

const AppBar = ({ rootDataRelay, hideWelcomeMessage, showBreadcrumps, breadcrumbs }: Props) => {
  const rootData = useFragment<appBar_query$key>(
    graphql`
      fragment appBar_query on Query {
        ...modernAppBar_query
        ...oldAppBar_query
      }
    `,
    rootDataRelay,
  );

  const switchToModernUI = useContext(SwitchToModernUIContext);

  return switchToModernUI ? (
    <ModernAppBar rootDataRelay={rootData} hideWelcomeMessage={hideWelcomeMessage} showBreadcrumps={showBreadcrumps} breadcrumbs={breadcrumbs} />
  ) : (
    <OldAppBar rootDataRelay={rootData} />
  );
};

export default memo(AppBar);

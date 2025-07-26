import { GridContainer } from '@/components/commons';
import type { PropsWithChildren } from 'react';
import { memo } from 'react';

type Props = {};

const TwoSideVerticalWizard = ({ children }: PropsWithChildren<Props>) => (
  <GridContainer
    spacing={1}
    sx={{
      height: (theme) => {
        const appBarMinHeight = theme.mixins.toolbar.minHeight?.toString();

        return `calc(100vh - ${(appBarMinHeight ? parseInt(appBarMinHeight) : 56) + 10}px)`;
      },
    }}
    wrap="nowrap"
  >
    {children}
  </GridContainer>
);

export default memo(TwoSideVerticalWizard);

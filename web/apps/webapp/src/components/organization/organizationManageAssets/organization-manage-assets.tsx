import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import { BodyIconTypography, SectionIconTypography, StackColumn, StackColumnWithSaveExitCancelAppBar } from '@repo/shared/components/commons';
import { defaultPadding } from '@repo/shared/libs/theme';
import { useSearchParams } from 'next/navigation';
import { memo, useEffect, useRef } from 'react';
import { expandedDrawerWidthPx } from './commons';
import OrganizationManageAssetsLeftSideNavigationMenuContent from './organization-manage-assets-left-side-navigation-menu-content';

type Props = {
  organizationId: string;
};

const OrganizationManageAssets = ({ organizationId }: Props) => {
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});

  useEffect(() => {
    if (!section || section === 'zones-setup') {
      return;
    }

    const element = sectionRefs.current[section];
    if (!element) {
      return;
    }

    const appBarHeight = document.querySelector('.app-bar')?.clientHeight || 0;
    const elementTop = element.getBoundingClientRect().top + window.scrollY;
    window.scrollTo({
      top: elementTop - appBarHeight,
      behavior: 'smooth',
    });
  }, [section]);

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <OrganizationManageAssetsLeftSideNavigationMenuContent organizationId={organizationId} hideIcons />
        <Box sx={{ marginLeft: expandedDrawerWidthPx, flexGrow: 1 }}>
          <StackColumnWithSaveExitCancelAppBar label="Manage Assets" hideCancel hideSaveAndExit>
            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['zones-setup'] = divElement;
              }}
            >
              <SectionIconTypography label="Zones Setup" />
              <BodyIconTypography label="Edit your organization zones details" />
              <Divider />
            </StackColumn>

            <StackColumn
              sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
              ref={(divElement) => {
                sectionRefs.current['desk-types-setup'] = divElement;
              }}
            >
              <SectionIconTypography label="Desk Types Setup" />
              <BodyIconTypography label="Edit your organization desk types details" />
              <Divider />
            </StackColumn>
          </StackColumnWithSaveExitCancelAppBar>
        </Box>
      </Box>
    </>
  );
};

export default memo(OrganizationManageAssets);

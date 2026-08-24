import { getOrganizationLocationResourceOpeningHoursBaseLink, getOrganizationLocationResourceSetupBaseLink } from '@/components/links';
import { useIntegratedPlatform } from '@skedular/shared';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import ExpandMoreRoundedIcon from '@mui/icons-material/ExpandMoreRounded';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import NextLink from 'next/link';
import { memo, useState, type MouseEvent } from 'react';

export type ResourceEditSection = 'setup' | 'opening-hours';

type Props = {
  activeSection: ResourceEditSection;
  organizationCustomDomain: string;
  locationId: string;
  resourceId: string;
  stickyTop?: number;
};

const sectionLabels: Record<ResourceEditSection, string> = {
  setup: 'Resource Setup',
  'opening-hours': 'Opening Hours',
};

const ResourceEditSectionNav = ({ activeSection, organizationCustomDomain, locationId, resourceId }: Props) => {
  const { integratedPlatform } = useIntegratedPlatform();
  const theme = useTheme();
  const isCompactNav = useMediaQuery(theme.breakpoints.down('sm'), { noSsr: true });
  const [menuAnchor, setMenuAnchor] = useState<HTMLElement | null>(null);
  const sectionLinks: Record<ResourceEditSection, string> = {
    setup: getOrganizationLocationResourceSetupBaseLink(integratedPlatform, organizationCustomDomain, locationId, resourceId),
    'opening-hours': getOrganizationLocationResourceOpeningHoursBaseLink(integratedPlatform, organizationCustomDomain, locationId, resourceId),
  };

  const handleOpenMenu = (event: MouseEvent<HTMLElement>) => {
    setMenuAnchor(event.currentTarget);
  };

  const handleCloseMenu = () => {
    setMenuAnchor(null);
  };

  return (
    <Box sx={{ width: '100%', minWidth: 0 }}>
      {isCompactNav ? (
        <>
          <Button
            fullWidth
            variant="outlined"
            color="inherit"
            onClick={handleOpenMenu}
            aria-haspopup="menu"
            aria-expanded={menuAnchor ? 'true' : undefined}
            aria-controls={menuAnchor ? 'resource-edit-sections-menu' : undefined}
            endIcon={<ExpandMoreRoundedIcon />}
            sx={{ justifyContent: 'space-between', minHeight: 48, borderRadius: 2.5, px: 2, textTransform: 'none' }}
          >
            {`Section: ${sectionLabels[activeSection]}`}
          </Button>

          <Menu anchorEl={menuAnchor} open={Boolean(menuAnchor)} onClose={handleCloseMenu} id="resource-edit-sections-menu">
            {(Object.keys(sectionLabels) as ResourceEditSection[]).map((section) => (
              <MenuItem key={section} component={NextLink} href={sectionLinks[section]} selected={activeSection === section} onClick={handleCloseMenu}>
                {sectionLabels[section]}
              </MenuItem>
            ))}
          </Menu>
        </>
      ) : (
        <Tabs
          value={activeSection}
          variant="scrollable"
          scrollButtons="auto"
          aria-label="Resource sections"
          sx={{ borderTop: 1, borderColor: 'divider', '& .MuiTabs-indicator': { height: 3, borderRadius: '3px 3px 0 0' } }}
        >
          {(Object.keys(sectionLabels) as ResourceEditSection[]).map((section) => (
            <Tab
              key={section}
              value={section}
              component={NextLink}
              href={sectionLinks[section]}
              label={sectionLabels[section]}
              disableRipple
              role="link"
              className={activeSection === section ? 'MuiButton-contained' : 'MuiButton-text'}
              sx={{
                minWidth: 112,
                minHeight: 52,
                px: 2.5,
                textTransform: 'none',
                color: 'text.secondary',
                fontWeight: 500,
                '&.Mui-selected': { color: 'primary.main', fontWeight: 600 },
                '&:hover': { color: 'text.primary', backgroundColor: 'action.hover' },
              }}
            ></Tab>
          ))}
        </Tabs>
      )}
    </Box>
  );
};

export default memo(ResourceEditSectionNav);

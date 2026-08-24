import { getOrganizationBookingsBaseLink, getOrganizationTeamManageTeamBaseLink, getOrganizationTeamMembersBaseLink, getOrganizationTeamSetupBaseLink } from '@/components/links';
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

export type OrganizationTeamSection = 'setup' | 'members' | 'manage-team';

type Props = {
  activeSection: OrganizationTeamSection;
  organizationCustomDomain: string;
  teamId: string;
  stickyTop?: number;
};

const sectionLabels: Record<OrganizationTeamSection, string> = {
  setup: 'Presentation',
  members: 'Members',
  'manage-team': 'Manage',
};

const OrganizationTeamSectionNav = ({ activeSection, organizationCustomDomain, teamId }: Props) => {
  const { integratedPlatform } = useIntegratedPlatform();
  const theme = useTheme();
  const isCompactNav = useMediaQuery(theme.breakpoints.down('sm'), { noSsr: true });
  const [menuAnchor, setMenuAnchor] = useState<HTMLElement | null>(null);
  const bookingsLink = getOrganizationBookingsBaseLink(integratedPlatform, organizationCustomDomain, { teamId });
  const sectionLinks: Record<OrganizationTeamSection, string> = {
    setup: getOrganizationTeamSetupBaseLink(integratedPlatform, organizationCustomDomain, teamId),
    members: getOrganizationTeamMembersBaseLink(integratedPlatform, organizationCustomDomain, teamId),
    'manage-team': getOrganizationTeamManageTeamBaseLink(integratedPlatform, organizationCustomDomain, teamId),
  };

  const handleOpenMenu = (event: MouseEvent<HTMLElement>) => {
    setMenuAnchor(event.currentTarget);
  };

  const handleCloseMenu = () => {
    setMenuAnchor(null);
  };

  return (
    <Box sx={{ width: '100%', minWidth: 0, position: 'relative', zIndex: 2 }}>
      {isCompactNav ? (
        <>
          <Button
            fullWidth
            variant="outlined"
            color="inherit"
            onClick={handleOpenMenu}
            aria-haspopup="menu"
            aria-expanded={menuAnchor ? 'true' : undefined}
            aria-controls={menuAnchor ? 'organization-team-sections-menu' : undefined}
            endIcon={<ExpandMoreRoundedIcon />}
            sx={{ justifyContent: 'space-between', minHeight: 48, borderRadius: 2.5, px: 2, textTransform: 'none' }}
          >
            {`Section: ${sectionLabels[activeSection]}`}
          </Button>

          <Menu anchorEl={menuAnchor} open={Boolean(menuAnchor)} onClose={handleCloseMenu} id="organization-team-sections-menu">
            {(Object.keys(sectionLabels) as OrganizationTeamSection[]).map((section) => (
              <MenuItem
                key={section}
                component={NextLink}
                href={sectionLinks[section]}
                aria-label={section === 'setup' ? 'Team Setup' : undefined}
                selected={activeSection === section}
                onClick={handleCloseMenu}
              >
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
          aria-label="Team sections"
          sx={{ borderTop: 1, borderColor: 'divider', '& .MuiTabs-indicator': { height: 3, borderRadius: '3px 3px 0 0' } }}
        >
          {(Object.keys(sectionLabels) as OrganizationTeamSection[]).map((section) => (
            <Tab
              key={section}
              value={section}
              component={NextLink}
              href={sectionLinks[section]}
              role="link"
              aria-label={section === 'setup' ? 'Team Setup' : undefined}
              className={activeSection === section ? 'MuiButton-contained' : 'MuiButton-text'}
              label={sectionLabels[section]}
              disableRipple
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

      <Button
        component={NextLink}
        href={bookingsLink}
        variant="outlined"
        color="inherit"
        sx={{ flexShrink: 0, borderRadius: 2.5, alignSelf: { xs: 'stretch', md: 'flex-end' }, px: 2, textTransform: 'none', whiteSpace: 'nowrap', mt: 1 }}
      >
        View team bookings
      </Button>
    </Box>
  );
};

export default memo(OrganizationTeamSectionNav);

import { getOrganizationBookingsBaseLink, getOrganizationUserManageBaseLink, getOrganizationUserManageTeamsBaseLink, getOrganizationUserProfileBaseLink } from '@/components/links';
import { useIntegratedPlatform } from '@skedular/shared';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import NextLink from 'next/link';
import { memo, useState, type MouseEvent } from 'react';

export type OrganizationUserSection = 'profile' | 'manage-teams' | 'manage-user';

type Props = {
  activeSection: OrganizationUserSection;
  organizationCustomDomain: string;
  customerId: string;
};

const sectionLabels: Record<OrganizationUserSection, string> = {
  profile: 'Profile',
  'manage-teams': 'Teams',
  'manage-user': 'Manage',
};

const OrganizationUserSectionNav = ({ activeSection, organizationCustomDomain, customerId }: Props) => {
  const { integratedPlatform } = useIntegratedPlatform();
  const theme = useTheme();
  const isCompactNav = useMediaQuery(theme.breakpoints.down('md'), { noSsr: true });
  const [menuAnchor, setMenuAnchor] = useState<HTMLElement | null>(null);
  const bookingsLink = getOrganizationBookingsBaseLink(integratedPlatform, organizationCustomDomain, { customerId });
  const sectionLinks: Record<OrganizationUserSection, string> = {
    profile: getOrganizationUserProfileBaseLink(integratedPlatform, organizationCustomDomain, customerId),
    'manage-teams': getOrganizationUserManageTeamsBaseLink(integratedPlatform, organizationCustomDomain, customerId),
    'manage-user': getOrganizationUserManageBaseLink(integratedPlatform, organizationCustomDomain, customerId),
  };

  const handleOpenMenu = (event: MouseEvent<HTMLElement>) => {
    setMenuAnchor(event.currentTarget);
  };

  const handleCloseMenu = () => {
    setMenuAnchor(null);
  };

  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: { xs: 'column', md: 'row' },
        alignItems: { xs: 'stretch', md: 'center' },
        gap: 1,
        mt: 1.5,
        pt: 1,
        borderTop: 1,
        borderColor: 'divider',
        borderRadius: 0,
        boxShadow: 'none',
        bgcolor: 'transparent',
        px: { xs: 2, sm: 3 },
        py: 0,
        position: 'relative',
      }}
    >
      {isCompactNav ? (
        <>
          <Button
            variant="contained"
            color="primary"
            onClick={handleOpenMenu}
            aria-haspopup="menu"
            aria-expanded={menuAnchor ? 'true' : undefined}
            aria-controls={menuAnchor ? 'organization-user-sections-menu' : undefined}
            sx={{
              justifyContent: 'space-between',
              borderRadius: 999,
              px: 2,
              textTransform: 'none',
            }}
          >
            {`Section: ${sectionLabels[activeSection]}`}
          </Button>

          <Menu anchorEl={menuAnchor} open={Boolean(menuAnchor)} onClose={handleCloseMenu} id="organization-user-sections-menu">
            {(Object.keys(sectionLabels) as OrganizationUserSection[]).map((section) => (
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
          aria-label="User profile sections"
          sx={{
            flex: '1 1 0%',
            minWidth: 0,
            mb: -2,
            '& .MuiTabs-indicator': { height: 3, borderRadius: '3px 3px 0 0' },
          }}
        >
          {(Object.keys(sectionLabels) as OrganizationUserSection[]).map((section) => (
            <Tab
              key={section}
              value={section}
              component={NextLink}
              href={sectionLinks[section]}
              label={sectionLabels[section]}
              disableRipple
              sx={{
                minWidth: 112,
                minHeight: 52,
                px: 2.5,
                textTransform: 'none',
                whiteSpace: 'nowrap',
                color: 'text.secondary',
                fontWeight: 500,
                '&.Mui-selected': { color: 'primary.main', fontWeight: 600 },
              }}
            />
          ))}
        </Tabs>
      )}

      <Button
        component={NextLink}
        href={bookingsLink}
        variant="outlined"
        color="inherit"
        sx={{
          flexShrink: 0,
          borderRadius: 999,
          alignSelf: { xs: 'stretch', md: 'center' },
          px: 2,
          textTransform: 'none',
          whiteSpace: 'nowrap',
        }}
      >
        View user bookings
      </Button>
    </Box>
  );
};

export default memo(OrganizationUserSectionNav);

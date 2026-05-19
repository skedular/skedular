import {
  getOrganizationBookingsBaseLink,
  getOrganizationLocationFloorPlansBaseLink,
  getOrganizationLocationManageLocationBaseLink,
  getOrganizationLocationManageResourcesBaseLink,
  getOrganizationLocationOpeningHoursBaseLink,
  getOrganizationLocationPhysicalAddressSetupBaseLink,
  getOrganizationLocationRestrictedInformationBaseLink,
  getOrganizationLocationSetupBaseLink,
} from '@/components/links';
import { useIntegratedPlatrform } from '@skedular/shared';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import { useTheme } from '@mui/material/styles';
import useMediaQuery from '@mui/material/useMediaQuery';
import NextLink from 'next/link';
import { memo, useState, type MouseEvent } from 'react';

export type OrganizationLocationSection = 'setup' | 'physical-address-setup' | 'opening-hours' | 'floor-plans' | 'manage-resources' | 'restricted-information' | 'manage-location';

type Props = {
  activeSection: OrganizationLocationSection;
  organizationCustomDomain: string;
  locationId: string;
  stickyTop?: number;
};

const sectionLabels: Record<OrganizationLocationSection, string> = {
  setup: 'Location Setup',
  'physical-address-setup': 'Physical Address',
  'opening-hours': 'Opening Hours',
  'floor-plans': 'Floor Plans',
  'manage-resources': 'Resources',
  'restricted-information': 'Restricted Info',
  'manage-location': 'Manage',
};

const OrganizationLocationSectionNav = ({ activeSection, organizationCustomDomain, locationId, stickyTop = 0 }: Props) => {
  const { integratedPlatrform } = useIntegratedPlatrform();
  const theme = useTheme();
  const isCompactNav = useMediaQuery(theme.breakpoints.down('md'), { noSsr: true });
  const [menuAnchor, setMenuAnchor] = useState<HTMLElement | null>(null);
  const bookingsLink = getOrganizationBookingsBaseLink(integratedPlatrform, organizationCustomDomain, { locationId });
  const sectionLinks: Record<OrganizationLocationSection, string> = {
    setup: getOrganizationLocationSetupBaseLink(integratedPlatrform, organizationCustomDomain, locationId),
    'physical-address-setup': getOrganizationLocationPhysicalAddressSetupBaseLink(integratedPlatrform, organizationCustomDomain, locationId),
    'opening-hours': getOrganizationLocationOpeningHoursBaseLink(integratedPlatrform, organizationCustomDomain, locationId),
    'floor-plans': getOrganizationLocationFloorPlansBaseLink(integratedPlatrform, organizationCustomDomain, locationId),
    'manage-resources': getOrganizationLocationManageResourcesBaseLink(integratedPlatrform, organizationCustomDomain, locationId),
    'restricted-information': getOrganizationLocationRestrictedInformationBaseLink(integratedPlatrform, organizationCustomDomain, locationId),
    'manage-location': getOrganizationLocationManageLocationBaseLink(integratedPlatrform, organizationCustomDomain, locationId),
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
        px: { xs: 2, sm: 3 },
        py: 2,
        border: 1,
        borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
        borderRadius: 4,
        bgcolor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.88)' : theme.palette.background.paper),
        boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : theme.shadows[1]),
        position: 'sticky',
        top: stickyTop,
        zIndex: 2,
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
            aria-controls={menuAnchor ? 'organization-location-sections-menu' : undefined}
            sx={{
              justifyContent: 'space-between',
              borderRadius: 999,
              px: 2,
              textTransform: 'none',
            }}
          >
            {`Section: ${sectionLabels[activeSection]}`}
          </Button>

          <Menu anchorEl={menuAnchor} open={Boolean(menuAnchor)} onClose={handleCloseMenu} id="organization-location-sections-menu">
            {(Object.keys(sectionLabels) as OrganizationLocationSection[]).map((section) => (
              <MenuItem key={section} component={NextLink} href={sectionLinks[section]} selected={activeSection === section} onClick={handleCloseMenu}>
                {sectionLabels[section]}
              </MenuItem>
            ))}
          </Menu>
        </>
      ) : (
        <Box
          sx={{
            display: 'flex',
            gap: 1,
            overflowX: 'auto',
            flex: '1 1 0%',
            minWidth: 0,
            scrollbarWidth: 'none',
            '&::-webkit-scrollbar': {
              display: 'none',
            },
          }}
        >
          {(Object.keys(sectionLabels) as OrganizationLocationSection[]).map((section) => (
            <Button
              key={section}
              component={NextLink}
              href={sectionLinks[section]}
              variant={activeSection === section ? 'contained' : 'text'}
              color={activeSection === section ? 'primary' : 'inherit'}
              sx={{
                flexShrink: 0,
                borderRadius: 999,
                px: 2,
                textTransform: 'none',
                whiteSpace: 'nowrap',
              }}
            >
              {sectionLabels[section]}
            </Button>
          ))}
        </Box>
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
        View location bookings
      </Button>
    </Box>
  );
};

export default memo(OrganizationLocationSectionNav);

import {
  getOrganizationBookingsBaseLink,
  getOrganizationLocationFloorPlansBaseLink,
  getOrganizationLocationManageLocationBaseLink,
  getOrganizationLocationManageResourcesBaseLink,
  getOrganizationLocationOpeningHoursBaseLink,
  getOrganizationLocationPhysicalAddressSetupBaseLink,
  getOrganizationLocationSetupBaseLink,
} from '@/components/links';
import { useIntegratedPlatrform } from '@/libs/providers';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import NextLink from 'next/link';
import { memo } from 'react';

export type OrganizationLocationSection = 'setup' | 'physical-address-setup' | 'opening-hours' | 'floor-plans' | 'manage-resources' | 'manage-location';

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
  'manage-location': 'Manage',
};

const OrganizationLocationSectionNav = ({ activeSection, organizationCustomDomain, locationId, stickyTop = 0 }: Props) => {
  const { integratedPlatrform } = useIntegratedPlatrform();
  const bookingsLink = getOrganizationBookingsBaseLink(integratedPlatrform, organizationCustomDomain, { locationId });
  const sectionLinks: Record<OrganizationLocationSection, string> = {
    setup: getOrganizationLocationSetupBaseLink(integratedPlatrform, organizationCustomDomain, locationId),
    'physical-address-setup': getOrganizationLocationPhysicalAddressSetupBaseLink(integratedPlatrform, organizationCustomDomain, locationId),
    'opening-hours': getOrganizationLocationOpeningHoursBaseLink(integratedPlatrform, organizationCustomDomain, locationId),
    'floor-plans': getOrganizationLocationFloorPlansBaseLink(integratedPlatrform, organizationCustomDomain, locationId),
    'manage-resources': getOrganizationLocationManageResourcesBaseLink(integratedPlatrform, organizationCustomDomain, locationId),
    'manage-location': getOrganizationLocationManageLocationBaseLink(integratedPlatrform, organizationCustomDomain, locationId),
  };

  return (
    <Box
      sx={{
        display: 'flex',
        flexWrap: 'wrap',
        gap: 1,
        px: { xs: 2, sm: 3 },
        py: 2,
        borderBottom: 1,
        borderColor: 'divider',
        bgcolor: 'background.paper',
        position: 'sticky',
        top: stickyTop,
        zIndex: 2,
      }}
    >
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

      <Button
        component={NextLink}
        href={bookingsLink}
        variant="outlined"
        color="inherit"
        sx={{
          flexShrink: 0,
          borderRadius: 999,
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

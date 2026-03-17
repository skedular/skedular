import { AppBarWithStackColumn, GridContainer, SectionIconTypography, StackColumn } from '@/components/commons';
import { getOrganizationBaseLink } from '@/components/links';
import { LocationBookingInsightRoot } from '@/components/location/locationBookingInsight';
import { LocationDeskOccupancyInsightRoot } from '@/components/location/locationDeskOccupancyInsight';
import { LocationSelector } from '@/components/location/locationSelector';
import { OrganizationBookingInsightRoot } from '@/components/organization/organizationBookingInsight';
import { OrganizationMemberAttendancyInsightRoot } from '@/components/organization/organizationMemberAttendancyInsight';
import { useIntegratedPlatrform } from '@/libs/providers';
import { defaultPadding, secondDrawerExpandedDrawerWidthPx } from '@/libs/theme';
import type { organizationAnalytics_query$key } from '@/queries/__generated__/organizationAnalytics_query.graphql';
import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useEffect, useMemo, useRef, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
import OrganizationAnalyticsLeftSideNavigationMenuContent from './organization-analytics-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationAnalytics_query$key;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

const OrganizationAnalytics = ({ rootDataRelay, onReloadRequired, organizationCustomDomain }: Props) => {
  const rootData = useFragment<organizationAnalytics_query$key>(
    graphql`
      fragment organizationAnalytics_query on Query {
        ...locationSelector_allLocations_query
        locations(where: { organizationCustomDomain: $organizationCustomDomain }, orderBy: $locationsSortingValues) {
          __id
          totalCount
          edges {
            node {
              id
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const searchParams = useSearchParams();
  const section = searchParams.get('section');
  const sectionRefs = useRef<{ [key: string]: HTMLDivElement | null }>({});
  const [locationIds, setLocationIds] = useState<string[]>([]);
  const locations = useMemo(() => (rootData.locations ? rootData.locations.edges.map((edge) => edge.node) : []), [rootData.locations]);
  const locationIdsToDisplay = useMemo(
    () => (locationIds.length > 0 ? locations.filter((item) => locationIds.includes(item.id)) : locations).map((item) => item.id),
    [locationIds, locations],
  );

  useEffect(() => {
    if (!section || section === 'organization') {
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

  const handlLocationChanged = (id?: string) => {
    setLocationIds(id ? [id] : []);
  };

  const handleCloseClick = () => {
    router.push(getOrganizationBaseLink(integratedPlatrform, organizationCustomDomain));
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <OrganizationAnalyticsLeftSideNavigationMenuContent organizationCustomDomain={organizationCustomDomain} hideIcons />
      <Box sx={{ marginLeft: secondDrawerExpandedDrawerWidthPx, flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Analytics">
          <StackColumn
            sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
            ref={(divElement) => {
              sectionRefs.current['organization'] = divElement;
            }}
          >
            <SectionIconTypography label="Organization Analytics" />
            <Divider />
          </StackColumn>

          <GridContainer sx={{ padding: defaultPadding }}>
            <Grid>
              <OrganizationBookingInsightRoot onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} />
            </Grid>
            <Grid>
              <OrganizationMemberAttendancyInsightRoot onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} />
            </Grid>
          </GridContainer>

          <StackColumn
            sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}
            ref={(divElement) => {
              sectionRefs.current['locations'] = divElement;
            }}
          >
            <SectionIconTypography label="Locations Analytics" />
            <Divider />
          </StackColumn>

          <GridContainer sx={{ padding: defaultPadding }}>
            <LocationSelector rootDataRelay={rootData} onChange={handlLocationChanged} />
          </GridContainer>

          {locationIdsToDisplay.length > 0 && (
            <>
              {locationIdsToDisplay.map((locationId) => (
                <>
                  <GridContainer sx={{ padding: defaultPadding }}>
                    <Grid>
                      <LocationBookingInsightRoot onReloadRequired={onReloadRequired} locationId={locationId} />
                    </Grid>
                    <Grid>
                      <LocationDeskOccupancyInsightRoot onReloadRequired={onReloadRequired} locationId={locationId} />
                    </Grid>
                  </GridContainer>
                </>
              ))}
            </>
          )}
        </AppBarWithStackColumn>
      </Box>
    </Box>
  );
};

export default memo(OrganizationAnalytics);

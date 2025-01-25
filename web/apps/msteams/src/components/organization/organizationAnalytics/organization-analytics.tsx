import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import { AppBarWithStackColumn, GridContainer, SectionIconTypography, StackColumn } from '@repo/shared/components/commons';
import { defaultPadding, secondDrawerExpandedDrawerWidthPx } from '@repo/shared/libs/theme';
import graphql from 'babel-plugin-relay/macro';
import { getOrganizationBaseLink } from 'components/links';
import { LocationBookingInsightRoot } from 'components/location/locationBookingInsight';
import { LocationDeskOccupancyInsightRoot } from 'components/location/locationDeskOccupancyInsight';
import { LocationSelector } from 'components/location/locationSelector';
import { OrganizationBookingInsightRoot } from 'components/organization/organizationBookingInsight';
import { OrganizationMemberAttendancyInsightRoot } from 'components/organization/organizationMemberAttendancyInsight';
import { memo, useEffect, useMemo, useRef, useState } from 'react';
import { useFragment } from 'react-relay';
import { useNavigate, useSearchParams } from 'react-router-dom';
import type { organizationAnalytics_query$key } from './__generated__/organizationAnalytics_query.graphql';
import OrganizationAnalyticsLeftSideNavigationMenuContent from './organization-analytics-left-side-navigation-menu-content';

type Props = {
  rootDataRelay: organizationAnalytics_query$key;
  onReloadRequired: () => void;
  organizationId: string;
};

const OrganizationAnalytics = ({ rootDataRelay, onReloadRequired, organizationId }: Props) => {
  const rootData = useFragment<organizationAnalytics_query$key>(
    graphql`
      fragment organizationAnalytics_query on Query {
        ...locationSelector_allLocations_query
        locations(where: { organizationId: $organizationId }, orderBy: $locationsSortingValues) {
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

  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
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
    navigate(getOrganizationBaseLink(organizationId));
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <OrganizationAnalyticsLeftSideNavigationMenuContent organizationId={organizationId} hideIcons />
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
              <OrganizationBookingInsightRoot onReloadRequired={onReloadRequired} organizationId={organizationId} />
            </Grid>
            <Grid>
              <OrganizationMemberAttendancyInsightRoot onReloadRequired={onReloadRequired} organizationId={organizationId} />
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

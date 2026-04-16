import { BodyIconTypography, GridContainer, LeadIconTypography, SectionIconTypography, SmallIconTypography, StackColumn, StackRow } from '@/components/commons';
import { LocationBookingInsightRoot } from '@/components/location/locationBookingInsight';
import { LocationDeskOccupancyInsightRoot } from '@/components/location/locationDeskOccupancyInsight';
import { LocationSelector } from '@/components/location/locationSelector';
import { OrganizationBookingInsightRoot } from '@/components/organization/organizationBookingInsight';
import { OrganizationMemberAttendancyInsightRoot } from '@/components/organization/organizationMemberAttendancyInsight';
import { useIntegratedPlatrform } from '@/libs/providers';
import { defaultPadding } from '@/libs/theme';
import type { organizationAnalytics_query$key } from '@/queries/__generated__/organizationAnalytics_query.graphql';
import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import { PageHeaderPanel, SettingsSectionCard } from '@skedular/ui';
import { useSearchParams } from 'next/navigation';
import { memo, useEffect, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
import OrganizationAnalyticsSectionNav, { OrganizationAnalyticsSection } from './organization-analytics-section-nav';

type Props = {
  rootDataRelay: organizationAnalytics_query$key;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

const getActiveSection = (value: string | null): OrganizationAnalyticsSection => {
  switch (value) {
    case 'locations':
      return 'locations';
    case 'organization':
    default:
      return 'organization';
  }
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
              name
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  useIntegratedPlatrform();
  const searchParams = useSearchParams();
  const activeSection = getActiveSection(searchParams.get('section'));
  const [locationIds, setLocationIds] = useState<string[]>([]);
  const [stickyTop, setStickyTop] = useState(0);
  const locations = useMemo(() => (rootData.locations ? rootData.locations.edges.map((edge) => edge.node) : []), [rootData.locations]);
  const locationsToDisplay = useMemo(() => (locationIds.length > 0 ? locations.filter((item) => locationIds.includes(item.id)) : locations), [locationIds, locations]);

  useEffect(() => {
    const updateStickyTop = () => {
      setStickyTop(document.querySelector('.app-bar')?.clientHeight ?? 0);
    };

    updateStickyTop();
    window.addEventListener('resize', updateStickyTop);

    return () => {
      window.removeEventListener('resize', updateStickyTop);
    };
  }, []);

  const handlLocationChanged = (id?: string) => {
    setLocationIds(id ? [id] : []);
  };

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 0, sm: 1, md: 2 }, pb: defaultPadding }}>
      <StackColumn
        sx={{
          width: '100%',
          maxWidth: 1120,
          mx: 'auto',
          pt: { xs: 1, sm: 1, md: 2 },
          backgroundColor: 'transparent',
          gap: 2,
        }}
      >
        <PageHeaderPanel
          eyebrow="Analytics"
          title="Organization Analytics"
          description="Review organization-wide and location-level trends in bookings, attendance, and occupancy."
        >
          <StackColumn spacing={0.5}>
            <SmallIconTypography label="Insights & trends" />
            <BodyIconTypography label="This iteration focuses on bringing analytics into the same shell and navigation language as the rest of organization management." />
          </StackColumn>
        </PageHeaderPanel>

        <OrganizationAnalyticsSectionNav activeSection={activeSection} organizationCustomDomain={organizationCustomDomain} stickyTop={stickyTop} />

        {activeSection === 'organization' && (
          <Box
            sx={{
              borderRadius: 4,
              border: 1,
              borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
              bgcolor: (theme) => (theme.palette.mode === 'light' ? 'common.white' : theme.palette.background.paper),
              boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 12px 32px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
              overflow: 'hidden',
            }}
          >
            <SettingsSectionCard title="Organization Insights" description="Track organization-level booking and attendance trends from a single overview.">
              <GridContainer sx={{ paddingTop: defaultPadding }}>
                <Grid>
                  <OrganizationBookingInsightRoot onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} />
                </Grid>
                <Grid>
                  <OrganizationMemberAttendancyInsightRoot onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} />
                </Grid>
              </GridContainer>
            </SettingsSectionCard>
          </Box>
        )}

        {activeSection === 'locations' && (
          <Box
            sx={{
              borderRadius: 4,
              border: 1,
              borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
              bgcolor: (theme) => (theme.palette.mode === 'light' ? 'common.white' : theme.palette.background.paper),
              boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 12px 32px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
              overflow: 'hidden',
            }}
          >
            <SettingsSectionCard title="Location Insights" description="Filter locations and compare booking and desk-occupancy trends for each site.">
              <StackColumn spacing={2}>
                <StackRow sx={{ justifyContent: 'flex-start' }}>
                  <LocationSelector rootDataRelay={rootData} onChange={handlLocationChanged} />
                </StackRow>

                {locationsToDisplay.length > 0 ? (
                  <StackColumn spacing={2}>
                    {locationsToDisplay.map((location) => (
                      <StackColumn spacing={2} key={location.id}>
                        <StackColumn spacing={0.5}>
                          <LeadIconTypography label={location.name} />
                        </StackColumn>

                        <GridContainer>
                          <Grid>
                            <LocationBookingInsightRoot onReloadRequired={onReloadRequired} locationId={location.id} />
                          </Grid>
                          <Grid>
                            <LocationDeskOccupancyInsightRoot onReloadRequired={onReloadRequired} locationId={location.id} />
                          </Grid>
                        </GridContainer>
                      </StackColumn>
                    ))}
                  </StackColumn>
                ) : (
                  <StackColumn sx={{ paddingTop: defaultPadding }}>
                    <SectionIconTypography label="No locations available" />
                    <Divider />
                  </StackColumn>
                )}
              </StackColumn>
            </SettingsSectionCard>
          </Box>
        )}
      </StackColumn>
    </Box>
  );
};

export default memo(OrganizationAnalytics);

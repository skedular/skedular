import { StackRow } from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { Zones } from '@/components/zone';
import { keyboardDebounceTimeout } from '@/libs/utils';
import type { customerFloorPlanView_availableResources_query$key } from '@/queries/__generated__/customerFloorPlanView_availableResources_query.graphql';
import type { customerFloorPlanView_availableResources_refetchableFragment } from '@/queries/__generated__/customerFloorPlanView_availableResources_refetchableFragment.graphql';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import EventAvailableIcon from '@mui/icons-material/EventAvailable';
import EventBusyIcon from '@mui/icons-material/EventBusy';
import FitScreenIcon from '@mui/icons-material/FitScreen';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import PeopleIcon from '@mui/icons-material/People';
import ZoomInIcon from '@mui/icons-material/ZoomIn';
import ZoomOutIcon from '@mui/icons-material/ZoomOut';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import CircularProgress from '@mui/material/CircularProgress';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Paper from '@mui/material/Paper';
import Popover from '@mui/material/Popover';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import dayjs, { Dayjs } from 'dayjs';
import { memo, useCallback, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';
import { useDebounceCallback } from 'usehooks-ts';

type ResourcePosition = {
  readonly id: string;
  readonly x: number;
  readonly y: number;
  readonly width: number;
  readonly height: number;
  readonly shape: string | null | undefined;
  readonly metadata: string | null | undefined;
  readonly resource: {
    readonly id: string;
    readonly name: string;
  };
};

type FloorPlan = {
  readonly id: string;
  readonly name: string;
  readonly floorLevel: number;
  readonly floorName: string | null | undefined;
  readonly imagePath: string;
  readonly thumbnailPath: string | null | undefined;
  readonly width: number;
  readonly height: number;
  readonly isActive: boolean;
  readonly resourcePositions: ReadonlyArray<ResourcePosition>;
};

type ResourceData = {
  id: string;
  name: string;
  inactive: boolean;
  color: string | null | undefined;
  capacity: number;
  requireBookingApproval: boolean;
  resourceType: {
    uniqueId: string;
    name: string | null | undefined;
    tagType: string | null | undefined;
    color: string | null | undefined;
  };
  customTags?: ReadonlyArray<{
    uniqueId: string;
    name: string | null | undefined;
    color: string | null | undefined;
  }>;
  zones?: ReadonlyArray<{
    uniqueId: string;
    name: string | null | undefined;
    color: string | null | undefined;
  }>;
};

type Props = {
  organizationId: string;
  locationId: string;
  locationName: string;
  floorPlans: ReadonlyArray<FloorPlan>;
  resources: ReadonlyArray<ResourceData>;
  rootDataAvailableResourcesRelay: customerFloorPlanView_availableResources_query$key;
  onBookResource?: (resourceId: string) => void;
  platform?: string;
};

const CustomerFloorPlanView = ({
  organizationId,
  locationId,
  locationName: _locationName,
  floorPlans,
  resources,
  rootDataAvailableResourcesRelay,
  onBookResource,
  platform = 'web',
}: Props) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const [imageScale, setImageScale] = useState(1);
  const [hoveredResourceId, setHoveredResourceId] = useState<string | null>(null);
  const [selectedResourceId, setSelectedResourceId] = useState<string | null>(null);
  const [imageLoaded, setImageLoaded] = useState(false);
  const [isPending, startTransition] = useTransition();
  const [isMounted, setIsMounted] = useState(false);
  const [tabsReady, setTabsReady] = useState(false);

  const [selectedDateTime, setSelectedDateTime] = useState<Dayjs>(dayjs());
  const [duration, setDuration] = useState(1); // hours

  const [popoverAnchorEl, setPopoverAnchorEl] = useState<HTMLElement | null>(null);

  const [rootDataAvailableResources, refetchAvailableResources] = useRefetchableFragment<
    customerFloorPlanView_availableResources_refetchableFragment,
    customerFloorPlanView_availableResources_query$key
  >(
    graphql`
      fragment customerFloorPlanView_availableResources_query on Query @refetchable(queryName: "customerFloorPlanView_availableResources_refetchableFragment") {
        availableResources(where: { organizationId: $organizationId, locationId: $locationId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources }) {
          uniqueId
          name
          customTags {
            uniqueId
            name
            color
          }
          zones {
            uniqueId
            name
            color
          }
        }
      }
    `,
    rootDataAvailableResourcesRelay,
  );

  const availableResourceIds = new Set((rootDataAvailableResources?.availableResources || []).map((r) => r.uniqueId));

  const activeFloorPlans = useMemo(() => (floorPlans || []).filter((fp) => fp.isActive), [floorPlans]);

  const [selectedFloorPlanId, setSelectedFloorPlanId] = useState<string>('');

  useEffect(() => {
    setIsMounted(true);
    const timer = setTimeout(() => {
      setTabsReady(true);
    }, 100);
    return () => {
      setIsMounted(false);
      setTabsReady(false);
      clearTimeout(timer);
    };
  }, []);

  useEffect(() => {
    if (isMounted && activeFloorPlans.length > 0) {
      if (!selectedFloorPlanId || !activeFloorPlans.find((fp) => fp.id === selectedFloorPlanId)) {
        setSelectedFloorPlanId(activeFloorPlans[0].id);
      }
    } else if (isMounted) {
      setSelectedFloorPlanId('');
    }
  }, [activeFloorPlans, isMounted, selectedFloorPlanId]);

  const currentFloorPlan = activeFloorPlans.find((fp) => fp.id === selectedFloorPlanId);

  const getResourceById = (resourceId: string): ResourceData | undefined => resources.find((r) => r.id === resourceId);

  const isResourceAvailable = (resourceId: string): boolean => {
    const resource = getResourceById(resourceId);
    if (!resource || resource.inactive) return false;
    return availableResourceIds.has(resourceId);
  };

  const handleRefetchAvailableResources = useDebounceCallback(
    useCallback(
      (dateTime: Dayjs, durationHours: number) => {
        const from = dateTime;
        const until = dateTime.add(durationHours, 'hour');

        startTransition(() => {
          refetchAvailableResources(
            {
              dateFromToGetAvailableResources: from.utc().toISOString(),
              dateUntilToGetAvailableResources: until.utc().toISOString(),
            },
            {
              fetchPolicy: 'store-and-network',
            },
          );
        });
      },
      [refetchAvailableResources],
    ),
    keyboardDebounceTimeout,
  );

  useEffect(() => {
    handleRefetchAvailableResources(selectedDateTime, duration);
  }, [selectedDateTime, duration, handleRefetchAvailableResources]);

  const handleResourceClick = (event: React.MouseEvent<HTMLElement>, resourceId: string) => {
    if (!isResourceAvailable(resourceId)) return;

    setSelectedResourceId(resourceId);
    setPopoverAnchorEl(event.currentTarget);
  };

  const handleClosePopover = () => {
    setPopoverAnchorEl(null);
    setSelectedResourceId(null);
  };

  const handleBookResource = () => {
    if (!selectedResourceId) return;

    if (onBookResource) {
      onBookResource(selectedResourceId);
    }

    handleClosePopover();
  };

  const handleZoomIn = () => {
    setImageScale((prev) => Math.min(prev * 1.2, 3));
  };

  const handleZoomOut = () => {
    setImageScale((prev) => Math.max(prev / 1.2, 0.5));
  };

  const handleFitToScreen = useCallback(() => {
    if (!containerRef.current || !currentFloorPlan || !imageLoaded) return;

    const containerWidth = containerRef.current.offsetWidth - 40; // Padding
    const containerHeight = containerRef.current.offsetHeight - 40;

    const scaleX = containerWidth / currentFloorPlan.width;
    const scaleY = containerHeight / currentFloorPlan.height;
    const scale = Math.min(scaleX, scaleY);

    setImageScale(scale);
  }, [currentFloorPlan, imageLoaded]);

  useEffect(() => {
    if (imageLoaded) {
      handleFitToScreen();
    }
  }, [imageLoaded, handleFitToScreen, currentFloorPlan]);

  const getResourceColor = (resource: ResourceData, isAvailable: boolean) => {
    if (!isAvailable) return '#e57373';
    if (resource.resourceType.color) return resource.resourceType.color;
    return resource.color || '#4caf50';
  };

  if (activeFloorPlans.length === 0) {
    return (
      <Card variant="outlined">
        <CardContent>
          <Typography variant="body1" color="text.secondary" align="center">
            No floor plans available for this location.
          </Typography>
        </CardContent>
      </Card>
    );
  }

  if (!currentFloorPlan) {
    return (
      <Card variant="outlined">
        <CardContent>
          <Typography variant="body1" color="text.secondary" align="center">
            Loading floor plan...
          </Typography>
        </CardContent>
      </Card>
    );
  }

  const selectedResource = selectedResourceId ? getResourceById(selectedResourceId) : null;

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <Card variant="outlined" sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
        <CardContent sx={{ p: 0, height: '100%', display: 'flex', flexDirection: 'column' }}>
          <Stack sx={{ height: '100%' }}>
            <Box sx={{ p: 2, borderBottom: 1, borderColor: 'divider', bgcolor: 'background.default' }}>
              <Stack direction="row" spacing={2} alignItems="center">
                <DateTimePicker
                  label="Select Date & Time"
                  value={selectedDateTime}
                  onChange={(newValue) => {
                    if (newValue) setSelectedDateTime(newValue);
                  }}
                  minDateTime={dayjs()}
                  slotProps={{
                    textField: {
                      size: 'small',
                      sx: { minWidth: 250 },
                    },
                  }}
                />
                <Typography variant="body2" color="text.secondary">
                  Duration: {duration} hour{duration > 1 ? 's' : ''}
                </Typography>
                {isPending && <CircularProgress size={20} />}
              </Stack>
            </Box>

            {activeFloorPlans.length > 1 && (
              <Box
                sx={{
                  borderBottom: 1,
                  borderColor: 'divider',
                  display: 'flex',
                  overflowX: 'auto',
                  bgcolor: 'background.paper',
                }}
              >
                {activeFloorPlans.map((fp) => (
                  <Button
                    key={fp.id}
                    onClick={() => {
                      setSelectedFloorPlanId(fp.id);
                      setImageLoaded(false);
                    }}
                    sx={{
                      minWidth: 'auto',
                      px: 2,
                      py: 1.5,
                      borderRadius: 0,
                      borderBottom: selectedFloorPlanId === fp.id ? 2 : 0,
                      borderBottomColor: selectedFloorPlanId === fp.id ? 'primary.main' : 'transparent',
                      color: selectedFloorPlanId === fp.id ? 'primary.main' : 'text.secondary',
                      fontWeight: selectedFloorPlanId === fp.id ? 600 : 400,
                      '&:hover': {
                        bgcolor: 'action.hover',
                      },
                    }}
                  >
                    {fp.name} {fp.floorName ? `- ${fp.floorName}` : ''}
                  </Button>
                ))}
              </Box>
            )}

            <Box sx={{ p: 2, borderBottom: 1, borderColor: 'divider', bgcolor: 'background.default' }}>
              <Stack direction="row" spacing={2} alignItems="center" justifyContent="space-between">
                <Stack direction="row" spacing={2} alignItems="center">
                  <Typography variant="body2" fontWeight="bold">
                    Legend:
                  </Typography>
                  <Chip size="small" icon={<EventAvailableIcon />} label="Available" sx={{ bgcolor: '#4caf50', color: 'white' }} />
                  <Chip size="small" icon={<EventBusyIcon />} label="Not Available" sx={{ bgcolor: '#e57373', color: 'white' }} />
                  <Typography variant="caption" color="text.secondary">
                    Click on available resources to book
                  </Typography>
                </Stack>
                <Stack direction="row" spacing={1}>
                  <IconButton size="small" onClick={handleZoomOut} title="Zoom out">
                    <ZoomOutIcon />
                  </IconButton>
                  <IconButton size="small" onClick={handleFitToScreen} title="Fit to screen">
                    <FitScreenIcon />
                  </IconButton>
                  <IconButton size="small" onClick={handleZoomIn} title="Zoom in">
                    <ZoomInIcon />
                  </IconButton>
                </Stack>
              </Stack>
            </Box>

            {currentFloorPlan && (
              <Box
                ref={containerRef}
                sx={{
                  flex: 1,
                  overflow: 'hidden',
                  backgroundColor: '#f5f5f5',
                  position: 'relative',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  p: 2,
                }}
              >
                <Box
                  sx={{
                    position: 'relative',
                    width: currentFloorPlan.width * imageScale,
                    height: currentFloorPlan.height * imageScale,
                    margin: 'auto',
                  }}
                >
                  {currentFloorPlan.imagePath ? (
                    <img
                      src={currentFloorPlan.imagePath}
                      alt={currentFloorPlan.name}
                      style={{
                        width: '100%',
                        height: '100%',
                        display: 'block',
                      }}
                      onLoad={() => setImageLoaded(true)}
                      onError={() => {
                        console.error('Failed to load floor plan image:', currentFloorPlan.imagePath);
                        setImageLoaded(true); // Set loaded to true even on error to prevent infinite loading
                      }}
                    />
                  ) : (
                    <Box
                      sx={{
                        width: '100%',
                        height: '100%',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        backgroundColor: '#f0f0f0',
                      }}
                    >
                      <Typography variant="body1" color="text.secondary">
                        No floor plan image available
                      </Typography>
                    </Box>
                  )}

                  {currentFloorPlan.resourcePositions &&
                    currentFloorPlan.resourcePositions.map((position) => {
                      const resource = getResourceById(position.resource.id);
                      if (!resource) return null;

                      const isAvailable = isResourceAvailable(resource.id);
                      const isHovered = hoveredResourceId === resource.id;

                      return (
                        <Paper
                          key={position.id}
                          elevation={isHovered ? 6 : 2}
                          sx={{
                            position: 'absolute',
                            left: position.x * imageScale,
                            top: position.y * imageScale,
                            width: position.width * imageScale,
                            height: position.height * imageScale,
                            backgroundColor: getResourceColor(resource, isAvailable),
                            opacity: isAvailable ? 0.85 : 0.6,
                            cursor: isAvailable ? 'pointer' : 'not-allowed',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            transition: 'all 0.2s ease',
                            transform: isHovered ? 'scale(1.08)' : 'scale(1)',
                            border: `2px solid ${isAvailable ? 'rgba(255,255,255,0.8)' : 'rgba(0,0,0,0.3)'}`,
                            borderRadius: position.shape === 'circle' ? '50%' : '4px',
                            overflow: 'visible',
                            '&:hover': {
                              opacity: isAvailable ? 1 : 0.6,
                              zIndex: 10,
                            },
                          }}
                          onClick={(e) => handleResourceClick(e, resource.id)}
                          onMouseEnter={() => setHoveredResourceId(resource.id)}
                          onMouseLeave={() => setHoveredResourceId(null)}
                        >
                          <Stack spacing={0} alignItems="center" sx={{ p: 0.5, position: 'relative' }}>
                            <Typography
                              variant="caption"
                              sx={{
                                color: 'white',
                                fontWeight: 'bold',
                                textAlign: 'center',
                                fontSize: imageScale > 0.8 ? '0.75rem' : '0.65rem',
                                lineHeight: 1.2,
                                textShadow: '0 1px 2px rgba(0,0,0,0.5)',
                              }}
                            >
                              {resource.name}
                            </Typography>
                            {imageScale > 0.7 && (
                              <Box
                                sx={{
                                  position: 'absolute',
                                  top: 2,
                                  right: 2,
                                  width: 12,
                                  height: 12,
                                  borderRadius: '50%',
                                  backgroundColor: isAvailable ? '#4caf50' : '#e57373',
                                  border: '2px solid white',
                                  boxShadow: '0 2px 4px rgba(0,0,0,0.3)',
                                }}
                              />
                            )}
                          </Stack>
                        </Paper>
                      );
                    })}
                </Box>
              </Box>
            )}
          </Stack>
        </CardContent>
      </Card>

      <Popover
        open={Boolean(popoverAnchorEl)}
        anchorEl={popoverAnchorEl}
        onClose={handleClosePopover}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'center',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'center',
        }}
      >
        {selectedResource && (
          <Box sx={{ p: 2, minWidth: 300 }}>
            <Stack spacing={2}>
              <Typography variant="h6" fontWeight="bold">
                {selectedResource.name}
              </Typography>

              <Divider />

              <Stack spacing={1}>
                <StackRow spacing={1}>
                  <LocationOnIcon fontSize="small" color="action" />
                  <Typography variant="body2">{selectedResource.resourceType.name}</Typography>
                </StackRow>

                <StackRow spacing={1}>
                  <PeopleIcon fontSize="small" color="action" />
                  <Typography variant="body2">Capacity: {selectedResource.capacity} people</Typography>
                </StackRow>

                <StackRow spacing={1}>
                  <CalendarTodayIcon fontSize="small" color="action" />
                  <Typography variant="body2">{selectedDateTime.format('MMM D, YYYY')}</Typography>
                </StackRow>

                <StackRow spacing={1}>
                  <AccessTimeIcon fontSize="small" color="action" />
                  <Typography variant="body2">
                    {selectedDateTime.format('h:mm A')} - {selectedDateTime.add(duration, 'hour').format('h:mm A')}
                  </Typography>
                </StackRow>

                {selectedResource.requireBookingApproval && (
                  <Typography variant="caption" color="text.secondary" sx={{ fontStyle: 'italic' }}>
                    * This resource requires booking approval
                  </Typography>
                )}
              </Stack>

              {selectedResource.customTags && selectedResource.customTags.length > 0 && (
                <>
                  <Divider />
                  <Box>
                    <Typography variant="caption" color="text.secondary" gutterBottom>
                      Features
                    </Typography>
                    <CustomTags customTags={selectedResource.customTags.map((tag) => ({ id: tag.uniqueId, name: tag.name, color: tag.color }))} />
                  </Box>
                </>
              )}

              {selectedResource.zones && selectedResource.zones.length > 0 && (
                <>
                  <Divider />
                  <Box>
                    <Typography variant="caption" color="text.secondary" gutterBottom>
                      Zones
                    </Typography>
                    <Zones zones={selectedResource.zones.map((zone) => ({ id: zone.uniqueId, name: zone.name, color: zone.color }))} />
                  </Box>
                </>
              )}

              <Divider />

              <Stack direction="row" spacing={1} justifyContent="flex-end">
                <Button variant="outlined" size="small" onClick={handleClosePopover}>
                  Cancel
                </Button>
                <Button variant="contained" size="small" startIcon={<EventAvailableIcon />} onClick={handleBookResource} color="primary">
                  Book Resource
                </Button>
              </Stack>
            </Stack>
          </Box>
        )}
      </Popover>
    </LocalizationProvider>
  );
};

export { CustomerFloorPlanView };
export default memo(CustomerFloorPlanView);

import { AppBarWithStackColumn, BodyIconTypography, SectionIconTypography, StackColumn, StackRow, GridContainer, SmallIconTypography } from '@/components/commons';
import { Resource } from '@/components/resource';
import { ResourceType } from '@/components/resourceType';
import { Search } from '@/components/search';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { floorPlanEditor_updateResourcePositionsMutation } from '@/queries/__generated__/floorPlanEditor_updateResourcePositionsMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Dialog from '@mui/material/Dialog';
import Grid from '@mui/material/Grid';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import { nanoid } from 'nanoid';
import { memo, useCallback, useEffect, useRef, useState } from 'react';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { NotificationContent } from '@/components/notification';

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
  resourceType: {
    uniqueId: string;
    name: string | null | undefined;
    color: string | null | undefined;
  };
};

type Props = {
  open: boolean;
  onClose: () => void;
  floorPlan: FloorPlan;
  locationId: string;
  organizationId: string;
  resources: ReadonlyArray<ResourceData>;
  onReloadRequired: () => void;
};

type PositionedResource = {
  id: string;
  resourceId: string;
  x: number;
  y: number;
  width: number;
  height: number;
  shape: string;
  metadata: string | null;
};

const DEFAULT_RESOURCE_SIZE = 60;
const GRID_SNAP = 10;

const FloorPlanEditor = ({ open, onClose, floorPlan, resources, onReloadRequired }: Props) => {
  const [searchText, setSearchText] = useState('');
  const [selectedResourceId, setSelectedResourceId] = useState<string | null>(null);
  const [positionedResources, setPositionedResources] = useState<Map<string, PositionedResource>>(new Map());
  const [removedResourceIds, setRemovedResourceIds] = useState<Set<string>>(new Set());
  const [isDragging, setIsDragging] = useState(false);
  const [dragOffset, setDragOffset] = useState({ x: 0, y: 0 });
  const [imageScale, setImageScale] = useState(1);
  const imageRef = useRef<HTMLImageElement>(null);
  const containerRef = useRef<HTMLDivElement>(null);

  const [commitUpdateResourcePositions] = useMutation<floorPlanEditor_updateResourcePositionsMutation>(graphql`
    mutation floorPlanEditor_updateResourcePositionsMutation($input: UpdateResourcePositionsInput!) {
      updateResourcePositions(input: $input) {
        resourcePositions {
          id
          x
          y
          width
          height
          shape
          metadata
          resource {
            id
            name
          }
        }
      }
    }
  `);

  const [commitRemoveResourcePosition] = useMutation(graphql`
    mutation floorPlanEditor_removeResourcePositionMutation($input: RemoveResourcePositionInput!) {
      removeResourcePosition(input: $input) {
        clientMutationId
        success
      }
    }
  `);

  useEffect(() => {
    const initialPositions = new Map<string, PositionedResource>();
    floorPlan.resourcePositions.forEach((pos) => {
      initialPositions.set(pos.resource.id, {
        id: pos.id,
        resourceId: pos.resource.id,
        x: pos.x,
        y: pos.y,
        width: pos.width,
        height: pos.height,
        shape: pos.shape || 'rectangle',
        metadata: pos.metadata || null,
      });
    });
    setPositionedResources(initialPositions);
  }, [floorPlan]);

  useEffect(() => {
    if (imageRef.current && containerRef.current) {
      const containerWidth = containerRef.current.clientWidth - 48; // padding
      const scale = containerWidth / floorPlan.width;
      setImageScale(Math.min(scale, 1));
    }
  }, [floorPlan.width, open]);

  const handleResourceClick = (resource: ResourceData) => {
    if (positionedResources.has(resource.id)) {
      setSelectedResourceId(resource.id);
    } else {
      const newPosition: PositionedResource = {
        id: `temp_${nanoid()}`, // temp_ for new positions
        resourceId: resource.id,
        x: 50,
        y: 50,
        width: DEFAULT_RESOURCE_SIZE,
        height: DEFAULT_RESOURCE_SIZE,
        shape: 'rectangle',
        metadata: null,
      };
      setPositionedResources(new Map(positionedResources).set(resource.id, newPosition));
      setSelectedResourceId(resource.id);
    }
  };

  const handleMouseDown = (e: React.MouseEvent, resourceId: string) => {
    e.preventDefault();
    setSelectedResourceId(resourceId);
    setIsDragging(true);

    const resource = positionedResources.get(resourceId);
    if (resource && containerRef.current) {
      const rect = containerRef.current.getBoundingClientRect();
      setDragOffset({
        x: (e.clientX - rect.left) / imageScale - resource.x,
        y: (e.clientY - rect.top) / imageScale - resource.y,
      });
    }
  };

  const handleMouseMove = useCallback(
    (e: MouseEvent) => {
      if (!isDragging || !selectedResourceId || !containerRef.current) return;

      const rect = containerRef.current.getBoundingClientRect();
      const x = Math.max(0, Math.round(((e.clientX - rect.left) / imageScale - dragOffset.x) / GRID_SNAP) * GRID_SNAP);
      const y = Math.max(0, Math.round(((e.clientY - rect.top) / imageScale - dragOffset.y) / GRID_SNAP) * GRID_SNAP);

      setPositionedResources((prev) => {
        const newMap = new Map(prev);
        const resource = prev.get(selectedResourceId);
        if (resource) {
          newMap.set(selectedResourceId, { ...resource, x, y });
        }
        return newMap;
      });
    },
    [isDragging, selectedResourceId, imageScale, dragOffset],
  );

  const handleMouseUp = useCallback(() => {
    setIsDragging(false);
  }, []);

  useEffect(() => {
    if (isDragging) {
      document.addEventListener('mousemove', handleMouseMove);
      document.addEventListener('mouseup', handleMouseUp);
      return () => {
        document.removeEventListener('mousemove', handleMouseMove);
        document.removeEventListener('mouseup', handleMouseUp);
      };
    }
  }, [isDragging, handleMouseMove, handleMouseUp]);

  const handleRemoveResource = (resourceId: string) => {
    const positionedResource = positionedResources.get(resourceId);
    if (positionedResource && positionedResource.id && !positionedResource.id.startsWith('temp_')) {
      setRemovedResourceIds((prev) => new Set(prev).add(resourceId));
    }

    setPositionedResources((prev) => {
      const newMap = new Map(prev);
      newMap.delete(resourceId);
      return newMap;
    });
    if (selectedResourceId === resourceId) {
      setSelectedResourceId(null);
    }
  };

  const handleSave = async () => {
    try {
      const removedIds = Array.from(removedResourceIds);
      for (const resourceId of removedIds) {
        await new Promise<void>((resolve, reject) => {
          commitRemoveResourcePosition({
            variables: {
              input: {
                clientMutationId: nanoid(),
                resourceId,
              },
            },
            onCompleted: (_, errors) => {
              if (errors && errors.length > 0) {
                reject(new Error(joinErrors(errors)));
              } else {
                resolve();
              }
            },
            onError: (error) => {
              reject(error);
            },
          });
        });
      }

      const positions = Array.from(positionedResources.values()).map((pos) => ({
        id: pos.id,
        resourceId: pos.resourceId,
        x: pos.x,
        y: pos.y,
        width: pos.width,
        height: pos.height,
        shape: pos.shape,
        metadata: pos.metadata,
      }));

      commitUpdateResourcePositions({
        variables: {
          input: {
            clientMutationId: nanoid(),
            floorPlanId: floorPlan.id,
            positions,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            toast.error(<NotificationContent content={`Failed to save resource positions. Error: ${joinErrors(errors)}`} />);
            return;
          }

          toast.success(<NotificationContent content="Resource positions saved successfully" />);
          onReloadRequired();
        },
        onError: (error) => {
          toast.error(<NotificationContent content={`Failed to save resource positions. Error: ${error.message}`} />);
        },
      });
    } catch (error) {
      toast.error(<NotificationContent content={`Failed to remove resource position. Error: ${error instanceof Error ? error.message : 'Unknown error'}`} />);
    }
  };

  const filteredResources = resources.filter((r) => r.name.toLowerCase().includes(searchText.toLowerCase()));

  const getResourceById = (resourceId: string): ResourceData | undefined => resources.find((r) => r.id === resourceId);

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xl" fullWidth>
      <AppBarWithStackColumn onClose={onClose} label={`Edit Floor Plan - ${floorPlan.name}`}>
        <Box sx={{ display: 'flex', height: 'calc(100vh - 100px)' }}>
          {/* Left sidebar with resources */}
          <Box sx={{ width: 300, borderRight: 1, borderColor: 'divider', p: 2 }}>
            <StackColumn spacing={2}>
              <SectionIconTypography label="Resources" />
              <Search size="small" placeholder="Search resources" defaultValue={searchText} onChange={setSearchText} />

              <List>
                {filteredResources.map((resource: ResourceData) => {
                  const isPositioned = positionedResources.has(resource.id);
                  return (
                    <ListItem key={resource.id} disablePadding>
                      <ListItemButton onClick={() => handleResourceClick(resource)} selected={selectedResourceId === resource.id} disabled={resource.inactive}>
                        <StackRow spacing={1} sx={{ width: '100%', alignItems: 'center' }}>
                          <Resource resource={resource as any} />
                          {isPositioned && <Chip label="Placed" size="small" color="success" component="div" clickable={false} onClick={(e) => e.stopPropagation()} />}
                          {resource.inactive && <Chip label="Inactive" size="small" color="warning" component="div" clickable={false} onClick={(e) => e.stopPropagation()} />}
                        </StackRow>
                      </ListItemButton>
                    </ListItem>
                  );
                })}
              </List>
            </StackColumn>
          </Box>

          {/* Main floor plan area */}
          <Box sx={{ flex: 1, p: 3, overflow: 'auto' }}>
            <StackColumn spacing={2}>
              <GridContainer sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
                <Grid>
                  <StackColumn spacing={1}>
                    <BodyIconTypography label="Drag resources from the left panel to position them on the floor plan" />
                    <SmallIconTypography
                      label="Note: A resource can only be on one floor plan. Moving a resource here will remove it from other floors."
                      sx={{ color: 'text.secondary', fontStyle: 'italic' }}
                    />
                    {removedResourceIds.size > 0 && <SmallIconTypography label={`${removedResourceIds.size} resource(s) will be removed`} sx={{ color: 'error.main' }} />}
                  </StackColumn>
                </Grid>
                <Grid>
                  <Button variant="contained" onClick={handleSave} sx={defaultButtonStyle}>
                    Save Positions
                  </Button>
                </Grid>
              </GridContainer>

              <Box
                ref={containerRef}
                sx={{
                  position: 'relative',
                  display: 'inline-block',
                  cursor: isDragging ? 'grabbing' : 'default',
                  userSelect: 'none',
                }}
              >
                <img
                  ref={imageRef}
                  src={floorPlan.imagePath}
                  alt={floorPlan.name}
                  style={{
                    width: floorPlan.width * imageScale,
                    height: floorPlan.height * imageScale,
                    display: 'block',
                  }}
                  draggable={false}
                />

                {/* Render positioned resources */}
                {Array.from(positionedResources.entries()).map(([resourceId, position]) => {
                  const resource = getResourceById(resourceId);
                  if (!resource) return null;

                  return (
                    <Box
                      key={resourceId}
                      onMouseDown={(e) => handleMouseDown(e, resourceId)}
                      sx={{
                        position: 'absolute',
                        left: position.x * imageScale,
                        top: position.y * imageScale,
                        width: position.width * imageScale,
                        height: position.height * imageScale,
                        backgroundColor: resource.resourceType.color || '#ccc',
                        opacity: resource.inactive ? 0.5 : 0.8,
                        border: selectedResourceId === resourceId ? '2px solid #1976d2' : '1px solid #666',
                        borderRadius: position.shape === 'circle' ? '50%' : 1,
                        cursor: 'grab',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        color: 'white',
                        fontSize: 12 * imageScale,
                        fontWeight: 'bold',
                        textAlign: 'center',
                        padding: '2px',
                        boxSizing: 'border-box',
                        '&:hover': {
                          border: '2px solid #1976d2',
                        },
                      }}
                    >
                      {resource.name}
                    </Box>
                  );
                })}
              </Box>

              {/* Selected resource controls */}
              {selectedResourceId && getResourceById(selectedResourceId) && (
                <Card>
                  <CardContent>
                    <StackRow spacing={2} sx={{ alignItems: 'center' }}>
                      <BodyIconTypography label="Selected:" />
                      <Resource resource={getResourceById(selectedResourceId)!} />
                      <Button size="small" color="error" onClick={() => handleRemoveResource(selectedResourceId)}>
                        Remove from Floor Plan
                      </Button>
                    </StackRow>
                  </CardContent>
                </Card>
              )}
            </StackColumn>
          </Box>
        </Box>
      </AppBarWithStackColumn>
    </Dialog>
  );
};

export { FloorPlanEditor };
export default memo(FloorPlanEditor);

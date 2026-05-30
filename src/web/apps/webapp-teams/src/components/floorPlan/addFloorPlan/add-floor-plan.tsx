'use client';

import type { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import type { addFloorPlan_addFloorPlanMutation } from '@/queries/__generated__/addFloorPlan_addFloorPlanMutation.graphql';
import type { addFloorPlan_resources_query$key } from '@/queries/__generated__/addFloorPlan_resources_query.graphql';
import type { addFloorPlan_resources_refetchableFragment } from '@/queries/__generated__/addFloorPlan_resources_refetchableFragment.graphql';
import type { addFloorPlan_rootQuery } from '@/queries/__generated__/addFloorPlan_rootQuery.graphql';
import DeskIcon from '@mui/icons-material/Desk';
import LocalParkingIcon from '@mui/icons-material/LocalParking';
import MeetingRoomIcon from '@mui/icons-material/MeetingRoom';
import NotListedLocationIcon from '@mui/icons-material/NotListedLocation';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Checkbox from '@mui/material/Checkbox';
import CircularProgress from '@mui/material/CircularProgress';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemAvatar from '@mui/material/ListItemAvatar';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText';
import { errorNotificationOptions, getRelayErrorMessage, NotificationContent, PaletteModeContext, RelayError, toRootError } from '@skedular/shared';
import { BodyIconTypography, EditorActionBar, FormFieldLabel, FormStackColumn, PageHeaderPanel, SettingsSectionCard, StackColumn } from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

const RootQuery = graphql`
  query addFloorPlan_rootQuery($locationId: String!, $floorPlanId: String!, $resourcesSortingValues: [ResourceOrderInput!]) {
    deskResourceType
    roomResourceType
    parkingResourceType
    ...addFloorPlan_resources_query
  }
`;

const ResourcesFragment = graphql`
  fragment addFloorPlan_resources_query on Query
  @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
  @refetchable(queryName: "addFloorPlan_resources_refetchableFragment") {
    location(id: $locationId) {
      resources(first: $count, after: $cursor, where: { floorPlanId: $floorPlanId }, orderBy: $resourcesSortingValues) @connection(key: "addFloorPlanResourcesQuery_resources") {
        edges {
          node {
            id
            name
            inactive
            color
            capacity
            customTags {
              id
              name
              color
            }
            zones {
              id
              name
              color
            }
            productTags {
              id
              name
              color
            }
            resourceType {
              id
              name
              color
              type
            }
          }
        }
      }
    }
  }
`;

const AddMutation = graphql`
  mutation addFloorPlan_addFloorPlanMutation($input: AddFloorPlanInput!) @raw_response_type {
    addFloorPlan(input: $input) {
      floorPlan {
        id
        name
        image {
          original {
            url
            height
            width
          }
          thumbnail {
            url
            height
            width
          }
        }
        resourcePositions {
          x
          y
          resource {
            id
          }
        }
      }
    }
  }
`;

type ResourceNode = {
  id: string;
  name: string;
  inactive: boolean;
  color: string | null;
  capacity: number | null;
  customTags: { id: string; name: string; color: string }[];
  zones: { id: string; name: string; color: string }[];
  productTags: { id: string; name: string; color: string }[];
  resourceType: { id: string; name: string; color: string; type: string | null };
};

type FloorPlanSaveInput = {
  id: string;
  name: string;
  locationId: string;
  image: {
    original: { url: string; height: number; width: number };
    thumbnail: { url: string; height: number; width: number };
  };
  resourcePositions: { resourceId: string; x: number; y: number }[];
};

type FloorPlanDetails = { name: string };

const floorPlanSchema = object({
  name: string().min(3, 'Floor plan name must be at least three characters long.').required('Floor plan name is required'),
});

type InnerProps = {
  queryReference: PreloadedQuery<addFloorPlan_rootQuery>;
  locationId: string;
  onAdded: (id: string) => void;
  onCancel: () => void;
  onReloadRequired: () => void;
  addLabel?: string;
  showDismiss: boolean;
};

const AddFloorPlanInner = ({ queryReference, locationId, onAdded, onCancel, onReloadRequired, addLabel, showDismiss }: InnerProps) => {
  const rootData = usePreloadedQuery<addFloorPlan_rootQuery>(RootQuery, queryReference);
  const [rootDataResources] = useRefetchableFragment<addFloorPlan_resources_refetchableFragment, addFloorPlan_resources_query$key>(ResourcesFragment, rootData);
  const [commitAddFloorPlan] = useMutation<addFloorPlan_addFloorPlanMutation>(AddMutation);
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateFloorPlanDetails = makeValidate(floorPlanSchema);
  const requiredFields = makeRequired(floorPlanSchema);
  const resources: ResourceNode[] = (rootDataResources.location?.resources?.edges?.map(({ node }) => node) ?? []) as ResourceNode[];
  const [image, setImage] = useState<FileUploadResponse>();
  const [resourcePositions, setResourcePositions] = useState<Map<string, { x: number; y: number }>>(new Map());
  const [draggingResourceId, setDraggingResourceId] = useState<string | null>(null);
  const [offset, setOffset] = useState({ x: 0, y: 0 });

  const handleCloseClick = () => {
    onCancel();
    onReloadRequired();
  };

  const handleFloorPlanAddClick = ({ name }: FloorPlanDetails) => {
    if (!image?.original.url || !image.original.height || !image.original.width || !image.thumbnail?.url || !image.thumbnail.height || !image.thumbnail.width) {
      themedToast(<NotificationContent content="Floor plan image is required." />, errorNotificationOptions);
      return;
    }

    const id = uuid();
    const input: FloorPlanSaveInput = {
      id,
      name,
      locationId,
      image: {
        original: { url: image.original.url, height: image.original.height, width: image.original.width },
        thumbnail: { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width },
      },
      resourcePositions: [...resourcePositions.entries()].map(([resourceId, { x, y }]) => ({ resourceId, x, y })),
    };

    commitAddFloorPlan({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: input.id,
          locationId: input.locationId,
          name: input.name,
          image: input.image,
          resourcePositions: input.resourcePositions,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to add new floor plan '${input.name}'. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);
          return;
        }

        onAdded(id);
        onReloadRequired();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to add new floor plan '${input.name}'. Error: ${error.message}.`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        addFloorPlan: {
          floorPlan: {
            id: input.id,
            name: input.name,
            image: input.image,
            resourcePositions: input.resourcePositions.map(({ resourceId, x, y }) => ({ id: '', resource: { id: resourceId }, x, y })),
          },
        },
      },
    });
  };

  const handleMouseDown = (event: React.MouseEvent, resourceId: string) => {
    event.preventDefault();
    const boundingRect = (event.target as HTMLElement).getBoundingClientRect();
    setOffset({ x: event.clientX - boundingRect.left, y: event.clientY - boundingRect.top });
    setDraggingResourceId(resourceId);
  };

  const handleMouseMove = (event: React.MouseEvent<HTMLDivElement>) => {
    if (!draggingResourceId || !image?.original?.width || !image?.original?.height) return;
    const containerRect = event.currentTarget.getBoundingClientRect();
    const displayX = event.clientX - containerRect.left - offset.x;
    const displayY = event.clientY - containerRect.top - offset.y;
    setResourcePositions(
      new Map(resourcePositions).set(draggingResourceId, {
        x: Math.round((displayX / containerRect.width) * image.original.width),
        y: Math.round((displayY / containerRect.height) * image.original.height),
      }),
    );
  };

  const handleMouseUp = () => setDraggingResourceId(null);

  const handleToggleResourcePosition = (id: string) => {
    if (resourcePositions.has(id)) {
      resourcePositions.delete(id);
      setResourcePositions(new Map(resourcePositions));
    } else {
      setResourcePositions(new Map(resourcePositions).set(id, { x: 50, y: 50 }));
    }
  };

  const resourceIcon = (type: string, color: string) => {
    if (type === rootData.deskResourceType) return <DeskIcon sx={{ color }} />;
    if (type === rootData.roomResourceType) return <MeetingRoomIcon sx={{ color }} />;
    if (type === rootData.parkingResourceType) return <LocalParkingIcon sx={{ color }} />;
    return <NotListedLocationIcon sx={{ color }} />;
  };

  return (
    <Box sx={{ px: { xs: 2, md: 3 }, py: 3 }}>
      <Box sx={{ maxWidth: 1320, mx: 'auto' }}>
        <Form
          onSubmit={handleFloorPlanAddClick}
          initialValues={{ name: '' }}
          validate={validateFloorPlanDetails}
          render={({ handleSubmit }) => (
            <FormStackColumn onSubmit={handleSubmit}>
              <StackColumn>
                <PageHeaderPanel title="Add Floor Plan" />

                <SettingsSectionCard title="Details">
                  <FormFieldLabel label="Name">
                    <TextField name="name" required={requiredFields.name} />
                  </FormFieldLabel>
                </SettingsSectionCard>

                <SettingsSectionCard title="Floor Plan Layout">
                  <ImageFileUploaderWithCropper onUploadCompleted={setImage} />
                  <Box
                    sx={{
                      display: 'grid',
                      gridTemplateColumns: {
                        xs: '1fr',
                        md: image?.original?.width && image?.original?.height ? '1fr 280px' : '1fr',
                      },
                      gap: 2,
                      alignItems: 'start',
                      mt: 2,
                    }}
                  >
                    {image?.original && image.original.height && image.original.width && (
                      <Box
                        onMouseMove={handleMouseMove}
                        onMouseUp={handleMouseUp}
                        onMouseLeave={handleMouseUp}
                        sx={{ position: 'relative', width: '100%', aspectRatio: `${image.original.width} / ${image.original.height}` }}
                      >
                        <img src={image.original.url ?? image.thumbnail?.url ?? ''} alt="" style={{ display: 'block', width: '100%', height: '100%' }} />
                        {[...resourcePositions.entries()].map(([id, position]) => {
                          const resource = resources.find((item) => item.id === id);
                          if (!resource) return null;
                          return (
                            <Box
                              key={resource.id}
                              sx={{
                                position: 'absolute',
                                left: `${(position.x / image.original.width!) * 100}%`,
                                top: `${(position.y / image.original.height!) * 100}%`,
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                width: 40,
                                height: 40,
                                borderRadius: '50%',
                                border: 2,
                                borderColor: 'common.white',
                                backgroundColor: 'warning.main',
                                boxShadow: 3,
                                cursor: 'grab',
                              }}
                              onMouseDown={(event) => handleMouseDown(event, resource.id)}
                              title={resource.name}
                            >
                              {resourceIcon(resource.resourceType.type ?? '', 'common.black')}
                            </Box>
                          );
                        })}
                      </Box>
                    )}
                    <Box sx={{ border: 1, borderColor: 'divider', borderRadius: 1, overflow: 'hidden' }}>
                      <Box sx={{ px: 2, py: 1.5, borderBottom: 1, borderColor: 'divider', backgroundColor: 'action.hover' }}>
                        <BodyIconTypography label="Resources" />
                      </Box>
                      <Box sx={{ maxHeight: 480, overflowY: 'auto' }}>
                        <List dense>
                          {resources.map((item) => (
                            <ListItem
                              key={item.id}
                              secondaryAction={<Checkbox edge="end" checked={resourcePositions.has(item.id)} onChange={() => handleToggleResourcePosition(item.id)} />}
                              disablePadding
                            >
                              <ListItemButton>
                                <ListItemAvatar>{resourceIcon(item.resourceType.type ?? '', item.color ?? '')}</ListItemAvatar>
                                <ListItemText primary={item.name} />
                              </ListItemButton>
                            </ListItem>
                          ))}
                        </List>
                      </Box>
                    </Box>
                  </Box>
                </SettingsSectionCard>

                <EditorActionBar
                  primaryAction={addLabel ?? 'Add floor plan'}
                  secondaryActions={
                    <>
                      {showDismiss && (
                        <Button variant="outlined" onClick={handleCloseClick} sx={{ textTransform: 'none' }}>
                          Dismiss
                        </Button>
                      )}
                      <Button variant="outlined" onClick={handleCloseClick} sx={{ textTransform: 'none' }}>
                        Cancel
                      </Button>
                    </>
                  }
                />
              </StackColumn>
            </FormStackColumn>
          )}
        />
      </Box>
    </Box>
  );
};

const MemoAddFloorPlanInner = memo(AddFloorPlanInner);

type Props = {
  locationId: string;
  onAdded: (id: string) => void;
  onCancel: () => void;
  onReloadRequired: () => void;
  addLabel?: string;
  showDismiss: boolean;
};

const AddFloorPlanWithRelay = ({ locationId, onAdded, onCancel, onReloadRequired, addLabel, showDismiss }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<addFloorPlan_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery({ locationId, floorPlanId: uuid(), resourcesSortingValues: [{ direction: 'ASCENDING', field: 'NAME' }] }, { fetchPolicy: 'store-and-network' });
  }, [loadQuery, triggerReloadId, locationId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
      onReloadRequired();
    });
  };

  if (!queryReference) return <CircularProgress />;

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoAddFloorPlanInner
        queryReference={queryReference}
        locationId={locationId}
        onAdded={onAdded}
        onCancel={onCancel}
        onReloadRequired={handleReloadRequired}
        addLabel={addLabel}
        showDismiss={showDismiss}
      />
    </ErrorBoundary>
  );
};

export default memo(AddFloorPlanWithRelay);

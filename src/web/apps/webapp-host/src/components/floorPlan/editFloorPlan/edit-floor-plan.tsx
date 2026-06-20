'use client';

import type { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import type { editFloorPlan_query$key } from '@/queries/__generated__/editFloorPlan_query.graphql';
import type { editFloorPlan_resources_query$key } from '@/queries/__generated__/editFloorPlan_resources_query.graphql';
import type { editFloorPlan_resources_refetchableFragment } from '@/queries/__generated__/editFloorPlan_resources_refetchableFragment.graphql';
import type { editFloorPlan_rootQuery } from '@/queries/__generated__/editFloorPlan_rootQuery.graphql';
import type { editFloorPlan_updateFloorPlanMutation } from '@/queries/__generated__/editFloorPlan_updateFloorPlanMutation.graphql';
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
import { useRouter } from 'next/navigation';
import { memo, useContext, useEffect, useRef, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form, FormSpy } from 'react-final-form';
import { graphql, PreloadedQuery, useFragment, useMutation, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

const RootQuery = graphql`
  query editFloorPlan_rootQuery($locationId: String!, $floorPlanId: String!, $resourcesSortingValues: [ResourceOrderInput!]) {
    ...editFloorPlan_query
    ...editFloorPlan_resources_query
  }
`;

const FloorPlanFragment = graphql`
  fragment editFloorPlan_query on Query {
    floorPlan(id: $floorPlanId) {
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
    deskResourceType
    roomResourceType
    parkingResourceType
  }
`;

const ResourcesFragment = graphql`
  fragment editFloorPlan_resources_query on Query
  @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
  @refetchable(queryName: "editFloorPlan_resources_refetchableFragment") {
    location(id: $locationId) {
      resources(first: $count, after: $cursor, where: { floorPlanId: $floorPlanId }, orderBy: $resourcesSortingValues) @connection(key: "editFloorPlanResourcesQuery_resources") {
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

const UpdateMutation = graphql`
  mutation editFloorPlan_updateFloorPlanMutation($input: UpdateFloorPlanInput!) @raw_response_type {
    updateFloorPlan(input: $input) {
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

type FloorPlanPatchField = 'IMAGE' | 'NAME' | 'RESOURCE_POSITIONS';

type EditFloorPlanSavePatch = {
  id: string;
  fieldsToUpdate: FloorPlanPatchField[];
  name: string;
  image: {
    original: { url: string; height: number; width: number };
    thumbnail: { url: string; height: number; width: number };
  };
  resourcePositions: { resourceId: string; x: number; y: number }[];
};

type FloorPlanData = {
  id: string;
  name: string | null | undefined;
  image:
    | {
        original: { url: string | null; height: number | null; width: number | null };
        thumbnail: { url: string | null; height: number | null; width: number | null } | null;
      }
    | null
    | undefined;
  resourcePositions: { x: number; y: number; resource: { id: string } }[];
};

type FloorPlanDetails = { name: string };

const floorPlanSchema = object({
  name: string().min(3, 'Floor plan name must be at least three characters long.').required('Floor plan name is required'),
});

const floorPlanAutosaveDebounceTimeout = 1000;

type InnerProps = {
  queryReference: PreloadedQuery<editFloorPlan_rootQuery>;
  onReloadRequired: () => void;
};

const EditFloorPlanInner = ({ queryReference }: InnerProps) => {
  const router = useRouter();
  const rootData = usePreloadedQuery<editFloorPlan_rootQuery>(RootQuery, queryReference);
  const floorPlanData = useFragment<editFloorPlan_query$key>(FloorPlanFragment, rootData);
  const [resourcesData] = useRefetchableFragment<editFloorPlan_resources_refetchableFragment, editFloorPlan_resources_query$key>(ResourcesFragment, rootData);
  const [commitUpdateFloorPlan] = useMutation<editFloorPlan_updateFloorPlanMutation>(UpdateMutation);
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateFloorPlanDetails = makeValidate(floorPlanSchema);
  const requiredFields = makeRequired(floorPlanSchema);
  const resources: ResourceNode[] = (resourcesData.location?.resources?.edges?.map(({ node }) => node) ?? []) as ResourceNode[];
  const floorPlan: FloorPlanData = floorPlanData.floorPlan as FloorPlanData;

  const [name, setName] = useState(floorPlan?.name);
  const [image, setImage] = useState<FileUploadResponse | null>(
    floorPlan?.image?.original
      ? {
          id: '',
          original: {
            url: floorPlan.image.original.url ?? '',
            height: floorPlan.image.original.height ?? 0,
            width: floorPlan.image.original.width ?? 0,
          },
          thumbnail: floorPlan.image.thumbnail
            ? { url: floorPlan.image.thumbnail.url ?? '', height: floorPlan.image.thumbnail.height ?? 0, width: floorPlan.image.thumbnail.width ?? 0 }
            : null,
        }
      : null,
  );
  const [resourcePositions, setResourcePositions] = useState<Map<string, { x: number; y: number }>>(() =>
    (floorPlan?.resourcePositions ?? []).reduce((acc, { x, y, resource }) => acc.set(resource.id, { x, y }), new Map<string, { x: number; y: number }>()),
  );
  const [draggingResourceId, setDraggingResourceId] = useState<string | null>(null);
  const [offset, setOffset] = useState({ x: 0, y: 0 });
  const previousFloorPlanName = useRef<string | null | undefined>(null);
  const previousFloorPlanImage = useRef<FileUploadResponse | null>(image);
  const previousFloorPlanResourcePositions = useRef<Map<string, { x: number; y: number }>>(resourcePositions);

  const handleFloorPlanDetailUpdate = (fieldsToUpdate: FloorPlanPatchField[], { name: updatedName }: FloorPlanDetails) => {
    if (!floorPlanSchema.isValidSync({ name: updatedName })) return;

    if (!image?.original.url || !image.original.height || !image.original.width || !image.thumbnail?.url || !image.thumbnail.height || !image.thumbnail.width) {
      themedToast(<NotificationContent content="Floor plan image is required." />, errorNotificationOptions);
      return;
    }

    const patch: EditFloorPlanSavePatch = {
      id: floorPlan.id,
      fieldsToUpdate,
      name: updatedName,
      image: {
        original: { url: image.original.url, height: image.original.height, width: image.original.width },
        thumbnail: { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width },
      },
      resourcePositions: [...resourcePositions.entries()].map(([resourceId, { x, y }]) => ({ resourceId, x, y })),
    };

    commitUpdateFloorPlan({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: patch.id,
          fieldsToUpdate: patch.fieldsToUpdate,
          name: patch.name,
          image: patch.image,
          resourcePositions: patch.resourcePositions,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to update floor plan '${floorPlan.name}'. Error: ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to update floor plan '${floorPlan.name}'. Error: ${error.message}.`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        updateFloorPlan: {
          floorPlan: {
            id: patch.id,
            name: patch.name,
            image: patch.image,
            resourcePositions: patch.resourcePositions.map(({ resourceId, x, y }) => ({ id: '', resource: { id: resourceId }, x, y })),
          },
        },
      },
    });
  };

  const debouncedFloorPlanDetailUpdate = useDebounceCallback(handleFloorPlanDetailUpdate, floorPlanAutosaveDebounceTimeout);

  useEffect(() => {
    if (previousFloorPlanName.current === null) {
      previousFloorPlanName.current = name;
      previousFloorPlanImage.current = image;
      previousFloorPlanResourcePositions.current = resourcePositions;
      return;
    }
    if (!name) return;
    if (draggingResourceId !== null) return;
    const changedFields: FloorPlanPatchField[] = [];
    if (previousFloorPlanName.current !== name) changedFields.push('NAME');
    if (JSON.stringify(previousFloorPlanImage.current) !== JSON.stringify(image)) changedFields.push('IMAGE');
    if (JSON.stringify([...previousFloorPlanResourcePositions.current.entries()]) !== JSON.stringify([...resourcePositions.entries()])) changedFields.push('RESOURCE_POSITIONS');
    if (changedFields.length > 0) {
      previousFloorPlanName.current = name;
      previousFloorPlanImage.current = image;
      previousFloorPlanResourcePositions.current = resourcePositions;
      debouncedFloorPlanDetailUpdate(changedFields, { name });
    }
  }, [name, image, resourcePositions, draggingResourceId, debouncedFloorPlanDetailUpdate]);

  const handleCloseClick = () => router.back();

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
    if (type === floorPlanData.deskResourceType) return <DeskIcon sx={{ color }} />;
    if (type === floorPlanData.roomResourceType) return <MeetingRoomIcon sx={{ color }} />;
    if (type === floorPlanData.parkingResourceType) return <LocalParkingIcon sx={{ color }} />;
    return <NotListedLocationIcon sx={{ color }} />;
  };

  if (!floorPlan) return null;

  return (
    <Box sx={{ px: { xs: 2, md: 3 }, py: 3 }}>
      <Box sx={{ maxWidth: 1320, mx: 'auto' }}>
        <Form
          onSubmit={() => undefined}
          initialValues={{ name: name ?? undefined }}
          validate={validateFloorPlanDetails}
          render={({ handleSubmit }) => (
            <FormStackColumn onSubmit={handleSubmit}>
              <FormSpy subscription={{ values: true }} onChange={({ values }) => setName(values.name)} />
              <StackColumn>
                <PageHeaderPanel title="Edit Floor Plan" />

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
                  secondaryActions={
                    <Button variant="outlined" onClick={handleCloseClick} sx={{ textTransform: 'none' }}>
                      Close
                    </Button>
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

const MemoEditFloorPlanInner = memo(EditFloorPlanInner);

type Props = {
  locationId: string;
  floorPlanId: string;
  onReloadRequired: () => void;
};

const EditFloorPlanWithRelay = ({ locationId, floorPlanId, onReloadRequired }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<editFloorPlan_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery({ locationId, floorPlanId, resourcesSortingValues: [{ direction: 'ASCENDING', field: 'NAME' }] }, { fetchPolicy: 'store-and-network' });
  }, [loadQuery, triggerReloadId, locationId, floorPlanId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
      onReloadRequired();
    });
  };

  if (!queryReference) return <CircularProgress />;

  return (
    <ErrorBoundary fallbackRender={({ error: relayError }) => <RelayError error={toRootError(relayError)} />}>
      <MemoEditFloorPlanInner queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(EditFloorPlanWithRelay);

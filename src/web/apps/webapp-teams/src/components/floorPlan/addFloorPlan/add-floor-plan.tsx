import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { DeskIcon, OtherResourceIcon, ParkingIcon, RoomIcon } from '@/components/icons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { RelayError, toRootError } from '@/components/relayError';
import { ImageFileUploader } from '@/libs/image-file-uploader';
import type { addFloorPlan_addFloorPlanMutation } from '@/queries/__generated__/addFloorPlan_addFloorPlanMutation.graphql';
import type { addFloorPlan_resources_query$key } from '@/queries/__generated__/addFloorPlan_resources_query.graphql';
import type { addFloorPlan_resources_refetchableFragment } from '@/queries/__generated__/addFloorPlan_resources_refetchableFragment.graphql';
import type { addFloorPlan_rootQuery } from '@/queries/__generated__/addFloorPlan_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Checkbox from '@mui/material/Checkbox';
import Divider from '@mui/material/Divider';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemAvatar from '@mui/material/ListItemAvatar';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText';
import { getRelayErrorMessage, PaletteModeContext } from '@skedular/shared';
import {
  AppBarWithStackColumn,
  BodyIconTypography,
  defaultButtonStyle,
  defaultPadding,
  FormFieldLabel,
  FormStackColumn,
  SectionIconTypography,
  StackColumn,
  StackRow,
} from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useEffect, useMemo, useState, useTransition } from 'react';
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

type Props = {
  queryReference: PreloadedQuery<addFloorPlan_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  locationId: string;
  onAdded: (locationId: string) => void;
  onCancel: () => void;
  addLabel?: string;
  showDismiss: boolean;
};

type FloorPlanDetails = {
  name: string;
};

const floorPlanSchema = object({
  name: string().min(3, 'Floor plan name must be at least three characters long.').required('Floor plan name is required'),
});

const AddFloorPlan = ({ queryReference, onReloadRequired, locationId, onAdded, onCancel, addLabel, showDismiss }: Props) => {
  const rootData = usePreloadedQuery<addFloorPlan_rootQuery>(RootQuery, queryReference);
  const [rootDataResources] = useRefetchableFragment<addFloorPlan_resources_refetchableFragment, addFloorPlan_resources_query$key>(
    graphql`
      fragment addFloorPlan_resources_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "addFloorPlan_resources_refetchableFragment") {
        location(id: $locationId) {
          resources(first: $count, after: $cursor, where: { floorPlanId: $floorPlanId }, orderBy: $resourcesSortingValues)
            @connection(key: "addFloorPlanResourcesQuery_resources") {
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
    `,
    rootData,
  );

  const [commitAddFloorPlan] = useMutation<addFloorPlan_addFloorPlanMutation>(graphql`
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
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateFloorPlanDetails = makeValidate(floorPlanSchema);
  const requiredFields = makeRequired(floorPlanSchema);
  const [image, setImage] = useState<FileUploadResponse>();
  const resources = useMemo(() => (rootDataResources.location ? rootDataResources.location.resources.edges.map(({ node }) => node) : []), [rootDataResources.location]);
  const [resourcePositions, setResourcePositions] = useState<Map<string, { x: number; y: number }>>(new Map<string, { x: number; y: number }>());
  const [draggingResourceId, setDraggingResourceId] = useState<string | null>(null);
  const [offset, setOffset] = useState({ x: 0, y: 0 });

  const handleCloseClick = () => {
    onCancel();
    onReloadRequired();
  };

  const handleFloorPlanAddClick = ({ name }: FloorPlanDetails) => {
    const id = uuid();

    if (
      !image ||
      !image.original.url ||
      !image.original.height ||
      !image.original.width ||
      !image.thumbnail ||
      !image.thumbnail.url ||
      !image.thumbnail.height ||
      !image.thumbnail.width
    ) {
      themedToast(<NotificationContent content={`Floor plan image is required.`} />, errorNotificationOptions);

      return;
    }

    const finalImage = {
      original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
      thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
    };

    commitAddFloorPlan({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
          locationId,
          name,
          image: finalImage,
          resourcePositions: [...resourcePositions.entries()].map(([resourceId, { x, y }]) => ({
            resourceId,
            x,
            y,
          })),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to add new location '${name}'. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);

          return;
        }

        onAdded(id);
        onReloadRequired();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to add new location '${name}'. Error: ${error.message}.`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        addFloorPlan: {
          floorPlan: {
            id,
            name,
            image: finalImage,
            resourcePositions: [...resourcePositions.entries()].map(([id, { x, y }]) => ({
              id: '',
              resource: { id },
              x,
              y,
            })),
          },
        },
      },
    });
  };

  const handleImageUploadCompleted = (response: FileUploadResponse) => {
    setImage(response);
  };

  const handleMouseDown = (event: React.MouseEvent, resourceId: string) => {
    event.preventDefault();

    const boundingRect = (event.target as HTMLElement).getBoundingClientRect();

    setOffset({ x: event.clientX - boundingRect.left, y: event.clientY - boundingRect.top });
    setDraggingResourceId(resourceId);
  };

  const handleMouseMove = (event: React.MouseEvent<HTMLDivElement>) => {
    if (!draggingResourceId) {
      return;
    }

    const containerRect = event.currentTarget.getBoundingClientRect();
    const newX = event.clientX - containerRect.left - offset.x;
    const newY = event.clientY - containerRect.top - offset.y;

    setResourcePositions(new Map(resourcePositions).set(draggingResourceId, { x: newX, y: newY }));
  };

  const handleMouseUp = () => {
    setDraggingResourceId(null);
  };

  const handleToggleResourcePosition = (id: string) => {
    if (resourcePositions.has(id)) {
      resourcePositions.delete(id);
      setResourcePositions(new Map(resourcePositions));
    } else {
      setResourcePositions(new Map(resourcePositions).set(id, { x: 50, y: 50 }));
    }
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Add Location">
          <Form
            onSubmit={handleFloorPlanAddClick}
            initialValues={{
              name: '',
            }}
            validate={validateFloorPlanDetails}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <SectionIconTypography label="Floor Plan Setup" />
                  <BodyIconTypography label="Add your floor plan name and details" />
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <FormFieldLabel label="Name">
                    <TextField name="name" required={requiredFields.name} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Layout">
                    <StackRow sx={{ alignItems: 'top' }}>
                      <List
                        dense
                        sx={{
                          backgroundColor: (theme) => theme.palette.background.paper,
                          borderRight: 1,
                          borderColor: (theme) => theme.palette.divider,
                          paddingTop: { xs: 1, sm: 1, md: 3 },
                          height: image?.original.height ? image.original.height : 300,
                          overflowY: 'auto',
                        }}
                      >
                        {resources.map((item) => (
                          <ListItem
                            key={item.id}
                            secondaryAction={<Checkbox edge="end" checked={resourcePositions.has(item.id)} onChange={() => handleToggleResourcePosition(item.id)} />}
                            disablePadding
                          >
                            <ListItemButton>
                              <ListItemAvatar>
                                {item.resourceType.type === rootData.deskResourceType ? (
                                  <DeskIcon sx={{ color: item.color }} />
                                ) : item.resourceType.type === rootData.roomResourceType ? (
                                  <RoomIcon sx={{ color: item.color }} />
                                ) : item.resourceType.type === rootData.parkingResourceType ? (
                                  <ParkingIcon sx={{ color: item.color }} />
                                ) : (
                                  <OtherResourceIcon sx={{ color: item.color }} />
                                )}
                              </ListItemAvatar>
                              <ListItemText primary={item.name} />
                            </ListItemButton>
                          </ListItem>
                        ))}
                      </List>
                      <StackColumn>
                        {image?.original && image.original.height && image.original.width && (
                          <Box
                            onMouseMove={handleMouseMove}
                            onMouseUp={handleMouseUp}
                            onMouseLeave={handleMouseUp}
                            sx={{
                              position: 'relative',
                              display: 'inline-block',
                              width: image.original.width,
                              height: image.original.height,
                            }}
                          >
                            {/* eslint-disable-next-line @next/next/no-img-element */}
                            <img src={image.original?.url ?? image.thumbnail?.url ?? ''} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />

                            {[...resourcePositions.entries()].map(([id, position]) => {
                              const resource = resources.find((item) => item.id === id);
                              if (!resource) {
                                return null;
                              }

                              return (
                                <Box
                                  key={resource.id}
                                  sx={{
                                    position: 'absolute',
                                    left: position.x,
                                    top: position.y,
                                    color: resource.color,
                                    display: 'flex',
                                    alignItems: 'center',
                                    justifyContent: 'center',
                                    width: 40,
                                    height: 40,
                                    borderRadius: '50%',
                                    border: 2,
                                    backgroundColor: (theme) => theme.palette.background.paper,
                                    boxShadow: 1,
                                  }}
                                  onMouseDown={(event) => handleMouseDown(event, resource.id)}
                                  title={resource.name}
                                >
                                  {resource.resourceType.type === rootData.deskResourceType ? (
                                    <DeskIcon sx={{ color: resource.color }} />
                                  ) : resource.resourceType.type === rootData.roomResourceType ? (
                                    <RoomIcon sx={{ color: resource.color }} />
                                  ) : resource.resourceType.type === rootData.parkingResourceType ? (
                                    <ParkingIcon sx={{ color: resource.color }} />
                                  ) : (
                                    <OtherResourceIcon sx={{ color: resource.color }} />
                                  )}
                                </Box>
                              );
                            })}
                          </Box>
                        )}
                        <ImageFileUploader onUploadCompleted={handleImageUploadCompleted} />
                      </StackColumn>
                    </StackRow>
                  </FormFieldLabel>
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <StackRow>
                    {showDismiss && (
                      <Button variant="contained" sx={defaultButtonStyle} onClick={handleCloseClick}>
                        <BodyIconTypography label="Dismiss" invertDefaultColor={paletteMode === 'dark'} />
                      </Button>
                    )}
                    <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                      <BodyIconTypography label={addLabel ?? 'Add'} invertDefaultColor={paletteMode === 'dark'} />
                    </Button>
                  </StackRow>
                </StackColumn>
              </FormStackColumn>
            )}
          />
        </AppBarWithStackColumn>
      </Box>
    </Box>
  );
};

const MemoAddFloorPlan = memo(AddFloorPlan);

type RelayProps = {
  onReloadRequired: () => void;
  locationId: string;
  onAdded: (locationId: string) => void;
  onCancel: () => void;
  addLabel?: string;
  showDismiss: boolean;
};

const AddFloorPlanWithRelay = ({ onReloadRequired, locationId, onAdded, onCancel, addLabel, showDismiss }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addFloorPlan_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        locationId,
        floorPlanId: uuid(),
        resourcesSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, locationId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
      onReloadRequired();
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoAddFloorPlan
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        locationId={locationId}
        onAdded={onAdded}
        onCancel={onCancel}
        addLabel={addLabel}
        showDismiss={showDismiss}
      />
    </ErrorBoundary>
  );
};

export default memo(AddFloorPlanWithRelay);

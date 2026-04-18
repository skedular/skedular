import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, SectionIconTypography, StackColumn, StackRow } from '@/components/commons';
import { DeskIcon, OtherResourceIcon, ParkingIcon, RoomIcon } from '@/components/icons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { ImageFileUploader } from '@/libs/image-file-uploader';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { getRelayErrorMessage } from '@/libs/utils';
import type { editFloorPlan_query$key } from '@/queries/__generated__/editFloorPlan_query.graphql';
import type { editFloorPlan_resources_query$key } from '@/queries/__generated__/editFloorPlan_resources_query.graphql';
import type { editFloorPlan_resources_refetchableFragment } from '@/queries/__generated__/editFloorPlan_resources_refetchableFragment.graphql';
import type { editFloorPlan_updateFloorPlanMutation } from '@/queries/__generated__/editFloorPlan_updateFloorPlanMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Checkbox from '@mui/material/Checkbox';
import Divider from '@mui/material/Divider';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemAvatar from '@mui/material/ListItemAvatar';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { useRouter } from 'next/navigation';
import { memo, useContext, useMemo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

type Props = {
  rootDataRelay: editFloorPlan_query$key;
  rootDataResourcesRelay: editFloorPlan_resources_query$key;
  onReloadRequired: () => void;
};

type FloorPlanDetails = {
  name: string;
};

const floorPlanSchema = object({
  name: string().min(3, 'Floor plan name must be at least three characters long.').required('Floor plan name is required'),
});

const EditFloorPlan = ({ rootDataRelay, rootDataResourcesRelay }: Props) => {
  const rootData = useFragment<editFloorPlan_query$key>(
    graphql`
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
    `,
    rootDataRelay,
  );

  const [rootDataResources] = useRefetchableFragment<editFloorPlan_resources_refetchableFragment, editFloorPlan_resources_query$key>(
    graphql`
      fragment editFloorPlan_resources_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "editFloorPlan_resources_refetchableFragment") {
        location(id: $locationId) {
          resources(first: $count, after: $cursor, where: { floorPlanId: $floorPlanId }, orderBy: $resourcesSortingValues)
            @connection(key: "editFloorPlanResourcesQuery_resources") {
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
    `,
    rootDataResourcesRelay,
  );

  const [commitUpdateFloorPlan] = useMutation<editFloorPlan_updateFloorPlanMutation>(graphql`
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
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateFloorPlanDetails = makeValidate(floorPlanSchema);
  const requiredFields = makeRequired(floorPlanSchema);
  const [name, setName] = useState(rootData.floorPlan?.name);
  const [image, setImage] = useState<FileUploadResponse | null>(
    rootData.floorPlan?.image && rootData.floorPlan?.image.original
      ? {
          id: '',
          original: {
            url: rootData.floorPlan?.image.original.url,
            height: rootData.floorPlan?.image.original.height,
            width: rootData.floorPlan?.image.original.width,
          },
          thumbnail: rootData.floorPlan?.image.thumbnail
            ? {
                url: rootData.floorPlan?.image.thumbnail.url,
                height: rootData.floorPlan?.image.thumbnail.height,
                width: rootData.floorPlan?.image.thumbnail.width,
              }
            : null,
        }
      : null,
  );
  const resources = useMemo(() => (rootDataResources.location ? rootDataResources.location.resources.edges.map(({ node }) => node) : []), [rootDataResources.location]);
  const [resourcePositions, setResourcePositions] = useState<Map<string, { x: number; y: number }>>(() =>
    (rootData.floorPlan?.resourcePositions ? rootData.floorPlan.resourcePositions : []).reduce(
      (acc, { x, y, resource }) => acc.set(resource.id, { x, y }),
      new Map<string, { x: number; y: number }>(),
    ),
  );
  const [draggingResourceId, setDraggingResourceId] = useState<string | null>(null);
  const [offset, setOffset] = useState({ x: 0, y: 0 });

  const handleFloorPlanDetailUpdateClick = ({ name }: FloorPlanDetails) => {
    const floorPlan = rootData.floorPlan;
    if (!floorPlan) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating floor plan '${floorPlan.name}'...`} />, infoNotificationOptions);

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
      toast.update(toastId, {
        ...errorNotificationOptions,
        render: <NotificationContent content={`Floor plan image is required.`} />,
      });

      return;
    }
    const finalImage = {
      original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
      thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
    };

    commitUpdateFloorPlan({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: floorPlan.id,
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
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update floor plan '${floorPlan.name}'. Error: ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Floor plan ${name} updated.`} />,
        });

        router.back();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update floor plan '${floorPlan.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateFloorPlan: {
          floorPlan: {
            id: floorPlan.id,
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

  const handleCloseClick = () => {
    router.back();
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

  const floorPlan = rootData.floorPlan;
  if (!floorPlan) {
    return null;
  }

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Floor Plan">
          <Form
            onSubmit={handleFloorPlanDetailUpdateClick}
            initialValues={{
              name,
            }}
            validate={validateFloorPlanDetails}
            render={({ handleSubmit, values }) => {
              setName(values!.name);

              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <SectionIconTypography label="Floor Plan Setup" />
                    <BodyIconTypography label="Edit your floor plan name and details" />
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
                      <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                        Update
                      </Button>
                    </StackRow>
                  </StackColumn>
                </FormStackColumn>
              );
            }}
          />
        </AppBarWithStackColumn>
      </Box>
    </Box>
  );
};

export default memo(EditFloorPlan);

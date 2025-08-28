import {
  AppBarWithStackColumn,
  BodyIconTypography,
  ColorPicker,
  FormFieldLabel,
  FormStackColumn,
  GridContainer,
  SectionIconTypography,
  StackColumn,
  StackRow,
} from '@/components/commons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { MultipleChoicesCustomTags, MultipleChoicesProductTags, MultipleChoicesZones, SingleChoiceResourceType } from '@/components/organization';
import { WeekOpeningHours, WeekOpeningHoursDetails } from '@/components/weekOpeningHours';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { editResource_query$key } from '@/queries/__generated__/editResource_query.graphql';
import type { editResource_updateLocationResourceAvailableHoursMutation } from '@/queries/__generated__/editResource_updateLocationResourceAvailableHoursMutation.graphql';
import type { editResource_updateResourceMutation } from '@/queries/__generated__/editResource_updateResourceMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import Switch from '@mui/material/Switch';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { array, number, object, string } from 'yup';

type Props = {
  rootDataRelay: editResource_query$key;
  onReloadRequired?: () => void;
  organizationUniqueAlphanumericName: string;
};

type ResourceDetails = {
  name: string;
  resourceTypeId: string;
  customTagIds: string[];
  zoneIds: string[];
  productTagIds: string[];
  capacity: number;
};

const ResourceSchema = object({
  resourceTypeId: string().required('Resource type is required'),
  name: string().required('Resource name is required'),
  customTagIds: array().nullable(),
  zoneIds: array().nullable(),
  productTagIds: array().nullable(),
  capacity: number().required('Capacity is required').min(1, 'Capacity must be greater than 0'),
});

const EditResource = ({ rootDataRelay, organizationUniqueAlphanumericName }: Props) => {
  const rootData = useFragment<editResource_query$key>(
    graphql`
      fragment editResource_query on Query {
        organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
          type {
            type
          }
        }
        location(id: $locationId) {
          openingHours {
            weekOpeningHours {
              monday {
                closed
                openAllDay
                from
                until
              }
              tuesday {
                closed
                openAllDay
                from
                until
              }
              wednesday {
                closed
                openAllDay
                from
                until
              }
              thursday {
                closed
                openAllDay
                from
                until
              }
              friday {
                closed
                openAllDay
                from
                until
              }
              saturday {
                closed
                openAllDay
                from
                until
              }
              sunday {
                closed
                openAllDay
                from
                until
              }
            }
          }
        }
        resource(id: $resourceId) {
          id
          name
          inactive
          requireBookingApproval
          color
          capacity
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
          productTags {
            uniqueId
            name
            color
          }
          resourceType {
            uniqueId
            name
            color
          }
          isAvailableHoursOverridden
          availableHours {
            weekOpeningHours {
              monday {
                closed
                openAllDay
                from
                until
              }
              tuesday {
                closed
                openAllDay
                from
                until
              }
              wednesday {
                closed
                openAllDay
                from
                until
              }
              thursday {
                closed
                openAllDay
                from
                until
              }
              friday {
                closed
                openAllDay
                from
                until
              }
              saturday {
                closed
                openAllDay
                from
                until
              }
              sunday {
                closed
                openAllDay
                from
                until
              }
            }
          }
        }
        ...singleChoiceResourceType_query
        ...multipleChoicesCustomTags_query
        ...multipleChoicesZones_query
        ...multipleChoicesProductTags_query
        ...weekOpeningHours_query
      }
    `,
    rootDataRelay,
  );

  const [commitUpdateResource] = useMutation<editResource_updateResourceMutation>(graphql`
    mutation editResource_updateResourceMutation($input: UpdateResourceInput!) @raw_response_type {
      updateResource(input: $input) {
        resource {
          id
          name
          inactive
          requireBookingApproval
          color
          capacity
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
          productTags {
            uniqueId
            name
            color
          }
          resourceType {
            uniqueId
            name
            color
          }
          isAvailableHoursOverridden
          availableHours {
            weekOpeningHours {
              monday {
                closed
                openAllDay
                from
                until
              }
              tuesday {
                closed
                openAllDay
                from
                until
              }
              wednesday {
                closed
                openAllDay
                from
                until
              }
              thursday {
                closed
                openAllDay
                from
                until
              }
              friday {
                closed
                openAllDay
                from
                until
              }
              saturday {
                closed
                openAllDay
                from
                until
              }
              sunday {
                closed
                openAllDay
                from
                until
              }
            }
          }
        }
      }
    }
  `);

  const [commitUpdateLocationResourceAvailableHours] = useMutation<editResource_updateLocationResourceAvailableHoursMutation>(graphql`
    mutation editResource_updateLocationResourceAvailableHoursMutation($input: UpdateLocationResourceAvailableHoursInput!) @raw_response_type {
      updateLocationResourceAvailableHours(input: $input) {
        resource {
          id
          name
          inactive
          requireBookingApproval
          color
          capacity
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
          productTags {
            uniqueId
            name
            color
          }
          resourceType {
            uniqueId
            name
            color
          }
          isAvailableHoursOverridden
          availableHours {
            weekOpeningHours {
              monday {
                closed
                openAllDay
                from
                until
              }
              tuesday {
                closed
                openAllDay
                from
                until
              }
              wednesday {
                closed
                openAllDay
                from
                until
              }
              thursday {
                closed
                openAllDay
                from
                until
              }
              friday {
                closed
                openAllDay
                from
                until
              }
              saturday {
                closed
                openAllDay
                from
                until
              }
              sunday {
                closed
                openAllDay
                from
                until
              }
            }
          }
        }
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateResourceDetails = makeValidate(ResourceSchema);
  const requiredFields = makeRequired(ResourceSchema);
  const [selectedColor, setSelectedColor] = useState(rootData.resource?.color);
  const [isAvailableHoursOverridden, setIsAvailableHoursOverridden] = useState(rootData.resource ? rootData.resource.isAvailableHoursOverridden : false);

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleCloseClick = () => {
    router.back();
  };

  const handleResourceDetailUpdateClick = ({ resourceTypeId, name, customTagIds, zoneIds, productTagIds, capacity: capacityStr }: ResourceDetails) => {
    const resource = rootData.resource;
    if (!resource) {
      return;
    }

    const oldName = resource.name;
    const toastId = themedToast(<NotificationContent content={`Updating zone '${oldName}'...`} />, infoNotificationOptions);
    const capacity = parseInt(capacityStr.toString(), 10);

    commitUpdateResource({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: resource.id,
          name,
          inactive: resource.inactive,
          requireBookingApproval: resource.requireBookingApproval,
          customTagIds,
          zoneIds,
          productTagIds,
          color: selectedColor,
          capacity,
          organizationResourceTypeId: resourceTypeId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update Resource '${oldName}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Resource ${name} updated.`} />,
        });

        router.back();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update Resource '${oldName}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateResource: {
          resource: {
            id: resource.id,
            name,
            inactive: resource.inactive,
            requireBookingApproval: resource.requireBookingApproval,
            customTags: [],
            zones: [],
            productTags: [],
            color: selectedColor,
            capacity,
            resourceType: {
              uniqueId: resourceTypeId,
              name: '',
              color: '',
            },
            isAvailableHoursOverridden: resource.isAvailableHoursOverridden,
            availableHours: resource.availableHours,
          },
        },
      },
    });
  };

  const handleResourceAvailableHoursUpdateClick = (weekOpeningHours: WeekOpeningHoursDetails) => {
    const resource = rootData.resource;
    if (!resource) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating resource '${resource.name}' available hours...`} />, infoNotificationOptions);

    commitUpdateLocationResourceAvailableHours({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: resource.id,
          overrideAvailableHours: isAvailableHoursOverridden,
          availableHours: isAvailableHoursOverridden ? weekOpeningHours : null,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update resource '${resource?.name}' available hours . Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Resource ${resource.name} available hours updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update resource '${resource?.name}' available hours. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateLocationResourceAvailableHours: {
          resource: {
            id: resource.id,
            name: resource.name,
            inactive: resource.inactive,
            requireBookingApproval: resource.requireBookingApproval,
            customTags: resource.customTags,
            zones: resource.zones,
            productTags: resource.productTags,
            color: resource.color,
            capacity: resource.capacity,
            resourceType: resource.resourceType,
            isAvailableHoursOverridden,
            availableHours: isAvailableHoursOverridden ? { weekOpeningHours: weekOpeningHours } : null,
          },
        },
      },
    });
  };

  const handleIsAvailableHoursOverriddenChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setIsAvailableHoursOverridden(event.target.checked);

    if (event.target.checked) {
      return;
    }

    const resource = rootData.resource;
    if (!resource) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating resource '${resource.name}' available hours...`} />, infoNotificationOptions);

    commitUpdateLocationResourceAvailableHours({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: resource.id,
          overrideAvailableHours: false,
          availableHours: null,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update resource '${resource?.name}' available hours . Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Resource ${resource.name} available hours updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update resource '${resource?.name}' available hours. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateLocationResourceAvailableHours: {
          resource: {
            id: resource.id,
            name: resource.name,
            inactive: resource.inactive,
            requireBookingApproval: resource.requireBookingApproval,
            customTags: resource.customTags,
            zones: resource.zones,
            productTags: resource.productTags,
            color: resource.color,
            capacity: resource.capacity,
            resourceType: resource.resourceType,
            isAvailableHoursOverridden: false,
            availableHours: null,
          },
        },
      },
    });
  };

  if (!rootData.location || !rootData.resource) {
    return <></>;
  }

  const location = rootData.location;
  const resource = rootData.resource;

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Resource Information">
          <Form
            onSubmit={handleResourceDetailUpdateClick}
            initialValues={{
              name: resource.name,
              resourceTypeId: resource.resourceType.uniqueId,
              customTagIds: resource.customTags.map(({ uniqueId }) => uniqueId),
              zoneIds: resource.zones.map(({ uniqueId }) => uniqueId),
              productTagIds: resource.productTags.map(({ uniqueId }) => uniqueId),
              capacity: resource.capacity,
            }}
            validate={validateResourceDetails}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <SectionIconTypography label="Resource Setup" />
                  <BodyIconTypography label="Edit your resource name and details" />
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <FormFieldLabel label="Resource Type">
                    <SingleChoiceResourceType rootDataRelay={rootData} name="resourceTypeId" required={requiredFields.resourceTypeId} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Name">
                    <TextField name="name" required={requiredFields.name} helperText="Add your resource name" />
                  </FormFieldLabel>

                  <FormFieldLabel label="Tags">
                    <MultipleChoicesCustomTags
                      rootDataRelay={rootData}
                      name="customTagIds"
                      required={requiredFields.customTagIds}
                      organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
                    />
                  </FormFieldLabel>

                  <FormFieldLabel label="Zones">
                    <MultipleChoicesZones
                      rootDataRelay={rootData}
                      name="zoneIds"
                      required={requiredFields.zoneIds}
                      organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
                    />
                  </FormFieldLabel>

                  {rootData.organization?.type.type === 'MARKETPLACE' && (
                    <FormFieldLabel label="Product Tags">
                      <MultipleChoicesProductTags
                        rootDataRelay={rootData}
                        name="productTagIds"
                        required={requiredFields.productTagIds}
                        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
                      />
                    </FormFieldLabel>
                  )}

                  <FormFieldLabel label="Color">
                    <ColorPicker onChange={handleColorChange} defaultColor={rootData.resource?.color} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Capacity">
                    <TextField name="capacity" required={requiredFields.capacity} />
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
            )}
          />

          <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
            <GridContainer sx={{ justifyContent: 'space-between' }}>
              <Grid>
                <SectionIconTypography label="Opening Hours" />
                <BodyIconTypography label="Manage your resource opening hours" />
              </Grid>
            </GridContainer>
            <Divider />
          </StackColumn>

          <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
            <FormFieldLabel label="Override available hours">
              <Switch defaultChecked={isAvailableHoursOverridden} onChange={handleIsAvailableHoursOverriddenChange} />
            </FormFieldLabel>
          </StackColumn>
          {isAvailableHoursOverridden && (
            <WeekOpeningHours
              rootDataRelay={rootData}
              defaultValue={
                resource.availableHours
                  ? {
                      monday: resource.availableHours.weekOpeningHours.monday,
                      tuesday: resource.availableHours.weekOpeningHours.tuesday,
                      wednesday: resource.availableHours.weekOpeningHours.wednesday,
                      thursday: resource.availableHours.weekOpeningHours.thursday,
                      friday: resource.availableHours.weekOpeningHours.friday,
                      saturday: resource.availableHours.weekOpeningHours.saturday,
                      sunday: resource.availableHours.weekOpeningHours.sunday,
                    }
                  : location.openingHours.weekOpeningHours
              }
              onWeekOpeningHoursDetailUpdateClick={handleResourceAvailableHoursUpdateClick}
            />
          )}
        </AppBarWithStackColumn>
      </Box>
    </Box>
  );
};

export default memo(EditResource);
